#include <vcl.h>
#pragma hdrstop

#include "GpibDevice.h"

#pragma package(smart_init)

__fastcall TGpibDevice::TGpibDevice()
{
    m_iInterfaceHandle = -1;
    m_iHandle = -1;
    m_iDevice = -1;
    m_strMark = "";
    m_strRetCode = "";
}
__fastcall TGpibDevice::~TGpibDevice()
{
}

bool __fastcall TGpibDevice::SetInterfaceHandle(TGpibInterface gpibInterface)
{
    bool    bRet = gpibInterface.GetHandle(m_iInterfaceHandle);
    if (false == bRet)
        return false;

    return true;
}

void __fastcall TGpibDevice::SetPA (int iDevice)
{
    m_iDevice = iDevice;
}

void __fastcall TGpibDevice::GetPA(int &iDevice)
{
	iDevice = m_iDevice;
}

// 注意：必须用对象
// 必须保证 g_gpibInterface已开启
bool __fastcall TGpibDevice::SetHandle()
{
    // open the device by using ibdev() function with primary address stored
    // in m_siDevicePA
    m_iHandle = ibdev(m_iInterfaceHandle, m_iDevice, 0, T10s, 1, 0);
    if (ibsta & ERR)    // failed
    {
        m_strRetCode.printf("0x%4X:\t", ibsta);
        m_strMark = m_strRetCode + "ibdev Function call terminated on error";
        return false;
    }

    return true;
}

bool __fastcall TGpibDevice::GetHandle(int &iHandle)
{
	iHandle = m_iHandle;
    if(m_iHandle < 0)
    	return false;

    return true;
}

///////////////////////////////////////////////////////////////////////////////
// reset the instrument by using ibonl() function
// ibonl(m_iHandle, 0);
///////////////////////////////////////////////////////////////////////////////
bool __fastcall TGpibDevice::GetIDN()
{
	String	strWriteBuf = "*IDN?";
	char	caReadBuf[256] = { 0x00 };

	ibwrt(m_iHandle, strWriteBuf.c_str(), strWriteBuf.Length());
	if (ibsta & ERR)	// failed
	{
		m_strRetCode.printf("0x%4X:\t", ibsta);
		m_strMark = m_strRetCode + "ibwrt Function call terminated on error";
		return false;
	}
	ibwait(m_iHandle, CMPL);

	ibrd(m_iHandle, caReadBuf, 255);
	if (ibsta & ERR)	// failed
	{
		m_strRetCode.printf("0x%4X:\t", ibsta);
		m_strMark = m_strRetCode + "ibrd Function call terminated on error";
		return false;
	}
	ibwait(m_iHandle, CMPL);

	return true;
}

bool __fastcall TGpibDevice::SetRST()
{
	String	strWriteBuf = "*RST";

	ibwrt(m_iHandle, strWriteBuf.c_str(), strWriteBuf.Length());
	if (ibsta & ERR)	// failed
	{
		m_strRetCode.printf("0x%4X:\t", ibsta);
		m_strMark = m_strRetCode + "ibwrt Function call terminated on error";
		return false;
	}
	ibwait(m_iHandle, CMPL);

	return true;
}

bool __fastcall TGpibDevice::SetTST()
{
	String	strWriteBuf = "*TST?";
	char	caReadBuf[256] = { 0x00 };

	ibwrt(m_iHandle, strWriteBuf.c_str(), strWriteBuf.Length());
	if (ibsta & ERR)	// failed
	{
		m_strRetCode.printf("0x%4X:\t", ibsta);
		m_strMark = m_strRetCode + "ibwrt Function call terminated on error";
		return false;
	}
	ibwait(m_iHandle, CMPL);

	ibrd(m_iHandle, caReadBuf, 255);
	if (ibsta & ERR)	// failed
	{
		m_strRetCode.printf("0x%4X:\t", ibsta);
		m_strMark = m_strRetCode + "ibwrt Function call terminated on error";
		return false;
	}
	ibwait(m_iHandle, CMPL);

	return true;
}

bool __fastcall TGpibDevice::Init(TGpibInterface gpibInterface, int iDevice)
{
	if (m_iHandle >= 0)
    {
        return true;
    }

	bool	bRet = SetInterfaceHandle(gpibInterface);
	if (false == bRet) return false;

	SetPA(iDevice);

	// 初始化设备
	bRet = SetHandle();
	return bRet;
}

bool __fastcall TGpibDevice::Close()
{
	if (m_iHandle < 0) return true;

	ibonl(m_iHandle, 0);
	m_iHandle = -1;
	if (ibsta & ERR)	// failed
	{
		m_strRetCode.printf("0x%4X:\t", ibsta);
		m_strMark = m_strRetCode + "ibwrt Function call terminated on error";
		return false;
	}

	return true;
}

void __fastcall TGpibDevice::SetRetCode()
{
	char	caReadBuf[256] = { 0x00 };
	sprintf(caReadBuf, "0x%4X", ibsta);
	m_strRetCode = caReadBuf;
}




