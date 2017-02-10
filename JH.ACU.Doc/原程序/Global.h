#ifndef GlobalH
#define GlobalH

#pragma once

#define NOWIN32_LEAN_AND_MEAN
#include <shlobj.hpp>
#include <vcl.h>
#include <IniFiles.hpp>
#include <stdio.h>

#define MES_ADDRESS 		0x0602      // Flash��ʼ��ַ
#define MES_BYTES 			134			// ��ȡ���ֽ���

// �忨�ͼ̵���
#define MES_SUBBOARD_NUM  		6		// �Ӱ���
#define MES_RELAYGROUP      	9 		// �̵�������
#define MES_RELAYNUM    		8		// ÿ��̵�������

// DMM������ѹ������������
#define MES_COEFFICIENT 		0.618	// ���ַ�
#define MES_MAX_WAITTIME		15000 	// ms

// PcPower������
#define MES_BUFFER_LENGTH 		80

// FC������������
#define MES_RECURSIVE_LOW       0.01        
#define MES_RECURSIVE_HIGH      0.001
#define MES_SPEC_BIG            3000.0

///////////////////////////////////////////////////////////////////////////////
// �����е��ظ�����
extern unsigned int 	g_iItemRepeat;		// �������Բ���ʧ��ʱ�ظ���������̵����͹���������ɣ���ҪӲ������
extern unsigned int		g_iFindRepeat;		// Klineͨ��ʧ��ʱ���ظ���������Ҫ���������̵�������ǰ�̵����͵�Դ��Ӧ��鲢��Ӧ

///////////////////////////////////////////////////////////////////////////////
// ������Ϣ��ʾ on testing / Charmber Process
extern int  	g_iViewChamber; 	// Charmber Process
extern int  	g_iViewTest; 		// on testing
extern bool 	g_bViewChamber;		// Chamber Process
extern bool 	g_bViewTest;		// on testing
extern String	g_strCurrSet;		// current setting

///////////////////////////////////////////////////////////////////////////////
// �¶�OnTimer ��ʾ��д��TempSheet
extern bool 	g_bChamberIdle;	   	// �����Ƿ����
extern bool 	g_bTempReal;		// �Ƿ���ȡ�¶�
extern int  	g_iTempCount;		// ÿ10S��ȡʵ���¶�
extern int  	g_iTempInterval;	// ����д���ļ�Interval
extern int		g_iTempRow;			// д��TempSheet��������

///////////////////////////////////////////////////////////////////////////////
// ��ѹOnTimer ��ʾ��д��Volt.dat
extern int  	g_iVoltCount;		// ÿ10S��ȡʵ�ʵ�ѹ
extern bool 	g_bVoltReal;		// �Ƿ���ȡ��ѹ

///////////////////////////////////////////////////////////////////////////////
// ״̬���������Ϣ��ʾ����
extern bool 		g_bTimeStart;	// on testing �Ƿ�������ʼ
extern unsigned int g_iSecond;		// �����ۻ�ʱ��
extern String 	g_strMainTip;		// ��ʾ��Ϣ������
extern String 	g_strSubTip;		// ��ʾ��Ϣ��С����

///////////////////////////////////////////////////////////////////////////////
// ���ߣ��¶�����
extern bool 	g_bTempRenew;		// ���������¶�
extern float 	g_fTimeTotal;		// ÿ�����������¶ȹ�������ʱ�� minute
extern float 	g_fTimeRest;		// ÿ�����������¶�����ʱ��
extern float 	g_fTempInit;	 	// ÿ���������������¶�ʱ�ĳ�ʼ�¶�
extern float 	g_fTempCurveX;		// ʵ���¶����ߺ����� m_pTempCurveReal
extern float	g_fTempCurveY;		// ��ǰ������¶ȣ�ʵ���¶�����������
///////////////////////////////////////////////////////////////////////////////
// ���ߣ�����������
extern int		g_iCurveMode;		// 0-����ģʽ 1-HoldTimeģʽ 2-Crashģʽ
extern double 	g_dCurveCurrX;   	// ������������ʾX
extern float 	g_fCurveCurrY;		// ������������ʾY
extern double 	g_dCurveLength;		// ������������ʾ���ȣ�	�������Ӧ0-19.0 1-999.0 2-999.0
extern double 	g_daCurveY[1000];   // ��1��2ʱ������ֵ����
extern String	g_strCurveUnit;		// ������ʾ��ߵ�λ,	�������Ӧ0-ohm 1-v 2-v

///////////////////////////////////////////////////////////////////////////////
// ������Ϣ����
extern bool 	g_bProPos;			// �Ƿ񵥲�
extern int 		g_iProPos;			// ���ǵ���ʱǰ��������

///////////////////////////////////////////////////////////////////////////////
// ������
// �¶�3����ѹ3������-ģ����-����3��ACU6��ʱ����1��ʱ���ӳ�3����19��
#define MES_ITEM_REAL			74		// ���в��Ե�������
#define MES_ITEM_RET			63		// ��Ҫ��������
typedef struct Ex_STestInfo
{
        String m_strTemp[3];			// ���ֵ���ֵ ����
        String m_strVolt[3];            // ���ֵ�ѹֵ ����
        String m_strInterval;           // ʱ������second������
        String m_strDelay[3];           // ʱ���ӳ�LT��NT��HT��minute������
        String m_strInfo[3];            // ��������-ģ����-����
        String m_strEcuId[6];           // ECU��ID��
        int m_iTVCond[9]; 	   			// �����������¶ȵ�ѹ�ľ���������
        int m_iItem[MES_ITEM_REAL];		// ���Բ��裬ÿһ�������忨��Ϣ
        int m_iSelACU;    				// ���б�ѡACU�Ӱ�Ż���
        int m_iDaqType;   				// DAQ������� 	0 advanced   	1 conventional
        int m_iChamType;  				// �����ʶ    	0 ���������� 	1 ��������
        int m_iTempSeq[3];				// �¶Ȳ���˳�� 0 ���� 			1 ���� 			2 ����
        float m_fPrecision[4];			// ��Ӧtest.ini�ļ��еĲ��Ծ���
        float m_fAmendRes[8][4];    	// �Ӵ��ߵȵ��裬������������
}STestInfo;
extern STestInfo g_STestInfo;	 	// �����е�ǰ��Ϣ����
extern int g_iTempPhase;			// �����¶Ƚ׶� based 0  0-LT 1-NT 2-HT
extern int g_iVoltPhase;			// ���õ�ѹ�׶� based 0  0-LV 1-NT 2-HV
extern float g_fTempTarget;			// ��ǰĿ���¶ȣ��������Ӧ
extern float g_fVoltTarget;			// ��ǰĿ���ѹ���������Ӧ
extern float g_fVoltInit;	 		// ÿ���������õ�Դ��ѹʱ�ĳ�ʼ��ѹ
extern float g_fVoltCurr;    		// ʵʱ��Դ��ѹ
extern int 	g_iCRC;					// ��ĳһ��TempVolt�µ�ĳһ���忨�µ�FM��������
extern int 	g_iTVCond;			   	// ���ڲ��Ե�TV�������������������	based 0
extern int  g_iCurrBoard;			// ���ڲ��Ե�ECU�������Ӱ� based 0
extern int  g_iCurrItem;			// ���ڲ��Եĵ�ǰItem���� based 0
extern bool	g_bOpState;				// ��������״̬
extern int  g_iOpMode;              // 0-�Զ���1-�ֹ�����ģʽ
extern String	g_strMark;      	// ������ʾ��Ϣ
extern String 	g_strItemValue;		// ��������ɺ������ֵ
void 	SetGlobalStateForNewTest();	// ����ȫ�ֱ���Ϊ���µĲ���



///////////////////////////////////////////////////////////////////////////////
// �ۺ�
extern String 	g_strAppPath;		// ����·��
void 	GetAppPath(void);			// ��ȡ��������·��
void 	G_SDelay(DWORD dwMilliSecond);	// �����ȴ�
void 	G_MDelay(DWORD dwMilliSecond);	// �첽�ȴ�
String 	DToH2(int iInput);			// ��1-15���ֱ��16����
String 	DToH1(int iInput);			// ���0xFFģʽ��ֻ��ʾ16���ƺ�2λ
String 	IntToStrAdjust(int iInput);	// ��1-9����2λ���������9ֱ�����
String 	IntToStrTime(int iSecond);	// ʱ���֣�����ʾ
bool 	GetStrType(String strTemp); // �ж��Ƿ��Ǵ������ַ������Ƿ����棬���򷵻ؼ�


///////////////////////////////////////////////////////////////////////////////
// �豸������-���䡢Kline��D2KDASK-�ɼ�����
//		 GPIB-�ӿڡ���Դ��״̬�����ã���DMM��RES1��RES2��
//		 ״̬-����״̬
extern bool g_bAllDevice;	// Ӳ���豸�Ƿ�����
bool 	OpenDevice(); 		// �������豸
bool    CloseDevice(); 		// �ر������豸


///////////////////////////////////////////////////////////////////////////////
// Kline��������
extern FILE     *g_pFileComKline;
extern FILE	    *g_pFileErrReal;
extern FILE		*g_pFileVolt;
bool    OpenLogFile(int iFileType, String strPath);	// ��Log�ļ�
bool    WriteLogFile(FILE *pFile, String str);		// д������
bool    CloseLogFile(FILE *pFile);                 	// �ر�Log�ļ�


///////////////////////////////////////////////////////////////////////////////
// ������
// �淶�ʹ���
#define MES_ERRINFO_LENGTH		65		// ����������󳤶� ��󳤶�59 + 2 �Ϳ���
#define MES_SPEC_LENGTH 		85		// ���Թ淶����󳤶� ��󳤶�79 + 2 �Ϳ���
// ����Fault_Code.txt�и�������������
#define MES_ERRORS_NUM 			220		// �ܵĴ����������
// lISTBOX
#define MES_LISTLINE_HEIGHT 	14    	// �и�
#define MES_LISTLINE_DISTANCE 	2      	// �м��
extern int	g_iCurrFaultCode;			// ��ǰ�Ĵ�����
extern char	g_caSpec[MES_ITEM_REAL][MES_SPEC_LENGTH];			// ��ȡ�󱣳ֲ��� SPEC.TXT
extern char g_caErrInfo[MES_ERRORS_NUM][MES_ERRINFO_LENGTH];	// ��ȡ�󱣳ֲ��� FAULTCODE.TXT
void 	SetSPECInfo();	// ��ȡSPEC.TXT������g_caSpec
String 	GetSPECInfo(int iFullItem, int iType);	// iFullItem based 0�� iType based 0
void 	ReadFaultInfo();// ��ȡFAULTCODE.TXT������g_caSpec
bool	GetFaultInfo(int iFaultCode, String & strCode, String & strRemark);

#endif
