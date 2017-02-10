#ifndef DaqDigitH
#define DaqDigitH

#include "DaqDio.h"

#define MES_FIND	0
#define MES_MESURE	1

class TDaqDigit : public TDaqDio
{
private:
    // ��һ�ֲ�����ʽ
    void __fastcall GetRange(int iRelay, int & iLow, int & iHigh);

    // �ڶ��ֲ�����ʽ
    void __fastcall GetCurrMask(int iCardNum, int iRelay, bool bEnable, int & iGroup, int & iMask);
    void __fastcall GetPrevMask(int iCardNum, int iGroup, int & iMask); // �忨�ƶ�ʱ��ԭ�ȵ�״̬
    static int m_siCurrSubBoard;        // ��ǰ���ǿ��Ӱ��ϲ���
    static int m_siCurrGroup;           // ��ǰ�������Ӱ�̵�����
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
    bool __fastcall EnableRelayGroup(int iItem, int iType);	// iItem�����Բ��� iType������0-����1-���ñ��ֵ

    // ��һ�ֲ�����ʽ
    // ����iCardNum��0��ʼ,��ʾ�ڼ����Ӱ�,iRelay��ʾ�̵����ı��,����3λ��,����200,201
    bool __fastcall RelayControlSoft(int iCardNum, int iRelay, bool bEnable); 	// ������
    bool __fastcall RelayAction(int iCardNum, int iRelay, bool bEnable);        // �ι���
    bool __fastcall Enable138(int iCardNum);									// �̵���ʹ��

    // �ڶ��ֲ�����ʽ
    bool __fastcall SetSbRelayGroupStatus(int iCardNum, int iGroup, int iStatus);
    bool __fastcall SetSbRelay(int iCardNum, int iGroup, int iBit, bool bEnable);
    bool __fastcall SetSbRelay(int iCardNum, int iRelay, bool bEnable);
    bool __fastcall SetMbRelay(int iRelay, bool bEnable);						// ����

public:
    bool __fastcall SetParentObjectHandle();
};

extern TDaqDigit g_daqDigit;

#endif
 