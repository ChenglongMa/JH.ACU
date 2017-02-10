#include <vcl.h>
#pragma hdrstop

#include "FormMainU.h"
#include "FormInitSetU.h"
#include "FormManualU.h"
#include "ThreadMain.h"
#include <stdio.h>
#include <dos.h>
#include <math.h>
#include <time.h>
#include "ComChamber.h"
#include "GpibPcPower.h"
#include "GpibPcDMM.h"
#include "GpibPcRes.h"
#include "DaqDigit.h"
#include "DaqAnalog.h"

#include <winbase.h>

#pragma package(smart_init)

#pragma once
#pragma resource "*.dfm"

TFormMain	*FormMain;

int TFormMain::m_siRowErrorSheet = 1;		// ��¼ErrLogSheet��д����
int TFormMain::m_siRowTempSheet = 1;		// ��¼TempSheet��д����

///////////////////////////////////////////////////////////////////////////////
// S:00
__fastcall TFormMain::TFormMain(TComponent *Owner) :
	TForm(Owner)
{
    m_iModeRun = 0;		// 0-Init State 1-Pause State 2-Run State 
    // ��ȡ����·��
    GetAppPath();
    // �����Թ淶����
    SetSPECInfo();
    // ����¼�����
    ReadFaultInfo();

    m_bWrite = false;		// ���Ʊ��д��
    m_bTVState = true;		// Start = true, End = false;  ����д��TestInfo��TVSheet
    m_strInfo = "";			// Test Information
    m_bTempBoard = true;	// ����д��TempSheet�е��¶���ÿ��忨�ǲ�һ����

    // ��ʼ��FMRow��Ӧ������ͼ 11*9(TV)*6(Board) = 594
	for (int i = 0; i < 9; i++)
	{
		for (int j = 0; j < MES_SUBBOARD_NUM; j++)
		{
			m_iaFMRow[i][j] = 600;
		}
	}

    // ��ʼ��ÿ�����貽���Ӧ����
	for (int j = 0; j < MES_ITEM_REAL; j++)
	{
		m_iaItem[j] = MES_ITEM_REAL + 4;
	}
}

///////////////////////////////////////////////////////////////////////////////
// S:01
void __fastcall TFormMain::FormCreate(TObject * Sender)
{
    // ȱʡ���Զ�ģʽ
    m_bThread = false;	// û���߳�

    // �����ʼ��
    m_pF1BookACUInfo->Enabled = false;
    m_pF1BookACUInfo->SetActiveCell(1, 1);
    m_pF1BookACUInfo->ShowActiveCell();

    m_pF1BookTestItem->Enabled = true;
    m_pF1BookTestItem->SetActiveCell(1, 1);
    m_pF1BookTestItem->ShowActiveCell();

    m_pF1BookTestReport->Enabled = true;
	m_pF1BookTestReport->Sheet = 1;
    m_pF1BookTestReport->SetActiveCell(1, 1);
    m_pF1BookTestReport->ShowActiveCell();

    SetStateCommonality(true);
    m_pBtnManual->Enabled = false;

    struct time time;
	struct date date;
	gettime(&time);
	getdate(&date);
    String strVoltFile = g_strAppPath + "\\data\\" + IntToStr(date.da_year) + IntToStrAdjust(date.da_mon) +
        IntToStrAdjust(date.da_day) + IntToStrAdjust(time.ti_hour) + IntToStrAdjust(time.ti_min) + "Volt.dat";

    OpenLogFile(2, strVoltFile);
}

///////////////////////////////////////////////////////////////////////////////
// S:02
void __fastcall TFormMain::FormDestroy(TObject *Sender)
{
	ShowMessage("FormDestroy");
}

///////////////////////////////////////////////////////////////////////////////
// S:03
void __fastcall TFormMain::FormClose(TObject *Sender, TCloseAction &Action)
{
    CloseLogFile(g_pFileVolt);
}

///////////////////////////////////////////////////////////////////////////////
// S:04
void __fastcall TFormMain::OnTimer(TObject *Sender)
{
    // 1-״̬����Ϣ
    ViewStatusBar();

    // ������Ϣ��ʾ-�¶Ⱥ�On Testing��m_pLabelSpark
    ViewLabelSpark();

    // ʵʱ�¶Ȳɼ�
    RefreshTempData();

    // ʵʱ��ѹ�ɼ�
    RefreshVoltData();
}

///////////////////////////////////////////////////////////////////////////////
// S:05
void __fastcall TFormMain::OnShapeAutoRunMouseDown(TObject * Sender, TMouseButton Button, TShiftState Shift, int X, int Y)
{
	if (m_pShapeAutoRun->Brush->Color == clLime) return;
    g_iOpMode = 0;

    if (true == m_bThread)
	{
		if (!g_pThreadMain->Suspended) g_pThreadMain->Suspend();
        g_daqDigit.ResetRelay();
        g_comChamber.Stop();
		g_pThreadMain->Terminate();
        int iRetCode = WaitForSingleObject((void *)(g_pThreadMain->Handle), 1000);
        if(WAIT_TIMEOUT == iRetCode)
        {
            g_pThreadMain->FreeInstance();
            g_pThreadMain = NULL;
        }
        m_iModeRun = 0;
		m_bThread = false;
	}

	// ��Manualģʽ��һ�ν���
	m_pShapeAutoRun->Brush->Color = clLime;
	m_pShapeManualRun->Brush->Color = clWhite;
    SetStateCommonality(true);
    SetStateAuto(true);
    SetStateManual(false);
    SetContent(true);

    // Reset Global Variable
    ResetGVariable();
    // RefreshForm
    ResetBookInfo();
    ResetBookItem();
    ResetBookReport();
}

///////////////////////////////////////////////////////////////////////////////
// S:06
void __fastcall TFormMain::OnShapeManualRunMouseDown(TObject * Sender, TMouseButton Button, TShiftState Shift, int X, int Y)
{
	if (m_pShapeManualRun->Brush->Color == clLime) return;
    g_iOpMode = 1;

    if (true == m_bThread)
	{
		if (!g_pThreadMain->Suspended) g_pThreadMain->Suspend();
        g_daqDigit.ResetRelay();
        g_comChamber.Stop();
		g_pThreadMain->Terminate();
        int iRetCode = WaitForSingleObject((void *)(g_pThreadMain->Handle), 1000);
        if(WAIT_TIMEOUT == iRetCode)
        {
            g_pThreadMain->FreeInstance();
            g_pThreadMain = NULL;
        }
        m_iModeRun = 0;
		m_bThread = false;
	}

	// ��Autoģʽ��һ�ν���
    m_iModeRun = 0;
    m_bThread = false;

	m_pShapeAutoRun->Brush->Color = clWhite;
	m_pShapeManualRun->Brush->Color = clLime;
    SetStateCommonality(true);
	SetStateAuto(false);
    SetStateManual(true);
    SetContent(true);

    bool bRet = false;
    bRet = OpenDevice();
	if (false == bRet)
	{
		g_strMark = "Open All Device or Reset All Device Failed.!";
        ShowMessage(g_strMark);
	}
    else
    {
        g_gpibPcPower.SetOCP(false);
        g_gpibPcPower.SetOutPut(false);
        g_gpibPcPower.SetCurr(5.0);
        g_gpibPcPower.SetOutPut(true);
    }

    // Reset Global Variable
    ResetGVariable();
}

///////////////////////////////////////////////////////////////////////////////
// S:07
// 		ֻ��Auto Mode����Ч
void __fastcall TFormMain::OnBtnInitClick(TObject *Sender)
{
	// �ж��Ƿ����Զ�ģʽ��
	if (clLime == m_pShapeManualRun->Brush->Color)
	{
		ShowMessage("OnBtnInitClick in Error Mode.!");
		return;
	}

    // ����ؼ�ʹ�ܺ��������
    SetStateCommonality(false);
    SetStateAuto(false);
    SetStateManual(false);
    SetContent(true);

    // ��ʼ�����̳߳�������ǰ״̬
    m_iModeRun = 0;

    // �����̲߳���������
    if (true == m_bThread)
	{
		if (!g_pThreadMain->Suspended)      g_pThreadMain->Suspend();
        g_daqDigit.ResetRelay();
        g_comChamber.Stop();
		g_pThreadMain->Terminate();
        int iRetCode = WaitForSingleObject((void *)(g_pThreadMain->Handle), 1000);
        if(WAIT_TIMEOUT == iRetCode)
        {
            g_pThreadMain->FreeInstance();
            g_pThreadMain = NULL;
        }
		m_bThread = false;
	}

    // Reset Global Variable
    ResetGVariable();

    // �������������
	SaveErrorToFile();

    // �������öԻ���
    FormInitSet->ShowModal();
	Application->ProcessMessages();

    // RefreshForm
    ResetBookInfo();
	InitBookInfo();
    ResetBookItem();
    ResetBookReport();
    InitBookReport();

    // ����ؼ�ʹ�ܺ���������
    SetStateCommonality(true);
    SetStateAuto(true);
}

///////////////////////////////////////////////////////////////////////////////
// S:08
// ֻ����ҪС�������ĵ���ֵ����13��
void __fastcall TFormMain::OnBtnRelayClick(TObject *Sender)
{
	if (true == m_bThread)
	{
		ShowMessage("System is testing.!");
		return;
	}

	if (false == g_bAllDevice)
	{
		ShowMessage("Device is not ready.!");
		return;
	}

    // Control Disable
	SetStateCommonality(false);
    SetStateAuto(false);
	SetStateManual(false);

	int			iRelayGroup = 0, iRelayBit = 0;
	float		fRes = 0.0;
	TIniFile	*pIniFile = NULL;
	pIniFile = new TIniFile(g_strAppPath + "\\config\\config.ini");

	for (int iBoard = 0; iBoard < MES_SUBBOARD_NUM; iBoard++)
	{
		// DSB��PSB��PADS��FC1��FC2��FC3��FC4��FC5��FC6��FC7��FC8��
		for (int iGroup = 0; iGroup < 13; iGroup++)
		{
			iRelayGroup = iGroup / 2;
			iRelayBit = 3 + iGroup % 2 * 4;

			// �趨������ĵ���ֵ
			g_daqDigit.SetSbRelay(iBoard, iRelayGroup, iRelayBit, true);
			g_daqDigit.SetSbRelay(iBoard, iRelayGroup, iRelayBit - 2, true);
			if (iGroup < 3) g_daqDigit.SetMbRelay(302, true);

			if (iGroup < 3) g_gpibPcResS.SetRes(1.0);
			else g_gpibPcResF.SetRes(1.0);

			g_daqDigit.SetSbRelay(iBoard, iRelayGroup, iRelayBit - 1, true);
			g_daqDigit.SetSbRelay(iBoard, iRelayGroup, iRelayBit - 2, false);
			g_daqDigit.SetSbRelay(iBoard, iRelayGroup, iRelayBit - 3, true);

			// ��������������(���ߵ��������),����ֵͨ����������ֵ����ʽ����
			g_gpibPcDMM.GetFRes(fRes);
			fRes = fRes - 1.0;
			pIniFile->WriteFloat("contact_res", "Card" + IntToStr(iBoard + 1) + "_" + IntToStr(iGroup + 1), fRes);

			g_daqDigit.SetSbRelay(iBoard, iRelayGroup, iRelayBit, false);
			g_daqDigit.SetSbRelay(iBoard, iRelayGroup, iRelayBit - 1, false);
			g_daqDigit.SetSbRelay(iBoard, iRelayGroup, iRelayBit - 2, false);
			g_daqDigit.SetSbRelay(iBoard, iRelayGroup, iRelayBit - 3, false);
			if (iGroup < 3) g_daqDigit.SetMbRelay(300 + 2, false);
		}
	}

	delete pIniFile;
	ShowMessage("Test Contact Resistance Success.!");

    // ����ؼ�ʹ�ܺ���������:�ж��Ƿ����Զ�ģʽ��
    SetStateCommonality(true);
	if (clLime == m_pShapeManualRun->Brush->Color)
	{
        SetStateAuto(false);
        SetStateManual(true);
	}
    else
    {
        SetStateAuto(true);
        SetStateManual(false);
    }
}

///////////////////////////////////////////////////////////////////////////////
// S:09
void __fastcall TFormMain::OnBtnRunClick(TObject *Sender)
{
	// ǰ��Ϊ���Σ������ť����ǰ����
	if (0 == m_iModeRun)
	{
		SetStateCommonality(false);
        SetStateAuto(false);
        SetStateManual(false);
        m_pBtnRun->Caption = "Pause";
		m_pBtnRun->Enabled = true;
	}

    // ǰ��Ϊ���У������ť����ǰ�ݶ�
    if (2 == m_iModeRun)
	{
        m_pBtnRun->Caption = "Run";
        if (m_pShapeAutoRun->Brush->Color == clLime)
		{
            SetStateCommonality(true);
            SetStateAuto(true);
			SetStateManual(false);
		} else
		{
            SetStateCommonality(true);
            SetStateAuto(false);
			SetStateManual(true);
		}
	}

	// ǰ��Ϊ�ݶ��������ť����ǰΪ��������
	if (1 == m_iModeRun)
	{
		m_pBtnRun->Caption = "Pause";
        SetStateCommonality(false);
        SetStateAuto(false);
        SetStateManual(false);
        m_pBtnRun->Enabled = true;
	}

	// �ڶ���:�жϸ���������ڸ�������´���Run
	// �ǵ�һ�ΰ��£���Run��Pause
	if (true == m_bThread)
	{
		if (1 == m_iModeRun)
		{
			g_pThreadMain->Resume();
			m_iModeRun = 2;
			g_bTimeStart = true;
			return;
		} else if (2 == m_iModeRun)
		{
			g_pThreadMain->Suspend();
			m_iModeRun = 1;
			g_bTimeStart = false;
			return;
		}
	}

	// �ڵ�ǰģʽ�µ�һ�ε��Run
	struct time time;
	struct date date;
	gettime(&time);
	getdate(&date);

	String	strFilePath = g_strAppPath + "\\data\\" + IntToStr(date.da_year) + IntToStrAdjust(date.da_mon) + IntToStrAdjust(date.da_day) + IntToStrAdjust(time.ti_hour) + IntToStrAdjust(time.ti_min) + ".dat";
	OpenLogFile(1, strFilePath);

	if (m_pShapeAutoRun->Brush->Color == clLime)
	{
    	m_siRowErrorSheet = 1;
		g_pThreadMain = new TThreadMain(false);
        m_pBtnRun->Caption = "Pause";
		g_pThreadMain->OnTerminate = OnThreadTerminate;
		m_bThread = true;
		m_iModeRun = 2;
        g_bTimeStart = true;

		return;
	}

	// �ж��ֹ���ʽ�������Ƿ�����
    m_iModeRun = 0;
    m_bThread = false;
	bool bRet = RefreshGStruct();
    if(false == bRet)
    {
    	ShowMessage("Incomplete test settings for manual model!");
        SetStateCommonality(true);
		m_pBtnInit->Enabled = false;
        m_pBtnRelay->Enabled = false;
        SetStateCommonality(true);
        SetStateAuto(false);
    	SetStateManual(true);
        m_pBtnRun->Caption = "Run";
		return;
    }
	ChangeItem();
	g_pThreadMain = new TThreadMain(false);
    m_pBtnRun->Caption = "Pause";
	g_pThreadMain->OnTerminate = OnThreadTerminate;
	m_bThread = true;
	m_iModeRun = 2;
	g_iSecond = 0;
	g_bTimeStart = true;
	g_bViewChamber = false;
	g_bViewTest = true;
}

///////////////////////////////////////////////////////////////////////////////
// S:10
void __fastcall TFormMain::OnBtnQuitClick(TObject *Sender)
{
	// ʹ�����пؼ�ΪʧЧ
	SetStateCommonality(false);
    SetStateAuto(false);
    SetStateManual(false);

    // �ж��߳��Ƿ���ִ��
    if (true == m_bThread)
	{
		if (!g_pThreadMain->Suspended) g_pThreadMain->Suspend();
        g_daqDigit.ResetRelay();
        g_comChamber.Stop();
		g_pThreadMain->Terminate();
        int iRetCode = WaitForSingleObject((void *)(g_pThreadMain->Handle), 1000);
        if(WAIT_TIMEOUT == iRetCode)
        {
            g_pThreadMain->FreeInstance();
            g_pThreadMain = NULL;
        }
		m_bThread = false;
	}

    // �ر��豸
    bool bRet = CloseDevice();
    if(false == bRet)
    {
    	ShowMessage("CloseDevice in OnBtnQuitClick Failed.!");
    }
    else
    {
    	g_bAllDevice = false;
    }

    Close();
}

///////////////////////////////////////////////////////////////////////////////
// S:11
void __fastcall TFormMain::OnBtnManualClick(TObject *Sender)
{
	if (clLime != m_pShapeManualRun->Brush->Color)
	{
		ShowMessage("OnPanelACUNumClick in Error Mode.!");
		return;
	}

    // ����ؼ�ʹ�ܺ��������
    SetStateCommonality(false);
    SetStateAuto(false);
    SetStateManual(false);

    bool bRet = OpenDevice();
    if(false == bRet)
    {
    	ShowMessage(g_strMark);
    	SetStateCommonality(true);
    	SetStateAuto(false);
    	SetStateManual(true);
        return;
    }
    FormManual->ShowModal();
}

///////////////////////////////////////////////////////////////////////////////
// S:12
void __fastcall TFormMain::OnShapeLowTempMouseDown(TObject *Sender, TMouseButton Button, TShiftState Shift, int X, int Y)
{
	// ��ؿؼ���m_pPanelTempTarget��m_pPanelTempCurr
	m_pShapeTempLow->Brush->Color = clLime;
	m_pShapeTempNor->Brush->Color = clWhite;
	m_pShapeTempHigh->Brush->Color = clWhite;

	g_iTempPhase = 0;
	ViewTempTarget();
	this->Update();

    // ������ʵʱ�����¶��򲻴���
	if (false == m_pChbChamAuto->Checked) return;

	g_fTempTarget = g_STestInfo.m_strTemp[0].ToDouble();
	g_comChamber.SetTemp(g_fTempTarget);
	ViewWaveTempTheory();
	ViewTempCurr();

	// ��ؿؼ���m_pLabelSpark��m_pTempCurveReal��m_pF1BookTestReport
	// OnTimer���� ������ʾ
	g_bTempRenew = true;	// �����¶�������ʾ
	g_bTempReal = true;		// OnTimer�Ƿ���Ч
	g_iTempCount = 1;		// ����m_pPanelTempCurr
	g_iTempInterval = 1;	// д��Temp Sheet
	g_fTempCurveX = 10.00;	// �����¶�������ʾ

	// ����m_pLabelSpark��ʾ����
	g_bViewChamber = true;
	g_bViewTest = false;
	g_iViewChamber = 0;
}

///////////////////////////////////////////////////////////////////////////////
// S:13
void __fastcall TFormMain::OnShapeNorTempMouseDown(TObject *Sender, TMouseButton Button, TShiftState Shift, int X, int Y)
{
	// ��ؿؼ���m_pPanelTempTarget��m_pPanelTempCurr
    m_pShapeTempLow->Brush->Color = clWhite;
	m_pShapeTempNor->Brush->Color = clLime;
	m_pShapeTempHigh->Brush->Color = clWhite;

    g_iTempPhase = 1;
    ViewTempTarget();
	this->Update();

    // ������ʵʱ�����¶��򲻴���
	if (false == m_pChbChamAuto->Checked) return;

    g_fTempTarget = g_STestInfo.m_strTemp[1].ToDouble();
    g_comChamber.SetTemp(g_fTempTarget);
    ViewWaveTempTheory();
    ViewTempCurr();

    // ��ؿؼ���m_pLabelSpark��m_pTempCurveReal��m_pF1BookTestReport
    // OnTimer���� ������ʾ
    g_bTempRenew = true;	// �����¶�������ʾ
    g_bTempReal = true;		// OnTimer�Ƿ���Ч
	g_iTempCount = 1;		// ����m_pPanelTempCurr
	g_iTempInterval = 1; 	// д��Temp Sheet
    g_fTempCurveX = 10.00; 	// �����¶�������ʾ

    // ����m_pLabelSpark��ʾ����
	g_bViewChamber = true;
	g_bViewTest = false;
    g_iViewChamber = 0;
}

///////////////////////////////////////////////////////////////////////////////
// S:14
void __fastcall TFormMain::OnShapeHighTempMouseDown(TObject *Sender, TMouseButton Button, TShiftState Shift, int X, int Y)
{
	// ��ؿؼ���m_pPanelTempTarget��m_pPanelTempCurr
    m_pShapeTempLow->Brush->Color = clWhite;
	m_pShapeTempNor->Brush->Color = clWhite;
	m_pShapeTempHigh->Brush->Color = clLime;

    g_iTempPhase = 2;
    ViewTempTarget();
	this->Update();

    // ������ʵʱ�����¶��򲻴���
	if (false == m_pChbChamAuto->Checked) return;

    g_fTempTarget = g_STestInfo.m_strTemp[2].ToDouble();
    g_comChamber.SetTemp(g_fTempTarget);
    ViewWaveTempTheory();
    ViewTempCurr();

    // ��ؿؼ���m_pLabelSpark��m_pTempCurveReal��m_pF1BookTestReport
    // OnTimer���� ������ʾ
    g_bTempRenew = true;	// �����¶�������ʾ
    g_bTempReal = true;		// OnTimer�Ƿ���Ч
	g_iTempCount = 1;		// ����m_pPanelTempCurr
	g_iTempInterval = 1; 	// д��Temp Sheet
    g_fTempCurveX = 10.00; 	// �����¶�������ʾ

    // ����m_pLabelSpark��ʾ����
    g_bViewChamber = true;
	g_bViewTest = false;
	g_iViewChamber = 0;
}

///////////////////////////////////////////////////////////////////////////////
// S:15
void __fastcall TFormMain::OnShapeLowVoltMouseDown(TObject *Sender, TMouseButton Button, TShiftState Shift, int X, int Y)
{
	// ��ؿؼ���m_pPanelVoltTarget��m_pPanelVoltCurr
    m_pShapeVoltLow->Brush->Color = clLime;
	m_pShapeVoltNor->Brush->Color = clWhite;
	m_pShapeVoltHigh->Brush->Color = clWhite;

    g_iVoltPhase = 0;
    ViewVoltTarget();
	this->Update();

	float	fVolt = g_STestInfo.m_strVolt[0].ToDouble();
	g_gpibPcPower.SetVolt(fVolt);
    g_gpibPcPower.SetOutPut(true);
    G_SDelay(500);
    ViewVoltCurr();

    // OnTimer���� ��ѹ�ɼ�
    g_iVoltCount = 1;
	g_bVoltReal = true;
}

///////////////////////////////////////////////////////////////////////////////
// S:16
void __fastcall TFormMain::OnShapeNorVoltMouseDown(TObject *Sender, TMouseButton Button, TShiftState Shift, int X, int Y)
{
	// ��ؿؼ���m_pPanelVoltTarget��m_pPanelVoltCurr
    m_pShapeVoltLow->Brush->Color = clWhite;
	m_pShapeVoltNor->Brush->Color = clLime;
	m_pShapeVoltHigh->Brush->Color = clWhite;

    g_iVoltPhase = 1;
    ViewVoltTarget();
	this->Update();

	float	fVolt = g_STestInfo.m_strVolt[1].ToDouble();
	g_gpibPcPower.SetVolt(fVolt);
    g_gpibPcPower.SetOutPut(true);
    G_SDelay(500);
    ViewVoltCurr();

    // OnTimer���� ��ѹ�ɼ�
	g_iVoltCount = 1;
	g_bVoltReal = true;
}

///////////////////////////////////////////////////////////////////////////////
// S:17
void __fastcall TFormMain::OnShapeHighVoltMouseDown(TObject *Sender, TMouseButton Button, TShiftState Shift, int X, int Y)
{
	// ��ؿؼ���m_pPanelVoltTarget��m_pPanelVoltCurr
    m_pShapeVoltLow->Brush->Color = clWhite;
	m_pShapeVoltNor->Brush->Color = clWhite;
	m_pShapeVoltHigh->Brush->Color = clLime;

    g_iVoltPhase = 2;
    ViewVoltTarget();
	this->Update();

	float	fVolt = g_STestInfo.m_strVolt[2].ToDouble();
	g_gpibPcPower.SetVolt(fVolt);
    g_gpibPcPower.SetOutPut(true);
    G_SDelay(500);
    ViewVoltCurr();

    // OnTimer���� ��ѹ�ɼ�
	g_iVoltCount = 1;
	g_bVoltReal = true;
}

///////////////////////////////////////////////////////////////////////////////
// S:18
void __fastcall TFormMain::OnPanelACUNumClick(TObject *Sender)
{
	if (clLime != m_pShapeManualRun->Brush->Color)
	{
		ShowMessage("OnPanelACUNumClick in Error Mode.!");
		return;
	}

    // g_iCurrBoard��ʱ����
	g_iCurrBoard = m_pPanelACU->Caption.ToInt();
	g_iCurrBoard++;
	if (g_iCurrBoard == 7) g_iCurrBoard = 1;
	m_pPanelACU->Caption = IntToStr(g_iCurrBoard);
    // g_iCurrBoard��ʵֵ
    g_iCurrBoard--;
}

///////////////////////////////////////////////////////////////////////////////
// S:19
void __fastcall TFormMain::OnBtnSaveClick(TObject *Sender)
{
	SetStateCommonality(false);
    SetStateAuto(false);
    SetStateManual(false);

    String	TableRecPath = "";
	String	TestName = g_STestInfo.m_strInfo[0];
	String	ModelName = g_STestInfo.m_strInfo[1];

	ForceDirectories(g_strAppPath + "\\output");
	m_pDlgSave->Title = "Save As";
	m_pDlgSave->InitialDir = g_strAppPath + "\\output";
	m_pDlgSave->FileName = DateToStr(Date()) + "_" + TestName + "_" + ModelName + ".xls";
	m_pDlgSave->Filter = "XLS Files(*.xls)|*.xls";

    String	strTableName[9] =
	{
		"LowTemp-LowVolt",
		"LowTemp-MidVolt",
		"LowTemp-HighVolt",
		"MidTemp-LowVolt",
		"MidTemp-MidVolt",
		"MidTemp-HighVolt",
		"HighTemp-LowVolt",
		"HighTemp-MidVolt",
		"HighTemp-HighVolt"
	};

    for(int iSheet = 2; iSheet <= 10; iSheet++)
    {
    	m_pF1BookTestReport->SheetName[iSheet] = strTableName[iSheet - 2];
    }

    // �����ļ�
	if (FormMain->m_pDlgSave->Execute())
	{
		TableRecPath = m_pDlgSave->FileName;

		String	subs = TableRecPath.SubString(TableRecPath.Length() - 3, 4);
		if (subs == ".Xls" || subs == ".xls" || subs == ".XLS") TableRecPath = m_pDlgSave->FileName;
		else TableRecPath = FormMain->m_pDlgSave->FileName + ".xls";

		if (FileExists(TableRecPath))
		{
			String	sts = "File��" + TableRecPath + "already exists.Replace existing file��";
			if (MessageBox(NULL, sts.c_str(), "Attention!", MB_YESNO | MB_ICONWARNING) == IDYES)
			{
				DeleteFile(TableRecPath);
			} else
			{
				return ;
			}
		}

		String	vtspath;					// ���ñ���vts�ļ�
		vtspath = TableRecPath.SubString(1, TableRecPath.Length() - 4) + ".vts";
		if (FileExists(vtspath))
		{
			DeleteFile(vtspath);
		}

		m_pF1BookTestReport->Write(vtspath, 5);

		vtspath = g_strAppPath + "\\output\\temp.xls"; // ������ʱexcel�ļ�
		if (FileExists(vtspath))
		{
			DeleteFile(vtspath);
		}

		m_pF1BookTestReport->Write(vtspath, 4);

        // ������ʽexcel�ļ�
		SaveToExcel(TableRecPath);

		DeleteFile(vtspath);					// ɾ����ʱexcel�ļ�
		ShowMessage("Table Save Complete!");
	}

    // ��ԭ����
    SetStateCommonality(true);
    SetStateManual(false);

    String	strTableInitName[9] =
	{
		"LTLV",
		"LTNV",
		"LTHV",
		"NTLV",
		"NTNV",
		"NTHV",
		"HTLV",
		"HTNV",
		"HTHV"
	};

    for(int iSheet = 2; iSheet <= 10; iSheet++)
    {
    	m_pF1BookTestReport->SheetName[iSheet] = strTableInitName[iSheet - 2];
    }

    // ����ؼ�ʹ�ܺ���������:�ж��Ƿ����Զ�ģʽ��
    SetStateCommonality(true);
	if (clLime == m_pShapeManualRun->Brush->Color)
	{
        SetStateAuto(false);
        SetStateManual(true);
	}
    else
    {
        SetStateAuto(true);
        SetStateManual(false);
    }
}

///////////////////////////////////////////////////////////////////////////////
// S:20
void __fastcall TFormMain::OnBtnLoadClick(TObject *Sender)
{
	SetStateCommonality(false);
    SetStateAuto(false);
    SetStateManual(false);
    
    String	strPath = "";
	ForceDirectories(g_strAppPath + "\\output");

	FormMain->m_pDlgOpen->Title = "Open";
	FormMain->m_pDlgOpen->InitialDir = g_strAppPath + "\\output";
	FormMain->m_pDlgOpen->Filter = "XLS Files(*.xls)|*.xls";

	if (FormMain->m_pDlgOpen->Execute())
	{
		strPath = m_pDlgOpen->FileName;

		String	subs = strPath.SubString(strPath.Length() - 3, 4);
		if (subs == ".Xls" || subs == ".xls" || subs == ".XLS") strPath = m_pDlgOpen->FileName;
		else strPath = m_pDlgOpen->FileName + ".xls";

		if (!FileExists(strPath))
		{
			String	strTip = "File��" + strPath + " do not exist!";
			ShowMessage(strTip);
		} else
		{
			try // ����һ��excel��ʱ�ļ�
			{
				m_varApp = Variant::CreateObject("Excel.Application");
				m_varApp.OlePropertySet("Visible", (Variant) false);	// ʹExcel������ɼ�
				m_varBook = m_varApp.OlePropertyGet("WorkBooks");

				m_varBook.OleProcedure("Open", strPath.c_str());
				m_varBook = m_varApp.OlePropertyGet("ActiveWorkbook");
			}
			catch(Exception & exception)
			{
				Application->MessageBox("Failed to find EXCEL! Maybe you do not have EXCEL in computer! !", "Notice", MB_OK);
				return ;
			}

			String	strPath1 = strPath.SubString(1, strPath.Length() - 4) + "1.xls";
            m_varSheet = m_varBook.OlePropertyGet("WorkSheets", MES_SHEET_FM);
			m_varSheet.OleFunction("SaveAs", strPath1.c_str());
			m_varApp.OleFunction("Quit");

			OpenFromExcel(strPath1);
			DeleteFile(strPath1);
		}
	}

    String	strTableInitName[9] =
	{
		"LTLV",
		"LTNV",
		"LTHV",
		"NTLV",
		"NTNV",
		"NTHV",
		"HTLV",
		"HTNV",
		"HTHV"
	};

    for(int iSheet = 2; iSheet <= 10; iSheet++)
    {
    	m_pF1BookTestReport->SheetName[iSheet] = strTableInitName[iSheet - 2];
    }

    // ����ؼ�ʹ�ܺ���������:�ж��Ƿ����Զ�ģʽ��
    SetStateCommonality(true);
	if (clLime == m_pShapeManualRun->Brush->Color)
	{
        SetStateAuto(false);
        SetStateManual(true);
	}
    else
    {
        SetStateAuto(true);
        SetStateManual(false);
    }
}

///////////////////////////////////////////////////////////////////////////////
// S:21
void __fastcall TFormMain::OnF1BookTestItemClick(TObject * Sender, int nRow, int nCol)
{
	 ResetBookItem();

	m_pF1BookTestItem->SetSelection(nRow, 1, nRow, 2);
	m_pF1BookTestItem->SetFocus();

    if(clLime == m_pShapeManualRun->Brush->Color)
    {
    	m_pF1BookTestItem->SetPattern(1, 0xFFFF00, 0xFF0000);
        m_pF1BookTestItem->ShowActiveCell();
		g_iCurrItem = nRow - 1;
    }
}

///////////////////////////////////////////////////////////////////////////////
// S:22
///////////////////////////////////////////////////////////////////////////////
// S:23
void __fastcall TFormMain::OnChartTempClick(TObject *Sender)
{
	m_pChartWave->Visible = true;
	m_pChartTemp->Visible = false;
}

///////////////////////////////////////////////////////////////////////////////
// S:24
void __fastcall TFormMain::OnChartWaveClick(TObject *Sender)
{
	m_pChartWave->Visible = false;
	m_pChartTemp->Visible = true;	
}

///////////////////////////////////////////////////////////////////////////////
// S:25
void __fastcall TFormMain::OnThreadTerminate(TObject * pObject)
{
	// �ر�ʵʱ�ļ�
    CloseLogFile(g_pFileComKline);

    // �رտ���ʵʱ��ʾ�ı���
    // ��ؿؼ���m_pPanelTempCurr��m_pTempCurveReal��m_pLabelSpark��m_pF1BookTestReport
    // OnTimer����
	g_fTimeRest = g_fTimeTotal + 10 + 1 / 60.0;
	g_iTempCount = 0;		// ����m_pPanelTempCurr
	g_iTempInterval = 0; 	// д��Temp Sheet
	g_bTempRenew = false;	// �����¶�������ʾ
    g_fTempCurveX = 10.00; 	// �����¶�������ʾ

    g_bVoltReal = false; 		// m_pPanelVoltCurr
	g_bTempReal = false;		// m_pLabelTempCurr

    // ����m_pLabelSpark��ʾ����
	g_iViewChamber = 0;
	g_iViewTest = 0;
	g_bViewChamber = false;
	g_bViewTest = false;

    // �ر��߳�
    m_bThread = false;
    m_iModeRun = 0;

    MessageDlg("Test Finished.!",mtConfirmation,TMsgDlgButtons()<<mbOK,0);

	SetStateCommonality(true);
    m_pBtnRun->Caption = "Run";
	if (clLime == m_pShapeManualRun->Brush->Color)
	{
		SetStateAuto(false);
		SetStateManual(true);
	} else
	{
    	SetStateAuto(true);
		SetStateManual(false);
	}
    SetContent(true);
}

///////////////////////////////////////////////////////////////////////////////
// S:26 ������ť���� 7��
void __fastcall TFormMain::SetStateCommonality(bool bEnable)
{
	// Auto��Manualģʽѡ��
	m_pShapeAutoRun->Enabled = bEnable;
    m_pShapeManualRun->Enabled = bEnable;
    // Relay��Run/Pause��Quit��ť
    m_pBtnRelay->Enabled = bEnable;
    m_pBtnRun->Enabled = bEnable;
    m_pBtnQuit->Enabled = bEnable;
    // Save��Load��ť
    m_pBtnLoad->Enabled = bEnable;
    m_pBtnSave->Enabled = bEnable;
}

///////////////////////////////////////////////////////////////////////////////
// S:27 ���⣺ֻ���Զ�ģʽ�� 1��
void __fastcall TFormMain::SetStateAuto(bool bEnable)
{
	// Init��ť
	m_pBtnInit->Enabled = bEnable;
}

///////////////////////////////////////////////////////////////////////////////
// S:27 ���⣺ֻ���ֹ�ģʽ�� 10��
void __fastcall TFormMain::SetStateManual(bool bEnable)
{
	// Manual������ť
	m_pBtnManual->Enabled = bEnable;
	// �¶ȡ���ѹ��ECUѡ��ʹ��
	m_pShapeTempHigh->Enabled = bEnable;
    m_pShapeTempNor->Enabled = bEnable;
    m_pShapeTempLow->Enabled = bEnable;
    m_pShapeVoltLow->Enabled = bEnable;
    m_pShapeVoltNor->Enabled = bEnable;
    m_pShapeVoltHigh->Enabled = bEnable;
    // ECUѡ��������ơ�CrashOutType
    m_pPanelACU->Enabled = bEnable;
    m_pRgCouttype->Enabled = bEnable;
    m_pChbChamAuto->Enabled = bEnable;
}

///////////////////////////////////////////////////////////////////////////////
// S:28
void __fastcall TFormMain::SetContent(bool bReset)
{
	// ��������
	m_pTestCurve->Clear();
    m_pTempCurveTheory->Clear();
    m_pTempCurveReal->Clear();
    m_pChartWave->BottomAxis->Minimum = 0.0;
    m_pChartWave->BottomAxis->Maximum = 19.0;

    // ����ItemBook��ʽ
    ResetBookItem();

    //����������ʾ��������
    m_pProBaMain->Position = 0;
	m_pLabelProgress->Caption = "0%";
    // ��ʾ��Ϣ
    m_pLabelCond->Caption = "";
    m_pLabelSpark->Caption = "";

    // ������������
    m_pShapeTempLow->Brush->Color = clWhite;
	m_pShapeTempNor->Brush->Color = clWhite;
	m_pShapeTempHigh->Brush->Color = clWhite;
	m_pShapeVoltLow->Brush->Color = clWhite;
	m_pShapeVoltNor->Brush->Color = clWhite;
	m_pShapeVoltHigh->Brush->Color = clWhite;

	m_pPanelTempTarget->Caption = "";
	m_pPanelTempCurr->Caption = "";
    m_pPanelVoltTarget->Caption = "";
    m_pPanelVoltCurr->Caption = "";

	m_pPanelACU->Caption = "0";

	// ״̬��
    m_pStatusBar->Panels->Items[0]->Text = "";
    m_pStatusBar->Panels->Items[1]->Text = "";
    m_pStatusBar->Panels->Items[2]->Text = "";
}

///////////////////////////////////////////////////////////////////////////////
// S:29
void __fastcall TFormMain::SaveToExcel(String strPath)
{
	try
	{
		m_varApp = Variant::CreateObject("Excel.Application");
		m_varApp.OlePropertySet("Visible", (Variant) false);
		m_varBook = m_varApp.OlePropertyGet("WorkBooks");

        // ����
		m_varBook.OleProcedure("Open", (g_strAppPath + "\\output\\temp.xls").c_str());
		m_varBook = m_varApp.OlePropertyGet("ActiveWorkbook");
	}
	catch(Exception & exception)
	{
		Application->MessageBox("Failed to find EXCEL!Maybe you do not have EXCEL in computer! !", "Notice", MB_OK);
		return ;
	}
	this->Update();

	String	strName = "";
	String	strTestName = g_STestInfo.m_strInfo[0];
	String	strModelName = g_STestInfo.m_strInfo[1];

    // ���ó�TempSheet������Sheet
	for (int iSheet = 1; iSheet < 13; iSheet++)
	{
		m_varSheet = m_varBook.OlePropertyGet("WorkSheets", iSheet);
		m_varSheet.OlePropertyGet("PageSetup").OlePropertySet("PrintGridLines", false);	// ����ӡ�����;
		m_varSheet.OlePropertyGet("PageSetup").OlePropertySet("Orientation", 2);		// Orientation=poLandscape;1;2ҳ��Ϊ����;
		m_varSheet.OlePropertyGet("PageSetup").OlePropertySet("CenterHeader", "");		// ����ҳüΪ��
		m_varSheet.OlePropertyGet("PageSetup").OlePropertySet("TopMargin", 1.5/0.035);	// ����ҳ���ϱ߾�
		m_varSheet.OlePropertyGet("PageSetup").OlePropertySet("BottomMargin", 2/0.035); // ����ҳ���±߾�
	}

    // ����TVSheet��ҳ�������ƺ�״̬��
	for (int iSheet = 2; iSheet <= 10; iSheet++)
	{
		m_varSheet = m_varBook.OlePropertyGet("WorkSheets", iSheet);
		m_varSheet.OlePropertyGet("Rows", 1).OleProcedure("Insert");

		strName = strTestName + "_" + strModelName;
		m_varSheet.OlePropertyGet("Cells", 1, 1).OlePropertyGet("Font").OlePropertySet("Size", 10);	// �������������С
		m_varSheet.OlePropertyGet("Cells", 1, 1).OlePropertySet("Value", strName.c_str());			// �������
		m_varSheet.OlePropertyGet("Cells", 1, 1).OlePropertySet("HorizontalAlignment", 2);
		m_varSheet.OlePropertyGet("Cells", 1, 14).OlePropertyGet("Font").OlePropertySet("Size", 10);// �������������С
		m_varSheet.OlePropertyGet("Cells", 1, 14).OlePropertySet("HorizontalAlignment", 1);
	}

	m_pF1BookTestReport->Sheet = 2;
	m_varSheet.OleFunction("SaveAs", strPath.c_str());
	m_pStatusBar->Panels->Items[0]->Text = "                    Saving..." + IntToStr(100) + "%";

    // ���׹ر�EXCEL �������ù��Ķ�����Ϊ��
    m_varBook.OleProcedure("Close");
    String strSave = "DisplayAlerts";
    m_varApp.OlePropertySet(strSave.c_str(), false);
	m_varApp.OleFunction("Quit");
    m_varSheet = NULL;
    m_varBook = NULL;
    m_varApp = NULL;
}

///////////////////////////////////////////////////////////////////////////////
// S:30
// Ҫ��ϵͳ��ð�װEXCEL2003��Ͱ汾
void __fastcall TFormMain::OpenFromExcel(String strPath)
{
	try
	{
		m_varApp = Variant::CreateObject("Excel.Application");
		m_varApp.OlePropertySet("Visible", (Variant) false);
		m_varBook = m_varApp.OlePropertyGet("WorkBooks");
		m_varBook.OleProcedure("Open", strPath.c_str());
		m_varBook = m_varApp.OlePropertyGet("ActiveWorkbook");
	}
	catch(Exception & exception)
	{
		Application->MessageBox("Failed to find EXCEL! Maybe you do not have EXCEL in computer.! !", "Notice", MB_OK);
		return ;
	}

	int 			iRow = 1, iRowStart = 1, iSheet = 1, iRowTemp = 1;
	unsigned int	iColor = 0;
	bool			bBold = false;
	String			strTemp = "";

    // ɾ���Ÿ�TVSheet���������ƺ�״̬��(��һ��)
	for (iSheet = 2; iSheet <= 10; iSheet++)
	{
		m_varSheet = m_varBook.OlePropertyGet("WorkSheets", iSheet);
		m_varSheet.OlePropertyGet("Rows", 1).OleProcedure("Delete");
	}

    // ����TotalSheet��TVSheet��ErrorLogSheet
	for (iSheet = 1; iSheet <= MES_SHEET_ERRLOG; iSheet++)
	{	// ͳ��ÿ��Sheet��¼������
		// Application->ProcessMessages();
		m_pStatusBar->Panels->Items[0]->Text = "                    Loading..." + IntToStr(iSheet * 7) + "%";
		Update();

		m_varSheet = m_varBook.OlePropertyGet("WorkSheets", iSheet);
		m_pF1BookTestReport->Sheet = iSheet;

		strTemp = "1";					// ����������
		iRowTemp = 1;
        // TotalSheet��ErrlogSheet������TVSheet��һ��
		if (iSheet == MES_SHEET_TOTAL)
		{	// �ҵ�д���ݵ����һ�У��ӵ����п�ʼ�ж��з����ݴ洢
			for (iRow = 3; strTemp != ""; iRow++)
			{	// SPECCOL
				strTemp = m_varSheet.OlePropertyGet("Cells", iRow, 2).OlePropertyGet("Value");
				iRowTemp = iRow - 1;
			}
		} else if (iSheet == MES_SHEET_ERRLOG)
		{	// �ҵ�д���ݵ����һ�У��ӵڶ��п�ʼ�ж��з����ݴ洢
			for (iRow = 2; strTemp != ""; iRow++)
			{	//TIMECOL
				strTemp = m_varSheet.OlePropertyGet("Cells", iRow, 2).OlePropertyGet("Value");
				iRowTemp = iRow - 1;
			}
		} else
		{	// �ҵ�д���ݵ����һ�У��ӵڶ��п�ʼ�ж��з����ݴ洢
			for (iRow = 2; strTemp != "Test Information"; iRow++)
			{	//TESTITEMCOL
				strTemp = m_varSheet.OlePropertyGet("Cells", iRow, 1).OlePropertyGet("Value");
				iRowTemp = iRow + 1;
			}
		}

		if (iSheet == MES_SHEET_TOTAL) iRowStart = 3;
		else iRowStart = 2;

        // ��������
		for (int i = iRowStart; i < iRowTemp + 1; i++)
		{
			for (int j = 1; j < MES_TV_ERRCOL + 6; j++)
			{
				strTemp = m_varSheet.OlePropertyGet("Cells", i, j).OlePropertyGet("Value");
				m_pF1BookTestReport->SetSelection(i, j, i, j);
				iColor = m_varSheet.OlePropertyGet("Cells", i, j).OlePropertyGet("Font").OlePropertyGet("Color");
				bBold = m_varSheet.OlePropertyGet("Cells", i, j).OlePropertyGet("Font").OlePropertyGet("Bold");
				if (iColor == 255)
				{
					m_pF1BookTestReport->SetFont("Arial", 10, 1, 0, 0, 0, 0x0000FF, 0, 0);
				} else
				{
					if (bBold == true)
					{
						m_pF1BookTestReport->SetFont("Arial", 10, 1, 0, 0, 0, 0x000000, 0, 0);
					} else
					{
						m_pF1BookTestReport->SetFont("Arial", 10, 0, 0, 0, 0, 0x000000, 0, 0);
					}
				}

				if ((iSheet < MES_SHEET_ERRLOG) && (iSheet > MES_SHEET_TOTAL) && (i > iRowTemp - 2) && (j > MES_T_SPECCOL))
				{
					;
				} else m_pF1BookTestReport->Text = strTemp;
			}

            // �ж��Ƿ���Ҫ��ӿհ���
			if ((iSheet == MES_SHEET_TOTAL) && (iRowTemp > 2) && (i > 2) && (i < iRowTemp - 1))
			{
				m_pF1BookTestReport->SetSelection(i + 1, 1, i + 1, 1);
				m_pF1BookTestReport->EditInsert(3);
			} else if ((iSheet == MES_SHEET_ERRLOG) && (iRowTemp > 1) && (i > 1) && (i < iRowTemp - 1))
			{
				m_pF1BookTestReport->SetSelection(i + 1, 1, i + 1, 1);
				m_pF1BookTestReport->EditInsert(3);
			} else
			{
				if (i < iRowTemp - 2)
				{
					m_pF1BookTestReport->SetSelection(i + 1, 1, i + 1, 1);
					m_pF1BookTestReport->EditInsert(3);
					m_pF1BookTestReport->SetSelection(i + 1, MES_T_ITEMCOL, i + 1, MES_TV_AVGCOL);
					m_pF1BookTestReport->SetBorder(-1, -1, 1, -1, -1, -1, 0, 0, 0, 0, 0);
				}

				if (i > iRowTemp - 2)
				{
					m_pF1BookTestReport->SetSelection(i, MES_T_SPECCOL, i, MES_TV_JUDGECOL);
					m_pF1BookTestReport->SetAlignment(7, 0, 3, 0);
				}
			}
		}

        // ���ر���һ��
		m_pF1BookTestReport->SetSelection(1, 1, 1, 1);
	}

	m_pStatusBar->Panels->Items[0]->Text = "                    Loading..." + IntToStr(85) + "%";
	Update();

	// ����FM����
	m_varSheet = m_varBook.OlePropertyGet("WorkSheets", MES_SHEET_FM);
    m_pF1BookTestReport->Sheet = MES_SHEET_FM;
	int iFMRowAll = 0;
	strTemp = "FMR";
	for (int iFMRow = 6; (strTemp == "FMR") && (iFMRow < 810); iFMRow = iFMRow + 11)
	{
		strTemp = m_varSheet.OlePropertyGet("Cells", iFMRow, 1).OlePropertyGet("Value");
		iFMRowAll++;
	}
	for (int itmp = 0; itmp < (iFMRowAll - 1); itmp++)
	{
		m_pF1BookTestReport->SetSelection(1, 1, 11, 2);
		m_pF1BookTestReport->EditCopy();
		m_pF1BookTestReport->SetSelection(itmp * 11 + 1, 1, itmp * 11 + 11, 2);
		m_pF1BookTestReport->EditPaste();
	}

	iRowTemp = iFMRowAll * 11;
	for (int i = 1; i < iRowTemp + 1; i++)
	{
		for (int j = 1; j < 3; j++)
		{
			strTemp = m_varSheet.OlePropertyGet("Cells", i, j).OlePropertyGet("Value");
			m_pF1BookTestReport->SetSelection(i, j, i, j);
			bBold = m_varSheet.OlePropertyGet("Cells", i, j).OlePropertyGet("Font").OlePropertyGet("Bold");
			if (bBold == true)
			{
				if (strTemp.Length() > 100)
				{
					m_pF1BookTestReport->SetFont("Arial", 8, 1, 0, 0, 0, 0x000000, 0, 0);
				} else
				{
					m_pF1BookTestReport->SetFont("Arial", 10, 1, 0, 0, 0, 0x000000, 0, 0);
				}
			} else
			{
				if (strTemp.Length() > 100)
				{
					m_pF1BookTestReport->SetFont("Arial", 8, 0, 0, 0, 0, 0x000000, 0, 0);
				} else
				{
					m_pF1BookTestReport->SetFont("Arial", 10, 0, 0, 0, 0, 0x000000, 0, 0);
				}
			}

			m_pF1BookTestReport->Text = strTemp;
		}
	}

	m_pStatusBar->Panels->Items[0]->Text = "                    Loading..." + IntToStr(92) + "%";
	Update();
    // ��д�¶ȱ�
	m_varSheet = m_varBook.OlePropertyGet("WorkSheets", MES_SHEET_TEMP);
	m_pF1BookTestReport->Sheet = MES_SHEET_TEMP;
	strTemp = "time";
	iRowTemp = 0;

	TTime	timetmp;
	double	dbtime;

	for (int itmp = 1; (itmp < 16384) && (strTemp != ""); itmp++)
	{
		strTemp = m_varSheet.OlePropertyGet("Cells", itmp, 1).OlePropertyGet("Value");
		iRowTemp++;
	}

	for (int i = 2; i < iRowTemp; i++)
	{
		int j = 1;
		dbtime = m_varSheet.OlePropertyGet("Cells", i, j).OlePropertyGet("Value");
		timetmp = dbtime;
		m_pF1BookTestReport->SetSelection(i, j, i, j);
		iColor = m_varSheet.OlePropertyGet("Cells", i, j).OlePropertyGet("Font").OlePropertyGet("Color");
		if (iColor == 32768)
		{
			m_pF1BookTestReport->SetFont("Arial", 10, 0, 0, 0, 0, 0x00B000, 0, 0);
		} else
		{
			m_pF1BookTestReport->SetFont("Arial", 10, 0, 0, 0, 0, 0x000000, 0, 0);
		}

		m_pF1BookTestReport->Text = TimeToStr(timetmp);

		j = 2;
		strTemp = m_varSheet.OlePropertyGet("Cells", i, j).OlePropertyGet("Value");
		m_pF1BookTestReport->SetSelection(i, j, i, j);
		iColor = m_varSheet.OlePropertyGet("Cells", i, j).OlePropertyGet("Font").OlePropertyGet("Color");
		if (iColor == 32768)
		{
			m_pF1BookTestReport->SetFont("Arial", 10, 0, 0, 0, 0, 0x00B000, 0, 0);
		} else
		{
			m_pF1BookTestReport->SetFont("Arial", 10, 0, 0, 0, 0, 0x000000, 0, 0);
		}

		m_pF1BookTestReport->Text = strTemp;
	}
    m_pStatusBar->Panels->Items[0]->Text = "                    Loading..." + IntToStr(100) + "%";

	m_pF1BookTestReport->Sheet = MES_SHEET_TV;

	// ���׹ر�EXCEL �������ù��Ķ�����Ϊ��
    m_varBook.OleProcedure("Close");
    String strSave = "DisplayAlerts";
    m_varApp.OlePropertySet(strSave.c_str(), false);
	m_varApp.OleFunction("Quit");
    m_varSheet = NULL;
    m_varBook = NULL;
    m_varApp = NULL;
    iSheet = 0;
}

///////////////////////////////////////////////////////////////////////////////
// S:31 �ֹ�ģʽ�¸���ȫ�ֿ��Ʊ���g_STestInfo
bool __fastcall TFormMain::RefreshGStruct()
{
	int i = 0;
    // 1-m_iTVCond
    for(i = 0; i < 9; i++)
    {
    	g_STestInfo.m_iTVCond[i] = 0;
    }

    int iTempMode = 0;
    TColor corLtemp, corNtemp, corHtemp;
    corLtemp = m_pShapeTempLow->Brush->Color;
    corNtemp = m_pShapeTempNor->Brush->Color;
    corHtemp = m_pShapeTempHigh->Brush->Color;
	if (corLtemp == clLime) iTempMode = 0;
	if (corNtemp == clLime) iTempMode = 1;
	if (corHtemp == clLime) iTempMode = 2;
	if (corLtemp == clWhite && corNtemp == clWhite && corHtemp == clWhite)
    {
    	ShowMessage("Set Test Temperature failed in Manual Mode.!");
        return false;
    }
    int iVoltMode = 0;
    TColor corLvolt, corNvolt, corHvolt;
    corLvolt = m_pShapeVoltLow->Brush->Color;
    corNvolt = m_pShapeVoltNor->Brush->Color;
    corHvolt = m_pShapeVoltHigh->Brush->Color;
	if (corLvolt == clLime) iVoltMode = 0;
	if (corNvolt == clLime) iVoltMode = 1;
	if (corHvolt == clLime) iVoltMode = 2;
    if (corLvolt == clWhite && corNvolt == clWhite && corHvolt == clWhite)
    {
    	ShowMessage("Set Test Voltage failed in Manual Mode.!");
        return false;
    }

    g_iTVCond = -1;
	if (iTempMode == 0 && iVoltMode == 0) g_iTVCond = 0;
	if (iTempMode == 0 && iVoltMode == 1) g_iTVCond = 1;
	if (iTempMode == 0 && iVoltMode == 2) g_iTVCond = 2;
	if (iTempMode == 1 && iVoltMode == 0) g_iTVCond = 3;
	if (iTempMode == 1 && iVoltMode == 1) g_iTVCond = 4;
	if (iTempMode == 1 && iVoltMode == 2) g_iTVCond = 5;
	if (iTempMode == 2 && iVoltMode == 0) g_iTVCond = 6;
	if (iTempMode == 2 && iVoltMode == 1) g_iTVCond = 7;
	if (iTempMode == 2 && iVoltMode == 2) g_iTVCond = 8;
    g_STestInfo.m_iTVCond[g_iTVCond] = 1;

    // 4-m_iItem �� m_iSelACU
    g_iCurrBoard = m_pPanelACU->Caption.ToInt() - 1;
    if(g_iCurrBoard < 0 || g_iCurrBoard > MES_SUBBOARD_NUM - 1)
    {
    	ShowMessage("Set Test ECU failed in Manual Mode.!");
        return false;
    }
    if(g_iCurrItem < 0 || g_iCurrItem >= MES_ITEM_REAL - 1)
    {
    	ShowMessage("Set Test Item failed in Manual Mode.!");
        return false;
    }
    for(i = 0; i < MES_ITEM_REAL; i++)
    {
    	g_STestInfo.m_iItem[i] = 0;
    }
    g_STestInfo.m_iItem[g_iCurrItem] = 0x01 << (g_iCurrBoard);
    g_STestInfo.m_iSelACU = 0x01 << (g_iCurrBoard);

    // 5-Crash & Chamber Control
    int iTemp = 0;
    if(1 == m_pRgCouttype->ItemIndex)	iTemp = 1;
    else	iTemp = 0;
    g_STestInfo.m_iDaqType = iTemp;

    if(true == m_pChbChamAuto->Checked)	iTemp = 1;
    else	iTemp = 0;
    g_STestInfo.m_iChamType = iTemp;

    return true;
}

///////////////////////////////////////////////////////////////////////////////
// S:32 ����ʵʱ��ʾ���Ʊ���
void __fastcall TFormMain::ResetGVariable()
{
    // OnTimer:ViewLabelSpark��current setting
	g_iViewChamber = 0; 	// Chamber Process
  	g_iViewTest = 0; 		// on testing
 	g_bViewChamber = false;	// Chamber Process
 	g_bViewTest = false;	// on testing
    g_strCurrSet = "";		// current setting

    // OnTimer::RefreshTempData������д��Report
 	g_bChamberIdle = true;	// �����Ƿ����
 	g_bTempReal = false;	// �Ƿ���ȡ�¶�
  	g_iTempCount = 1;		// ÿ10S��ȡʵ���¶�
  	g_iTempInterval = 1;	// ����д���ļ�Interval
	g_iTempRow = 1;			// д��TempSheet��������

    // OnTimer::RefreshVoltData
	g_iVoltCount = 1;			// ÿ10S��ȡʵ�ʵ�ѹ
	g_bVoltReal = false;		// �Ƿ���ȡ��ѹ

    // OnTimer::ViewStatusBar
	g_bTimeStart = false;	// on testing �Ƿ�������ʼ
 	g_iSecond = 0;			// �����ۻ�ʱ��
	g_strMainTip = "";		// ��ʾ��Ϣ������
	g_strSubTip = "";		// ��ʾ��Ϣ��С����

    // TeeChart
    g_bTempRenew = true;	// ������ʾ�¶�����
 	g_fTimeTotal = 0.0;		// ÿ�������¶ȹ�������ʱ��
 	g_fTimeRest = 0.0;		// ÿ�������¶�����ʱ��
 	g_fTempInit = 0.0; 		// ÿÿ�����������¶�ʱ�ĳ�ʼ�¶�
 	g_fTempCurveX = 10.00;	// ʵ���¶����ߺ����� m_pTempCurveReal
	g_fTempCurveY = 0.0;	// ʵ���¶����������� m_pTempCurveReal

	g_iCurveMode = 0;		// 0-����ģʽ 1-HoldTimeģʽ 2-Crashģʽ
 	g_dCurveCurrX = 0.0;   	// ������������ʾX
 	g_fCurveCurrY = 0.0;	// ������������ʾY
 	g_dCurveLength = 19.0;	// ������������ʾ���ȣ�	�������Ӧ0-19.0 1-999.0 2-999.0
    g_strCurveUnit = "ohm";	// ������ʾ��ߵ�λ,	�������Ӧ0-ohm 1-v 2-v

    // ����
	g_iTempPhase = 0;		// �����¶Ƚ׶� based 0  0-LT 1-NT 2-HT
	g_iVoltPhase = 0;		// ���õ�ѹ�׶� based 0  0-LV 1-NT 2-HV
	g_fTempTarget = 0.0;	// ��ǰĿ���¶ȣ��������Ӧ
	g_fVoltTarget = 0.0;	// ��ǰĿ���ѹ���������Ӧ
	g_fVoltInit = 0.0;		// ÿ���������õ�Դ��ѹʱ�ĳ�ʼ��ѹ
	g_fVoltCurr = 0.0;    	// ʵʱ��Դ��ѹ
	g_iCRC = 0;				// ��ĳһ��TempVolt�µ�ĳһ���忨�µ�FM��������
	g_iTVCond = 0;			// ���ڲ��Ե�TV����	based 0
	g_iCurrBoard = 0;		// ���ڲ��Ե�ECU�������Ӱ� based 0
	g_iCurrItem = 0;		// ���ڲ��Եĵ�ǰReport���� based 0
	g_bOpState = false;		// ��������״̬
	g_iOpMode = 0;         	// 0-�Զ���1-�ֹ�����ģʽ
	g_strMark = "";			// ������ʾ��Ϣ
	g_strItemValue = "";	// ��������ɺ������ֵ
}

///////////////////////////////////////////////////////////////////////////////
// S:33 ����״̬��
void __fastcall TFormMain::ViewStatusBar()
{
	// 1 ��ʾ��Ϣ
    m_pStatusBar->Panels->Items[0]->Text = g_strMainTip + g_strSubTip;

    // 2 ���Գ���ʱ����Ϣ
    if (g_bTimeStart)
	{
		g_iSecond++;
		String	strTime = IntToStrTime(g_iSecond);
		m_pStatusBar->Panels->Items[1]->Text = "Testing Last " + strTime;
	} else
	{
		String	strTime = IntToStrTime(g_iSecond);
		m_pStatusBar->Panels->Items[1]->Text = "Testing Last " + strTime;
	}

    // 3 ��ǰʱ����Ϣ
	time_t	t1;
	time(&t1);
	m_pStatusBar->Panels->Items[2]->Text = ctime(&t1);
}

///////////////////////////////////////////////////////////////////////////////
// S:34 ����LabelSpark
void __fastcall TFormMain::ViewLabelSpark()
{
	if (g_bViewChamber)
	{
		switch (g_iViewChamber)
		{
			case 0:
				{
					g_fTimeRest = g_fTimeRest - 1 / 60.0;
					m_pLabelSpark->Caption = "Charmber Setting Process";
					g_iViewChamber++;
					break;
				}
			case 1:
				{
					g_fTimeRest = g_fTimeRest - 1 / 60.0;
					m_pLabelSpark->Caption = "";
					g_iViewChamber++;
					break;
				}
			case 2:
				{
					g_fTimeRest = g_fTimeRest - 1 / 60.0;
					if (g_fTimeRest < 0) g_fTimeRest = 0;

					int iRestMin = floor(g_fTimeRest);
					int iRestSec = (int)((g_fTimeRest - iRestMin) * 60);
					m_pLabelSpark->Caption = "Waiting " + IntToStr(iRestMin) + " min " + IntToStr(iRestSec) + " sec";
					g_iViewChamber++;
					break;
				}
			case 3:
				{
					g_fTimeRest = g_fTimeRest - 1 / 60.0;
					m_pLabelSpark->Caption = "";
					g_iViewChamber = 0;
					break;
				}
			default:
				{
					g_iViewChamber = 0;
				}
		}
	}
    
	if (g_bViewTest)
	{
		switch (g_iViewTest)
		{
			case 0:		{ m_pLabelSpark->Caption = "on testing"; g_iViewTest++; break; }
			case 1:		{ m_pLabelSpark->Caption = ""; g_iViewTest = 0; break; }
			default:	{ g_iViewTest = 0; }
		}
	}
}

///////////////////////////////////////////////////////////////////////////////
// S:35 �����¶����ݣ��豣��
//		ÿ10S��ȡʵ���¶Ȳ���Ҫ���¶ȱ��浽TempSheet
void __fastcall TFormMain::RefreshTempData()
{
	if ((g_bAllDevice == true) && g_bTempReal)
	{
		g_iTempCount--;
		if (g_iTempCount == 0)
		{
			ViewTempCurr();
			g_iTempCount = 10;
		}

		g_iTempInterval--;
		if (g_iTempInterval == 0)
		{
			int j = 0;
			for (j = 0; j < 100; j++)
			{
				G_SDelay(10);
				if (m_bWrite == false)
				{
					WriteTempSheet();
					g_iTempInterval = StrToInt(g_STestInfo.m_strInterval);
					break;
				}
			}
            // TempSheetû��׼������
			if (j == 100) g_iTempInterval++;
		}
	}
}

///////////////////////////////////////////////////////////////////////////////
// S:36 ���µ�ѹ���ݣ��豣��
//		ÿ10S��ȡʵ�ʵ�ѹ����Ҫ�󽫵�ѹ���浽�ļ�Volt.txt
void __fastcall TFormMain::RefreshVoltData()
{
	if ((g_bAllDevice == true) && g_bVoltReal)
	{
		String	strVoltTarget = "", strVoltCurr = "";
		double	dVoltTarget = 0.0, dVoltCurr = 0.0;

		g_iVoltCount--;
		if (g_iVoltCount == 0)
		{
           	// ʵ�ʵ�ѹ
			ViewVoltCurr();
            this->Update();
			g_iVoltCount = 10;

			strVoltTarget = m_pPanelVoltTarget->Caption;
            strVoltTarget.Trim();
            if(strVoltTarget.Length() < 1)
                return;
			while (strVoltTarget.Pos(' ') != 0) strVoltTarget.Delete(strVoltTarget.Pos(' '), 1);
			dVoltTarget = strVoltTarget.ToDouble();

            strVoltCurr = m_pPanelVoltCurr->Caption;
            strVoltCurr.Trim();
            if(strVoltCurr.Length() < 1)
                return;
			while (strVoltCurr.Pos(' ') != 0) strVoltCurr.Delete(strVoltCurr.Pos(' '), 1);
			dVoltCurr = strVoltCurr.ToDouble();

			if (fabs(dVoltTarget - dVoltCurr) >= dVoltTarget * 0.15)
			{
            	// ��ѹ��Ӧ�Ƿ�����
                bool bRet = false;
            	bRet = g_daqAnalog.GetPowerState();
                if(true == bRet)	return;

            	// ���ٻ�ȡʵ�ʵ�ѹ
				g_gpibPcPower.SetOutPut(false);
				g_bVoltReal = false;

                // �����߳�
				if (m_bThread)
				{
					if (!g_pThreadMain->Suspended) g_pThreadMain->Suspend();
					g_pThreadMain->Terminate();
					m_bThread = false;

                    // ��д����
                    SaveErrorToFile();
                    // �ؼ�״̬��ԭ
                    if (m_pShapeAutoRun->Brush->Color == clLime)
                    {
                    	SetStateCommonality(true);
                        m_pBtnManual->Enabled = false;
						SetStateManual(false);
						m_pBtnRun->Caption = "Run";
                    }
                    else
                    {
                    	SetStateCommonality(true);
						m_pBtnInit->Enabled = false;
    					SetStateManual(true);
                        m_pBtnRun->Caption = "Run";
                    }

				}

				ShowMessage("Overcurent Protection in Power!");
			}
		}
	}
}

///////////////////////////////////////////////////////////////////////////////
//
//
//
//
//
//
///////////////////////////////////////////////////////////////////////////////
// U:01 ����ؼ����ݷֱ��ʵ���
void __fastcall TFormMain::WndProc(Messages::TMessage & Message)
{
	static int iWidth, iHeight;
	if (Message.WParam == SC_MAXIMIZE || Message.WParam == SC_MAXIMIZE + HTCAPTION)
	{
    	//int iStatusWidth = m_pStatusBar->Width;
        int iStatusHeight = m_pStatusBar->Height;
    	int iFormWidth = this->Width;
        int iFormHeight = this->Height;
        int iFormWidthClient = this->ClientWidth;
        //int iFormHeightClient = this->ClientHeight;
        //int iVerticalTitle = iFormHeight - iFormHeightClient;
        int iHorizontalTitle = iFormWidth - iFormWidthClient;
        int iScreenWidth = Screen->Width;
        int iScreenHeight = Screen->Height;

    	m_pImgCorp->Left = iScreenWidth - 150;
     	m_pLabelTitle->Width = iScreenWidth - iHorizontalTitle;

        int iLeft = m_pPanelCurve->Left + iHorizontalTitle + 4;	// 4��Ԥ���հ�
        iWidth = m_pPanelCurve->Width;
    	m_pPanelCurve->Width = iScreenWidth - iLeft;
        m_pChartTemp->Width = iScreenWidth - iLeft;
        m_pChartWave->Width = iScreenWidth - iLeft;
        iWidth = m_pPanelCurve->Width - iWidth;

        m_pPanelTestReport->Width = iScreenWidth - iLeft;
        m_pF1BookTestReport->Width = iScreenWidth - iLeft - 10;

        iHeight = (iScreenHeight - iFormHeight - iStatusHeight)/2;
    	m_pPanelCurve->Height = m_pPanelCurve->Height + iHeight;
        m_pChartTemp->Height = m_pChartTemp->Height + iHeight;
        m_pChartWave->Height = m_pChartWave->Height + iHeight;

        m_pPanelTestReport->Top = m_pPanelTestReport->Top + iHeight;
        m_pPanelTestReport->Height = m_pPanelTestReport->Height + iHeight;
        m_pF1BookTestReport->Height = m_pF1BookTestReport->Height + iHeight;

        m_pBtnSave->Top = m_pBtnSave->Top + iHeight;
        m_pBtnLoad->Top = m_pBtnLoad->Top + iHeight;
        m_pLCurrSet->Top = m_pLCurrSet->Top + iHeight;

     	m_pPanelTestItems->Height = m_pPanelTestItems->Height + iHeight*2;
        m_pF1BookTestItem->Height = m_pF1BookTestItem->Height + iHeight*2;
	}
    else if(Message.WParam == SC_RESTORE || Message.WParam == SC_RESTORE + HTCAPTION)
    {
    	m_pImgCorp->Left = m_pImgCorp->Left - iWidth;
     	m_pLabelTitle->Width = m_pLabelTitle->Width - iWidth;

    	m_pPanelCurve->Width = m_pPanelCurve->Width - iWidth;
        m_pChartTemp->Width = m_pChartTemp->Width - iWidth;
        m_pChartWave->Width = m_pChartWave->Width - iWidth;

        m_pPanelTestReport->Width = m_pPanelTestReport->Width - iWidth;
        m_pF1BookTestReport->Width = m_pF1BookTestReport->Width - iWidth - 10;

    	m_pPanelCurve->Height = m_pPanelCurve->Height - iHeight;
        m_pChartTemp->Height = m_pChartTemp->Height - iHeight;
        m_pChartWave->Height = m_pChartWave->Height - iHeight;

        m_pPanelTestReport->Top = m_pPanelTestReport->Top - iHeight;
        m_pPanelTestReport->Height = m_pPanelTestReport->Height - iHeight;
        m_pF1BookTestReport->Height = m_pF1BookTestReport->Height - iHeight;

        m_pBtnSave->Top = m_pBtnSave->Top - iHeight;
        m_pBtnLoad->Top = m_pBtnLoad->Top - iHeight;
        m_pLCurrSet->Top = m_pLCurrSet->Top - iHeight;

     	m_pPanelTestItems->Height = m_pPanelTestItems->Height - iHeight*2;
        m_pF1BookTestItem->Height = m_pF1BookTestItem->Height - iHeight*2;
    }
    
	TForm::WndProc(Message);
}

///////////////////////////////////////////////////////////////////////////////
// U:02
// �Ѹ�������������Ϊֻ����λС�����ٰѸ�λ�ϵ��������������������ϼӿո���ʾ
// ������ĸ�������������Ϊֻ����λС�����ٰѸ�λ�ϵ��������������󷵻�������־λ����Ϊ0����Ϊ1��
// fData Ϊ����ĸ�������piΪ���������׵�ַ������������ŷ����ĸ�����
int __fastcall TFormMain::AdjustData(float fData, int * piValue)
{
	int i = 0, j = 1000, iRet = 0, iTemp = 0;
	if (fData >= 0)
	{
		iRet = 0;
		iTemp = floor(fData * 100 + 0.5);
	} else
	{
		iRet = 1;
		iTemp = abs(floor(fData * 100 - 0.5) + 1);
	}
    // piValue[0]Ϊ���λ��piValue[3]Ϊ���λ
	for (i = 0; i < 4; i++)
	{
		piValue[i] = iTemp / j;
		iTemp = iTemp - piValue[i] * j;
		j = j / 10;
	}
	return iRet;
}

///////////////////////////////////////////////////////////////////////////////
// U:03
void __fastcall TFormMain::ViewTempTarget()
{
	int		iSign = 0, iaBit[4] = {0};
	String	strSign[2] = { "+", "-" };
	switch (g_iTempPhase)
	{
		case 0:		// LT
			{ g_fTempTarget = StrToFloat(g_STestInfo.m_strTemp[0]); break; }
		case 1:		// NT
			{ g_fTempTarget = StrToFloat(g_STestInfo.m_strTemp[1]); break; }
		case 2:		// HT
			{ g_fTempTarget = StrToFloat(g_STestInfo.m_strTemp[2]); break; }
		default:	g_fTempTarget = -88.88;
	}
	iSign = AdjustData(g_fTempTarget, iaBit);
	m_pPanelTempTarget->Caption = strSign[iSign] + IntToStr(iaBit[0]) + " " + IntToStr(iaBit[1]) + ". " +
    		IntToStr(iaBit[2]) + " " + IntToStr(iaBit[3]);
}

///////////////////////////////////////////////////////////////////////////////
// U:04
void __fastcall TFormMain::ViewVoltTarget()
{
	int		iSign = 0, iaBit[4] = {0};
	String	strSign[2] = { "+", "-" };
	switch (g_iVoltPhase)
	{
		case 0:		// LV
			{ g_fVoltTarget = StrToFloat(g_STestInfo.m_strVolt[0]); break; }
		case 1:		// NV
			{ g_fVoltTarget = StrToFloat(g_STestInfo.m_strVolt[1]); break; }
		case 2:		// HV
			{ g_fVoltTarget = StrToFloat(g_STestInfo.m_strVolt[2]); break; }
		default:	g_fVoltTarget = -88.88;
	}
	iSign = AdjustData(g_fVoltTarget, iaBit);
	m_pPanelVoltTarget->Caption = strSign[iSign] + IntToStr(iaBit[0]) + " " + IntToStr(iaBit[1]) + ". " +
    		IntToStr(iaBit[2]) + " " + IntToStr(iaBit[3]);
}

///////////////////////////////////////////////////////////////////////////////
// U:05
void __fastcall TFormMain::ViewTempCurr()
{
	int		iSign = 0, iaBit[4] = {0};
	String	strSign[2] = { "+", "-" };
	if (g_bTempRenew)
	{
		m_pTempCurveReal->AddXY(0.0, (double)(g_fTempInit), "", clGreen);
		iSign = AdjustData(g_fTempInit, iaBit);
		m_pPanelTempCurr->Caption = strSign[iSign] + IntToStr(iaBit[0]) + " " + IntToStr(iaBit[1]) + ". " +
        		IntToStr(iaBit[2]) + " " + IntToStr(iaBit[3]);
		g_bTempRenew = false;
	} else
	{
		while (!g_bChamberIdle)
		{
        	;
		}
		g_bChamberIdle = false;
        g_comChamber.ReadTemp(g_fTempCurveY);
		g_bChamberIdle = true;

        // ����ʵ���¶�����
        m_pTempCurveReal->AddXY(g_fTempCurveX, (double)(g_fTempCurveY), "", clGreen);
		g_fTempCurveX = g_fTempCurveX + 10.0;
        // ����ʵ���¶�Shape
		iSign = AdjustData(g_fTempCurveY, iaBit);
		m_pPanelTempCurr->Caption = strSign[iSign] + IntToStr(iaBit[0]) + " " + IntToStr(iaBit[1]) + ". " +
        		IntToStr(iaBit[2]) + " " + IntToStr(iaBit[3]);
	}
}

///////////////////////////////////////////////////////////////////////////////
// U:06
void __fastcall TFormMain::ViewVoltCurr()
{
	int		iSign = 0, iaBit[4] = {0};
	String	strSign[2] = { "+", "-" };

    double fVoltSingle = 0.0, fVoltMulti = 0.0;
    String  strVolt = "";
    // ��ȡʵ�ʵ�ѹֵ
    g_daqAnalog.GetVoltFromChannel(1, MES_VOLT_IGNCH, fVoltSingle);
    g_daqAnalog.GetVoltFromChannel(2, MES_VOLT_IGNCH, fVoltMulti);
    strVolt = FloatToStr(fVoltSingle);
    int iLength = strVolt.Length();
    for(; iLength <= 16; iLength++)
    {
        strVolt = strVolt + "0";
    }
    WriteLogFile(g_pFileVolt, strVolt + "S\n");

    strVolt = FloatToStr(fVoltMulti);
    g_fVoltCurr = fVoltMulti;
    iLength = strVolt.Length();
    for(; iLength <= 16; iLength++)
    {
        strVolt = strVolt + "0";
    }
    WriteLogFile(g_pFileVolt, strVolt + "M\n");

    iSign = AdjustData(fVoltMulti, iaBit);
	m_pPanelVoltCurr->Caption = strSign[iSign] + IntToStr(iaBit[0]) + " " + IntToStr(iaBit[1]) + ". " + IntToStr(iaBit[2]) + " " + IntToStr(iaBit[3]);
}

///////////////////////////////////////////////////////////////////////////////
// U:07
void __fastcall TFormMain::ViewBoardNum()
{
	m_pPanelACU->Caption = String(g_iCurrBoard + 1);
}

///////////////////////////////////////////////////////////////////////////////
// U:08
// bPos�Ƿ񵥲�
void __fastcall TFormMain::ViewProgressInfo()
{
	if(false == g_bProPos)
    {
     	//m_pProBaMain->StepBy(g_iProPos);
        m_pProBaMain->Position = g_iProPos;
        m_pLabelProgress->Caption = IntToStr(m_pProBaMain->Position) + "%";
    }
	else
    {
        m_pProBaMain->StepIt();
        m_pLabelProgress->Caption = IntToStr(m_pProBaMain->Position) + "%";
    }
    m_pProBaMain->Update();
}

///////////////////////////////////////////////////////////////////////////////
// U:09
void __fastcall TFormMain::ViewWave()
{
	m_pChartWave->LeftAxis->Title->Caption = g_strCurveUnit;
	if (0 == g_iCurveMode)
	{
		m_pTestCurve->AddXY(g_dCurveCurrX, g_fCurveCurrY, "", clRed);
		if (g_dCurveCurrX >= 20.0)
		{
			FormMain->m_pChartWave->BottomAxis->Maximum = g_dCurveCurrX;
			FormMain->m_pChartWave->BottomAxis->Minimum = g_dCurveCurrX - 20.0 + 1.0;
		}

		g_dCurveCurrX = g_dCurveCurrX + 1.0;
	}

	if (1 == g_iCurveMode)
	{
		for (int i = 0; i <= g_dCurveLength; i++)
		{
			m_pTestCurve->AddXY(g_dCurveCurrX, g_daCurveY[i], "", clBlack);
			g_dCurveCurrX = g_dCurveCurrX + 1.0;
		}
        G_SDelay(500);
	}

	m_pChartWave->Update();
}

///////////////////////////////////////////////////////////////////////////////
// U:10
void __fastcall TFormMain::ClearWave()
{
	m_pChartWave->LeftAxis->Title->Caption = "";
	m_pTestCurve->Clear();
	m_pChartWave->BottomAxis->Minimum = 0.0;
    if(0 == g_iCurveMode)
		m_pChartWave->BottomAxis->Maximum = 19.0;
    else
		m_pChartWave->BottomAxis->Maximum = 999.0;
	g_dCurveCurrX = 0.0;
}

void __fastcall TFormMain::ViewLabelCurrSet()
{
	m_pLCurrSet->Caption = "CurrSetting:" + g_strCurrSet;
	m_pLCurrSet->Update();
}

///////////////////////////////////////////////////////////////////////////////
// U:11
// ����ֵ�Ĵ����д�����
void __fastcall TFormMain::AddErrorByCode()
{
	int			i = 0;
	bool		bFound = false;
	String		strErrCode = "", strErrRemark = "", strTempCurr = "", strVoltCurr = "", strErrInfo = "";
	struct time stime;
	struct date sdate;
    gettime(&stime);
	getdate(&sdate);

    // �ɹ�����ֱ�ӷ���
	for (i = 0; i < m_iErrItemCount; i++)
	{
		if (g_iCurrFaultCode == m_iaErrItem[i])
			break;
	}
	if (i < m_iErrItemCount)
	{
		return;
	}

	// ȷΪ�¹���
	strTempCurr = m_pPanelTempCurr->Caption;
	strTempCurr.Trim();
	if (strTempCurr == "") strTempCurr = "+88.88";
	strVoltCurr = m_pPanelVoltCurr->Caption;
	strVoltCurr.Trim();
    if (strVoltCurr == "") strTempCurr = "+35.00";

	while (strTempCurr.Pos(' ') != 0) strTempCurr.Delete(strTempCurr.Pos(' '), 1);
	while (strVoltCurr.Pos(' ') != 0) strVoltCurr.Delete(strVoltCurr.Pos(' '), 1);

	m_iaErrItem[m_iErrItemCount] = g_iCurrFaultCode;
	m_iErrItemCount++;
	bFound = GetFaultInfo(g_iCurrFaultCode, strErrCode, strErrRemark);
    // 1-���Tlistbox
	m_pLstbError->Height = MES_LISTLINE_HEIGHT + MES_LISTLINE_DISTANCE;
	if (true == bFound)
	{
		strErrInfo = IntToStr(sdate.da_year) + "-" + IntToStrAdjust(sdate.da_mon) + "-" +
        	IntToStrAdjust(sdate.da_day) + "  " + IntToStrAdjust(stime.ti_hour) + ":" +
            IntToStrAdjust(stime.ti_min) + ":" + IntToStrAdjust(stime.ti_sec) + "  [" +
            strTempCurr + " 'C " + strVoltCurr + " V]  [ACU " + IntToStr(g_iCurrBoard) + "]  [" +
			strErrCode + "]  [" + strErrRemark + "]";
	}
	else
	{
		 strErrInfo = IntToStr(sdate.da_year) + "-" + IntToStrAdjust(sdate.da_mon) + "-" +
            IntToStrAdjust(sdate.da_day) + "  " + IntToStrAdjust(stime.ti_hour) + ":" +
            IntToStrAdjust(stime.ti_min) + ":" + IntToStrAdjust(stime.ti_sec) + "  [" +
            strTempCurr + " 'C " + strVoltCurr + " V]  [ACU " + IntToStr(g_iCurrBoard) + "]  [0x" +
			DToH1(g_iCurrFaultCode) + "]  [Unknown error code!]";
	}
    m_pLstbError->Items->Strings[m_iErrLstCount] = strErrInfo;
	m_pLstbError->TopIndex = m_iErrLstCount;
    m_iErrLstCount++;

    // 2-���ErrLogSheet
    WriteErrorSheet(strErrInfo);
	// 3-���ErrCol
    WriteErrorCol();
    // 4-���ErrLog.txt
    strErrInfo = strErrInfo + "\n";
    WriteLogFile(g_pFileErrReal, strErrInfo);
}

///////////////////////////////////////////////////////////////////////////////
// U:12
void __fastcall TFormMain::AddErrorByMark()
{
	String		strTempCurr = "\0", strVoltCurr = "\0", strErrInfo = "";
	struct time stime;
	struct date sdate;
	gettime(&stime);
	getdate(&sdate);

	strTempCurr = m_pPanelTempCurr->Caption;
	strTempCurr.Trim();
	if (true == strTempCurr.IsEmpty()) strTempCurr = "+88.88";
	strVoltCurr = m_pPanelVoltCurr->Caption;
	strVoltCurr.Trim();
	if (true == strVoltCurr.IsEmpty()) strVoltCurr = "+35.00";
	while (strVoltCurr.Pos(' ') != 0) strVoltCurr.Delete(strVoltCurr.Pos(' '), 1);
	while (strVoltCurr.Pos(' ') != 0) strVoltCurr.Delete(strVoltCurr.Pos(' '), 1);

	// 1-���Tlistbox
	m_pLstbError->Height = MES_LISTLINE_HEIGHT + MES_LISTLINE_DISTANCE;
	strErrInfo = IntToStr(sdate.da_year) + "-" + IntToStrAdjust(sdate.da_mon) + "-" +
		IntToStrAdjust(sdate.da_day) + "  " + IntToStrAdjust(stime.ti_hour) + ":" +
		IntToStrAdjust(stime.ti_min) + ":" + IntToStrAdjust(stime.ti_sec) + "  [" +
		strTempCurr + " 'C " + strVoltCurr + " V]  [ACU " + IntToStr(g_iCurrBoard) +
		"]  [****]  [" + g_strMark + "]";

	m_pLstbError->Items->Strings[m_iErrLstCount] = strErrInfo;
	m_pLstbError->TopIndex = m_iErrLstCount;
	m_iErrLstCount++;

	// 2-���ErrLogSheet
	WriteErrorSheet(strErrInfo);
    // 3-���ErrCol
	WriteErrorCol();
	// 4-���ErrLog.txt
	strErrInfo = strErrInfo + "\n";
	WriteLogFile(g_pFileErrReal, strErrInfo);
}

///////////////////////////////////////////////////////////////////////////////
// U:13
void __fastcall TFormMain::SaveErrorToFile()
{
	FILE	*pFile = NULL;
	String	strPath = g_strAppPath + "\\output\\ACU_Error.log";
	pFile = fopen(strPath.c_str(), "a");
	if (pFile == NULL)
	{
		ShowMessage("Unable to open or create the ACU_Error.log file to record errorlog!");
		return ;
	}

	for (int i = 0; i < m_iErrLstCount; i++)
	{
		fputs(&(m_pLstbError->Items->Strings[i].operator[](1)), pFile);	// ȡ�ַ������ַ���1��ʼ
		fputc('\n', pFile);
	}

	fclose(pFile);
	m_pLstbError->Items->Clear();
	m_pLstbError->Height = 0;
	m_iErrLstCount = 0;
}

///////////////////////////////////////////////////////////////////////////////
// U:14 ���´��������Ϣ������������ɡ�ȫ��������ɣ���ǿ���˳���
// ������
// 		iState 0-����������� 1-ȫ��������ɣ���ǿ���˳���
// ˵����
//		Ϊ��һ���������¼�¼���ֵĹ�������׼��
void __fastcall TFormMain::ResetErrorInfo(int iState)
{
    if(0 == iState)
    {
    	for(int i = 0; i < MES_ERRORS_NUM; i++)
        {
        	m_iaErrItem[i] = 0;
        }
        m_iErrItemCount = 0;
    }
    else if(1 == iState)
    {
    	for(int i = 0; i < MES_ERRORS_NUM; i++)
        {
        	m_iaErrItem[i] = 0;
        }
        m_iErrItemCount = 0;
        m_pLstbError->Clear();
        m_iErrLstCount = 0;
    }
}

///////////////////////////////////////////////////////////////////////////////
// U:15 ���InfoBook����
void __fastcall TFormMain::ResetBookInfo()
{
	m_pF1BookACUInfo->ClearRange(2, 1, 2, 3, F1ClearValues);
}

///////////////////////////////////////////////////////////////////////////////
// U:15 ���InfoBook����
void __fastcall TFormMain::InitBookInfo()
{
	m_pF1BookACUInfo->SetSelection(2, 1, 2, 1);
	m_pF1BookACUInfo->Text = g_STestInfo.m_strInfo[0];
	m_pF1BookACUInfo->SetSelection(2, 2, 2, 2);
	m_pF1BookACUInfo->Text = g_STestInfo.m_strInfo[1];
	m_pF1BookACUInfo->SetSelection(2, 3, 2, 3);
	m_pF1BookACUInfo->Text = g_STestInfo.m_strInfo[2];
}

///////////////////////////////////////////////////////////////////////////////
// U:15 m_pF1BookTestItem����
void __fastcall TFormMain::ResetBookItem()
{
	// ���������ݺ͸�ʽ�仯����ʼ���
    for (int iItem = 1; iItem <= MES_ITEM_REAL; iItem++)
	{
		m_pF1BookTestItem->SetSelection(iItem, 1, iItem, 2);
		m_pF1BookTestItem->SetPattern(0, 0x000000, 0xff0000);
	}
}

///////////////////////////////////////////////////////////////////////////////
// U:16 �ı������
void __fastcall TFormMain::ChangeItem()
{
	ResetBookItem();

	if ((g_iCurrItem >= 0) && (g_iCurrItem < MES_ITEM_REAL))
	{
		m_pF1BookTestItem->SetActiveCell(g_iCurrItem + 1, 1);
		m_pF1BookTestItem->SetPattern(1, 0xFFFF00, 0xFF0000);
		m_pF1BookTestItem->ShowActiveCell();
	}
}

///////////////////////////////////////////////////////////////////////////////
// U:17 ����ReportBook
void __fastcall TFormMain::LoadBookReport()
{
}

///////////////////////////////////////////////////////////////////////////////
// U:18 ����ReportBook
void __fastcall TFormMain::SaveBookReport()
{
}

///////////////////////////////////////////////////////////////////////////////
// U:19 ����ReportBook������հױ�׼��ʽBook
void __fastcall TFormMain::ResetBookReport()
{
	// ����հױ�׼��ʽ���
    short iFileType = F1FileExcel4;
	m_pF1BookTestReport->Read((g_strAppPath + "\\config\\Rstd.vts").c_str(), iFileType);
	m_pF1BookTestReport->ShowEditBar = false;
}

///////////////////////////////////////////////////////////////////////////////
// U:20 ��ʼ��ReportBook�������벢���û�����Ϣ
void __fastcall TFormMain::InitBookReport()
{
	m_bWrite = false;

    // ��ʼ��FMRow��Ӧ������ͼ
	for (int i = 0; i < 9; i++)
	{
		for (int j = 0; j < MES_SUBBOARD_NUM; j++)
		{
			m_iaFMRow[i][j] = 800;
		}
	}

    // ��ʼ��ÿ�����貽���Ӧ����
	for (int j = 0; j < MES_ITEM_REAL; j++)
	{
		m_iaItem[j] = MES_ITEM_REAL + 4;
	}

	WriteSPECInTVSheet();
    WriteEcuIDInTVSheet();
    WriteTestInfoBaseRowInTVSheet();
    InitFMSheet();
    m_bWrite = true;
}

///////////////////////////////////////////////////////////////////////////////
// U:21 ���TotalSheet��Ϣ
void __fastcall TFormMain::ResetSheetTotal()
{
	int				iRowWritten = 2;
	short		   	iTemp = 0, iType = 0;	// iType��ʾ���ײ��߿��ߵ�����
	unsigned int	iColor = 0;				// ��ʾ�߿���ɫ
	int				iItem = 1;

	m_pF1BookTestReport->Sheet = MES_SHEET_TOTAL;

    // �ж�ACU_ID����������
	for (iItem = 1; iItem <= MES_ITEM_REAL; iItem++)
	{
		iRowWritten++;
		m_pF1BookTestReport->Sheet = MES_SHEET_TOTAL;
		m_pF1BookTestReport->SetSelection(iRowWritten, MES_T_ITEMCOL, iRowWritten, MES_T_ITEMCOL);
		m_pF1BookTestReport->GetBorder(iTemp, iTemp, iTemp, iType, iTemp, iColor, iColor, iColor, iColor);
		if (iType == 5)
		{
			iItem = 100;
		}
	}

	if (iItem == 80)
	{
    	g_strMark = "Table Total Wrong!";
		AddErrorByMark();
	} else
	{
		for (; iRowWritten > 5; iRowWritten--)
		{
        	// ɾ��ĩ���пհ�
			m_pF1BookTestReport->Sheet = MES_SHEET_TOTAL;
			m_pF1BookTestReport->SetSelection(iRowWritten - 1, MES_T_ITEMCOL, iRowWritten - 1, MES_T_ITEMCOL);
			m_pF1BookTestReport->EditDelete(F1ShiftRows);
		}

        // ɾ��ĩ���пհ�
		m_pF1BookTestReport->Sheet = MES_SHEET_TOTAL;
		m_pF1BookTestReport->SetSelection(3, MES_T_ITEMCOL, 5, MES_T_SAVGCOL);
		m_pF1BookTestReport->EditClear(F1ShiftRows);
	}
}

///////////////////////////////////////////////////////////////////////////////
// U:22 ���������дTotalSheet������ǰ���ȸ��ֱ��ƽ��ֵ��д���ܱ�����ܱ�
void __fastcall TFormMain::WriteSheetTotal()
{
	m_bWrite = true;

	int			iItem = 0, iSheet = 0, iCount3 = 0, iCount9 = 0, iRow = 2, iTemp = 0;
	double		dAvg3 = 0.0, dAvg9 = 0.0, dMin = 100000.0, dMax = 0.0, dTemp = 0.0;
	bool		bSign = false;
	String		strTemp = "", strItem = "", strStand = "", straData[9] = {0.0};

	ResetSheetTotal();
	for (iItem = 0; iItem < MES_ITEM_REAL; iItem++)
	{
    	if(iItem == MES_ITEM_CRASH)	continue;

		strItem = GetSPECInfo(iItem, 0);
		bSign = false;

        // ��ȡTVSheet�е�ƽ��ֵ
		for (iSheet = 2; iSheet <= 10; iSheet++)
		{
			straData[iSheet - 2] = ReadAvg(iSheet, strItem);
			if (straData[iSheet - 2] != "") bSign = true;
		}

		m_pF1BookTestReport->Sheet = MES_SHEET_TOTAL;
		if (bSign)
		{
        	// ȥ��û�����䷶Χ�Ĳ����� && (iItem <= 58) && (iItem >= 63)
            if ((iItem < MES_ITEM_REAL) && (iItem >= 0) && (iRow < MES_ITEM_REAL + 3))
			{
            	// ��¼����
				iRow++;
                // ���һ��
				m_pF1BookTestReport->Sheet = MES_SHEET_TOTAL;
				m_pF1BookTestReport->SetSelection(iRow + 1, MES_T_ITEMCOL, iRow + 1, MES_T_ITEMCOL);
				m_pF1BookTestReport->EditInsert(3);
                // ��дǰ����
				m_pF1BookTestReport->SetSelection(iRow, MES_T_ITEMCOL, iRow, MES_T_ITEMCOL);
				m_pF1BookTestReport->Text = strItem;
				strStand = GetSPECInfo(iItem, 1);
				m_pF1BookTestReport->SetSelection(iRow, MES_T_SPECCOL, iRow, MES_T_SPECCOL);
				m_pF1BookTestReport->Text = strStand;

                // ��ù淶��ȡֵ��Χ
				strTemp = strStand.SubString(1, strStand.Pos("~") - 1);
                if(strTemp.Length() != 0)
					dMin = strTemp.ToDouble();
				strTemp = strStand.SubString(strStand.Pos("~") + 1, strStand.Length() - strStand.Pos("~"));
				if (strTemp == "vbatt")
				{
					strTemp = "24";
				}

                if(strTemp.Length() != 0 && strTemp != " ")
				    dMax = strTemp.ToDouble();

				iCount3 = 0;
				iCount9 = 0;
				dAvg3 = 0.0;
				dAvg9 = 0.0;
                // ��дǰ����
				for (iTemp = 0; iTemp < 3; iTemp++)
				{
					m_pF1BookTestReport->SetSelection(iRow, iTemp + 3, iRow, iTemp + 3);
					m_pF1BookTestReport->SetFont("Arial", 10, 0, 0, 0, 0, 0x000000, 0, 0);
					m_pF1BookTestReport->Text = straData[iTemp];
					if (straData[iTemp] != "")
					{
						dTemp = straData[iTemp].ToDouble();
						if ((dTemp > dMax) || (dTemp < dMin))	m_pF1BookTestReport->SetFont("Arial", 10, 1, 0, 0, 0, 0x0000FF, 0, 0);

						iCount3++;
						iCount9++;
						dAvg3 = dAvg3 + straData[iTemp].ToDouble();
						dAvg9 = dAvg9 + straData[iTemp].ToDouble();
					}
				}

				if (iTemp > 0)
				{
					dAvg3 = dAvg3 / iCount3;
					WriteCellInTVSheet(iRow, MES_T_LAVGCOL, dAvg3);
					m_pF1BookTestReport->Sheet = MES_SHEET_TOTAL;
					m_pF1BookTestReport->SetSelection(iRow, MES_T_LAVGCOL, iRow, MES_T_LAVGCOL);
					if ((dAvg3 > dMax) || (dAvg3 < dMin))	m_pF1BookTestReport->SetFont("Arial", 10, 1, 0, 0, 0, 0x0000FF, 0, 0);
					else									m_pF1BookTestReport->SetFont("Arial", 10, 0, 0, 0, 0, 0x000000, 0, 0);
				}

				iTemp = 0;
				dAvg3 = 0.0;
                // ��д������
				for (iTemp = 3; iTemp < 6; iTemp++)
				{
					m_pF1BookTestReport->Sheet = MES_SHEET_TOTAL;
					m_pF1BookTestReport->SetSelection(iRow, iTemp + 4, iRow, iTemp + 4);
					m_pF1BookTestReport->Text = straData[iTemp];
					m_pF1BookTestReport->SetFont("Arial", 10, 0, 0, 0, 0, 0x000000, 0, 0);
					if (straData[iTemp] != "")
					{
						dTemp = straData[iTemp].ToDouble();
						if ((dTemp > dMax) || (dTemp < dMin))
						{
							m_pF1BookTestReport->SetFont("Arial", 10, 1, 0, 0, 0, 0x0000FF, 0, 0);
						}

						iCount3++;
						iCount9++;
						dAvg3 = dAvg3 + straData[iTemp].ToDouble();
						dAvg9 = dAvg9 + straData[iTemp].ToDouble();
					}
				}

				if (iCount3 > 0)
				{
					dAvg3 = dAvg3 / iCount3;
					WriteCellInTVSheet(iRow, MES_T_MAVGCOL, dAvg3);
				}

				m_pF1BookTestReport->SetSelection(iRow, MES_T_MAVGCOL, iRow, MES_T_MAVGCOL);
				if ((dAvg3 > dMax) || (dAvg3 < dMin))	m_pF1BookTestReport->SetFont("Arial", 10, 1, 0, 0, 0, 0x0000FF, 0, 0);
				else									m_pF1BookTestReport->SetFont("Arial", 10, 0, 0, 0, 0, 0x000000, 0, 0);

				iTemp = 0;
				dAvg3 = 0.0;
                // ��д������
				for (iTemp = 6; iTemp < 9; iTemp++)
				{
					m_pF1BookTestReport->Sheet = MES_SHEET_TOTAL;
					m_pF1BookTestReport->SetSelection(iRow, iTemp + 5, iRow, iTemp + 5);
					m_pF1BookTestReport->Text = straData[iTemp];
					m_pF1BookTestReport->SetFont("Arial", 10, 0, 0, 0, 0, 0x000000, 0, 0);
					if (straData[iTemp] != "")
					{
						dTemp = straData[iTemp].ToDouble();
                        m_pF1BookTestReport->Sheet = MES_SHEET_TOTAL;
						if ((dTemp > dMax) || (dTemp < dMin))
							m_pF1BookTestReport->SetFont("Arial", 10, 1, 0, 0, 0, 0x0000FF, 0, 0);

						iCount3++;
						iCount9++;
						dAvg3 = dAvg3 + straData[iTemp].ToDouble();
						dAvg9 = dAvg9 + straData[iTemp].ToDouble();
					}
				}

				if (iCount3 > 0)
				{
					dAvg3 = dAvg3 / iCount3;
					WriteCellInTVSheet(iRow, MES_T_HAVGCOL, dAvg3);
				}

				m_pF1BookTestReport->SetSelection(iRow, MES_T_HAVGCOL, iRow, MES_T_HAVGCOL);
				if ((dAvg3 > dMax) || (dAvg3 < dMin))	m_pF1BookTestReport->SetFont("Arial", 10, 1, 0, 0, 0, 0x0000FF, 0, 0);
				else									m_pF1BookTestReport->SetFont("Arial", 10, 0, 0, 0, 0, 0x000000, 0, 0);

				dAvg9 = dAvg9 / iCount9;
				WriteCellInTVSheet(iRow, MES_T_SAVGCOL, dAvg9);
				m_pF1BookTestReport->Sheet = MES_SHEET_TOTAL;
				m_pF1BookTestReport->SetSelection(iRow, MES_T_SAVGCOL, iRow, MES_T_SAVGCOL);
				if ((dAvg9 > dMax) || (dAvg9 < dMin))	m_pF1BookTestReport->SetFont("Arial", 10, 1, 0, 0, 0, 0x0000FF, 0, 0);
				else									m_pF1BookTestReport->SetFont("Arial", 10, 0, 0, 0, 0, 0x000000, 0, 0);
			} else
			{
				;
			}
		}
	}

	if ((iRow > 4) && (iRow < MES_ITEM_REAL + 3))
	{
		m_pF1BookTestReport->SetSelection(iRow - 1, MES_T_ITEMCOL, iRow, MES_T_SAVGCOL);
		m_pF1BookTestReport->EditCopy();
		m_pF1BookTestReport->SetSelection(iRow + 2, MES_T_ITEMCOL, iRow + 3, MES_T_SAVGCOL);
		m_pF1BookTestReport->EditPaste();
		m_pF1BookTestReport->SetSelection(iRow + 3, MES_T_ITEMCOL, iRow + 3, MES_T_SAVGCOL);
		m_pF1BookTestReport->SetBorder(-1, -1, -1, -1, 5, 0, 0, 0, 0, 0, 0);
        // ɾ��ĩ���пհ�
		m_pF1BookTestReport->SetSelection(iRow - 1, MES_T_ITEMCOL, iRow - 1, MES_T_ITEMCOL);
		m_pF1BookTestReport->EditDelete(3);
        // ɾ��ĩ���пհ�
		m_pF1BookTestReport->SetSelection(iRow - 1, MES_T_ITEMCOL, iRow - 1, MES_T_ITEMCOL);
		m_pF1BookTestReport->EditDelete(3);
        // ɾ��ĩ���пհ�
		m_pF1BookTestReport->SetSelection(iRow - 1, MES_T_ITEMCOL, iRow - 1, MES_T_ITEMCOL);
		m_pF1BookTestReport->EditDelete(3);
	} else if (iRow == 4)
	{
    	// ɾ��ĩ���пհ�
		m_pF1BookTestReport->SetSelection(iRow + 1, MES_T_ITEMCOL, iRow + 1, MES_T_ITEMCOL);
		m_pF1BookTestReport->EditDelete(3);
		m_pF1BookTestReport->SetSelection(iRow + 1, MES_T_ITEMCOL, iRow + 1, MES_T_ITEMCOL);
		m_pF1BookTestReport->EditDelete(3);
	} else if (iRow == 3)
	{
    	// ɾ��ĩ���пհ�
		m_pF1BookTestReport->SetSelection(iRow + 1, MES_T_ITEMCOL, iRow + 1, MES_T_ITEMCOL);
		m_pF1BookTestReport->EditDelete(3);
	} else
	{
		Application->MessageBox("Total Sheet do not write yet!", "Notice", MB_OK);
	}

	m_bWrite = false;
}

///////////////////////////////////////////////////////////////////////////////
// U:23 ��¼���ֵĹ��ϣ���дErrLogSheet
// strErrMarkΪ�ַ�������Ӧ��ʽΪ��
// 2006-09-23  21:18:46  [-88.88 'C -88.88 V]  [ACU 8]  [0x19]  [Crash Output STG]
void __fastcall TFormMain::WriteErrorSheet(String strErrMark)
{
	m_bWrite = true;
	String	strDate = "", strTime = "", strTemp = "", strVolt = "", strEcu = "", strCode = "", strMark = "";

	// �жϽṹ�Ƿ���ȷ [Crash Output STG]�е�C��λ����63
	if (strErrMark.Length() > 63)
	{
		String	strTemp = "start";
		String	strECUNum = strErrMark.SubString(50, 1);

		// �ж�ACU��
		if ((strECUNum == "0") || (strECUNum == "1") || (strECUNum == "2") || (strECUNum == "3") || (strECUNum == "4") ||
			(strECUNum == "5"))
		{
			// �ֽ⿪�����ַ���
			strDate = strErrMark.SubString(1, 10);
			strTime = strErrMark.SubString(13, 8);
			strTemp = strErrMark.SubString(24, 9);
			strVolt = strErrMark.SubString(34, 8);
			strEcu = strErrMark.SubString(46, 5);
			strCode = strErrMark.SubString(55, 4);
			strMark = strErrMark.SubString(63, strErrMark.Length() - 63);

			// ��¼Smode_on fail����ز�����Ϣ
			if (strMark == "Smode_on fail")
			{
				String	strItemName = "";
				if ((g_iCurrItem >= 0) && (g_iCurrItem < MES_ITEM_RET))
				{
					m_pF1BookTestItem->SetSelection(g_iCurrItem + 1, 1, g_iCurrItem + 1, 1);
					strItemName = m_pF1BookTestItem->Text;
				} else strItemName = "ItemError";
				strMark = strMark + "   [ItemName: " + strItemName + "]";
			}

			// ��д���ϱ��
            m_siRowErrorSheet++;
			m_pF1BookTestReport->Sheet = MES_SHEET_ERRLOG;
			if (m_siRowErrorSheet > 4)
			{
				m_pF1BookTestReport->Sheet = MES_SHEET_ERRLOG;
				m_pF1BookTestReport->SetSelection(m_siRowErrorSheet - 1, 1, m_siRowErrorSheet, 7);	// �����ʽ
				m_pF1BookTestReport->EditClear(F1ClearFormats);

				m_pF1BookTestReport->Sheet = MES_SHEET_ERRLOG;
				m_pF1BookTestReport->SetSelection(m_siRowErrorSheet - 1, 1, m_siRowErrorSheet, 7);	// ����ϸ�߿���
				m_pF1BookTestReport->SetBorder(-1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0);

				m_pF1BookTestReport->Sheet = MES_SHEET_ERRLOG;
				m_pF1BookTestReport->SetSelection(m_siRowErrorSheet - 1, 1, m_siRowErrorSheet, 1);	// ���ô����߿���
				m_pF1BookTestReport->SetBorder(-1, 5, -1, -1, -1, 0, 0, 0, 0, 0, 0);

				m_pF1BookTestReport->Sheet = MES_SHEET_ERRLOG;
				m_pF1BookTestReport->SetSelection(m_siRowErrorSheet - 1, 7, m_siRowErrorSheet, 7);	// ���ô��Ҳ�߿���
				m_pF1BookTestReport->SetBorder(-1, -1, 5, -1, -1, 0, 0, 0, 0, 0, 0);

				m_pF1BookTestReport->Sheet = MES_SHEET_ERRLOG;
				m_pF1BookTestReport->SetSelection(m_siRowErrorSheet, 1, m_siRowErrorSheet, 7);		// ���ôֵײ�߿���
				m_pF1BookTestReport->SetBorder(-1, -1, -1, -1, 5, 0, 0, 0, 0, 0, 0);

				m_pF1BookTestReport->Sheet = MES_SHEET_ERRLOG;
				m_pF1BookTestReport->SetSelection(m_siRowErrorSheet - 1, 1, m_siRowErrorSheet, 7);	// ���þ��еȷ�ʽ
				m_pF1BookTestReport->SetAlignment(3, 0, 3, 0);
			}

			m_pF1BookTestReport->Sheet = MES_SHEET_ERRLOG;
			m_pF1BookTestReport->SetSelection(m_siRowErrorSheet, 1, m_siRowErrorSheet, 1);
			m_pF1BookTestReport->SetFont("Arial", 10, 0, 0, 0, 0, 0x000000, 0, 0);
			m_pF1BookTestReport->Text = strDate;

			m_pF1BookTestReport->Sheet = MES_SHEET_ERRLOG;
			m_pF1BookTestReport->SetSelection(m_siRowErrorSheet, 2, m_siRowErrorSheet, 2);
			m_pF1BookTestReport->SetFont("Arial", 10, 0, 0, 0, 0, 0x000000, 0, 0);
			m_pF1BookTestReport->Text = strTime;

			m_pF1BookTestReport->Sheet = MES_SHEET_ERRLOG;
			m_pF1BookTestReport->SetSelection(m_siRowErrorSheet, 3, m_siRowErrorSheet, 3);
			m_pF1BookTestReport->SetFont("Arial", 10, 0, 0, 0, 0, 0x000000, 0, 0);
			m_pF1BookTestReport->Text = strTemp;

			m_pF1BookTestReport->Sheet = MES_SHEET_ERRLOG;
			m_pF1BookTestReport->SetSelection(m_siRowErrorSheet, 4, m_siRowErrorSheet, 4);
			m_pF1BookTestReport->SetFont("Arial", 10, 0, 0, 0, 0, 0x000000, 0, 0);
			m_pF1BookTestReport->Text = strVolt;

			m_pF1BookTestReport->Sheet = MES_SHEET_ERRLOG;
			m_pF1BookTestReport->SetSelection(m_siRowErrorSheet, 5, m_siRowErrorSheet, 5);
			m_pF1BookTestReport->SetFont("Arial", 10, 0, 0, 0, 0, 0x000000, 0, 0);
			m_pF1BookTestReport->Text = strEcu;

			m_pF1BookTestReport->Sheet = MES_SHEET_ERRLOG;
			m_pF1BookTestReport->SetSelection(m_siRowErrorSheet, 6, m_siRowErrorSheet, 6);
			m_pF1BookTestReport->SetFont("Arial", 10, 0, 0, 0, 0, 0x000000, 0, 0);
			m_pF1BookTestReport->Text = strCode;

			m_pF1BookTestReport->Sheet = MES_SHEET_ERRLOG;
			m_pF1BookTestReport->SetSelection(m_siRowErrorSheet, 7, m_siRowErrorSheet, 7);
			m_pF1BookTestReport->SetFont("Arial", 10, 0, 0, 0, 0, 0x000000, 0, 0);
			m_pF1BookTestReport->Text = strMark;
            m_pF1BookTestReport->ShowActiveCell();
		}
	} else
	{
		g_strMark = "ErrLog Wrong.!";
		AddErrorByMark();
	}

	m_bWrite = false;
}

///////////////////////////////////////////////////////////////////////////////
// U:24 ��TVSheet����д������
void __fastcall TFormMain::WriteErrorCol()
{
	bool		bFound = false;
	String		strErrCode = "", strErrRemark = "";
    bFound = GetFaultInfo(g_iCurrFaultCode, strErrCode, strErrRemark);

	m_pF1BookTestReport->Sheet = g_iTVCond + MES_SHEET_TV;
    int iRow = m_iaItem[g_iCurrItem];
    m_pF1BookTestReport->SetSelection(iRow, MES_TV_ERRCOL + g_iCurrBoard, iRow, MES_TV_ERRCOL + g_iCurrBoard);
    m_pF1BookTestReport->SetFont("Arial", 10, 0, 0, 0, 0, 0x000000, 0, 0);

    if(true == bFound)	m_pF1BookTestReport->Text = m_pF1BookTestReport->Text + " " + strErrCode;
    else                m_pF1BookTestReport->Text = m_pF1BookTestReport->Text + " 0xFF";
    m_pF1BookTestReport->ShowActiveCell();

    m_strItemError = m_pF1BookTestReport->Text;
}

///////////////////////////////////////////////////////////////////////////////
// U:25 ��дTempSheet OK
void __fastcall TFormMain::WriteTempSheet()
{
	m_bWrite = true;
	int iSheetTemp = 1;
	String	strTime = TimeToStr(Time());

	if ((g_iTempRow < 16384) && (g_iTempRow >= 1))
	{
		String	strTemp = FloatToStr(g_fTempCurveY);
		if ((g_fTempCurveY > -0.01) && (g_fTempCurveY < 0.01))
        {
        	strTemp = "0";
        }
		else if (strTemp.Pos(".") == 0)
		{
			strTemp = strTemp.SubString(1, 4);
		} else
        {
        	strTemp = strTemp.SubString(1, strTemp.Pos(".") + 2);
        }

		iSheetTemp = m_pF1BookTestReport->Sheet;
		g_iTempRow++;
		m_pF1BookTestReport->Sheet = MES_SHEET_TEMP;
		m_pF1BookTestReport->SetSelection(g_iTempRow, 1, g_iTempRow, 1);
		m_pF1BookTestReport->Text = strTime;
		m_pF1BookTestReport->SetSelection(g_iTempRow, 2, g_iTempRow, 2);
		m_pF1BookTestReport->Text = strTemp;
		if (m_bTempBoard == true)
		{
			m_pF1BookTestReport->Sheet = MES_SHEET_TEMP;
			m_pF1BookTestReport->SetSelection(g_iTempRow, 1, g_iTempRow, 2);
			m_pF1BookTestReport->SetFont("Arial", 10, 0, 0, 0, 0, 0x00B000, 0, 0);
			m_pF1BookTestReport->SetAlignment(3, 0, 3, 0);
		} else
		{
			m_pF1BookTestReport->Sheet = MES_SHEET_TEMP;
			m_pF1BookTestReport->SetSelection(g_iTempRow, 1, g_iTempRow, 2);
			m_pF1BookTestReport->SetFont("Arial", 10, 0, 0, 0, 0, 0x000000, 0, 0);
			m_pF1BookTestReport->SetAlignment(3, 0, 3, 0);
		}

		m_pF1BookTestReport->Sheet = iSheetTemp;
	}

	m_bWrite = false;
}

//////////////////////////////////////////////////////////////////////////////
// U:26 ����FMSheet��ܣ�m_iTVCond m_iSelACU
void __fastcall TFormMain::InitFMSheet()
{
	m_bWrite = true;
	int iCondCurr = 0, iBoardCurr = 0, iFMRow = 1;

	for (int iTVCond = 0; iTVCond < 9; iTVCond++)
	{
		iCondCurr = g_STestInfo.m_iTVCond[iTVCond];
		for (int iBoard = 0; iBoard < MES_SUBBOARD_NUM; iBoard++)
		{
			iBoardCurr = (g_STestInfo.m_iSelACU >> iBoard) & 0x01;
			if ((iCondCurr != 1) || (iBoardCurr != 1)) continue;

			if (iFMRow < 11)
			{
				m_pF1BookTestReport->Sheet = MES_SHEET_FM;
				m_pF1BookTestReport->SetSelection(1, 1, 11, 2);
				m_pF1BookTestReport->EditCopy();
			} else
			{
				m_pF1BookTestReport->Sheet = MES_SHEET_FM;
				m_pF1BookTestReport->SetSelection(iFMRow, 1, iFMRow + 10, 2);
				m_pF1BookTestReport->EditPaste();
			}

			m_pF1BookTestReport->Sheet = MES_SHEET_FM;
			m_pF1BookTestReport->SetSelection(iFMRow, 2, iFMRow, 2);
			m_pF1BookTestReport->Text = "Temp: " + g_STestInfo.m_strTemp[iTVCond / 3] + "��, Volt: " +
				g_STestInfo.m_strVolt[iTVCond % 3] + "V, ACU " + IntToStr(iBoard + 1);
			m_pF1BookTestReport->SetFont("Arial", 10, 0, 0, 0, 0, 0x000000, 0, 0);
			m_iaFMRow[iTVCond][iBoard] = iFMRow;
			iFMRow = iFMRow + 11;
		}
	}
    m_pF1BookTestReport->Update();
    m_bWrite = false;
}

//////////////////////////////////////////////////////////////////////////////
// U:27
void __fastcall TFormMain::WriteSmodeInFMSheet()
{
	m_bWrite = true;
	int iFMRow = 0;
	if ((g_iCurrBoard < 6) && (g_iCurrBoard >= 0) && (g_iTVCond < 9) && (g_iTVCond >= 0))
	{
		iFMRow = m_iaFMRow[g_iTVCond][g_iCurrBoard] + 1;

		String	strTemp = "Smode:" + g_strMark;
		m_pF1BookTestReport->Sheet = MES_SHEET_FM;
		m_pF1BookTestReport->SetSelection(iFMRow, 2, iFMRow, 2);
		m_pF1BookTestReport->SetFont("Arial", 8, 0, 0, 0, 0, 0x000000, 0, 0);
		m_pF1BookTestReport->Text = strTemp;
        m_pF1BookTestReport->ShowActiveCell();
	}

	m_pF1BookTestReport->Update();
	m_bWrite = false;
}

//////////////////////////////////////////////////////////////////////////////
// U:28
void __fastcall TFormMain::WriteFMRInFMSheet()
{
	m_bWrite = true;
    int iFMRow = 0;
	if ((g_iCurrBoard < 6) && (g_iCurrBoard >= 0) && (g_iTVCond < 10) && (g_iTVCond >= 0) && (g_iCRC < 3) && (g_iCRC >= 0))
	{
		iFMRow = m_iaFMRow[g_iTVCond][g_iCurrBoard] + g_iCRC*3 + 2;

		String	strTemp = g_strMark.SubString(1, MES_BYTES);
		m_pF1BookTestReport->Sheet = MES_SHEET_FM;
        //m_pF1BookTestReport->ShowTabs = F1TabsBottom;
        //m_pF1BookTestReport->ShowHScrollBar = F1Off;

		m_pF1BookTestReport->SetSelection(iFMRow, 2, iFMRow, 2);
		m_pF1BookTestReport->SetFont("Arial", 8, 0, 0, 0, 0, 0x000000, 0, 0);
		m_pF1BookTestReport->Text = strTemp;
        m_pF1BookTestReport->ShowActiveCell();

		strTemp = g_strMark.SubString(MES_BYTES + 1, MES_BYTES);
		m_pF1BookTestReport->Sheet = MES_SHEET_FM;
		m_pF1BookTestReport->SetSelection(iFMRow + 1, 2, iFMRow + 1, 2);
		m_pF1BookTestReport->SetFont("Arial", 8, 0, 0, 0, 0, 0x000000, 0, 0);
		m_pF1BookTestReport->Text = strTemp;
        m_pF1BookTestReport->ShowActiveCell();
	}

	m_pF1BookTestReport->Update();
	m_bWrite = false;
}

//////////////////////////////////////////////////////////////////////////////
// U:29
void __fastcall TFormMain::WriteFRCInFMSheet()
{
	m_bWrite = true;
    int iFMRow = 0;
	if ((g_iCurrBoard < 6) && (g_iCurrBoard >= 0) && (g_iTVCond < 9) && (g_iTVCond >= 0) && (g_iCRC < 3) && (g_iCRC >= 0))
	{
		iFMRow = m_iaFMRow[g_iTVCond][g_iCurrBoard] + g_iCRC*3 + 4;

		String	strTemp = "";
		if (g_bOpState == true) strTemp = "ALL F";
		else strTemp = "ERROR";
		m_pF1BookTestReport->Sheet = MES_SHEET_FM;
		m_pF1BookTestReport->SetSelection(iFMRow, 2, iFMRow, 2);
		m_pF1BookTestReport->Text = strTemp;
		m_pF1BookTestReport->ShowActiveCell();
	}

	m_pF1BookTestReport->Update();
	m_bWrite = false;
}

// ĳһ��������ɺ����
void __fastcall TFormMain::WriteItemTestData()
{
	m_bWrite = true;
    String	strItemValue = g_strItemValue, strJudge = "";
    double	dItemValue = 0.0;
    if (g_iCurrItem != 73) dItemValue = strItemValue.ToDouble();
      // ������ȡС��λ��1000����Ϊ�ޣ�100����Ϊ1λ������Ϊ2λ
	if (dItemValue > 1000)
	{
    	int iPos = strItemValue.Pos('.');
        if(0 == iPos)
        	strItemValue = strItemValue.SubString(1, strItemValue.Length());
        else
        	strItemValue = strItemValue.SubString(1, strItemValue.Pos('.') - 1);
		dItemValue = strItemValue.ToDouble();
	} else if (dItemValue > 10)
	{
		strItemValue = strItemValue.SubString(1, 5);
		dItemValue = strItemValue.ToDouble();
	} else if (dItemValue > 0.01)
	{
		strItemValue = strItemValue.SubString(1, 4);
		dItemValue = strItemValue.ToDouble();
	}
//--START--------------------2009-08-13------------------//
    else  if(dItemValue < -0.01)
	{

		strItemValue = strItemValue.SubString(1, 4);
		dItemValue = strItemValue.ToDouble();
	}
//--END--------------------2009-08-13------------------//
    else
	{
		strItemValue = "0";
		dItemValue = strItemValue.ToDouble();
	}

    // С�ڱ�׼ACU����״̬���Ͳ�����
	if ((g_iCurrBoard < MES_SUBBOARD_NUM) && (g_iCurrBoard >= 0) && (g_iTVCond < 9) && (g_iTVCond >= 0) &&
    	(g_iCurrItem >= 0) && (g_iCurrItem < MES_ITEM_REAL))
	{
		if (g_iOpMode == 1)
		{	// �˹���ʽ
			m_pF1BookTestItem->SetSelection(g_iCurrItem + 1, 2, g_iCurrItem + 1, 2);
			m_pF1BookTestItem->Text = "  " + strItemValue + "  [" + m_strItemError + "]";
		} else
		{  	// �Զ���ʽ �ж��Ƿ��д�����
        	int iSheet = g_iTVCond + MES_SHEET_TV;
            int iRow = m_iaItem[g_iCurrItem];
            if ((g_iCurrItem >= 0)&& (g_iCurrItem != MES_ITEM_CRASH))
			{
                iRow = m_iaItem[g_iCurrItem];
				m_pF1BookTestReport->Sheet = iSheet;
				WriteCellInTVSheet(iRow, g_iCurrBoard + MES_SHEET_TV + 1, dItemValue);
                m_pF1BookTestReport->ShowActiveCell();
				m_pF1BookTestReport->Sheet = iSheet;
				WriteStatInTVSheet(iRow);

				m_pF1BookTestReport->Sheet = iSheet;
				WriteJudgeInTVSheet(iRow, g_iCurrBoard + MES_SHEET_TV + 1);
                m_pF1BookTestReport->ShowActiveCell();

				m_pF1BookTestReport->Sheet = iSheet;
				WriteJudgeInTVSheet(iRow, MES_TV_MINCOL);
                m_pF1BookTestReport->ShowActiveCell();

				m_pF1BookTestReport->Sheet = iSheet;
				WriteJudgeInTVSheet(iRow, MES_TV_MAXCOL);
                m_pF1BookTestReport->ShowActiveCell();

				m_pF1BookTestReport->Sheet = iSheet;
				WriteJudgeInTVSheet(iRow, MES_TV_AVGCOL);
                m_pF1BookTestReport->ShowActiveCell();
			} else
			{  	// Crash����
                WriteCrash();
			}

			m_pF1BookTestReport->SetSelection(iRow, MES_TV_JUDGECOL, iRow, MES_TV_JUDGECOL);
            m_pF1BookTestReport->ShowActiveCell();
			strJudge = m_pF1BookTestReport->Text;
		}
	} else
	{
    	g_strMark = "ACUNum or SheetNum Wrong!";
		AddErrorByMark();
	}
	m_bWrite = false;
}

//////////////////////////////////////////////////////////////////////////////
// U:30
// ����:
//		iRow ���� 1-74
//		iCol ����
//		dValue д�����ֵ��Ϊ˫���ȸ�������������дʱ������ȡС��λ��
//  		1000����Ϊ�ޣ�100����Ϊ1λ������Ϊ2λ��
void __fastcall TFormMain::WriteCellInTVSheet(int iRow, int iCol, double dValue)
{
	String	strdValue = "";
    // ��ʽ������
	if (dValue > 1000.0)
	{
		strdValue = (String) dValue;
        int iPos = strdValue.Pos('.');
        if(0 == iPos)
        	strdValue = strdValue.SubString(1, strdValue.Length());
        else
        	strdValue = strdValue.SubString(1, strdValue.Pos('.') - 1);
		dValue = strdValue.ToDouble();
	} else if (dValue > 10)
	{
		strdValue = (String) dValue;
		strdValue = strdValue.SubString(1, 5);
		dValue = strdValue.ToDouble();
	} else if (dValue > 0.01)
	{
		strdValue = (String) dValue;
		strdValue = strdValue.SubString(1, 4);
		dValue = strdValue.ToDouble();
	}
//--START--------------------2009-08-13------------------//
     else if (dValue < -0.01)
	{
        if (dValue <-256)
             dValue = 0;
		strdValue = (String) dValue;
		strdValue = strdValue.SubString(1, 4);
		dValue = strdValue.ToDouble();
	}
//--END--------------------2009-08-13------------------//
    else
	{
		strdValue = "0";
		dValue = strdValue.ToDouble();
	}

    // ��д���ݲ�����
	m_pF1BookTestReport->SetSelection(iRow, iCol, iRow, iCol);
	m_pF1BookTestReport->Text = String(dValue);
    m_pF1BookTestReport->ShowActiveCell();
}

//////////////////////////////////////////////////////////////////////////////
// U:31
// ������
// 		iRow ���� 1-74
// ˵����
//		��д��ǰҳ�Ķ�Ӧ�еĲ������ݵ����ֵ����Сֵ����ƽ��ֵ��
//		��дʱ������ȡС��λ��1000����Ϊ�ޣ�100����Ϊ1λ������Ϊ2λ��
void __fastcall TFormMain::WriteStatInTVSheet(int iRow)
{
	double		dMin = 100000.0, dMax = 0.0, dAvg = 0.0, dTotal = 0.0, dTemp = 0.0;
	int			iSheetSum = 0;
	String		strTemp = "";

    // ��ȡ��ĳһ�����Բ����µĸ�ACU����ֵ
	for (int iSheet = 0; iSheet < MES_SUBBOARD_NUM; iSheet++)
	{
		m_pF1BookTestReport->SetSelection(iRow, 3 + iSheet, iRow, 3 + iSheet);
		strTemp = m_pF1BookTestReport->Text;
		if (strTemp != "")
		{
			dTemp = strTemp.ToDouble();
			dTotal = dTemp + dTotal;
			iSheetSum++;

			if (dTemp > dMax) dMax = dTemp;
			if (dTemp < dMin) dMin = dTemp;
		}
	}

    // �����ݽ��и�ʽ��
	dAvg = dTotal / iSheetSum;
	String	strAvg = "";
	if (dAvg > 1000.0)
	{
		strAvg = (String) dAvg;
        int iPos = strAvg.Pos('.');
        if(0 == iPos)
        	strAvg = strAvg.SubString(1, strAvg.Length());
        else
        	strAvg = strAvg.SubString(1, strAvg.Pos('.') - 1);

		dAvg = strAvg.ToDouble();
	} else if (dAvg > 10)
	{
		strAvg = (String) dAvg;
		strAvg = strAvg.SubString(1, 5);
		dAvg = strAvg.ToDouble();
	} else if (dAvg > 0.01)
	{
		strAvg = (String) dAvg;
		strAvg = strAvg.SubString(1, 4);
		dAvg = strAvg.ToDouble();
	} else
	{
		strAvg = "0";
		dAvg = strAvg.ToDouble();
	}

	WriteCellInTVSheet(iRow, MES_TV_MINCOL, dMin);
	WriteCellInTVSheet(iRow, MES_TV_MAXCOL, dMax);
	WriteCellInTVSheet(iRow, MES_TV_AVGCOL, dAvg);
}

//////////////////////////////////////////////////////////////////////////////
// U:32
// ����:
//		iRow ���� 1-74
//		iCol ����
// ˵����
// 		�ж϶�Ӧcell�����Ƿ��������ֵ�ķ�Χ������д"fail""ok"�������ݶ�Ӧ��
//		��"judgement"����
void __fastcall TFormMain::WriteJudgeInTVSheet(int iRow, int iCol)
{
	double	dMaxSpec = 0.0, dMinSpec = 0.0, dTemp = 0.0;
	String	strTemp = "", strSpec = "";

	m_pF1BookTestReport->SetSelection(iRow, iCol, iRow, iCol);
	strTemp = m_pF1BookTestReport->Text;
	if (strTemp == "") return;

	dTemp = strTemp.ToDouble();
	m_pF1BookTestReport->SetSelection(iRow, MES_T_SPECCOL, iRow, MES_T_SPECCOL);
	strSpec = m_pF1BookTestReport->Text;

	strTemp = strSpec.SubString(1, strSpec.Pos("~") - 1);
	dMinSpec = strTemp.ToDouble();
	strTemp = strSpec.SubString(strSpec.Pos("~") + 1, strSpec.Length() - strSpec.Pos("~"));
	if (strTemp == "vbatt")
	{
		strTemp = "24.0";
	}
	dMaxSpec = strTemp.ToDouble();

	if ((dTemp > dMaxSpec) || (dTemp < dMinSpec))
	{
		m_pF1BookTestReport->SetSelection(iRow, iCol, iRow, iCol);
		m_pF1BookTestReport->SetFont("Arial", 10, 1, 0, 0, 0, 0x0000FF, 0, 0);
		m_pF1BookTestReport->SetSelection(iRow, MES_TV_JUDGECOL, iRow, MES_TV_JUDGECOL);
		m_pF1BookTestReport->SetFont("Arial", 10, 1, 0, 0, 0, 0x0000FF, 0, 0);
		m_pF1BookTestReport->Text = "failed";
		m_pF1BookTestReport->ShowActiveCell();
	} else
	{
		m_pF1BookTestReport->SetSelection(iRow, iCol, iRow, iCol);
		m_pF1BookTestReport->SetFont("Arial", 10, 0, 0, 0, 0, 0x000000, 0, 0);
		m_pF1BookTestReport->SetSelection(iRow, MES_TV_JUDGECOL, iRow, MES_TV_JUDGECOL);
		strTemp = m_pF1BookTestReport->Text;
		if (strTemp != "failed")
		{
			m_pF1BookTestReport->SetFont("Arial", 10, 1, 0, 0, 0, 0x000000, 0, 0);
			m_pF1BookTestReport->Text = "ok";
		}

		m_pF1BookTestReport->ShowActiveCell();
	}
}

//////////////////////////////////////////////////////////////////////////////
// U:33 ��дBaseTestInfo���ڸ���TVSheet testName testModel testRemark
void __fastcall TFormMain::WriteTestInfoBaseRowInTVSheet()
{
	m_bWrite = true;
    int		iRow = 0;
    String strTestName = g_STestInfo.m_strInfo[0];
    String strTestModel	= g_STestInfo.m_strInfo[1];
    String strTestRemark = g_STestInfo.m_strInfo[2];
/*
	m_pF1BookTestReport->SetSelection(2, 1, 2, 1);
	m_pF1BookTestReport->Text = strTestName;
	m_pF1BookTestReport->SetSelection(2, 2, 2, 2);
	m_pF1BookTestReport->Text = strTestModel;
	m_pF1BookTestReport->SetSelection(2, 3, 2, 3);
	m_pF1BookTestReport->Text = strTestRemark;
*/
    // �����б�ͷ
	for (int iSheet = MES_SHEET_TV; iSheet <= 10; iSheet++)
	{
		m_pF1BookTestReport->Sheet = iSheet;
        GetRowCountInTVSheet(iSheet, iRow);
		m_pF1BookTestReport->SetSelection(iRow + 1, 2, iRow + 1, 2);
		m_pF1BookTestReport->Text = strTestName + " - " + strTestModel + " - " + strTestRemark;
	}

	m_bWrite = false;
}

//////////////////////////////////////////////////////////////////////////////
// U:34 ��дTestInfo���ڸ���TVSheet������дTVSheet��"ACU Information"Row������
void __fastcall TFormMain::WriteTestInfoRowInTVSheet()
{
    m_bWrite = true;
    String 	strTempTarget = "", strVoltTarget = "", strTempInit = "", strVoltInit = "", strTempEnd = "", strVoltEnd = "";
    int 	iSheet = 1;

    strTempTarget = FloatToStr(g_fTempTarget);
    strVoltTarget = FloatToStr(g_fVoltTarget);
    strTempInit = FloatToStr(g_fTempInit);
    strVoltInit = FloatToStr(g_fVoltInit);

    int iPos = strVoltInit.Pos('.');
    if(0 == iPos)
    	strVoltInit = strVoltInit.SubString(1, strVoltInit.Length());
    else
    	strVoltInit = strVoltInit.SubString(1, strVoltInit.Pos('.') + 2);

	if ((g_iTVCond < 9) && (g_iTVCond >= 0))
	{
		strTempEnd = FloatToStr(g_fTempCurveY);
		if ((g_fTempCurveY > -0.01) && (g_fTempCurveY < 0.01)) strTempEnd = "0";
		else if (strTempEnd.Pos(".") != 0)
		{
			strTempEnd = strTempEnd.SubString(1, strTempEnd.Pos(".") + 2);
		}

        strVoltEnd = FloatToStr(g_fVoltCurr);
        if(0 == iPos)
    		strVoltEnd = strVoltEnd.SubString(1, strVoltEnd.Length());
    	else
    		strVoltEnd = strVoltEnd.SubString(1, strVoltEnd.Pos('.') + 2);

		iSheet = g_iTVCond + MES_SHEET_TV;
        if(m_bTVState == true)
        	m_strInfo = DateToStr(Date()) + "  " + TimeToStr(Time()) +
        		" Temp(Set " + strTempTarget + "��, Start " + strTempInit + "��" + ", Finish " + strTempEnd + "��)" +
            	" Volt(Set " + strVoltTarget + "V, Start " + strVoltInit + "V" + ", Finish " + strVoltEnd + "V)";
        else
        {
        	m_strInfo = m_strInfo.SubString(1, m_strInfo.Pos("T") - 2) + "~" + TimeToStr(Time()) +
				" Temp(Set " + strTempTarget + "��, Start " + strTempInit + "��" + ", Finish " + strTempEnd + "��)" +
            	" Volt(Set " + strVoltTarget + "V, Start " + strVoltInit + "V" + ", Finish " + strVoltEnd + "V)";
        }

		m_pF1BookTestReport->Sheet = iSheet;

        // ��д���Խ�����Ϣ
		int iRow = 0;
        GetRowCountInTVSheet(iSheet, iRow);
		m_pF1BookTestReport->Sheet = iSheet;
		m_pF1BookTestReport->SetSelection(iRow + 2, 2, iRow + 2, 2);
		m_pF1BookTestReport->Text = m_strInfo;
        // �������ظ�����ʾ
		for (int itmp = 1; itmp <= MES_ITEM_REAL; itmp++)
		{
			m_pF1BookTestItem->SetSelection(itmp, 1, itmp, 1);
			m_pF1BookTestItem->SetPattern(0, 0x000000, 0xFF0000);
		}
	} else
	{
        g_strMark = "ConditionNum Wrong!";
        AddErrorByMark();
	}

	m_bWrite = false;
}

void __fastcall TFormMain::WriteEcuIDInTVSheet()
{
    int iTVModeSel = 0, iBoardSel = 0;
    for(int iTVMode = 0; iTVMode < 9; iTVMode++)
    {
    	iTVModeSel = g_STestInfo.m_iTVCond[iTVMode];
        if(0 == iTVModeSel)	continue;
    	for(int iBoard = 0; iBoard < MES_SUBBOARD_NUM; iBoard++)
        {
        	iBoardSel = (g_STestInfo.m_iSelACU>>iBoard)&0x01;
            if(0 == iBoardSel)	continue;

            WriteSingleEcuIDInTVSheet(iTVMode, iBoard, g_STestInfo.m_strEcuId[iBoard]);
        }
    }
}

//////////////////////////////////////////////////////////////////////////////
// U:35 ��д��ECU�Ķ�Ӧ���Ƶ���ӦTV�ֱ�
// ������
// 		iTVModeΪ����״̬��ţ������������ӵ͵���Ϊ0-8��������
// 		iBoardΪACU��ţ�Ϊ0-5����������
// 		strEcuIdΪECU��ID���ƣ�Ϊ�ַ������ͣ�������255���ַ�
void __fastcall TFormMain::WriteSingleEcuIDInTVSheet(int iTVMode, int iBoard, String strEcuId)
{
	m_bWrite = true;
	String	strTemp = "";

	if ((iTVMode < 9) && (iTVMode >= 0) && (iBoard < 6) && (iBoard >= 0))
	{
		// ���ȶ����#��ͷ��
		for (int iColumn = 3; iColumn <= 8; iColumn++)
		{
			m_pF1BookTestReport->Sheet = iTVMode + MES_SHEET_TV;
			m_pF1BookTestReport->SetSelection(1, iColumn, 1, iColumn);
			strTemp = m_pF1BookTestReport->Text;
			strTemp = strTemp.SubString(1, 1);
			if (strTemp == "#") m_pF1BookTestReport->Text = "";
		}

        // ��д��ǰTVSheet��ECUID
		m_pF1BookTestReport->Sheet = iTVMode + MES_SHEET_TV;
		m_pF1BookTestReport->SetSelection(1, iBoard + MES_TV_BOARDCOL, 1, iBoard + MES_TV_BOARDCOL);
		m_pF1BookTestReport->SetFont("Arial", 10, 1, 0, 0, 0, 0x000000, 0, 0);
		m_pF1BookTestReport->Text = strEcuId;
	} else
	{
		g_strMark = "ECUNum or TempVoltMode Wrong.!";
		AddErrorByMark();
	}

	m_bWrite = false;
}

//////////////////////////////////////////////////////////////////////////////
// U:36 ��ȡ����
// ˵����
// 		�����TVSheet����д�Ĳ��Բ�������һ�е����������ص���������Ϊ
//		"Test Information"����һ�У�����п����Ƿ��Ҫɾ��
void __fastcall TFormMain::GetRowCountInTVSheet(int iSheet, int & iRow)
{
	int		iRowCurr = 1;
	String	StrTemp = "";

    // �ж�ACU Information����������,ֻѰ�ҵ�MES_ITEM_REAL + 5 + 2 = 81�У����ⳬ����Χ����ʱ��79��
	for (iRowCurr = 1; (iRowCurr <= MES_ITEM_REAL + 5 + 2)&&(StrTemp != "ACU Information"); iRowCurr++)
	{
		m_pF1BookTestReport->Sheet = iSheet;
		m_pF1BookTestReport->SetSelection(iRowCurr, MES_TV_ITEMCOL, iRowCurr, MES_TV_ITEMCOL);
		StrTemp = m_pF1BookTestReport->Text;
	}

    // �ж�ITEM��Ԫ��Ϊ�յ�����������ֻѰ�ҵ�MES_ITEM_REAL + 5 + 2 = 81�У����ⳬ����Χ
	if (iRowCurr >= MES_ITEM_REAL + 5 + 2)
	{
		ShowMessage("Find Row Count Failed in TVSheet.!");
	}

    // ������
 	iRow = iRowCurr - 3;
}

//////////////////////////////////////////////////////////////////////////////
// U:37 ��TVSheet��д���ƺͲ��Թ淶(ȫ��)
// ˵����
//		iTVCond[9]:�¶ȡ���ѹ�ľ������
//		pItem[MES_ITEM_REAL]:ÿ��������������
void __fastcall TFormMain::WriteSPECInTVSheet()
{
	for (int iSheet = MES_SHEET_TV; iSheet <= 10; iSheet++)
	{
    	int iTVModeSel = g_STestInfo.m_iTVCond[iSheet - MES_SHEET_TV];
        if(1 != iTVModeSel)	continue;
		m_pF1BookTestReport->Sheet = iSheet;
		for (int iItem = 0; iItem < MES_ITEM_REAL; iItem++)
		{
        	int iItemSel = g_STestInfo.m_iItem[iItem];
			if (iItemSel != 0)
			{
            	int iRow = 0;
				WriteSPECSingleInTVSheet(iItem, iSheet);
                GetRowCountInTVSheet(iSheet, iRow);
                m_iaItem[iItem] = iRow;
			}
		}
	}
}

//////////////////////////////////////////////////////////////////////////////
// U:38 ��TVSheet = iSheet������һ�в��Բ��裬��д�ò�������ƺ�����ֵ��Χ��
// ˵����
// 		iItem��Ϊ�����based 0����Ӧ��С������ţ�Ϊ0----MES_ITEM_REAL-1=74����������
// 		iSheet��Ϊҳ�����based 1��Ϊ1-10��������
void __fastcall TFormMain::WriteSPECSingleInTVSheet(int iItem, int iSheet)
{
	m_bWrite = true;
    int		iRow = 0;

	if ((iSheet <= 10) && (iSheet >= 2))
	{
    	// ���1�б��
        GetRowCountInTVSheet(iSheet, iRow);
		m_pF1BookTestReport->Sheet = iSheet;
		m_pF1BookTestReport->SetSelection(iRow + 1, MES_T_ITEMCOL, iRow + 1, MES_T_ITEMCOL);
		m_pF1BookTestReport->EditInsert(F1ShiftRows);
		m_pF1BookTestReport->SetSelection(iRow + 1, MES_T_ITEMCOL, iRow + 1, MES_TV_AVGCOL);
		m_pF1BookTestReport->SetBorder(-1, -1, 1, -1, -1, -1, 0, 0, 0, 0, 0);

        // ����ڶ���Ϊ����ɾ���ڶ���
		m_pF1BookTestReport->Sheet = iSheet;
		m_pF1BookTestReport->SetSelection(2, MES_T_ITEMCOL, 2, MES_T_ITEMCOL);
		String	strTemp = m_pF1BookTestReport->Text;
		if (strTemp == "")
		{
			m_pF1BookTestReport->EditDelete(F1ShiftRows);
		}
        GetRowCountInTVSheet(iSheet, iRow);

        // ��ȡ��д�벽��͹淶
		String	strItemName = "", strItemSpec = "";
		strItemName = GetSPECInfo(iItem, 0);
		strItemSpec = GetSPECInfo(iItem, 1);
		m_pF1BookTestReport->Sheet = iSheet;
		m_pF1BookTestReport->SetSelection(iRow, MES_T_ITEMCOL, iRow, MES_T_ITEMCOL);
		m_pF1BookTestReport->Text = strItemName;
		m_pF1BookTestReport->SetSelection(iRow, MES_T_SPECCOL, iRow, MES_T_SPECCOL);
		m_pF1BookTestReport->Text = strItemSpec;
	} else
	{
    	g_strMark = "SheetIndex Wrong.!";
		AddErrorByMark();
	}

	m_pF1BookTestReport->Sheet = iSheet;
	m_bWrite = false;
}

//////////////////////////////////////////////////////////////////////////////
// U:39 ���ݵ�ǰ����TV���������¶Ƚ׶� LT NT HT
void __fastcall TFormMain::ViewTempPhase()
{
    m_pShapeTempLow->Brush->Color = clWhite;
	m_pShapeTempNor->Brush->Color = clWhite;
	m_pShapeTempHigh->Brush->Color = clWhite;
    switch(g_iTempPhase)
    {
    	case 0:
        	m_pShapeTempLow->Brush->Color = clLime;
        	break;
        case 1:
        	m_pShapeTempNor->Brush->Color = clLime;
        	break;
        case 2:
        	m_pShapeTempHigh->Brush->Color = clLime;
        	break;
        default:
        	m_pShapeTempLow->Brush->Color = clLime;
        	break;
    }
}

//////////////////////////////////////////////////////////////////////////////
// U:40 ���ݵ�ǰ����TV���������¶Ƚ׶� LV NV HV
void __fastcall TFormMain::ViewVoltPhase()
{
	m_pShapeVoltLow->Brush->Color = clWhite;
	m_pShapeVoltNor->Brush->Color = clWhite;
	m_pShapeVoltHigh->Brush->Color = clWhite;
    switch(g_iVoltPhase)
    {
    	case 0:
        	m_pShapeVoltLow->Brush->Color = clLime;
        	break;
        case 1:
        	m_pShapeVoltNor->Brush->Color = clLime;
        	break;
        case 2:
        	m_pShapeVoltHigh->Brush->Color = clLime;
        	break;
        default:
        	m_pShapeVoltLow->Brush->Color = clLime;
        	break;
    }
}

//////////////////////////////////////////////////////////////////////////////
// U:41 ������ʾ-�����¶�����
void __fastcall TFormMain::ViewWaveTempTheory()
{
	g_fTimeTotal = fabs(g_fTempTarget - g_fTempInit) / 2.0;
    g_fTimeRest = g_fTimeTotal + 10 + 1 / 60.0;
	m_pTempCurveReal->Clear();
	m_pTempCurveTheory->Clear();
	m_pTempCurveTheory->AddXY(0.0, (F64) (g_fTempInit), "", clRed);
	m_pTempCurveTheory->AddXY((F64) (g_fTimeTotal * 60.0), (double) (g_fTempTarget), "", clRed);
	m_pTempCurveTheory->AddXY((F64) (g_fTimeTotal * 60.0 + 600.0), (double) (g_fTempTarget), "", clRed);
}

//////////////////////////////////////////////////////////////////////////////
// U:42 ��LabelCond����ʾ��ǰTempVoltģʽ
void __fastcall TFormMain::ViewTVCondInLabel()
{
	String strTempTarget = "", strVoltTarget = "";
	if((g_iTVCond < 9)&&(g_iTVCond >= 0))
    {
        strTempTarget = FloatToStr(g_fTempTarget);
        if((g_fTempTarget > -0.01)&&(g_fTempTarget < 0.01))
        {
            strTempTarget = "0";
        }
        else if(strTempTarget.Pos(".") != 0)
        {
        	strTempTarget = strTempTarget.SubString(1, strTempTarget.Pos(".") + 2);
        }

        strVoltTarget = FloatToStr(g_fVoltTarget);
        if((g_fVoltTarget > -0.01)&&(g_fVoltTarget < 0.01))
        {
             strVoltTarget = "0";
        }
        else if(strVoltTarget.Pos(".") != 0)
        {
        	strVoltTarget = strVoltTarget.SubString(1, strVoltTarget.Pos(".") + 2);
        }

        m_pLabelCond->Caption = strTempTarget + "�� " + strVoltTarget + "V  " + DateToStr(Date());
    }
}

// ��ȡ��Ӧҳ��Ķ�Ӧ���Բ����ƽ��ֵ������������ַ����ĸ�ʽ���أ����û��ƽ��ֵ���ؿա�
// iSheetΪ��ӦTVSheet��ţ�Ϊ2-10����������
// strItemNameΪ���Բ�������ƣ�Ϊ�ַ������ͣ����ӦΪ���ڵĲ���������
String __fastcall TFormMain::ReadAvg(int iSheet, String strItemName)
{
	int		iRow = 0;
	bool	bHave = false;
	String	strItem = "", strAvg = "";

	if ((iSheet <= 10) && (iSheet >= 2))
	{
		m_pF1BookTestReport->Sheet = iSheet;
		for (iRow = 0; iRow < MES_ITEM_REAL; iRow++)
		{
			m_pF1BookTestReport->Sheet = iSheet;
			m_pF1BookTestReport->SetSelection(iRow + 2, MES_T_ITEMCOL, iRow + 2, MES_T_ITEMCOL);
			strItem = m_pF1BookTestReport->Text;
			if (strItem == strItemName)
			{
				m_pF1BookTestReport->Sheet = iSheet;
				m_pF1BookTestReport->SetSelection(iRow + 2, MES_TV_AVGCOL, iRow + 2, MES_TV_AVGCOL);
				strAvg = m_pF1BookTestReport->Text;
				bHave = 1;
			}
		}
	}

	// ���û�в����������ֵΪ""
	if (false == bHave)
	{
		strAvg = "";
	}

	return strAvg;
}

void __fastcall TFormMain::WriteCrash()
{
	m_bWrite = true;

	int		iRow = 0;
	String	strTemp = "";
	if ((g_iCurrBoard < MES_SUBBOARD_NUM) && (g_iCurrBoard >= 0) && (g_iTVCond >= 0) && (g_iTVCond < 9) &&
		(g_iCurrItem == MES_ITEM_CRASH))
	{
		m_pF1BookTestReport->Sheet = g_iTVCond + MES_SHEET_TV;
		iRow = m_iaItem[g_iCurrItem];

		if (g_strItemValue == "ok")
		{
			m_pF1BookTestReport->Sheet = g_iTVCond + MES_SHEET_TV;
			m_pF1BookTestReport->SetSelection(iRow, g_iCurrBoard + 3, iRow, g_iCurrBoard + 3);
			m_pF1BookTestReport->SetFont("Arial", 10, 0, 0, 0, 0, 0x000000, 0, 0);
			m_pF1BookTestReport->Text = "ok";
			m_pF1BookTestReport->ShowActiveCell();

			m_pF1BookTestReport->Sheet = g_iTVCond + MES_SHEET_TV;
			m_pF1BookTestReport->SetSelection(iRow, MES_TV_JUDGECOL, iRow, MES_TV_JUDGECOL);
			strTemp = m_pF1BookTestReport->Text;
			if (strTemp != "failed")
			{
				m_pF1BookTestReport->SetFont("Arial", 10, 1, 0, 0, 0, 0x000000, 0, 0);
				m_pF1BookTestReport->Text = "ok";
				m_pF1BookTestReport->ShowActiveCell();
			}

			m_pF1BookTestReport->Sheet = g_iTVCond + MES_SHEET_TV;
			m_pF1BookTestReport->SetSelection(iRow, MES_TV_AVGCOL, iRow, MES_TV_AVGCOL);
			m_pF1BookTestReport->SetFont("Arial", 10, 0, 0, 0, 0, 0x000000, 0, 0);
			m_pF1BookTestReport->Text = "------";
			m_pF1BookTestReport->ShowActiveCell();
			m_pF1BookTestReport->SetSelection(iRow, MES_TV_MINCOL, iRow, MES_TV_MINCOL);
			m_pF1BookTestReport->SetFont("Arial", 10, 0, 0, 0, 0, 0x000000, 0, 0);
			m_pF1BookTestReport->Text = "------";
			m_pF1BookTestReport->ShowActiveCell();
			m_pF1BookTestReport->SetSelection(iRow, MES_TV_MAXCOL, iRow, MES_TV_MAXCOL);
			m_pF1BookTestReport->SetFont("Arial", 10, 0, 0, 0, 0, 0x000000, 0, 0);
			m_pF1BookTestReport->Text = "------";
			m_pF1BookTestReport->ShowActiveCell();
		} else if (g_strItemValue == "failed")
		{
			m_pF1BookTestReport->Sheet = g_iTVCond + MES_SHEET_TV;
			m_pF1BookTestReport->SetSelection(iRow, g_iCurrBoard + 3, iRow, g_iCurrBoard + 3);
			m_pF1BookTestReport->SetFont("Arial", 10, 1, 0, 0, 0, 0x0000FF, 0, 0);
			m_pF1BookTestReport->Text = "failed";
			m_pF1BookTestReport->ShowActiveCell();

			m_pF1BookTestReport->SetSelection(iRow, MES_TV_JUDGECOL, iRow, MES_TV_JUDGECOL);
			m_pF1BookTestReport->SetFont("Arial", 10, 1, 0, 0, 0, 0x0000FF, 0, 0);
			m_pF1BookTestReport->Text = "failed";
			m_pF1BookTestReport->ShowActiveCell();

			m_pF1BookTestReport->Sheet = g_iTVCond + MES_SHEET_TV;
			m_pF1BookTestReport->SetSelection(iRow, MES_TV_AVGCOL, iRow, MES_TV_AVGCOL);
			m_pF1BookTestReport->SetFont("Arial", 10, 0, 0, 0, 0, 0x000000, 0, 0);
			m_pF1BookTestReport->Text = "------";
			m_pF1BookTestReport->ShowActiveCell();
			m_pF1BookTestReport->SetSelection(iRow, MES_TV_MINCOL, iRow, MES_TV_MINCOL);
			m_pF1BookTestReport->SetFont("Arial", 10, 0, 0, 0, 0, 0x000000, 0, 0);
			m_pF1BookTestReport->Text = "------";
			m_pF1BookTestReport->ShowActiveCell();
			m_pF1BookTestReport->SetSelection(iRow, MES_TV_MAXCOL, iRow, MES_TV_MAXCOL);
			m_pF1BookTestReport->SetFont("Arial", 10, 0, 0, 0, 0, 0x000000, 0, 0);
			m_pF1BookTestReport->Text = "------";
			m_pF1BookTestReport->ShowActiveCell();
		} else
		{
			if (g_iCurrItem == MES_ITEM_CRASH)
			{
				g_strMark = "Crash Output fail!";
				AddErrorByMark();
			} else
			{
				g_strMark = "Step" + IntToStr(g_iCurrItem) + " fail!";
				AddErrorByMark();
			}
		}

		m_pF1BookTestReport->Sheet = g_iTVCond + MES_SHEET_TV;
		m_pF1BookTestReport->SetSelection(iRow, MES_TV_JUDGECOL, iRow, MES_TV_JUDGECOL);
		m_pF1BookTestReport->ShowActiveCell();
	} else
	{
		g_strMark = "ACUNum Wrong!";
		AddErrorByMark();
	}

	m_bWrite = false;
}

