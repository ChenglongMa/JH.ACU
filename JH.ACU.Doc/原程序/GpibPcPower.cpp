#include <vcl.h>
#pragma hdrstop

#include "GpibPcPower.h"
#include <stdio.h>

#pragma package(smart_init)

TGpibPcPower g_gpibPcPower;

__fastcall TGpibPcPower::TGpibPcPower()
{
}

__fastcall TGpibPcPower::~TGpibPcPower()
{
}

bool __fastcall TGpibPcPower::SetVolt(float fVolt)
{
	String	strTemp = ":CHAN1:VOLT \n", strWriteBuf = "";
	strWriteBuf = strTemp.Insert(FloatToStr(fVolt), 13);

	ibwrt(m_iHandle, strWriteBuf.c_str(), strWriteBuf.Length());
	if (ibsta & ERR)	// failed
	{
		m_strRetCode.printf("0x%4X:\t", ibsta);
		m_strMark = m_strRetCode + "SetVolt:ibwrt Function call terminated on error";
		return false;
	}
	ibwait(m_iHandle, CMPL);

	return true;
}

bool __fastcall TGpibPcPower::GetVolt(float &fVolt)
{
	String	strWriteBuf = ":CHAN1:VOLT?\n"; // ":CHAN1:MEAS:VOLT?\n"
	char	caReadBuf[MES_BUFFER_LENGTH] = { 0x00 };

	ibwrt(m_iHandle, strWriteBuf.c_str(), strWriteBuf.Length());
	if (ibsta & ERR)						// failed
	{
		m_strRetCode.printf("0x%4X:\t", ibsta);
		m_strMark = m_strRetCode + "SetVolt:ibwrt Function call terminated on error";
		return false;
	}
	ibwait(m_iHandle, CMPL);

	ibrd(m_iHandle, caReadBuf, MES_BUFFER_LENGTH);
	if (ibsta & ERR)						// failed
	{
		m_strRetCode.printf("0x%4X:\t", ibsta);
		m_strMark = m_strRetCode + "SetVolt:ibrd Function call terminated on error";
		return false;
	}
	ibwait(m_iHandle, CMPL);

	sscanf(caReadBuf, "%f", &fVolt);
	return true;
}

bool __fastcall TGpibPcPower::SetCurr(float fCurr)
{
	String	strTemp = ":CHAN1:CURR \n", strWriteBuf = "";
	strWriteBuf = strTemp.Insert(FloatToStr(fCurr), 13);

	ibwrt(m_iHandle, strWriteBuf.c_str(), strWriteBuf.Length());
	if (ibsta & ERR)	// failed
	{
		m_strRetCode.printf("0x%4X:\t", ibsta);
		m_strMark = m_strRetCode + "SetCurr:ibwrt Function call terminated on error";
		return false;
	}
    ibwait(m_iHandle, CMPL);

	return true;
}

bool __fastcall TGpibPcPower::GetCurr(float &fCurr)
{
	String	strWriteBuf = ":CHAN1:CURR?\n"; // ":CHAN1:MEAS:CURR?\n";
	char	caReadBuf[MES_BUFFER_LENGTH] = { 0x00 };

	ibwrt(m_iHandle, strWriteBuf.c_str(), strWriteBuf.Length());
	if (ibsta & ERR)						// failed
	{
		m_strRetCode.printf("0x%4X:\t", ibsta);
		m_strMark = m_strRetCode + "GetCurr:ibwrt Function call terminated on error";
		return false;
	}
	ibwait(m_iHandle, CMPL);

	ibrd(m_iHandle, caReadBuf, MES_BUFFER_LENGTH);
	if (ibsta & ERR)						// failed
	{
		m_strRetCode.printf("0x%4X:\t", ibsta);
		m_strMark = m_strRetCode + "GetCurr:ibrd Function call terminated on error";
		return false;
	}
	ibwait(m_iHandle, CMPL);

	sscanf(caReadBuf, "%f", &fCurr);
	return true;
}

bool __fastcall TGpibPcPower::SetOCP(bool bEnable)
{
	String	strWriteBuf = "";
	if (true == bEnable) strWriteBuf = ":CHAN1:PROT:CURR 1\n";
	else strWriteBuf = ":CHAN1:PROT:CURR 0\n";

	ibwrt(m_iHandle, strWriteBuf.c_str(), strWriteBuf.Length());
	if (ibsta & ERR)	// failed
	{
		m_strRetCode.printf("0x%4X:\t", ibsta);
		m_strMark = m_strRetCode + "SetOCP:ibwrt Function call terminated on error";
		return false;
	}
	ibwait(m_iHandle, CMPL);

	return true;
}

bool __fastcall TGpibPcPower::GetOCPState(bool &bEnable)
{
	String	strWriteBuf = ":CHAN1:PROT:CURR?\n";
	char	caReadBuf[MES_BUFFER_LENGTH] = { 0x00 };

	ibwrt(m_iHandle, strWriteBuf.c_str(), strWriteBuf.Length());
	if (ibsta & ERR)	// failed
	{
		m_strRetCode.printf("0x%4X:\t", ibsta);
		m_strMark = m_strRetCode + "GetOCPState:ibwrt Function call terminated on error";
		return false;
	}
    ibwait(m_iHandle, CMPL);

	ibrd(m_iHandle, caReadBuf, MES_BUFFER_LENGTH);
	if (ibsta & ERR)	// failed
	{
		m_strRetCode.printf("0x%4X:\t", ibsta);
		m_strMark = m_strRetCode + "GetOCPState:ibrd Function call terminated on error";
		return false;
	}
	ibwait(m_iHandle, CMPL);

	int iRet = 0;
	sscanf(caReadBuf, "%i", &iRet);
	if (1 == iRet) bEnable = true;
	else bEnable = false;

	return true;
}

bool __fastcall TGpibPcPower::SetOVP(bool bEnable)
{
	String	strWriteBuf = "";
	if (true == bEnable) strWriteBuf = ":CHAN1:PROT:VOLT 1\n";
	else strWriteBuf = ":CHAN1:PROT:VOLT 0\n";

	ibwrt(m_iHandle, strWriteBuf.c_str(), strWriteBuf.Length());
	if (ibsta & ERR)	// failed
	{
		m_strRetCode.printf("0x%4X:\t", ibsta);
		m_strMark = m_strRetCode + "SetOVP:ibwrt Function call terminated on error";
		return false;
	}
	ibwait(m_iHandle, CMPL);

	return true;
}

bool __fastcall TGpibPcPower::GetOVPState(bool &bEnable)
{
	String	strWriteBuf = ":CHAN1:PROT:VOLT?\n";
	char	caReadBuf[MES_BUFFER_LENGTH] = { 0x00 };

	ibwrt(m_iHandle, strWriteBuf.c_str(), strWriteBuf.Length());
	if (ibsta & ERR)	// failed
	{
		m_strRetCode.printf("0x%4X:\t", ibsta);
		m_strMark = m_strRetCode + "GetOVPState:ibwrt Function call terminated on error";
		return false;
	}
    ibwait(m_iHandle, CMPL);

	ibrd(m_iHandle, caReadBuf, MES_BUFFER_LENGTH);
	if (ibsta & ERR)	// failed
	{
		m_strRetCode.printf("0x%4X:\t", ibsta);
		m_strMark = m_strRetCode + "GetOVPState:ibrd Function call terminated on error";
		return false;
	}
	ibwait(m_iHandle, CMPL);

	int iRet = 0;
	sscanf(caReadBuf, "%i", &iRet);
	if (1 == iRet) bEnable = true;
	else bEnable = false;

	return true;
}

// 计算机处理太快，而硬件反应不过来，需要延迟
bool __fastcall TGpibPcPower::SetOutPut(bool bEnable)
{
	String	strWriteBuf = ":OUTP:PROT:CLE\n";

	// 清除保护状态
	ibwrt(m_iHandle, strWriteBuf.c_str(), strWriteBuf.Length());
	if (ibsta & ERR)	// failed
	{
		m_strRetCode.printf("0x%4X:\t", ibsta);
		m_strMark = m_strRetCode + "SetOutPut:ibwrt Function call terminated on error";
		return false;
	}
    ibwait(m_iHandle, CMPL);

	if (true == bEnable) strWriteBuf = ":OUTP:STAT 1\n";
	else strWriteBuf = ":OUTP:STAT 0\n";

	ibwrt(m_iHandle, strWriteBuf.c_str(), strWriteBuf.Length());
	if (ibsta & ERR)	// failed
	{
		m_strRetCode.printf("0x%4X:\t", ibsta);
		m_strMark = m_strRetCode + "SetOutPut:ibwrt Function call terminated on error";
		return false;
	}
    ibwait(m_iHandle, CMPL);

	if (true == bEnable) G_SDelay(2000);
	return true;
}

bool __fastcall TGpibPcPower::GetOutPutState(bool &bEnable)
{
	String	strWriteBuf = ":OUTP:STAT?";
	char	caReadBuf[MES_BUFFER_LENGTH] = { 0x00 };

	ibwrt(m_iHandle, strWriteBuf.c_str(), strWriteBuf.Length());
	if (ibsta & ERR)	// failed
	{
		m_strRetCode.printf("0x%4X:\t", ibsta);
		m_strMark = m_strRetCode + "SetOutPut:ibwrt Function call terminated on error";
		return false;
	}
    ibwait(m_iHandle, CMPL);

	ibrd(m_iHandle, caReadBuf, MES_BUFFER_LENGTH);
	if (ibsta & ERR)	// failed
	{
		m_strRetCode.printf("0x%4X:\t", ibsta);
		m_strMark = m_strRetCode + "GetOVPState:ibrd Function call terminated on error";
		return false;
	}
	ibwait(m_iHandle, CMPL);

	int iRet = 0;
	sscanf(caReadBuf, "%i", &iRet);
	if (1 == iRet) bEnable = true;
	else bEnable = false;

	return true;
}

bool __fastcall TGpibPcPower::Write(char *Str)
{
	String	strWriteBuf = Str;
	ibwrt(m_iHandle, strWriteBuf.c_str(), strWriteBuf.Length());
	if (ibsta & ERR)	// failed
	{
		m_strRetCode.printf("0x%4X:\t", ibsta);
		m_strMark = m_strRetCode + "Write:ibwrt Function call terminated on error";
		return false;
	}
	ibwait(m_iHandle, CMPL);

	return true;
}

bool __fastcall TGpibPcPower::Read(char *Str)
{
	ibrd(m_iHandle, Str, MES_BUFFER_LENGTH);
	if (ibsta & ERR)	// failed
	{
		m_strRetCode = printf("0x%4X", ibsta);
		m_strMark = m_strRetCode + "Read:ibrd Function call terminated on error";
		return false;
	}
	ibwait(m_iHandle, CMPL);

	return true;
}

bool __fastcall TGpibPcPower::GetState(String &strSate)
{
	String	strWriteBuf = ":SYST:ERR?";
	char	caReadBuf[MES_BUFFER_LENGTH] = { 0x00 };

	ibwrt(m_iHandle, strWriteBuf.c_str(), strWriteBuf.Length());
	if (ibsta & ERR)	// failed
	{
		m_strRetCode.printf("0x%4X:\t", ibsta);
		m_strMark = m_strRetCode + "SetOutPut:ibwrt Function call terminated on error";
		return false;
	}
	ibwait(m_iHandle, CMPL);

	ibrd(m_iHandle, caReadBuf, MES_BUFFER_LENGTH);
	if (ibsta & ERR)	// failed
	{
		m_strRetCode.printf("0x%4X:\t", ibsta);
		m_strMark = m_strRetCode + "GetOVPState:ibrd Function call terminated on error";
		return false;
	}
	ibwait(m_iHandle, CMPL);

	return true;
}

