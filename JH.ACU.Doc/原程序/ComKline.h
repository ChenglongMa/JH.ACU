#ifndef ComKlineH
#define ComKlineH

#include "Com.h"
#include "Global.h"

// 串口设备与ECU的通信是否正常
// 是否找到错误码
#define MES_KLINE_ZERO		-1
#define MES_KLINE_NOTFOUND	0
#define MES_KLINE_FOUND		1

#define MES_COM_KLINE 		"COM1"
#define MES_SMODE_LEN		19
#define MES_REPEAT_BASE		3

#define MES_ACUID					0  		// ECU Identification
#define MES_REALTIMEFAULT_CHECK		1 		// Real time fault & status check
#define MES_INTERRUPT_SMODE			2       // Interrupt S-mode (user function)
#define MES_REALTIMEVALUE_CHECK		3       // Real time value check
#define MES_CLEAR_FAULT_RECORD		4       // Clear Fault Record
#define MES_CLEAR_EEPROM_RECORD		5       // Clear EEPROM Crash Record
#define MES_CLEAR_FRAM_RECORD		6       // Clear FRAM Crash Record
#define MES_START_DIAG				7       // Start Diag.
#define MES_STOP_DIAG				8       // Stop Diag.
#define MES_CRASH_OUT				9       // Crash Out
#define MES_CLEAR_OTC				10      // Clear Operation Timer Counter Area
#define MES_CLEAR_FIC				11      // Clear FRAM IGN Counter
#define MES_ERASE_FRAM				12      // FRAM Mass Erase
#define MES_ERASE_EEPROM			13      // EEPROM Mass Erase
#define MES_WL1_ON					14      // WL1 On
#define MES_WL1_OFF					15      // WL1 Off
#define MES_WL2_ON					16      // WL2 On
#define MES_WL2_OFF					17      // WL2 Off
#define MES_RESET_RSA				18      // Front Side Algorithm Reset
#define MES_RESET_ECU				19      // ECU Reset

class TComKline : public TCom
{
private:
	void __fastcall ResetKlineState();

public:
  __fastcall TComKline();
  __fastcall ~TComKline();

public:
	bool	m_bKline;

public:
  virtual bool __fastcall Open();
  virtual bool __fastcall Close();

public:
  	// SMode协议功能
  	bool __fastcall OpenSMode();
  	bool __fastcall CloseSMode();
  	bool __fastcall ReadMemory(short iAddress, short iLength, String &strInfo);
  	bool __fastcall WriteMemory(short iAddress, short iLength, String &strInfo);
    bool __fastcall ReadFRAM(short iAddress, short iLength, String &strInfo);
    bool __fastcall WriteFRAM(short iAddress, short iLength, String &strInfo);
    bool __fastcall ReadEEPROM(short iAddress, short iLength, String &strInfo);
    bool __fastcall WriteEEPROM(short iAddress, short iLength, String &strInfo);
  	bool __fastcall OpenRealtimeFault();
  	bool __fastcall CloseRealtimeFault();
  	bool __fastcall EnableWarnLamp(int iLamp, bool bEnable);
  	bool __fastcall EnableCrashOutput();
    bool __fastcall RealTimeValueCheck(int iCheckValueCode, int &iResult);
    bool __fastcall ExecuteCommand(const int iCommandId, unsigned char *caBufRead);

  	// 针对ECU提取出的函数 有的是在RTF下，有的不是
    bool __fastcall ReadChar(unsigned char & cRet);						// 所有状态，只要涉及到读，都要调用
    bool __fastcall ReadBuf(unsigned char * caBuf, unsigned int iNum);	// 与ECU通信协议没有关系
  	bool __fastcall Write(unsigned char * caBuf, unsigned int iNum);	// 命令写，并进行了回读，成功表示串口设备正常
  	bool __fastcall Read(unsigned char * caBuf, unsigned char cChar, bool & bKline);	// 开启Smode后，对命令操作的反馈
  	int  __fastcall SearchChar(unsigned char cChar, unsigned char *caBuf, unsigned int & iReadCount); 	// RTF
};

extern TComKline g_comKline;

#endif
