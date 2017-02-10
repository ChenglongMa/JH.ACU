#ifndef FormMainUH
#define FormMainUH

#include <Classes.hpp>
#include <Controls.hpp>
#include <StdCtrls.hpp>
#include <Forms.hpp>
#include <AxCtrls.hpp>
#include <Buttons.hpp>
#include <Chart.hpp>
#include <ComCtrls.hpp>
#include <Dialogs.hpp>
#include <ExtCtrls.hpp>
#include <Graphics.hpp>
#include <jpeg.hpp>
#include <OleCtrls.hpp>
#include <Series.hpp>
#include <TeEngine.hpp>
#include <TeeProcs.hpp>
#include <VCF1.hpp>
#include "Global.h"

// Sheet ���򣺶��ǵ�ǰλ��
#define MES_SHEET_TOTAL		1		// Total Sheet
#define MES_SHEET_TV		2		// TV Sheet Start
#define MES_SHEET_ERRLOG	11   	// Errorlog Sheet
#define MES_SHEET_FM		12		// FM Sheet
#define MES_SHEET_TEMP		13 		// TempRecord Sheet
// Total Sheet
#define MES_T_ITEMCOL	1
#define MES_T_SPECCOL	2
#define MES_T_LAVGCOL   6
#define MES_T_MAVGCOL	10
#define MES_T_HAVGCOL	14
#define MES_T_SAVGCOL	15
// XXTemp-XXVolt Sheet ����ǰһ��
#define MES_TV_ITEMCOL	1
#define MES_TV_BOARDCOL	3
#define MES_TV_MINCOL	9
#define MES_TV_MAXCOL	10
#define MES_TV_AVGCOL	11
#define MES_TV_JUDGECOL	12
#define MES_TV_ERRCOL	13

#define MES_ITEM_CRASH	73

class TFormMain : public TForm
{
__published:	// IDE-managed Components
	// Top����
	TLabel *m_pLabelTitle;
	TImage *m_pImgCorp;
	TListBox *m_pLstbError;

    // Test Mode & Control
	TPanel *m_pPanelMode;
	TLabel *m_pLabelMode;
	TLabel *m_pLabelAutoRun;
	TLabel *m_pLabelManualRun;
	TShape *m_pShapeAutoRun;
	TShape *m_pShapeManualRun;
    TOpenDialog *m_pDlgOpen;
	TSaveDialog *m_pDlgSave;
	TTimer *m_pTimer;
    TButton *m_pBtnInit;
	TButton *m_pBtnRelay;
	TButton *m_pBtnRun;
	TButton *m_pBtnQuit;
	TButton *m_pBtnManual;

    // ��������
	TPanel *m_pPanelCondition;
	TLabel *m_pLabelCondition;

    TLabel *m_pLabelTempTarget;
	TPanel *m_pPanelTempTarget;
    TLabel *m_pLabelVoltTarget;
	TPanel *m_pPanelVoltTarget;
	TLabel *m_pLabelTempCurr;
	TPanel *m_pPanelTempCurr;
    TLabel *m_pLabelVoltCurr;
	TPanel *m_pPanelVoltCurr;
	TPanel *m_pPanelACU;
	TLabel *m_pLabelACU;

	TShape *m_pShapeTempLow;
    TShape *m_pShapeTempNor;
    TShape *m_pShapeTempHigh;
    TShape *m_pShapeVoltLow;
	TShape *m_pShapeVoltNor;
	TShape *m_pShapeVoltHigh;
	TLabel *m_pLabelHigh;
	TLabel *m_pLabelNor;
	TLabel *m_pLabelLow;
    TLabel *m_pLabelBattVolt;
	TLabel *m_pLabelCharmTemp;

    TRadioGroup *m_pRgCouttype;
	TCheckBox *m_pChbChamAuto;

    // ���߲���
	TPanel *m_pPanelCurve;
	TChart *m_pChartWave;
	TFastLineSeries *m_pTestCurve;
	TChart *m_pChartTemp;
	TFastLineSeries *m_pTempCurveTheory;
	TFastLineSeries *m_pTempCurveReal;

    // ���Բ���
	TPanel *m_pPanelTestItems;
	TLabel *m_pLabelTestItemInfo;
	TF1Book *m_pF1BookTestItem;		// ��ʾ
	TF1Book *m_pF1BookACUInfo;		// ��ʾ
    TLabel *m_pLabelReport;
	TLabel *m_pLabelTVCond;
	TLabel *m_pLabelCond;			// ��ʾ
	TLabel *m_pLabelSpark;			// ��ʾ
    TProgressBar *m_pProBaMain;		// ��ʾ
	TLabel *m_pLabelProgress;		// ��ʾ

    // ���Ա���
	TPanel *m_pPanelTestReport;
	TF1Book *m_pF1BookTestReport;
    TButton *m_pBtnLoad;
	TButton *m_pBtnSave;
    TLabel *m_pLCurrSet;

	TStatusBar *m_pStatusBar;

	void __fastcall FormCreate(TObject *Sender);		// S:01
	void __fastcall FormDestroy(TObject *Sender);		// S:02
	void __fastcall FormClose(TObject *Sender, TCloseAction &Action);	// S:03
	void __fastcall OnTimer(TObject *Sender);			// S:04
	void __fastcall OnShapeAutoRunMouseDown(TObject *Sender, TMouseButton Button, TShiftState Shift, int X, int Y);		// S:05
	void __fastcall OnShapeManualRunMouseDown(TObject *Sender, TMouseButton Button, TShiftState Shift, int X, int Y);	// S:06
	void __fastcall OnBtnInitClick(TObject *Sender);	// S:07
	void __fastcall OnBtnRelayClick(TObject *Sender);	// S:08
	void __fastcall OnBtnRunClick(TObject *Sender);		// S:09
	void __fastcall OnBtnQuitClick(TObject *Sender);	// S:10
	void __fastcall OnBtnManualClick(TObject *Sender);	// S:11
	void __fastcall OnShapeLowTempMouseDown(TObject *Sender, TMouseButton Button, TShiftState Shift, int X, int Y);		// S:12
	void __fastcall OnShapeNorTempMouseDown(TObject *Sender, TMouseButton Button, TShiftState Shift, int X, int Y);		// S:13
	void __fastcall OnShapeHighTempMouseDown(TObject *Sender, TMouseButton Button, TShiftState Shift, int X, int Y);	// S:14
    void __fastcall OnShapeLowVoltMouseDown(TObject *Sender, TMouseButton Button, TShiftState Shift, int X, int Y);		// S:15
	void __fastcall OnShapeNorVoltMouseDown(TObject *Sender, TMouseButton Button, TShiftState Shift, int X, int Y);		// S:16
	void __fastcall OnShapeHighVoltMouseDown(TObject *Sender, TMouseButton Button, TShiftState Shift, int X, int Y);	// S:17
	void __fastcall OnPanelACUNumClick(TObject *Sender);// S:18
	void __fastcall OnBtnSaveClick(TObject *Sender);	// S:19
	void __fastcall OnBtnLoadClick(TObject *Sender);	// S:20
	void __fastcall OnF1BookTestItemClick(TObject *Sender, int nRow, int nCol);// S:22
	void __fastcall OnChartTempClick(TObject *Sender);	// S:23
	void __fastcall OnChartWaveClick(TObject *Sender);	// S:24
private:
	// User declarations
	int m_iModeRun;
	bool m_bThread;
    Variant		m_varApp;
	Variant		m_varBook;
	Variant		m_varSheet;
	static int m_siRowErrorSheet;
	static int m_siRowTempSheet;
	void __fastcall OnThreadTerminate(TObject * pObject);	// S:25 �߳���ֹ�ĺ��ڴ���

public:
    void __fastcall SetStateCommonality(bool bEnable);		// S:26 ���Զ�������ؿؼ�
    void __fastcall SetStateAuto(bool bEnable);
	void __fastcall SetStateManual(bool bEnable);	// S:27 ���ֹ�������ؿؼ�
	void __fastcall SetContent(bool bReset);		// S:28 ���ֹ�������ؿؼ�����
	void __fastcall SaveToExcel(String strPath);	// S:29 ���水ť�Ӻ���
	void __fastcall OpenFromExcel(String strPath);	// S:30 ���밴ť�Ӻ���
	bool __fastcall RefreshGStruct();				// S:31 �ֹ�ģʽ�¸���ȫ�ֿ��Ʊ���g_STestInfo
    void __fastcall ResetGVariable();				// S:32 ����ȫ�ֱ�����OnTimer������
    void __fastcall ViewStatusBar();				// S:33 ����״̬��
    void __fastcall ViewLabelSpark();				// S:34 ����LabelSpark
    void __fastcall RefreshTempData();				// S:35 �����¶����ݣ��豣��
    void __fastcall RefreshVoltData();				// S:36 ���µ�ѹ���ݣ��豣��
public:
	int m_iErrLstCount;
	int m_iErrItemCount;
	int m_iaErrItem[MES_ERRORS_NUM];
    bool m_bTempBoard;
    bool m_bWrite;
    bool m_bTVState;	// Start = true, End = false; ����д��TestInfo��TVSheet
    String m_strInfo;	// Test Information
    int  m_iaItem[MES_ITEM_REAL];
    int	 m_iaFMRow[9][MES_SUBBOARD_NUM];	// TempVolt�������忨��
    String m_strItemError;		// User declarations
	__fastcall TFormMain(TComponent* Owner);// S:00


    // ���ڱ仯ʱ�������
	virtual void __fastcall WndProc(Messages::TMessage & Message);	// U:01 ����ؼ����ݷֱ��ʵ���


    // ���沿�֣������������ߡ�ECU�����󡢡�OnTimer�����¶ȡ���ѹ��״̬��������״̬
    int  __fastcall AdjustData(float fData, int * piValue);			// U:02 ������Shape����ʾ��ֵ
    void __fastcall ViewTempPhase();			// U:39 ���ݵ�ǰ����TV���������¶Ƚ׶� LT NT HT OK
    void __fastcall ViewVoltPhase();			// U:40 ���ݵ�ǰ����TV���������¶Ƚ׶� LV NV HV OK
    void __fastcall ViewTempTarget();			// U:03 based 0 OK
	void __fastcall ViewVoltTarget();			// U:04 based 0	OK
	void __fastcall ViewTempCurr();				// U:05 ��ʾ��ǰ�¶ȵ�Shape��ʵʱ�¶����� OK
	void __fastcall ViewVoltCurr();				// U:06 ��ʾ��ǰ��ѹ��Shape��ʵʱ��¼��ѹ OK
    void __fastcall ViewBoardNum();				// U:07 Board��ʾ OK
	void __fastcall ViewProgressInfo();			// U:08 ��������ʾ OK
    void __fastcall ViewWaveTempTheory();		// U:41 ������ʾ-�����¶����� OK
	void __fastcall ViewWave();					// U:09 ������ʾ-���Խ������ OK
	void __fastcall ClearWave();				// U:10 ������ʾ-���Խ������ OK
    void __fastcall ViewLabelCurrSet();
    void __fastcall ViewTVCondInLabel();		// U:42 ��LabelCond����ʾ��ǰTempVoltģʽ OK
	void __fastcall AddErrorByCode();			// U:11 ������
	void __fastcall AddErrorByMark(); 			// U:12 ������
    void __fastcall SaveErrorToFile();			// U:13 ���������Ϣ���ļ� OK
    void __fastcall ResetErrorInfo(int iState);	// U:14 ���´��������Ϣ������������ɡ�ȫ��������ɣ���ǿ���˳���


    // �������Table�ı仯��ʵ�ַ���FormMainTable.cpp�ļ���
    ///////////////////////////////////////////////////////////////////////////
    // InfoBook
    void __fastcall ResetBookInfo();			// U:15 ���InfoBook���� OK
    void __fastcall InitBookInfo();				// U:15 ���InfoBook���� OK
    ///////////////////////////////////////////////////////////////////////////
    // ItemBook
    void __fastcall ResetBookItem();			// U:15 ����ItemBook��ʽ OK
    void __fastcall ChangeItem();				// U:16 �ı������ OK
    ///////////////////////////////////////////////////////////////////////////
    // ReportBook
    void __fastcall LoadBookReport();			// U:17 ����ReportBook Empty
    void __fastcall SaveBookReport();			// U:18 ����ReportBook Empty
    // 1�����úͳ�ʼ��
    void __fastcall ResetBookReport();			// U:19 ����ReportBook������հױ�׼��ʽBook OK
    void __fastcall InitBookReport();			// U:20 ��ʼ��ReportBook�������벢���û�����Ϣ OK
    // 2���ܱ�
    void __fastcall ResetSheetTotal();			// U:21 ���TotalSheet��Ϣ
	void __fastcall WriteSheetTotal(); 			// U:22 ���������дTotalSheet
    // 3��ErrorLogSheet��TVSheet�е�ErrCol
    void __fastcall WriteErrorSheet(String strErrMark);	// U:23
	void __fastcall WriteErrorCol();			// U:24
    // 4��TempSheet
    void __fastcall WriteTempSheet();			// U:25 ��дTempSheet OK
    // 5��FMSheet
    void __fastcall InitFMSheet();				// U:26 ����FMSheet��ܣ�m_iTVCond m_iSelACU OK
    void __fastcall WriteSmodeInFMSheet();		// U:27 OK
    void __fastcall WriteFMRInFMSheet();	   	// U:28 OK
    void __fastcall WriteFRCInFMSheet();		// U:29 OK
    // 6��TVSheet
    void __fastcall WriteItemTestData();		// ĳһ��������ɺ����
    void __fastcall WriteCellInTVSheet(int iRow, int iCol, double dValue);	// U:30 
    void __fastcall WriteStatInTVSheet(int iRow);							// U:31
    void __fastcall WriteJudgeInTVSheet(int iRow, int iCol);				// U:32
    void __fastcall WriteTestInfoBaseRowInTVSheet();// U:33 ��дBaseTestInfo���ڸ���TVSheet testName testModel testRemark
    void __fastcall WriteTestInfoRowInTVSheet();	// U:34 ��дTestInfo���ڸ���TVSheet(������ʼ�ͽ���)
    void __fastcall WriteEcuIDInTVSheet();
    void __fastcall WriteSingleEcuIDInTVSheet(int iTVMode, int iBoard, String strEcuId);	// U:35 ��д��ECU�Ķ�Ӧ���Ƶ���ӦTV�ֱ�
	void __fastcall GetRowCountInTVSheet(int iSheet, int & iRow);		// U:36 ��ȡ����
	void __fastcall WriteSPECInTVSheet(); 		// U:37 ��TVSheet��д���ƺͲ��Թ淶(ȫ��)
	void __fastcall WriteSPECSingleInTVSheet(int iItem, int iSheet);	// U:38 �����Ӻ���
    String __fastcall ReadAvg(int iSheet, String strItemName);
    void __fastcall WriteCrash();
};

extern PACKAGE TFormMain *FormMain;

#endif
