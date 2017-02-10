#pragma hdrstop

#include "DaqAnalog.h"
#include <math.h>

#pragma package(smart_init)

TDaqAnalog g_daqAnalog;

__fastcall TDaqAnalog::TDaqAnalog()
{
}

__fastcall TDaqAnalog::~TDaqAnalog()
{
}

bool __fastcall TDaqAnalog::GetPowerState()
{
	double dVolt = 0.0;
    bool bSingle = true, bMulti = true;
    String strTemp = "";
	// ReadSingle
	GetVoltFromChannel(1, MES_VOLT_5VCH, dVolt);
    if(fabs(5.0 - dVolt) > 1.0)
    	bSingle = false;
    strTemp = FloatToStr(dVolt) + ":    ";

    GetVoltFromChannel(1, MES_VOLT_12VCH, dVolt);
    if(fabs(12.0 - dVolt) > 1.0)
    	bSingle = false;
    strTemp = strTemp + FloatToStr(dVolt) + ":    ";

    GetVoltFromChannel(1, MES_VOLT_IGNCH, dVolt);
    if(fabs(g_fVoltTarget - dVolt) > 2.0)
    	bSingle = false;
    strTemp = strTemp + FloatToStr(dVolt) + ":    ";

    //GetVoltFromChannel(1, 4, dVolt);
    //if(fabs(6.0 - dVolt) > 1.0)
    //	bSingle = false;



    // ReadMulti
    GetVoltFromChannel(2, MES_VOLT_5VCH, dVolt);
    if(fabs(5.0 - dVolt) > 1.0)
    	bMulti = false;
    strTemp = strTemp + FloatToStr(dVolt) + ":    ";

    GetVoltFromChannel(2, MES_VOLT_12VCH, dVolt);
    if(fabs(12.0 - dVolt) > 1.0)
    	bMulti = false;
    strTemp = strTemp + FloatToStr(dVolt) + ":    ";

    GetVoltFromChannel(2, MES_VOLT_IGNCH, dVolt);
    if(fabs(g_fVoltTarget - dVolt) > 2.0)
    	bMulti = false;
    strTemp = strTemp + FloatToStr(dVolt) + ":\t\n";

    //GetVoltFromChannel(2, 4, dVolt);
    //if(fabs(6.0 - dVolt) > 1.0)
    //	bSingle = false;

    WriteLogFile(g_pFileVolt, strTemp);
    if(false == bSingle && false == bMulti)
    	return false;

    return true;
}

bool __fastcall TDaqAnalog::GetVoltFromChannel(int iReadType, unsigned short iChannel, double &dVolt)
{
	bool bRet = false;
	if(1 == iReadType)
    {
    	bRet = GetVoltFromChannelBySingle(iChannel, dVolt);
        return bRet;
    }
    else if(2 == iReadType)
    {
    	bRet = GetVoltFromChannelByMulti(iChannel, dVolt);
        return bRet;
    }
    return false;
}

bool __fastcall TDaqAnalog::GetVoltFromChannelBySingle(unsigned short iChannel, double &dVolt)
{
	short iRetCode = 0;
    iRetCode = D2K_AI_VReadChannel(m_iHandleCard, iChannel, &dVolt);
    if(NoError != iRetCode)
    {
    	m_strMark = "D2K_AI_VReadChannel Error.!";
    	return false;
    }

    switch(iChannel)
    {
    	case 1:		// 5V 没有分压
        	break;
        case 2:		// 12V 分压
        	dVolt = dVolt * 2.0;
        	break;
        case 3:		// POW_ECU 分压
        	dVolt = dVolt * MES_APMPLIFY_IGN;
        	break;
        case 4:		// AD_T Sensor 分压
        	break;
        default:
        	break;
    }

    return true;
}

bool __fastcall TDaqAnalog::GetVoltFromChannelByMulti (unsigned short iChannel, double &dVolt)
{
	short Buf[100] = {0}, iRetCode = 0;
	unsigned short iBufId = 0, iCountScan = 100, iAIRange = AD_B_10_V;
	//unsigned long iIntervalScan = MES_FREQ_TIMEBASE / MES_SCAN_COUNT;

	D2K_AI_CH_Config(m_iHandleCard, iChannel, iAIRange);
	D2K_AI_ContBufferSetup(m_iHandleCard, Buf, iCountScan, &iBufId);
	iRetCode = D2K_AI_ContReadChannel(m_iHandleCard, iChannel, iBufId, iCountScan, MES_SCAN_INTERVAL, MES_SCAN_INTERVAL, SYNCH_OP);
    if (iRetCode != NoError)
	{
		m_strMark = "D2K_AI_ContReadChannel error!";
		return false;
	}

    long iVoltSum =0;
    for (unsigned short i = 0; i < iCountScan; i++)
    	iVoltSum += Buf[i];
    iVoltSum = iVoltSum /iCountScan;
    dVolt = (float)iVoltSum * 10.0 /32768.0;

    switch(iChannel)
    {
    	case 1:		// 5V 没有分压
        	break;
        case 2:		// 12V 分压
        	dVolt = dVolt * 2.0;
        	break;
        case 3:		// POW_ECU 分压
        	dVolt = dVolt * MES_APMPLIFY_IGN;
        	break;
        case 4:		// AD_T Sensor 分压
        	break;
        default:
        	break;
    }

    return true;
}

bool __fastcall TDaqAnalog::SetCrashConfig(short iBuf[5000])
{
	unsigned short	iBufId = 0;
    short iRetCode = D2K_AIO_Config(m_iHandleCard, DAQ2K_IntTimeBase, Above_High_Level | CH0ATRIG, MES_ANATRIG_H, MES_ANATRIG_L);
	if (iRetCode != NoError)
	{
		m_strMark = "D2K_AIO_Config error!";
		return false;
	}

	iRetCode = D2K_AI_CH_Config(m_iHandleCard, MES_VOLT_CRASH, AD_B_10_V);
	if (iRetCode != NoError)
	{
		m_strMark = "D2K_AI_CH_Config error!";
		return false;
	}

	iRetCode = D2K_AI_Config(m_iHandleCard, DAQ2K_AI_ADCONVSRC_Int, DAQ2K_AI_TRGSRC_SOFT | DAQ2K_AI_TRGMOD_POST,
    	0, 0, 0, MES_BUF_RESET);
	if (iRetCode != NoError)
	{
		m_strMark = "D2K_AI_Config error!";
		return false;
	}

	iRetCode = D2K_AI_ContBufferSetup(m_iHandleCard, iBuf, MES_SCAN_COUNT, &iBufId);
	if (iRetCode != NoError)
	{
		m_strMark = "D2K_AI_ContBufferSetup error!";
		return false;
	}

	iRetCode = D2K_AI_ContReadChannel(m_iHandleCard, MES_VOLT_CRASH, iBufId, MES_SCAN_COUNT, MES_SCAN_INTERVAL,
    	MES_SCAN_INTERVAL, SYNCH_OP);
	if (iRetCode != NoError)
	{
		m_strMark = "D2K_AI_ContReadChannel error!";
		return false;
	}

    return true;
}

bool __fastcall TDaqAnalog::GetCrashCheck (long iModel, double *pfData)
{
	short	i = 0, j = 0, k = 0, iCount[100] = { 0 };
	float	fRate[100] = { 0.0 };
	bool	bRet = false;

	for (i = 1; i < 1000; i++)
	{
		if (fabs(pfData[i] - pfData[j]) >= 1)
		{
			iCount[k] = i - j + 1;
			k++;
			j = i + 1;
		}
	}

	if (iModel == 0)
	{
		for (i = 1; i < 100; i++)
		{
			if (iCount[i] == 0)
			{
				for (j = 1; j < (i - 2) / 2 * 2; j++)
				{
					fRate[j] = (float)iCount[2 * j] / iCount[2 * j - 1];
					if (abs(2 * j - i) <= 1)
					{
						k = j;
						break;
					}
				}
				break;
			}

		}
		for (i = 1; i < k; i++)
		{
			if (fabs(fRate[i] - 5.0) <= 0.2 || fabs(fRate[i] - 0.2) <= 0.02) bRet = true;
			else bRet = false;
		}
	} else
	{
		if (fabs(iCount[1] * 5 - 200.0) <= 2.0) bRet = true;
		else bRet = false;
	}
	return bRet;
}

bool __fastcall TDaqAnalog::SetParentObjectHandle()
{
    this->m_iHandleCard = g_daqDio.m_iHandleCard;
    return true;
}


