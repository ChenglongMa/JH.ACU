#ifndef DaqAnalogH
#define DaqAnalogH

#include "DaqDio.h"

class TDaqAnalog : public TDaqDio
{
private:

protected:

public:
  __fastcall TDaqAnalog();
  __fastcall ~TDaqAnalog();

public:
	bool __fastcall GetPowerState();	// ��ȡ5V��12V��IGN��Sensor�ĵ�Դ��Ӧ���
	bool __fastcall GetVoltFromChannel(int iReadType, unsigned short iChannel, double &dVolt);
    bool __fastcall GetVoltFromChannelBySingle(unsigned short iChannel, double &dVolt);
    bool __fastcall GetVoltFromChannelByMulti(unsigned short iChannel, double &dVolt);
    bool __fastcall SetCrashConfig(short iBuf[5000]);
    bool __fastcall GetCrashCheck(long iModel, double *pfData);

public:
    bool __fastcall SetParentObjectHandle();
};

extern TDaqAnalog g_daqAnalog;
#endif
 