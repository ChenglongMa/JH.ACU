#include <vcl.h>
#pragma hdrstop

#include "ThreadMain.h"
#include "Global.h"
#include "GpibPcPower.h"
#include "GpibPcDMM.h"
#include "GpibPcRes.h"
#include "DaqDigit.h"
#include "DaqAnalog.h"
#include "ComKline.h"
#include "ComChamber.h"
#include "FormMainU.h"

#include <math.h>

#pragma package(smart_init)

TThreadMain *g_pThreadMain;

//---------------------------------------------------------------------------

//   Important: Methods and properties of objects in VCL can only be
//   used in a method called using Synchronize, for example:
//
//      Synchronize(UpdateCaption);
//
//   where UpdateCaption could look like:
//
//      void __fastcall TThreadMain::UpdateCaption()
//      {
//        Form1->Caption = "Updated in a thread";
//      }
//---------------------------------------------------------------------------

__fastcall TThreadMain::TThreadMain(bool CreateSuspended) :
	TThread(CreateSuspended)
{
	TIniFile  *pIniFile = NULL;
	try
	{
		pIniFile = new TIniFile(g_strAppPath + "\\config\\config.ini");
        // 查找错误码精度
		g_STestInfo.m_fPrecision[0] = pIniFile->ReadFloat("Squib", "a", 0.01);
		g_STestInfo.m_fPrecision[1] = pIniFile->ReadFloat("Sbelt", "a", 0.01);
		g_STestInfo.m_fPrecision[2] = pIniFile->ReadFloat("PADS", "a", 0.01);
		g_STestInfo.m_fPrecision[3] = pIniFile->ReadFloat("batt", "a", 0.01);
        // 接触电阻
		for (int i = 0; i < 8; i++)
		{
			for (int j = 0; j < 4; j++)
			{	// 获取相关参数
				g_STestInfo.m_fAmendRes[i][j] = pIniFile->ReadFloat("Res_Wire", "Card" + IntToStr(i) + "-" + IntToStr(j), 0.2);
			}
		}
		delete pIniFile;
	}
	catch(Exception & exception)
	{
		Application->ShowException(&exception);
		delete pIniFile;
	}
}

///////////////////////////////////////////////////////////////////////////////
// U00: 线程执行函数
void __fastcall TThreadMain::Execute()
{
	if(g_iOpMode == 0)
    {
    	AutoTest();
        return;
    }

    ManualTest();
}

///////////////////////////////////////////////////////////////////////////////
// U01: 自动测试主函数
void __fastcall TThreadMain::AutoTest()
{
	// 第一步启动设备和检测
	bool			bRet = false;
	bool			bStart[9] = { false };	// 九种模式的温箱TVSheet填充TestInfo行
	unsigned int	iRepeat = 0;

	bRet = OpenDevice();
	if (false == bRet)
	{
		Synchronize(ShowTip);
        FormMain->m_pBtnInit->Enabled = false;
        FormMain->m_pBtnRun->Caption = "Run";
        this->Terminate();
		return;
	}

	// 第二步：外部条件满足，开始测试
	SetGlobalStateForNewTest();

	int iTempSeq = 0;						// 经过m_iTempSeq转换0-Low 1-Nor 2-High
	for (int iTempMode = 0; iTempMode < 3; iTempMode++)
	{
        ViewProgressInfo(false, 0);
		bool	bVoltLow = false, bVoltNor = false, bVoltHigh = false;

		// 注意有温度控制顺序
		iTempSeq = g_STestInfo.m_iTempSeq[iTempMode];
		bVoltLow = g_STestInfo.m_iTVCond[iTempSeq * 3];
		bVoltNor = g_STestInfo.m_iTVCond[iTempSeq * 3 + 1];
		bVoltHigh = g_STestInfo.m_iTVCond[iTempSeq * 3 + 2];

		if (false == bVoltLow && false == bVoltNor && false == bVoltHigh)
		{
			continue;
		}

		// 设定温度：界面-State、Target、Curr、Curve
		g_iTempPhase = iTempSeq;
		Synchronize(FormMain->ViewTempPhase);
		Synchronize(FormMain->ViewTempTarget);
		if (1 == g_STestInfo.m_iChamType) SetTempByState(g_iTempPhase);

		int iVoltMode = 0;					// 0-Low 1-Nor 2-High
		for (iVoltMode = 0; iVoltMode < 3; iVoltMode++)
		{
			bool	bVoltSel = g_STestInfo.m_iTVCond[iTempSeq * 3 + iVoltMode];
			if (false == bVoltSel)
			{
				continue;
			}

			g_iTVCond = 3 * iTempSeq + iVoltMode;

			// 设定电压 界面-State、Target、Curr
            g_iVoltPhase = iVoltMode;
			Synchronize(FormMain->ViewVoltPhase);
			Synchronize(FormMain->ViewVoltTarget);

            // 显示LabelCond
			Synchronize(FormMain->ViewTVCondInLabel);

			// 电源基本设定
			g_gpibPcPower.SetOCP(false);
			g_gpibPcPower.SetOutPut(false);
			g_gpibPcPower.SetVolt(g_fVoltTarget);
			g_gpibPcPower.SetCurr(5.0);
			g_gpibPcPower.SetOutPut(true);
            double dVoltInit = 0.0;
            g_daqAnalog.GetVoltFromChannel(2, MES_VOLT_IGNCH, dVoltInit);
            g_fVoltInit = dVoltInit;
			Synchronize(FormMain->ViewVoltCurr);
			g_iVoltCount = 1;
			g_bVoltReal = true;
            ViewProgressInfo(false, 2);

			// 填写某一个TVSheet的开始测试信息行TestInfo
			if (bStart[iTempSeq * 3 + iVoltMode] == false)
			{
				while (!g_bChamberIdle)
				{
					;
				}
				if (1 == g_STestInfo.m_iChamType)
				{
					g_bChamberIdle = false;
					g_comChamber.ReadTemp(g_fTempCurveY);
					g_bChamberIdle = true;
				}
                ViewProgressInfo(false, 10);
                FormMain->m_bTVState = true;
				Synchronize(FormMain->WriteTestInfoRowInTVSheet);
			}

			// 板卡操作
			for (int iBoard = 0; iBoard < MES_SUBBOARD_NUM; iBoard++)
			{
				int iBoardSel = (g_STestInfo.m_iSelACU >> (iBoard)) & 0x01;
				if (0x01 != iBoardSel)
				{
					continue;
				}
                ViewProgressInfo(false, 10);

				// 控制写入TempSheet中的温度时每块板卡是不一样的
				FormMain->m_bTempBoard = !FormMain->m_bTempBoard;
				g_iCurrBoard = iBoard;
                // 板卡显示
                Synchronize(FormMain->ViewBoardNum);
				// 公共继电器控制：注意时序
                g_strMainTip = "Set Base Relay and Power SelfTest:\t";
				g_daqDigit.EnableRelayBase(iBoard, true);
				//g_daqDigit.EnableRelayBase(iBoard, true);
				// 主要设备自检
				bRet = g_daqAnalog.GetPowerState();
				if (false == bRet)
				{
					g_strMark = "Please check Device:PcRES、PcDMM、PcPower(Local or Error).!";
					Synchronize(ShowTip);
				}

				// 开始测试
				// A: OpenSMode
                g_strMainTip = "OpenSMode:\t";
				iRepeat = 0;
				do
				{
					iRepeat++;
					bRet = g_comKline.OpenSMode();
					if (false == bRet)
					{
						g_strMark = g_comKline.m_strMark;
						Synchronize(FormMain->AddErrorByMark);

						// 重置设备
                        bool bTempReal = g_bTempReal;
						ResetDevice();
                        g_bTempReal = bTempReal;
					}
				}
				while (false == bRet && iRepeat < g_iItemRepeat);
				if (false == bRet)
				{
					Synchronize(ShowTip);
					return;
				}
				Synchronize(FormMain->WriteSmodeInFMSheet);
				ViewProgressInfo(false, 11);


				// B: ReadMemory WriteMemory Kline出错的可能性非常低
                g_strMainTip = "Memory Operation 1:\t";
				g_iCRC = 0;
				g_bOpState = g_comKline.ReadMemory(MES_ADDRESS, MES_BYTES, g_strMark);
				Synchronize(FormMain->WriteFMRInFMSheet);
				ViewProgressInfo(false, 13);
				g_bOpState = g_comKline.WriteMemory(MES_ADDRESS, MES_BYTES, g_strMark);
				Synchronize(FormMain->WriteFRCInFMSheet);
				ViewProgressInfo(false, 15);

				// C: TestHasFaultCode 需要故障码的测试
                g_strMainTip = "Test Has Fault Code:\t";
				bRet = NeedTestHasFault(iBoard);
				if (true == bRet) TestHasFaultCode();
				ViewProgressInfo(false, 76);

                for (int iSubItem = 1; iSubItem <= 4; iSubItem++)
				{
					TestSISSensor(iSubItem);
            		ViewProgressInfo(true, 0);
				}
        		ViewProgressInfo(false, 80);

				// D: ReadMemory WriteMemory Kline出错的可能性非常低
                g_strMainTip = "Memory Operation 2:\t";
				g_iCRC = 1;
				g_bOpState = g_comKline.ReadMemory(MES_ADDRESS, MES_BYTES, g_strMark);
				Synchronize(FormMain->WriteFMRInFMSheet);
				ViewProgressInfo(false, 82);
				g_bOpState = g_comKline.WriteMemory(MES_ADDRESS, MES_BYTES, g_strMark);
				Synchronize(FormMain->WriteFRCInFMSheet);
				ViewProgressInfo(false, 84);

				// E: WL HoldTime ACUCurr Crash
				TestWarnLamp();				// WL1,WL2测试
				ViewProgressInfo(false, 92);
                g_strMainTip = "TestHoldTime:\t";
				TestHoldTime();				// 能量保持时间
				ViewProgressInfo(false, 93);
				TestACUCurr();				// ECU电流测试
				ViewProgressInfo(false, 94);
                g_strMainTip = "TestCrashOut:\t";
				TestCrashOut();				// Crash测试
				ViewProgressInfo(false, 95);

				// F: ReadMemory WriteMemory Kline出错的可能性非常低
                g_strMainTip = "Memory Operation 3:\t";
				g_iCRC = 2;
				g_bOpState = g_comKline.ReadMemory(MES_ADDRESS, MES_BYTES, g_strMark);
				Synchronize(FormMain->WriteFMRInFMSheet);
				ViewProgressInfo(false, 97);
				g_bOpState = g_comKline.WriteMemory(MES_ADDRESS, MES_BYTES, g_strMark);
				Synchronize(FormMain->WriteFRCInFMSheet);
				ViewProgressInfo(false, 99);

				// G: CloseSMode
                g_strMainTip = "Close SMode:\t";
				iRepeat = 0;
				do
				{
					iRepeat++;
					bRet = g_comKline.CloseSMode();
					if (false == bRet)
					{
						g_strMark = g_comKline.m_strMark;
						Synchronize(FormMain->AddErrorByMark);

						// 重置设备
						bool bTempReal = g_bTempReal;
						ResetDevice();
                        g_bTempReal = bTempReal;
					}
				}
				while (false == bRet && iRepeat < g_iItemRepeat);
				if (false == bRet)
				{
					Synchronize(ShowTip);
					return;
				}

				ViewProgressInfo(false, 100);
				// 断开公共继电器
				g_daqDigit.EnableRelayBase(iBoard, false);
			}

			// 获取某一个TVSheet的结束温度并填表
			if (bStart[iTempMode * 3 + iVoltMode] == false)
			{
				while (!g_bChamberIdle)
				{
					;
				}
				g_bChamberIdle = false;
				g_comChamber.ReadTemp(g_fTempCurveY);
				g_bChamberIdle = true;
                FormMain->m_bTVState = false;
				Synchronize(FormMain->WriteTestInfoRowInTVSheet);
				bStart[iTempMode * 3 + iVoltMode] = true;
			}
		}
	}

	// 填写总表
	Synchronize(FormMain->WriteSheetTotal);
	// 测试结束后把温箱置为常温
	g_iTempPhase = 1;
	Synchronize(FormMain->ViewTempPhase);
	Synchronize(FormMain->ViewTempTarget);
	if (g_STestInfo.m_iChamType == 1)
    {
    	SetTempByState(g_iTempPhase);
        // 停止温箱
    	g_comChamber.Stop();
    }
}

///////////////////////////////////////////////////////////////////////////////
void __fastcall TThreadMain::ManualTest()
{
	// 外部设备准备好
	bool			bRet = false;
	unsigned int	iRepeat = 0;
	g_gpibPcPower.SetOCP(false);
	g_gpibPcPower.SetOutPut(false);
	g_gpibPcPower.SetCurr(5.0);
	g_gpibPcPower.SetOutPut(true);

	g_daqDigit.EnableRelayBase(g_iCurrBoard, true);

	// 开始测试
	// A: OpenSMode
	iRepeat = 0;
	do
	{
		iRepeat++;
		bRet = g_comKline.OpenSMode();
		if (false == bRet)
		{
			g_strMark = g_comKline.m_strMark;
			Synchronize(FormMain->AddErrorByMark);

			// 重置设备
			bool bTempReal = g_bTempReal;
            ResetDevice();
            g_bTempReal = bTempReal;
		}
	}
    while (false == bRet && iRepeat < g_iItemRepeat);
	if (false == bRet)
	{
		Synchronize(ShowTip);
		return;
	}

	// 需要故障码的测试
	bRet = NeedTestHasFault(g_iCurrBoard);
	if (true == bRet) TestHasFaultCode();

	TestWarnLamp();
	TestHoldTime();
	TestACUCurr();
	TestCrashOut();

	iRepeat = 0;
	do
	{
		iRepeat++;
		bRet = g_comKline.CloseSMode();
		if (false == bRet)
		{
			g_strMark = g_comKline.m_strMark;
			Synchronize(FormMain->AddErrorByMark);

			// 重置设备
			bool bTempReal = g_bTempReal;
            ResetDevice();
            g_bTempReal = bTempReal;
		}
	}
    while (false == bRet && iRepeat < g_iItemRepeat);
	if (false == bRet)
	{
		Synchronize(ShowTip);
		return;
	}

	// 断开公共继电器
	g_daqDigit.EnableRelayBase(g_iCurrBoard, false);
}

///////////////////////////////////////////////////////////////////////////////
// U03: 有错误码测试总函数
void __fastcall TThreadMain::TestHasFaultCode()
{
	bool	bRet = g_comKline.OpenRealtimeFault();
	if (false == bRet)
	{
        g_strMark = "OpenRealtimeFault Failed.";
        Synchronize(ShowTip);
        g_strMark = g_comKline.m_strMark;
        Synchronize(FormMain->AddErrorByMark);

		// 设备重置
        bool bTempReal = g_bTempReal;
        ResetDevice();
        g_bTempReal = bTempReal;
        g_comKline.OpenSMode();
        g_comKline.OpenRealtimeFault();
	}
    ViewProgressInfo(false, 16);

	int iMethod = 0, iSubItem = 1;	// iMethod based 0 iSubItem based 1
    //bool bNeed = NeedTestShortToVBatt(g_iCurrBoard);
	for (iMethod = 0; iMethod < 4; iMethod++)
	{
        /*
        if(iMethod >= 3 && bNeed)
        {
            g_strSubTip = "Open All Board Base Relay Start.";
            for(int i = 0; i < MES_SUBBOARD_NUM; i++)
            {
                g_daqDigit.EnableRelayBase(i, true);
            }
            g_strSubTip = "Open All Board Base Relay End.";
        }
        */

		for (iSubItem = 1; iSubItem <= 10; iSubItem++)
		{
			TestSquib(iSubItem, iMethod);
            ViewProgressInfo(true, 0);
		}
        ViewProgressInfo(false, (iMethod + 1)*10 + 16);
	}
    ViewProgressInfo(false, 56);

    /*
    if(iMethod >= 3 && bNeed)
    {
        g_strSubTip = "Close Other Boards Base Relay Start.";
        for(int i = 0; i < MES_SUBBOARD_NUM; i++)
        {
            if(i != g_iCurrBoard)
                g_daqDigit.EnableRelayBase(i, false);
        }
        g_strSubTip = "Close Other Boards Base Relay End.";
    }
    */

	for (iMethod = 0; iMethod < 3; iMethod++)
	{	// DSB PSB PADS
		for (iSubItem = 1; iSubItem <= 3; iSubItem++)
		{
			TestSbelt(iSubItem, iMethod);
            ViewProgressInfo(true, 0);
		}
        ViewProgressInfo(false, (iMethod + 1)*3 + 56);
	}
    ViewProgressInfo(false, 65);

	for (iMethod = 0; iMethod <= 1; iMethod++)
	{
		TestBatt(iMethod);
        ViewProgressInfo(true, 0);
	}
    ViewProgressInfo(false, 67);

	// Sensor Output
	for (iMethod = 0; iMethod < 2; iMethod++)
	{
		for (iSubItem = 1; iSubItem < 5; iSubItem++)
		{
			TestSIS(iSubItem, iMethod);
            ViewProgressInfo(true, 0);
		}
        ViewProgressInfo(false, (iMethod + 1)*4 + 67);
	}
    ViewProgressInfo(false, 75);

	bRet = g_comKline.CloseRealtimeFault();
	if (false == bRet)
	{
        g_strMark = "CloseRealtimeFault Failed.";
        Synchronize(ShowTip);
        g_strMark = g_comKline.m_strMark;


		// 设备重置
        bool bTempReal = g_bTempReal;
        ResetDevice();
        g_bTempReal = bTempReal;
        g_comKline.OpenSMode();
	}
    ViewProgressInfo(false, 76);
}


///////////////////////////////////////////////////////////////////////////////
// U04:
void __fastcall TThreadMain::TestSquib(int iSubItem, int iMethod)
{
	// A1: 判断条件是否选择
	g_iCurrItem = iSubItem + iMethod * 10 - 1;	// based 0

    unsigned char	iACUTotal = g_STestInfo.m_iItem[g_iCurrItem];
	if ((g_STestInfo.m_iSelACU >> (g_iCurrBoard)) & 0x01 != 0x01)
	{
		iSubItem = iSubItem;
		return;
	}

    unsigned char cValue = (iACUTotal >> (g_iCurrBoard));
    cValue = cValue & 0x01;
    if (cValue != 0x01)
	{
		iSubItem = iSubItem;
		return;
	}

    // A2：操作测试项
    char			iGroup = iSubItem / 2 + 1;
	char			iBit = 7 - (iSubItem - 1) % 2 * 4;
    int				iTypeRet = MES_KLINE_ZERO;
	float			fMin = 10000.0, fMax = 0.0;
	String			strTemp = "", strFull = "";
    unsigned int	iFaultCode = 0, iRepeat = 0;

    // 界面及变量变化
    g_iCurveMode = 0;
    FormMain->ResetErrorInfo(0);
    Synchronize(FormMain->ClearWave);
	Synchronize(FormMain->ChangeItem);

    // 读取测试规范
    WriteLogFile(g_pFileComKline, "\n\n\n");
    String strSpec =  GetSPECInfo(g_iCurrItem, 0) + "\t\t" + GetSPECInfo(g_iCurrItem, 1);
    strSpec =  strSpec + "\t\t" + GetSPECInfo(g_iCurrItem, 2) + "\t\t" + GetSPECInfo(g_iCurrItem, 3);
    WriteLogFile(g_pFileComKline, strSpec + "\n");

    strFull = GetSPECInfo(g_iCurrItem, 1);
	strTemp = strFull.SubString(1, strFull.Pos("~") - 1);
	fMin = strTemp.ToDouble();
	strFull = strFull.SubString(strFull.Pos("~") + 1, strFull.Length() - strFull.Pos("~"));
	fMax = strFull.ToDouble();

	iFaultCode = GetSPECInfo(g_iCurrItem, 3).ToInt();

	do
	{
    	iRepeat++;
		g_daqDigit.SetSbRelay(g_iCurrBoard, iGroup, iBit, false);
		g_daqDigit.SetSbRelay(g_iCurrBoard, iGroup, iBit - 1, false);
		g_daqDigit.SetSbRelay(g_iCurrBoard, iGroup, iBit - 2, false);
		g_daqDigit.SetSbRelay(g_iCurrBoard, iGroup, iBit - 3, false);
        g_daqDigit.SetSbRelayGroupStatus(g_iCurrBoard, iGroup, 0);
        if (iMethod > 1)
        {
        	g_daqDigit.SetSbRelay(g_iCurrBoard, 284, false);
            g_daqDigit.SetSbRelay(g_iCurrBoard, 285, false);
            g_daqDigit.SetSbRelayGroupStatus(g_iCurrBoard, 8, 0);
        }

		g_daqDigit.SetSbRelay(g_iCurrBoard, iGroup, iBit, true);
		if (iMethod > 1)
		{
			g_daqDigit.SetMbRelay(300, true);
			g_daqDigit.SetMbRelay(302 - iMethod % 2, true);
		} else
        {
        	g_daqDigit.SetSbRelay(g_iCurrBoard, iGroup, iBit - 2, true);
            int iRelayGroup = 0x01 << iBit;
            iRelayGroup += 0x01 << (iBit - 2);
            g_daqDigit.SetSbRelayGroupStatus(g_iCurrBoard, iGroup, iRelayGroup);
        }

		float fRet = 0.0;
        bool  bRet = false;
        // 进行最多3次连续查找查找
		for (unsigned int i = 0; i < g_iFindRepeat; i++)
		{
        	g_strSubTip = "Find State.";
			iTypeRet = FindSquib(fMin, fMax, iFaultCode, iMethod, fRet);
            WriteLogFile(g_pFileComKline, "inner\n");

			if (MES_KLINE_ZERO == iTypeRet)
            {
            	// 设备重置
                g_strSubTip = "Reset Device to Current State.";
                bool bTempReal = g_bTempReal;
                bRet = ResetDevice();
                g_bTempReal = bTempReal;
                if(false == bRet)	continue;
                // 开启Smode
                bRet = g_comKline.OpenSMode();
                if(false == bRet)	continue;
                // 开启RFSC
                bRet = g_comKline.OpenRealtimeFault();
                if(false == bRet)	continue;
                // 启动当前测试步骤继电器
                g_daqDigit.EnableRelayGroup(g_iCurrItem, MES_FIND);
            	continue;
            }
        	break;
		}

        // 测电阻
		if (iMethod > 1)
		{
			g_daqDigit.SetSbRelay(g_iCurrBoard, iGroup, iBit, false);
			g_daqDigit.SetMbRelay(300, false);
			g_daqDigit.SetMbRelay(302 - iMethod % 2, false);
			g_daqDigit.SetSbRelay(g_iCurrBoard, 284, true);
			g_daqDigit.SetSbRelay(g_iCurrBoard, 285, true);
		} else
		{
			g_daqDigit.SetSbRelay(g_iCurrBoard, iGroup, iBit, true);
			g_daqDigit.SetSbRelay(g_iCurrBoard, iGroup, iBit - 1, true);
			g_daqDigit.SetSbRelay(g_iCurrBoard, iGroup, iBit - 2, false);
			g_daqDigit.SetSbRelay(g_iCurrBoard, iGroup, iBit - 3, true);
		}

		float	fRes = 0.0;
        if(MES_KLINE_FOUND == iTypeRet)
		    g_gpibPcDMM.GetFRes(fRes);

        // 小电阻调整
		if (iMethod < 2 && MES_KLINE_FOUND == iTypeRet) fRes = fRes + g_STestInfo.m_fAmendRes[g_iCurrBoard][iMethod];

		for (int j = 0; j < 100; j++)
		{
			G_SDelay(10);
			if (FormMain->m_bWrite == false) break;
		}

        g_strItemValue = FloatToStr(fRes);
		Synchronize(FormMain->WriteItemTestData);

		if (MES_KLINE_FOUND != iTypeRet) G_SDelay(1000);
	}
    while (iRepeat < g_iItemRepeat && MES_KLINE_FOUND != iTypeRet);

	if (iMethod > 1)
	{
		g_daqDigit.SetSbRelay(g_iCurrBoard, 284, false);
		g_daqDigit.SetSbRelay(g_iCurrBoard, 285, false);
	} else
	{
		g_daqDigit.SetSbRelay(g_iCurrBoard, iGroup, iBit, false);
		g_daqDigit.SetSbRelay(g_iCurrBoard, iGroup, iBit - 1, false);
		g_daqDigit.SetSbRelay(g_iCurrBoard, iGroup, iBit - 3, false);
	}
}

///////////////////////////////////////////////////////////////////////////////
// U05:
void __fastcall TThreadMain::TestSbelt(int iSubItem, int iMethod)
{
	// A1: 判断条件是否选择
	g_iCurrItem = iMethod*3 + iSubItem + 40 - 1;	// based 0
    unsigned char	iACUTotal = g_STestInfo.m_iItem[g_iCurrItem];
	if ((g_STestInfo.m_iSelACU >> (g_iCurrBoard)) & 0x01 != 0x01)
	{
		iSubItem = iSubItem;
		return;
	}

    unsigned char cValue = (iACUTotal >> (g_iCurrBoard));
    cValue = cValue & 0x01;
    if (cValue != 0x01)
	{
		iSubItem = iSubItem;
		return;
	}

    // A2：操作测试项
    char			iGroup = iMethod/2;
	char			iBit = (iMethod%2) * 4;
    int				iTypeRet = MES_KLINE_ZERO;
	unsigned int	iFaultCode = 0, iRepeat = 0;
	float			fMin, fMax;
	String			strTemp = "", strFull = "";

    // 界面及变量变化
	g_iCurveMode = 0;
    FormMain->ResetErrorInfo(0);
    Synchronize(FormMain->ClearWave);
	Synchronize(FormMain->ChangeItem);

    WriteLogFile(g_pFileComKline, "\n\n\n");
    String strSpec =  GetSPECInfo(g_iCurrItem, 0) + "\t\t" + GetSPECInfo(g_iCurrItem, 1);
    strSpec =  strSpec + "\t\t" + GetSPECInfo(g_iCurrItem, 2) + "\t\t" + GetSPECInfo(g_iCurrItem, 3);
    WriteLogFile(g_pFileComKline, strSpec + "\n");

    strFull = GetSPECInfo(g_iCurrItem, 1);
	strTemp = strFull.SubString(1, strFull.Pos("~") - 1);
	fMin = strTemp.ToDouble();
	strFull = strFull.SubString(strFull.Pos("~") + 1, strFull.Length() - strFull.Pos("~"));
	fMax = strFull.ToDouble();
	iFaultCode = GetSPECInfo(g_iCurrItem, 3).ToInt();

	do
	{
    	iRepeat++;
        g_daqDigit.SetSbRelay(g_iCurrBoard, iGroup, iBit, false);
        g_daqDigit.SetSbRelay(g_iCurrBoard, iGroup, iBit + 1, false);
        g_daqDigit.SetSbRelay(g_iCurrBoard, iGroup, iBit + 2, false);
        g_daqDigit.SetSbRelay(g_iCurrBoard, iGroup, iBit + 3, false);
        g_daqDigit.SetMbRelay(302, false);

        g_daqDigit.SetSbRelay(g_iCurrBoard, iGroup, iBit + 1, true);
        g_daqDigit.SetSbRelay(g_iCurrBoard, iGroup, iBit + 3, true);
        g_daqDigit.SetMbRelay(302, true);

		float fRet = 0.0;
        bool  bRet = false;
        // 进行最多3次连续查找查找
		for (unsigned int i = 0; i < g_iFindRepeat; i++)
		{
        	g_strSubTip = "Find State.";
			iTypeRet = FindSbelt(fMin, fMax, iFaultCode, fRet);
            WriteLogFile(g_pFileComKline, "inner\n");

			if (MES_KLINE_ZERO == iTypeRet)
            {
            	g_strSubTip = "Reset Device to Current State.";
            	// 设备重置
                bool bTempReal = g_bTempReal;
                bRet = ResetDevice();
                g_bTempReal = bTempReal;
                if(false == bRet)	continue;
                // 开启Smode
                bRet = g_comKline.OpenSMode();
                if(false == bRet)	continue;
                // 开启RFSC
                bRet = g_comKline.OpenRealtimeFault();
                if(false == bRet)	continue;
                // 启动当前测试步骤继电器
                g_daqDigit.EnableRelayGroup(g_iCurrItem, MES_FIND);
            	continue;
            }
        	break;
		}

        // 测电阻
        g_daqDigit.SetSbRelay(g_iCurrBoard, iGroup, iBit, true);
        g_daqDigit.SetSbRelay(g_iCurrBoard, iGroup, iBit + 1, false);
        g_daqDigit.SetSbRelay(g_iCurrBoard, iGroup, iBit + 2, true);
        g_daqDigit.SetSbRelay(g_iCurrBoard, iGroup, iBit + 3, true);

		float	fRes = 0.0;
        if(MES_KLINE_FOUND == iTypeRet)
		    g_gpibPcDMM.GetFRes(fRes);

		for (int j = 0; j < 100; j++)
		{
			G_SDelay(10);
			if (FormMain->m_bWrite == false) break;
		}

        g_strItemValue = FloatToStr(fRes);
		Synchronize(FormMain->WriteItemTestData);

		if (MES_KLINE_FOUND != iTypeRet) G_SDelay(1000);
	}
    while (iRepeat < g_iItemRepeat && MES_KLINE_FOUND != iTypeRet);

	g_daqDigit.SetSbRelay(g_iCurrBoard, iGroup, iBit, false);
    g_daqDigit.SetSbRelay(g_iCurrBoard, iGroup, iBit + 1, false);
    g_daqDigit.SetSbRelay(g_iCurrBoard, iGroup, iBit + 2, false);
    g_daqDigit.SetSbRelay(g_iCurrBoard, iGroup, iBit + 3, false);
    g_daqDigit.SetMbRelay(302, false);
}

///////////////////////////////////////////////////////////////////////////////
// U06:
void __fastcall TThreadMain::TestBatt(int iSubItem)
{
	// A1: 判断条件是否选择
	g_iCurrItem = iSubItem + 49;	// based 0
    unsigned char	iACUTotal = g_STestInfo.m_iItem[g_iCurrItem];
	if ((g_STestInfo.m_iSelACU >> (g_iCurrBoard)) & 0x01 != 0x01)
	{
		iSubItem = iSubItem;
		return;
	}

    unsigned char cValue = (iACUTotal >> (g_iCurrBoard));
    cValue = cValue & 0x01;
    if (cValue != 0x01)
	{
		iSubItem = iSubItem;
		return;
	}

    // A2：操作测试项
    g_bVoltReal = false;
    int				iTypeRet = MES_KLINE_ZERO;
	float			fMin = 10000.0, fMax =0.0;
	String			strTemp = "", strFull = "";
    unsigned int	iFaultCode = 0, iRepeat = 0;

    // 界面及变量变化
	g_iCurveMode = 0;
    FormMain->ResetErrorInfo(0);
    Synchronize(FormMain->ClearWave);
	Synchronize(FormMain->ChangeItem);

    WriteLogFile(g_pFileComKline, "\n\n\n");
    String strSpec =  GetSPECInfo(g_iCurrItem, 0) + "\t\t" + GetSPECInfo(g_iCurrItem, 1);
    strSpec =  strSpec + "\t\t" + GetSPECInfo(g_iCurrItem, 2) + "\t\t" + GetSPECInfo(g_iCurrItem, 3);
    WriteLogFile(g_pFileComKline, strSpec + "\n");

    strFull = GetSPECInfo(g_iCurrItem, 1);
	strTemp = strFull.SubString(1, strFull.Pos("~") - 1);
	fMin = strTemp.ToDouble();
	strFull = strFull.SubString(strFull.Pos("~") + 1, strFull.Length() - strFull.Pos("~"));
	fMax = strFull.ToDouble();
	iFaultCode = GetSPECInfo(g_iCurrItem, 3).ToInt();

	do
	{
    	iRepeat++;
        g_daqDigit.SetSbRelay(g_iCurrBoard, 266, false);
        float fRet = 0.0;
        bool  bRet = false;
        // 进行最多3次连续查找查找
		for (unsigned int i = 0; i < g_iFindRepeat; i++)
		{
        	g_strSubTip = "Find State.";
            iTypeRet = FindBatt(fMin, fMax, iFaultCode, iSubItem, fRet);
            WriteLogFile(g_pFileComKline, "inner\n");

			if (MES_KLINE_ZERO == iTypeRet)
            {
            	g_strSubTip = "Reset Device to Current State.";
            	// 设备重置
                bool bTempReal = g_bTempReal;
                bRet = ResetDevice();
                g_bTempReal = bTempReal;
                if(false == bRet)	continue;
                // 开启Smode
                bRet = g_comKline.OpenSMode();
                if(false == bRet)	continue;
                // 开启RFSC
                bRet = g_comKline.OpenRealtimeFault();
                if(false == bRet)	continue;
                // 启动当前测试步骤继电器
                g_daqDigit.EnableRelayGroup(g_iCurrItem, MES_FIND);
            	continue;
            }
        	break;
		}

        // 测电压并还原到原始电压状态
        g_daqDigit.SetSbRelay(g_iCurrBoard, 266, true);
		float	fRes = 0.0;
        if(MES_KLINE_FOUND == iTypeRet)
		    g_gpibPcDMM.GetVolt(fRes);
        g_gpibPcPower.SetVolt(g_fVoltTarget);


		for (int j = 0; j < 100; j++)
		{
			G_SDelay(10);
			if (FormMain->m_bWrite == false) break;
		}

        g_strItemValue = FloatToStr(fRes);
		Synchronize(FormMain->WriteItemTestData);

		if (MES_KLINE_FOUND != iTypeRet) G_SDelay(1000);
	}
    while (iRepeat < g_iItemRepeat && MES_KLINE_FOUND != iTypeRet);

	g_daqDigit.SetSbRelay(g_iCurrBoard, 266, false);
    g_bVoltReal = true;
}

///////////////////////////////////////////////////////////////////////////////
// U07:
void __fastcall TThreadMain::TestSIS(int iSubItem, int iMethod)
{
	// A1: 判断条件是否选择
	g_iCurrItem = iSubItem + iMethod * 4 + 50;	// based 0
	unsigned char	iACUTotal = g_STestInfo.m_iItem[g_iCurrItem];
	if ((g_STestInfo.m_iSelACU >> (g_iCurrBoard)) & 0x01 != 0x01)
	{
		iSubItem = iSubItem;
		return;
	}

	unsigned char	cValue = (iACUTotal >> (g_iCurrBoard));
	cValue = cValue & 0x01;
	if (cValue != 0x01)
	{
		iSubItem = iSubItem;
		return;
	}

	// A2：操作测试项
	int				iGroup = 8;
	int				iBit = 3;
	int				iTypeRet = MES_KLINE_ZERO;
	float			fMin = 10000.0, fMax = 0.0;
	String			strTemp = "", strFull = "";
	unsigned int	iFaultCode = 0, iRepeat = 0;

	// 界面及变量变化
	g_iCurveMode = 0;
	FormMain->ResetErrorInfo(0);
	Synchronize(FormMain->ClearWave);
	Synchronize(FormMain->ChangeItem);

	WriteLogFile(g_pFileComKline, "\n\n\n");
	String	strSpec = GetSPECInfo(g_iCurrItem, 0) + "\t\t" + GetSPECInfo(g_iCurrItem, 1);
	strSpec = strSpec + "\t\t" + GetSPECInfo(g_iCurrItem, 2) + "\t\t" + GetSPECInfo(g_iCurrItem, 3);
	WriteLogFile(g_pFileComKline, strSpec + "\n");
	strFull = GetSPECInfo(g_iCurrItem, 1);
	strTemp = strFull.SubString(1, strFull.Pos("~") - 1);
	fMin = strTemp.ToDouble();
	strFull = strFull.SubString(strFull.Pos("~") + 1, strFull.Length() - strFull.Pos("~"));
	fMax = strFull.ToDouble();
	iFaultCode = GetSPECInfo(g_iCurrItem, 3).ToInt();

	do
	{
		iRepeat++;
		g_daqDigit.SetSbRelay(g_iCurrBoard, iGroup, iBit, false);
		g_daqDigit.SetSbRelay(g_iCurrBoard, iGroup, iBit - 1, false);
		g_daqDigit.SetSbRelay(g_iCurrBoard, iGroup, iBit - 2, false);
		g_daqDigit.SetSbRelay(g_iCurrBoard, iGroup, iBit - 3, false);

		g_daqDigit.SetSbRelay(g_iCurrBoard, iGroup, iSubItem - 1, true);
		g_daqDigit.SetMbRelay(301 + iMethod, true);

		float	fRet = 0.0;
        bool	bRet = false;
		// 进行最多3次连续查找查找
		for (unsigned int i = 0; i < g_iFindRepeat; i++)
		{
        	g_strSubTip = "Find State.";
			iTypeRet = FindSIS(fMin, fMax, iFaultCode, iMethod, fRet);
			WriteLogFile(g_pFileComKline, "inner\n");
			if (MES_KLINE_ZERO == iTypeRet)
			{
            	g_strSubTip = "Reset Device to Current State.";
				// 设备重置
				bool bTempReal = g_bTempReal;
                bRet = ResetDevice();
                g_bTempReal = bTempReal;
				if (false == bRet) continue;
				// 开启Smode
				bRet = g_comKline.OpenSMode();
				if (false == bRet) continue;
				// 开启RFSC
				bRet = g_comKline.OpenRealtimeFault();
				if (false == bRet) continue;
				// 启动当前测试步骤继电器
				g_daqDigit.EnableRelayGroup(g_iCurrItem, MES_FIND);
				continue;
			}
            break;
		}

		g_daqDigit.SetSbRelay(g_iCurrBoard, iGroup, iSubItem - 1, false);
		g_daqDigit.SetMbRelay(301 + iMethod, false);

		g_daqDigit.SetSbRelay(g_iCurrBoard, iGroup, 4, true);
		g_daqDigit.SetSbRelay(g_iCurrBoard, iGroup, 5, true);

		float	fRes = 0.0;
		if (MES_KLINE_FOUND == iTypeRet) g_gpibPcDMM.GetFRes(fRes);

		for (int j = 0; j < 100; j++)
		{
			G_SDelay(10);
			if (FormMain->m_bWrite == false) break;
		}

		g_strItemValue = FloatToStr(fRes);
		Synchronize(FormMain->WriteItemTestData);

		if (MES_KLINE_FOUND != iTypeRet) G_SDelay(1000);
	}
	while (iRepeat < g_iItemRepeat && MES_KLINE_FOUND != iTypeRet);

	// 继电器还原
	g_daqDigit.SetSbRelay(g_iCurrBoard, iGroup, 4, false);
	g_daqDigit.SetSbRelay(g_iCurrBoard, iGroup, 5, false);
}

void __fastcall TThreadMain::TestSISSensor(int iSubItem)
{
	// A1: 判断条件是否选择
	g_iCurrItem = iSubItem + 58;	// based 0
	unsigned char	iACUTotal = g_STestInfo.m_iItem[g_iCurrItem];
	if ((g_STestInfo.m_iSelACU >> (g_iCurrBoard)) & 0x01 != 0x01)
	{
		iSubItem = iSubItem;
		return;
	}

	unsigned char	cValue = (iACUTotal >> (g_iCurrBoard));
	cValue = cValue & 0x01;
	if (cValue != 0x01)
	{
		iSubItem = iSubItem;
		return;
	}

	// A2：操作测试项
	//int				iGroup = 8;
	//int				iBit = 3;
	int				iTypeRet = MES_KLINE_ZERO;
	//float			fMin = 10000.0, fMax = 0.0;
	//String			strTemp = "", strFull = "";
	//unsigned int	iFaultCode = 0, iRepeat = 0;
    unsigned int iRepeat = 0;

	// 界面及变量变化
	g_iCurveMode = 0;
	FormMain->ResetErrorInfo(0);
	Synchronize(FormMain->ClearWave);
	Synchronize(FormMain->ChangeItem);

	WriteLogFile(g_pFileComKline, "\n\n\n");
	String	strSpec = GetSPECInfo(g_iCurrItem, 0) + "\t\t" + GetSPECInfo(g_iCurrItem, 1);
	strSpec = strSpec + "\t\t" + GetSPECInfo(g_iCurrItem, 2) + "\t\t" + GetSPECInfo(g_iCurrItem, 3);
	WriteLogFile(g_pFileComKline, strSpec + "\n");
    /*
	strFull = GetSPECInfo(g_iCurrItem, 1);
	strTemp = strFull.SubString(1, strFull.Pos("~") - 1);
	fMin = strTemp.ToDouble();
	strFull = strFull.SubString(strFull.Pos("~") + 1, strFull.Length() - strFull.Pos("~"));
	fMax = strFull.ToDouble();
	iFaultCode = GetSPECInfo(g_iCurrItem, 3).ToInt();
    */
	do
	{
		iRepeat++;

		float	fRet = 0.0;
        bool	bRet = false;
        int 	iCodeRet = 0;
		// 进行最多3次连续查找查找
		for (unsigned int i = 0; i < g_iFindRepeat; i++)
		{
        	g_strSubTip = "Find State.";
			iTypeRet = FindSISSensor(iSubItem, iCodeRet);
            fRet = iCodeRet;
			WriteLogFile(g_pFileComKline, "inner\n");
			if (MES_KLINE_ZERO == iTypeRet)
			{
            	g_strSubTip = "Reset Device to Current State.";
				// 设备重置
				bool bTempReal = g_bTempReal;
                bRet = ResetDevice();
                g_bTempReal = bTempReal;
				if (false == bRet) continue;
				// 开启Smode
				bRet = g_comKline.OpenSMode();
				if (false == bRet) continue;
				continue;
			}
            break;
		}

		float	fRes = fRet;
		for (int j = 0; j < 100; j++)
		{
			G_SDelay(10);
			if (FormMain->m_bWrite == false) break;
		}

		g_strItemValue = FloatToStr(fRes);
		Synchronize(FormMain->WriteItemTestData);

		if (MES_KLINE_FOUND != iTypeRet) G_SDelay(1000);
	}
	while (iRepeat < g_iItemRepeat && MES_KLINE_FOUND != iTypeRet);
}

// Resistance Too High 小电阻肯定没有
// Resistance Too Low or Short 大电阻肯定没有
// Short to Ground 大电阻肯定没有
// Short to Battery 大电阻肯定没有
int __fastcall TThreadMain::FindSquib(float fMin, float fMax, unsigned int iFaultCode, int iMethod, float & fRet)
{
	fMin = 0.0;
    long lMax = fMax + 0.5;
    fMax = lMax;

	bool	bMax = false, bMin = false, bMean = false, bKline = false, bDirection = true;
	int		iMode = 0;
	double	dMax =0.0, dMin = 10000.0, dMean = 0.0, dLen = 0.0;

    // 正向 小电阻肯定没有：表示错误码在测试规范的大值处附近
    // 反向 大电阻肯定没有：表示错误码在测试范围的小值处附近
	switch (iMethod)
	{
		case 0:		bDirection = true; break;		  	// 往小方向找
		case 1:		bDirection = false; break;			// 往大方向找
		case 2:		bDirection = false; break;			// 往大方向找
		case 3:		bDirection = false; break;			// 往大方向找
		default:	bDirection = true; break;
	}

	// 清空测试时值曲线显示
    g_iCurveMode = 0;
	Synchronize(FormMain->ClearWave);
	dLen = fMax - fMin;

	// 先判断端点上的情况 max min
	bMin = GetFaultCodeStateByItem(iMethod, fMin, iFaultCode, bKline, MES_TYPE_FC);
    if (false == bKline) return MES_KLINE_ZERO;
	bMax = GetFaultCodeStateByItem(iMethod, fMax, iFaultCode, bKline, MES_TYPE_FC);
    if (false == bKline) return MES_KLINE_ZERO;

	if (false == bDirection)
	{
		bMin = !bMin;
		bMax = !bMax;
	}

	if ((bMax == true) && (bMin == false)) iMode = 0;				// 正向举例：直接找到了区间
	if ((bMax == true) && (bMin == true)) iMode = 1;				// 正向举例：要找的电阻值在min之下
	if ((bMax == false) && (bMin == false)) iMode = 2;				// 正向举例：要找的电阻值在max之上
	if ((bMax == false) && (bMin == true)) return MES_KLINE_ZERO;	// 正向举例：物理意义上不可能出现

	switch (iMode)
	{
		case 0:
			{
				dMax = fMax;
				dMin = fMin;
				break;
			}

		case 1:
			{
				dMax = fMin;
				dMin = 0.5 * fMin;						// (需要验正)
				bMin = GetFaultCodeStateByItem(iMethod, dMin, iFaultCode, bKline, MES_TYPE_FC);
                if (MES_KLINE_ZERO == bKline) return MES_KLINE_ZERO;

				if (false == bDirection) bMin = !bMin;

				while ((bMin == true) && (dMin >= 0.01))
				{
					dMax = dMin;
					dMin = 0.5 * dMin;
					bMin = GetFaultCodeStateByItem(iMethod, dMin, iFaultCode, bKline, MES_TYPE_FC);
                    if (MES_KLINE_ZERO == bKline) return MES_KLINE_ZERO;

					if (false == bDirection) bMin = !bMin;
				}

				// 下面对出循环的各种情况进行筛选，分别处理
				if ((bMin == true) && (dMin < 0.01))
				{
					fRet = 0.01;
					return MES_KLINE_NOTFOUND;
				}

				break;
			}

		case 2:
			{
				dMin = fMax;
				dMax = 2 * fMax;							// (需要验证)
				bMax = GetFaultCodeStateByItem(iMethod, dMax, iFaultCode, bKline, MES_TYPE_FC);
                if (MES_KLINE_ZERO == bKline) return MES_KLINE_ZERO;
				if (false == bDirection) bMax = !bMax;

				while ((bMax == false) && (dMax <= 10000))
				{
					dMin = dMax;
					dMax = 2 * dMax;
					bMax = GetFaultCodeStateByItem(iMethod, dMax, iFaultCode, bKline, MES_TYPE_FC);
                    if (MES_KLINE_ZERO == bKline) return MES_KLINE_ZERO;

					if (!bDirection) bMax = !bMax;
				}

				// 下面对出循环的各种情况进行筛选，分别处理
				if ((bMax == false) && (dMax > 20000))
				{
					fRet = 20000;
					return MES_KLINE_NOTFOUND;
				}

				break;
			}

		default:
			{
            	;
			}
	}							// end switch

	// 确定了Rmin,Rmax后，在(Rmin,Rmax)区间内折半查找
    float fPrecision = MES_RECURSIVE_LOW;
    if(fMax >= 1000.0)
        fPrecision = MES_RECURSIVE_HIGH;
	while ((dMax - dMin) / dLen > fPrecision)
	{
		dMean = (dMin + dMax) / 2;
		bMean = GetFaultCodeStateByItem(iMethod, dMean, iFaultCode, bKline, MES_TYPE_FC);
        if (MES_KLINE_ZERO == bKline) return MES_KLINE_ZERO;
		if (false == bDirection) bMean = !bMean;
		switch (bMean)
		{
			case true:				// 折半电阻高
				dMax = dMean; break;
			case false:				// 折半电阻低
				dMin = dMean; break;
			default:	;
		}
	}

	fRet = (dMax + dMin) / 2;	// 循环结束后Rmax Rmin之间的距离相对足够小，再取其平均值作为此次测量的结果
	return MES_KLINE_FOUND;
}

int __fastcall TThreadMain::FindSbelt(float fMin, float fMax, unsigned int iFaultCode, float & fRet)
{
	fMin = 0.0;
    long lMax = fMax + 0.5;
    fMax = lMax;

	bool	bMax = false, bMin = false, bMean = false, bKline = true;
	int		iMode = 0;
	double	dMax = 0.0, dMin = 10000.0, dMean = 0.0, dLen = 0.0;

    // 正向 小电阻肯定没有：表示错误码在测试规范的大值处附近
    // 反向 大电阻肯定没有：表示错误码在测试范围的小值处附近

	// 清空测试时值曲线显示
    g_iCurveMode = 0;
	Synchronize(FormMain->ClearWave);
	dLen = fMax - fMin;

	// 先判断端点上的情况 max min
    int iMethod = 0;
	bMin = GetFaultCodeStateByItem(iMethod, fMin, iFaultCode, bKline, MES_TYPE_SBELT);
    if (MES_KLINE_ZERO == bKline) return MES_KLINE_ZERO;
	bMax = GetFaultCodeStateByItem(iMethod, fMax, iFaultCode, bKline, MES_TYPE_SBELT);
    if (MES_KLINE_ZERO == bKline) return MES_KLINE_ZERO;

	if ((bMax == true) && (bMin == false)) 	iMode = 0;				// 正向举例：直接找到了区间
	if ((bMax == true) && (bMin == true)) 	iMode = 1;				// 正向举例：要找的电阻值在min之下
	if ((bMax == false) && (bMin == false)) iMode = 2;				// 正向举例：要找的电阻值在max之上
	if ((bMax == false) && (bMin == true)) 	return MES_KLINE_ZERO;	// 正向举例：物理意义上不可能出现

	switch (iMode)
	{
		case 0:
			{
				dMax = fMax;
				dMin = fMin;
				break;
			}

		case 1:
			{
				dMax = fMin;
				dMin = 0.5 * fMin;						// (需要验正)
				bMin = GetFaultCodeStateByItem(iMethod, dMin, iFaultCode, bKline, MES_TYPE_SBELT);
                if (MES_KLINE_ZERO == bKline) return MES_KLINE_ZERO;

				while ((bMin == true) && (dMin >= 0.01))
				{
					dMax = dMin;
					dMin = 0.5 * dMin;
					bMin = GetFaultCodeStateByItem(iMethod, dMin, iFaultCode, bKline, MES_TYPE_SBELT);
                    if (MES_KLINE_ZERO == bKline) return MES_KLINE_ZERO;
				}

				// 下面对出循环的各种情况进行筛选，分别处理
				if ((bMin == true) && (dMin < 0.01))
				{
					fRet = 0.01;
					return MES_KLINE_NOTFOUND;
				}

				break;
			}

		case 2:
			{
				dMin = fMax;
				dMax = 2 * fMax;							// (需要验证)
				bMax = GetFaultCodeStateByItem(iMethod, dMax, iFaultCode, bKline, MES_TYPE_SBELT);
                if (MES_KLINE_ZERO == bKline) return MES_KLINE_ZERO;

				while ((bMax == false) && (dMax <= 10000))
				{
					dMin = dMax;
					dMax = 2 * dMax;
					bMax = GetFaultCodeStateByItem(iMethod, dMax, iFaultCode, bKline, MES_TYPE_SBELT);
                    if (MES_KLINE_ZERO == bKline) return MES_KLINE_ZERO;
				}

				// 下面对出循环的各种情况进行筛选，分别处理
				if ((bMax == false) && (dMax > 20000))
				{
					fRet = 20000;
					return MES_KLINE_NOTFOUND;
				}

				break;
			}

		default:
			{
            	;
			}
	}							// end switch

	// 确定了Rmin,Rmax后，在(Rmin,Rmax)区间内折半查找
    float fPrecision = MES_RECURSIVE_LOW;
    if(fMax >= 1000.0)
        fPrecision = MES_RECURSIVE_HIGH;
	while ((dMax - dMin) / dLen > fPrecision)
	{
		dMean = (dMin + dMax) / 2;
		bMean = GetFaultCodeStateByItem(iMethod, dMean, iFaultCode, bKline, MES_TYPE_SBELT);
        if (MES_KLINE_ZERO == bKline) return MES_KLINE_ZERO;
		switch (bMean)
		{
			case true:				// 折半电阻高
				dMax = dMean; break;
			case false:				// 折半电阻低
				dMin = dMean; break;
			default:	;
		}
	}

	fRet = (dMax + dMin) / 2;	// 循环结束后Rmax Rmin之间的距离相对足够小，再取其平均值作为此次测量的结果
	return MES_KLINE_FOUND;
}

int __fastcall TThreadMain::FindBatt(float fMin, float fMax, unsigned int iFaultCode, int iMethod, float & fVolt)
{
	bool	bMax = 0, bMin = 0, bMean = 0, bKline = true, bDirection = false;
	int		iMode = 0;
	double	dMax = 0.0, dMin = 10000.0, dMean = 0.0, dLen = 0.0;

    // 正向 小电阻肯定没有：表示错误码在测试规范的大值处附近
    // 反向 大电阻肯定没有：表示错误码在测试范围的小值处附近
	switch (iMethod)
	{
		case 0:		bDirection = true; break;		  	// 往小方向找
		case 1:		bDirection = false; break;			// 往大方向找
		default:	bDirection = true; break;
	}

	// 清空测试时值曲线显示
    g_iCurveMode = 0;
	Synchronize(FormMain->ClearWave);
	dLen = fMax - fMin;

	// 先判断端点上的情况 max min
	bMin = GetFaultCodeStateByVolt(iMethod, fMin, iFaultCode, bKline);
    if (MES_KLINE_ZERO == bKline) return MES_KLINE_ZERO;
	bMax = GetFaultCodeStateByVolt(iMethod, fMax, iFaultCode, bKline);
    if (MES_KLINE_ZERO == bKline) return MES_KLINE_ZERO;

	if (false == bDirection)
	{
		bMin = !bMin;
		bMax = !bMax;
	}

	if ((bMax == true) && (bMin == false)) 	iMode = 0;				// 正向举例：直接找到了区间
	if ((bMax == true) && (bMin == true)) 	iMode = 1;				// 正向举例：要找的电阻值在min之下
	if ((bMax == false) && (bMin == false)) iMode = 2;				// 正向举例：要找的电阻值在max之上
	if ((bMax == false) && (bMin == true)) 	return MES_KLINE_ZERO;	// 正向举例：物理意义上不可能出现

	switch (iMode)
	{
		case 0:
			{
				dMax = fMax;
				dMin = fMin;
				break;
			}

		case 1:
			{
				dMax = fMin;
				dMin = 0.5 * fMin;
				bMin = GetFaultCodeStateByVolt(iMethod, dMin, iFaultCode, bKline);
                if (MES_KLINE_ZERO == bKline) return MES_KLINE_ZERO;

				if (false == bDirection) bMin = !bMin;

				while ((bMin == true) && (dMin >= 3.0))
				{
					dMax = dMin;
					dMin = 0.5 * dMin;
					bMin = GetFaultCodeStateByVolt(iMethod, dMin, iFaultCode, bKline);
                    if (MES_KLINE_ZERO == bKline) return MES_KLINE_ZERO;

					if (false == bDirection) bMin = !bMin;
				}

				// 下面对出循环的各种情况进行筛选，分别处理
				if ((bMin == true) && (dMin < 3.0))
				{
					fVolt = 0.01;
					return MES_KLINE_NOTFOUND;
				}

				break;
			}

		case 2:
			{
				dMin = fMax;
				dMax = 2 * fMax;							// (需要验证)
				bMax = GetFaultCodeStateByVolt(iMethod, dMax, iFaultCode, bKline);
                if (MES_KLINE_ZERO == bKline) return MES_KLINE_ZERO;
				if (false == bDirection) bMax = !bMax;

				while ((bMax == false) && (dMax <= 35.0))
				{
					dMin = dMax;
					dMax = 2 * dMax;
					bMax = GetFaultCodeStateByVolt(iMethod, dMax, iFaultCode, bKline);
                    if (MES_KLINE_ZERO == bKline) return MES_KLINE_ZERO;

					if (!bDirection) bMax = !bMax;
				}

				// 下面对出循环的各种情况进行筛选，分别处理
				if ((bMax == false) && (dMax > 35.0))
				{
					fVolt = 35.0;
					return MES_KLINE_NOTFOUND;
				}

				break;
			}

		default:
			{
            	;
			}
	}							// end switch

	// 确定了Rmin,Rmax后，在(Rmin,Rmax)区间内折半查找
    float fPrecision = g_STestInfo.m_fPrecision[3];
	while ((dMax - dMin) / dLen > fPrecision)
	{
		dMean = (dMin + dMax) / 2;
		bMean = GetFaultCodeStateByVolt(iMethod, dMean, iFaultCode, bKline);
        if (MES_KLINE_ZERO == bKline) return MES_KLINE_ZERO;
		if (false == bDirection) bMean = !bMean;
		switch (bMean)
		{
			case true:				// 折半电阻高
				dMax = dMean; break;
			case false:				// 折半电阻低
				dMin = dMean; break;
			default:	;
		}
	}

    // 循环结束后Rmax Rmin之间的距离相对足够小，再取其平均值作为此次测量的结果
	fVolt = (dMax + dMin) / 2;

    return MES_KLINE_FOUND;
}

// 正向 表示错误码在测试规范的大值处附近
int __fastcall TThreadMain::FindSIS(float fMin, float fMax, unsigned int iFaultCode, int iMethod, float & fRes)
{
	bool	bMax = false, bMin = false, bMean = false, bKline = true, bDirection = false;
	int		iMode = 0;
	double	dMax = 0.0, dMin = 10000.0, dMean = 0.0, dLen = 0.0;

    fMin = 0.0;

    // 正向 小电阻肯定没有：表示错误码在测试规范的大值处附近
    // 反向 大电阻肯定没有：表示错误码在测试范围的小值处附近
	switch (iMethod)
	{
		case 0:		bDirection = false; break;
		case 1:		bDirection = false; break;
		case 2:		bDirection = false; break;//此句无用 mcl 
		default:	bDirection = true; break;
	}

	// 清空测试时值曲线显示
    g_iCurveMode = 0;
	Synchronize(FormMain->ClearWave);
	dLen = fMax - fMin;

	// 先判断端点上的情况 max min
	bMin = GetFaultCodeStateByItem(iMethod, fMin, iFaultCode, bKline, MES_TYPE_SIS);
    if (MES_KLINE_ZERO == bKline) return MES_KLINE_ZERO;
	bMax = GetFaultCodeStateByItem(iMethod, fMax, iFaultCode, bKline, MES_TYPE_SIS);
    if (MES_KLINE_ZERO == bKline) return MES_KLINE_ZERO;

	if (!bDirection)
	{
		bMin = !bMin;
		bMax = !bMax;
	}

	if ((bMax == true) && (bMin == false)) iMode = 0;				// 直接找到了区间
	if ((bMax == true) && (bMin == true)) iMode = 1;				// 要找的电阻值在min之下
	if ((bMax == false) && (bMin == false)) iMode = 2;				// 要找的电阻值在max之上
	if ((bMax == false) && (bMin == true)) return MES_KLINE_ZERO;	// 物理意义上不可能出现

	switch (iMode)
	{
		case 0:
			{
				dMax = fMax;
				dMin = fMin;
				break;
			}

		case 1:
			{
				dMax = fMin;
				dMin = 0.5 * fMin;
				bMin = GetFaultCodeStateByItem(iMethod, dMin, iFaultCode, bKline, MES_TYPE_SIS);
                if (MES_KLINE_ZERO == bKline) return MES_KLINE_ZERO;

				if (!bDirection) bMin = !bMin;

				while ((bMin == true) && (dMin >= 0.01))
				{
					dMax = dMin;
					dMin = 0.5 * dMin;
					bMin = GetFaultCodeStateByItem(iMethod, dMin, iFaultCode, bKline, MES_TYPE_SIS);
                    if (MES_KLINE_ZERO == bKline) return MES_KLINE_ZERO;

					if (!bDirection) bMin = !bMin;
				}

				// 下面对出循环的各种情况进行筛选，分别处理
				if ((bMin == true) && (dMin < 0.01))
				{
					fRes = 0.01;
					return MES_KLINE_NOTFOUND;
				}

				break;
			}

		case 2:
			{
				dMin = fMax;
				dMax = 2 * fMax;
				bMax = GetFaultCodeStateByItem(iMethod, dMax, iFaultCode, bKline, MES_TYPE_SIS);
                if (MES_KLINE_ZERO == bKline) return MES_KLINE_ZERO;
				if (!bDirection) bMax = !bMax;

				while ((bMax == false) && (dMax <= 10000))
				{
					dMin = dMax;
					bMax = GetFaultCodeStateByItem(iMethod, dMax, iFaultCode, bKline, MES_TYPE_SIS);
                    if (MES_KLINE_ZERO == bKline) return MES_KLINE_ZERO;

					if (!bDirection) bMax = !bMax;
				}

				// 下面对出循环的各种情况进行筛选，分别处理
				if ((bMax == false) && (dMax > 10000))
				{
					fRes = 10000;
					return MES_KLINE_NOTFOUND;
				}

				break;
			}

		default:
			{ ;
			}
	}							// end switch

	// 确定了Rmin,Rmax后，在(Rmin,Rmax)区间内折半查找
	while ((dMax - dMin) / dLen > 0.01)
	{
		dMean = (dMin + dMax) / 2;
		bMean = GetFaultCodeStateByItem(iMethod, dMean, iFaultCode, bKline, MES_TYPE_SIS);
        if (MES_KLINE_ZERO == bKline) return MES_KLINE_ZERO;
		if (!bDirection) bMean = !bMean;
		switch (bMean)
		{
			case true:				// 折半电阻高
				dMax = dMean; break;
			case false:				// 折半电阻低
				dMin = dMean; break;
			default:	;
		}
	}

	fRes = (dMax + dMin) / 2;	// 循环结束后Rmax Rmin之间的距离相对足够小，再取其平均值作为此次测量的结果
    return MES_KLINE_FOUND;
}

int __fastcall TThreadMain::FindSISSensor(int iSubItem, int &iCodeRet)
{
	bool bKline = true;
    int iCharRet = 0;
  //	if(iSubItem == 2 || iSubItem == 3)  //20090813  delte the limit
      {
    	bKline = g_comKline.RealTimeValueCheck(iSubItem + 3  , iCharRet);
        iCodeRet = iCharRet;
        if(iCodeRet > 128)   iCodeRet = iCodeRet -256;
        if(false == bKline)
        {
        	if(-2 == iCodeRet)		return MES_KLINE_ZERO;
            else if(-1 == iCodeRet)	return MES_KLINE_ZERO;
        }
        else	return MES_KLINE_FOUND;
    }
    return MES_KLINE_FOUND;
}

void __fastcall TThreadMain::TestWarnLamp()
{
	// WL1
	// A1: 判断条件是否选择
	g_iCurrItem = 63;
	if ((g_STestInfo.m_iSelACU >> (g_iCurrBoard)) & 0x01 == 0x01)
	{
		if ((g_STestInfo.m_iItem[63] >> (g_iCurrBoard)) & 0x01 == 0x01)
		{
        	WriteLogFile(g_pFileComKline, "\nTestWarnLamp::GetWarnLmapVolt 1 false Start:    \t\n");
			GetWarnLmapVolt(1, false);
            WriteLogFile(g_pFileComKline, "\nTestWarnLamp::GetWarnLmapVolt 1 false End:    \t\n");
		}
	}
	ViewProgressInfo(true, 0);

	// A1: 判断条件是否选择
    g_iCurrItem = 64;
    if ((g_STestInfo.m_iSelACU >> (g_iCurrBoard)) & 0x01 == 0x01)
	{
		if ((g_STestInfo.m_iItem[64] >> (g_iCurrBoard)) & 0x01 == 0x01)
		{
            WriteLogFile(g_pFileComKline, "\nTestWarnLamp::GetWarnLmapVolt 1 true Start:    \t\n");
			GetWarnLmapVolt(1, true);
            WriteLogFile(g_pFileComKline, "\nTestWarnLamp::GetWarnLmapVolt 1 true End:    \t\n");
		}
	}
	ViewProgressInfo(true, 0);

	// A1: 判断条件是否选择
	g_iCurrItem = 65;
    if ((g_STestInfo.m_iSelACU >> (g_iCurrBoard)) & 0x01 == 0x01)
	{
		if ((g_STestInfo.m_iItem[65] >> (g_iCurrBoard)) & 0x01 == 0x01)
		{
            WriteLogFile(g_pFileComKline, "\nTestWarnLamp::GetWarnLampCurr 1 false Start:    \t\n");
			GetWarnLampCurr(1, false);
            WriteLogFile(g_pFileComKline, "\nTestWarnLamp::GetWarnLampCurr 1 false End:    \t\n");
		}
	}
	ViewProgressInfo(true, 0);

	// A1: 判断条件是否选择
	g_iCurrItem = 66;
    if ((g_STestInfo.m_iSelACU >> (g_iCurrBoard)) & 0x01 == 0x01)
	{
		if ((g_STestInfo.m_iItem[66] >> (g_iCurrBoard)) & 0x01 == 0x01)
		{
            WriteLogFile(g_pFileComKline, "\nTestWarnLamp::GetWarnLampCurr 1 true Start:    \t\n");
			GetWarnLampCurr(1, true);
            WriteLogFile(g_pFileComKline, "\nTestWarnLamp::GetWarnLampCurr 1 true End:    \t\n");
		}
	}
	ViewProgressInfo(true, 0);

	// WL2
	// A1: 判断条件是否选择
	g_iCurrItem = 67;
    if ((g_STestInfo.m_iSelACU >> (g_iCurrBoard)) & 0x01 == 0x01)
	{
		if ((g_STestInfo.m_iItem[67] >> (g_iCurrBoard)) & 0x01 == 0x01)
		{
            WriteLogFile(g_pFileComKline, "\nTestWarnLamp::GetWarnLmapVolt 2 false Start:    \t\n");
			GetWarnLmapVolt(2, false);
            WriteLogFile(g_pFileComKline, "\nTestWarnLamp::GetWarnLmapVolt 2 false End:    \t\n");
		}
	}
	ViewProgressInfo(true, 0);

	// A1: 判断条件是否选择
	g_iCurrItem = 68;
    if ((g_STestInfo.m_iSelACU >> (g_iCurrBoard)) & 0x01 == 0x01)
	{
		if ((g_STestInfo.m_iItem[68] >> (g_iCurrBoard)) & 0x01 == 0x01)
		{
            WriteLogFile(g_pFileComKline, "\nTestWarnLamp::GetWarnLmapVolt 2 true Start:    \t\n");
			GetWarnLmapVolt(2, true);
            WriteLogFile(g_pFileComKline, "\nTestWarnLamp::GetWarnLmapVolt 2 true End:    \t\n");
		}
	}
	ViewProgressInfo(true, 0);

	// A1: 判断条件是否选择
	g_iCurrItem = 69;
    if ((g_STestInfo.m_iSelACU >> (g_iCurrBoard)) & 0x01 == 0x01)
	{
		if ((g_STestInfo.m_iItem[69] >> (g_iCurrBoard)) & 0x01 == 0x01)
		{
            WriteLogFile(g_pFileComKline, "\nTestWarnLamp::GetWarnLampCurr 2 false Start:    \t\n");
			GetWarnLampCurr(2, false);
            WriteLogFile(g_pFileComKline, "\nTestWarnLamp::GetWarnLampCurr 2 false End:    \t\n");
		}
	}
	ViewProgressInfo(true, 0);

	// A1: 判断条件是否选择
	g_iCurrItem = 70;
    if ((g_STestInfo.m_iSelACU >> (g_iCurrBoard)) & 0x01 == 0x01)
	{
		if ((g_STestInfo.m_iItem[70] >> (g_iCurrBoard)) & 0x01 == 0x01)
		{
            WriteLogFile(g_pFileComKline, "\nTestWarnLamp::GetWarnLampCurr 2 true Start:    \t\n");
			GetWarnLampCurr(2, true);
            WriteLogFile(g_pFileComKline, "\nTestWarnLamp::GetWarnLampCurr 2 true End:    \t\n");
		}
	}
	ViewProgressInfo(true, 0);
}

bool __fastcall TThreadMain::GetWarnLmapVolt(int iLamp, bool bMethod)
{
	WriteLogFile(g_pFileComKline, "TThreadMain::GetWarnLmapVolt Start:");
    // 界面及变量变化
    g_iCurveMode = 0;	// 多余
    FormMain->ResetErrorInfo(0);
	Synchronize(FormMain->ChangeItem);

    // A2：操作测试项
    g_comKline.EnableWarnLamp(1, !bMethod);
    G_SDelay(100);

	if (1 == iLamp) 		g_daqDigit.SetSbRelay(g_iCurrBoard, 272, true);
	else if (2 == iLamp) 	g_daqDigit.SetSbRelay(g_iCurrBoard, 276, true);

	float			fMin = 100.0, fMax = 0.0, fVolt = 0.0;
	unsigned int	iRepeat = 0;
    bool			bRet = false;
	String	strSpec = GetSPECInfo(g_iCurrItem, 1);
	String	strTemp = strSpec.SubString(1, strSpec.Pos("~") - 1);
	fMin = strTemp.ToDouble();
	if (bMethod == true)
	{
		strTemp = strSpec.SubString(strSpec.Pos("~") + 1, strSpec.Length() - strSpec.Pos("~"));
        if(strTemp == "vbatt")  fMax = 24.0;
        else                    fMax = strTemp.ToDouble();
	}

	do
	{
		iRepeat++;
		bRet = g_comKline.EnableWarnLamp(iLamp, bMethod);
        if(bRet == false)
    	{
        	g_daqDigit.EnableRelayBase(g_iCurrBoard, true);
            if (1 == iLamp) 		g_daqDigit.SetSbRelay(g_iCurrBoard, 272, true);
			else if (2 == iLamp) 	g_daqDigit.SetSbRelay(g_iCurrBoard, 276, true);
            bool bPowerState = g_daqAnalog.GetPowerState();
            if(false == bPowerState)
            {
            	g_gpibPcPower.SetOutPut(true);
                g_strMark = "PcPower Err.!";
                Synchronize(FormMain->AddErrorByMark);
            }
            continue;
    	}

		g_gpibPcDMM.GetVolt(fVolt);

		// 填表
        g_strItemValue = FloatToStr(fVolt);
		Synchronize(FormMain->WriteItemTestData);
	}
	while (iRepeat < g_iItemRepeat);

	if (1 == iLamp) 		g_daqDigit.SetSbRelay(g_iCurrBoard, 272, false);
	else if (2 == iLamp)	g_daqDigit.SetSbRelay(g_iCurrBoard, 276, false);

    WriteLogFile(g_pFileComKline, "\nTThreadMain::GetWarnLmapVolt End:");
	if (bMethod == false)
	{
		if (fVolt > fMin) return true;
		else return false;
	}

	if (bMethod == true)
	{
		if (fVolt > fMin && fVolt < fMax) return true;
		else return false;
	}

	return true;
}

bool __fastcall TThreadMain::GetWarnLampCurr(int iLamp, bool bMethod)
{
	WriteLogFile(g_pFileComKline, "TThreadMain::GetWarnLampCurr:");
    // 界面及变量变化
    g_iCurveMode = 0;	// 多余
    FormMain->ResetErrorInfo(0);
	Synchronize(FormMain->ChangeItem);

    // A2：操作测试项
    g_comKline.EnableWarnLamp(iLamp, false);
    G_SDelay(100);

	if (true == bMethod)
	{
		if (1 == iLamp) 		g_daqDigit.SetSbRelay(g_iCurrBoard, 270, true);
		else if (2 == iLamp)	g_daqDigit.SetSbRelay(g_iCurrBoard, 274, true);
	}

	if (1 == iLamp) 		g_daqDigit.SetSbRelay(g_iCurrBoard, 271, true);
	else if (2 == iLamp)    g_daqDigit.SetSbRelay(g_iCurrBoard, 275, true);

	float	fMin = 0.0, fMax = 0.0, fCurr = 0.0;
	int		iRepeat = 0;
    bool	bRet = false;
	String	strSpec = GetSPECInfo(g_iCurrItem, 1);
	String	strTemp = strSpec.SubString(1, strSpec.Pos("~") - 1);
	fMin = strTemp.ToDouble();
	strTemp = strSpec.SubString(strSpec.Pos("~") + 1, strSpec.Length() - strSpec.Pos("~"));
	fMax = strTemp.ToDouble();

	do
	{
		iRepeat++;
		bRet = g_comKline.EnableWarnLamp(iLamp, true);
        if(bRet == false)
    	{
        	g_daqDigit.EnableRelayBase(g_iCurrBoard, true);
            if (true == bMethod)
			{
				if (1 == iLamp) 		g_daqDigit.SetSbRelay(g_iCurrBoard, 270, true);
				else if (2 == iLamp)	g_daqDigit.SetSbRelay(g_iCurrBoard, 274, true);
			}
			if (1 == iLamp) 		g_daqDigit.SetSbRelay(g_iCurrBoard, 271, true);
			else if (2 == iLamp)    g_daqDigit.SetSbRelay(g_iCurrBoard, 275, true);
            bool bPowerState = g_daqAnalog.GetPowerState();
            if(false == bPowerState)
            {
            	g_gpibPcPower.SetOutPut(true);
                g_strMark = "PcPower Err.!";
                Synchronize(FormMain->AddErrorByMark);
            }
            continue;
    	}

		g_gpibPcDMM.GetCurr(fCurr);

		// 填表
        g_strItemValue = FloatToStr(fCurr);
		Synchronize(FormMain->WriteItemTestData);
	}
	while (iRepeat < 1);

	if (1 == iLamp) 		g_daqDigit.SetSbRelay(g_iCurrBoard, 271, false);
	else if (2 == iLamp)    g_daqDigit.SetSbRelay(g_iCurrBoard, 275, false);

	if (true == bMethod)
	{
		if (1 == iLamp) 		g_daqDigit.SetSbRelay(g_iCurrBoard, 270, false);
		else if (2 == iLamp)    g_daqDigit.SetSbRelay(g_iCurrBoard, 274, false);
	}

    WriteLogFile(g_pFileComKline, "\nTThreadMain::GetWarnLampCurr End:");
	if (fCurr > fMin && fCurr < fMax) return true;
	else return false;
}

void __fastcall TThreadMain::TestHoldTime()
{
	// A1: 判断条件是否选择
	g_iCurrItem = 71;	// based 0
    unsigned char	iACUTotal = g_STestInfo.m_iItem[g_iCurrItem];
	if ((g_STestInfo.m_iSelACU >> (g_iCurrBoard)) & 0x01 != 0x01)
	{
		g_iCurrItem = 71;
		return;
	}

    unsigned char cValue = (iACUTotal >> (g_iCurrBoard));
    cValue = cValue & 0x01;
    if (cValue != 0x01)
	{
		g_iCurrItem = 71;
		return;
	}

    // 界面及变量变化
    g_iCurveMode = 1;
    FormMain->ResetErrorInfo(0);
    Synchronize(FormMain->ClearWave);
	Synchronize(FormMain->ChangeItem);

    // A2：操作测试项
 	g_daqDigit.EnableRelayBase(g_iCurrBoard, true);
    g_daqDigit.SetSbRelayGroupStatus(g_iCurrBoard, 0, 6);
    g_daqDigit.SetSbRelayGroupStatus(g_iCurrBoard, 1, 16);

    bool bRet = false, bRangeF = false, bZero = false, bRangeS = false, bTrig = false, bView = false;
    float fVolt = 0.0;

    // 判断是否成功进行硬件设置
    g_gpibPcDMM.GetVolt(fVolt);
    String strVolt = FloatToStr(fVolt);
    int iLength = strVolt.Length();
    for(; iLength <= 16; iLength++)
    {
        strVolt = strVolt + "0";
    }
    WriteLogFile(g_pFileVolt, "TestHoldTime:" + strVolt + "\n");
    bRet = g_gpibPcDMM.SetRangeAndGetVolt(bRangeF, fVolt);
    bRet = g_gpibPcDMM.CloseAutoZero(bZero);
    bRet = g_gpibPcDMM.SetRange(bRangeS);
    bRet = g_gpibPcDMM.SetTrigDelay(bTrig);
    bRet = g_gpibPcDMM.CloseView(bView);

    DWORD dwStart = 0, dwEnd = 0, dwTime = 0;
    char fReadBuf[40000] = {0x00};
    if ((bRangeF && bZero && bRangeS && bTrig && bView) != false)
	{		// 开始采样数据
        g_gpibPcDMM.SetHits();
		g_daqDigit.SetSbRelay(g_iCurrBoard, 265, false);
		dwStart = GetTickCount();
        g_gpibPcDMM.GetRangeValue(fReadBuf);
                                                                                                    
		dwEnd = GetTickCount();							// 记录采样结束时系统时钟
		dwTime = dwEnd - dwStart;					   // 采样的总时间

		// 处理采样数据，计算出能量保持时间
		int j = 0, iCount = 0;
        bool bNoEnter = false;
        float fTemp = 0.0, fHodeTime = 0.0;
        float  faData[2500] = {0.0};
        String strTemp = "";

		for (int i = 1; i <= 2500; i++)
		{
			strTemp = "\0";
			for (; j < 16 * i - 1; j++)
                strTemp += fReadBuf[j];
			j++;
			sscanf(strTemp.c_str(), "%f", &fTemp);
			faData[i - 1] = fTemp;
            if ((fabs(1.1 - fTemp) <= 0.5) && !bNoEnter) bNoEnter=true;//出现满足条件电平就使能计数
            if ((fabs(1.1 - fTemp) > 0.5) && bNoEnter)
			{
				iCount = i + 1;
				bNoEnter = false;
                break;
			}
		}
		fHodeTime = iCount / 2500.0 * dwTime;					// 计算出能量保持时间
        fHodeTime = fHodeTime;

        // 曲线
        int iSel = 0;
        g_dCurveLength = 999.0;
        AnsiString strWrite = "", strData = "";
        for(int i = 0; i < 2500; i++)
        {
        	iSel = i*2/5;
            g_daCurveY[iSel] = faData[i];
            strData = FloatToStr(faData[i]).SetLength(6) + "\t";
            strWrite += strData;
            if((i + 1)%100 == 0)
            {
            	WriteLogFile(g_pFileVolt, strWrite + "\n");
                strWrite = "";
            }
        }
        Synchronize(FormMain->ViewWave);
        G_SDelay(1000);

		// 填表
        g_strItemValue = FloatToStr(fHodeTime);
		Synchronize(FormMain->WriteItemTestData);
	}

    // 采样结束后恢复初始设置
    g_daqDigit.SetSbRelay(g_iCurrBoard, 201, false);
    g_daqDigit.SetSbRelay(g_iCurrBoard, 202, false);
    g_daqDigit.SetSbRelay(g_iCurrBoard, 214, false);
	g_daqDigit.SetSbRelay(g_iCurrBoard, 264, false);
	g_gpibPcDMM.SetRST();
    g_daqDigit.EnableRelayBase(g_iCurrBoard, true);

    // ACU掉过电，故需要重新启动PMode模式
	bRet = g_comKline.OpenSMode();
	if(false == bRet)
    {
    	// SMode开启失败。
        g_strMark = g_comKline.m_strMark;
        Synchronize(FormMain->AddErrorByMark);
    }
}


void __fastcall TThreadMain::TestACUCurr()
{
	// A1: 判断条件是否选择
	g_iCurrItem = 72;	// based 0
    unsigned char	iACUTotal = g_STestInfo.m_iItem[g_iCurrItem];
	if ((g_STestInfo.m_iSelACU >> (g_iCurrBoard)) & 0x01 != 0x01)
	{
		g_iCurrItem = 72;
		return;
	}

    unsigned char cValue = (iACUTotal >> (g_iCurrBoard));
    cValue = cValue & 0x01;
    if (cValue != 0x01)
	{
		g_iCurrItem = 72;
		return;
	}

    // 界面及变量变化
    g_iCurveMode = 0;       // 多余
    FormMain->ResetErrorInfo(0);
	Synchronize(FormMain->ChangeItem);

//--START----------------------2009-08-13 修改-------------------------------------------//
        float fRes=0.0 ;
        int  iRep = 0;
        do
           {
               iRep++;
               g_daqDigit.EnableRelayBase(g_iCurrBoard, true);//
               G_SDelay(300);
               g_daqDigit.SetSbRelay(g_iCurrBoard, 266 ,true);	//get ign volt
               G_SDelay(300);
               g_gpibPcDMM.GetVolt(fRes);
          }
        while(fabs(fRes - g_fVoltTarget)>0.5  && iRep < 10 );
        g_daqDigit.SetSbRelay(g_iCurrBoard, 266 ,false);
//--END---------------------2009-08-13 修改--------------------------------------------//

    // A2：操作测试项
	g_daqDigit.SetSbRelay(g_iCurrBoard, 267, false);
    g_daqDigit.SetSbRelay(g_iCurrBoard, 267, true);
    G_SDelay(1000);
    float fCurr = 0.0;
    int iRepeat = 0;

    do
    {
        iRepeat++;
        g_gpibPcDMM.GetCurr(fCurr);

        // 填表
        g_strItemValue = FloatToStr(fCurr);
		Synchronize(FormMain->WriteItemTestData);
    }
    while (iRepeat < 1);
    g_daqDigit.SetSbRelay(g_iCurrBoard, 267, false);
}
void __fastcall TThreadMain::TestCrashOut()
{
	// A1: 判断条件是否选择
	g_iCurrItem = 73;	// based 0
	unsigned char	iACUTotal = g_STestInfo.m_iItem[g_iCurrItem];
	if ((g_STestInfo.m_iSelACU >> (g_iCurrBoard)) & 0x01 != 0x01)
	{
		g_iCurrItem = 73;
		return;
	}

	unsigned char	cValue = (iACUTotal >> (g_iCurrBoard));
	cValue = cValue & 0x01;
	if (cValue != 0x01)
	{
		g_iCurrItem = 73;
		return;
	}

    // 界面及变量变化
    g_iCurveMode = 1;
    FormMain->ResetErrorInfo(0);
    Synchronize(FormMain->ClearWave);
	Synchronize(FormMain->ChangeItem);

	// A2：操作测试项
	bool	bRet = false;
	short	iReadBuf[5000] = { 0 };
    unsigned int iRepeat = 0;
    double	dVoltBuf[1000] = {0.0};
	do
	{
		iRepeat++;
		g_daqDigit.SetSbRelay(g_iCurrBoard, 277, true);
		bRet = g_comKline.EnableCrashOutput();
        if(bRet == false)
    	{
            //--START----------------------2009-07-30 by anro修改-------------------------------------------//
            //确保ACU供电并在s-Mode中
            float fRes=0.0 ;
            int iRep = 0;
            do
            {
                iRep++;
                g_daqDigit.EnableRelayBase(g_iCurrBoard, true);//
                G_SDelay(300);
                g_daqDigit.SetSbRelay(g_iCurrBoard, 266 ,true);	//get ign volt
                G_SDelay(300);
                g_gpibPcDMM.GetVolt(fRes);
            }
            while(fabs(fRes - g_fVoltTarget)>0.5  && iRep < 10 );
            g_daqDigit.SetSbRelay(g_iCurrBoard, 266 ,false);
            G_SDelay(600);
            g_comKline.OpenSMode();
            //--END---------------------2009-07-30 修改--------------------------------------------//

        	g_daqDigit.EnableRelayBase(g_iCurrBoard, true);
            g_daqDigit.SetSbRelay(g_iCurrBoard, 277, true);		// 测WL1电压
            bool bPowerState = g_daqAnalog.GetPowerState();
            if(false == bPowerState)
            {
            	g_gpibPcPower.SetOutPut(true);
                g_strMark = "PcPower Err.!";
                Synchronize(FormMain->AddErrorByMark);
            }
            continue;
    	}

		g_daqAnalog.SetCrashConfig(iReadBuf);
        // Here to handle the data stored in ready buffer
        int j = 0;
		for (int i = 0; i < 5000; i++)
		{
			if (i % 5 == 0)
			{
				dVoltBuf[j] = ((double) (iReadBuf[i])) / 32768 * 10.0;
				j++;
			}
		}

        bRet = g_daqAnalog.GetCrashCheck(g_STestInfo.m_iDaqType, dVoltBuf);

        // 曲线
        g_dCurveLength = 999.0;
        AnsiString strWrite = "", strData = "";
        WriteLogFile(g_pFileVolt, "TestCrashOut:\n");
        for(int i = 0; i < 1000; i++)
        {
            g_daCurveY[i] = dVoltBuf[i];
            strData = FloatToStr(dVoltBuf[i]).SetLength(6) + "\t";
            strWrite += strData;
            if((i + 1)%100 == 0)
            {
            	WriteLogFile(g_pFileVolt, strWrite + "\n");
                strWrite = "";
            }
        }
        Synchronize(FormMain->ViewWave);
        G_SDelay(1000);

        // 填表
        if(true == bRet)	g_strItemValue = "ok";
        else 				g_strItemValue = "failed";
		Synchronize(FormMain->WriteItemTestData);
	}
	while (iRepeat < g_iItemRepeat);

    g_daqDigit.SetSbRelay(g_iCurrBoard, 277, false);
}

  /*
void __fastcall TThreadMain::TestCrashOut()
{
	// A1: 判断条件是否选择
	g_iCurrItem = 73;	// based 0
	unsigned char	iACUTotal = g_STestInfo.m_iItem[g_iCurrItem];
	if ((g_STestInfo.m_iSelACU >> (g_iCurrBoard)) & 0x01 != 0x01)
	{
		g_iCurrItem = 73;
		return;
	}

	unsigned char	cValue = (iACUTotal >> (g_iCurrBoard));
	cValue = cValue & 0x01;
	if (cValue != 0x01)
	{
		g_iCurrItem = 73;
		return;
	}

    // 界面及变量变化
    g_iCurveMode = 1;
    FormMain->ResetErrorInfo(0);
    Synchronize(FormMain->ClearWave);
	Synchronize(FormMain->ChangeItem);

	// A2：操作测试项
	bool	bRet = false;
	short	iReadBuf[5000] = { 0 };
    unsigned int iRepeat = 0;
    double	dVoltBuf[1000] = {0.0};
	do
	{
		iRepeat++;
		g_daqDigit.SetSbRelay(g_iCurrBoard, 277, true);
		bRet = g_comKline.EnableCrashOutput();
        if(bRet == false)
    	{
        	g_strSubTip = "Reset Relay and Communication to Current State.";
        	g_daqDigit.EnableRelayBase(g_iCurrBoard, true);
            g_daqDigit.SetSbRelay(g_iCurrBoard, 277, true);		// 测WL1电压
            g_comKline.OpenSMode();
            bool bPowerState = g_daqAnalog.GetPowerState();
            if(false == bPowerState)
            {
            	g_gpibPcPower.SetOutPut(true);
                g_strMark = "PcPower Err.!";
                Synchronize(FormMain->AddErrorByMark);
            }
            continue;
    	}

		g_daqAnalog.SetCrashConfig(iReadBuf);
        // Here to handle the data stored in ready buffer
        int j = 0;
		for (int i = 0; i < 5000; i++)
		{
			if (i % 5 == 0)
			{
				dVoltBuf[j] = ((double) (iReadBuf[i])) / 32768 * 10.0;
				j++;
			}
		}

        bRet = g_daqAnalog.GetCrashCheck(g_STestInfo.m_iDaqType, dVoltBuf);

        // 曲线
        g_dCurveLength = 999.0;
        AnsiString strWrite = "", strData = "";
        WriteLogFile(g_pFileVolt, "TestCrashOut:\n");
        for(int i = 0; i < 1000; i++)
        {
            g_daCurveY[i] = dVoltBuf[i];
            strData = FloatToStr(dVoltBuf[i]).SetLength(6) + "\t";
            strWrite += strData;
            if((i + 1)%100 == 0)
            {
            	WriteLogFile(g_pFileVolt, strWrite + "\n");
                strWrite = "";
            }
        }
        Synchronize(FormMain->ViewWave);
        G_SDelay(1000);

        // 填表
        if(true == bRet)	g_strItemValue = "ok";
        else 				g_strItemValue = "failed";
		Synchronize(FormMain->WriteItemTestData);
	}
	while (iRepeat < g_iItemRepeat);

    g_daqDigit.SetSbRelay(g_iCurrBoard, 277, false);
}  */

bool __fastcall TThreadMain::NeedTestHasFault (int iBoard)
{
	for (int iItem = 0; iItem < MES_ITEM_RET; iItem++)
	{
		int iItemSel = (g_STestInfo.m_iItem[iItem] >> iBoard) & 0x01;
		if (1 == iItemSel) return true;
	}

	return false;
}

bool __fastcall TThreadMain::NeedTestShortToVBatt(int iBoard)
{
    for (int iItem = 30; iItem < 39; iItem++)
	{
		int iItemSel = (g_STestInfo.m_iItem[iItem] >> iBoard) & 0x01;
		if (1 == iItemSel) return true;
	}

	return false;
}

// 若读取的数据数为零，值得考虑
// 传递数据的转换
bool __fastcall TThreadMain::GetFaultCodeState(unsigned int iFaultCode, bool & bKline)
{
	unsigned char   cReadData[1024] = {0};
    unsigned char	cFaultCode = iFaultCode;
    unsigned int    iReadBytes = 0, iRepeat = 0;
    int				iRet = 0;

    do
    {
    	iRepeat++;
    	iRet = g_comKline.SearchChar(cFaultCode, cReadData, iReadBytes);
        if(iRet == MES_KLINE_ZERO)
        {
        	g_strSubTip = "Reset Relay and Communication to Current State.";
        	g_daqDigit.EnableRelayBase(g_iCurrBoard, true);
            g_comKline.OpenSMode();
            g_comKline.OpenRealtimeFault();
            g_daqDigit.EnableRelayGroup(g_iCurrItem, MES_FIND);
            bool bPowerState = g_daqAnalog.GetPowerState();
            if(false == bPowerState)
            {
            	g_gpibPcPower.SetOutPut(true);
                g_strMark = "PcPower Err.!";
                Synchronize(FormMain->AddErrorByMark);
            }
        }
    }
    while(iRepeat <= g_iFindRepeat && iRet == MES_KLINE_ZERO);

    if(iRet == MES_KLINE_ZERO)
    {
        g_strMark = g_comKline.m_strMark;
        bKline = false;
        return false;
    }

    for (unsigned int i = 0; i < iReadBytes; i++)
    {
        if (cReadData[i] != cFaultCode)
        {
            // 填充错误数组和显示
            g_iCurrFaultCode = cReadData[i];
            Synchronize(FormMain->AddErrorByCode);
        }
    }

    bKline = true;
    if(MES_KLINE_FOUND == iRet) return true;
    return false;
}

// iBlock: 0--FC、1--SBelt 2--SIS
bool __fastcall TThreadMain::GetFaultCodeStateByItem(int iMethod, float fRes, unsigned int iFaultCode, bool & bKline, int iBlock)
{
	if(iBlock == MES_TYPE_FC)
    {
        if (iMethod < 2)
        {
            g_strCurveUnit = "ohm";
            g_gpibPcResF.SetRes(fRes);
        }
        else
        {
            g_strCurveUnit = "Kohm";
            g_gpibPcResS.SetRes(fRes);
        }
    }
    else if(iBlock == MES_TYPE_SBELT)
    {
        g_strCurveUnit = "Kohm";
        g_gpibPcResS.SetRes(fRes);
    }
    else if(iBlock == MES_TYPE_SIS)
    {
        g_strCurveUnit = "ohm";
        g_gpibPcResS.SetRes(fRes);
        G_SDelay(100);
    }
/*    String strOut = "";
    strOut = "Board:" + IntToStr(g_iCurrBoard) + ":\t";
    strOut = "TVCond:" + IntToStr(g_iTVCond) + ":\t";
    strOut = strOut + "Item:" + IntToStr(g_iCurrItem) + ":\t";
    strOut = strOut + "FaultCode Dec:" + IntToStr(iFaultCode) + ":\t";
    strOut = strOut + "Hex:" + IntToHex((int)iFaultCode, 2) + ":\t";
    strOut = strOut + "Res:" + FloatToStr(fRes) + ":\t\n";
    WriteLogFile(g_pFileComKline, strOut);

    // 清曲线
    bool bFind = false;
    g_fCurveCurrY = fRes;
    g_strCurrSet = FloatToStr(fRes) + "ohm";
    Synchronize(FormMain->ViewWave);
    Synchronize(FormMain->ViewLabelCurrSet);*/
	//以上无用 mcl
    bFind = GetFaultCodeState(iFaultCode, bKline);
    return bFind;
}

bool __fastcall TThreadMain::GetFaultCodeStateByVolt (int iMethod, float fVolt, unsigned int iFaultCode, bool & bKline)
{
	g_gpibPcPower.SetVolt(fVolt);

	String	strOut = "";
	strOut = IntToStr(g_iCurrBoard) + ":\t";
	strOut = strOut + IntToStr(g_iCurrItem) + ":\t";
	strOut = strOut + IntToStr(iFaultCode) + ":\t";
	strOut = strOut + IntToHex((int)iFaultCode, 2) + ":\t";
	strOut = strOut + FloatToStr(fVolt) + ":\t\n";
	WriteLogFile(g_pFileComKline, strOut);

	// 清曲线
	bool bFind = false;
    g_fCurveCurrY = fVolt;
    g_strCurrSet = FloatToStr(fVolt) + "v";
    Synchronize(FormMain->ViewWave);
    Synchronize(FormMain->ViewLabelCurrSet);
    bFind = GetFaultCodeState(iFaultCode, bKline);
    return bFind;
}

void __fastcall TThreadMain::ShowTip()
{
    ShowMessage(g_strMark);
}

void __fastcall TThreadMain::ViewProgressInfo(bool bPos, int iPos)
{
	g_bProPos = bPos;
    g_iProPos = iPos;
	Synchronize(FormMain->ViewProgressInfo);
}

void __fastcall TThreadMain::SetTempByState(int iTempState)
{
	// 对温箱操作
	// 温箱温度设定
	g_strSubTip = "温箱温度设定到1度误差.";
	g_comChamber.SetTemp(g_fTempTarget);
	g_bViewChamber = true;
	g_bViewTest = false;
	ViewProgressInfo(false, 4);

	// 显示理论温度曲线
	Synchronize(FormMain->ViewWaveTempTheory);

	// OnTimer
	g_bTempReal = true;
	g_fTempCurveX = 10.00;
	g_bViewChamber = true;
	g_bViewTest = false;

	// 温箱等待到1度误差
	g_comChamber.Wait(g_fTempTarget);

	// 温箱等待
	g_strSubTip = "温箱等待，使温度稳定.";
	switch (iTempState)
	{
		case 0:		// LT
			G_MDelay(StrToInt(g_STestInfo.m_strDelay[0]) * 60 * 1000); break;
		case 1:		// NT
			G_MDelay(StrToInt(g_STestInfo.m_strDelay[1]) * 60 * 1000); break;
		case 2:		// HT
			G_MDelay(StrToInt(g_STestInfo.m_strDelay[2]) * 60 * 1000); break;
		default:	break;
	}

	ViewProgressInfo(false, 8);

	g_bViewChamber = false;
	g_bViewTest = true;
}


bool __fastcall TThreadMain::SelfCheckDevice()
{
	bool bRet = false;
    ///////////////////////////////////////////////////////////////////////////
    // 电源供应
    bRet = g_daqAnalog.GetPowerState();
    if(false == bRet)
    {
    	g_strMark = "Please Check Power Supply.!";
    	Synchronize(ShowTip);
        return false;
    }

    ///////////////////////////////////////////////////////////////////////////
    // 电阻箱检查
    bRet = SelfCheckPcRes();
    if(false == bRet)
    {
    	g_strMark = "Please Check PcRes State(Remote/Local).!";
    	Synchronize(ShowTip);
        return false;
    }

    return true;
    ///////////////////////////////////////////////////////////////////////////
    // 电源检查
/*    String strState = "";
    bRet = g_gpibPcPower.GetState(strState);
    if(false == bRet)
    {
        g_strMark = "Please Check PcPower State(Err).!";
        Synchronize(ShowTip);
        return false;
    }

    return true;
*/
}

// 自检电阻箱1、2到Remote
bool __fastcall TThreadMain::SelfCheckPcRes()
{
	int		iRepeat = 0;
	float	faRes[] = { 10.0, 100.0, 1000.0, 10000.0 };
    float 	fResRet = 0.0;
	do
	{
		// 设置电阻
		g_daqDigit.SetSbRelay(g_iCurrBoard, 217, false);
		g_daqDigit.SetSbRelay(g_iCurrBoard, 216, false);
		g_daqDigit.SetSbRelay(g_iCurrBoard, 215, false);
		g_daqDigit.SetSbRelay(g_iCurrBoard, 214, false);
		if (iRepeat % 2 == 0)
		{
			g_gpibPcResS.SetRes(faRes[iRepeat/2]);
		} else
		{
			g_gpibPcResF.SetRes(faRes[iRepeat/2]);
		}

		// 测电阻
		if (iRepeat % 2 == 0)
		{
			g_daqDigit.SetSbRelay(g_iCurrBoard, 217, false);
			g_daqDigit.SetMbRelay(300, false);
			g_daqDigit.SetMbRelay(302, false);
			g_daqDigit.SetSbRelay(g_iCurrBoard, 284, true);
			g_daqDigit.SetSbRelay(g_iCurrBoard, 285, true);
		} else
		{
			g_daqDigit.SetSbRelay(g_iCurrBoard, 217, true);
			g_daqDigit.SetSbRelay(g_iCurrBoard, 216, true);
			g_daqDigit.SetSbRelay(g_iCurrBoard, 215, false);
			g_daqDigit.SetSbRelay(g_iCurrBoard, 214, true);
		}

		g_gpibPcDMM.GetFRes(fResRet);
        if((fabs(fResRet - faRes[iRepeat/2])/faRes[iRepeat/2] > 0.4))
        	return false;
        iRepeat++;
	}
	while (iRepeat <= 7);

	g_daqDigit.SetSbRelay(g_iCurrBoard, 284, false);
	g_daqDigit.SetSbRelay(g_iCurrBoard, 285, false);
	g_daqDigit.SetSbRelay(g_iCurrBoard, 217, false);
	g_daqDigit.SetSbRelay(g_iCurrBoard, 216, false);
	g_daqDigit.SetSbRelay(g_iCurrBoard, 214, false);
 	return true;
}

bool __fastcall TThreadMain::ResetDevice()
{
	bool	bRet = false;
    g_bVoltReal = false;
    g_bTempReal = false;

    // 关闭设备到初始位置
	bRet = CloseDevice();
	if (false == bRet)
	{
    	Synchronize(ShowTip);
		return false;
	}

    // 开启设备到合适位置
	if (false == g_bAllDevice)
	{
		bRet = OpenDevice();
		if (false == bRet)
        {
        	Synchronize(ShowTip);
        	return false;
        }
	}

    // 继电器重置
	g_daqDigit.ResetRelay();
    // 温度设置
    //if (1 == g_STestInfo.m_iChamType)
    //	SetTempByState(g_iTempPhase);
	float	fTargetVolt = StrToFloat(g_STestInfo.m_strVolt[g_iVoltPhase]);
    // 设定电压
    g_bVoltReal = false;
	g_gpibPcPower.SetVolt(fTargetVolt);
    // 电源输出设定
    g_gpibPcPower.SetOutPut(true);
    bool bState = false;
    for(int i = 0; i < 10; i++)
    {
    	g_gpibPcPower.GetOutPutState(bState);
    	if(true == bState)	break;
    	g_gpibPcPower.SetOutPut(true);

    }
    bState = SelfCheckDevice();
    if(false == bState)
    {
    	g_strMark = "Volt State Error";
    	Synchronize(ShowTip);
    }
    g_iVoltCount = 1;
    g_bVoltReal = true;

	return true;
}
