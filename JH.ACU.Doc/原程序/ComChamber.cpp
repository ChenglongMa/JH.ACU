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
//    按温箱协议要求打开串口并按要求初始化；察看Modbus microPLC ENG
//    vers1.2 Protocol_rs232.pdf Page 13
///////////////////////////////////////////////////////////////////////////////
bool __fastcall TComChamber::Open()
{
    // 串口已初始化，即温箱串口已经成功打开,就不用再去打开
    if (m_bComOpen)
        return true;

    // 温箱串口未打开或打开不成功
    // 串口初始化
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

    // 非法句柄值，即串口创建失败
    return false;
}



///////////////////////////////////////////////////////////////////////////////
// 关闭温箱串口
// 原则：只要串口被关闭，return true，其余return false
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
// 设置温箱温度
// fTemp: 百分之一摄氏度，规则可察看pdf文件
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

    // 取最大整数
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
        	// 送出数据
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
// 读取温箱的当前温度
///////////////////////////////////////////////////////////////////////////////
bool __fastcall TComChamber::ReadTemp(float & fTemp)
{
    unsigned int    uCrcData;
    unsigned long   lSetCount, lRetCount;	// 设置的字节数、真正的字节数
    COMSTAT         cs;
    DWORD           dwBytesRead, dwError;
    Byte            cWritebuff[8] = {0}, cInBuff[256] = {0};

    cWritebuff[0] = 0x11;
    cWritebuff[1] = 0x03;

    // Address 2000 = 0x07D0
    cWritebuff[2] = 0x07;
    cWritebuff[3] = 0xD0;

    // 读入Registers数量 2 = 0x0002，共4bytes
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
    	// 送出数据
        bRet = WriteFile(m_hCom, cWritebuff, lSetCount, &lRetCount, NULL);
        G_SDelay(400);
        if (false == bRet)
            return false;
    }
    catch(Exception & exception)
    {
        return false;
    }

    // 取得状态
    ClearCommError(m_hCom, &dwError, &cs);

    // 清除通信端口数据
    // 数据是否大于所准备的缓冲区，大于则清除通信端口数据
    if (cs.cbInQue > sizeof(cInBuff))
        PurgeComm(m_hCom, PURGE_RXCLEAR);

    if (cs.cbInQue > 256)
        cs.cbInQue = 256;

    // 接收通信端口的数据
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
            // 上面已经定义了要求4bytes，为PT100 MEASURES
            // PT100 0. 0，Registers 2000-2001
            int iCriticalTemp = 0;
            iCriticalTemp = (((unsigned int)cInBuff[i + 5]) << 24) + (((unsigned int)cInBuff[i + 6]) << 16) + (((unsigned int)cInBuff[i + 3]) << 8) + (((unsigned int)cInBuff[i + 4]));
            fTemp = iCriticalTemp / 100.0;
            break;
        }
    }

    return true;
}

///////////////////////////////////////////////////////////////////////////////
// Davidion 这个函数值得考虑一下
///////////////////////////////////////////////////////////////////////////////
void __fastcall TComChamber::Wait(float fTemp)
{
	WriteLogFile(g_pFileComKline, "\nTComChamber Wait Start:    \t\n");

	DWORD	dwTimeStart, dwTimeEnd;
	// 获取系统时钟
	dwTimeStart = GetTickCount();
	while (!g_bChamberIdle)
	{
		;
	}
	g_bChamberIdle = false;
	ReadTemp(g_fTempCurveY);
	g_bChamberIdle = true;

	// 实际温度和理论温度相差小于1度跳出
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

		// 当Cham_Wait中等待时间超过一个小时跳出
		if (abs(dwTimeEnd - dwTimeStart) > 3600000)
		{
			break;
		}
	}
    
	WriteLogFile(g_pFileComKline, "\nTComChamber Wait End:    \t\n");
}


///////////////////////////////////////////////////////////////////////////////
// 在线程结束时停止温箱操作
///////////////////////////////////////////////////////////////////////////////
bool __fastcall TComChamber::Stop()
{
	if (false == m_bComOpen)
        return true;
	WriteLogFile(g_pFileComKline, "\nTComChamber Stop Start:    \t\n");
    COMSTAT         cs;
    unsigned long   lBs, lRc;	// 写入字节数，真正写入字节数
    unsigned int    uCrcData;
    Byte            cWritebuff[24];
    String			strTemp = "";

    cWritebuff[0] = 0x11;                                           // 温箱地址
    cWritebuff[1] = 0x10;                                           // 写功能码
    // Address 2300 = 0x08FC
    cWritebuff[2] = 0x08;                                           // 写寄存器高位地址
    cWritebuff[3] = 0xFC;                                           // 写寄存器地位地址
    // Registers数量 1
    cWritebuff[4] = 0x00;                                           // 写寄存器数量高八位
    cWritebuff[5] = 0x01;                                           // 写1个连续寄存器(10个字节)
    // Bytes数量 2 = Registers数量 x 2
    cWritebuff[6] = 0x02;                                           // 2个字节
    // 填充数据：上面定义的2字节
    cWritebuff[7] = 0x00;                                           // 高八位数据
    cWritebuff[8] = 0x00;                                           // 低八位数据

    CRCInit();                                                      // 初始化校验表

    uCrcData = CRCCalc(cWritebuff, 9);
    cWritebuff[9] = uCrcData & 0xff;
    cWritebuff[10] = uCrcData >> 8;
    lBs = 11;

    bool    bRet = false;
    while (false == bRet)
    {
        try
        {
        	// 送出数据
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

    pcWriteBuff[0] = 0x11;  // 温箱地址
    pcWriteBuff[1] = 0x10;  // 写功能码
    // Address 2300 = 0x08FC
    pcWriteBuff[2] = 0x08;  // 写寄存器高位地址
    pcWriteBuff[3] = 0xFC;  // 写寄存器地位地址
    pcWriteBuff[4] = 0x00;  // 写寄存器数量高八位
    pcWriteBuff[5] = 0x06;  // 写5个连续寄存器(10个字节)
    pcWriteBuff[6] = 0x0c;  // 12个字节
    pcWriteBuff[7] = 0x01;  // 高八位数据
    pcWriteBuff[8] = 0x01;  // 低八位数据
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

    // 和规格说明书上有出入，pcWriteBuff[19]、pcWriteBuff[20]应该是做CRC的
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

    // 初始化校验表
    CRCInit();
    uCrcData = CRCCalc(pcWriteBuff, 21);
    pcWriteBuff[21] = uCrcData & 0xff;
    pcWriteBuff[22] = uCrcData >> 8;

    return true;
}





