#ifndef GpibPcResH
#define GpibPcResH

#include "GpibDevice.h"

#define MES_PCRES_PAF	16
#define MES_PCRES_PAS	17

class TGpibPcRes : public TGpibDevice
{
private:

protected:

public:
  __fastcall TGpibPcRes();
  __fastcall ~TGpibPcRes();

public:
  	bool __fastcall SetRes(float fRes);

};

extern TGpibPcRes g_gpibPcResF;
extern TGpibPcRes g_gpibPcResS;

#endif
 