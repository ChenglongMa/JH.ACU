#ifndef ComKlineH
#define ComKlineH

#include "Com.h"
#include "Global.h"

// �����豸��ECU��ͨ���Ƿ�����
// �Ƿ��ҵ�������
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
  	// SModeЭ�鹦��
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

  	// ���ECU��ȡ���ĺ��� �е�����RTF�£��еĲ���
    bool __fastcall ReadChar(unsigned char & cRet);						// ����״̬��ֻҪ�漰��������Ҫ����
    bool __fastcall ReadBuf(unsigned char * caBuf, unsigned int iNum);	// ��ECUͨ��Э��û�й�ϵ
  	bool __fastcall Write(unsigned char * caBuf, unsigned int iNum);	// ����д���������˻ض����ɹ���ʾ�����豸����
  	bool __fastcall Read(unsigned char * caBuf, unsigned char cChar, bool & bKline);	// ����Smode�󣬶���������ķ���
  	int  __fastcall SearchChar(unsigned char cChar, unsigned char *caBuf, unsigned int & iReadCount); 	// RTF
};

extern TComKline g_comKline;

#endif
