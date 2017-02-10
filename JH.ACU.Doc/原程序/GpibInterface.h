#ifndef GpibInterfaceH
#define GpibInterfaceH

#include "Adgpib_bc.h"
#include <stdio.h>

///////////////////////////////////////////////////////////////////////////////
#ifndef _GPIB_Setting_Structure_H
#define _GPIB_Setting_Structure_H

#ifdef __cplusplus
extern "C" {
#endif

typedef	struct tagGPIB_INI_Setting
{
	Addr4882_t		m_iPad;				//primary address
	Addr4882_t		m_iSad;				//secondart address
	int				m_iT1;				//t1 delays
	int				m_iBusTimeOut;		//GPIB bus timeoue
	bool			m_bSystemControl;	//enable/disable system control
	bool			m_bAutoPolling;		//enable/disable auto polling
} GPIB_INI_Setting;

#ifdef __cplusplus
}
#endif

#endif
///////////////////////////////////////////////////////////////////////////////

class TGpibInterface
{
private:
  	int m_iHandle;
    String m_strMark;
    String m_strRetCode;

protected:

public:
  __fastcall TGpibInterface();
  __fastcall ~TGpibInterface();

public:
  	bool __fastcall SetHandle(int iInterface);
  	bool __fastcall GetHandle(int &iHandle);
  	bool __fastcall SetConfig(GPIB_INI_Setting GPIBSetting);
  	bool __fastcall GetDeviceCount(int &iDeviceCount);
    bool __fastcall Close();

    void __fastcall GetMark(String strMark);
    void __fastcall SetRetCode();
};

extern TGpibInterface 	g_gpibInterface;

#endif
 