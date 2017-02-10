#ifndef GpibPcPowerH
#define GpibPcPowerH

#include "GpibDevice.h"

#define MES_PCPOWER_PA 	8

// 缺ibonl
// m_sbInit和Release是为了测试流程的需要

class TGpibPcPower : public TGpibDevice
{
private:

protected:

public:
  __fastcall TGpibPcPower();
  __fastcall ~TGpibPcPower();

public:
  // 程控电源具有状态保持功能
  bool __fastcall SetVolt(float fVolt);
  bool __fastcall GetVolt(float &fVolt);
  bool __fastcall SetCurr(float fCurr);
  bool __fastcall GetCurr(float &fCurr);
  bool __fastcall SetOCP(bool bEnable = true);
  bool __fastcall GetOCPState(bool &bEnable);
  bool __fastcall SetOVP(bool bEnable = true);
  bool __fastcall GetOVPState(bool &bEnable);
  bool __fastcall SetOutPut(bool bEnable = true);
  bool __fastcall GetOutPutState(bool &bEnable);
  bool __fastcall GetState(String & strSate);

  // Gpib通信 原子操作
  bool __fastcall Write(char *Str);
  bool __fastcall Read(char *Str);
};

extern TGpibPcPower g_gpibPcPower;

#endif
 