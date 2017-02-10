/***************************************************************************
     gpib_user.h  -  constant definition for 32-bit GPIB DLL (gpib-32.dll)
   

    copyright            : (C) ADLINK Tech. Inc.
 ***************************************************************************/

#ifndef _GPIB_USER_H
#define _GPIB_USER_H

#define GPIB_MAX_NUM_BOARDS 16
#define GPIB_MAX_NUM_DESCRIPTORS 0x1000

typedef unsigned char   U8;
typedef short           I16;
typedef unsigned short  U16;
typedef long            I32;
typedef unsigned long   U32;
typedef float           F32;
typedef double          F64;

enum ibsta_bit_numbers
{
	DCAS_NUM = 0,
	DTAS_NUM = 1,
	LACS_NUM = 2,
	TACS_NUM = 3,
	ATN_NUM = 4,
	CIC_NUM = 5,
	REM_NUM = 6,
	LOK_NUM = 7,
	CMPL_NUM = 8,
	EVENT_NUM = 9,
	SPOLL_NUM = 10,
	RQS_NUM = 11,
	SRQI_NUM = 12,
	END_NUM = 13,
	TIMO_NUM = 14,
	ERR_NUM = 15
};

/* IBSTA status bits (returned by all functions) */
enum ibsta_bits
{
	DCAS = ( 1 << DCAS_NUM ),	/* device clear state */
	DTAS = ( 1 << DTAS_NUM ),	/* device trigger state */
	LACS = ( 1 <<  LACS_NUM ),	/* GPIB interface is addressed as Listener */
	TACS = ( 1 <<  TACS_NUM ),	/* GPIB interface is addressed as Talker */
	ATN = ( 1 <<  ATN_NUM ),	/* Attention is asserted */
	CIC = ( 1 <<  CIC_NUM ),	/* GPIB interface is Controller-in-Charge */
	REM = ( 1 << REM_NUM ),	/* remote state */
	LOK = ( 1 << LOK_NUM ),	/* lockout state */
	CMPL = ( 1 <<  CMPL_NUM ),	/* I/O is complete  */
	EVENT = ( 1 << EVENT_NUM ),	/* DCAS, DTAS, or IFC has occurred */
	SPOLL = ( 1 << SPOLL_NUM ),	/* board serial polled by busmaster */
	RQS = ( 1 <<  RQS_NUM ),	/* Device requesting service  */
	SRQI = ( 1 << SRQI_NUM ),	/* SRQ is asserted */
	END = ( 1 << END_NUM ),	/* EOI or EOS encountered */
	TIMO = ( 1 << TIMO_NUM ),	/* Time limit on I/O or wait function exceeded */
	ERR = ( 1 << ERR_NUM )	/* Function call terminated on error */
};
/* IBERR error codes */
enum iberr_code
{
	EDVR = 0,		/* system error */
	ECIC = 1,	/* not CIC */
	ENOL = 2,		/* no listeners */
	EADR = 3,		/* CIC and not addressed before I/O */
	EARG = 4,		/* bad argument to function call */
	ESAC = 5,		/* not SAC */
	EABO = 6,		/* I/O operation was aborted */
	ENEB = 7,		/* non-existent board (GPIB interface offline) */
	EDMA = 8,		/* DMA hardware error detected */
	EOIP = 10,		/* new I/O attempted with old I/O in progress  */
	ECAP = 11,		/* no capability for intended opeation */
	EFSO = 12,		/* file system operation error */
	EBUS = 14,		/* bus error */
	ESTB = 15,		/* lost serial poll bytes */
	ESRQ = 16,		/* SRQ stuck on */
	ETAB = 20              /* Table Overflow */
};
/* Timeout values and meanings */

enum gpib_timeout
{
	TNONE = 0,		/* Infinite timeout (disabled)     */
	T10us = 1,		/* Timeout of 10 usec (ideal)      */
	T30us = 2,		/* Timeout of 30 usec (ideal)      */
	T100us = 3,		/* Timeout of 100 usec (ideal)     */
	T300us = 4,		/* Timeout of 300 usec (ideal)     */
	T1ms = 5,		/* Timeout of 1 msec (ideal)       */
	T3ms = 6,		/* Timeout of 3 msec (ideal)       */
	T10ms = 7,		/* Timeout of 10 msec (ideal)      */
	T30ms = 8,		/* Timeout of 30 msec (ideal)      */
	T100ms = 9,		/* Timeout of 100 msec (ideal)     */
	T300ms = 10,	/* Timeout of 300 msec (ideal)     */
	T1s = 11,		/* Timeout of 1 sec (ideal)        */
	T3s = 12,		/* Timeout of 3 sec (ideal)        */
	T10s = 13,		/* Timeout of 10 sec (ideal)       */
	T30s = 14,		/* Timeout of 30 sec (ideal)       */
	T100s = 15,		/* Timeout of 100 sec (ideal)      */
	T300s = 16,		/* Timeout of 300 sec (ideal)      */
	T1000s = 17		/* Timeout of 1000 sec (maximum)   */
};

/* End-of-string (EOS) modes for use with ibeos */

enum eos_flags
{
	EOS_MASK = 0x1c00,
	REOS = 0x0400,		/* Terminate reads on EOS	*/
	XEOS = 0x800,	/* assert EOI when EOS char is sent */
	BIN = 0x1000		/* Do 8-bit compare on EOS	*/
};

/* GPIB Bus Control Lines bit vector */
enum bus_control_line
{
	ValidDAV = 0x01,
	ValidNDAC = 0x02,
	ValidNRFD = 0x04,
	ValidIFC = 0x08,
	ValidREN = 0x10,
	ValidSRQ = 0x20,
	ValidATN = 0x40,
	ValidEOI = 0x80,
	ValidALL = 0xff,
	BusDAV = 0x0100,		/* DAV  line status bit */
	BusNDAC = 0x0200,		/* NDAC line status bit */
	BusNRFD = 0x0400,		/* NRFD line status bit */
	BusIFC = 0x0800,		/* IFC  line status bit */
	BusREN = 0x1000,		/* REN  line status bit */
	BusSRQ = 0x2000,		/* SRQ  line status bit */
	BusATN = 0x4000,		/* ATN  line status bit */
	BusEOI = 0x8000		/* EOI  line status bit */
};


static const int gpib_addr_max = 30;	/* max address for primary/secondary gpib addresses */

enum ibask_option
{
	IbaPAD = 0x1,
	IbaSAD = 0x2,
	IbaTMO = 0x3,
	IbaEOT = 0x4,
	IbaPPC = 0x5,	/* board only */
	IbaREADDR = 0x6,	/* device only */
	IbaAUTOPOLL = 0x7,	/* board only */
	IbaCICPROT = 0x8,	/* board only */
	IbaIRQ = 0x9,	/* board only */
	IbaSC = 0xa,	/* board only */
	IbaSRE = 0xb,	/* board only */
	IbaEOSrd = 0xc,
	IbaEOSwrt = 0xd,
	IbaEOScmp = 0xe,
	IbaEOSchar = 0xf,
	IbaPP2 = 0x10,	/* board only */
	IbaTIMING = 0x11,	/* board only */
	IbaDMA = 0x12,	/* board only */
	IbaReadAdjust = 0x13,
	IbaWriteAdjust = 0x14,
	IbaEventQueue = 0x15,	/* board only */
	IbaSPollBit = 0x16,	/* board only */
	IbaSpollBit = 0x16,	/* board only */
	IbaSendLLO = 0x17,	/* board only */
	IbaSPollTime = 0x18,	/* device only */
	IbaPPollTime = 0x19,	/* board only */
	IbaEndBitIsNormal = 0x1a,
	IbaUnAddr = 0x1b,	/* device only */
	IbaHSCableLength = 0x1f,	/* board only */
	IbaIst = 0x20,	/* board only */
	IbaRsv = 0x21,	/* board only */
	IbaBNA = 0x200,	/* device only */
    IbaBaseAddr	= 0x201	/* GPIB board's base I/O address.*/
};

enum ibconfig_option
{
	IbcPAD = 0x1,
	IbcSAD = 0x2,
	IbcTMO = 0x3,
	IbcEOT = 0x4,
	IbcPPC = 0x5,	/* board only */
	IbcREADDR = 0x6,	/* device only */
	IbcAUTOPOLL = 0x7,	/* board only */
	IbcCICPROT = 0x8,	/* board only */
	IbcIRQ = 0x9,	/* board only */
	IbcSC = 0xa,	/* board only */
	IbcSRE = 0xb,	/* board only */
	IbcEOSrd = 0xc,
	IbcEOSwrt = 0xd,
	IbcEOScmp = 0xe,
	IbcEOSchar = 0xf,
	IbcPP2 = 0x10,	/* board only */
	IbcTIMING = 0x11,	/* board only */
	IbcDMA = 0x12,	/* board only */
	IbcReadAdjust = 0x13,
	IbcWriteAdjust = 0x14,
	IbcEventQueue = 0x15,	/* board only */
	IbcSPollBit = 0x16,	/* board only */
	IbcSpollBit = 0x16,	/* board only */
	IbcSendLLO = 0x17,	/* board only */
	IbcSPollTime = 0x18,	/* device only */
	IbcPPollTime = 0x19,	/* board only */
	IbcEndBitIsNormal = 0x1a,
	IbcUnAddr = 0x1b,	/* device only */
	IbcHSCableLength = 0x1f,	/* board only */
	IbcIst = 0x20,	/* board only */
	IbcRsv = 0x21,	/* board only */
	IbcLON = 0x22,
	IbcBNA = 0x200	/* device only */
};

enum t1_delays
{
	T1_DELAY_2000ns = 1,
	T1_DELAY_500ns = 2,
	T1_DELAY_350ns = 3
};

#endif	/* _GPIB_USER_H */
