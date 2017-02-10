#include <vcl.h>
#pragma hdrstop

#include "GpibPcRes.h"

#pragma package(smart_init)

TGpibPcRes g_gpibPcResF;
TGpibPcRes g_gpibPcResS;

__fastcall TGpibPcRes::TGpibPcRes()
{
}

__fastcall TGpibPcRes::~TGpibPcRes()
{
}

bool __fastcall TGpibPcRes::SetRes(float fRes)
{
	String			strWriteBuf = "SOURce:DATA ";
	unsigned long	iRes = fRes * 1000;
	String			strBuf = String(iRes);
	int				iLength = strBuf.Length();

	for (int i = 0; i < (12 - iLength); i++) strWriteBuf = strWriteBuf + "0";
	strWriteBuf = strWriteBuf + strBuf;

	ibwrt(m_iHandle, strWriteBuf.c_str(), strWriteBuf.Length());
	if (ibsta & ERR)	// failed
	{
		m_strRetCode.printf("0x%4X:\t", ibsta);
		m_strMark = m_strRetCode + "SetOutPut:ibwrt Function call terminated on error";
		return false;
	}

	G_SDelay(200);
	return true;
}





