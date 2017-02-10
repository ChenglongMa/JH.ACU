#ifndef DaqDigitH
#define DaqDigitH

#include "DaqDio.h"

#define MES_FIND	0
#define MES_MESURE	1

class TDaqDigit : public TDaqDio
{
private:
    // 第一种操作方式
    void __fastcall GetRange(int iRelay, int & iLow, int & iHigh);

    // 第二种操作方式
    void __fastcall GetCurrMask(int iCardNum, int iRelay, bool bEnable, int & iGroup, int & iMask);
    void __fastcall GetPrevMask(int iCardNum, int iGroup, int & iMask); // 板卡移动时，原先的状态
    static int m_siCurrSubBoard;        // 当前在那块子版上操作
    static int m_siCurrGroup;           // 当前操作的子版继电器组
    static const int m_siRelayGroup[MES_ITEM_RET][3];

public:
    static int m_ulMbRelayStatus;
    static int m_scRelayStatus[MES_SUBBOARD_NUM][MES_RELAYGROUP];

protected:

public:
  __fastcall TDaqDigit();
  __fastcall ~TDaqDigit();

public:
	bool __fastcall EnableIgnWL();
    bool __fastcall EnableIgn(int iCardNum, bool bEnable);
    bool __fastcall EnableWL(int iCardNum, bool bEnable);
    bool __fastcall EnableKline(int iCardNum, bool bEnable);
    bool __fastcall EnableCrash(int iCardNum, bool bEnable);

    bool __fastcall ResetRelay();
    bool __fastcall EnableRelayBase(int iCardNum, bool bEnable);
    bool __fastcall EnableRelayGroup(int iItem, int iType);	// iItem：测试步骤 iType：类型0-查找1-万用表测值

    // 第一种操作方式
    // 其中iCardNum从0开始,表示第几块子板,iRelay表示继电器的标号,都是3位数,例如200,201
    bool __fastcall RelayControlSoft(int iCardNum, int iRelay, bool bEnable); 	// 主功能
    bool __fastcall RelayAction(int iCardNum, int iRelay, bool bEnable);        // 次功能
    bool __fastcall Enable138(int iCardNum);									// 继电器使能

    // 第二种操作方式
    bool __fastcall SetSbRelayGroupStatus(int iCardNum, int iGroup, int iStatus);
    bool __fastcall SetSbRelay(int iCardNum, int iGroup, int iBit, bool bEnable);
    bool __fastcall SetSbRelay(int iCardNum, int iRelay, bool bEnable);
    bool __fastcall SetMbRelay(int iRelay, bool bEnable);						// 公共

public:
    bool __fastcall SetParentObjectHandle();
};

extern TDaqDigit g_daqDigit;

#endif
 