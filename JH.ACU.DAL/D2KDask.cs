using System;
using System.Runtime.InteropServices;

namespace JH.ACU.DAL
{
    public delegate void CallbackDelegate();

    /// <summary>
    /// DAQ SDK
    /// </summary>
    public static class D2KDask
    {
        //DAQ2000 Device

        #region DAQ2000 Device

        public const ushort DAQ_2010 = 1;
        public const ushort DAQ_2205 = 2;
        public const ushort DAQ_2206 = 3;
        public const ushort DAQ_2005 = 4;
        public const ushort DAQ_2204 = 5;
        public const ushort DAQ_2006 = 6;
        public const ushort DAQ_2501 = 7;
        public const ushort DAQ_2502 = 8;
        public const ushort DAQ_2208 = 9;
        public const ushort DAQ_2213 = 10;
        public const ushort DAQ_2214 = 11;
        public const ushort DAQ_2016 = 12;
        public const ushort DAQ_2020 = 13;
        public const ushort DAQ_2022 = 14;

        public const ushort MAX_CARD = 32;

        #endregion


        //Error Number

        #region Error Number

        public enum Error : short
        {
            NoError = 0,
            ErrorUnknownCardType = -1,
            ErrorInvalidCardNumber = -2,
            ErrorTooManyCardRegistered = -3,
            ErrorCardNotRegistered = -4,
            ErrorFuncNotSupport = -5,
            ErrorInvalidIoChannel = -6,
            ErrorInvalidAdRange = -7,
            ErrorContIoNotAllowed = -8,
            ErrorDiffRangeNotSupport = -9,
            ErrorLastChannelNotZero = -10,
            ErrorChannelNotDescending = -11,
            ErrorChannelNotAscending = -12,
            ErrorOpenDriverFailed = -13,
            ErrorOpenEventFailed = -14,
            ErrorTransferCountTooLarge = -15,
            ErrorNotDoubleBufferMode = -16,
            ErrorInvalidSampleRate = -17,
            ErrorInvalidCounterMode = -18,
            ErrorInvalidCounter = -19,
            ErrorInvalidCounterState = -20,
            ErrorInvalidBinBcdParam = -21,
            ErrorBadCardType = -22,
            ErrorInvalidDaRefVoltage = -23,
            ErrorAdTimeOut = -24,
            ErrorNoAsyncAI = -25,
            ErrorNoAsyncAO = -26,
            ErrorNoAsyncDI = -27,
            ErrorNoAsyncDO = -28,
            ErrorNotInputPort = -29,
            ErrorNotOutputPort = -30,
            ErrorInvalidDioPort = -31,
            ErrorInvalidDioLine = -32,
            ErrorContIoActive = -33,
            ErrorDblBufModeNotAllowed = -34,
            ErrorConfigFailed = -35,
            ErrorInvalidPortDirection = -36,
            ErrorBeginThreadError = -37,
            ErrorInvalidPortWidth = -38,
            ErrorInvalidCtrSource = -39,
            ErrorOpenFile = -40,
            ErrorAllocateMemory = -41,
            ErrorDaVoltageOutOfRange = -42,
            ErrorInvalidSyncMode = -43,
            ErrorInvalidBufferID = -44,
            ErrorInvalidCNTInterval = -45,
            ErrorReTrigModeNotAllowed = -46,
            ErrorResetBufferNotAllowed = -47,
            ErrorAnaTriggerLevel = -48,
            ErrorDAQEvent = -49,
            ErrorInvalidCounterValue = -50,
            ErrorOffsetCalibration = -51,
            ErrorGainCalibration = -52,
            ErrorCountOutofSDRAMSize = -53,
            ErrorNotStartTriggerModule = -54,
            ErrorInvalidRouteLine = -55,
            ErrorInvalidSignalCode = -56,
            ErrorInvalidSignalDirection = -57,
            ErrorTRGOSCalibration = -58,
            ErrorNoSDRAM = -59,
            ErrorIntegrationGain = -60,
            ErrorAcquisitionTiming = -61,
            ErrorIntegrationTiming = -62,
            ErrorInvalidTimeBase = -70,
            ErrorUndefinedParameter = -71,
            //Error number for calibration API
            ErrorCalAddress = -110,
            ErrorInvalidCalBank = -111,
            //Error number for driver API 
            ErrorConfigIoctl = -201,
            ErrorAsyncSetIoctl = -202,
            ErrorDBSetIoctl = -203,
            ErrorDBHalfReadyIoctl = -204,
            ErrorContOPIoctl = -205,
            ErrorContStatusIoctl = -206,
            ErrorPIOIoctl = -207,
            ErrorDIntSetIoctl = -208,
            ErrorWaitEvtIoctl = -209,
            ErrorOpenEvtIoctl = -210,
            ErrorCOSIntSetIoctl = -211,
            ErrorMemMapIoctl = -212,
            ErrorMemUMapSetIoctl = -213,
            ErrorCTRIoctl = -214,
            ErrorGetResIoctl = -215,
        }

        #endregion

        //Synchronous Mode

        #region Synchronous Mode

        public const ushort SYNCH_OP = 1;
        public const ushort ASYNCH_OP = 2;

        #endregion


        //AD/AI Range

        #region AD/AI Range

        #region Valid values for DAQ-2206

        public const ushort AD_B_10_V = 1; //default
        public const ushort AD_B_5_V = 2;
        public const ushort AD_B_2_5_V = 3;
        public const ushort AD_B_1_25_V = 4;
        public const ushort AD_U_10_V = 15;
        public const ushort AD_U_5_V = 16;
        public const ushort AD_U_2_5_V = 17;
        public const ushort AD_U_1_25_V = 18;

        #endregion


        public const ushort AD_B_0_625_V = 5;
        public const ushort AD_B_0_3125_V = 6;
        public const ushort AD_B_0_5_V = 7;
        public const ushort AD_B_0_05_V = 8;
        public const ushort AD_B_0_005_V = 9;
        public const ushort AD_B_1_V = 10;
        public const ushort AD_B_0_1_V = 11;
        public const ushort AD_B_0_01_V = 12;
        public const ushort AD_B_0_001_V = 13;
        public const ushort AD_U_20_V = 14;



        public const ushort AD_U_1_V = 19;
        public const ushort AD_U_0_1_V = 20;
        public const ushort AD_U_0_01_V = 21;
        public const ushort AD_U_0_001_V = 22;
        public const ushort AD_B_2_V = 23;
        public const ushort AD_B_0_25_V = 24;
        public const ushort AD_B_0_2_V = 25;
        public const ushort AD_U_4_V = 26;
        public const ushort AD_U_2_V = 27;
        public const ushort AD_U_0_5_V = 28;
        public const ushort AD_U_0_4_V = 29;

        #endregion

        //DIO Port Direction

        #region DIO Port Direction

        /// <summary>
        /// D2K_DI_ReadPortÓÃ
        /// </summary>
        public const ushort INPUT_PORT = 1;

        /// <summary>
        /// D2K_DO_WritePortÓÃ
        /// </summary>
        public const ushort OUTPUT_PORT = 2;

        #endregion

        //Channel & Port

        #region Channel & Port

        public const ushort Channel_P1A = 0;
        public const ushort Channel_P1B = 1;
        public const ushort Channel_P1C = 2;
        public const ushort Channel_P1CL = 3;
        public const ushort Channel_P1CH = 4;
        public const ushort Channel_P1AE = 10;
        public const ushort Channel_P1BE = 11;
        public const ushort Channel_P1CE = 12;
        public const ushort Channel_P2A = 5;
        public const ushort Channel_P2B = 6;
        public const ushort Channel_P2C = 7;
        public const ushort Channel_P2CL = 8;
        public const ushort Channel_P2CH = 9;
        public const ushort Channel_P2AE = 15;
        public const ushort Channel_P2BE = 16;
        public const ushort Channel_P2CE = 17;
        public const ushort Channel_P3A = 10;
        public const ushort Channel_P3B = 11;
        public const ushort Channel_P3C = 12;
        public const ushort Channel_P3CL = 13;
        public const ushort Channel_P3CH = 14;
        public const ushort Channel_P4A = 15;
        public const ushort Channel_P4B = 16;
        public const ushort Channel_P4C = 17;
        public const ushort Channel_P4CL = 18;
        public const ushort Channel_P4CH = 19;
        public const ushort Channel_P5A = 20;
        public const ushort Channel_P5B = 21;
        public const ushort Channel_P5C = 22;
        public const ushort Channel_P5CL = 23;
        public const ushort Channel_P5CH = 24;
        public const ushort Channel_P6A = 25;
        public const ushort Channel_P6B = 26;
        public const ushort Channel_P6C = 27;
        public const ushort Channel_P6CL = 28;
        public const ushort Channel_P6CH = 29;

        #endregion


        /*-------- Constants for DAQ2000 --------------------*/

        #region Constants for DAQ2000

        public const short All_Channels = -1;
        public const short BufferNotUsed = -1;
        /* Constants for Analog trigger */

        #region Constants for Analog trigger

        /* define analog trigger condition constants */

        #region define analog trigger condition constants

        public const ushort Below_Low_level = 0x0000;
        public const ushort Above_High_Level = 0x0100;
        public const ushort Inside_Region = 0x0200;
        public const ushort High_Hysteresis = 0x0300;
        public const ushort Low_Hysteresis = 0x0400;

        #endregion

        /* define analog trigger Dedicated Channel */

        #region define analog trigger Dedicated Channel

        public const ushort CH0ATRIG = 0x00;
        public const ushort CH1ATRIG = 0x02;
        public const ushort CH2ATRIG = 0x04;
        public const ushort CH3ATRIG = 0x06;
        public const ushort EXTATRIG = 0x01;
        public const ushort ADCATRIG = 0x00; //used for DAQ-2205/2206

        #endregion

        /* Time Base */

        #region Time Base

        public const ushort DAQ2K_IntTimeBase = 0x00;
        public const ushort DAQ2K_ExtTimeBase = 0x01;
        public const ushort DAQ2K_SSITimeBase = 0x02;
        public const ushort DAQ2K_ExtTimeBase_AFI0 = 0x03;
        public const ushort DAQ2K_ExtTimeBase_AFI1 = 0x04;
        public const ushort DAQ2K_ExtTimeBase_AFI2 = 0x05;
        public const ushort DAQ2K_ExtTimeBase_AFI3 = 0x06;
        public const ushort DAQ2K_ExtTimeBase_AFI4 = 0x07;
        public const ushort DAQ2K_ExtTimeBase_AFI5 = 0x08;
        public const ushort DAQ2K_ExtTimeBase_AFI6 = 0x09;
        public const ushort DAQ2K_ExtTimeBase_AFI7 = 0xa;
        public const ushort DAQ2K_PXI_CLK = 0xc;
        public const ushort DAQ2K_StarTimeBase = 0xd;
        public const ushort DAQ2K_SMBTimeBase = 0xe;

        #endregion

        #endregion



        /* Constants for AD	*/

        #region Constants for AD

        public const ushort DAQ2K_AI_ADSTARTSRC_Int = 0x00;
        public const ushort DAQ2K_AI_ADSTARTSRC_AFI0 = 0x10;
        public const ushort DAQ2K_AI_ADSTARTSRC_SSI = 0x20;

        public const ushort DAQ2K_AI_ADCONVSRC_Int = 0x00;
        public const ushort DAQ2K_AI_ADCONVSRC_AFI0 = 0x04;
        public const ushort DAQ2K_AI_ADCONVSRC_SSI = 0x08;
        public const ushort DAQ2K_AI_ADCONVSRC_AFI1 = 0x0C;
        public const ushort DAQ2K_AI_ADCONVSRC_AFI2 = 0x100;
        public const ushort DAQ2K_AI_ADCONVSRC_AFI3 = 0x200;
        public const ushort DAQ2K_AI_ADCONVSRC_AFI4 = 0x300;
        public const ushort DAQ2K_AI_ADCONVSRC_AFI5 = 0x400;
        public const ushort DAQ2K_AI_ADCONVSRC_AFI6 = 0x500;
        public const ushort DAQ2K_AI_ADCONVSRC_AFI7 = 0x600;

        public const ushort DAQ2K_AI_ADCONVSRC_PFI0 = DAQ2K_AI_ADCONVSRC_AFI0;

        //AI Delay Counter SRC: only available for DAQ-250X
        public const ushort DAQ2K_AI_DTSRC_Int = 0x00;
        public const ushort DAQ2K_AI_DTSRC_AFI1 = 0x10;
        public const ushort DAQ2K_AI_DTSRC_GPTC0 = 0x20;
        public const ushort DAQ2K_AI_DTSRC_GPTC1 = 0x30;

        public const ushort DAQ2K_AI_TRGSRC_SOFT = 0x00;
        public const ushort DAQ2K_AI_TRGSRC_ANA = 0x01;
        public const ushort DAQ2K_AI_TRGSRC_ExtD = 0x02;
        public const ushort DAQ2K_AI_TRSRC_SSI = 0x03;
        public const ushort DAQ2K_AI_TRGMOD_POST = 0x00; //Post Trigger Mode
        public const ushort DAQ2K_AI_TRGMOD_DELAY = 0x08; //Delay Trigger Mode
        public const ushort DAQ2K_AI_TRGMOD_PRE = 0x10; //Pre-Trigger Mode
        public const ushort DAQ2K_AI_TRGMOD_MIDL = 0x18; //Middle Trigger Mode
        public const ushort DAQ2K_AI_ReTrigEn = 0x80;
        public const ushort DAQ2K_AI_Dly1InSamples = 0x100;
        public const ushort DAQ2K_AI_Dly1InTimebase = 0x000;
        public const ushort DAQ2K_AI_MCounterEn = 0x400;
        public const ushort DAQ2K_AI_TrgPositive = 0x0000;
        public const ushort DAQ2K_AI_TrgNegative = 0x1000;

        public const uint DAQ2K_AI_TRGSRC_AFI0 = 0x10000;
        public const uint DAQ2K_AI_TRGSRC_AFI1 = 0x20000;
        public const uint DAQ2K_AI_TRGSRC_AFI2 = 0x30000;
        public const uint DAQ2K_AI_TRGSRC_AFI3 = 0x40000;
        public const uint DAQ2K_AI_TRGSRC_AFI4 = 0x50000;
        public const uint DAQ2K_AI_TRGSRC_AFI5 = 0x60000;
        public const uint DAQ2K_AI_TRGSRC_AFI6 = 0x70000;
        public const uint DAQ2K_AI_TRGSRC_AFI7 = 0x80000;
        public const uint DAQ2K_AI_TRGSRC_PXIStar = 0xa0000;
        public const uint DAQ2K_AI_TRGSRC_SMB = 0xb0000;

        //AI Reference ground

        #region AI Reference ground

        public const ushort AI_RSE = 0x0000; //default
        public const ushort AI_DIFF = 0x0100;
        public const ushort AI_NRSE = 0x0200;

        #endregion


        #endregion

        /* Constants for DA	*/

        #region Constants for DA

        //DA CH config constant
        public const ushort DAQ2K_DA_BiPolar = 0x1;
        public const ushort DAQ2K_DA_UniPolar = 0x0;
        public const ushort DAQ2K_DA_Int_REF = 0x0;
        public const ushort DAQ2K_DA_Ext_REF = 0x1;
        //DA control constant
        public const ushort DAQ2K_DA_WRSRC_Int = 0x00;
        public const ushort DAQ2K_DA_WRSRC_AFI1 = 0x01;
        public const ushort DAQ2K_DA_WRSRC_SSI = 0x02;

        public const ushort DAQ2K_DA_WRSRC_PFI0 = DAQ2K_DA_WRSRC_AFI0;
        public const ushort DAQ2K_DA_WRSRC_AFI0 = DAQ2K_DA_WRSRC_AFI1;
        //DA group 
        public const ushort DA_Group_A = 0x00;
        public const ushort DA_Group_B = 0x04;
        public const ushort DA_Group_AB = 0x08;
        //DA TD Counter SRC: only available for DAQ-250X
        public const ushort DAQ2K_DA_TDSRC_Int = 0x00;
        public const ushort DAQ2K_DA_TDSRC_AFI0 = 0x10;
        public const ushort DAQ2K_DA_TDSRC_GPTC0 = 0x20;
        public const ushort DAQ2K_DA_TDSRC_GPTC1 = 0x30;
        //DA BD Counter SRC: only available for DAQ-250X
        public const ushort DAQ2K_DA_BDSRC_Int = 0x00;
        public const ushort DAQ2K_DA_BDSRC_AFI0 = 0x40;
        public const ushort DAQ2K_DA_BDSRC_GPTC0 = 0x80;
        public const ushort DAQ2K_DA_BDSRC_GPTC1 = 0xC0;

        //DA trigger constant
        public const ushort DAQ2K_DA_TRGSRC_SOFT = 0x00;
        public const ushort DAQ2K_DA_TRGSRC_ANA = 0x01;
        public const ushort DAQ2K_DA_TRGSRC_ExtD = 0x02;
        public const ushort DAQ2K_DA_TRSRC_SSI = 0x03;
        public const ushort DAQ2K_DA_TRGMOD_POST = 0x00;
        public const ushort DAQ2K_DA_TRGMOD_DELAY = 0x04;
        public const ushort DAQ2K_DA_ReTrigEn = 0x20;
        public const ushort DAQ2K_DA_Dly1InUI = 0x40;
        public const ushort DAQ2K_DA_Dly1InTimebase = 0x00;
        public const ushort DAQ2K_DA_Dly2InUI = 0x80;
        public const ushort DAQ2K_DA_Dly2InTimebase = 0x00;
        public const ushort DAQ2K_DA_DLY2En = 0x100;
        public const ushort DAQ2K_DA_TrgPositive = 0x000;
        public const ushort DAQ2K_DA_TrgNegative = 0x200;

        //DA stop mode
        public const ushort DAQ2K_DA_TerminateImmediate = 0;
        public const ushort DAQ2K_DA_TerminateUC = 1;
        public const ushort DAQ2K_DA_TerminateIC = 2;

        public const ushort DAQ2K_DA_TerminateFIFORC = DAQ2K_DA_TerminateIC;
        //DA stop source : only available for DAQ-250X
        public const ushort DAQ2K_DA_STOPSRC_SOFT = 0;
        public const ushort DAQ2K_DA_STOPSRC_AFI0 = 1;
        public const ushort DAQ2K_DA_STOPSRC_ATrig = 2;
        public const ushort DAQ2K_DA_STOPSRC_AFI1 = 3;

        #endregion

        #endregion

        /*-------- Timer/Counter -----------------------------*/

        #region Timer/Counter

        //Counter Mode (8254)
        public const ushort TOGGLE_OUTPUT = 0; //Toggle output from low to high on terminal count
        public const ushort PROG_ONE_SHOT = 1; //Programmable one-shot
        public const ushort RATE_GENERATOR = 2; //Rate generator
        public const ushort SQ_WAVE_RATE_GENERATOR = 3; //Square wave rate generator
        public const ushort SOFT_TRIG = 4; //Software-triggered strobe
        public const ushort HARD_TRIG = 5; //Hardware-triggered strobe
        //16-bit binary or 4-decade BCD counter
        public const ushort BIN = 0;
        public const ushort BCD = 1;
        //General Purpose Timer/Counter
        //Counter Mode
        public const ushort SimpleGatedEventCNT = 0x01;
        public const ushort SinglePeriodMSR = 0x02;
        public const ushort SinglePulseWidthMSR = 0x03;
        public const ushort SingleGatedPulseGen = 0x04;
        public const ushort SingleTrigPulseGen = 0x05;
        public const ushort RetrigSinglePulseGen = 0x06;
        public const ushort SingleTrigContPulseGen = 0x07;
        public const ushort ContGatedPulseGen = 0x08;
        //GPTC clock source
        public const ushort GPTC_GATESRC_EXT = 0x04;
        public const ushort GPTC_GATESRC_INT = 0x00;
        public const ushort GPTC_CLKSRC_EXT = 0x08;
        public const ushort GPTC_CLKSRC_INT = 0x00;
        public const ushort GPTC_UPDOWN_SEL_EXT = 0x10;
        public const ushort GPTC_UPDOWN_SEL_INT = 0x00;
        //GPTC clock polarity
        public const ushort GPTC_CLKEN_LACTIVE = 0x01;
        public const ushort GPTC_CLKEN_HACTIVE = 0x00;
        public const ushort GPTC_GATE_LACTIVE = 0x02;
        public const ushort GPTC_GATE_HACTIVE = 0x00;
        public const ushort GPTC_UPDOWN_LACTIVE = 0x04;
        public const ushort GPTC_UPDOWN_HACTIVE = 0x00;
        public const ushort GPTC_OUTPUT_LACTIVE = 0x08;
        public const ushort GPTC_OUTPUT_HACTIVE = 0x00;
        public const ushort GPTC_INT_LACTIVE = 0x10;
        public const ushort GPTC_INT_HACTIVE = 0x00;
        //GPTC paramID
        public const ushort GPTC_IntGATE = 0x00;
        public const ushort GPTC_IntUpDnCTR = 0x01;
        public const ushort GPTC_IntENABLE = 0x02;
        //SSI signal code
        public const ushort SSI_TIME = 1;
        public const ushort SSI_CONV = 2;
        public const ushort SSI_WR = 4;
        public const ushort SSI_ADSTART = 8;
        public const ushort SSI_ADTRIG = 0x20;
        public const ushort SSI_DATRIG = 0x40;

        //signal code for GPTC
        public const ushort GPTC_CLK_0 = 0x100;
        public const ushort GPTC_GATE_0 = 0x200;
        public const ushort GPTC_OUT_0 = 0x300;
        public const ushort GPTC_CLK_1 = 0x400;
        public const ushort GPTC_GATE_1 = 0x500;
        public const ushort GPTC_OUT_1 = 0x600;
        //signal code for clockoutToSMB source
        public const ushort PXI_CLK_10_M = 0x1000;
        public const ushort CLK_20_M = 0x2000;
        public const ushort SMB_CLK_IN = 0x3000;

        //signal route lines
        public const ushort PXI_TRIG_0 = 0;
        public const ushort PXI_TRIG_1 = 1;
        public const ushort PXI_TRIG_2 = 2;
        public const ushort PXI_TRIG_3 = 3;
        public const ushort PXI_TRIG_4 = 4;
        public const ushort PXI_TRIG_5 = 5;
        public const ushort PXI_TRIG_6 = 6;
        public const ushort PXI_TRIG_7 = 7;
        public const ushort PXI_STAR_TRIG = 8;
        public const ushort TRG_IO = 9;
        public const ushort SMB_CLK_OUT = 10;
        public const ushort AFI0 = 0x10;
        public const ushort AFI1 = 0x11;
        public const ushort AFI2 = 0x12;
        public const ushort AFI3 = 0x13;
        public const ushort AFI4 = 0x14;
        public const ushort AFI5 = 0x15;
        public const ushort AFI6 = 0x16;
        public const ushort AFI7 = 0x17;
        public const ushort PXI_CLK = 0x18;

        //export signal plarity
        public const ushort Signal_ActiveHigh = 0x0;
        public const ushort Signal_ActiveLow = 0x1;

        //DAQ Event type for the event message  
        public const ushort DAQEnd = 0;
        public const ushort DBEvent = 1;
        public const ushort TrigEvent = 2;
        public const ushort DAQEnd_A = 0;
        public const ushort DAQEnd_B = 2;
        public const ushort DAQEnd_AB = 3;
        public const ushort DATrigEvent = 4;
        public const ushort DATrigEvent_A = 4;
        public const ushort DATrigEvent_B = 5;
        public const ushort DATrigEvent_AB = 6;

        #endregion



        /*------------------------------------------------------------------
	** PCIS-DASK Function prototype
	------------------------------------------------------------------*/

        #region °å¿¨³õÊ¼»¯

        /// <summary>
        /// ×¢²á°å¿¨
        /// </summary>
        /// <param name="CardType">°å¿¨ÀàÐÍ</param>
        /// <param name="card_num"></param>
        /// <returns>°å¿¨ºÅ</returns>
        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_Register_Card(ushort CardType, ushort card_num);

        /// <summary>
        /// ÊÍ·Å°å¿¨
        /// </summary>
        /// <param name="CardNumber"></param>
        /// <returns></returns>
        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_Release_Card(ushort CardNumber);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_AIO_Config(ushort wCardNumber, ushort TimerBase, ushort AnaTrigCtrl,
            ushort H_TrgLevel, ushort L_TrgLevel);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_Register_Card_By_PXISlot_GA(ushort CardType, ushort ga);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_GetPXISlotGeographAddr(ushort wCardNumber, out bool geo_addr);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_SoftTrigGen(ushort wCardNumber, byte op);

        #endregion

        #region AIÅäÖÃ¼°¶ÁÐ´

        /*---------------------------------------------------------------------------*/

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_AI_Config(ushort wCardNumber, ushort ConfigCtrl, uint TrigCtrl,
            uint MidOrDlyScans,
            ushort MCnt, ushort ReTrgCnt, bool AutoResetBuf);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_AI_ConfigEx(ushort wCardNumber, ushort ConfigCtrl, uint TrigCtrl,
            uint MidOrDlyScans,
            uint MCnt, uint ReTrgCnt, bool AutoResetBuf);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_AI_PostTrig_Config(ushort wCardNumber, ushort ClkSrc, uint TrigSrcCtrl,
            ushort ReTrgEn, ushort ReTrgCnt, bool AutoResetBuf);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_AI_PostTrig_ConfigEx(ushort wCardNumber, ushort ClkSrc, uint TrigSrcCtrl,
            ushort ReTrgEn, uint ReTrgCnt, bool AutoResetBuf);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_AI_DelayTrig_Config(ushort wCardNumber, ushort ClkSrc, uint TrigSrcCtrl,
            uint DlyScans, ushort ReTrgEn, ushort ReTrgCnt, bool AutoResetBuf);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_AI_DelayTrig_ConfigEx(ushort wCardNumber, ushort ClkSrc, uint TrigSrcCtrl,
            uint DlyScans, ushort ReTrgEn, uint ReTrgCnt, bool AutoResetBuf);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_AI_PreTrig_Config(ushort wCardNumber, ushort ClkSrc, uint TrigSrcCtrl,
            ushort MCtrEn,
            ushort MCnt, bool AutoResetBuf);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_AI_PreTrig_ConfigEx(ushort wCardNumber, ushort ClkSrc, uint TrigSrcCtrl,
            ushort MCtrEn, uint MCnt, bool AutoResetBuf);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_AI_MiddleTrig_Config(ushort wCardNumber, ushort ClkSrc, uint TrigSrcCtrl,
            uint MiddleScans, ushort MCtrEn, ushort MCnt, bool AutoResetBuf);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_AI_MiddleTrig_ConfigEx(ushort wCardNumber, ushort ClkSrc, uint TrigSrcCtrl,
            uint MiddleScans, ushort MCtrEn, uint MCnt, bool AutoResetBuf);

        /// <summary>
        /// Informs the D2K-DASK library of the selected AI range for the
        /// specified channel of the card with CardNumber ID. After calling
        /// the D2K_Register_Card function, all analog input channels are
        /// configured as AD_B_10_V (for DAQ-2010, DAQ-2005, DAQ-
        /// 2006, DAQ-2016, DAQ-2020/2022, DAQ-2501, and DAQ-2502) or
        /// AD_B_10_V with AI_RSE (for DAQ-2204, DAQ-2205, DAQ-2206,
        /// DAQ-2213, DAQ-2214, and DAQ-2208) by default. If you want to
        /// use the device with the default settings, there is no need to call
        /// this function again to configure the channel(s). You must call this
        /// function to program the device based on your settings before call-
        /// ing function to perform analog input operation.
        /// </summary>
        /// <param name="wCardNumber"></param>
        /// <param name="wChannel"></param>
        /// <param name="wAdRange_RefGnd">AI Range |AI Reference ground</param>
        /// <returns></returns>
        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_AI_CH_Config(ushort wCardNumber, short wChannel, ushort wAdRange_RefGnd);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_AI_InitialMemoryAllocated(ushort CardNumber, out uint MemSize);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_AI_ReadChannel(ushort CardNumber, ushort Channel, out ushort Value);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_AI_VReadChannel(ushort CardNumber, ushort Channel, out double voltage);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_AI_SimuReadChannel(ushort wCardNumber, ushort wNumChans, ushort[] pwChans,
            ushort[] pwBuffer);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_AI_SimuReadChannel(ushort wCardNumber, ushort wNumChans, ushort[] pwChans,
            IntPtr pwBuffer);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_AI_SimuReadChannel(ushort wCardNumber, ushort wNumChans, IntPtr pwChans,
            IntPtr pwBuffer);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_AI_ScanReadChannels(ushort wCardNumber, ushort wNumChans, ushort[] pwChans,
            ushort[] pwBuffer);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_AI_ScanReadChannels(ushort wCardNumber, ushort wNumChans, ushort[] pwChans,
            IntPtr pwBuffer);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_AI_ScanReadChannels(ushort wCardNumber, ushort wNumChans, IntPtr pwChans,
            IntPtr pwBuffer);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_AI_VoltScale(ushort CardNumber, ushort AdRange, short reading, out double voltage);

        /// <summary>
        /// Performs continuous A/D conversions on the specified analog
        /// input channel at a rate closest to the specified rate.
        /// </summary>
        /// <param name="CardNumber"></param>
        /// <param name="Channel">0~63</param>
        /// <param name="BufId"></param>
        /// <param name="ReadScans"></param>
        /// <param name="ScanIntrv"></param>
        /// <param name="SampIntrv"></param>
        /// <param name="SyncMode">SYNCH_OP or ASYNCH_OP</param>
        /// <returns></returns>
        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_AI_ContReadChannel(ushort CardNumber, ushort Channel,
            ushort BufId, uint ReadScans, uint ScanIntrv, uint SampIntrv, ushort SyncMode);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_AI_ContReadMultiChannels(ushort CardNumber, ushort NumChans, ushort[] Chans,
            ushort BufId, uint ReadScans, uint ScanIntrv, uint SampIntrv, ushort SyncMode);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_AI_ContReadMultiChannels(ushort CardNumber, ushort NumChans, IntPtr Chans,
            ushort BufId, uint ReadScans, uint ScanIntrv, uint SampIntrv, ushort SyncMode);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_AI_ContScanChannels(ushort CardNumber, ushort Channel,
            ushort BufId, uint ReadScans, uint ScanIntrv, uint SampIntrv, ushort SyncMode);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_AI_ContReadChannelToFile(ushort CardNumber, ushort Channel, ushort BufId,
            string FileName, uint ReadScans, uint ScanIntrv, uint SampIntrv, ushort SyncMode);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_AI_ContReadMultiChannelsToFile(ushort CardNumber, ushort NumChans, ushort[] Chans,
            ushort BufId, string FileName, uint ReadScans, uint ScanIntrv, uint SampIntrv, ushort SyncMode);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_AI_ContReadMultiChannelsToFile(ushort CardNumber, ushort NumChans, IntPtr Chans,
            ushort BufId, string FileName, uint ReadScans, uint ScanIntrv, uint SampIntrv, ushort SyncMode);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_AI_ContScanChannelsToFile(ushort CardNumber, ushort Channel, ushort BufId,
            string FileName, uint ReadScans, uint ScanIntrv, uint SampIntrv, ushort SyncMode);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_AI_ContStatus(ushort CardNumber, out ushort Status);

        /// <summary>
        /// Converts the values of an array of acquired binary data from a
        /// continuous A/D conversion call to actual input voltages. The
        /// acquired binary data in the reading array may include the channel
        /// information (refer to D2K_AI_ContReadChannel or
        /// D2K_AI_ContScanChannels for the detailed data format). How-
        /// ever, the calculated voltage values in the voltage array returned
        /// does not include the channel message.
        /// </summary>
        /// <param name="wCardNumber"></param>
        /// <param name="adRange"></param>
        /// <param name="readingArray"></param>
        /// <param name="voltageArray"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_AI_ContVScale(ushort wCardNumber, ushort adRange, short[] readingArray,
            out double[] voltageArray, int count);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_AI_ContVScale(ushort wCardNumber, ushort adRange, IntPtr readingArray,
            out double[] voltageArray, int count);//QUES:Ôö¼Óout¹Ø¼ü×Ö

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_AI_ContVScale(ushort wCardNumber, ushort adRange, IntPtr readingArray,
            out IntPtr voltageArray, int count);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_AI_AsyncCheck(ushort CardNumber, out byte Stopped, out uint AccessCnt);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_AI_AsyncClear(ushort CardNumber, out uint StartPos, out uint AccessCnt);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_AI_AsyncClearEx(ushort CardNumber, out uint StartPos, out uint AccessCnt,
            uint NoWait);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_AI_AsyncDblBufferHalfReady(ushort CardNumber, out byte HalfReady,
            out byte StopFlag);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_AI_AsyncDblBufferMode(ushort CardNumber, bool Enable);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_AI_AsyncDblBufferToFile(ushort CardNumber);

        /// <summary>
        /// Sets up the buffer for continuous analog input. The function has to
        /// be called repeatedly to setup all the data buffers (two buffers max-
        /// imum). For double buffer mode and infinite re-trigger mode of con-
        /// tinuous AI, calling D2K_AI_ContBufferSetup twice sets up the ring
        /// buffer to store the data.
        /// </summary>
        /// <param name="wCardNumber"></param>
        /// <param name="pwBuffer">Starting address of the memory to contain the input data.</param>
        /// <param name="dwReadCount">Buffer size (in samples); value must be even.</param>
        /// <param name="BufferId">Returns the index of the currently set up buffer.</param>
        /// <returns></returns>
        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_AI_ContBufferSetup(ushort wCardNumber, short[] pwBuffer, uint dwReadCount,
            out ushort BufferId);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_AI_ContBufferSetup(ushort wCardNumber, IntPtr pwBuffer, uint dwReadCount,
            out ushort BufferId);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_AI_ContBufferReset(ushort wCardNumber);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_AI_MuxScanSetup(ushort wCardNumber, ushort wNumChans, ushort[] pwChans,
            ushort[] pwAdRange_RefGnds);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_AI_MuxScanSetup(ushort wCardNumber, ushort wNumChans, IntPtr pwChans,
            IntPtr pwAdRange_RefGnds);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_AI_ReadMuxScan(ushort wCardNumber, ushort[] pwBuffer);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_AI_ReadMuxScan(ushort wCardNumber, IntPtr pwBuffer);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_AI_ContMuxScan(ushort wCardNumber, ushort BufId, uint ReadScans, uint ScanIntrv,
            uint SampIntrv, ushort wSyncMode);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_AI_ContMuxScanToFile(ushort wCardNumber, ushort BufId, string fileName,
            uint ReadScans, uint ScanIntrv, uint SampIntrv, ushort wSyncMode);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_AI_EventCallBack(ushort wCardNumber, short mode, ushort EventType,
            MulticastDelegate callbackAddr);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_AI_AsyncReTrigNextReady(ushort wCardNumber, out byte trgReady, out byte StopFlag,
            out ushort RdyTrigCnt);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_AI_AsyncReTrigNextReadyEx(ushort wCardNumber, out byte trgReady,
            out byte StopFlag,
            out uint RdyTrigCnt);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_AI_AsyncDblBufferHandled(ushort wCardNumber);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_AI_AsyncDblBufferOverrun(ushort wCardNumber, ushort op, out ushort overrunFlag);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_AI_SetTimeout(ushort wCardNumber, uint msec);

        #endregion

        #region AOÅäÖÃ¼°¶ÁÐ´

        /*---------------------------------------------------------------------------*/

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_AO_CH_Config(ushort wCardNumber, ushort wChannel, ushort wOutputPolarity,
            ushort wIntOrExtRef, double refVoltage);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_AO_Config(ushort wCardNumber, ushort ConfigCtrl, ushort TrigCtrl, ushort ReTrgCnt,
            ushort DLY1Cnt, ushort DLY2Cnt, bool AutoResetBuf);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_AO_PostTrig_Config(ushort wCardNumber, ushort ClkSrc, ushort TrigSrcCtrl,
            ushort DLY2Ctrl, ushort DLY2Cnt, ushort ReTrgEn, ushort ReTrgCnt, bool AutoResetBuf);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_AO_DelayTrig_Config(ushort wCardNumber, ushort ClkSrc, ushort TrigSrcCtrl,
            ushort DLY1Cnt, ushort DLY2Ctrl, ushort DLY2Cnt, ushort ReTrgEn, ushort ReTrgCnt, bool AutoResetBuf);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_AO_InitialMemoryAllocated(ushort CardNumber, out uint MemSize);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_AO_WriteChannel(ushort CardNumber, ushort Channel, short Value);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_AO_VWriteChannel(ushort CardNumber, ushort Channel, double Voltage);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_AO_VoltScale(ushort CardNumber, ushort Channel, double Voltage,
            out short binValue);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_AO_ContWriteChannel(ushort wCardNumber, ushort wChannel,
            ushort BufId, uint dwUpdateCount, uint wIterations, uint dwCHUI, ushort definite, ushort wSyncMode);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_AO_ContWriteMultiChannels(ushort wCardNumber, ushort wNumChans, ushort[] pwChans,
            ushort BufId, uint dwUpdateCount, uint wIterations, uint dwCHUI, ushort definite, ushort wSyncMode);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_AO_ContWriteMultiChannels(ushort wCardNumber, ushort wNumChans, IntPtr pwChans,
            ushort BufId, uint dwUpdateCount, uint wIterations, uint dwCHUI, ushort definite, ushort wSyncMode);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_AO_AsyncCheck(ushort CardNumber, out byte Stopped, out uint WriteCnt);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_AO_AsyncClear(ushort CardNumber, out uint WriteCnt, ushort stop_mode);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_AO_AsyncClearEx(ushort CardNumber, out uint WriteCnt, ushort stop_mode,
            uint NoWait);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_AO_AsyncDblBufferHalfReady(ushort CardNumber, out byte HalfReady);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_AO_AsyncDblBufferMode(ushort CardNumber, bool Enable);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_AO_SimuWriteChannel(ushort wCardNumber, ushort wNumChans, ushort[] pwBuffer);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_AO_SimuWriteChannel(ushort wCardNumber, ushort wNumChans, IntPtr pwBuffer);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_AO_ContBufferSetup(ushort wCardNumber, short[] pwBuffer, uint dwWriteCount,
            out ushort BufferId);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_AO_ContBufferSetup(ushort wCardNumber, IntPtr pwBuffer, uint dwWriteCount,
            out ushort BufferId);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_AO_ContBufferReset(ushort wCardNumber);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_AO_ContStatus(ushort CardNumber, out ushort Status);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_AO_ContBufferComposeAll(ushort wCardNumber, ushort group, uint dwUpdateCount,
            short[] ConBuffer, short[] pwBuffer, bool fifoload);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_AO_ContBufferComposeAll(ushort wCardNumber, ushort group, uint dwUpdateCount,
            IntPtr ConBuffer, IntPtr pwBuffer, bool fifoload);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_AO_ContBufferCompose(ushort wCardNumber, ushort group, ushort wChannel,
            uint dwUpdateCount, short[] ConBuffer, short[] pwBuffer, bool fifoload);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_AO_ContBufferCompose(ushort wCardNumber, ushort group, ushort wChannel,
            uint dwUpdateCount, IntPtr ConBuffer, IntPtr pwBuffer, bool fifoload);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_AO_EventCallBack(ushort wCardNumber, short mode, short EventType,
            MulticastDelegate callbackAddr);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_AO_SetTimeout(ushort wCardNumber, uint msec);

        #endregion

        #region AO Group²Ù×÷

        /*---------------------------------------------------------------------------*/

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_AO_Group_Setup(ushort wCardNumber, ushort group, ushort wNumChans,
            ushort[] pwChans);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_AO_Group_Setup(ushort wCardNumber, ushort group, ushort wNumChans, IntPtr pwChans);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_AO_Group_Update(ushort CardNumber, ushort group, short[] pwBuffer);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_AO_Group_Update(ushort CardNumber, ushort group, IntPtr pwBuffer);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_AO_Group_VUpdate(ushort CardNumber, ushort group, double[] pVoltage);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_AO_Group_VUpdate(ushort CardNumber, ushort group, IntPtr pVoltage);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_AO_Group_FIFOLoad(ushort wCardNumber, ushort group, ushort BufId,
            uint dwWriteCount);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_AO_Group_FIFOLoad_2(ushort wCardNumber, ushort group, ushort BufId,
            uint dwWriteCount);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_AO_Group_WFM_StopConfig(ushort wCardNumber, ushort group, ushort stopSrc,
            ushort stopMode);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_AO_Group_WFM_Start(ushort wardNumber, ushort group, ushort fstBufIdOrNotUsed,
            ushort sndBufId,
            uint dwUpdateCount, uint wIterations, uint dwCHUI, ushort definite);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_AO_Group_WFM_AsyncCheck(ushort CardNumber, ushort group, out byte Stopped,
            out uint WriteCnt);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_AO_Group_WFM_AsyncClear(ushort CardNumber, ushort group, out uint WriteCnt,
            ushort stop_mode);

        #endregion

        #region DI¶ÁÈ¡

        /*---------------------------------------------------------------------------*/

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_DI_ReadLine(ushort CardNumber, ushort Port, ushort Line, out ushort State);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_DI_ReadPort(ushort CardNumber, ushort Port, out uint Value);

        #endregion

        #region DO¶ÁÐ´

        /*---------------------------------------------------------------------------*/

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_DO_WriteLine(ushort CardNumber, ushort Port, ushort Line, ushort Value);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_DO_WritePort(ushort CardNumber, ushort Port, byte Value);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_DO_ReadLine(ushort CardNumber, ushort Port, ushort Line, out ushort Value);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_DO_ReadPort(ushort CardNumber, ushort Port, out uint Value);

        #endregion

        #region DIOÅäÖÃ

        /*---------------------------------------------------------------------------*/

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_DIO_PortConfig(ushort CardNumber, ushort Port, ushort Direction);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_DIO_LineConfig(ushort CardNumber, ushort Port, uint Line, ushort Direction);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_DIO_LinesConfig(ushort CardNumber, ushort Port, uint Linesdirmap);

        #endregion

        #region ÆäËû°å¿¨²Ù×÷

        /*---------------------------------------------------------------------------*/

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_GCTR_Setup(ushort wCardNumber, ushort wGCtr, ushort wMode, byte SrcCtrl,
            byte PolCtrl,
            ushort LReg1_Val, ushort LReg2_Val);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_GCTR_SetupEx(ushort wCardNumber, ushort wGCtr, ushort wMode, byte SrcCtrl,
            byte PolCtrl, uint LReg1_Val, uint LReg2_Val);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_GCTR_Control(ushort wCardNumber, ushort wGCtr, ushort ParamID, ushort Value);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_GCTR_Reset(ushort wCardNumber, ushort wGCtr);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_GCTR_Read(ushort wCardNumber, ushort wGCtr, out uint pValue);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_GCTR_Status(ushort wCardNumber, ushort wGCtr, out ushort pValue);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_SSI_SourceConn(ushort wCardNumber, ushort sigCode);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_SSI_SourceDisConn(ushort wCardNumber, ushort sigCode);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_SSI_SourceClear(ushort wCardNumber);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_Route_Signal(ushort wCardNumber, ushort signal, ushort polarity, ushort Line,
            ushort dir);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_Signal_DisConn(ushort wCardNumber, ushort signal, ushort polarity, ushort Line);

        [DllImport("D2K-Dask.dll")]
        public static extern short DAQ2205_Acquire_DA_Error(ushort wCardNumber, ushort channel, ushort polarity,
            out float da0v_err, out float da5v_err);

        [DllImport("D2K-Dask.dll")]
        public static extern short DAQ2205_Acquire_AD_Error(ushort wCardNumber, out float gain_err,
            out float bioffset_err,
            out float unioffset_err, out float hg_bios_err);

        [DllImport("D2K-Dask.dll")]
        public static extern short DAQ2206_Acquire_DA_Error(ushort wCardNumber, ushort channel, ushort polarity,
            out float da0v_err, out float da5v_err);

        [DllImport("D2K-Dask.dll")]
        public static extern short DAQ2206_Acquire_AD_Error(ushort wCardNumber, out float gain_err,
            out float bioffset_err,
            out float unioffset_err, out float hg_bios_err);

        [DllImport("D2K-Dask.dll")]
        public static extern short DAQ2213_Acquire_AD_Error(ushort wCardNumber, out float gain_err,
            out float bioffset_err,
            out float unioffset_err, out float hg_bios_err);

        [DllImport("D2K-Dask.dll")]
        public static extern short DAQ2214_Acquire_DA_Error(ushort wCardNumber, ushort channel, ushort polarity,
            out float da0v_err, out float da5v_err);

        [DllImport("D2K-Dask.dll")]
        public static extern short DAQ2214_Acquire_AD_Error(ushort wCardNumber, out float gain_err,
            out float bioffset_err,
            out float unioffset_err, out float hg_bios_err);

        [DllImport("D2K-Dask.dll")]
        public static extern short DAQ2010_Acquire_AD_Error(ushort wCardNumber, ushort channel, ushort polarity,
            out float gain_err, out float offset_err);

        [DllImport("D2K-Dask.dll")]
        public static extern short DAQ2010_Acquire_DA_Error(ushort wCardNumber, ushort channel, ushort polarity,
            out float gain_err, out float offset_err);

        [DllImport("D2K-Dask.dll")]
        public static extern short DAQ2005_Acquire_AD_Error(ushort wCardNumber, ushort channel, ushort polarity,
            out float gain_err, out float offset_err);

        [DllImport("D2K-Dask.dll")]
        public static extern short DAQ2005_Acquire_DA_Error(ushort wCardNumber, ushort channel, ushort polarity,
            out float gain_err, out float offset_err);

        [DllImport("D2K-Dask.dll")]
        public static extern short DAQ2006_Acquire_AD_Error(ushort wCardNumber, ushort channel, ushort polarity,
            out float gain_err, out float offset_err);

        [DllImport("D2K-Dask.dll")]
        public static extern short DAQ2006_Acquire_DA_Error(ushort wCardNumber, ushort channel, ushort polarity,
            out float gain_err, out float offset_err);

        [DllImport("D2K-Dask.dll")]
        public static extern short DAQ2016_Acquire_AD_Error(ushort wCardNumber, ushort channel, ushort polarity,
            out float gain_err, out float offset_err);

        [DllImport("D2K-Dask.dll")]
        public static extern short DAQ2016_Acquire_DA_Error(ushort wCardNumber, ushort channel, ushort polarity,
            out float gain_err, out float offset_err);

        [DllImport("D2K-Dask.dll")]
        public static extern short DAQ2204_Acquire_DA_Error(ushort wCardNumber, ushort channel, ushort polarity,
            out float da0v_err, out float da5v_err);

        [DllImport("D2K-Dask.dll")]
        public static extern short DAQ2204_Acquire_AD_Error(ushort wCardNumber, out float gain_err,
            out float bioffset_err,
            out float unioffset_err, out float hg_bios_err);

        [DllImport("D2K-Dask.dll")]
        public static extern short DAQ2208_Acquire_AD_Error(ushort wCardNumber, out float gain_err,
            out float bioffset_err,
            out float unioffset_err, out float hg_bios_err);

        [DllImport("D2K-Dask.dll")]
        public static extern short DAQ250X_Acquire_DA_Error(short wCardNumber, ushort channel, ushort polarity,
            out float gain_err, out float offset_err);

        [DllImport("D2K-Dask.dll")]
        public static extern short DAQ250X_Acquire_AD_Error(short wCardNumber, ushort polarity, out float gain_err,
            out float offset_err);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_DB_Auto_Calibration_ALL(ushort wCardNumber);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_EEPROM_CAL_Constant_Update(ushort wCardNumber, ushort bank);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_Load_CAL_Data(ushort wCardNumber, ushort bank);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_Set_Default_Load_Area(ushort wCardNumber, ushort bank);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_Get_Default_Load_Area(ushort wCardNumber);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_AI_GetEvent(ushort wCardNumber, out long hEvent);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_AO_GetEvent(ushort wCardNumber, out long hEvent);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_DI_GetEvent(ushort wCardNumber, out long hEvent);

        [DllImport("D2K-Dask.dll")]
        public static extern short D2K_DO_GetEvent(ushort wCardNumber, out long hEvent);

        #endregion

        #region ¹«¹²·½·¨

        public static void ThrowException(Error errorCode, Exception innerException = null)
        {
            if (errorCode >= Error.NoError) return;
            throw new Exception(errorCode.ToString(), innerException);
        }

        #endregion

    }
}