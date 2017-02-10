#ifndef FormInitSetUH
#define FormInitSetUH

#include <Classes.hpp>
#include <Controls.hpp>
#include <StdCtrls.hpp>
#include <Forms.hpp>
#include <Buttons.hpp>
#include <Dialogs.hpp>
#include <ExtCtrls.hpp>

#include "Global.h"
#include <ComCtrls.hpp>

class TFormInitSet : public TForm
{
__published:	// IDE-managed Components
	// 目标值设置
    TGroupBox *m_pGbTargetValue;
    TLabeledEdit *m_pLedtLowTemp;
    TLabeledEdit *m_pLedtNorTemp;
    TLabeledEdit *m_pLedtHighTemp;
    TLabeledEdit *m_pLedtLTDelay;
    TLabeledEdit *m_pLedtNTDelay;
	TLabeledEdit *m_pLedtHTDelay;
    TLabeledEdit *m_pLedtSampleInterval;
    TLabel *m_pLabelTempSeq;
    TListBox *m_pLstbTempSeq;
    TLabeledEdit *m_pLedtLowVolt;
    TLabeledEdit *m_pLedtNorVolt;
    TLabeledEdit *m_pLedtHighVolt;
	TButton *m_pBtnUp;
	TButton *m_pBtnDown;
    TButton *m_pBtnDefault;
	TStatusBar *m_pStatusBar;

    // ACU信息
    TGroupBox *m_pGbACUInfo;
    TLabeledEdit *m_pLedtTestName;
    TLabeledEdit *m_pLedtModelName;
    TLabeledEdit *m_pLedtRemark;

    // ACU ID号对应
    TGroupBox *m_pGbACUID;
    TLabeledEdit *m_pLedtID1;
    TLabeledEdit *m_pLedtID2;
    TLabeledEdit *m_pLedtID3;
    TLabeledEdit *m_pLedtID4;
    TLabeledEdit *m_pLedtID5;
    TLabeledEdit *m_pLedtID6;

    // 测试条件设置
    TGroupBox *m_pGbTestCond;
    TCheckBox *m_pChbLTLV;
    TCheckBox *m_pChbLTNV;
    TCheckBox *m_pChbLTHV;
    TCheckBox *m_pChbNTLV;
    TCheckBox *m_pChbNTNV;
    TCheckBox *m_pChbNTHV;
    TCheckBox *m_pChbHTLV;
    TCheckBox *m_pChbHTNV;
    TCheckBox *m_pChbHTHV;
    TButton *m_pBtnConSelAll;
    TButton *m_pBtnConUnSelAll;

    // ACU选择
    TGroupBox *m_pGbACUSel;
    TCheckBox *m_pChbACU1;
    TCheckBox *m_pChbACU2;
    TCheckBox *m_pChbACU3;
    TCheckBox *m_pChbACU4;
    TCheckBox *m_pChbACU5;
    TCheckBox *m_pChbACU6;
    TButton *m_pBtnACUSelAll;
    TButton *m_pBtnACUUnSelAll;

    // Item选择
    TLabel *m_pLabelOriginalSteps;
    TSpeedButton *m_pSpBtnOMRight;
    TSpeedButton *m_pSpBtnOMRightAll;
    TSpeedButton *m_pSpBtnOMLeft;
    TSpeedButton *m_pSpBtnOMLeftAll;
    TListBox *m_pLstbOri;
    TLabel *m_pLabelMidSelSteps;
	TListBox *m_pLstbMid;
    TButton *m_pBtnMSRight;
    TLabel *m_pLabelSelSteps;
	TListBox *m_pLstbSel;
    TButton *m_pBtnDel;

    // CrashOut输出模式
    TRadioGroup *m_pRgCouttype;
    // 温箱控制模式
    TCheckBox *m_pChbChamAuto;

    // 保存控制
    TButton *m_pBtnOK;
    TButton *m_pBtnCancel;
    TButton *m_pBtnSave;
    TButton *m_pBtnLoad;
    TSaveDialog *m_pSaveDlg;
    TOpenDialog *m_pOpenDlg;

    void __fastcall FormCreate(TObject *Sender);
    void __fastcall OnLedtLowTempChange(TObject *Sender);
    void __fastcall OnLedtLowTempExit(TObject *Sender);
    void __fastcall OnLedtLowVoltChange(TObject *Sender);
    void __fastcall OnLedtLowVoltExit(TObject *Sender);
    void __fastcall OnLedtNorTempChange(TObject *Sender);
    void __fastcall OnLedtNorTempExit(TObject *Sender);
    void __fastcall OnLedtNorVoltChange(TObject *Sender);
    void __fastcall OnLedtNorVoltExit(TObject *Sender);
    void __fastcall OnLedtSampleIntervalChange(TObject *Sender);
    void __fastcall OnLedtSampleIntervalExit(TObject *Sender);
    void __fastcall OnLedtHighTempChange(TObject *Sender);
    void __fastcall OnLedtHighTempExit(TObject *Sender);
    void __fastcall OnLedtHighVoltChange(TObject *Sender);
    void __fastcall OnLedtHighVoltExit(TObject *Sender);
    void __fastcall OnBtnDefaultClick(TObject *Sender);
    void __fastcall OnLedtTestNameChange(TObject *Sender);
    void __fastcall OnLedtModelNameChange(TObject *Sender);
    void __fastcall OnLedtRemarkChange(TObject *Sender);
    void __fastcall OnLedtID1Change(TObject *Sender);
    void __fastcall OnLedtID2Change(TObject *Sender);
    void __fastcall OnLedtID3Change(TObject *Sender);
    void __fastcall OnLedtID4Change(TObject *Sender);
    void __fastcall OnLedtID5Change(TObject *Sender);
    void __fastcall OnLedtID6Change(TObject *Sender);
    void __fastcall OnChbLTLVClick(TObject *Sender);
    void __fastcall OnChbLTNVClick(TObject *Sender);
    void __fastcall OnChbLTHVClick(TObject *Sender);
    void __fastcall OnChbNTLVClick(TObject *Sender);
    void __fastcall OnChbNTNVClick(TObject *Sender);
    void __fastcall OnChbNTHVClick(TObject *Sender);
    void __fastcall OnChbHTLVClick(TObject *Sender);
    void __fastcall OnChbHTNVClick(TObject *Sender);
    void __fastcall OnChbHTHVClick(TObject *Sender);
    void __fastcall OnLedtLTDelayChange(TObject *Sender);
    void __fastcall OnLedtLTDelayExit(TObject *Sender);
    void __fastcall OnLedtNTDelayChange(TObject *Sender);
    void __fastcall OnLedtNTDelayExit(TObject *Sender);
    void __fastcall OnLedtHTDelayChange(TObject *Sender);
    void __fastcall OnLedtHTDelayExit(TObject *Sender);
    void __fastcall OnBtnConSelAllClick(TObject *Sender);
    void __fastcall OnBtnConUnSelAllClick(TObject *Sender);
    void __fastcall OnChbACU1Click(TObject *Sender);
    void __fastcall OnChbACU2Click(TObject *Sender);
    void __fastcall OnChbACU3Click(TObject *Sender);
    void __fastcall OnChbACU4Click(TObject *Sender);
    void __fastcall OnChbACU5Click(TObject *Sender);
    void __fastcall OnChbACU6Click(TObject *Sender);
    void __fastcall OnChbACU7Click(TObject *Sender);
    void __fastcall OnChbACU8Click(TObject *Sender);
    void __fastcall OnBtnACUSelAllClick(TObject *Sender);
    void __fastcall OnBtnACUUnSelAllClick(TObject *Sender);
    void __fastcall OnSpBtnOMRightClick(TObject *Sender);
    void __fastcall OnSpBtnOMRightAllClick(TObject *Sender);
    void __fastcall OnSpBtnOMLeftClick(TObject *Sender);
    void __fastcall OnSpBtnOMLeftAllClick(TObject *Sender);
    void __fastcall OnBtnMSRightClick(TObject *Sender);
    void __fastcall OnBtnDelClick(TObject *Sender);
    void __fastcall OnRgCouttypeClick(TObject *Sender);
    void __fastcall OnChbChamAutoClick(TObject *Sender);
    void __fastcall OnBtnOKClick(TObject *Sender);
    void __fastcall OnBtnCancelClick(TObject *Sender);
    void __fastcall OnBtnSaveClick(TObject *Sender);
    void __fastcall OnBtnLoadClick(TObject *Sender);
	void __fastcall OnBtnUpClick(TObject *Sender);
	void __fastcall OnBtnDownClick(TObject *Sender);
private:
	// 开关量 true-已改变 false-未变
	bool m_bLedt;		// LabeledEdit：温度、电压、间隔、测试信息、EcuId、TDelay--false
	bool m_bItem;		// 步骤选择框改变
	bool m_bCond; 		// 条件选择框TV组合、ComChamber、Crash out
	int m_iItemCurr[MES_ITEM_REAL];	// User declarations
    void __fastcall EnableACUID(bool bEnable);	// U:01 ACU板卡统一选择或取消
	void __fastcall EnableCond(bool bEnable);  	// U:02 TV条件统一选择或取消
    void __fastcall LoadLatest();				// U:03 初始化时导入配置文件到界面和g_STestInfo
	void __fastcall SaveLatest();				// U:04 仅仅保存到配置文件
	void __fastcall GetTargetValue();			// U:05 文件不存在或第一次运行程序：温度、电压、温度延迟、温度操作顺序、温度记录时间间隔
	void __fastcall GetLineFromFile(FILE * pFile, String & strTemp, int & iEqual);	// U:06 读文件一行
	void __fastcall SetItem(TListBox * pList, int iIndex);							// U:07 TListBox的移动
	int  __fastcall GetFirstSelInList(TCustomListBox * pList);						// U:08 TListBox的移动
	void __fastcall MoveInList(TCustomListBox * pList, TStrings * pItems);			// U:09 TListBox的移动
    void __fastcall ResetFormAndStruct();			// U:10 清空Form和g_STestInfo
    void __fastcall StructFromFile(String strPath); // U:11 文件到全局变量g_STestInfo
	void __fastcall RefreshFormFromStruct();		// U:12 全局变量g_STestInfo到控件
   	void __fastcall RefreshStructFromForm();		// U:13 控件到全局变量g_STestInfo
    void __fastcall SetItemCurrInfo();				// U:14 测试项m_pLstbSel到m_iItemCurr
public:
	__fastcall TFormInitSet(TComponent* Owner);
};

extern PACKAGE TFormInitSet *FormInitSet;

#endif
