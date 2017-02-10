#pragma hdrstop

#include "Global.h"
#include <math.h>
#include <stdio.h>
#include "ComChamber.h"
#include "ComKline.h"
#include "GpibInterface.h"
#include "GpibPcPower.h"
#include "GpibPcRes.h"
#include "GpibPcDMM.h"
#include "DaqDio.h"
#include "DaqDigit.h"
#include "DaqAnalog.h"

#pragma package(smart_init)

///////////////////////////////////////////////////////////////////////////////
// 测试中的重复次数
///////////////////////////////////////////////////////////////////////////////
unsigned int 	g_iItemRepeat = 2;		// 单个测试步骤失败时重复次数，其继电器和供电检查已完成，需要硬件重启
unsigned int	g_iFindRepeat = 2;		// Kline通信失败时的重复次数，需要重启公共继电器、当前继电器和电源供应检查并供应




///////////////////////////////////////////////////////////////////////////////
// 测试信息提示 on testing / Charmber Process
///////////////////////////////////////////////////////////////////////////////
int  	g_iViewChamber = 0; 	// Chamber Process
int  	g_iViewTest = 0; 		// on testing
bool 	g_bViewChamber = false;	// Chamber Process
bool 	g_bViewTest = false;	// on testing
String	g_strCurrSet = "";		// current setting



///////////////////////////////////////////////////////////////////////////////
// 温度OnTimer 显示和写入TempSheet
bool 	g_bChamberIdle = true;	// 温箱是否空闲
bool 	g_bTempReal = false;	// 是否立取温度
int  	g_iTempCount = 1;		// 每10S获取实际温度
int  	g_iTempInterval = 1;	// 控制写到文件Interval
int		g_iTempRow = 1;			// 写入TempSheet的总行数



///////////////////////////////////////////////////////////////////////////////
// 电压OnTimer 显示和写入Volt.dat
///////////////////////////////////////////////////////////////////////////////
int  g_iVoltCount = 1;			// 每10S获取实际电压
bool g_bVoltReal = false;		// 是否立取电压



///////////////////////////////////////////////////////////////////////////////
// 状态条及相关信息显示控制
///////////////////////////////////////////////////////////////////////////////
bool 			g_bTimeStart = false;	// on testing 是否真正开始
unsigned int 	g_iSecond = 0;			// 测试累积时间
String 			g_strMainTip = "";		// 提示信息：大步骤
String 			g_strSubTip = "";		// 提示信息：小流程



///////////////////////////////////////////////////////////////////////////////
// 曲线：温度曲线
///////////////////////////////////////////////////////////////////////////////
bool 	g_bTempRenew = true;	// 重新显示温度曲线
float 	g_fTimeTotal = 0.0;		// 每次设置温度估计所需时间
float 	g_fTimeRest = 0.0;		// 每次设置温度余下时间
float 	g_fTempInit = 0.0; 		// 每每次设置温箱温度时的初始温度
float 	g_fTempCurveX = 10.00;	// 实际温度曲线横坐标 m_pTempCurveReal
float	g_fTempCurveY = 0.0;	// 实际温度曲线纵坐标 m_pTempCurveReal
///////////////////////////////////////////////////////////////////////////////
// 曲线：测试项曲线
///////////////////////////////////////////////////////////////////////////////
int		g_iCurveMode = 0;		// 0-电阻模式 1-HoldTime模式 2-Crash模式
double 	g_dCurveCurrX = 0.0;   	// 测试项曲线显示X
float 	g_fCurveCurrY = 0.0;	// 测试项曲线显示Y
double 	g_dCurveLength = 19.0;	// 测试项曲线显示长度，	与上面对应0-19.0 1-999.0 2-999.0
double 	g_daCurveY[1000]={0.0};	// 在1和2时采样的值集合
String	g_strCurveUnit = "ohm";	// 曲线显示左边单位,	与上面对应0-ohm 1-v 2-v



///////////////////////////////////////////////////////////////////////////////
// 进度信息控制
///////////////////////////////////////////////////////////////////////////////
bool 	g_bProPos = false;	 		// 是否单步
int 	g_iProPos = 0;				// 不是单步时前进到多少



///////////////////////////////////////////////////////////////////////////////
// 测试项
///////////////////////////////////////////////////////////////////////////////
STestInfo g_STestInfo;		// 测试中当前信息汇总
int g_iTempPhase = 0;		// 设置温度阶段 based 0  0-LT 1-NT 2-HT
int g_iVoltPhase = 0;		// 设置电压阶段 based 0  0-LV 1-NT 2-HV
float g_fTempTarget = 0.0;	// 当前目标温度，和上面对应
float g_fVoltTarget = 0.0;	// 当前目标电压，和上面对应
float g_fVoltInit = 0.0;	// 每次重新设置电源电压时的初始电压
float g_fVoltCurr = 0.0;    // 实时电源电压
int g_iCRC = 0;				// 在某一个TempVolt下的某一个板卡下的FM操作次数
int g_iTVCond = 0;			// 正在测试的TV条件	based 0
int g_iCurrBoard = 0;		// 正在测试的ECU所连的子板 based 0
int g_iCurrItem = 0;		// 正在测试的当前Report步骤 based 0
bool g_bOpState = false;	// 操作返回状态
int  g_iOpMode = 0;         // 0-自动、1-手工操作模式
String	g_strMark = "";		// 测试提示信息
String 	g_strItemValue = "";// 测试项完成后测量的值
// 更新全局变量为了新的测试
void SetGlobalStateForNewTest()
{
	// OnTimer:LabelSpark, Temp, Volt, StatusBar
    g_bViewChamber = false;
	g_bViewTest = false;
    g_iViewChamber = 0;
    g_iViewTest = 0;

    g_bChamberIdle = true;
    g_bTempReal = false;
    g_iTempCount = 1;
    g_iTempInterval = 1;

    g_bVoltReal = false;
    g_iVoltCount = 1;

    g_bTimeStart = true;
    g_iSecond = 0;

    // 曲线
    g_bTempRenew = true;
}



///////////////////////////////////////////////////////////////////////////////
// 综合
///////////////////////////////////////////////////////////////////////////////
String 	g_strAppPath = "";		// 程序路径
// 获取程序运行路径
void GetAppPath(void)
{
    g_strAppPath = GetCurrentDir();
}

// 使程序顺延多少毫秒而不使当前线程挂起
// 注意：GetTickCount有极限47.9天
// 单步等待
void G_SDelay(DWORD dwMilliSecond)
{
    long    lMillisecond = GetTickCount();
    while (GetTickCount() - lMillisecond < dwMilliSecond)
    {
        if ((GetTickCount() - lMillisecond) <= 0)
            lMillisecond = GetTickCount();
    }
}

// 异步等待
void G_MDelay(DWORD dwMilliSecond)
{
	long    lMillisecond = GetTickCount();
    while (GetTickCount() - lMillisecond < dwMilliSecond)
    {
    	Application->ProcessMessages();
        if ((GetTickCount() - lMillisecond) <= 0)
            lMillisecond = GetTickCount();
    }
}

// 把1-15数字编程16进制
String DToH2(int iInput)
{
	switch (iInput)
	{
		case 0:		return "0";
		case 1:		return "1";
		case 2:		return "2";
		case 3:		return "3";
		case 4:		return "4";
		case 5:		return "5";
		case 6:		return "6";
		case 7:		return "7";
		case 8:		return "8";
		case 9:		return "9";
		case 10:	return "A";
		case 11:	return "B";
		case 12:	return "C";
		case 13:	return "D";
		case 14:	return "E";
		case 15:	return "F";
		default:	return "error";
	}
}

// 编程0xFF模式，只显示16进制后2位
String DToH1(int iInput)
{
	int			iaR[2];
	String	strTemp = "";
	if (iInput < 16)
	{
		iaR[0] = fmod(iInput, 16);
		iaR[1] = 0;
	}
    else
	{
		iaR[0] = fmod(iInput, 16);
		iaR[1] = fmod(iInput / 16, 16);
	}

	strTemp = DToH2(iaR[1]) + DToH2(iaR[0]);
	return strTemp;
}

// 将输入的日期或时间进行格式调整，1-9变为"01"-"09"
// Input为日期或时间
// 日期、时间调整输出子函数
// 把1-9进行2位输出，大于9直接输出
String IntToStrAdjust(int iInput)
{
	String	strTemp = "";
	if (-1 < iInput && iInput < 10) strTemp = "0" + IntToStr(iInput);
	else strTemp = IntToStr(iInput);
	return strTemp;
}

// 时：分：秒显示
String IntToStrTime(int iSecond)
{
	String strTemp = "";
    int iTemp = 0;
    iTemp = iSecond/3600;
    strTemp = IntToStrAdjust(iTemp) + ":";
    iTemp = iSecond%3600;
    iTemp = iTemp/60;
    strTemp = strTemp + IntToStrAdjust(iTemp) + ":";
    iTemp = iTemp%60;
    strTemp = strTemp + IntToStrAdjust(iTemp);

    return strTemp;
}

// 判断是否是纯数字字符串，是返回真，否则返回假
bool GetStrType(String strTemp)
{
	int		i = 0;
	bool	bRet = true;

	// 删除一次最左边'.'
	strTemp.Delete(strTemp.Pos('.'), 1);

	// 第一位为符号位
	if (strTemp.operator[](1) == '+' || strTemp.operator[](1) == '-')
	{
		for (i = 2; i <= strTemp.Length(); i++)
		{
			if (strTemp.operator[](i) < 48 || strTemp.operator[](i) > 57)
			{
				bRet = false;
				break;
			}
		}
	} else
	{
		for (i = 1; i <= strTemp.Length(); i++)
		{
			if (strTemp.operator[](i) < 48 || strTemp.operator[](i) > 57)
			{
				bRet = false;
				break;
			}
		}
	}

	return bRet;
}




///////////////////////////////////////////////////////////////////////////////
// 外设操作
// 设备：温箱串口、Kline串口、数字采集卡、GPIB接口、
//       GPIB设备-（电源、电阻箱、数字万用表）
///////////////////////////////////////////////////////////////////////////////
bool 	g_bAllDevice = false;      // 设备是否都准备好了
// 打开所有设备
bool OpenDevice()
{
	if(true == g_bAllDevice)
    	return true;

	bool bRet = false;
    // 1-串口设备：温箱、Kline
    bRet = g_comChamber.Open();
    if(false == bRet)
    {
        g_strMark = "Open Chamber Com Failed.!!";
        return false;
    }

    bRet = g_comKline.Open();
    if(false == bRet)
    {
        g_strMark = "Open Kline Com Failed.!!";
        return false;
    }

    // 2-D2KDASK：采集卡、继电器
    bRet = g_daqDio.Open();
    if(false == bRet)
    {
        g_strMark = "Open PIO port Failed.!!";
        return false;
    }
    g_daqDigit.SetParentObjectHandle();
    g_daqAnalog.SetParentObjectHandle();

    // 3-GPIB:接口、电源（状态、设置）、DMM、RES1、RES2
    bRet = g_gpibInterface.SetHandle(0);
    if(false == bRet)
    {
        g_strMark = "Open GPIB Interface Failed.!!";
        return false;
    }
    int iNum = 0;
    bRet = g_gpibInterface.GetDeviceCount(iNum);
    if(false == bRet)
    {
        g_strMark = "Get GPIB Device Number Failed.!!";
        return false;
    }
    if(4 != iNum)
    {
        g_strMark = "Get GPIB Device Number Error.!!";
        return false;
    }

    bRet = g_gpibPcPower.Init(g_gpibInterface, 8);
    if(false == bRet)
    {
        g_strMark = "Init GPIB Device PcPower Failed.!!";
        return false;
    }
    bRet = g_gpibPcPower.SetRST();
    if(false == bRet)
    {
        g_strMark = "SetRST GPIB Device PcPower Failed.!!";
        return false;
    }
    g_gpibPcPower.SetOutPut(false);
    g_gpibPcPower.SetVolt(13.5);
    g_gpibPcPower.SetCurr(5.0);
    g_gpibPcPower.SetOCP(false);

    bRet = g_gpibPcResF.Init(g_gpibInterface, 16);
    if(false == bRet)
    {
        g_strMark = "Init GPIB Device PcResF One Failed.!!";
        return false;
    }
    bRet = g_gpibPcResF.SetRST();
    if(false == bRet)
    {
        g_strMark = "SetRST GPIB Device PcResF Failed.!!";
        return false;
    }

    bRet = g_gpibPcResS.Init(g_gpibInterface, 17);
    if(false == bRet)
    {
        g_strMark = "Init GPIB Device PcPower Two Failed.!!";
        return false;
    }
    bRet = g_gpibPcResS.SetRST();
    if(false == bRet)
    {
        g_strMark = "SetRST GPIB Device PcResS Failed.!!";
        return false;
    }

    bRet = g_gpibPcDMM.Init(g_gpibInterface, 22);
    if(false == bRet)
    {
        g_strMark = "Init GPIB Device PcDMM Failed.!!";
        return false;
    }
    bRet = g_gpibPcDMM.SetRST();
    if(false == bRet)
    {
        g_strMark = "SetRST GPIB Device PcDMM Failed.!!";
        return false;
    }

    g_bAllDevice = true;
    return true;
}

// 关闭所遇设备
bool CloseDevice(void)
{
	if(false == g_bAllDevice)
    	return true;
    
	bool	bRet = false;
    // 1-D2KDASK:采集卡、继电器
    bRet = g_daqDigit.ResetRelay();
    if (false == bRet)
	{
        g_strMark = "Reset Relay Failed.!!";
		return false;
	}
    bRet = g_daqDio.Close();
	if (false == bRet)
	{
        g_strMark = "Close PIO port Failed.!!";
		return false;
	}

    // 2-GPIB:接口、电源（状态、设置）、DMM、RES1、RES2
	bRet = g_gpibPcPower.SetOutPut(false);
	if (false == bRet)
	{
        g_strMark = "SetOutPut false GPIB Device PcPower Failed.!!";
		return false;
	}
	bRet = g_gpibPcPower.Close();
	if (false == bRet)
	{
        g_strMark = "Close GPIB Device PcPower Failed.!!";
		return false;
	}

	bRet = g_gpibPcResF.SetRST();
	if (false == bRet)
	{
        g_strMark = "SetRST GPIB Device PcResF Failed.!!";
		return false;
	}
	bRet = g_gpibPcResF.Close();
	if (false == bRet)
	{
        g_strMark = "Close GPIB Device PcResF One Failed.!!";
		return false;
	}

	bRet = g_gpibPcResS.SetRST();
	if (false == bRet)
	{
        g_strMark = "SetRST GPIB Device PcResS Failed.!!";
		return false;
	}

	bRet = g_gpibPcResS.Close();
	if (false == bRet)
	{
        g_strMark = "Close GPIB Device PcResS Two Failed.!!";
		return false;
	}

	bRet = g_gpibPcDMM.SetRST();
	if (false == bRet)
	{
        g_strMark = "SetRST GPIB Device PcDMM Failed.!!";
		return false;
	}

	bRet = g_gpibPcDMM.Close();
	if (false == bRet)
	{
        g_strMark = "Close GPIB Device PcDMM Failed.!!";
		return false;
	}

	bRet = g_gpibInterface.Close();
	if (false == bRet)
	{
        g_strMark = "Close GPIB Interface Failed.!!";
		return false;
	}

    // 3-串口:温箱、Kline
	bRet = g_comChamber.Close();
	if (false == bRet)
	{
        g_strMark = "Close Chamber Com Failed.!!";
		return false;
	}

	bRet = g_comKline.Close();
	if (false == bRet)
	{
        g_strMark = "Close Kline Com Failed.!!";
		return false;
	}

	g_bAllDevice = false;
	return true;
}



///////////////////////////////////////////////////////////////////////////////
// Kline串口数据
///////////////////////////////////////////////////////////////////////////////
FILE	*g_pFileComKline = NULL;
FILE	*g_pFileErrReal = NULL;
FILE	*g_pFileVolt = NULL;
// 打开Log文件
bool OpenLogFile(int iFileType, String strPath)
{
	// 已打开，则返回
    FILE *pFile = NULL;
	if (pFile != NULL) return true;

    // 以追加形式打开文件
	pFile = fopen(strPath.c_str(), "a+");
	if (pFile == NULL)
	{
		ShowMessage("Unable to open the file.!");
		return false;
	}

    if(1 == iFileType)          g_pFileComKline = pFile;
    else if(2 == iFileType)     g_pFileVolt = pFile;

	return true;
}

// 写入数据
bool WriteLogFile (FILE *pFile, String str)
{
	// 文件不存在，退出
	if (NULL == pFile) return true;

    // 写入数据
	int iRet = fputs(str.c_str(), pFile);
    // flush the data to pFile without closing it
    fflush(pFile);

	if (iRet == 0) return false;
	else return true;
}

// 关闭Log文件
bool CloseLogFile (FILE *pFile)
{
	if (pFile != NULL)
	{
		int iRet = fclose(pFile);
		if (0 == iRet) return true;
		else return false;
	}

	return true;
}



///////////////////////////////////////////////////////////////////////////////
// 错误处理
///////////////////////////////////////////////////////////////////////////////
int		g_iCurrFaultCode = 1;		// 当前的错误码
char	g_caSpec[MES_ITEM_REAL][MES_SPEC_LENGTH] = {0x00};
char	g_caErrInfo[MES_ERRORS_NUM][MES_ERRINFO_LENGTH] = {0x00};
// 读取SPEC.TXT，配置g_caSpec
void SetSPECInfo ()
{
    FILE	*pFile = NULL;
	pFile = fopen((g_strAppPath + "\\config\\SPEC_unit.txt").c_str(), "r");
    // 文件不存在，置为缺省值
	if (pFile == NULL)
	{
		ShowMessage("Unable to open name_SPEC_unit file to get SPEC!");
		for (int k = 0; k < MES_ITEM_REAL; k++)
		{
			strcpy(g_caSpec[k], "00-no step name#                            0.00-0.00#          ***#      0x00#");
		}

		return;
	}

    // 测试规范每行最大长度 + 2
    char	strTemp[MES_SPEC_LENGTH] = { "\0" }, *pCh = NULL;
	int		iRepeat = 0;
    // 用do-while循环会多一次循环，这样最后一次记录读了两次
	pCh = fgets(strTemp, MES_SPEC_LENGTH, pFile);
	while (pCh != NULL && iRepeat < MES_ITEM_REAL)
	{
		strcpy(g_caSpec[iRepeat], strTemp);
		iRepeat++;

		for (int k = 0; k < MES_SPEC_LENGTH; k++)
        {
        	strTemp[k] = '\0';
        }
        // 缓冲区不用清空，遇到第一个\0就停止赋值
		pCh = fgets(strTemp, MES_SPEC_LENGTH, pFile);
	}

	fclose(pFile);
}

// 查找指定步骤，指定类型的测试规范信息，并以返回值形式返回
// iFullItem 为测试步骤号 iType为信息类型：0为name，1为规范，2为unit，3为fault code
String GetSPECInfo(int iFullItem, int iType)
{
	String	strName = "\0", strSPEC = "\0", strUnit = "\0", strFaultCode = "\0";
	int		i;
	int		j = 0;
	int		iSecondCol = 44;
	int		iThirdCol = 64;
	int		iFourthCol = 74;

	// 记录第一个#号
	while (g_caSpec[iFullItem][j] != '#' && j < MES_SPEC_LENGTH)
	{
		j++;
	}

	for (i = 3; i < j; i++)
	{
		strName = strName + g_caSpec[iFullItem][i];
	}

	// 第二列开始位置
	if (g_caSpec[iFullItem][iSecondCol] != ' ')
	{
		j = iSecondCol;
		while (g_caSpec[iFullItem][j] != '#' && j < MES_SPEC_LENGTH)
		{
			j++;
		}

		for (i = iSecondCol; i < j; i++)
		{
			strSPEC = strSPEC + g_caSpec[iFullItem][i];
		}
	} else strSPEC = " ";

	if (g_caSpec[iFullItem][iThirdCol] != ' ')
	{
		j = iThirdCol;
		while (g_caSpec[iFullItem][j] != '#' && j < MES_SPEC_LENGTH)
		{
			j++;
		}

		for (i = iThirdCol; i < j; i++)
		{
			strUnit = strUnit + g_caSpec[iFullItem][i];
		}
	} else strUnit = " ";

	if (g_caSpec[iFullItem][iFourthCol] != ' ')
	{
		j = iFourthCol;
		while (g_caSpec[iFullItem][j] != '#' && j < MES_SPEC_LENGTH)
		{
			j++;
		}

		for (i = iFourthCol; i < j; i++)
		{
			strFaultCode = strFaultCode + g_caSpec[iFullItem][i];
		}
	} else strFaultCode = " ";

	switch (iType)
	{
		case 0:		return strName;			// 0为name
		case 1:		return strSPEC;			// 1为SPEC
		case 2:		return strUnit;			// 2为unit
		case 3:		return strFaultCode;	// 3为fault code
		default:	return "error";
	}
}


// 程序上电运行，将故障码及其说明信息从文件里读到二维数组g_caErrInfo里
void ReadFaultInfo()
{
	FILE	*pFile = NULL;
	pFile = fopen((g_strAppPath + "\\config\\fault_code.txt").c_str(), "r");
    // 文件不存在，置为缺省值
	if (pFile == NULL)
	{
		ShowMessage("Unable to open the fault_code file to get fault_code!");
		for (int k = 0; k < MES_ERRORS_NUM; k++)
		{
			strcpy(g_caErrInfo[k], "(0x00) Unable to open the fault_code file");
		}

		return;
	}

    char	strTemp[MES_ERRINFO_LENGTH] = { "\0" }, *pCh = NULL;
	int		iErrCount = 0;
    // 用do-while循环会多一次循环，这样最后一次记录读了两次
	pCh = fgets(strTemp, MES_ERRINFO_LENGTH, pFile);
	while (pCh != NULL && iErrCount < MES_ERRORS_NUM)
	{
		strcpy(g_caErrInfo[iErrCount], strTemp);
		iErrCount++;

		for (int k = 0; k < MES_ERRINFO_LENGTH; k++)
        {
        	strTemp[k] = '\0';
        }

        // 缓冲区不用清空，遇到第一个\0就停止赋值
		pCh = fgets(strTemp, MES_ERRINFO_LENGTH, pFile);
	}

	fclose(pFile);
}

bool GetFaultInfo (int iFaultCode, String & strCode, String & strRemark)
{
	int		k = 7, iCurrFaultCode = 0x01;
	for (int i = 0; i < MES_ERRORS_NUM; i++)
	{
		// 取出标示符
		for (int j = 1; j <= 4; j++)
		{
			strCode = strCode + g_caErrInfo[i][j];
		}

        strCode.Trim();
		if (strCode[1] == '\0')
        {
        	break;
        }

        // StrToInt不允许转换空串
		iCurrFaultCode = StrToInt(strCode);
		if (iFaultCode == iCurrFaultCode)
		{	// 取remark
			while (g_caErrInfo[i][k] != '\n' && (k < 49))
			{
				strRemark = strRemark + g_caErrInfo[i][k];
				k++;
			}
            return true;
		}
        strCode = "";
	}

	return false;
}






