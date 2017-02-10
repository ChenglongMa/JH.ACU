#pragma hdrstop

#include "DaqDio.h"
#include <math.h>

#pragma package(smart_init)

TDaqDio g_daqDio;

__fastcall TDaqDio::TDaqDio()
{
	m_iHandleCard = -1;
	m_strMark = "";
}

__fastcall TDaqDio::~TDaqDio()
{

}

// Init2206Card
bool __fastcall TDaqDio::Open()
{
	// 2206 已经成功初始化
	if (m_iHandleCard >= 0) return true;

	// 2206 card 未初始化或初始化不成功
	try
	{
		m_iHandleCard = D2K_Register_Card(DAQ_2206, 0);
		if (m_iHandleCard >= 0)
		{
			// config all PIO in OUTPUT mode
			D2K_DIO_PortConfig(m_iHandleCard, Channel_P1A, OUTPUT_PORT);
			D2K_DIO_PortConfig(m_iHandleCard, Channel_P1B, OUTPUT_PORT);
			D2K_DIO_PortConfig(m_iHandleCard, Channel_P1C, OUTPUT_PORT);

			// init. DO register value
			D2K_DO_WritePort(m_iHandleCard, Channel_P1A, 0x00);
			D2K_DO_WritePort(m_iHandleCard, Channel_P1B, 0x10);
			D2K_DO_WritePort(m_iHandleCard, Channel_P1C, 0x00);

			// AI Auto calibration
			// D2K_DB_Auto_Calibration_ALL(m_siCardHandle);
		}
	}
	catch(Exception & exception)
	{
		return false;
	}

	return true;	// 初始化成功
}

bool __fastcall TDaqDio::Close()
{
	try
	{
		if (m_iHandleCard >= 0) D2K_Release_Card(m_iHandleCard);
	}
	catch(Exception & exception)
	{
        m_iHandleCard = -1;
		return false;
	}

    m_iHandleCard = -1;
	return true;
}

bool __fastcall TDaqDio::GetHadle(int &iHandle)
{
	iHandle = m_iHandleCard;
    if(m_iHandleCard < 0)
    	return false;

    return true;
}


