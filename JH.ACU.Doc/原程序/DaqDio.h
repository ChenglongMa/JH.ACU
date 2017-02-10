#ifndef DaqDioH
#define DaqDioH

#include "Global.h"
#include "D2kDask.h"

#define MES_PA Channel_P1A
#define MES_PB Channel_P1B
#define MES_PC Channel_P1C

// 进行CrashOut的测试所需
#define MES_FREQ_TIMEBASE   40000000
#define MES_SCAN_COUNT      5000
#define MES_BUF_RESET   	1
#define MES_SCAN_INTERVAL  	40000
#define MES_ANATRIG_H 		0x80
#define MES_ANATRIG_L 		0x80

// 数字采集卡回测电压供应
#define MES_VOLT_CRASH			0
#define MES_VOLT_5VCH			1
#define MES_VOLT_12VCH			2
#define MES_VOLT_IGNCH			3
#define MES_APMPLIFY_12V		2		// (4.7K + 4.7K)/4.7K
#define MES_APMPLIFY_IGN		3.12	// (10.0k + 4.7K)/4.7k

// 对D2K-DASK卡的封装，它有模拟部分AI和数字部分DO
// 其中模拟部分AI 获取连续数据  注意要集中起来
// 数字部分DO 用于继电器的控制

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
 