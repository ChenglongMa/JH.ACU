#ifndef DaqDioH
#define DaqDioH

#include "Global.h"
#include "D2kDask.h"

#define MES_PA Channel_P1A
#define MES_PB Channel_P1B
#define MES_PC Channel_P1C

// ����CrashOut�Ĳ�������
#define MES_FREQ_TIMEBASE   40000000
#define MES_SCAN_COUNT      5000
#define MES_BUF_RESET   	1
#define MES_SCAN_INTERVAL  	40000
#define MES_ANATRIG_H 		0x80
#define MES_ANATRIG_L 		0x80

// ���ֲɼ����ز��ѹ��Ӧ
#define MES_VOLT_CRASH			0
#define MES_VOLT_5VCH			1
#define MES_VOLT_12VCH			2
#define MES_VOLT_IGNCH			3
#define MES_APMPLIFY_12V		2		// (4.7K + 4.7K)/4.7K
#define MES_APMPLIFY_IGN		3.12	// (10.0k + 4.7K)/4.7k

// ��D2K-DASK���ķ�װ������ģ�ⲿ��AI�����ֲ���DO
// ����ģ�ⲿ��AI ��ȡ��������  ע��Ҫ��������
// ���ֲ���DO ���ڼ̵����Ŀ���

class TDaqDio
{
private:

protected:

public:
	I16 m_iHandleCard;
    String m_strMark;

public:
  __fastcall TDaqDio();
  __fastcall ~TDaqDio();

public:
	 bool __fastcall Open();
     bool __fastcall Close();
     bool __fastcall GetHadle(int &iHandle);
};

extern TDaqDio g_daqDio;

#endif
 