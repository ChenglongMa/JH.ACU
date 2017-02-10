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
	// Ŀ��ֵ����
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

    // ACU��Ϣ
    TGroupBox *m_pGbACUInfo;
    TLabeledEdit *m_pLedtTestName;
    TLabeledEdit *m_pLedtModelName;
    TLabeledEdit *m_pLedtRemark;

    // ACU ID�Ŷ�Ӧ
    TGroupBox *m_pGbACUID;
    TLabeledEdit *m_pLedtID1;
    TLabeledEdit *m_pLedtID2;
    TLabeledEdit *m_pLedtID3;
    TLabeledEdit *m_pLedtID4;
    TLabeledEdit *m_pLedtID5;
    TLabeledEdit *m_pLedtID6;

    // ������������
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

    // ACUѡ��
    TGroupBox *m_pGbACUSel;
    TCheckBox *m_pChbACU1;
    TCheckBox *m_pChbACU2;
    TCheckBox *m_pChbACU3;
    TCheckBox *m_pChbACU4;
    TCheckBox *m_pChbACU5;
    TCheckBox *m_pChbACU6;
    TButton *m_pBtnACUSelAll;
    TButton *m_pBtnACUUnSelAll;

    // Itemѡ��
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

    // CrashOut���ģʽ
    TRadioGroup *m_pRgCouttype;
    // �������ģʽ
    TCheckBox *m_pChbChamAuto;

    // �������
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
	// ������ true-�Ѹı� false-δ��
	bool m_bLedt;		// LabeledEdit���¶ȡ���ѹ�������������Ϣ��EcuId��TDelay--false
	bool m_bItem;		// ����ѡ���ı�
	bool m_bCond; 		// ����ѡ���TV��ϡ�ComChamber��Crash out
	int m_iItemCurr[MES_ITEM_REAL];	// User declarations
    void __fastcall EnableACUID(bool bEnable);	// U:01 ACU�忨ͳһѡ���ȡ��
	void __fastcall EnableCond(bool bEnable);  	// U:02 TV����ͳһѡ���ȡ��
    void __fastcall LoadLatest();				// U:03 ��ʼ��ʱ���������ļ��������g_STestInfo
	void __fastcall SaveLatest();				// U:04 �������浽�����ļ�
	void __fastcall GetTargetValue();			// U:05 �ļ������ڻ��һ�����г����¶ȡ���ѹ���¶��ӳ١��¶Ȳ���˳���¶ȼ�¼ʱ����
	void __fastcall GetLineFromFile(FILE * pFile, String & strTemp, int & iEqual);	// U:06 ���ļ�һ��
	void __fastcall SetItem(TListBox * pList, int iIndex);							// U:07 TListBox���ƶ�
	int  __fastcall GetFirstSelInList(TCustomListBox * pList);						// U:08 TListBox���ƶ�
	void __fastcall MoveInList(TCustomListBox * pList, TStrings * pItems);			// U:09 TListBox���ƶ�
    void __fastcall ResetFormAndStruct();			// U:10 ���Form��g_STestInfo
    void __fastcall StructFromFile(String strPath); // U:11 �ļ���ȫ�ֱ���g_STestInfo
	void __fastcall RefreshFormFromStruct();		// U:12 ȫ�ֱ���g_STestInfo���ؼ�
   	void __fastcall RefreshStructFromForm();		// U:13 �ؼ���ȫ�ֱ���g_STestInfo
    void __fastcall SetItemCurrInfo();				// U:14 ������m_pLstbSel��m_iItemCurr
public:
	__fastcall TFormInitSet(TComponent* Owner);
};

extern PACKAGE TFormInitSet *FormInitSet;

#endif
