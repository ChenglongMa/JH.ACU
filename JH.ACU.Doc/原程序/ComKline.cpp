#pragma hdrstop

#include "ComKline.h"
#include "stdio.h"
#include "GpibPcPower.h"
#include "DaqDigit.h"
#include "DaqAnalog.h"

#pragma package(smart_init)

TComKline g_comKline;

__fastcall TComKline::TComKline()
{
	m_bKline = true;
}

__fastcall TComKline::~TComKline()
{
}

void __fastcall TComKline:: ResetKlineState()
{
	bool bRet = g_daqDigit.EnableRelayBase(g_iCurrBoard, true);
	if (false == bRet)
	{
		WriteLogFile(g_pFileComKline, "\nEnableBaseRelay Failed:");
	}

	bRet = g_daqAnalog.GetPowerState();
	if (false == bRet)
	{
		WriteLogFile(g_pFileComKline, "\nGetPowerState Failed:");
		bRet = g_gpibPcPower.SetOutPut(true);
		if (false == bRet) WriteLogFile(g_pFileComKline, "\nSetOutPut true Failed:");
	}
}

///////////////////////////////////////////////////////////////////////////////
// 设备级操作
///////////////////////////////////////////////////////////////////////////////
bool __fastcall TComKline::Open()
{
	// 串口已初始化，即KLine串口已经成功打开,就不用再去打开
	if (true == m_bComOpen) return true;

	// 串口初始化
	m_hCom = CreateFile(MES_COM_KLINE, GENERIC_READ | GENERIC_WRITE, 0, NULL, OPEN_EXISTING, 0, 0);
	if (m_hCom != INVALID_HANDLE_VALUE)
	{
		m_bComOpen = true;

		DCB dcb;
		GetCommState(m_hCom, &dcb);
		dcb.BaudRate = CBR_9600;
		dcb.ByteSize = 8;
		dcb.Parity = NOPARITY;
		dcb.StopBits = ONESTOPBIT;
		SetCommState(m_hCom, &dcb);

		// 测试K-Line，前提是Kline继电器打开，并且ACU电源已供上
        // 以上说法不正确，只要串口设备正常就可以
		unsigned char	caBuf[3] = { 0xAA, 0x55, 0x11 };
		if (Write(caBuf, 3) == false)
		{
			Close();
			m_strMark = "Test ComKline Failed in Open.!";
			return false;
		}

		return true;
	}

    // 非法句柄值，即串口不存在或者已被其它程序占用
	return false;
}

///////////////////////////////////////////////////////////////////////////////
// 设备级操作
///////////////////////////////////////////////////////////////////////////////
bool __fastcall TComKline::Close()
{
	if (false == m_bComOpen) return true;

	try
	{
		CloseHandle(m_hCom);
	}
	catch(Exception & exception)
	{
		m_strMark = "Close ComKline Failed.!";
		return false;
	}

	m_bComOpen = false;
	return true;
}

///////////////////////////////////////////////////////////////////////////////
// 在设备正常的情况下开启Smode通信
///////////////////////////////////////////////////////////////////////////////
bool __fastcall TComKline::OpenSMode()
{
	WriteLogFile(g_pFileComKline, "\nThreadMain OpenSMode:    \t\n");
	unsigned char	cWrite = 0x53, caBuf[100] = {0x00};
	unsigned int	i = 0, iRepeat = 0;
	String			strTemp = "";
	bool			bRet = false;

	do
	{
    	iRepeat++;
		i = 0;
		strTemp = "";
        memset(caBuf, '\0', sizeof(caBuf));

        // 发送Smode开始命令0x53
        g_strSubTip = "Send Smode command";
		bRet = Write(&cWrite, 1);
		if (!bRet)
		{
			m_strMark = "ComKline Device Operate fail.!";
			WriteLogFile(g_pFileComKline, "\nTComKline OpenSMode Write 0x53 Failed:    \t\n\n");
            g_strSubTip = "SMode Opened Failed.";
			return false;
		}

		bRet = Read(caBuf, MES_SMODE_LEN, m_bKline);
		if (true == bRet)
		{
			for (i = 1; i <= MES_SMODE_LEN; i++)
			{
				strTemp = strTemp + IntToHex(caBuf[i], 2);
			}

            // 发送校验和
			Write(&caBuf[MES_SMODE_LEN], 1);
			ReadChar(caBuf[MES_SMODE_LEN + 1]);

			// Smode模式启动成功
			if (caBuf[MES_SMODE_LEN + 1] == 0x4F)
			{
				g_strMark = strTemp;
				WriteLogFile(g_pFileComKline, "\nTComKline OpenSMode End:    \t\n\n");
                g_strSubTip = "SMode Opened.";
				return true;
			}
		}

        // 有可能设备不正常
        if(false == m_bKline)
        {
         	ResetKlineState();
        }
	}
	while (iRepeat <= MES_REPEAT_BASE);

	if (iRepeat > MES_REPEAT_BASE)
	{
		m_strMark = "Open Smode Fail.!";
		WriteLogFile(g_pFileComKline, "\nTComKline OpenSMode Failed:    \t\n\n");
		return false;
	}

    g_strSubTip = "SMode Opened.";
	return true;
}

// 中断ECU的Pmode模式
bool __fastcall TComKline::CloseSMode()
{
	WriteLogFile(g_pFileComKline, "\nTComKline CloseSMode:    \t\n");

	unsigned char 	caBufRead[100] = {0};
	unsigned char	caBufWriteReset[4] = { 0x03, 0x79, 0xFF, 0x85 };	// RESET
	unsigned char	caBufWriteSMode[3] = { 0x02, 0x78, 0x7A };			// Interrupt
	int				iRepeat = 0;
    bool            bRet = false;

    // 为了保证ACU确实可以重置，该段代码执行多遍，以下两者其一就可以
    do
	{
		iRepeat++;
		// Interrupt Pmode
        bRet = g_comKline.Write(caBufWriteSMode, 3);
		if (bRet == false)
		{
			m_strMark = "ComKline Device Operate fail-Interrupt.!";
           	WriteLogFile(g_pFileComKline, m_strMark);
		} else
		{
            bRet = Read(caBufRead, 0x02, m_bKline);
			if (bRet == true)	// 0x02 0x** 0x**
            {
                WriteLogFile(g_pFileComKline, "\nThreadMain CloseSMode-Interrupt End:    \t\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n");
				return true;
            }

            // 有可能设备不正常
        	if(false == m_bKline)
        	{
        		ResetKlineState();
        	}
		}

		// RESET
        bRet = g_comKline.Write(caBufWriteReset, 4);
		if (bRet == false)
		{
			m_strMark = "ComKline Device Operate fail-Reset.!";
			WriteLogFile(g_pFileComKline, m_strMark);
		} else
		{
            unsigned char cReadChar = 0;
            bRet = g_comKline.ReadChar(cReadChar);
			if (bRet == true)
			{
				if (cReadChar == 0xFF)
                {
                    WriteLogFile(g_pFileComKline, "\nThreadMain CloseSMode-Reset End:    \t\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n");
                    return true;
                }
			}
		}
	}while (iRepeat <= MES_REPEAT_BASE);

    return false;
}

// 02 : 72/75 : Start Address High byte : Start Address Low byte : Read bytes number
bool __fastcall TComKline::ReadMemory(short iAddress, short iLength, String &strInfo)
{
	WriteLogFile(g_pFileComKline, "\nTComKline ReadMemory Start:    \t\n");

	unsigned char	caBufWrite[6] = { 0x05, 0x7A, iAddress >> 8, iAddress & 0xFF, iLength };
	unsigned char	caBufRead[300] = {0x00};
	unsigned char	cCheckSum = caBufWrite[0];
    int 			i = 0;
	for (i = 1; i < 5; i++)
	{
		cCheckSum = cCheckSum ^ caBufWrite[i];
	}
	caBufWrite[5] = cCheckSum;

	bool	bRet = false;
	bRet = Write(caBufWrite, 6);
	if (false == bRet)
	{
		m_strMark = "ComKline Device Operate fail.!";
        WriteLogFile(g_pFileComKline, "\nTComKline FlashRead Write Failed:");
		return false;
	}

    bRet = Read(caBufRead, iLength + 5, m_bKline);
	if (false == bRet)
	{
     	m_strMark = "kline communication fail.!";
        WriteLogFile(g_pFileComKline, "\nTComKline FlashRead Read Failed:");
        // 有可能设备不正常
        if(false == m_bKline)
        {
        	ResetKlineState();
        }
		return false;
	}

    for (i = 0; i < iLength + 5; i++)
    {
    	strInfo = strInfo + IntToHex(caBufRead[i], 2);
    }

    WriteLogFile(g_pFileComKline, "\nTComKline FlashRead End:    \t\n\n");
	return true;
}

bool __fastcall TComKline::WriteMemory(short iAddress, short iLength, String &strInfo)
{
	WriteLogFile(g_pFileComKline, "\nTComKline WriteMemory Start:    \t\n");
	unsigned char	caBufWrite[150] = { iLength + 0x05, 0x7B, iAddress >> 8, iAddress & 0xFF, iLength };
	unsigned char	caBufRead[300] = {0x00};
	unsigned char	cCheckSum = caBufWrite[0];
    int				i = 0;
	for (i = 1; i < 5; i++)
	{
		cCheckSum = cCheckSum ^ caBufWrite[i];
	}
    for (i = 5; i < iLength + 5; i++)
    {
    	caBufWrite[i] = 0xFF;
    	cCheckSum = cCheckSum ^ caBufWrite[i];
    }
	caBufWrite[i] = cCheckSum;

	bool	bRet = false;
	String	strTemp = "";

	bRet = Write(caBufWrite, iLength + 0x05 + 0x01);
	if (false == bRet)
	{
		m_strMark = "ComKline Device Operate fail.!";
        WriteLogFile(g_pFileComKline, "TComKline WriteMemory Write Failed:    \t\n\n");
		return false;
	}

    WriteLogFile(g_pFileComKline, "\nTComKline Read Memory In WriteMemory:    \t\n");
    bRet = Read(caBufRead, iLength + 5, m_bKline);
	if (false == bRet)
    {
    	m_strMark = "kline communication fail!";
        WriteLogFile(g_pFileComKline, "\nTComKline WriteMemory Read Failed:    \t\n\n");
		return false;
    }

    for (int i = 0; i < iLength + 5; i++)
    {
    	strInfo = strInfo + IntToHex(caBufRead[i], 2);
    }

    WriteLogFile(g_pFileComKline, "\nTComKline WriteMemory End:    \t\n\n");
	return true;
}

bool __fastcall TComKline::ReadFRAM(short iAddress, short iLength, String &strInfo)
{
	WriteLogFile(g_pFileComKline, "\nTComKline ReadFRAM Start:    \t\n");
	unsigned char	caBufWrite[6] = { 0x05, 0x7A, iAddress >> 8, iAddress & 0xFF, iLength };
	unsigned char	caBufRead[300] = {0x00};
	unsigned char	cCheckSum = caBufWrite[0];
    int 			i = 0;
	for (i = 1; i < 5; i++)
	{
		cCheckSum = cCheckSum ^ caBufWrite[i];
	}
	caBufWrite[5] = cCheckSum;

	bool	bRet = false;
	bRet = Write(caBufWrite, 6);
	if (false == bRet)
	{
		m_strMark = "ComKline Device Operate fail.!";
        WriteLogFile(g_pFileComKline, "\nTComKline ReadFRAM Write Failed:");
		return false;
	}

    bRet = Read(caBufRead, iLength + 5, m_bKline);
	if (false == bRet)
	{
     	m_strMark = "kline communication fail.!";
        WriteLogFile(g_pFileComKline, "\nTComKline ReadFRAM Read Failed:");
        // 有可能设备不正常
        if(false == m_bKline)
        {
        	ResetKlineState();
        }
		return false;
	}

    for (i = 0; i < iLength + 5; i++)
    {
    	strInfo = strInfo + IntToHex(caBufRead[i], 2);
    }

    WriteLogFile(g_pFileComKline, "\nTComKline ReadFRAM End:    \t\n\n");
	return true;
}

bool __fastcall TComKline::WriteFRAM(short iAddress, short iLength, String &strInfo)
{
	WriteLogFile(g_pFileComKline, "\nTComKline WriteFRAM Start:    \t\n");
	unsigned char	caBufWrite[150] = { iLength + 0x05, 0x7B, iAddress >> 8, iAddress & 0xFF, iLength };
	unsigned char	caBufRead[300] = {0x00};
	unsigned char	cCheckSum = caBufWrite[0];
    int				i = 0;
	for (i = 1; i < 5; i++)
	{
		cCheckSum = cCheckSum ^ caBufWrite[i];
	}
    for (i = 5; i < iLength + 5; i++)
    {
    	caBufWrite[i] = 0xFF;
    	cCheckSum = cCheckSum ^ caBufWrite[i];
    }
	caBufWrite[i] = cCheckSum;

	bool	bRet = false;
	String	strTemp = "";

	bRet = Write(caBufWrite, iLength + 0x05 + 0x01);
	if (false == bRet)
	{
		m_strMark = "ComKline Device Operate fail.!";
        WriteLogFile(g_pFileComKline, "TComKline WriteFRAM Write Failed:    \t\n\n");
		return false;
	}

    bRet = Read(caBufRead, iLength + 5, m_bKline);
	if (false == bRet)
    {
    	m_strMark = "kline communication fail!";
        WriteLogFile(g_pFileComKline, "\nTComKline WriteFRAM Read Failed:    \t\n\n");
		return false;
    }

    for (int i = 0; i < iLength + 5; i++)
    {
    	strInfo = strInfo + IntToHex(caBufRead[i], 2);
    }

    WriteLogFile(g_pFileComKline, "\nTComKline WriteFRAM End:    \t\n\n");
	return true;
}

bool __fastcall TComKline::ReadEEPROM(short iAddress, short iLength, String &strInfo)
{
	WriteLogFile(g_pFileComKline, "\nTComKline ReadEEPROM Start:    \t\n");
	unsigned char	caBufWrite[6] = { 0x05, 0x72, iAddress >> 8, iAddress & 0xFF, iLength };
	unsigned char	caBufRead[300] = {0x00};
	unsigned char	cCheckSum = caBufWrite[0];
    int 			i = 0;
	for (i = 1; i < 5; i++)
	{
		cCheckSum = cCheckSum ^ caBufWrite[i];
	}
	caBufWrite[5] = cCheckSum;

	bool	bRet = false;
	bRet = Write(caBufWrite, 6);
	if (false == bRet)
	{
		m_strMark = "ComKline Device Operate fail.!";
        WriteLogFile(g_pFileComKline, "\nTComKline ReadEEPROM Write Failed:");
		return false;
	}

    bRet = Read(caBufRead, iLength + 5, m_bKline);
	if (false == bRet)
	{
     	m_strMark = "kline communication fail.!";
        WriteLogFile(g_pFileComKline, "\nTComKline ReadEEPROM Read Failed:");
        // 有可能设备不正常
        if(false == m_bKline)
        {
        	ResetKlineState();
        }
		return false;
	}

    for (i = 0; i < iLength + 5; i++)
    {
    	strInfo = strInfo + IntToHex(caBufRead[i], 2);
    }

    WriteLogFile(g_pFileComKline, "\nTComKline ReadEEPROM End:    \t\n\n");
	return true;
}

bool __fastcall TComKline::WriteEEPROM(short iAddress, short iLength, String &strInfo)
{
	WriteLogFile(g_pFileComKline, "\nTComKline WriteFRAM Start:    \t\n");
	unsigned char	caBufWrite[150] = { iLength + 0x05, 0x73, iAddress >> 8, iAddress & 0xFF, iLength };
	unsigned char	caBufRead[300] = {0x00};
	unsigned char	cCheckSum = caBufWrite[0];
    int				i = 0;
	for (i = 1; i < 5; i++)
	{
		cCheckSum = cCheckSum ^ caBufWrite[i];
	}
    for (i = 5; i < iLength + 5; i++)
    {
    	caBufWrite[i] = 0xFF;
    	cCheckSum = cCheckSum ^ caBufWrite[i];
    }
	caBufWrite[i] = cCheckSum;

	bool	bRet = false;
	String	strTemp = "";

	bRet = Write(caBufWrite, iLength + 0x05 + 0x01);
	if (false == bRet)
	{
		m_strMark = "ComKline Device Operate fail.!";
        WriteLogFile(g_pFileComKline, "TComKline WriteFRAM Write Failed:    \t\n\n");
		return false;
	}

    bRet = Read(caBufRead, iLength + 5, m_bKline);
	if (false == bRet)
    {
    	m_strMark = "kline communication fail!";
        WriteLogFile(g_pFileComKline, "\nTComKline WriteFRAM Read Failed:    \t\n\n");
		return false;
    }

    for (int i = 0; i < iLength + 5; i++)
    {
    	strInfo = strInfo + IntToHex(caBufRead[i], 2);
    }

    WriteLogFile(g_pFileComKline, "\nTComKline WriteFRAM End:    \t\n\n");
	return true;
}

// 打开RealTime Fault & Status Check
bool __fastcall TComKline::OpenRealtimeFault()
{
	WriteLogFile(g_pFileComKline, "\nTComKline OpenRealtimeFault Start:    \t\n");
	unsigned char		caBufWrite[3] = { 0x02, 0x75, 0x77 };
    unsigned char		caBufRead[100] = { 0x00 };
	bool	bRet = false;
    int		iRepeat = 0;
    
	do
	{
		bRet = Write(caBufWrite, 3);
        if(false == bRet)
        {
        	m_strMark = "ComKline Device Operate fail.!";
            WriteLogFile(g_pFileComKline, "\nTComKline OpenRealtimeFault Failed:    \t\n\n");
            return false;
        }

        if(true == bRet)
        {
        	// 强行读取20字节
            WriteLogFile(g_pFileComKline, "\n");
        	bRet = g_comKline.ReadBuf(caBufRead, 20);
        	WriteLogFile(g_pFileComKline, "\nTComKline OpenRealtimeFault End:    \t\n\n");
        	return true;
        }
		iRepeat++;
	}
	while (iRepeat <= MES_REPEAT_BASE);

    WriteLogFile(g_pFileComKline, "\nTComKline OpenRealtimeFault Exception End:    \t\n\n");
	return false;
}

// 关闭RealTime Fault & Status Check
bool __fastcall TComKline::CloseRealtimeFault()
{
	WriteLogFile(g_pFileComKline, "\nTComKline CloseRealtimeFault Start:    \t\n");
	bool bRet = false;
    bRet = EscapeCommFunction(m_hCom, SETBREAK);
    if(false == bRet)
    {
    	WriteLogFile(g_pFileComKline, "TComKline CloseRealtimeFault Error End:    \t\n\n");
        return false;
    }
    bRet = EscapeCommFunction(m_hCom, CLRBREAK);
    WriteLogFile(g_pFileComKline, "TComKline CloseRealtimeFault End:    \t\n\n");
    G_SDelay(1000);
    return bRet;
}

///////////////////////////////////////////////////////////////////////////////
// 返回值：
// 			TRUE-Kline通信成功
//			FALSE-Kline通信失败
// 使能WarnLamp iLamp 哪一个灯
// { 0x03, 0x79, 0x14, 0x6E } 1 true
// { 0x03, 0x79, 0x15, 0x6F } 1 false
// { 0x03, 0x79, 0x16, 0x6C } 2 true
// { 0x03, 0x79, 0x17, 0x6D } 2 false
bool __fastcall TComKline::EnableWarnLamp(int iLamp, bool bEnable)
{
	WriteLogFile(g_pFileComKline, "\nTComKline EnableWarnLamp Start:    \t\n");
	unsigned char	caBufRead[100] = {0};
    unsigned char   caBufWrite[][5] = {{ 0x03, 0x79, 0x14, 0x6E },
    								{ 0x03, 0x79, 0x15, 0x6F },
                                    { 0x03, 0x79, 0x16, 0x6C },
                                    { 0x03, 0x79, 0x17, 0x6D }};
    int 			iIndex = 0;

    if(1 == iLamp)
    {
    	if(true == bEnable) iIndex = 0;
        else				iIndex = 1;
    }
    else if(2 == iLamp)
    {
    	if(true == bEnable) iIndex = 2;
        else				iIndex = 3;
    }

	int iRepeat = 0;
    bool bRet = false;

	do
	{
    	iRepeat++;
        memset(caBufRead, '0', sizeof(caBufRead));

        bRet = Write(caBufWrite[iIndex], 4);
		if (false == bRet)
		{
			m_strMark = "ComKline Device Operate fail.!";
            WriteLogFile(g_pFileComKline, "\nTComKline EnableWarnLamp Failed:    \t\n\n");
            return false;
		}

		// 取得状态
        bRet = Read(caBufRead, 0x03, m_bKline);
		if (true == bRet)
		{
        	WriteLogFile(g_pFileComKline, "\nTComKline EnableWarnLamp End:");
			return true;
		}
        // 有可能设备不正常
        if(false == m_bKline)
        {
        	ResetKlineState();
        }
	}
	while (iRepeat <= MES_REPEAT_BASE);

    WriteLogFile(g_pFileComKline, "\nTComKline EnableWarnLamp End:");
    if(false == m_bKline)	return false;
    return true;
}

///////////////////////////////////////////////////////////////////////////////
// 返回值：
// 			TRUE-Kline通信成功
//			FALSE-Kline通信失败
//	{ 0x03, 0x79, 0x08, 0x72 }	发
//  { 0x03, 0x86, 0x08, 0x8D }  收
bool __fastcall TComKline::EnableCrashOutput()
{
	WriteLogFile(g_pFileComKline, "\nTComKline EnableCrashOutput Start:    \t\n");
	unsigned char	caBufRead[100] = {0},caBufWrite[4] = { 0x03, 0x79, 0x08, 0x72 };
	int				iRepeat = 0;
	bool			bRet = false;

	do
	{	// set crash out mode
		iRepeat++;
        memset(caBufRead, '0', sizeof(caBufRead));

		bRet = Write(caBufWrite, 4);
		if (bRet == false)
		{
			m_strMark = "ComKline Device Operate fail.!";
            WriteLogFile(g_pFileComKline, "\nThreadMain EnableCrashOutput Failed:    \t\n\n");
			return false;
		}

		// 取得状态
        // {0x03, 0x86, 0x08, 0x8d}
        bRet = Read(caBufRead, 0x03, m_bKline);
		if (true == bRet)
		{
        	WriteLogFile(g_pFileComKline, "\nThreadMain EnableCrashOutput End:    \t\n\n");
			return true;
		}

        // 有可能设备不正常
        if(false == m_bKline)
        {
        	ResetKlineState();
        }
	}
	while (iRepeat <= MES_REPEAT_BASE);

    WriteLogFile(g_pFileComKline, "ThreadMain EnableCrashOutput Failed End:    \t\n\n");
    if(false == m_bKline)	return false;
    return true;
}

bool __fastcall TComKline::RealTimeValueCheck(int iCheckValueCode, int &iResult)
{
	WriteLogFile(g_pFileComKline, "\nTComKline RealTimeValueCheck Start:    \t\n");
	unsigned char	caBufWrite[4] = { 0x03, 0x77, 0x00, 0x00 };
    unsigned char	cCheckSum = caBufWrite[0], cRetChar = 0;
	int				iRepeat = 0, i = 0;
	bool			bRet = false;

    // Caculate Check Summary
    caBufWrite[2] = iCheckValueCode;
    for(i = 1; i < 3; i++)
    {
    	cCheckSum = cCheckSum ^ caBufWrite[i];
    }
    caBufWrite[i] = cCheckSum;
    
	do
	{	// set Real Time Value Check mode
		iRepeat++;
		bRet = Write(caBufWrite, 4);
		if (bRet == false)
		{
        	g_strSubTip = "ComKline Device Operate fail.!";
            WriteLogFile(g_pFileComKline, "\nThreadMain RealTimeValueCheck Failed:    \t\n\n");
            iResult = -2;
			return false;
		}

		// 取得状态
        bRet = ReadChar(cRetChar);
		if (true == bRet)
		{
        	iResult = cRetChar;
        	WriteLogFile(g_pFileComKline, "\nThreadMain RealTimeValueCheck End:    \t\n\n");
			return true;
		}

        // 有可能设备不正常
       	ResetKlineState();
	}
	while (iRepeat <= MES_REPEAT_BASE);

    WriteLogFile(g_pFileComKline, "ThreadMain RealTimeValueCheck Failed End:    \t\n\n");
    g_strSubTip = "ComKline Device ReadChar fail.!";
    iResult = -1;
    return false;
}

bool __fastcall TComKline::ExecuteCommand(const int iCommandId, unsigned char *caBufRead)
{
	unsigned char	caBufWrite[10] = { 0 }, cCheckSum = 0x00, iPos = 0;
	int				iRepeat = 0;
	bool			bRet = false;
	unsigned char caCommand[][4] = {
  	//      command            ECU 返回的长度
        {0x02, 0x71, 0x00, 0x13},         // ECU Identification
        {0x02, 0x75, 0x00, 0x00},         // Real time fault & status check
        {0x02, 0x78, 0x00, 0x02},         // Interrupt S-mode (user function)
        {0x03, 0x77, 0x00, 0x03},         // Real time value check
        {0x03, 0x79, 0x00, 0x03},         // Clear Fault Record
        {0x03, 0x79, 0x01, 0x03},         // Clear EEPROM Crash Record
        {0x03, 0x79, 0x02, 0x03},         // Clear FRAM Crash Record
        {0x03, 0x79, 0x03, 0x03},         // Start Diag.
        {0x03, 0x79, 0x04, 0x03},         // Stop Diag.
        {0x03, 0x79, 0x08, 0x03},         // Crash Out
        {0x03, 0x79, 0x0A, 0x03},         // Clear Operation Timer Counter Area
        {0x03, 0x79, 0x0B, 0x03},         // Clear FRAM IGN Counter
        {0x03, 0x79, 0x0D, 0x03},         // FRAM Mass Erase
        {0x03, 0x79, 0x0E, 0x03},         // EEPROM Mass Erase
        {0x03, 0x79, 0x14, 0x03},         // WL1 On
        {0x03, 0x79, 0x15, 0x03},         // WL1 Off
        {0x03, 0x79, 0x16, 0x03},         // WL2 On
        {0x03, 0x79, 0x17, 0x03},         // WL2 Off
        {0x03, 0x79, 0x1E, 0x03},         // Front Side Algorithm Reset
        {0x03, 0x79, 0xFF, 0x00}};        // ECU Reset

	if (iCommandId > MES_RESET_ECU)
		return true;

	// 填充命令
	for (iPos = 0; iPos < caCommand[iCommandId][0]; iPos++)
	{
		caBufWrite[iPos] = caCommand[iCommandId][iPos];
		cCheckSum ^= caCommand[iCommandId][iPos];
	}
	caBufWrite[iPos] = cCheckSum;

    // 操作命令
	do
	{
		iRepeat++;
		memset(caBufRead, '0', sizeof(caBufRead));

		bRet = Write(caBufWrite, iPos + 1);
		if (bRet == false)
		{
			m_strMark = "ComKline Device Operate fail.!";
			WriteLogFile(g_pFileComKline, "\nThreadMain EnableCrashOutput Failed:    \t\n\n");
			return false;
		}

        if(19 == iCommandId)
        {
            unsigned char *caBuf = "\n";
            bRet = ReadBuf(caBuf, 1);
        }

		// 取得状态
		unsigned int	iLength = caCommand[iCommandId][3];
        if(0 != iLength)
		    bRet = Read(caBufRead, iLength, m_bKline);
		if (true == bRet)
		{
			WriteLogFile(g_pFileComKline, "\nThreadMain EnableCrashOutput End:    \t\n\n");
			return true;
		}

		// 有可能设备不正常
		if (false == m_bKline)
		{
			ResetKlineState();
		}
	}
	while (iRepeat <= MES_REPEAT_BASE);

	WriteLogFile(g_pFileComKline, "ThreadMain EnableCrashOutput Failed End:    \t\n\n");
	return false;
}

/////////////////////////////////////////////////////////////////////
// 将数据缓冲区里的指定数目的数写到Kline端口里，成功返回真，失败返回假
// caBuf 为数据缓冲区的首地址，iNum 为数据的个数
// 前提是设备正常打开，写入没有问题（测试，流程有问题），
// 最终报的是读取数据有问题，即MES_KLINE_ZERO
/////////////////////////////////////////////////////////////////////
bool __fastcall TComKline::Write(unsigned char *caBuf, unsigned int iNum)
{
	DWORD			dwRc = 0;
	unsigned char	iRepeat = 0, cRet = 0;
	bool			bRet = false;


    // Kline 没打开就去打开 Kline
    // 这一步按照正常轨迹是不应该发生的，应该是不会发生的
    if (false == m_bComOpen)
    {
      	WriteLogFile(g_pFileComKline, "ReadChar In Com Close.!");
    	return false;
    }

	// Kline 成功打开后 向里面写数
	do
	{
		iRepeat++;
        // 清除接收缓冲区中的所有数据
		PurgeComm(m_hCom, PURGE_RXCLEAR);
		bRet = WriteFile(m_hCom, caBuf, iNum, &dwRc, NULL);
		if (bRet == false)
		{
			m_strMark = "Operate ComKline Failed!";
            WriteLogFile(g_pFileComKline, m_strMark);
			return false;
		}

		unsigned int i = 0;
		for (i = 0; i < iNum; i++)
		{
        	// 判断接收数与发送数是否相等，数据是否一致
			if ((ReadChar(cRet) == false) || (caBuf[i] != cRet))
			{
				cRet = cRet;
				break;
			}
		}

		if (i == iNum)
		{
			i = i;
			return true;
		}
	}
	while (iRepeat <= MES_REPEAT_BASE);

	return false;
}

/////////////////////////////////////////////////////////////////////
// 接收一个数据帧,以cChar开头,长度为cChar,并计算检查和
// 从Kline端口里读一个数据帧存放到数据缓冲区里，
// 该数据帧的起始位为cChar，后面还有cChar个数据。
// 如果能读到这样的数据帧，且满足校验和为0，返回为真；否则返回为假。
// caBuf 为数据缓冲区的首地址，cChar为要求的起始位。
// bKline 能否从Kline中读取数据
/////////////////////////////////////////////////////////////////////
bool __fastcall TComKline::Read(unsigned char *caBuf, unsigned char cChar, bool & bKline)
{
	unsigned char	iRepeat = 0;
    unsigned char	cRetChar = 0, cTemp = 0;
	bool			bRet = false;
    bKline = true;

    // Kline 没打开就去打开 Kline
    // 这一步按照正常轨迹是不应该发生的，应该是不会发生的
    if (false == m_bComOpen)
    {
      	WriteLogFile(g_pFileComKline, "ReadChar In Com Close.!");
    	return false;
    }

	// 找数据帧头，即数据长度cChar，最多读100Byte
	do
	{
		iRepeat++;
		bRet = ReadChar(cRetChar);
		if (false == bRet)
        {
        	if(1 == iRepeat) bKline = false;
        	return false;
        }

		if (100 == iRepeat)
        {
        	return false;
        }
	}
	while (cRetChar != cChar);

	*caBuf = cRetChar;
	caBuf++;

    // 计算校验和
	for (iRepeat = 0; iRepeat < cChar; iRepeat++)
	{
		bRet = ReadChar(cTemp);
		if (false == bRet)
        {
        	return false;
        }

		*caBuf = cTemp;
		cRetChar = cRetChar ^ *caBuf;
		caBuf++;
	}

	if (cRetChar == 0) return true;

	return false;
}

/////////////////////////////////////////////////////////////////////
//从Kline端口里读一个数
//s 为存放数据的变量的首地址，成功返回真，失败返回假
/////////////////////////////////////////////////////////////////////
bool __fastcall TComKline::ReadChar(unsigned char &cRet)
{
    COMSTAT cs;
    DWORD   dwBytesRead, dwError, dwTimeEnd = 0, dwTimeInit = 0;

    dwTimeInit = GetTickCount();
    while (dwTimeEnd < MES_MAX_WAITTIME)
    {
        // 查询K-Line状态
        ClearCommError(m_hCom, &dwError, &cs);
        if (cs.cbInQue >= 1)
        {
            ReadFile(m_hCom, &cRet, 1, &dwBytesRead, NULL);
            ///////////////////////////////////////////////////////////////////////////
            String strTemp = "";
            if(dwBytesRead != 0)
            {
                strTemp = IntToHex(cRet, 2) + " ";
            }
            WriteLogFile(g_pFileComKline, strTemp);
            ////////////////////////////////////////////////////////////////////////////
            return true;
        }

        // 获取系统时间
        dwTimeEnd = GetTickCount();
        if (dwTimeEnd < dwTimeInit)
        {
            dwTimeInit = GetTickCount();
            dwTimeEnd = 0;
        }
        else
            dwTimeEnd = dwTimeEnd - dwTimeInit;
    }

    return false;
}

bool __fastcall TComKline::ReadBuf(unsigned char * caBuf, unsigned int iNum)
{
	COMSTAT cs;
    DWORD   dwBytesRead = 0, dwError = 0, dwTimeEnd = 0, dwTimeStart = 0;

    // Kline 没打开就去打开 Kline
    // 这一步按照正常轨迹是不应该发生的，应该是不会发生的
    if (false == m_bComOpen)
    {
      	WriteLogFile(g_pFileComKline, "ReadBuf In Com Close.!");
    	return false;
    }

    dwTimeStart = GetTickCount();
    while (dwTimeEnd < MES_MAX_WAITTIME)
    {
        ClearCommError(m_hCom, &dwError, &cs);
        if (cs.cbInQue >= iNum)
        {
            ReadFile(m_hCom, caBuf, iNum, &dwBytesRead, NULL);
            ///////////////////////////////////////////////////////////////////////////
            String strTemp = "";
            for(unsigned int i = 0; i < dwBytesRead; i++)
            {
                strTemp = IntToHex(caBuf[i], 2) + " ";
            }
            WriteLogFile(g_pFileComKline, strTemp);
            ////////////////////////////////////////////////////////////////////////////
            return true;
        }

        // 获取系统时间
        G_SDelay(400);
        dwTimeEnd = GetTickCount();
        if (dwTimeEnd < dwTimeStart)
        {
            dwTimeStart = GetTickCount();
            dwTimeEnd = 0;
        }
        else
            dwTimeEnd = dwTimeEnd - dwTimeStart;
    }

    WriteLogFile(g_pFileComKline, "ReadBuf Failed.!");
    return false;
}

/////////////////////////////////////////////////////////////////////
// 已开启实时RTF模式
// 从Kline端口里查找指定的数据，并将查找过程中读取的数据存放到数据缓冲区里。
// 找到返回MES_KLINE_FOUND(1)，没找到返回MES_KLINE_NOTFOUND(0)，
// 通讯失败返回MES_KLINE_ZERO(-1)
// cChar 为指定的数据，caBuf为数据缓冲区首地址，
// iReadCount为一变量的地址，该变量存放查找过程中读取数据的总数
/////////////////////////////////////////////////////////////////////
int __fastcall TComKline::SearchChar(unsigned char cChar, unsigned char *caBuf, unsigned int & iReadCount)
{
	COMSTAT			cs;
	DWORD			dwCount = 0, dwBytesRead = 0, dwError = 0, dwTimeStart = 0, dwTimeEnd = 0;
	unsigned char	cBufCode = 0;
    unsigned int	iRepeat = 0;

    // Kline 没打开就去打开 Kline
    // 这一步按照正常轨迹是不应该发生的，应该是不会发生的
    if (false == m_bComOpen)
    {
      	WriteLogFile(g_pFileComKline, "SearchChar In Com Close.!");
    	return MES_KLINE_ZERO;
    }

    // 时间切入
	do
	{
		dwTimeStart = GetTickCount();
	}
	while (dwTimeStart > dwTimeStart + MES_MAX_WAITTIME);

    // 清除接收缓冲区中的所有数据
	PurgeComm(m_hCom, PURGE_RXCLEAR);
    // 串口里的数多于100个或超出最大容许时间都将退出循环
	do
	{
    	iRepeat++;
		Sleep(400); 							// ECU周期
		ClearCommError(m_hCom, &dwError, &cs);	// 查询K-Line状态
		dwCount = cs.cbInQue;
        // Frank
		dwTimeEnd = GetTickCount();				// 计时终点
	}
	while (iRepeat <= MES_REPEAT_BASE && dwTimeEnd <= dwTimeStart + MES_MAX_WAITTIME);

    // 串口没有数据
	if (dwCount == 0)
	{
		iReadCount = 0;
        m_strMark = "ComKline Device Operate fail.!";
        WriteLogFile(g_pFileComKline, "TComKline SearchChar no data in COMKline:    \t\n\n");
		return MES_KLINE_ZERO;
	}

    // 读取数据
	ReadFile(m_hCom, caBuf, dwCount, &dwBytesRead, NULL);
    ///////////////////////////////////////////////////////////////////////////
	String			strOut = "";
	unsigned char	cValue = 0;
	for (unsigned int iPos = 0; iPos < dwBytesRead; iPos++)
	{
		cValue = caBuf[iPos];
		strOut = strOut + IntToHex(cValue, 2) + " ";
	}
	strOut = strOut + ":\r\n";
	WriteLogFile(g_pFileComKline, strOut);
    ///////////////////////////////////////////////////////////////////////////

    // 从串口实际读到的数为0，一般不可能出现
	iReadCount = dwBytesRead;
	if (dwBytesRead == 0)
	{
    	m_strMark = "ComKline Device Operate fail.!";
        WriteLogFile(g_pFileComKline, "TComKline SearchChar read COMKline no data:    \t\n\n");
		return MES_KLINE_ZERO;
	}

	for (unsigned int iBytes = 0; iBytes < dwBytesRead; iBytes++)
	{
		cBufCode = caBuf[iBytes];
		if (cBufCode == cChar)
		{
			return MES_KLINE_FOUND;
		}
	}

	return MES_KLINE_NOTFOUND;
}


