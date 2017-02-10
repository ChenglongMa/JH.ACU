
#pragma hdrstop

#include "GpibPcDMM.h"
#include <stdio.h>

#pragma package(smart_init)

TGpibPcDMM g_gpibPcDMM;

__fastcall TGpibPcDMM::TGpibPcDMM()
{
}

__fastcall TGpibPcDMM::~TGpibPcDMM()
{
}

///////////////////////////////////////////////////////////////////////////////
// caReadBuf[100] = {'\0'}
bool __fastcall TGpibPcDMM::GetCurr(float &fCurr)
{
	String	ansiStrCommand = "CONF:CURR:DC DEF";
	char	caReadBuf[100] = {0x00};

	do
	{
		ibwrt(m_iHandle, ansiStrCommand.c_str(), ansiStrCommand.Length());
		if (ibsta & ERR)	// failed
		{
			m_strRetCode.printf("0x%4X:\t", ibsta);
			m_strMark = m_strRetCode + "GetCurr:ibwrt Function call terminated on error";
		}
	}
	while (ibsta & ERR != 0);
	ibwait(m_iHandle, CMPL);

	ansiStrCommand = "READ?";
	ibwrt(m_iHandle, ansiStrCommand.c_str(), ansiStrCommand.Length());
	if (ibsta & ERR)		// failed
	{
		m_strRetCode.printf("0x%4X:\t", ibsta);
		m_strMark = m_strRetCode + "GetCurr:ibwrt Function call terminated on error";
		return false;
	}

	ibwait(m_iHandle, CMPL);

	ibrd(m_iHandle, caReadBuf, 100);
	if (ibsta & ERR)		// failed
	{
		m_strRetCode.printf("0x%4X:\t", ibsta);
		m_strMark = m_strRetCode + "ibrd Function call terminated on error";
		return false;
	}

	ibwait(m_iHandle, CMPL);

	caReadBuf[ibcnt] = 0;
	sscanf(caReadBuf, "%f", &fCurr);
	fCurr = fCurr * 1000.00;

	return true;
}

bool __fastcall TGpibPcDMM::GetVolt(float &fVolt)
{
	String	strWriteBuf = "CONF:VOLT:DC DEF";
	char	caReadBuf[100] = {0x00};

	ibwrt(m_iHandle, strWriteBuf.c_str(), strWriteBuf.Length());
	if (ibsta & ERR)	// failed
	{
		m_strRetCode.printf("0x%4X:\t", ibsta);
		m_strMark = m_strRetCode + "GetVolt:ibwrt Function call terminated on error";
		return false;
	}
	ibwait(m_iHandle, CMPL);

	strWriteBuf = "READ?";
	ibwrt(m_iHandle, strWriteBuf.c_str(), strWriteBuf.Length());
	if (ibsta & ERR)	// failed
	{
		m_strRetCode.printf("0x%4X:\t", ibsta);
		m_strMark = m_strRetCode + "GetVolt:ibwrt Function call terminated on error";
		return false;
	}
	ibwait(m_iHandle, CMPL);

	ibrd(m_iHandle, caReadBuf, 100);
	if (ibsta & ERR)	// failed
	{
		m_strRetCode.printf("0x%4X:\t", ibsta);
		m_strMark = m_strRetCode + "GetVolt:ibrd Function call terminated on error";
		return false;
	}
	ibwait(m_iHandle, CMPL);

	caReadBuf[ibcnt] = 0;
	sscanf(caReadBuf, "%f", &fVolt);

	return true;
}

bool __fastcall TGpibPcDMM::GetFRes(float &fRes)
{
	String	strWriteBuf = "CONF:FRES DEF";
	char	caReadBuf[100] = {0x00};

	ibwrt(m_iHandle, strWriteBuf.c_str(), strWriteBuf.Length());
	if (ibsta & ERR)	// failed
	{
		m_strRetCode.printf("0x%4X:\t", ibsta);
		m_strMark = m_strRetCode + "GetFRes:ibwrt Function call terminated on error";
		return false;
	}
	ibwait(m_iHandle, CMPL);

	strWriteBuf = "READ?";
	ibwrt(m_iHandle, strWriteBuf.c_str(), strWriteBuf.Length());
	if (ibsta & ERR)	// failed
	{
		m_strRetCode.printf("0x%4X:\t", ibsta);
		m_strMark = m_strRetCode + "GetFRes:ibwrt Function call terminated on error";
		return false;
	}
	ibwait(m_iHandle, CMPL);

	ibrd(m_iHandle, caReadBuf, 100);
	if (ibsta & ERR)	// failed
	{
		m_strRetCode.printf("0x%4X:\t", ibsta);
		m_strMark = m_strRetCode + "GetFRes:ibrd Function call terminated on error";
		return false;
	}
	ibwait(m_iHandle, CMPL);

	// caReadBuf[ibcnt] = 0;
	*(strchr(caReadBuf, '\n')) = 0;
	sscanf(caReadBuf, "%f", &fRes);

	return true;
}

bool __fastcall TGpibPcDMM::GetRes(float &fRes)
{
	String	strWriteBuf = "CONF:RES DEF";
	char	caReadBuf[100] = {0x00};

	ibwrt(m_iHandle, strWriteBuf.c_str(), strWriteBuf.Length());
	if (ibsta & ERR)	// failed
	{
		m_strRetCode.printf("0x%4X:\t", ibsta);
		m_strMark = m_strRetCode + "GetFRes:ibwrt Function call terminated on error";
		return false;
	}
	ibwait(m_iHandle, CMPL);

	strWriteBuf = "READ?";
	ibwrt(m_iHandle, strWriteBuf.c_str(), strWriteBuf.Length());
	if (ibsta & ERR)	// failed
	{
		m_strRetCode.printf("0x%4X:\t", ibsta);
		m_strMark = m_strRetCode + "GetFRes:ibwrt Function call terminated on error";
		return false;
	}
	ibwait(m_iHandle, CMPL);

	ibrd(m_iHandle, caReadBuf, 100);
	if (ibsta & ERR)	// failed
	{
		m_strRetCode.printf("0x%4X:\t", ibsta);
		m_strMark = m_strRetCode + "GetFRes:ibrd Function call terminated on error";
		return false;
	}
	ibwait(m_iHandle, CMPL);

	// caReadBuf[ibcnt] = 0;
	*(strchr(caReadBuf, '\n')) = 0;
	sscanf(caReadBuf, "%f", &fRes);

	return true;
}

bool __fastcall TGpibPcDMM::GetFreq(float &fFreq)
{
	String	strWriteBuf = "CONF:FREQ DEF";
	char	caReadBuf[100] = {0x00};

	ibwrt(m_iHandle, strWriteBuf.c_str(), strWriteBuf.Length());
	if (ibsta & ERR)	// failed
	{
		m_strRetCode.printf("0x%4X:\t", ibsta);
		m_strMark = m_strRetCode + "GetFreq:ibwrt Function call terminated on error";
		return false;
	}
	ibwait(m_iHandle, CMPL);

	strWriteBuf = "READ?";
	ibwrt(m_iHandle, strWriteBuf.c_str(), strWriteBuf.Length());
	if (ibsta & ERR)	// failed
	{
		m_strRetCode.printf("0x%4X:\t", ibsta);
		m_strMark = m_strRetCode + "GetFreq:ibwrt Function call terminated on error";
		return false;
	}
	ibwait(m_iHandle, CMPL);

	ibrd(m_iHandle, caReadBuf, 100);
	if (ibsta & ERR)	// failed
	{
		m_strRetCode.printf("0x%4X:\t", ibsta);
		m_strMark = m_strRetCode + "GetFreq:ibrd Function call terminated on error";
		return false;
	}
	ibwait(m_iHandle, CMPL);

	//caReadBuf[ibcnt] = 0;
    *(strchr(caReadBuf, '\n')) = 0;
	sscanf(caReadBuf, "%f", &fFreq);

	return true;
}

///////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////
// 设定三用表测量电压量程为20v，精确到0.1v
bool __fastcall TGpibPcDMM::SetRangeAndGetVolt(bool &bState, float &fVolt)
{
	String	strWriteBuf = "CONF:VOLT:DC 20,0.1";
	char	caReadBuf[20] = {0x00};

	ibwrt(m_iHandle, strWriteBuf.c_str(), strWriteBuf.Length());
	if (ibsta & ERR)	// failed
	{
		m_strRetCode.printf("0x%4X:\t", ibsta);
		m_strMark = m_strRetCode + "SetRange:ibwrt Function call terminated on error";
		bState = false;
		return false;
	}
	ibwait(m_iHandle, CMPL);

	strWriteBuf = "READ?";
	ibwrt(m_iHandle, strWriteBuf.c_str(), strWriteBuf.Length());
	if (ibsta & ERR)	// failed
	{
		m_strRetCode.printf("0x%4X:\t", ibsta);
		m_strMark = m_strRetCode + "SetRange:ibwrt Function call terminated on error";
		bState = false;
		return false;
	}
	ibwait(m_iHandle, CMPL);

	// 从设备中采样数据然后把数据放到缓冲区sdbuf中
	ibrd(m_iHandle, caReadBuf, 20);
	if (ibsta & ERR)	// failed
	{
		m_strRetCode.printf("0x%4X:\t", ibsta);
		m_strMark = m_strRetCode + "SetRange:ibrd Function call terminated on error";
		bState = false;
		return false;
	}
	ibwait(m_iHandle, CMPL);

	// 采样初始值放在变量volt中
	sscanf(caReadBuf, "%f", &fVolt);
	bState = true;

	return true;
}

// 设定三用表测量电压量程为20v，精确到0.1v
bool __fastcall TGpibPcDMM::SetRange(bool &bState)
{
	String	strWriteBuf = "CONF:VOLT:DC 20,0.1";

	ibwrt(m_iHandle, strWriteBuf.c_str(), strWriteBuf.Length());
	if (ibsta & ERR)	// failed
	{
		m_strRetCode.printf("0x%4X:\t", ibsta);
		m_strMark = m_strRetCode + "SetRange:ibwrt Function call terminated on error";
		bState = false;
		return false;
	}
	ibwait(m_iHandle, CMPL);

	bState = true;
	return true;
}

// 关闭三用表自动调零功能
bool __fastcall TGpibPcDMM::CloseAutoZero(bool &bState)
{
	String	strWriteBuf = "ZERO:AUTO OFF";

	ibwrt(m_iHandle, strWriteBuf.c_str(), strWriteBuf.Length());
	if (ibsta & ERR)	// failed
	{
		m_strRetCode.printf("0x%4X:\t", ibsta);
		m_strMark = m_strRetCode + "CloseAutoZero:ibwrt Function call terminated on error";
		bState = false;
		return false;
	}
	ibwait(m_iHandle, CMPL);

	bState = true;
	return true;
}

bool __fastcall TGpibPcDMM::SetTrigDelay(bool &bState)
{
	String	strWriteBuf = "TRIG:DEL 0.001347";

	ibwrt(m_iHandle, strWriteBuf.c_str(), strWriteBuf.Length());
	if (ibsta & ERR)	// failed
	{
		m_strRetCode.printf("0x%4X:\t", ibsta);
		m_strMark = m_strRetCode + "SetTrigDelay:ibwrt Function call terminated on error";
		bState = false;
		return false;
	}
	ibwait(m_iHandle, CMPL);

	bState = true;
	return true;
}

// 关闭三用表显示屏开始测量，为了有更高的采样速度
bool __fastcall TGpibPcDMM::CloseView(bool &bState)
{
	String	strWriteBuf = "DISP OFF";

	ibwrt(m_iHandle, strWriteBuf.c_str(), strWriteBuf.Length());
	if (ibsta & ERR)	// failed
	{
		m_strRetCode.printf("0x%4X:\t", ibsta);
		m_strMark = m_strRetCode + "CloseView:ibwrt Function call terminated on error";
		bState = false;
		return false;
	}

	ibwait(m_iHandle, CMPL);

	bState = true;
	return true; 
}

bool __fastcall TGpibPcDMM::SetHits()
{
	String	strWriteBuf = "SAMP:COUN 2500"; // 采样2500个点

	ibwrta(m_iHandle, strWriteBuf.c_str(), strWriteBuf.Length());
	if (ibsta & ERR)						// failed
	{
		m_strRetCode.printf("0x%4X:\t", ibsta);
		m_strMark = m_strRetCode + "SetHits:ibwrt Function call terminated on error";
		return false;
	}

	ibwait(m_iHandle, CMPL);

	return true;
}

bool __fastcall TGpibPcDMM::GetRangeValue(char caReadBuf[40000])
{
	String	strWriteBuf = "READ?";		// 启动测量

	ibwrt(m_iHandle, strWriteBuf.c_str(), strWriteBuf.Length());
	if (ibsta & ERR)					// failed
	{
		m_strRetCode.printf("0x%4X:\t", ibsta);
		m_strMark = m_strRetCode + "GetRangeValue:ibwrt Function call terminated on error";
		return false;
	}
	ibwait(m_iHandle, CMPL);

	ibrd(m_iHandle, caReadBuf, 40000);	// 从设备中采样数据然后把数据放到缓冲区rdbuf中
	if (ibsta & ERR)					// failed
	{
		m_strRetCode.printf("0x%4X:\t", ibsta);
		m_strMark = m_strRetCode + "GetRangeValue:ibrd Function call terminated on error";
		return false;
	}

	ibwait(m_iHandle, CMPL);

    // 结束符
	//int iEof = ibcnt;
	//caReadBuf[iEof] = 0;
    *(strchr(caReadBuf, '\n')) = 0;

	return true;
}


