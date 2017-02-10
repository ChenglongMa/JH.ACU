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
// �����е��ظ�����
///////////////////////////////////////////////////////////////////////////////
unsigned int 	g_iItemRepeat = 2;		// �������Բ���ʧ��ʱ�ظ���������̵����͹���������ɣ���ҪӲ������
unsigned int	g_iFindRepeat = 2;		// Klineͨ��ʧ��ʱ���ظ���������Ҫ���������̵�������ǰ�̵����͵�Դ��Ӧ��鲢��Ӧ




///////////////////////////////////////////////////////////////////////////////
// ������Ϣ��ʾ on testing / Charmber Process
///////////////////////////////////////////////////////////////////////////////
int  	g_iViewChamber = 0; 	// Chamber Process
int  	g_iViewTest = 0; 		// on testing
bool 	g_bViewChamber = false;	// Chamber Process
bool 	g_bViewTest = false;	// on testing
String	g_strCurrSet = "";		// current setting



///////////////////////////////////////////////////////////////////////////////
// �¶�OnTimer ��ʾ��д��TempSheet
bool 	g_bChamberIdle = true;	// �����Ƿ����
bool 	g_bTempReal = false;	// �Ƿ���ȡ�¶�
int  	g_iTempCount = 1;		// ÿ10S��ȡʵ���¶�
int  	g_iTempInterval = 1;	// ����д���ļ�Interval
int		g_iTempRow = 1;			// д��TempSheet��������



///////////////////////////////////////////////////////////////////////////////
// ��ѹOnTimer ��ʾ��д��Volt.dat
///////////////////////////////////////////////////////////////////////////////
int  g_iVoltCount = 1;			// ÿ10S��ȡʵ�ʵ�ѹ
bool g_bVoltReal = false;		// �Ƿ���ȡ��ѹ



///////////////////////////////////////////////////////////////////////////////
// ״̬���������Ϣ��ʾ����
///////////////////////////////////////////////////////////////////////////////
bool 			g_bTimeStart = false;	// on testing �Ƿ�������ʼ
unsigned int 	g_iSecond = 0;			// �����ۻ�ʱ��
String 			g_strMainTip = "";		// ��ʾ��Ϣ������
String 			g_strSubTip = "";		// ��ʾ��Ϣ��С����



///////////////////////////////////////////////////////////////////////////////
// ���ߣ��¶�����
///////////////////////////////////////////////////////////////////////////////
bool 	g_bTempRenew = true;	// ������ʾ�¶�����
float 	g_fTimeTotal = 0.0;		// ÿ�������¶ȹ�������ʱ��
float 	g_fTimeRest = 0.0;		// ÿ�������¶�����ʱ��
float 	g_fTempInit = 0.0; 		// ÿÿ�����������¶�ʱ�ĳ�ʼ�¶�
float 	g_fTempCurveX = 10.00;	// ʵ���¶����ߺ����� m_pTempCurveReal
float	g_fTempCurveY = 0.0;	// ʵ���¶����������� m_pTempCurveReal
///////////////////////////////////////////////////////////////////////////////
// ���ߣ�����������
///////////////////////////////////////////////////////////////////////////////
int		g_iCurveMode = 0;		// 0-����ģʽ 1-HoldTimeģʽ 2-Crashģʽ
double 	g_dCurveCurrX = 0.0;   	// ������������ʾX
float 	g_fCurveCurrY = 0.0;	// ������������ʾY
double 	g_dCurveLength = 19.0;	// ������������ʾ���ȣ�	�������Ӧ0-19.0 1-999.0 2-999.0
double 	g_daCurveY[1000]={0.0};	// ��1��2ʱ������ֵ����
String	g_strCurveUnit = "ohm";	// ������ʾ��ߵ�λ,	�������Ӧ0-ohm 1-v 2-v



///////////////////////////////////////////////////////////////////////////////
// ������Ϣ����
///////////////////////////////////////////////////////////////////////////////
bool 	g_bProPos = false;	 		// �Ƿ񵥲�
int 	g_iProPos = 0;				// ���ǵ���ʱǰ��������



///////////////////////////////////////////////////////////////////////////////
// ������
///////////////////////////////////////////////////////////////////////////////
STestInfo g_STestInfo;		// �����е�ǰ��Ϣ����
int g_iTempPhase = 0;		// �����¶Ƚ׶� based 0  0-LT 1-NT 2-HT
int g_iVoltPhase = 0;		// ���õ�ѹ�׶� based 0  0-LV 1-NT 2-HV
float g_fTempTarget = 0.0;	// ��ǰĿ���¶ȣ��������Ӧ
float g_fVoltTarget = 0.0;	// ��ǰĿ���ѹ���������Ӧ
float g_fVoltInit = 0.0;	// ÿ���������õ�Դ��ѹʱ�ĳ�ʼ��ѹ
float g_fVoltCurr = 0.0;    // ʵʱ��Դ��ѹ
int g_iCRC = 0;				// ��ĳһ��TempVolt�µ�ĳһ���忨�µ�FM��������
int g_iTVCond = 0;			// ���ڲ��Ե�TV����	based 0
int g_iCurrBoard = 0;		// ���ڲ��Ե�ECU�������Ӱ� based 0
int g_iCurrItem = 0;		// ���ڲ��Եĵ�ǰReport���� based 0
bool g_bOpState = false;	// ��������״̬
int  g_iOpMode = 0;         // 0-�Զ���1-�ֹ�����ģʽ
String	g_strMark = "";		// ������ʾ��Ϣ
String 	g_strItemValue = "";// ��������ɺ������ֵ
// ����ȫ�ֱ���Ϊ���µĲ���
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

    // ����
    g_bTempRenew = true;
}



///////////////////////////////////////////////////////////////////////////////
// �ۺ�
///////////////////////////////////////////////////////////////////////////////
String 	g_strAppPath = "";		// ����·��
// ��ȡ��������·��
void GetAppPath(void)
{
    g_strAppPath = GetCurrentDir();
}

// ʹ����˳�Ӷ��ٺ������ʹ��ǰ�̹߳���
// ע�⣺GetTickCount�м���47.9��
// �����ȴ�
void G_SDelay(DWORD dwMilliSecond)
{
    long    lMillisecond = GetTickCount();
    while (GetTickCount() - lMillisecond < dwMilliSecond)
    {
        if ((GetTickCount() - lMillisecond) <= 0)
            lMillisecond = GetTickCount();
    }
}

// �첽�ȴ�
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

// ��1-15���ֱ��16����
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

// ���0xFFģʽ��ֻ��ʾ16���ƺ�2λ
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

// ����������ڻ�ʱ����и�ʽ������1-9��Ϊ"01"-"09"
// InputΪ���ڻ�ʱ��
// ���ڡ�ʱ���������Ӻ���
// ��1-9����2λ���������9ֱ�����
String IntToStrAdjust(int iInput)
{
	String	strTemp = "";
	if (-1 < iInput && iInput < 10) strTemp = "0" + IntToStr(iInput);
	else strTemp = IntToStr(iInput);
	return strTemp;
}

// ʱ���֣�����ʾ
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

// �ж��Ƿ��Ǵ������ַ������Ƿ����棬���򷵻ؼ�
bool GetStrType(String strTemp)
{
	int		i = 0;
	bool	bRet = true;

	// ɾ��һ�������'.'
	strTemp.Delete(strTemp.Pos('.'), 1);

	// ��һλΪ����λ
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
// �������
// �豸�����䴮�ڡ�Kline���ڡ����ֲɼ�����GPIB�ӿڡ�
//       GPIB�豸-����Դ�������䡢�������ñ�
///////////////////////////////////////////////////////////////////////////////
bool 	g_bAllDevice = false;      // �豸�Ƿ�׼������
// �������豸
bool OpenDevice()
{
	if(true == g_bAllDevice)
    	return true;

	bool bRet = false;
    // 1-�����豸�����䡢Kline
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

    // 2-D2KDASK���ɼ������̵���
    bRet = g_daqDio.Open();
    if(false == bRet)
    {
        g_strMark = "Open PIO port Failed.!!";
        return false;
    }
    g_daqDigit.SetParentObjectHandle();
    g_daqAnalog.SetParentObjectHandle();

    // 3-GPIB:�ӿڡ���Դ��״̬�����ã���DMM��RES1��RES2
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

// �ر������豸
bool CloseDevice(void)
{
	if(false == g_bAllDevice)
    	return true;
    
	bool	bRet = false;
    // 1-D2KDASK:�ɼ������̵���
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

    // 2-GPIB:�ӿڡ���Դ��״̬�����ã���DMM��RES1��RES2
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

    // 3-����:���䡢Kline
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
// Kline��������
///////////////////////////////////////////////////////////////////////////////
FILE	*g_pFileComKline = NULL;
FILE	*g_pFileErrReal = NULL;
FILE	*g_pFileVolt = NULL;
// ��Log�ļ�
bool OpenLogFile(int iFileType, String strPath)
{
	// �Ѵ򿪣��򷵻�
    FILE *pFile = NULL;
	if (pFile != NULL) return true;

    // ��׷����ʽ���ļ�
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

// д������
bool WriteLogFile (FILE *pFile, String str)
{
	// �ļ������ڣ��˳�
	if (NULL == pFile) return true;

    // д������
	int iRet = fputs(str.c_str(), pFile);
    // flush the data to pFile without closing it
    fflush(pFile);

	if (iRet == 0) return false;
	else return true;
}

// �ر�Log�ļ�
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
// ������
///////////////////////////////////////////////////////////////////////////////
int		g_iCurrFaultCode = 1;		// ��ǰ�Ĵ�����
char	g_caSpec[MES_ITEM_REAL][MES_SPEC_LENGTH] = {0x00};
char	g_caErrInfo[MES_ERRORS_NUM][MES_ERRINFO_LENGTH] = {0x00};
// ��ȡSPEC.TXT������g_caSpec
void SetSPECInfo ()
{
    FILE	*pFile = NULL;
	pFile = fopen((g_strAppPath + "\\config\\SPEC_unit.txt").c_str(), "r");
    // �ļ������ڣ���Ϊȱʡֵ
	if (pFile == NULL)
	{
		ShowMessage("Unable to open name_SPEC_unit file to get SPEC!");
		for (int k = 0; k < MES_ITEM_REAL; k++)
		{
			strcpy(g_caSpec[k], "00-no step name#                            0.00-0.00#          ***#      0x00#");
		}

		return;
	}

    // ���Թ淶ÿ����󳤶� + 2
    char	strTemp[MES_SPEC_LENGTH] = { "\0" }, *pCh = NULL;
	int		iRepeat = 0;
    // ��do-whileѭ�����һ��ѭ�����������һ�μ�¼��������
	pCh = fgets(strTemp, MES_SPEC_LENGTH, pFile);
	while (pCh != NULL && iRepeat < MES_ITEM_REAL)
	{
		strcpy(g_caSpec[iRepeat], strTemp);
		iRepeat++;

		for (int k = 0; k < MES_SPEC_LENGTH; k++)
        {
        	strTemp[k] = '\0';
        }
        // ������������գ�������һ��\0��ֹͣ��ֵ
		pCh = fgets(strTemp, MES_SPEC_LENGTH, pFile);
	}

	fclose(pFile);
}

// ����ָ�����裬ָ�����͵Ĳ��Թ淶��Ϣ�����Է���ֵ��ʽ����
// iFullItem Ϊ���Բ���� iTypeΪ��Ϣ���ͣ�0Ϊname��1Ϊ�淶��2Ϊunit��3Ϊfault code
String GetSPECInfo(int iFullItem, int iType)
{
	String	strName = "\0", strSPEC = "\0", strUnit = "\0", strFaultCode = "\0";
	int		i;
	int		j = 0;
	int		iSecondCol = 44;
	int		iThirdCol = 64;
	int		iFourthCol = 74;

	// ��¼��һ��#��
	while (g_caSpec[iFullItem][j] != '#' && j < MES_SPEC_LENGTH)
	{
		j++;
	}

	for (i = 3; i < j; i++)
	{
		strName = strName + g_caSpec[iFullItem][i];
	}

	// �ڶ��п�ʼλ��
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
		case 0:		return strName;			// 0Ϊname
		case 1:		return strSPEC;			// 1ΪSPEC
		case 2:		return strUnit;			// 2Ϊunit
		case 3:		return strFaultCode;	// 3Ϊfault code
		default:	return "error";
	}
}


// �����ϵ����У��������뼰��˵����Ϣ���ļ��������ά����g_caErrInfo��
void ReadFaultInfo()
{
	FILE	*pFile = NULL;
	pFile = fopen((g_strAppPath + "\\config\\fault_code.txt").c_str(), "r");
    // �ļ������ڣ���Ϊȱʡֵ
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
    // ��do-whileѭ�����һ��ѭ�����������һ�μ�¼��������
	pCh = fgets(strTemp, MES_ERRINFO_LENGTH, pFile);
	while (pCh != NULL && iErrCount < MES_ERRORS_NUM)
	{
		strcpy(g_caErrInfo[iErrCount], strTemp);
		iErrCount++;

		for (int k = 0; k < MES_ERRINFO_LENGTH; k++)
        {
        	strTemp[k] = '\0';
        }

        // ������������գ�������һ��\0��ֹͣ��ֵ
		pCh = fgets(strTemp, MES_ERRINFO_LENGTH, pFile);
	}

	fclose(pFile);
}

bool GetFaultInfo (int iFaultCode, String & strCode, String & strRemark)
{
	int		k = 7, iCurrFaultCode = 0x01;
	for (int i = 0; i < MES_ERRORS_NUM; i++)
	{
		// ȡ����ʾ��
		for (int j = 1; j <= 4; j++)
		{
			strCode = strCode + g_caErrInfo[i][j];
		}

        strCode.Trim();
		if (strCode[1] == '\0')
        {
        	break;
        }

        // StrToInt������ת���մ�
		iCurrFaultCode = StrToInt(strCode);
		if (iFaultCode == iCurrFaultCode)
		{	// ȡremark
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






