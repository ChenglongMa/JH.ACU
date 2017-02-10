
#pragma hdrstop

#include "DaqDigit.h"

#pragma package(smart_init)

TDaqDigit g_daqDigit;

// �忨 �̵�����
int TDaqDigit::m_ulMbRelayStatus = 0x00;
// �忨 �̵�����
int TDaqDigit::m_scRelayStatus[MES_SUBBOARD_NUM][MES_RELAYGROUP] = {
	0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
	0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
	0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
	0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
	0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
	0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00};

const int TDaqDigit::m_siRelayGroup[MES_ITEM_RET][3] = {
	// FC1-FC10
	{215, 217}, 		{221, 223}, 		{225, 227}, 		{231, 233}, 		{235, 237},
    {241, 243}, 		{245, 247}, 		{251, 253}, 		{255, 257}, 		{261, 263},

    {215, 217}, 		{221, 223}, 		{225, 227}, 		{231, 233}, 		{235, 237},
    {241, 243}, 		{245, 247}, 		{251, 253}, 		{255, 257}, 		{261, 263},

    {217, 300, 302}, 	{223, 300, 302},	{227, 300, 302},	{233, 300, 302},	{237, 300, 302},
    {243, 300, 302}, 	{247, 300, 302},	{253, 300, 302},	{257, 300, 302},	{263, 300, 302},

    {217, 300, 301}, 	{223, 300, 301},	{227, 300, 301},	{233, 300, 301},	{237, 300, 301},
    {243, 300, 301}, 	{247, 300, 301},	{253, 300, 301},	{257, 300, 301},	{263, 300, 301},

    // DSB��PSB��PADS
    {201, 203, 302}, 	{201, 203, 302},	{201, 203, 302},
    {205, 207, 302}, 	{205, 207, 302},	{205, 207, 302},
    {211, 213, 302}, 	{211, 213, 302},	{211, 213, 302},

    // BATT
    {0x00},
    {0x00},

    // SIS
    {280, 301}, {281, 301},	{282, 301},	{283, 301},
    {280, 302}, {281, 302},	{282, 302},	{283, 302},
    {280, 302}, {281, 302},	{282, 302},	{283, 302}};

int TDaqDigit::m_siCurrSubBoard = -1;
int TDaqDigit::m_siCurrGroup = -1;

__fastcall TDaqDigit::TDaqDigit()
{
}

__fastcall TDaqDigit::~TDaqDigit()
{
}

bool __fastcall TDaqDigit::EnableIgnWL()
{
	bool bRet = false;

	for (I16 i = MES_SUBBOARD_NUM - 1; i >= 0; i--)
	{   // �õ���
        bRet = SetSbRelay(i, 264, true);	// WL1,2 on
        if(false == bRet)	return false;
 		bRet = SetSbRelay(i, 265, true);	// ign on
        if(false == bRet)	return false;
	}
    
    return true;
}

bool __fastcall TDaqDigit::EnableIgn(int iCardNum, bool bEnable)
{
	// ign on/off
	bool bRet = SetSbRelay(iCardNum, 265, bEnable);
    if(false == bRet)	return false;

	return true;
}

bool __fastcall TDaqDigit::EnableWL(int iCardNum, bool bEnable)
{
	// wl on/off
	bool bRet = SetSbRelay(iCardNum, 264, bEnable);
    if(false == bRet)	return false;

	return true;
}

bool __fastcall TDaqDigit::EnableKline(int iCardNum, bool bEnable)
{
	// kline on/off
	bool bRet = SetSbRelay(iCardNum, 273, bEnable);
    if(false == bRet)	return false;

	return true;
}

bool __fastcall TDaqDigit::EnableCrash(int iCardNum, bool bEnable)
{
	// crash output on/off
	bool bRet = SetSbRelay(iCardNum, 277, bEnable);
    if(false == bRet)	return false;

	return true;
}

// PB0-2:�Ӱ�ѡ���ַ
// PB3:�Ӱ�ѡ��ʹ�ܣ�����Ч��
// PB5:��λ������Ч��
bool __fastcall TDaqDigit::ResetRelay()
{
	// �����Ӱ帴λ��û�м̵�����Ч
    short iRetCode = 0;
	iRetCode = D2K_DO_WritePort(m_iHandleCard, Channel_P1B, 0x00);
	if (iRetCode != NoError) return false;

	for (char i = 0; i < MES_SUBBOARD_NUM; i++)
	{
		for (char j = 0; j < MES_RELAYGROUP; j++)
		{
			m_scRelayStatus[i][j] = 0x00;
		}
	}

	m_ulMbRelayStatus = 0x00;
    // �����Ӱ�ѡ��ʹ����Ч
    iRetCode = D2K_DO_WritePort(m_iHandleCard, Channel_P1B, 0x08);
	if (iRetCode != NoError) return false;

	return true;
}

// ע�⣺�̵�������ʱ��
bool __fastcall TDaqDigit::EnableRelayBase(int iCardNum, bool bEnable)
{
	bool bRet = false;
    // ��֤�̵����պ�

    bRet = SetSbRelay(iCardNum, 273, bEnable);	// kline on/off
    if(false == bRet)	return false;
    G_SDelay(100);
    bRet = SetSbRelay(iCardNum, 277, bEnable);	// crash on/off
    if(bEnable) SetSbRelayGroupStatus(iCardNum, 7, 136);
    else        SetSbRelayGroupStatus(iCardNum, 7, 0);

    G_SDelay(100);
    bRet = SetSbRelay(iCardNum, 265, bEnable);	// ign on/off
    if(false == bRet)	return false;
    G_SDelay(200);
    bRet = SetSbRelay(iCardNum, 264, bEnable);	// wl on/off
    if(false == bRet)	return false;
    if(bEnable)     SetSbRelayGroupStatus(iCardNum, 6, 48);
    else            SetSbRelayGroupStatus(iCardNum, 6, 0);
    G_SDelay(400);

    return true;
}

// һ��̵���ͬʱ����
bool __fastcall TDaqDigit::EnableRelayGroup(int iItem, int iType)
{
	if(iItem < 0 || iItem > 62) return true;
    for(int i = 0; i < 3; i++)
    {
    	int iRelay = m_siRelayGroup[iItem][i];
        if(iRelay < 286 && iRelay >= 200)	SetSbRelay(g_iCurrBoard, iRelay, true);
        else								SetMbRelay(iRelay, true);
    }
	return true;
}

///////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////


// �����忨����ԴIGN K265��IGN_WL K264������м̵����ͷ�
// ��Ҫ��Ϊ�˲��õ�����
bool __fastcall TDaqDigit::RelayControlSoft(int iCardNum, int iRelay, bool bEnable)
{
	bool bRet = false;
	int iLow = 0, iHigh = 0;
    if((iRelay < 200) || (iRelay > 285))
    	return true;

    // �Ͽ��̵���
    if(false == bEnable)
    {
    	bRet = RelayAction(iCardNum, iRelay, bEnable);
        return bRet;
    }

    // �պϼ̵���
    // �����忨����
    for(int iCard = 0; iCard < MES_SUBBOARD_NUM; iCard++)
    {
    	if(iCardNum == iCard)
        {
        	continue;
        }
    	for(int i = 200; i <= 285; i++)
        {
            if((i != 264) && (i != 265))
            	RelayAction(iCard, i, false);
        }
    }

    // ����忨
    GetRange(iRelay, iLow, iHigh);
    for(int i = 200; i <= 285; i++)
    {
    	// �ڹ�����ʱ
    	if(i >= iLow && i <= iHigh)
        {
        	continue;
        }

        // �������̵ܼ���ʱ
        if((i != 264) && (i != 265) && (i != 273) && (i != 277))
        {
        	// �������̵ܼ���ʱ
     	    RelayAction(iCardNum, i, false);
        }
    }

    bRet = RelayAction(iCardNum, iRelay, bEnable);
    return bRet;
}

bool __fastcall TDaqDigit::RelayAction(int iCardNum, int iRelay, bool bEnable)
{
	bool 	bRet = false;
	U32		ulPaStatus;
	int	    cPcStatus;
    int 	iGroup = (iRelay - 200) / 10;
    int 	iBit = (iRelay - 200) % 10;

	cPcStatus = iGroup | m_ulMbRelayStatus;
    D2K_DO_WritePort(m_iHandleCard, Channel_P1C, cPcStatus);

	ulPaStatus = 0x01;
	ulPaStatus = ulPaStatus << iBit;

	if (bEnable == true)
	{
		m_scRelayStatus[iCardNum][iGroup] = ulPaStatus | m_scRelayStatus[iCardNum][iGroup];
	}
    else
	{
		ulPaStatus = ~ulPaStatus;
		m_scRelayStatus[iCardNum][iGroup] = ulPaStatus & m_scRelayStatus[iCardNum][iGroup];
	}

	ulPaStatus = m_scRelayStatus[iCardNum][iGroup];

    short iRetCode = 0;
    iRetCode = D2K_DO_WritePort(m_iHandleCard, Channel_P1A, ulPaStatus);
    if (iRetCode != NoError) return false;
	bRet = Enable138(iCardNum);

    return bRet;
}

bool __fastcall TDaqDigit::Enable138(int iCardNum)
{
	try
	{
        D2K_DO_WritePort(m_iHandleCard, Channel_P1B, 0x20 | iCardNum);
        D2K_DO_WritePort(m_iHandleCard, Channel_P1B, 0x28 | iCardNum);
		G_SDelay(10);
        D2K_DO_WritePort(m_iHandleCard, Channel_P1B, 0x20 | iCardNum);
		G_SDelay(10);
	}
	catch(Exception & exception)
	{
		return false;
	}
	return true;
}

void __fastcall TDaqDigit::GetRange(int iRelay, int & iLow, int & iHigh)
{
	int iGroup = (iRelay - 200) / 10;
    int iBit = (iRelay - 200) % 10;
    if(iBit >= 0 && iBit <= 3)
    {
    	iLow = 200 + iGroup * 10;
        iHigh = iLow + 3;
    }
    else
    {
    	iLow = 200 + iGroup * 10 + 4;
        iHigh = iLow + 3;
    }
}

///////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////

// ��ǰ�忨�ĵ�ǰ���Mask
void __fastcall TDaqDigit::GetCurrMask (int iCardNum, int iRelay, bool bEnable, int &iGroup, int &iMask)
{
	iGroup = (iRelay - 200) / 10;
    int iBit = (iRelay - 200) % 10;
    U32 ulPaStatus;
    ulPaStatus = 0x01;
    ulPaStatus = ulPaStatus << iBit;
    if (false == bEnable)
    {
        ulPaStatus = ~ulPaStatus;
        iMask = m_scRelayStatus[iCardNum][iGroup] & ulPaStatus;
        return;
    }

    if (iBit >= 0 && iBit <= 3)
    {
        if (iGroup == 6)
            iMask = m_scRelayStatus[iCardNum][iGroup] & 0x3F | ulPaStatus;
        if (iGroup == 7)
            iMask = m_scRelayStatus[iCardNum][iGroup] & 0x8F | ulPaStatus;
        if ((iGroup != 6) && (iGroup != 7))
            iMask = m_scRelayStatus[iCardNum][iGroup] & 0x0F | ulPaStatus;
    }
    else
    {
        if (iGroup == 6)
            iMask = m_scRelayStatus[iCardNum][iGroup] & 0xF0 | ulPaStatus;
        if (iGroup == 7)
            iMask = m_scRelayStatus[iCardNum][iGroup] & 0xF8 | ulPaStatus;
        if ((iGroup != 6) && (iGroup != 7))
            iMask = m_scRelayStatus[iCardNum][iGroup] & 0xF0 | ulPaStatus;
    }
}

// -----------------------------------------------------------------------------
//    �忨�ƶ�ʱ��ԭ�ȵ�״̬
// -----------------------------------------------------------------------------
void __fastcall TDaqDigit::GetPrevMask(int iCardNum, int iGroup, int &iMask)
{
	if(iGroup == 6 )    iMask = 0x30 & m_scRelayStatus[iCardNum][iGroup];
    else                iMask = 0x00;
}

// ����ĳһ��鿨��ĳһ��̵���״̬
// ����ACU�Զ�����ϵͳ�����ֲ�
// ����������0����16���Ʊ�ʾ�ȽϺ����
// ״̬�������������
bool __fastcall TDaqDigit::SetSbRelayGroupStatus(int iCardNum, int iGroup, int iStatus)
{
    // PC0~3 A0~3 �Ӱ��ϼ̵���ѡ���ַ������һ����ÿһ�Ӱ��ϵ���һ��̵���
    D2K_DO_WritePort(this->m_iHandleCard, Channel_P1C, iGroup);

    // PA0~7 D0~7 ��λ���� ע������������
    D2K_DO_WritePort(this->m_iHandleCard, Channel_P1A, iStatus);

    // PB0~2 A4~6 �Ӱ�ѡ���ַ
    // PB3 En-138 �Ӱ�ѡ��ʹ�ܣ�����Ч��
    // PB5 RST ��λ������Ч��
    // ������������ȥ����λ������£���һ�������ش��������Ӱ�ѡ��ʹ����Ч->��Ч->��Ч
    if (D2K_DO_WritePort(this->m_iHandleCard, Channel_P1B, 0x20 | iCardNum) != NoError)   // 2�Ǹ�λ��ֹ
        return false;

    if (D2K_DO_WritePort(this->m_iHandleCard, Channel_P1B, 0x28 | iCardNum) != NoError)   // �����ش�����ʹ�ܴ���
        return  false;
    G_SDelay(10);

    if (D2K_DO_WritePort(this->m_iHandleCard, Channel_P1B, 0x20 | iCardNum) != NoError)   // ��������
        return false;
    G_SDelay(10);

    m_scRelayStatus[iCardNum][iGroup] = iStatus;
    return true;
}

bool __fastcall TDaqDigit::SetSbRelay(int iCardNum, int iGroup, int iBit, bool bEnable)
{
    int iRelay = 200 + iGroup*10 + iBit;
    bool bRet = SetSbRelay(iCardNum, iRelay, bEnable);
    return bRet;
}

// ����ĳһ��忨��ĳһ���̵���״̬
bool __fastcall TDaqDigit::SetSbRelay(int iCardNum, int iRelay, bool bEnable)
{
	if ((iRelay < 200) || (iRelay > 285)) return true;

	bool	bRet = false;
	int		iDoGroup = 0x00, iDoMask = 0x00, iPrevMask = 0x00;

	GetCurrMask(iCardNum, iRelay, bEnable, iDoGroup, iDoMask);
	if (false == bEnable)
	{
		bRet = SetSbRelayGroupStatus(iCardNum, iDoGroup, iDoMask);
		return bRet;
	}

	// A:��һ�β���
	if (m_siCurrSubBoard == -1)
	{
		m_siCurrSubBoard = iCardNum;
		m_siCurrGroup = iDoGroup;
		bRet = SetSbRelayGroupStatus(iCardNum, iDoGroup, iDoMask);
		return bRet;;
	}

	// B:�������
	// B1:�忨�л�
	if (m_siCurrSubBoard != iCardNum)
	{	// ԭ�Ȱ忨����
		GetPrevMask(m_siCurrSubBoard, 6, iPrevMask);
		if (m_scRelayStatus[m_siCurrSubBoard][6] != iPrevMask) 	SetSbRelayGroupStatus(m_siCurrSubBoard, 6, iPrevMask);
		if (m_scRelayStatus[m_siCurrSubBoard][7] != 0x00) 		SetSbRelayGroupStatus(m_siCurrSubBoard, 7, 0x00);
		if (m_siCurrGroup != 6 && m_siCurrGroup != 7 && m_scRelayStatus[m_siCurrSubBoard][m_siCurrGroup] != 0x00)
			SetSbRelayGroupStatus(m_siCurrSubBoard, m_siCurrGroup, 0x00);

		// �°忨����
		m_siCurrSubBoard = iCardNum;
		m_siCurrGroup = iDoGroup;
		bRet = SetSbRelayGroupStatus(iCardNum, iDoGroup, iDoMask);
		return bRet;;
	}

	// B2:����忨
	// C1:�����ڲ���
	if (m_siCurrGroup == iDoGroup)
	{
		bRet = SetSbRelayGroupStatus(iCardNum, iDoGroup, iDoMask);
		return bRet;;
	}

	// C2:���л���ҪôInit���ã�Ҫô�����л�
	// ֻ����R265��R264��R273��R277ԭ��״̬
	// D1:ԭ��״̬��ԭ
	int iValue = 0x00;
	if (m_siCurrGroup != 6 && m_siCurrGroup != 7) 	SetSbRelayGroupStatus(iCardNum, m_siCurrGroup, iValue);
	iValue = 0x30 & m_scRelayStatus[iCardNum][6];
	if (m_scRelayStatus[iCardNum][6] != iValue) 	SetSbRelayGroupStatus(iCardNum, 6, iValue);
	iValue = 0x88 & m_scRelayStatus[iCardNum][7];
	if (m_scRelayStatus[iCardNum][7] != iValue) 	SetSbRelayGroupStatus(iCardNum, 7, iValue);

	// D2:������״̬
	bRet = SetSbRelayGroupStatus(iCardNum, iDoGroup, iDoMask);
	m_siCurrSubBoard = iCardNum;
	m_siCurrGroup = iDoGroup;

	return bRet;
}


// ĸ��̵�����������
// iRelay�Ǽ̵����ţ�����������0��2֮��
// bEnable�ǶԼ̵����Ĳ�����trueΪ���ϣ�falseΪ�Ͽ�
bool __fastcall TDaqDigit::SetMbRelay(int iRelay, bool bEnable)
{
	if(iRelay < 300 || iRelay > 302)
    {
        String strTemp = IntToStr(iRelay);
        strTemp = "Mainboard Relay set fail!" + strTemp;
        return false;
    }

	iRelay = iRelay - 300;
	U32     lMbRelayNum = 0x10 << iRelay;
    short   iRetCode = 0;

    if (true == bEnable)
    {
        m_ulMbRelayStatus = m_ulMbRelayStatus | lMbRelayNum;
        iRetCode = D2K_DO_WritePort(m_iHandleCard, Channel_P1C, m_ulMbRelayStatus);
        if (iRetCode != NoError) return false;
        return true;
    }

    lMbRelayNum = ~lMbRelayNum;
    m_ulMbRelayStatus = m_ulMbRelayStatus & lMbRelayNum;
    iRetCode = D2K_DO_WritePort(m_iHandleCard, Channel_P1C, m_ulMbRelayStatus);
    if (iRetCode != NoError) return false;
    return true;
}

bool __fastcall TDaqDigit::SetParentObjectHandle()
{
    this->m_iHandleCard = g_daqDio.m_iHandleCard;
    return true;
}
