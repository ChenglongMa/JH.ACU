#ifndef GpibPcDMMH
#define GpibPcDMMH

#include "GpibDevice.h"

#define MES_PCDMM_PA 	22

class TGpibPcDMM : public TGpibDevice
{
private:

protected:

public:
  __fastcall TGpibPcDMM();
  __fastcall ~TGpibPcDMM();

public:
    // ����3�����ǵ��������
  	bool __fastcall GetCurr(float &fCurr);
  	bool __fastcall GetVolt(float &fVolt);
  	bool __fastcall GetFRes(float &fRes);
    bool __fastcall GetRes(float &fRes);
    bool __fastcall GetFreq(float &fFreq);

    // ���������������
    bool __fastcall SetRangeAndGetVolt(bool & bState, float & fVolt);
    bool __fastcall SetRange(bool & bState);
    bool __fastcall CloseAutoZero(bool & bState);
    bool __fastcall SetTrigDelay(bool & bState);
    bool __fastcall CloseView(bool & bState);
	bool __fastcall SetHits();
    bool __fastcall GetRangeValue(char caReadBuf[40000]);
};

extern TGpibPcDMM g_gpibPcDMM;

#endif
 