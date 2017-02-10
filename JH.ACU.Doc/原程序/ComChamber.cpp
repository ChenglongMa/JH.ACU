#pragma hdrstop

#include "ComChamber.h"
#include <math.h>

#pragma package(smart_init)

unsigned char TComChamber::m_sucTableInit[256] = {0x00};
unsigned char TComChamber::m_sucTableResult[256] = {0x00};
unsigned char TComChamber::m_ucaTableF[256] = {0x00};
unsigned char TComChamber::m_ucaTableS[256] = {0x00};

TComChamber g_comChamber;

__fastcall TComChamber::TComChamber()
{
}

__fastcall TComChamber::~TComChamber()
{
}

///////////////////////////////////////////////////////////////////////////////
//    ������Э��Ҫ��򿪴��ڲ���Ҫ���ʼ�����쿴Modbus microPLC ENG
//    vers1.2 Protocol_rs232.pdf Page 13
///////////////////////////////////////////////////////////////////////////////
bool __fastcall TComChamber::Open()
{
    // �����ѳ�ʼ���������䴮���Ѿ��ɹ���,�Ͳ�����ȥ��
    if (m_bComOpen)
        return true;

    // ���䴮��δ�򿪻�򿪲��ɹ�
    // ���ڳ�ʼ��
    m_hCom = CreateFile(MES_COM_CHAMBER, GENERIC_READ | GENERIC_WRITE, 0, NULL, OPEN_EXISTING, 0, 0);
    if (m_hCom != INVALID_HANDLE_VALUE)
    {
        DCB dcb;
        GetCommState(m_hCom, &dcb);
        dcb.BaudRate = CBR_19200;
        dcb.ByteSize = 8;
        dcb.Parity = NOPARITY;
        dcb.StopBits = ONESTOPBIT;
        SetCommState(m_hCom, &dcb);

        m_bComOpen = true;
        return true;
    }

    // �Ƿ����ֵ�������ڴ���ʧ��
    return false;
}



///////////////////////////////////////////////////////////////////////////////
// �ر����䴮��
// ԭ��ֻҪ���ڱ��رգ�return true������return false
///////////////////////////////////////////////////////////////////////////////
bool __fastcall TComChamber::Close()
{
    if (m_bComOpen == false)
        return true;

    try
    {
        CloseHandle(m_hCom);
    }
    catch(Exception & exception)
    {
        return false;
    }

    m_bComOpen = false;
    m_hCom = INVALID_HANDLE_VALUE;
    return true;
}

///////////////////////////////////////////////////////////////////////////////
//    Initialize the CRC table and compute the CRC bytes
///////////////////////////////////////////////////////////////////////////////
void __fastcall TComChamber::CRCInit(void)
{
    unsigned int    uMask, uCrc, uMem;

    for (uMask = 0; uMask < 256; uMask++)
    {
        uCrc = uMask;
        for (int i = 0; i < 8; i++)
        {
            uMem = (unsigned int)(uCrc & 0x0001);
            uCrc /= 2;
            if (uMem)
                uCrc ^= CRC16;
        }

        m_ucaTableS[uMask] = (unsigned char)(uCrc & 0xff);  /* lobyte */
        m_ucaTableF[uMask] = (unsigned char)(uCrc >> 8);    /* hibyte */
    }
}

///////////////////////////////////////////////////////////////////////////////
// Calculate the CRC
///////////////////////////////////////////////////////////////////////////////
unsigned int __fastcall TComChamber::CRCCalc(unsigned char *buf, unsigned char size)
{
    unsigned char   uCar, i;
    unsigned char   uCrc0, uCrc1;
    uCrc0 = 0xff;
    uCrc1 = 0xff;

    for (i = 0; i < size; i++)
    {
        uCar = buf[i];
        uCar ^= uCrc0;
        uCrc0 = (unsigned char)(uCrc1 ^ m_ucaTableS[uCar]);
        uCrc1 = m_ucaTableF[uCar];
    }

    return (uCrc1 << 8) + uCrc0;
}

///////////////////////////////////////////////////////////////////////////////
// ���������¶�
// fTemp: �ٷ�֮һ���϶ȣ�����ɲ쿴pdf�ļ�
///////////////////////////////////////////////////////////////////////////////
bool __fastcall TComChamber::SetTemp(float fTemp)
{
	WriteLogFile(g_pFileComKline, "\nTComChamber SetTemp Start:    \t\n");

    while (!g_bChamberIdle)
    {
        ;
    }
    g_bChamberIdle = false;
    bool    bRet = ReadTemp(g_fTempInit);
    G_SDelay(100);
    g_bChamberIdle = true;
    g_fTempCurveY = g_fTempInit;
    if (false == bRet)
        return false;

    COMSTAT         cs;
    unsigned long   lRc, lBs;
    int             iSetTemp;
    Byte            cWriteBuff[24];
    String			strTemp = "";

    // ȡ�������
    if (fTemp >= 0)
        iSetTemp = floor(fTemp * 100);
    else if (floor(fTemp * 100) < fTemp * 100)
        iSetTemp = floor(fTemp * 100) + 1;
    else
        iSetTemp = floor(fTemp * 100);

    SetCommand(cWriteBuff, iSetTemp, g_fTempCurveY);
    G_SDelay(400);

    lBs = 23;
    bRet = false;
    while (false == bRet)
    {
        try
        {
        	// �ͳ�����
            bRet = WriteFile(m_hCom, cWriteBuff, lBs, &lRc, NULL);
            strTemp = (char *)cWriteBuff;
            WriteLogFile(g_pFileComKline, strTemp);
        }
        catch(Exception & exception)
        {
        	WriteLogFile(g_pFileComKline, "\nTComChamber SetTemp Failed End:    \t\n");
            return false;
        }

        G_SDelay(1000);
    }

    WriteLogFile(g_pFileComKline, "\nTComChamber SetTemp End:    \t\n");
    return true;
}

///////////////////////////////////////////////////////////////////////////////
// ��ȡ����ĵ�ǰ�¶�
///////////////////////////////////////////////////////////////////////////////
bool __fastcall TComChamber::ReadTemp(float & fTemp)
{
    unsigned int    uCrcData;
    unsigned long   lSetCount, lRetCount;	// ���õ��ֽ������������ֽ���
    COMSTAT         cs;
    DWORD           dwBytesRead, dwError;
    Byte            cWritebuff[8] = {0}, cInBuff[256] = {0};

    cWritebuff[0] = 0x11;
    cWritebuff[1] = 0x03;

    // Address 2000 = 0x07D0
    cWritebuff[2] = 0x07;
    cWritebuff[3] = 0xD0;

    // ����Registers���� 2 = 0x0002����4bytes
    cWritebuff[4] = 0x00;
    cWritebuff[5] = 0x02;

    // CRC
    CRCInit();
    uCrcData = CRCCalc(cWritebuff, 6);
    cWritebuff[7] = uCrcData >> 8;
    cWritebuff[6] = uCrcData & 0xff;

    lSetCount = 8;

    bool    bRet = false;
    try
    {
    	// �ͳ�����
        bRet = WriteFile(m_hCom, cWritebuff, lSetCount, &lRetCount, NULL);
        G_SDelay(400);
        if (false == bRet)
            return false;
    }
    catch(Exception & exception)
    {
        return false;
    }

    // ȡ��״̬
    ClearCommError(m_hCom, &dwError, &cs);

    // ���ͨ�Ŷ˿�����
    // �����Ƿ������׼���Ļ����������������ͨ�Ŷ˿�����
    if (cs.cbInQue > sizeof(cInBuff))
        PurgeComm(m_hCom, PURGE_RXCLEAR);

    if (cs.cbInQue > 256)
        cs.cbInQue = 256;

    // ����ͨ�Ŷ˿ڵ�����
    try
    {
        bRet = ReadFile(m_hCom, cInBuff, cs.cbInQue, &dwBytesRead, NULL);
        //G_SDelay(100);
        if (false == bRet)
            return false;
    }

    catch(Exception & exception)
    {
        return false;
    }

    for (unsigned int i = 0; i < cs.cbInQue; i++)
    {
        if ((cInBuff[i] == 0x11) && (cInBuff[i + 1] == 0x03))
        {
            // �����Ѿ�������Ҫ��4bytes��ΪPT100 MEASURES
            // PT100 0. 0��Registers 2000-2001
            int iCriticalTemp = 0;
            iCriticalTemp = (((unsigned int)cInBuff[i + 5]) << 24) + (((unsigned int)cInBuff[i + 6]) << 16) + (((unsigned int)cInBuff[i + 3]) << 8) + (((unsigned int)cInBuff[i + 4]));
            fTemp = iCriticalTemp / 100.0;
            break;
        }
    }

    return true;
}

///////////////////////////////////////////////////////////////////////////////
// Davidion �������ֵ�ÿ���һ��
///////////////////////////////////////////////////////////////////////////////
void __fastcall TComChamber::Wait(float fTemp)
{
	WriteLogFile(g_pFileComKline, "\nTComChamber Wait Start:    \t\n");

	DWORD	dwTimeStart, dwTimeEnd;
	// ��ȡϵͳʱ��
	dwTimeStart = GetTickCount();
	while (!g_bChamberIdle)
	{
		;
	}
	g_bChamberIdle = false;
	ReadTemp(g_fTempCurveY);
	g_bChamberIdle = true;

	// ʵ���¶Ⱥ������¶����С��1������
	while (abs(g_fTempCurveY - fTemp) >= 1.0)
	{
		G_SDelay(1*60*1000);
		while (false == g_bChamberIdle)
		{
			;
		}
		g_bChamberIdle = false;
		ReadTemp(g_fTempCurveY);
		g_bChamberIdle = true;
        
		dwTimeEnd = GetTickCount();
		if ((dwTimeEnd - dwTimeStart) <= 0) dwTimeStart = GetTickCount();

		// ��Cham_Wait�еȴ�ʱ�䳬��һ��Сʱ����
		if (abs(dwTimeEnd - dwTimeStart) > 3600000)
		{
			break;
		}
	}
    
	WriteLogFile(g_pFileComKline, "\nTComChamber Wait End:    \t\n");
}


///////////////////////////////////////////////////////////////////////////////
// ���߳̽���ʱֹͣ�������
///////////////////////////////////////////////////////////////////////////////
bool __fastcall TComChamber::Stop()
{
	if (false == m_bComOpen)
        return true;
	WriteLogFile(g_pFileComKline, "\nTComChamber Stop Start:    \t\n");
    COMSTAT         cs;
    unsigned long   lBs, lRc;	// д���ֽ���������д���ֽ���
    unsigned int    uCrcData;
    Byte            cWritebuff[24];
    String			strTemp = "";

    cWritebuff[0] = 0x11;                                           // �����ַ
    cWritebuff[1] = 0x10;                                           // д������
    // Address 2300 = 0x08FC
    cWritebuff[2] = 0x08;                                           // д�Ĵ�����λ��ַ
    cWritebuff[3] = 0xFC;                                           // д�Ĵ�����λ��ַ
    // Registers���� 1
    cWritebuff[4] = 0x00;                                           // д�Ĵ��������߰�λ
    cWritebuff[5] = 0x01;                                           // д1�������Ĵ���(10���ֽ�)
    // Bytes���� 2 = Registers���� x 2
    cWritebuff[6] = 0x02;                                           // 2���ֽ�
    // ������ݣ����涨���2�ֽ�
    cWritebuff[7] = 0x00;                                           // �߰�λ����
    cWritebuff[8] = 0x00;                                           // �Ͱ�λ����

    CRCInit();                                                      // ��ʼ��У���

    uCrcData = CRCCalc(cWritebuff, 9);
    cWritebuff[9] = uCrcData & 0xff;
    cWritebuff[10] = uCrcData >> 8;
    lBs = 11;

    bool    bRet = false;
    while (false == bRet)
    {
        try
        {
        	// �ͳ�����
            bRet = WriteFile(m_hCom, cWritebuff, lBs, &lRc, NULL);
            strTemp = (char *)cWritebuff;
            WriteLogFile(g_pFileComKline, strTemp);
        }
        catch(Exception & exception)
        {
        	WriteLogFile(g_pFileComKline, "\nTComChamber Stop Failed End:    \t\n");
            return false;
        }
    }

    WriteLogFile(g_pFileComKline, "\nTComChamber Stop End:    \t\n");
    return true;
}

bool __fastcall TComChamber::SetCommand(Byte *pcWriteBuff, int iSetTemp, float fReadTemp)
{
    unsigned int    uCrcData;

    pcWriteBuff[0] = 0x11;  // �����ַ
    pcWriteBuff[1] = 0x10;  // д������
    // Address 2300 = 0x08FC
    pcWriteBuff[2] = 0x08;  // д�Ĵ�����λ��ַ
    pcWriteBuff[3] = 0xFC;  // д�Ĵ�����λ��ַ
    pcWriteBuff[4] = 0x00;  // д�Ĵ��������߰�λ
    pcWriteBuff[5] = 0x06;  // д5�������Ĵ���(10���ֽ�)
    pcWriteBuff[6] = 0x0c;  // 12���ֽ�
    pcWriteBuff[7] = 0x01;  // �߰�λ����
    pcWriteBuff[8] = 0x01;  // �Ͱ�λ����
    pcWriteBuff[9] = 0x00;
    pcWriteBuff[10] = 0xff;
    pcWriteBuff[11] = 0x00;
    pcWriteBuff[12] = 0x00;
    pcWriteBuff[13] = 0x00;
    pcWriteBuff[14] = 0x00;

    if (iSetTemp > 0)
    {
        pcWriteBuff[15] = (iSetTemp & 0xff00) >> 8;
        pcWriteBuff[16] = iSetTemp & 0x00ff;
        pcWriteBuff[17] = 0x00;
        pcWriteBuff[18] = 0x00;
    }
    else
    {
        iSetTemp = abs(iSetTemp + 1);
        iSetTemp = ~iSetTemp;
        pcWriteBuff[15] = (iSetTemp >> 8);
        pcWriteBuff[16] = iSetTemp & 0xff;
        pcWriteBuff[17] = 0xff;
        pcWriteBuff[18] = 0xff;
    }

    // �͹��˵�������г��룬pcWriteBuff[19]��pcWriteBuff[20]Ӧ������CRC��
    if (iSetTemp > fReadTemp)
    {
        pcWriteBuff[19] = 0xc8;
        pcWriteBuff[20] = 0x00;
    }
    else
    {
        pcWriteBuff[19] = 0x38;
        pcWriteBuff[20] = 0xff;
    }

    // ��ʼ��У���
    CRCInit();
    uCrcData = CRCCalc(pcWriteBuff, 21);
    pcWriteBuff[21] = uCrcData & 0xff;
    pcWriteBuff[22] = uCrcData >> 8;

    return true;
}





