#include <vcl.h>
#pragma hdrstop

#include "FormManualU.h"
#include "FormMainU.h"
#include "ComChamber.h"
#include "ComKline.h"
#include "GpibPcPower.h"
#include "GpibPcDMM.h"
#include "GpibPcRes.h"
#include "DaqDigit.h"
#include "DaqAnalog.h"

#pragma package(smart_init)

#pragma resource "*.dfm"

TFormManual *FormManual;

__fastcall TFormManual::TFormManual(TComponent *Owner) :
	TForm(Owner)
{
	FILE	*pFile = NULL;
	pFile = fopen((g_strAppPath + "\\config\\Control.txt").c_str(), "r");
    // 文件不存在，置为缺省值
	if (pFile == NULL)
	{
		ShowMessage("Unable to open Control file to get Control Text!");
		for (int k = 0; k < MES_ITEM_REAL; k++)
		{
			m_caControl[k] = " ";
            m_caControl[k + 1] = " ";
		}

		return;
	}

    char	strTemp[512] = { '\0' }, *pCh = NULL;
	int		iItemMain = 0, iItemSub = 0, k = 0;
    String	strItemMain = "", strItemSub = "", strContext = "", strRow = "";
    bool	bEnter = false;

	pCh = fgets(strTemp, 512, pFile);
	while (pCh != NULL)
	{
    	k = 0;
    	while (strTemp[k] != '\n' && (k < 512))
        {
				strRow = strRow + strTemp[k];
				k++;
        }
      	if(strTemp[0] == '[')
        {
        	if(true == bEnter)
            {
            	int iIndex = iItemMain*2 + iItemSub;
             	m_caControl[iIndex] = strContext.c_str();
            }
        	strItemMain = "";
        	for(int i = 1; i < 3; i++)
            {
        		strItemMain = strItemMain + strTemp[i];
        	}
            strItemSub = strTemp[5];
            iItemMain = StrToInt(strItemMain);
            iItemSub = StrToInt(strItemSub);
            strContext = strTemp;
            bEnter = true;
        }
        else
        {
			strRow = strTemp;
            strContext = strContext + strRow;
        }

        strRow = "";
		for (int k = 0; k < 512; k++)
        {
        	strTemp[k] = '\0';
        }
        // 缓冲区不用清空，遇到第一个\0就停止赋值
		pCh = fgets(strTemp, 512, pFile);
	}

	fclose(pFile);
}

void __fastcall TFormManual::FormCreate(TObject *Sender)
{
    EnablePcPowerState(true);
    EnablePcResState(true);
    EnablePcDMMState(true);
    EnableChamberState(true);
    EnableKlineState(true);
    EnableRelayState(true);
}

void __fastcall TFormManual::FormClose(TObject *Sender, TCloseAction &Action)
{
	bool bRet = false;
    bRet = CloseDevice();
    if(false == bRet) 	ShowMessage("CloseDevice failed.!");

    FormMain->SetStateCommonality(true);
    FormMain->m_pBtnInit->Enabled = false;
    FormMain->m_pBtnRelay->Enabled = false;
    FormMain->SetStateManual(true);
}

///////////////////////////////////////////////////////////////////////////////
// 程控电源模块
///////////////////////////////////////////////////////////////////////////////
void __fastcall TFormManual::OnBtnPcPowerOpenClick(TObject *Sender)
{
	ShowMessage("TFormManual::OnBtnPcPowerOpenClick");
}

void __fastcall TFormManual::OnBtnPcPowerCloseClick(TObject *Sender)
{
	ShowMessage("TFormManual::OnBtnPcPowerCloseClick");
}

void __fastcall TFormManual::OnBtnPcPowerIDNClick(TObject *Sender)
{
	ShowMessage("TFormManual::OnBtnPcPowerIDNClick");
}

void __fastcall TFormManual::OnBtnPcPowerRSTClick(TObject *Sender)
{
	ShowMessage("TFormManual::OnBtnPcPowerRSTClick");
}

void __fastcall TFormManual::OnBtnPcPowerTSTClick(TObject *Sender)
{
	ShowMessage("TFormManual::OnBtnPcPowerTSTClick");
}

void __fastcall TFormManual::OnBtnPcPowerWriteClick(TObject *Sender)
{
	ShowMessage("TFormManual::OnBtnPcPowerWriteClick");
}

void __fastcall TFormManual::OnBtnPcPowerReadClick(TObject *Sender)
{
	ShowMessage("TFormManual::OnBtnPcPowerReadClick");
}

void __fastcall TFormManual::OnBtnVoltClick(TObject *Sender)
{
    if(m_pEdtVolt->Text.IsEmpty() == true)
    {
    	ShowMessage("Please Set the PcPower Voltage First!");
    	return;
    }
	float fVoltTarget = StrToFloat(m_pEdtVolt->Text);
	bool bRet = false;
    EnablePcPowerState(false);
    bRet = g_gpibPcPower.SetVolt(fVoltTarget);
    EnablePcPowerState(true);
    if(false == bRet)
    {
    	ShowMessage("Set the PcPower Voltage Fail!");
    }
}

void __fastcall TFormManual::OnBtnCurrClick(TObject *Sender)
{
	if(m_pEdtCurr->Text.IsEmpty() == true)
    {
    	ShowMessage("Please Set the PcPower Current First!");
    	return;
    }
	float fCurrTarget = StrToFloat(m_pEdtCurr->Text);
	bool bRet = false;
    EnablePcPowerState(false);
    bRet = g_gpibPcPower.SetCurr(fCurrTarget);
    EnablePcPowerState(true);
    if(false == bRet)
    {
    	ShowMessage("Set the PcPower Current Fail!");
    }
}

void __fastcall TFormManual::OnBtnOutputClick(TObject *Sender)
{
    if (m_pCmbOutput->ItemIndex == -1)
	{
		ShowMessage("Please select Output Mode first!");
		return;
	}

	int		iIndex = m_pCmbOutput->ItemIndex;
    bool	bEnable = true;
    if(iIndex != 0)	bEnable = false;
    EnablePcPowerState(false);
	bool	bRet = g_gpibPcPower.SetOutPut(bEnable);
    EnablePcPowerState(true);
	if (false == bRet)
	{
		ShowMessage("Operate the PcPower Output Fail!");
		return;
	}
}

void __fastcall TFormManual::OnBtnOCPClick(TObject *Sender)
{
	if (m_pCmbOCP->ItemIndex == -1)
	{
		ShowMessage("Please select Output Mode first!");
		return;
	}

	int		iIndex = m_pCmbOCP->ItemIndex;
    bool	bEnable = true;
    if(iIndex != 0)	bEnable = false;
    EnablePcPowerState(false);
	bool	bRet = g_gpibPcPower.SetOCP(bEnable);
    EnablePcPowerState(true);
	if (false == bRet)
	{
		ShowMessage("Operate the PcPower Output Fail!");
		return;
	}
}

void __fastcall TFormManual::OnBtnOVPClick(TObject *Sender)
{
	if (m_pCmbOVP->ItemIndex == -1)
	{
		ShowMessage("Please select Output Mode first!");
		return;
	}

	int		iIndex = m_pCmbOVP->ItemIndex;
    bool	bEnable = true;
    if(iIndex != 0)	bEnable = false;
    EnablePcPowerState(false);
	bool	bRet = g_gpibPcPower.SetOVP(bEnable);
    EnablePcPowerState(true);
	if (false == bRet)
	{
		ShowMessage("Operate the PcPower Output Fail!");
		return;
	}
}

///////////////////////////////////////////////////////////////////////////////
// 电阻箱模块
///////////////////////////////////////////////////////////////////////////////
void __fastcall TFormManual::OnBtnPcResOpenClick(TObject * Sender)
{
	bool bRet = false;
    EnablePcResState(false);
    bRet = g_gpibPcResF.Init(g_gpibInterface, 16);
    EnablePcResState(true);
    if(false == bRet)
    {
    	ShowMessage("Init GPIB Device PcResF One Failed.!!");
        return ;
    }
    EnablePcResState(false);
    bRet = g_gpibPcResF.SetRST();
    EnablePcResState(true);
    if(false == bRet)
    {
    	ShowMessage("SetRST GPIB Device PcResF Failed.!!");
        return ;
    }

    EnablePcResState(false);
    bRet = g_gpibPcResS.Init(g_gpibInterface, 17);
    EnablePcResState(true);
    if(false == bRet)
    {
    	ShowMessage("Init GPIB Device PcPower Two Failed.!!");
        return ;
    }
    EnablePcResState(false);
    bRet = g_gpibPcResS.SetRST();
    EnablePcResState(true);
    if(false == bRet)
    {
    	ShowMessage("SetRST GPIB Device PcResS Failed.!!");
        return ;
    }
}
void __fastcall TFormManual::OnBtnPcResCloseClick(TObject *Sender)
{
	bool bRet = false;
	bRet = g_gpibPcResF.SetRST();
	if (false == bRet)
	{
		ShowMessage("SetRST GPIB Device PcResF Failed.!!");
		return ;
	}

	bRet = g_gpibPcResF.Close();
	if (false == bRet)
	{
		ShowMessage("Close GPIB Device PcResF One Failed.!!");
		return ;
	}

	bRet = g_gpibPcResS.SetRST();
	if (false == bRet)
	{
		ShowMessage("SetRST GPIB Device PcResS Failed.!!");
		return ;
	}

	bRet = g_gpibPcResS.Close();
	if (false == bRet)
	{
		ShowMessage("Close GPIB Device PcResS Two Failed.!!");
		return ;
	}
}

void __fastcall TFormManual::OnBtnPcResIDNClick(TObject *Sender)
{
	ShowMessage("TFormManual::OnBtnPcResIDNClick");
}

void __fastcall TFormManual::OnBtnPcResRSTClick(TObject *Sender)
{
	ShowMessage("TFormManual::OnBtnPcResRSTClick");
}

void __fastcall TFormManual::OnBtnPcResTSTClick(TObject *Sender)
{
	ShowMessage("TFormManual::OnBtnPcResTSTClick");
}

void __fastcall TFormManual::OnBtnPcResWriteClick(TObject *Sender)
{
	ShowMessage("TFormManual::OnBtnPcResWriteClick");
}

void __fastcall TFormManual::OnBtnPcResReadClick(TObject *Sender)
{
	ShowMessage("TFormManual::OnBtnPcResReadClick");
}

void __fastcall TFormManual::OnBtnSetResClick(TObject *Sender)
{
	ShowMessage("TFormManual::FormClose");
    if (m_pCmbResBox->ItemIndex == -1)
	{
		ShowMessage("Please select Resistance Box first!");
		return;
	}

    if(m_pEdtRes->Text.IsEmpty() == true)
    {
    	ShowMessage("Please Set the PcRes Resistance First!");
    	return;
    }

    float fCurrRes = StrToFloat(m_pEdtRes->Text);
	bool bRet = false;
    EnablePcResState(false);
    if(m_pCmbResBox->ItemIndex == 0)		bRet = g_gpibPcResF.SetRes(fCurrRes);
    else if(m_pCmbResBox->ItemIndex == 0)	bRet = g_gpibPcResS.SetRes(fCurrRes);
    EnablePcResState(true);
    if(false == bRet)
    {
    	ShowMessage("Set the PcRes Resistance Fail!");
    }
}

///////////////////////////////////////////////////////////////////////////////
// 数字万用表模块
///////////////////////////////////////////////////////////////////////////////
void __fastcall TFormManual::OnBtnPcDMMOpenClick(TObject * Sender)
{
    bool bRet = false;
    EnablePcDMMState(false);
    bRet = g_gpibInterface.SetHandle(0);
    if(false == bRet)
    {
    	ShowMessage("Open GPIB Interface Failed.!!");
        return ;
    }
    bRet = g_gpibPcResS.Init(g_gpibInterface, 17);
    if(false == bRet)
    {
    	ShowMessage("Init GPIB Device PcPower Two Failed.!!");
        return ;
    }
    EnablePcDMMState(true);
    m_pBtnPcDMMOpen->Enabled = false;
    if(false == bRet)
    {
    	ShowMessage("Open the ComChamber Fail!");
        m_pBtnChamberOpen->Enabled = true;
    }
}
void __fastcall TFormManual::OnBtnPcDMMCloseClick(TObject *Sender)
{
	bool	bRet = false;
	EnablePcDMMState(false);
	bRet = g_gpibPcDMM.SetRST();
	if (false == bRet)
	{
		ShowMessage("SetRST GPIB Device PcDMM Failed.!!");
		return ;
	}

	bRet = g_gpibPcDMM.Close();
	if (false == bRet)
	{
		ShowMessage("Close GPIB Device PcDMM Failed.!!");
		return ;
	}

	EnablePcDMMState(true);
}


void __fastcall TFormManual::OnBtnPcDMMIDNClick(TObject *Sender)
{
	ShowMessage("TFormManual::OnBtnPcDMMIDNClick");
}

void __fastcall TFormManual::OnBtnPcDMMRSTClick(TObject *Sender)
{
	ShowMessage("TFormManual::OnBtnPcDMMRSTClick");
}

void __fastcall TFormManual::OnBtnPcDMMTSTClick(TObject *Sender)
{
	ShowMessage("TFormManual::OnBtnPcDMMTSTClick");
}

void __fastcall TFormManual::OnBtnPcDMMWriteClick(TObject *Sender)
{
	ShowMessage("TFormManual::OnBtnPcDMMWriteClick");
}

void __fastcall TFormManual::OnBtnPcDMMReadClick(TObject *Sender)
{
	ShowMessage("TFormManual::OnBtnPcDMMReadClick");
}

void __fastcall TFormManual::OnBtnGetVoltClick(TObject *Sender)
{
    bool	bRet = false;
    float 	fVolt = 0.0;
	EnablePcDMMState(false);
	bRet = g_gpibPcDMM.GetVolt(fVolt);
    EnablePcDMMState(true);
	if (false == bRet)
	{
		ShowMessage("Get the Volt Failed.!!");
		return ;
	}
	m_pEdtPcDMMVolt->Text = FloatToStr(fVolt);
}

void __fastcall TFormManual::OnBtnGetCurrClick(TObject *Sender)
{
	bool	bRet = false;
    float 	fCurr = 0.0;
	EnablePcDMMState(false);
	bRet = g_gpibPcDMM.GetVolt(fCurr);
    EnablePcDMMState(true);
	if (false == bRet)
	{
		ShowMessage("Get the Curr Failed.!!");
		return ;
	}
	m_pEdtPcDMMCurr->Text = FloatToStr(fCurr);
}

void __fastcall TFormManual::OnBtnGetFResClick(TObject *Sender)
{
	bool	bRet = false;
    float 	fRes = 0.0;
	EnablePcDMMState(false);
	bRet = g_gpibPcDMM.GetFRes(fRes);
    EnablePcDMMState(true);
	if (false == bRet)
	{
		ShowMessage("Get the Resistance Failed.!!");
		return ;
	}
	m_pEdtPcDMMFRes->Text = FloatToStr(fRes);
}

void __fastcall TFormManual::OnBtnGetResClick(TObject *Sender)
{
	bool	bRet = false;
    float 	fRes = 0.0;
	EnablePcDMMState(false);
	bRet = g_gpibPcDMM.GetRes(fRes);
    EnablePcDMMState(true);
	if (false == bRet)
	{
		ShowMessage("Get the Resistance Failed.!!");
		return ;
	}
	m_pEdtPcDMMRes->Text = FloatToStr(fRes);
}

void __fastcall TFormManual::OnBtnGetFreqClick(TObject *Sender)
{
	bool	bRet = false;
    float 	fReq = 0.0;
	EnablePcDMMState(false);
	bRet = g_gpibPcDMM.GetFreq(fReq);
    EnablePcDMMState(true);
	if (false == bRet)
	{
		ShowMessage("Get the Frequnce Failed.!!");
		return ;
	}
	m_pEdtPcDMMFreq->Text = FloatToStr(fReq);
}

///////////////////////////////////////////////////////////////////////////////
// 通过串口操作温箱模块
///////////////////////////////////////////////////////////////////////////////
void __fastcall TFormManual::OnBtnChamberOpenClick(TObject * Sender)
{
    bool bRet = false;
    EnableChamberState(false);
    bRet = g_comChamber.Open();
    EnableChamberState(true);
    m_pBtnChamberOpen->Enabled = false;
    if(false == bRet)
    {
    	ShowMessage("Open the ComChamber Fail!");
        m_pBtnChamberOpen->Enabled = true;
    }
}
void __fastcall TFormManual::OnBtnChamberCloseClick(TObject *Sender)
{
	bool bRet = false;
    EnableChamberState(false);
    bRet = g_comChamber.Close();
    EnableChamberState(true);
    m_pBtnChamberClose->Enabled = false;
    if(false == bRet)
    {
    	ShowMessage("Open the ComChamber Fail!");
        m_pBtnChamberClose->Enabled = true;
    }
}

void __fastcall TFormManual::OnBtnSetTempClick(TObject *Sender)
{
	if(m_pEdtTemp->Text.IsEmpty() == true)	return;
	float fTempTarget = StrToFloat(m_pEdtTemp->Text);
    m_pPanelTempTarget->Caption = m_pEdtTemp->Text;
	bool bRet = false;
    EnableChamberState(false);
    bRet = g_comChamber.SetTemp(fTempTarget);
    EnableChamberState(true);
    if(false == bRet)
    {
    	ShowMessage("Set the ComChamber Temppreture Fail!");
    }
}

void __fastcall TFormManual::OnBtnGetTempClick(TObject *Sender)
{
	float fTempCurr = 0.0;
	bool bRet = false;
    EnableChamberState(false);
    bRet = g_comChamber.ReadTemp(fTempCurr);
    EnableChamberState(true);
    if(false == bRet)
    {
    	ShowMessage("Get the ComChamber Temppreture Fail!");
        return;
    }
	m_pEdtTempCurr->Text = FloatToStr(fTempCurr);
}

void __fastcall TFormManual::OnBtnWaitTempClick(TObject *Sender)
{
	float fTempCurr = 0.0;
	bool bRet = false;
    EnableChamberState(false);
    //bRet = g_comChamber.
    EnableChamberState(true);
    if(false == bRet)
    {
    	ShowMessage("Get the ComChamber Temppreture Fail!");
        return;
    }
	m_pEdtTempCurr->Text = FloatToStr(fTempCurr);
}

void __fastcall TFormManual::OnBtnChamberStopClick(TObject *Sender)
{
	bool bRet = false;
	EnableChamberState(false);
    bRet = g_comChamber.Stop();
    EnableChamberState(true);
    if(false == bRet)
    {
    	ShowMessage("Stop the ComChamber Operation Fail!");
        return;
    }
}

///////////////////////////////////////////////////////////////////////////////
// 通过串口操作ECU模块
///////////////////////////////////////////////////////////////////////////////
void __fastcall TFormManual::OnBtnKlineOpenClick(TObject * Sender)
{
    bool bRet = false;
    EnableKlineState(false);
    bRet = g_comKline.Open();
    EnableKlineState(true);
    m_pBtnKlineOpen->Enabled = false;
    if(false == bRet)	ShowMessage("Open the ComKline Fail!");
}
void __fastcall TFormManual::OnBtnKlineCloseClick(TObject *Sender)
{
	bool bRet = false;
    EnableKlineState(false);
    bRet = g_comKline.Close();
    EnableKlineState(true);
    m_pBtnKlineClose->Enabled = false;
    if(false == bRet)	ShowMessage("Open the ComKline Fail!");
}

void __fastcall TFormManual::OnBtnExeClick(TObject *Sender)
{
	unsigned char	caBufRead[100] = { 0 };
	int				iSel = m_pCmbCommand->ItemIndex;
	if (iSel == -1)
	{
		ShowMessage("Please select the function.!");
		return;
	}

	bool	bRet = false;
	EnableKlineState(false);
	if (iSel >= 0 && iSel <= 19 && iSel != 2)
	{
		bRet = g_comKline.ExecuteCommand(m_pCmbCommand->ItemIndex, caBufRead);
	}

	switch (iSel)
	{
		case 2:		bRet = g_comKline.CloseRealtimeFault(); break;
		case 20:	bRet = g_comKline.OpenSMode(); break;
		case 21:	bRet = g_comKline.ExecuteCommand(2, caBufRead); break;  // bRet = g_comKline.CloseSMode();
		case 22:	bRet = g_comKline.WriteEEPROM(MES_ADDRESS, MES_BYTES, g_strMark); break;
		case 23:	bRet = g_comKline.ReadEEPROM(MES_ADDRESS, MES_BYTES, g_strMark); break;
		case 24:	bRet = g_comKline.WriteFRAM(MES_ADDRESS, MES_BYTES, g_strMark); break;
		case 25:	bRet = g_comKline.ReadFRAM(MES_ADDRESS, MES_BYTES, g_strMark); break;
		default:	break;
	}
    G_SDelay(1000);
	EnableKlineState(true);
	if (false == bRet) ShowMessage("Open the ComKline Fail!");
}

///////////////////////////////////////////////////////////////////////////////
// 通过采集卡操作继电器模块
///////////////////////////////////////////////////////////////////////////////
void __fastcall TFormManual::OnBtnDaqOpenClick(TObject * Sender)
{
	bool	bRet = false;
    EnableRelayState(false);
	bRet = g_daqDio.Open();
    EnableRelayState(true);
	if (false == bRet)
	{
		ShowMessage("Open PIO port Failed.!!");
		return;
	}
	g_daqDigit.SetParentObjectHandle();
	g_daqAnalog.SetParentObjectHandle();
}

void __fastcall TFormManual::OnBtnDaqCloseClick(TObject *Sender)
{
	bool	bRet = false;
	// 1-D2KDASK:采集卡、继电器
    EnableRelayState(false);
	bRet = g_daqDigit.ResetRelay();
	if (false == bRet)
	{
		ShowMessage("Reset Relay Failed.!!");
        EnableRelayState(true);
		return;
	}

	bRet = g_daqDio.Close();
    EnableRelayState(true);
	if (false == bRet)
	{
		ShowMessage("Close PIO port Failed.!!");
		return;
	}
}

void __fastcall TFormManual::OnBtnMbOnClick(TObject * Sender)
{
	if (m_pCmbMbRelay->ItemIndex == -1)
	{
		ShowMessage("Please select the Relay on the MainBoard first!");
		return;
	}

	int		iIndex = m_pCmbMbRelay->ItemIndex;
    EnableRelayState(false);
	bool	bRet = g_daqDigit.SetMbRelay(300 + iIndex, true);
    EnableRelayState(true);
	if (false == bRet)
	{
		ShowMessage("Open the Mainboard Relay Fail!");
		return;
	}

	ViewStatusInMainboard();
}
void __fastcall TFormManual::OnBtnMbOffClick(TObject *Sender)
{
	if (m_pCmbMbRelay->ItemIndex == -1)
	{
		ShowMessage("Please select the Relay on the MainBoard first!");
		return;
	}

	int		iIndex = m_pCmbMbRelay->ItemIndex;
    EnableRelayState(false);
	bool	bRet = g_daqDigit.SetMbRelay(300 + iIndex, false);
    EnableRelayState(true);
	if (false == bRet)
	{
		ShowMessage("Open the Mainboard Relay Fail!");
		return;
	}

	ViewStatusInMainboard();
}

void __fastcall TFormManual::OnBtnMbTestClick(TObject *Sender)
{
	// 母板上的三个继电器
	for(int i = 300; i < 303; i++)
    {
    	EnableRelayState(false);
    	g_daqDigit.SetMbRelay(i, true);
        EnableRelayState(true);
        ViewStatusInMainboard();
        this->Update();

        EnableRelayState(false);
        g_daqDigit.SetMbRelay(i, false);
        EnableRelayState(true);
        ViewStatusInMainboard();
        this->Update();
    }
}

void __fastcall TFormManual::OnBtnSbOnClick(TObject *Sender)
{
	if (m_pCmbCardNum->ItemIndex == -1 || m_pCmbSbRelay->ItemIndex == -1)
	{
		ShowMessage("Please select the Relay on the SubBoard first!");
		return;
	}

	String	strCard = m_pCmbCardNum->Text;
	String	strRelay = m_pCmbSbRelay->Text;
	int			iCard = atoi(strCard.c_str());
	int			iRelay = atoi(strRelay.c_str());
    EnableRelayState(false);
	bool		bRet = g_daqDigit.SetSbRelay(iCard, iRelay, true);
    EnableRelayState(true);
	if (false == bRet)
	{
		ShowMessage("Open the Mainboard Relay Fail!");
		return;
	}

	ViewStatusInGrid(iCard);
}

void __fastcall TFormManual::OnBtnSbOffClick(TObject *Sender)
{
	if (m_pCmbCardNum->ItemIndex == -1 || m_pCmbSbRelay->ItemIndex == -1)
	{
		ShowMessage("Please select the Relay on the SubBoard first!");
		return;
	}

	String	strCard = m_pCmbCardNum->Text;
	String	strRelay = m_pCmbSbRelay->Text;
	int			iCard = atoi(strCard.c_str());
	int			iRelay = atoi(strRelay.c_str());
    EnableRelayState(false);
	bool		bRet = g_daqDigit.SetSbRelay(iCard, iRelay, false);
    EnableRelayState(true);
	if (false == bRet)
	{
		ShowMessage("Open the Subboard Relay Fail!");
		return;
	}

	ViewStatusInGrid(iCard);
}

void __fastcall TFormManual::OnBtnACUClick(TObject *Sender)
{
	if (m_pCmbCardNum->ItemIndex == -1)
	{
		ShowMessage("Please select the Board on the SubBoard first!");
		return;
	}

	String		strCard = m_pCmbCardNum->Text;
	int			iCard = atoi(strCard.c_str());
	int			iRelay = 265;
    EnableRelayState(false);
	bool		bRet = g_daqDigit.SetSbRelay(iCard, iRelay, true);
    EnableRelayState(true);
	if (false == bRet)
	{
		ShowMessage("Open the Subboard Relay Fail!");
		return;
	}

	ViewStatusInGrid(iCard);
}

void __fastcall TFormManual::OnBtnWLClick(TObject *Sender)
{
	if (m_pCmbCardNum->ItemIndex == -1)
	{
		ShowMessage("Please select the Board on the SubBoard first!");
		return;
	}

	String		strCard = m_pCmbCardNum->Text;
	int			iCard = atoi(strCard.c_str());
	int			iRelay = 264;
    EnableRelayState(false);
	bool		bRet = g_daqDigit.SetSbRelay(iCard, iRelay, true);
    EnableRelayState(true);
	if (false == bRet)
	{
		ShowMessage("Open the Subboard Relay Fail!");
		return;
	}

	ViewStatusInGrid(iCard);
}

void __fastcall TFormManual::OnBtnSbResetClick(TObject *Sender)
{
	EnableRelayState(false);
	bool		bRet = g_daqDigit.ResetRelay();
    EnableRelayState(true);
	if (false == bRet)
	{
		ShowMessage("Reset the Subboard Relay Fail!");
		return;
	}

	ViewStatusInGrid(0);
	ViewStatusInMainboard();
}

void __fastcall TFormManual::OnBtnSbTestClick(TObject *Sender)
{
	if (m_pCmbCardNum->ItemIndex == -1)
	{
		ShowMessage("Please select the Board on the SubBoard first!");
		return;
	}

	String	strCard = m_pCmbCardNum->Text;
	int		iCard = atoi(strCard.c_str());

	// 子板上的继电器
	if (true == m_pMbtBase->Checked)
	{
    	EnableRelayState(false);
		g_daqDigit.SetSbRelay(iCard, 265, true);
		ViewStatusInGrid(iCard);
		this->Update();
		g_daqDigit.SetSbRelay(iCard, 264, true);
		ViewStatusInGrid(iCard);
		this->Update();
		g_daqDigit.SetSbRelay(iCard, 273, true);
		ViewStatusInGrid(iCard);
		this->Update();
		g_daqDigit.SetSbRelay(iCard, 277, true);
		ViewStatusInGrid(iCard);
		this->Update();
	}

	for (int j = 0; j < MES_RELAYGROUP; j++)
	{
		for (int k = 0; k < MES_RELAYNUM; k++)
		{
			int iRelay = 200 + j * 10 + k;
			if (true == m_pMbtBase->Checked && (iRelay == 264 || iRelay == 265 || iRelay == 273 || iRelay == 277))
				continue;
            EnableRelayState(false);
			g_daqDigit.SetSbRelay(iCard, iRelay, true);
			ViewStatusInGrid(iCard);
			this->Update();

			g_daqDigit.SetSbRelay(iCard, iRelay, false);
			ViewStatusInGrid(iCard);
			this->Update();
		}
	}

    EnableRelayState(true);
}

void __fastcall TFormManual::OnBtnInitClick(TObject *Sender)
{
	if (m_pCmbCardNum->ItemIndex == -1)
	{
		ShowMessage("Please select the Board on the SubBoard first!");
		return;
	}

	String		strCard = m_pCmbCardNum->Text;
	int			iCard = atoi(strCard.c_str());
	int			iRelay = 265;
    EnableRelayState(false);
	bool		bRet = g_daqDigit.SetSbRelay(iCard, iRelay, true);
    G_SDelay(300);
    EnableRelayState(true);
	if (false == bRet)
	{
		ShowMessage("Open the Subboard Relay Fail!");
		return;
	}
	ViewStatusInGrid(iCard);

    iRelay = 264;
    EnableRelayState(false);
    bRet = g_daqDigit.SetSbRelay(iCard, iRelay, true);
    G_SDelay(300);
    EnableRelayState(true);
	if (false == bRet)
	{
		ShowMessage("Open the Subboard Relay Fail!");
		return;
	}
    ViewStatusInGrid(iCard);

    iRelay = 273;
    EnableRelayState(false);
    bRet = g_daqDigit.SetSbRelay(iCard, iRelay, true);
    G_SDelay(300);
    EnableRelayState(true);
	if (false == bRet)
	{
		ShowMessage("Open the Subboard Relay Fail!");
		return;
	}
    ViewStatusInGrid(iCard);
}

void __fastcall TFormManual::OnBtnKlineClick(TObject *Sender)
{
	if (m_pCmbCardNum->ItemIndex == -1)
	{
		ShowMessage("Please select the Board on the SubBoard first!");
		return;
	}

	String		strCard = m_pCmbCardNum->Text;
	int			iCard = atoi(strCard.c_str());
	int			iRelay = 273;
    EnableRelayState(false);
	bool		bRet = g_daqDigit.SetSbRelay(iCard, iRelay, true);
    EnableRelayState(true);
	if (false == bRet)
	{
		ShowMessage("Open the Subboard Relay Fail!");
		return;
	}

	ViewStatusInGrid(iCard);
}

void __fastcall TFormManual::OnBtnTestClick(TObject *Sender)
{
	// 母板上的三个继电器
    EnableRelayState(false);
	for(int i = 300; i < 303; i++)
    {
    	g_daqDigit.SetMbRelay(i, true);
        ViewStatusInMainboard();
        this->Update();

        g_daqDigit.SetMbRelay(i, false);
        ViewStatusInMainboard();
        this->Update();
    }

    // 子板上的继电器
	for (int i = 0; i < MES_SUBBOARD_NUM; i++)
	{
        if(true == m_pMbtBase->Checked)
        {
            g_daqDigit.SetSbRelay(i, 264, true);
            ViewStatusInGrid(i);
            this->Update();
            g_daqDigit.SetSbRelay(i, 265, true);
            ViewStatusInGrid(i);
            this->Update();
            g_daqDigit.SetSbRelay(i, 273, true);
            ViewStatusInGrid(i);
            this->Update();
            g_daqDigit.SetSbRelay(i, 277, true);
            ViewStatusInGrid(i);
            this->Update();
        }
		for (int j = 0; j < MES_RELAYGROUP; j++)
		{
			for (int k = 0; k < MES_RELAYNUM; k++)
			{
				int iRelay = 200 + j * 10 + k;
                if(true == m_pMbtBase->Checked && (iRelay == 264 || iRelay == 265 || iRelay == 273 || iRelay == 277))
                    continue;

				g_daqDigit.SetSbRelay(i, iRelay, true);
				ViewStatusInGrid(i);
                this->Update();

				g_daqDigit.SetSbRelay(i, iRelay, false);
				ViewStatusInGrid(i);
                this->Update();
			}
		}
        G_SDelay(100);		// 板卡之间进行切换
	}
    EnableRelayState(true);
}

///////////////////////////////////////////////////////////////////////////////
//公共操作
///////////////////////////////////////////////////////////////////////////////
void __fastcall TFormManual::OnLstItemClick(TObject *Sender)
{
    for (int i = 0; i < m_pLstItem->Items->Count; i++)
	{
		if (m_pLstItem->Selected[i])
        {
        	m_pMoRemark->Clear();
            m_pMoRemark->Text = m_caControl[i*2] + "\n\n\n" +m_caControl[i*2 + 1];
        }
	}
}

void __fastcall TFormManual::EnablePcPowerState(bool bEnable)
{
	m_pEdtVolt->Enabled = bEnable;
	m_pEdtCurr->Enabled = bEnable;
	m_pCmbOutput->Enabled = bEnable;
	m_pCmbOCP->Enabled = bEnable;
	m_pCmbOVP->Enabled = bEnable;
	m_pBtnPcPowerRST->Enabled = bEnable;
	m_pBtnPcPowerTST->Enabled = bEnable;
	m_pBtnPcPowerRead->Enabled = bEnable;
	m_pBtnCurr->Enabled = bEnable;
	m_pBtnVolt->Enabled = bEnable;
	m_pBtnOutput->Enabled = bEnable;
	m_pBtnOCP->Enabled = bEnable;
	m_pBtnOVP->Enabled = bEnable;
	m_pBtnPcPowerIDN->Enabled = bEnable;
	m_pEdtPcPowerRetStatus->Enabled = false;
	m_pEdtPcPowerCommand->Enabled = bEnable;
	m_pBtnPcPowerWrite->Enabled = bEnable;
	m_pMbtPcPowerAsync->Enabled = bEnable;
	m_pMbtMode->Enabled = bEnable;

	m_pBtnPcPowerOpen->Enabled = bEnable;
	m_pBtnPcPowerClose->Enabled = bEnable;
}

void __fastcall TFormManual::EnablePcResState(bool bEnable)
{
	m_pBbtPcResAsync->Enabled = bEnable;
	m_pEdtPcResRetStatus->Enabled = bEnable;
	m_pBtnPcResWrite->Enabled = bEnable;
	m_pBtnPcResRead->Enabled = bEnable;
	m_pEdtPcResCommand->Enabled = bEnable;
	m_pBtnPcResIDN->Enabled = bEnable;
	m_pBtnPcResTST->Enabled = bEnable;
	m_pBtnPcResRST->Enabled = bEnable;
	m_pEdtRes->Enabled = bEnable;
	m_pBtnSetRes->Enabled = bEnable;
    m_pCmbResBox->Enabled = bEnable;
	m_pLabelResBox->Enabled = bEnable;

    m_pBtnPcResOpen->Enabled = bEnable;
	m_pBtnPcResClose->Enabled = bEnable;
}

void __fastcall TFormManual::EnablePcDMMState(bool bEnable)
{
	m_pBtnPcDMMIDN->Enabled = bEnable;
	m_pBtnPcDMMRST->Enabled = bEnable;
	m_pBtnPcDMMTST->Enabled = bEnable;
	m_pBtnPcDMMCommand->Enabled = bEnable;
	m_pBtnPcDMMWrite->Enabled = bEnable;
	m_pBtnPcDMMRead->Enabled = bEnable;
	m_pEdtPcDMMRetStatus->Enabled = bEnable;
	m_pMbtPcDMMAsync->Enabled = bEnable;
	m_pBtnGetVolt->Enabled = bEnable;
	m_pBtnGetCurr->Enabled = bEnable;
	m_pBtnGetFRes->Enabled = bEnable;
    m_pBtnGetRes->Enabled = bEnable;
	m_pBtnGetFreq->Enabled = bEnable;
    m_pEdtPcDMMVolt->Enabled = bEnable;
    m_pEdtPcDMMCurr->Enabled = bEnable;
	m_pEdtPcDMMFRes->Enabled = bEnable;
    m_pEdtPcDMMRes->Enabled = bEnable;
	m_pEdtPcDMMFreq->Enabled = bEnable;

    m_pBtnPcDMMOpen->Enabled = bEnable;
	m_pBtnPcDMMClose->Enabled = bEnable;
}

void __fastcall TFormManual::EnableChamberState(bool bEnable)
{
	m_pEdtTemp->Enabled = bEnable;
	m_pBtnSetTemp->Enabled = bEnable;
	m_pBtnGetTemp->Enabled = bEnable;
	m_pBtnWaitTemp->Enabled = bEnable;

	m_pBtnChamberOpen->Enabled = bEnable;
	m_pBtnChamberClose->Enabled = bEnable;
    m_pBtnChamberStop->Enabled = bEnable;
}

void __fastcall TFormManual::EnableKlineState(bool bEnable)
{
    m_pBtnKlineOpen->Enabled = bEnable;
    m_pBtnKlineClose->Enabled = bEnable;
    
    m_pBtnExe->Enabled = bEnable;
    m_pCmbCommand->Enabled = bEnable;
}

void __fastcall TFormManual::EnableRelayState(bool bEnable)
{
	m_pBtnDaqOpen->Enabled = bEnable;
    m_pBtnDaqClose->Enabled = bEnable;
    
	m_pCmbMbRelay->Enabled = bEnable;
    m_pBtnMbOn->Enabled = bEnable;
    m_pBtnMbOff->Enabled = bEnable;
    m_pBtnMbTest->Enabled = bEnable;

    m_pCmbCardNum->Enabled = bEnable;
    m_pBtnSbReset->Enabled = bEnable;
    m_pBtnSbOn->Enabled = bEnable;
    m_pBtnSbOff->Enabled = bEnable;
    m_pCmbSbRelay->Enabled = bEnable;
    m_pBtnSbTest->Enabled = bEnable;
    m_pBtnInit->Enabled = bEnable;
    m_pBtnKline->Enabled = bEnable;
    m_pBtnACU->Enabled = bEnable;
    m_pBtnWL->Enabled = bEnable;
    m_pMbtBase->Enabled = bEnable;
    m_pBtnTest->Enabled = bEnable;
    m_pBtnHelp->Enabled = bEnable;
}

void __fastcall TFormManual::OnTimer(TObject * Sender)
{
	// Refresh Statusbar
}

void __fastcall TFormManual::OnStrGrdDrawCell(TObject * Sender, int ACol, int ARow, TRect & Rect, TGridDrawState State)
{
	m_pStrGrd->Canvas->Brush->Color = clWhite;
	m_pStrGrd->Canvas->FillRect(Rect);
	if (m_pStrGrd->Cells[ACol][ARow] == '1')
	{
		m_pStrGrd->Canvas->Brush->Color = clRed;
		m_pStrGrd->Canvas->Ellipse(Rect);
	}

	if ((9 == ARow || 0 == ARow) && ACol >= 6 && ACol <= 7)
	{
		m_pStrGrd->Canvas->Brush->Color = clYellow;
		m_pStrGrd->Canvas->Ellipse(Rect);
	}
} 

void __fastcall TFormManual::ViewStatusInGrid(short iCard)
{
	int iGroupState, iBit, iBitRet; // 组状态、当前假如为1时值、当前位真实值

	// 继电器组
	for (unsigned char i = 0; i < 9; i++)
	{
		if (i == iCard) m_pStrGrd->Cells[i][0] = '1';
		else m_pStrGrd->Cells[i][0] = '0';

		iGroupState = g_daqDigit.m_scRelayStatus[iCard][i];
		iBit = 0x01;

		// 每组8个继电器
		for (unsigned char j = 0; j < 8; j++)
		{
			iBitRet = iGroupState & iBit;
			if (iBit == iBitRet) m_pStrGrd->Cells[j][i + 1] = '1';
			else m_pStrGrd->Cells[j][i + 1] = '0';
			iBit = iBit << 1;
		}
	}
}

void __fastcall TFormManual::ViewStatusInMainboard()
{
	if ((g_daqDigit.m_ulMbRelayStatus & 0x10) == 0x10) m_pShapeF->Brush->Color = clRed;
	else m_pShapeF->Brush->Color = clWhite;

	if ((g_daqDigit.m_ulMbRelayStatus & 0x20) == 0x20) m_pShapeS->Brush->Color = clRed;
	else m_pShapeS->Brush->Color = clWhite;

	if ((g_daqDigit.m_ulMbRelayStatus & 0x40) == 0x40) m_pShapeT->Brush->Color = clRed;
	else m_pShapeT->Brush->Color = clWhite;
}


void __fastcall TFormManual::OnBtnHelpClick(TObject *Sender)
{
    bool bRet = false;
    bRet = OpenDevice();
	if (false == bRet)
	{
		g_strMark = "Open All Device or Reset All Device Failed.!";
		//Synchronize(FormMain->ShowTip);
	}
}
