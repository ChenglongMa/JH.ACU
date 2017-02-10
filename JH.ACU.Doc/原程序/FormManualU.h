//---------------------------------------------------------------------------

#ifndef FormManualUH
#define FormManualUH
//---------------------------------------------------------------------------
#include <Classes.hpp>
#include <Controls.hpp>
#include <StdCtrls.hpp>
#include <Forms.hpp>
#include <ExtCtrls.hpp>
#include <Grids.hpp>
#include <ComCtrls.hpp>
#include "Global.h"

class TFormManual : public TForm
{
__published:	// IDE-managed Components
	// 测试项部分
	TPanel *m_PanelItem;
	TLabel *m_pLlItem;
	TListBox *m_pLstItem;

    // 测试项流程说明部分
	TPanel *m_pPanelItemRemark;
	TLabel *m_pLRemark;
	TMemo *m_pMoRemark;

    // 测试过程中状态部分
	TPanel *m_pPanelStatus;
	TLabel *m_pLStatus;
	TMemo *m_pMoStatus;

    // 程控电源部分
	TPanel *m_pPanelPcPower;
	TLabel *m_pLPcPower;
	TEdit *m_pEdtVolt;
	TEdit *m_pEdtCurr;
	TComboBox *m_pCmbOutput;
	TComboBox *m_pCmbOCP;
	TComboBox *m_pCmbOVP;
	TButton *m_pBtnPcPowerRST;
	TButton *m_pBtnPcPowerTST;
	TButton *m_pBtnPcPowerRead;
	TButton *m_pBtnCurr;
	TButton *m_pBtnVolt;
	TButton *m_pBtnOutput;
	TButton *m_pBtnOCP;
	TButton *m_pBtnOVP;
	TButton *m_pBtnPcPowerIDN;
	TEdit *m_pEdtPcPowerRetStatus;
	TEdit *m_pEdtPcPowerCommand;
	TButton *m_pBtnPcPowerWrite;
	TCheckBox *m_pMbtPcPowerAsync;
	TCheckBox *m_pMbtMode;
	TButton *m_pBtnPcPowerOpen;
	TButton *m_pBtnPcPowerClose;

    // 程控电阻箱部分
	TPanel *m_pPanelPcRes;
	TLabel *m_pLPcRes;
	TCheckBox *m_pBbtPcResAsync;
	TEdit *m_pEdtPcResRetStatus;
	TButton *m_pBtnPcResWrite;
	TButton *m_pBtnPcResRead;
	TEdit *m_pEdtPcResCommand;
	TButton *m_pBtnPcResIDN;
	TButton *m_pBtnPcResTST;
	TButton *m_pBtnPcResRST;
	TEdit *m_pEdtRes;
	TButton *m_pBtnSetRes;
    TComboBox *m_pCmbResBox;
	TLabel *m_pLabelResBox;
    TButton *m_pBtnPcResOpen;
	TButton *m_pBtnPcResClose;

    // 数字万用表部分
	TPanel *m_pPanelPcDMM;
	TLabel *m_pLPcDMM;
	TButton *m_pBtnPcDMMIDN;
	TButton *m_pBtnPcDMMRST;
	TButton *m_pBtnPcDMMTST;
	TEdit *m_pBtnPcDMMCommand;
	TButton *m_pBtnPcDMMWrite;
	TButton *m_pBtnPcDMMRead;
	TEdit *m_pEdtPcDMMRetStatus;
	TCheckBox *m_pMbtPcDMMAsync;
	TButton *m_pBtnGetVolt;
	TButton *m_pBtnGetCurr;
	TButton *m_pBtnGetFRes;
    TButton *m_pBtnGetRes;
	TButton *m_pBtnGetFreq;
    TEdit *m_pEdtPcDMMVolt;
    TEdit *m_pEdtPcDMMCurr;
	TEdit *m_pEdtPcDMMFRes;
    TEdit *m_pEdtPcDMMRes;
	TEdit *m_pEdtPcDMMFreq;
    TButton *m_pBtnPcDMMOpen;
	TButton *m_pBtnPcDMMClose;

    // 串口温箱部分
	TPanel *m_pPanelComChamber;
	TLabel *m_pLComChamber;
	TEdit *m_pEdtTemp;
    TEdit *m_pEdtTempCurr;
	TButton *m_pBtnSetTemp;
	TButton *m_pBtnGetTemp;
	TButton *m_pBtnWaitTemp;
    TPanel *m_pPanelTempCurr;
	TPanel *m_pPanelTempTarget;
    TButton *m_pBtnChamberOpen;
	TButton *m_pBtnChamberClose;

    // 串口Kline部分
    TPanel *m_pPanelComKline;
	TButton *m_pBtnKlineOpen;
	TButton *m_pBtnKlineClose;
	TLabel *m_pLComKline;
	TComboBox *m_pCmbCommand;
	TButton *m_pBtnExe;
	TMemo *m_pMoSend;
	TMemo *m_pMoRead;
	TLabel *m_pLSend;
	TLabel *m_pLRead;

    // 继电器控制部分
    TGroupBox *m_pGbDaqDask;
	TButton *m_pBtnDaqOpen;
	TButton *m_pBtnDaqClose;
    
	TPanel *m_pPanelRelay;
    TGroupBox *m_pGbMainBoard;
	TComboBox *m_pCmbMbRelay;
	TButton *m_pBtnMbOn;
	TButton *m_pBtnMbOff;
	TButton *m_pBtnMbTest;

	TLabel *m_pLMbStatus;
	TShape *m_pShapeF;
	TShape *m_pShapeS;
	TShape *m_pShapeT;
	TLabel *m_pLTop;
	TLabel *m_pLCardIndex;
	TLabel *m_pL20;
	TLabel *m_pL21;
	TLabel *m_pL22;
	TLabel *m_pL23;
	TLabel *m_pL24;
	TLabel *m_pL25;
	TLabel *m_pL26;
	TLabel *m_pL27;
	TLabel *m_pL28;
	TLabel *m_pLBottom;
	TStringGrid *m_pStrGrd;

	TGroupBox *m_pGbSlaveBoard;
	TLabel *m_pLCardNum;
	TLabel *m_pLRelay;
	TComboBox *m_pCmbCardNum;
	TButton *m_pBtnSbReset;
	TButton *m_pBtnSbOn;
	TButton *m_pBtnSbOff;
	TComboBox *m_pCmbSbRelay;
	TButton *m_pBtnSbTest;
	TButton *m_pBtnInit;
	TButton *m_pBtnKline;
	TButton *m_pBtnACU;
	TButton *m_pBtnWL;
	TCheckBox *m_pMbtBase;
	TButton *m_pBtnTest;
	TButton *m_pBtnHelp;

    // 状态条
	TStatusBar *m_pStatusBar;
	TTimer *m_pTimer;
	TPanel *m_pPanelInterface;
	TLabel *m_pLGpibInterface;
	TButton *m_pBtnChamberStop;
	void __fastcall FormCreate(TObject *Sender);
	void __fastcall FormClose(TObject *Sender, TCloseAction &Action);
	void __fastcall OnBtnPcPowerOpenClick(TObject *Sender);
	void __fastcall OnBtnPcPowerCloseClick(TObject *Sender);
	void __fastcall OnBtnPcPowerIDNClick(TObject *Sender);
	void __fastcall OnBtnPcPowerRSTClick(TObject *Sender);
	void __fastcall OnBtnPcPowerTSTClick(TObject *Sender);
	void __fastcall OnBtnPcPowerWriteClick(TObject *Sender);
	void __fastcall OnBtnPcPowerReadClick(TObject *Sender);
	void __fastcall OnBtnVoltClick(TObject *Sender);
	void __fastcall OnBtnCurrClick(TObject *Sender);
	void __fastcall OnBtnOutputClick(TObject *Sender);
	void __fastcall OnBtnOCPClick(TObject *Sender);
	void __fastcall OnBtnOVPClick(TObject *Sender);
	void __fastcall OnBtnPcResOpenClick(TObject *Sender);
	void __fastcall OnBtnPcResCloseClick(TObject *Sender);
	void __fastcall OnBtnPcResIDNClick(TObject *Sender);
	void __fastcall OnBtnPcResRSTClick(TObject *Sender);
	void __fastcall OnBtnPcResTSTClick(TObject *Sender);
	void __fastcall OnBtnPcResWriteClick(TObject *Sender);
	void __fastcall OnBtnPcResReadClick(TObject *Sender);
	void __fastcall OnBtnSetResClick(TObject *Sender);
	void __fastcall OnBtnPcDMMOpenClick(TObject *Sender);
	void __fastcall OnBtnPcDMMCloseClick(TObject *Sender);
	void __fastcall OnBtnPcDMMIDNClick(TObject *Sender);
	void __fastcall OnBtnPcDMMRSTClick(TObject *Sender);
	void __fastcall OnBtnPcDMMTSTClick(TObject *Sender);
	void __fastcall OnBtnPcDMMWriteClick(TObject *Sender);
	void __fastcall OnBtnPcDMMReadClick(TObject *Sender);
	void __fastcall OnBtnGetVoltClick(TObject *Sender);
	void __fastcall OnBtnGetCurrClick(TObject *Sender);
	void __fastcall OnBtnGetFResClick(TObject *Sender);
	void __fastcall OnBtnGetResClick(TObject *Sender);
	void __fastcall OnBtnGetFreqClick(TObject *Sender);
	void __fastcall OnBtnChamberOpenClick(TObject *Sender);
	void __fastcall OnBtnChamberCloseClick(TObject *Sender);
	void __fastcall OnBtnSetTempClick(TObject *Sender);
	void __fastcall OnBtnGetTempClick(TObject *Sender);
	void __fastcall OnBtnWaitTempClick(TObject *Sender);
    void __fastcall OnBtnChamberStopClick(TObject *Sender);
    void __fastcall OnBtnDaqOpenClick(TObject *Sender);
	void __fastcall OnBtnDaqCloseClick(TObject *Sender);
	void __fastcall OnBtnKlineOpenClick(TObject *Sender);
	void __fastcall OnBtnKlineCloseClick(TObject *Sender);
	void __fastcall OnBtnExeClick(TObject *Sender);
	void __fastcall OnBtnMbOnClick(TObject *Sender);
	void __fastcall OnBtnMbOffClick(TObject *Sender);
	void __fastcall OnBtnMbTestClick(TObject *Sender);
	void __fastcall OnBtnSbOnClick(TObject *Sender);
	void __fastcall OnBtnSbOffClick(TObject *Sender);
	void __fastcall OnBtnACUClick(TObject *Sender);
	void __fastcall OnBtnWLClick(TObject *Sender);
	void __fastcall OnBtnSbResetClick(TObject *Sender);
	void __fastcall OnBtnSbTestClick(TObject *Sender);
	void __fastcall OnBtnInitClick(TObject *Sender);
	void __fastcall OnBtnKlineClick(TObject *Sender);
	void __fastcall OnBtnTestClick(TObject *Sender);
	void __fastcall OnLstItemClick(TObject *Sender);
	void __fastcall OnTimer(TObject *Sender);
	void __fastcall OnStrGrdDrawCell(TObject *Sender, int ACol, int ARow, TRect &Rect, TGridDrawState State);
    void __fastcall OnBtnHelpClick(TObject *Sender);
private:
	String	m_caControl[MES_ITEM_REAL*2];
	void __fastcall EnablePcPowerState(bool bEnable);
	void __fastcall EnablePcResState(bool bEnable);
	void __fastcall EnablePcDMMState(bool bEnable);
	void __fastcall EnableChamberState(bool bEnable);
	void __fastcall EnableKlineState(bool bEnable);
	void __fastcall EnableRelayState(bool bEnable);
	void __fastcall ViewStatusInGrid(short iCard);
	void __fastcall ViewStatusInMainboard();	// User declarations
public:		// User declarations
	__fastcall TFormManual(TComponent* Owner);
};
//---------------------------------------------------------------------------
extern PACKAGE TFormManual *FormManual;
//---------------------------------------------------------------------------
#endif
