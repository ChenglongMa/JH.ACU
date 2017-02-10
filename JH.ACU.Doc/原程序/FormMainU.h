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

// Sheet 规则：都是当前位置
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
// XXTemp-XXVolt Sheet 基于前一个
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
	// Top部分
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

    // 测试条件
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

    // 曲线部分
	TPanel *m_pPanelCurve;
	TChart *m_pChartWave;
	TFastLineSeries *m_pTestCurve;
	TChart *m_pChartTemp;
	TFastLineSeries *m_pTempCurveTheory;
	TFastLineSeries *m_pTempCurveReal;

    // 测试步骤
	TPanel *m_pPanelTestItems;
	TLabel *m_pLabelTestItemInfo;
	TF1Book *m_pF1BookTestItem;		// 显示
	TF1Book *m_pF1BookACUInfo;		// 显示
    TLabel *m_pLabelReport;
	TLabel *m_pLabelTVCond;
	TLabel *m_pLabelCond;			// 显示
	TLabel *m_pLabelSpark;			// 显示
    TProgressBar *m_pProBaMain;		// 显示
	TLabel *m_pLabelProgress;		// 显示

    // 测试报表
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
	void __fastcall OnThreadTerminate(TObject * pObject);	// S:25 线程终止的后期处理

public:
    void __fastcall SetStateCommonality(bool bEnable);		// S:26 与自动测试相关控件
    void __fastcall SetStateAuto(bool bEnable);
	void __fastcall SetStateManual(bool bEnable);	// S:27 与手工测试相关控件
	void __fastcall SetContent(bool bReset);		// S:28 与手工测试相关控件内容
	void __fastcall SaveToExcel(String strPath);	// S:29 保存按钮子函数
	void __fastcall OpenFromExcel(String strPath);	// S:30 导入按钮子函数
	bool __fastcall RefreshGStruct();				// S:31 手工模式下更新全局控制变量g_STestInfo
    void __fastcall ResetGVariable();				// S:32 重置全局变量，OnTimer和流程
    void __fastcall ViewStatusBar();				// S:33 更新状态条
    void __fastcall ViewLabelSpark();				// S:34 更新LabelSpark
    void __fastcall RefreshTempData();				// S:35 更新温度数据，需保存
    void __fastcall RefreshVoltData();				// S:36 更新电压数据，需保存
public:
	int m_iErrLstCount;
	int m_iErrItemCount;
	int m_iaErrItem[MES_ERRORS_NUM];
    bool m_bTempBoard;
    bool m_bWrite;
    bool m_bTVState;	// Start = true, End = false; 控制写入TestInfo到TVSheet
    String m_strInfo;	// Test Information
    int  m_iaItem[MES_ITEM_REAL];
    int	 m_iaFMRow[9][MES_SUBBOARD_NUM];	// TempVolt条件数板卡数
    String m_strItemError;		// User declarations
	__fastcall TFormMain(TComponent* Owner);// S:00


    // 窗口变化时界面调整
	virtual void __fastcall WndProc(Messages::TMessage & Message);	// U:01 界面控件根据分辨率调整


    // 界面部分：进度条、曲线、ECU、错误、【OnTimer处理】温度、电压、状态条、测试状态
    int  __fastcall AdjustData(float fData, int * piValue);			// U:02 调整在Shape中显示的值
    void __fastcall ViewTempPhase();			// U:39 根据当前测试TV条件设置温度阶段 LT NT HT OK
    void __fastcall ViewVoltPhase();			// U:40 根据当前测试TV条件设置温度阶段 LV NV HV OK
    void __fastcall ViewTempTarget();			// U:03 based 0 OK
	void __fastcall ViewVoltTarget();			// U:04 based 0	OK
	void __fastcall ViewTempCurr();				// U:05 显示当前温度到Shape和实时温度曲线 OK
	void __fastcall ViewVoltCurr();				// U:06 显示当前电压到Shape和实时记录电压 OK
    void __fastcall ViewBoardNum();				// U:07 Board显示 OK
	void __fastcall ViewProgressInfo();			// U:08 进度条显示 OK
    void __fastcall ViewWaveTempTheory();		// U:41 曲线显示-理论温度曲线 OK
	void __fastcall ViewWave();					// U:09 曲线显示-测试结果曲线 OK
	void __fastcall ClearWave();				// U:10 曲线显示-测试结果曲线 OK
    void __fastcall ViewLabelCurrSet();
    void __fastcall ViewTVCondInLabel();		// U:42 在LabelCond中显示当前TempVolt模式 OK
	void __fastcall AddErrorByCode();			// U:11 错误处理
	void __fastcall AddErrorByMark(); 			// U:12 错误处理
    void __fastcall SaveErrorToFile();			// U:13 保存错误信息到文件 OK
    void __fastcall ResetErrorInfo(int iState);	// U:14 更新错误变量信息：单步测试完成、全部测试完成（含强行退出）


    // 界面各个Table的变化，实现放在FormMainTable.cpp文件中
    ///////////////////////////////////////////////////////////////////////////
    // InfoBook
    void __fastcall ResetBookInfo();			// U:15 清空InfoBook内容 OK
    void __fastcall InitBookInfo();				// U:15 填充InfoBook内容 OK
    ///////////////////////////////////////////////////////////////////////////
    // ItemBook
    void __fastcall ResetBookItem();			// U:15 重置ItemBook格式 OK
    void __fastcall ChangeItem();				// U:16 改变测试项 OK
    ///////////////////////////////////////////////////////////////////////////
    // ReportBook
    void __fastcall LoadBookReport();			// U:17 导入ReportBook Empty
    void __fastcall SaveBookReport();			// U:18 保存ReportBook Empty
    // 1：重置和初始化
    void __fastcall ResetBookReport();			// U:19 重置ReportBook，载入空白标准格式Book OK
    void __fastcall InitBookReport();			// U:20 初始化ReportBook，即导入并设置基本信息 OK
    // 2：总表
    void __fastcall ResetSheetTotal();			// U:21 清除TotalSheet信息
	void __fastcall WriteSheetTotal(); 			// U:22 操作完成填写TotalSheet
    // 3：ErrorLogSheet及TVSheet中的ErrCol
    void __fastcall WriteErrorSheet(String strErrMark);	// U:23
	void __fastcall WriteErrorCol();			// U:24
    // 4：TempSheet
    void __fastcall WriteTempSheet();			// U:25 填写TempSheet OK
    // 5：FMSheet
    void __fastcall InitFMSheet();				// U:26 创建FMSheet框架：m_iTVCond m_iSelACU OK
    void __fastcall WriteSmodeInFMSheet();		// U:27 OK
    void __fastcall WriteFMRInFMSheet();	   	// U:28 OK
    void __fastcall WriteFRCInFMSheet();		// U:29 OK
    // 6：TVSheet
    void __fastcall WriteItemTestData();		// 某一测试项完成后填表
    void __fastcall WriteCellInTVSheet(int iRow, int iCol, double dValue);	// U:30 
    void __fastcall WriteStatInTVSheet(int iRow);							// U:31
    void __fastcall WriteJudgeInTVSheet(int iRow, int iCol);				// U:32
    void __fastcall WriteTestInfoBaseRowInTVSheet();// U:33 填写BaseTestInfo行在各个TVSheet testName testModel testRemark
    void __fastcall WriteTestInfoRowInTVSheet();	// U:34 填写TestInfo行在各个TVSheet(条件开始和结束)
    void __fastcall WriteEcuIDInTVSheet();
    void __fastcall WriteSingleEcuIDInTVSheet(int iTVMode, int iBoard, String strEcuId);	// U:35 填写各ECU的对应名称到对应TV分表
	void __fastcall GetRowCountInTVSheet(int iSheet, int & iRow);		// U:36 获取行数
	void __fastcall WriteSPECInTVSheet(); 		// U:37 在TVSheet填写名称和测试规范(全部)
	void __fastcall WriteSPECSingleInTVSheet(int iItem, int iSheet);	// U:38 上面子函数
    String __fastcall ReadAvg(int iSheet, String strItemName);
    void __fastcall WriteCrash();
};

extern PACKAGE TFormMain *FormMain;

#endif
