#ifndef ThreadMainH
#define ThreadMainH

#include <Classes.hpp> 

#define MES_TYPE_FC			0
#define MES_TYPE_SBELT		1
#define MES_TYPE_SIS		2

class TThreadMain : public TThread
{            
private:
	void __fastcall AutoTest();				// U01: 自动测试主函数
	void __fastcall ManualTest();			// U02: 手工测试主函数
	void __fastcall TestHasFaultCode();		// U03: 有错误码测试总函数
	void __fastcall TestSquib(int iSubItem, int iMethod);	// U04:
    void __fastcall TestSbelt(int iSubItem, int iMethod);	// U05:
    void __fastcall TestBatt(int iSubItem);					// U06:
    void __fastcall TestSIS(int iSubItem, int iMethod);		// U07:
    void __fastcall TestSISSensor(int iSubItem);			// U08:
	int __fastcall FindSquib(float fMin, float fMax, unsigned int iFaultCode, int iMethod, float & fRet);	// U08:
	int __fastcall FindSbelt(float fMin, float fMax, unsigned int iFaultCode, float & fRet);				// U09:
	int __fastcall FindBatt(float fMin, float fMax, unsigned int iFaultCode, int iMethod, float & fVolt);	// U10:
	int __fastcall FindSIS(float fMin, float fMax, unsigned int iFaultCode, int iMethod, float & fRes);		// U11:
    int __fastcall FindSISSensor(int iSubItem, int &iCodeRet);
	void __fastcall TestWarnLamp();								// U12:
    bool __fastcall GetWarnLmapVolt(int iLamp, bool bMethod);	// U13:
	bool __fastcall GetWarnLampCurr(int iLamp, bool bMethod);	// U14:
    void __fastcall TestHoldTime();		  				// U16:
	void __fastcall TestACUCurr();						// U15:
	void __fastcall TestCrashOut();						// U17:
    bool __fastcall NeedTestHasFault(int iBoard);		// U18:
    bool __fastcall NeedTestShortToVBatt(int iBoard);
    bool __fastcall GetFaultCodeState(unsigned int iFaultCode, bool & bKline);												// U19:
	bool __fastcall GetFaultCodeStateByItem(int iMethod, float fRes, unsigned int iFaultCode, bool & bKline, int iBlock);	// U20:
	bool __fastcall GetFaultCodeStateByVolt(int iMethod, float fVolt, unsigned int iFaultCode, bool & bKline);				// U21:
public:
    void __fastcall ShowTip();
	void __fastcall ViewProgressInfo(bool bPos, int iPos);
    void __fastcall SetTempByState(int iTempState);
    bool __fastcall SelfCheckDevice();
    bool __fastcall SelfCheckPcRes();	// 自检电阻箱1、2到Remote
    bool __fastcall ResetDevice();		// 硬件重置
protected:
	void __fastcall Execute(); 	// U00: 线程执行函数
public:
	__fastcall TThreadMain(bool CreateSuspended);
};

extern TThreadMain *g_pThreadMain;

#endif
