#include <vcl.h>
#pragma hdrstop

#include "GpibInterface.h"


#pragma package(smart_init)

TGpibInterface 	g_gpibInterface;

__fastcall TGpibInterface::TGpibInterface()
{
    m_iHandle = -1;
    m_strMark = "";
    m_strRetCode = "";
}

__fastcall TGpibInterface::~TGpibInterface()
{
}

bool __fastcall TGpibInterface::SetHandle(int iInterface)
{
	if(m_iHandle >= 0)
    	return true;
    String	strTemp = "";
	strTemp.printf("GPIB%d", iInterface);

    m_iHandle = ibfind(strTemp.c_str());
    if(ibsta & ERR) //failed
    {
      	SetRetCode();
        m_strMark = "ibfind Function call terminated on error";
    	return false;
    }

    if(m_iHandle >= 0)
		return true;

    return false;
}

bool __fastcall TGpibInterface::GetHandle(int &iHandle)
{
	iHandle = m_iHandle;
    if(m_iHandle >= 0)
		return true;

    return false;
}

///////////////////////////////////////////////////////////////////////////////
//ibonl(m_iHandle, 0);
//if(ibsta & ERR) //failed
//	return false;
///////////////////////////////////////////////////////////////////////////////
bool __fastcall TGpibInterface::SetConfig(GPIB_INI_Setting GPIBSetting)
{
	if (m_iHandle >= 0)
	{
		// change the primary address of GPIB card and set as GPIB system controller
		ibrsc(m_iHandle, 0);
        if(ibsta & ERR) //failed
        {
        	SetRetCode();
            m_strMark = "ibrsc Function call terminated on error";
			return false;
        }

		ibpad(m_iHandle, GPIBSetting.m_iPad);
        if(ibsta & ERR) //failed
        {
        	SetRetCode();
            m_strMark = "ibpad Function call terminated on error";
			return false;
        }

		ibrsc(m_iHandle, 1);
        if(ibsta & ERR) //failed
        {
        	SetRetCode();
            m_strMark = "ibrsc Function call terminated on error";
			return false;
        }

		SendIFC(m_iHandle);
        if(ibsta & ERR) //failed
        {
        	SetRetCode();
            m_strMark = "SendIFC Function call terminated on error";
			return false;
        }

        return true;
	}

	return false;
}

///////////////////////////////////////////////////////////////////////////////
// reset the GPIB interface card by using ibonl() function
// ibonl(m_iHandle, 0);
// if(ibsta & ERR) //failed
//		return false;
///////////////////////////////////////////////////////////////////////////////
bool __fastcall TGpibInterface::GetDeviceCount(int &iDeviceCount)
{
	Addr4882_t	iPAList[31] = {0x00};		// Array of primary addresses
	Addr4882_t	iResultList[31] = {0x00};	// Array of listen addresses

    if(m_iHandle < 0)
		return false;

    // if the corresponding GPIB card is successfully opened
	if (m_iHandle >= 0)
	{
		// reset the GPIB by sending interface clear command
		SendIFC(m_iHandle);
        if(ibsta & ERR) //failed
        {
        	SetRetCode();
            m_strMark = "SendIFC Function call terminated on error";
			return false;
        }

		// find all listener by using FindLstn() function
		for (int j = 0; j < 30; j++)
		{
			iPAList[j] = (Addr4882_t) (j);
			iResultList[j] = 0xffff;
		}
		iPAList[30] = NOADDR;
		FindLstn(m_iHandle, iPAList, iResultList, 31);
        // ibcntl contains the actual number of addresses stored in ResultList.
		iDeviceCount = ibcntl;
        if(ibsta & ERR) //failed
        {
        	SetRetCode();
            m_strMark = "FindLstn Function call terminated on error";
			return false;
        }
	}


	return true;
}

bool __fastcall TGpibInterface::Close()
{
	if(m_iHandle < 0)
    	return true;

	ibonl(m_iHandle, 0);
    m_iHandle = -1;
    if(ibsta & ERR) //failed
    {
    	SetRetCode();
        m_strMark = "ibonl Function call terminated on error";
    	return false;
    }
    return true;
}

void __fastcall TGpibInterface::GetMark(String strMark)
{
    strMark = m_strMark;
}

void __fastcall TGpibInterface::SetRetCode()
{
	char	caReadBuf[256] = { 0x00 };
	sprintf(caReadBuf, "0x%4X", ibsta);
	m_strRetCode = caReadBuf;
}




