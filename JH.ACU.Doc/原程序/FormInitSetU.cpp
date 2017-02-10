#include <vcl.h>
#pragma hdrstop

#include "FormInitSetU.h"
#include <stdio.h>
#include <dos.h>

#pragma package(smart_init)
#pragma resource "*.dfm"

TFormInitSet *FormInitSet;

String g_StepText[80]={"01-Squib 1 Resistance High","02-Squib 2 Resistance High",
                          "03-Squib 3 Resistance High","04-Squib 4 Resistance High",
                          "05-Squib 5 Resistance High","06-Squib 6 Resistance High",
                          "07-Squib 7 Resistance High","08-Squib 8 Resistance High",
                          "09-Squib 9 Resistance High","10-Squib 10 Resistance High",
                          "11-Squib 1 Resistance Low","12-Squib 2 Resistance Low",
                          "13-Squib 3 Resistance Low","14-Squib 4 Resistance Low",
                          "15-Squib 5 Resistance Low","16-Squib 6 Resistance Low",
                          "17-Squib 7 Resistance Low","18-Squib 8 Resistance Low",
                          "19-Squib 9 Resistance Low","20-Squib 10 Resistance Low",
                          "21-Squib 1 Short to GND","22-Squib 2 Short to GND",
                          "23-Squib 3 Short to GND","24-Squib 4 Short to GND",
                          "25-Squib 5 Short to GND","26-Squib 6 Short to GND",
                          "27-Squib 7 Short to GND","28-Squib 8 Short to GND",
                          "29-Squib 9 Short to GND","30-Squib 10 Short to GND",
                          "31-Squib 1 Short to Vbatt","32-Squib 2 Short to Vbatt",
                          "33-Squib 3 Short to Vbatt","34-Squib 4 Short to Vbatt",
                          "35-Squib 5 Short to Vbatt","36-Squib 6 Short to Vbatt",
                          "37-Squib 7 Short to Vbatt","38-Squib 8 Short to Vbatt",
                          "39-Squib 9 Short to Vbatt","40-Squib 10 Short to Vbatt",
                          "41-SBELT 1 STG <-> Unbelted","42-SBELT 1 Unbelted <-> Belted",
                          "43-SBELT 1 Buckled <-> STB","44-SBELT 2 STG <-> Unbelted",
                          "45-SBELT 2 Unbelted <-> Belted","46-SBELT 2 Buckled <-> STB",
                          "47-PADS  1 STG <-> OFF","48-PADS  1 OFF <-> Defected",
                          "49-PADS  1 ON <-> STB","50-BATT_Battery Too High",
                          "51-BATT_Battery Low (OK->LOW)","52-SIS 1 Short to Vbatt",
                          "53-SIS 2 Short to Vbatt","54-SIS 3 Short to Vbatt",
                          "55-SIS 4 Short to Vbatt","56-SIS 1 Short to GND",
                          "57-SIS 2 Short to GND","58-SIS 3 Short to GND",
                          "59-SIS 4 Short to GND","60-SIS 1 Sensor Output",
                          "61-SIS 2 Sensor Output","62-SIS 3 Sensor Output",
                          "63-SIS 4 Sensor Output","64-WL1_VOLT(OFF)",
                          "65-WL1_VOLT(ON)","66-WL1_CURRENT(normal)",
                          "67-WL1_CURRENT(short)","68-WL2_VOLT(OFF)",
                          "69-WL2_VOLT(ON)","70-WL2_CURRENT(normal)",
                          "71-WL2_CURRENT(short)","72-Autonomous time()","73-ACU_CURRENT()","74-Crash Output Check"};

__fastcall TFormInitSet::TFormInitSet(TComponent *Owner) :
    TForm(Owner)
{

}

void __fastcall TFormInitSet::FormCreate(TObject * Sender)
{
	LoadLatest();
    m_bLedt = false;
	m_bItem = false;
	m_bCond = false;
    m_pBtnSave->Enabled = false;
}

void __fastcall TFormInitSet::OnLedtLowTempChange(TObject * Sender)
{
	m_pBtnSave->Enabled = true; // 表示框中内容有改动，可再次存盘
	m_bLedt = true;				// 改动的内容还未存盘，直接点OK退出，应弹出对话框
}

void __fastcall TFormInitSet::OnLedtLowTempExit(TObject * Sender)
{
	int			iLength = 0;
	float		fValue = 0.0;
	String		str = "";

	str = m_pLedtLowTemp->Text;
	str = str.Trim();
	iLength = str.Length();
	if (str == "\0")
	{
		ShowMessage("Low temperature is empty!");
		m_pLedtLowTemp->SetFocus();
		return;
	}

    // 只检验出开始为字符的情况
	if (sscanf(&str.operator[](1), "%f", &fValue) == -1 || sscanf(&str.operator[](1), "%f", &fValue) == 0)
	{
		ShowMessage("Invalid low temperature value!");
		m_pLedtLowTemp->SetFocus();
		return;
	}

	m_pLedtLowTemp->Text = FloatToStr(fValue).SubString(1, iLength);
	if (fValue < -80.00 || fValue > 180.00)
	{
		ShowMessage("Low temperature is out of range!");
		m_pLedtLowTemp->SetFocus();
	}
}

void __fastcall TFormInitSet::OnLedtLowVoltChange(TObject *Sender)
{
    m_pBtnSave->Enabled = true;
	m_bLedt = true;
}

void __fastcall TFormInitSet::OnLedtLowVoltExit(TObject * Sender)
{
	int			iLength = 0;
	float		fValue = 0.0;
	String	str = "";

	str = m_pLedtLowVolt->Text;;
	str = str.Trim();
	iLength = str.Length();
	if (str == "\0")
	{
		ShowMessage("Low voltage is empty!");
		m_pLedtLowVolt->SetFocus();
		return;
	}

	// 只检验出开始为字符的情况
	if (sscanf(&str.operator[](1), "%f", &fValue) == -1 || sscanf(&str.operator[](1), "%f", &fValue) == 0)
	{
		ShowMessage("Invalid Low voltage value!");
		m_pLedtLowVolt->SetFocus();
		return;
	}

	m_pLedtLowVolt->Text = FloatToStr(fValue).SubString(1, iLength);
	if (fValue < 0.00 || fValue > 38.00)
	{
		ShowMessage("Low voltage is out of range!");
		m_pLedtLowVolt->SetFocus();
	}
}

void __fastcall TFormInitSet::OnLedtNorTempChange(TObject *Sender)
{
    m_pBtnSave->Enabled = true;
	m_bLedt = true;
}

void __fastcall TFormInitSet::OnLedtNorTempExit(TObject *Sender)
{
    int			iLength = 0;
	float		fValue = 0.0;
	String	str = "";

	str = m_pLedtNorTemp->Text;;
	str = str.Trim();
	iLength = str.Length();
	if (str == "\0")
	{
		ShowMessage("Nor temperature is empty!");
		m_pLedtNorTemp->SetFocus();
		return;
	}

    // 只检验出开始为字符的情况
	if (sscanf(&str.operator[](1), "%f", &fValue) == -1 || sscanf(&str.operator[](1), "%f", &fValue) == 0)
	{
		ShowMessage("Invalid Nor temperature value!");
		m_pLedtNorTemp->SetFocus();
		return;
	}

	m_pLedtNorTemp->Text = FloatToStr(fValue).SubString(1, iLength);
	if (fValue < -80.00 || fValue > 180.00)
	{
		ShowMessage("Nor temperature is out of range!");
		m_pLedtNorTemp->SetFocus();
	}
}

void __fastcall TFormInitSet::OnLedtNorVoltChange(TObject *Sender)
{
    m_pBtnSave->Enabled = true;
	m_bLedt = true;
}

void __fastcall TFormInitSet::OnLedtNorVoltExit(TObject *Sender)
{
    int			iLength = 0;
	float		fValue = 0.0;
	String	str = "";

	str = m_pLedtNorVolt->Text;;
	str = str.Trim();
	iLength = str.Length();
	if (str == "\0")
	{
		ShowMessage("Nor voltage is empty!");
		m_pLedtNorVolt->SetFocus();
		return;
	}

	// 只检验出开始为字符的情况
	if (sscanf(&str.operator[](1), "%f", &fValue) == -1 || sscanf(&str.operator[](1), "%f", &fValue) == 0)
	{
		ShowMessage("Invalid Nor voltage value!");
		m_pLedtNorVolt->SetFocus();
		return;
	}

	m_pLedtNorVolt->Text = FloatToStr(fValue).SubString(1, iLength);
	if (fValue < 0.00 || fValue > 38.00)
	{
		ShowMessage("Nor voltage is out of range!");
		m_pLedtNorVolt->SetFocus();
	}
}

void __fastcall TFormInitSet::OnLedtSampleIntervalChange(TObject *Sender)
{
    m_pBtnSave->Enabled = true;
	m_bLedt = true;
}

void __fastcall TFormInitSet::OnLedtSampleIntervalExit(TObject * Sender)
{
	int			iValue = 0.0;
	String	str = "";

	str = m_pLedtSampleInterval->Text;;
	str = str.Trim();
	if (str == "\0")
	{
		ShowMessage("sample interval is empty!");
		m_pLedtSampleInterval->SetFocus();
		return;
	}

	// 只检验出开始为字符的情况
	if (sscanf(&str.operator[](1), "%d", &iValue) == -1 || sscanf(&str.operator[](1), "%d", &iValue) == 0)
	{
		ShowMessage("Invalid sample interval value!");
		m_pLedtSampleInterval->SetFocus();
		return;
	}

	m_pLedtSampleInterval->Text = IntToStr(iValue);
	if (iValue < 10)
	{
		ShowMessage("sample interval is out of range!");
		m_pLedtSampleInterval->SetFocus();
	}
}

void __fastcall TFormInitSet::OnLedtHighTempChange(TObject *Sender)
{
    m_pBtnSave->Enabled = true;
	m_bLedt = true;
}

void __fastcall TFormInitSet::OnLedtHighTempExit(TObject *Sender)
{
    int			iLength = 0;
	float		fValue = 0.0;
	String	str = "";

	str = m_pLedtHighTemp->Text;;
	str = str.Trim();
	iLength = str.Length();
	if (str == "\0")
	{
		ShowMessage("High temperature is empty!");
		m_pLedtHighTemp->SetFocus();
		return;
	}

    // 只检验出开始为字符的情况
	if (sscanf(&str.operator[](1), "%f", &fValue) == -1 || sscanf(&str.operator[](1), "%f", &fValue) == 0)
	{
		ShowMessage("Invalid High temperature value!");
		m_pLedtHighTemp->SetFocus();
		return;
	}

	m_pLedtHighTemp->Text = FloatToStr(fValue).SubString(1, iLength);
	if (fValue < -80.00 || fValue > 180.00)
	{
		ShowMessage("High temperature is out of range!");
		m_pLedtHighTemp->SetFocus();
	}
}

void __fastcall TFormInitSet::OnLedtHighVoltChange(TObject *Sender)
{
    m_pBtnSave->Enabled = true;
	m_bLedt = true;
}

void __fastcall TFormInitSet::OnLedtHighVoltExit(TObject *Sender)
{
    int			iLength = 0;
	float		fValue = 0.0;
	String	str = "";

	str = m_pLedtHighVolt->Text;;
	str = str.Trim();
	iLength = str.Length();
	if (str == "\0")
	{
		ShowMessage("High voltage is empty!");
		m_pLedtHighVolt->SetFocus();
		return;
	}

	// 只检验出开始为字符的情况
	if (sscanf(&str.operator[](1), "%f", &fValue) == -1 || sscanf(&str.operator[](1), "%f", &fValue) == 0)
	{
		ShowMessage("Invalid High voltage value!");
		m_pLedtHighVolt->SetFocus();
		return;
	}

	m_pLedtHighVolt->Text = FloatToStr(fValue).SubString(1, iLength);
	if (fValue < 0.00 || fValue > 38.00)
	{
		ShowMessage("High voltage is out of range!");
		m_pLedtHighVolt->SetFocus();
	}
}

void __fastcall TFormInitSet::OnBtnDefaultClick(TObject *Sender)
{
    GetTargetValue();
}

void __fastcall TFormInitSet::OnLedtTestNameChange(TObject *Sender)
{
    m_pBtnSave->Enabled = true;
	m_bLedt = true;
}

void __fastcall TFormInitSet::OnLedtModelNameChange(TObject *Sender)
{
    m_pBtnSave->Enabled = true;
	m_bLedt = true;
}

void __fastcall TFormInitSet::OnLedtRemarkChange(TObject *Sender)
{
    m_pBtnSave->Enabled = true;
	m_bLedt = true;
}

void __fastcall TFormInitSet::OnLedtID1Change(TObject *Sender)
{
    m_pBtnSave->Enabled = true;
	m_bLedt = true;
}

void __fastcall TFormInitSet::OnLedtID2Change(TObject *Sender)
{
    m_pBtnSave->Enabled = true;
	m_bLedt = true;
}

void __fastcall TFormInitSet::OnLedtID3Change(TObject *Sender)
{
    m_pBtnSave->Enabled = true;
	m_bLedt = true;
}

void __fastcall TFormInitSet::OnLedtID4Change(TObject *Sender)
{
    m_pBtnSave->Enabled = true;
	m_bLedt = true;
}

void __fastcall TFormInitSet::OnLedtID5Change(TObject *Sender)
{
    m_pBtnSave->Enabled = true;
	m_bLedt = true;
}

void __fastcall TFormInitSet::OnLedtID6Change(TObject *Sender)
{
    m_pBtnSave->Enabled = true;
	m_bLedt = true;
}

void __fastcall TFormInitSet::OnChbLTLVClick(TObject *Sender)
{
    m_pBtnSave->Enabled = true;
    m_bCond = true;
}

void __fastcall TFormInitSet::OnChbLTNVClick(TObject *Sender)
{
    m_pBtnSave->Enabled = true;
    m_bCond = true;
}

void __fastcall TFormInitSet::OnChbLTHVClick(TObject *Sender)
{
    m_pBtnSave->Enabled = true;
    m_bCond = true;
}

void __fastcall TFormInitSet::OnChbNTLVClick(TObject *Sender)
{
    m_pBtnSave->Enabled = true;
    m_bCond = true;
}

void __fastcall TFormInitSet::OnChbNTNVClick(TObject *Sender)
{
    m_pBtnSave->Enabled = true;
    m_bCond = true;
}

void __fastcall TFormInitSet::OnChbNTHVClick(TObject *Sender)
{
    m_pBtnSave->Enabled = true;
    m_bCond = true;
}

void __fastcall TFormInitSet::OnChbHTLVClick(TObject *Sender)
{
    m_pBtnSave->Enabled = true;
    m_bCond = true;
}

void __fastcall TFormInitSet::OnChbHTNVClick(TObject *Sender)
{
    m_pBtnSave->Enabled = true;
    m_bCond = true;
}

void __fastcall TFormInitSet::OnChbHTHVClick(TObject *Sender)
{
    m_pBtnSave->Enabled = true;
    m_bCond = true;
}

void __fastcall TFormInitSet::OnLedtLTDelayChange(TObject *Sender)
{
    m_pBtnSave->Enabled = true;
	m_bLedt = true;
}

void __fastcall TFormInitSet::OnLedtLTDelayExit(TObject * Sender)
{
	int		iTemp = 0;
	String	str = "";
	str = m_pLedtLTDelay->Text;
	str = str.Trim();	// 消除前导和后续空格
	if (str == "\0")
	{
		ShowMessage("The chamber delay of low temperature is empty!");
		m_pLedtLTDelay->SetFocus();
		return;
	}
	// 只检验出开始为字符的情况
	if (sscanf(&str.operator[](1), "%d", &iTemp) == -1 || sscanf(&str.operator[](1), "%d", &iTemp) == 0)
	{
		ShowMessage("Invalid chamber delay of low temperature!");
		m_pLedtLTDelay->SetFocus();
		return;
	}

	m_pLedtLTDelay->Text = IntToStr(iTemp);
	if (iTemp < 0)
	{
		ShowMessage("The chamber delay of low temperature is out of range!");
		m_pLedtLTDelay->SetFocus();
	}
}

void __fastcall TFormInitSet::OnLedtNTDelayChange(TObject *Sender)
{
    m_pBtnSave->Enabled = true;
	m_bLedt = true;
}

void __fastcall TFormInitSet::OnLedtNTDelayExit(TObject * Sender)
{
	int			iTemp = 0;
	String	str = "";
	str = m_pLedtNTDelay->Text;
	str = str.Trim();	// 消除前导和后续空格
	if (str == "\0")
	{
		ShowMessage("The chamber delay of normal temperature is empty!");
		m_pLedtNTDelay->SetFocus();
		return;
	}
    // 只检验出开始为字符的情况
	if (sscanf(&str.operator[](1), "%d", &iTemp) == -1 || sscanf(&str.operator[](1), "%d", &iTemp) == 0)
	{
		ShowMessage("Invalid chamber delay of normal temperature!");
		m_pLedtNTDelay->SetFocus();
		return;
	}

	m_pLedtNTDelay->Text = IntToStr(iTemp);
	if (iTemp < 0)
	{
		ShowMessage("The chamber delay of normal temperature is out of range!");
		m_pLedtNTDelay->SetFocus();
	}
}

void __fastcall TFormInitSet::OnLedtHTDelayChange(TObject *Sender)
{
    m_pBtnSave->Enabled = true;
	m_bLedt = true;
}

void __fastcall TFormInitSet::OnLedtHTDelayExit(TObject * Sender)
{
	int			iTemp = 0;
	String	str = "";
	str = m_pLedtHTDelay->Text;
	str = str.Trim();	// 消除前导和后续空格
	if (str == "\0")
	{
		ShowMessage("The chamber delay of high temperature is empty!");
		m_pLedtHTDelay->SetFocus();
		return;
	}
	// 只检验出开始为字符的情况
	if (sscanf(&str.operator[](1), "%d", &iTemp) == -1 || sscanf(&str.operator[](1), "%d", &iTemp) == 0)
	{
		ShowMessage("Invalid chamber delay of high temperature!");
		m_pLedtHTDelay->SetFocus();
		return;
	}

	m_pLedtHTDelay->Text = IntToStr(iTemp);
	if (iTemp < 0)
	{
		ShowMessage("The chamber delay of high temperature is out of range!");
		m_pLedtHTDelay->SetFocus();
	}
}

void __fastcall TFormInitSet::OnBtnConSelAllClick(TObject *Sender)
{
    EnableCond(true);
}

void __fastcall TFormInitSet::OnBtnConUnSelAllClick(TObject *Sender)
{
    EnableCond(false);
}

void __fastcall TFormInitSet::OnChbACU1Click(TObject *Sender)
{
    int i = 0;
    i = i + 1;
}

void __fastcall TFormInitSet::OnChbACU2Click(TObject *Sender)
{
    int i = 0;
    i = i + 1;
}

void __fastcall TFormInitSet::OnChbACU3Click(TObject *Sender)
{
    int i = 0;
    i = i + 1;
}

void __fastcall TFormInitSet::OnChbACU4Click(TObject *Sender)
{
    int i = 0;
    i = i + 1;
}

void __fastcall TFormInitSet::OnChbACU5Click(TObject *Sender)
{
    int i = 0;
    i = i + 1;
}

void __fastcall TFormInitSet::OnChbACU6Click(TObject *Sender)
{
    int i = 0;
    i = i + 1;
}

void __fastcall TFormInitSet::OnChbACU7Click(TObject *Sender)
{
    int i = 0;
    i = i + 1;
}

void __fastcall TFormInitSet::OnChbACU8Click(TObject *Sender)
{
    int i = 0;
    i = i + 1;
}

void __fastcall TFormInitSet::OnBtnACUSelAllClick(TObject *Sender)
{
    EnableACUID(true);
}

void __fastcall TFormInitSet::OnBtnACUUnSelAllClick(TObject *Sender)
{
    EnableACUID(false);
}

void __fastcall TFormInitSet::OnSpBtnOMRightClick(TObject * Sender)
{
	int iIndex = GetFirstSelInList(m_pLstbOri);
	MoveInList(m_pLstbOri, m_pLstbMid->Items);
	SetItem(m_pLstbOri, iIndex);
}

void __fastcall TFormInitSet::OnSpBtnOMRightAllClick(TObject * Sender)
{
	for (int i = 0; i < m_pLstbOri->Items->Count; i++)
	{
		m_pLstbMid->Items->AddObject(m_pLstbOri->Items->Strings[i], m_pLstbOri->Items->Objects[i]);
	}
	m_pLstbOri->Items->Clear();
	SetItem(m_pLstbOri, 0);
}

void __fastcall TFormInitSet::OnSpBtnOMLeftClick(TObject *Sender)
{
    int Index = 0;

	Index = GetFirstSelInList(m_pLstbMid);
	MoveInList(m_pLstbMid, m_pLstbOri->Items);
	SetItem(m_pLstbMid, Index);
}

void __fastcall TFormInitSet::OnSpBtnOMLeftAllClick(TObject * Sender)
{
	for (int i = 0; i < m_pLstbMid->Items->Count; i++)
	{
		m_pLstbOri->Items->AddObject(m_pLstbMid->Items->Strings[i], m_pLstbMid->Items->Objects[i]);
	}

	m_pLstbMid->Items->Clear();
	SetItem(m_pLstbMid, 0);
}

void __fastcall TFormInitSet::OnBtnMSRightClick(TObject * Sender)
{
	int		iCount = 0;
	char	iShift = 0x01;

	SetItemCurrInfo();
	m_pLstbSel->Items->Clear();

	for (int i = 1; i <= MES_SUBBOARD_NUM; i++)
	{
		m_pLstbSel->Items->Strings[iCount] = "ACU" + IntToStr(i) + ":";
		iCount++;
		for (int j = 0; j < MES_ITEM_REAL; j++)
		{
			if ((m_iItemCurr[j] & iShift << (i - 1)) == iShift << (i - 1))
			{
				m_pLstbSel->Items->Strings[iCount] = g_StepText[j];
				iCount++;
			}
		}
	}
	m_bItem = true;

	m_pBtnSave->Enabled = true;
	m_pBtnDel->Enabled = true;
}

void __fastcall TFormInitSet::OnBtnDelClick(TObject * Sender)
{
	int iIndex = 0, iStep = 0, iCurrACU = 0;
	int iAnd = 0xFE;
	int iACU = 0, j;
	if (m_pLstbSel->ItemIndex == -1)
	{
		m_pLstbSel->ItemIndex = 0;		// 选中第一行
		return;
	}

	iIndex = m_pLstbSel->ItemIndex;
    char cRet = m_pLstbSel->Items->Strings[iIndex].operator[](1);
	if (cRet != 'A')
	{
		iStep = StrToInt(m_pLstbSel->Items->Strings[iIndex].SubString(1, 2));
		iACU = iIndex - 1;
		while (m_pLstbSel->Items->Strings[iACU].operator[](1) != 'A')
        	iACU--;

        String strTemp = m_pLstbSel->Items->Strings[iACU].SubString(4, 1);
		iCurrACU = StrToInt(strTemp);
		for (j = 1; j < iCurrACU; j++)
		{
			iAnd = iAnd << 1 | 0x01;
		}
		m_iItemCurr[iStep - 1] = m_iItemCurr[iStep - 1] & iAnd;
		m_pLstbSel->Items->Delete(iIndex);
        SetItem(m_pLstbSel, iIndex - 1);
		m_pBtnSave->Enabled = true;
		m_bItem = true;
	}
}

void __fastcall TFormInitSet::OnRgCouttypeClick(TObject *Sender)
{
    m_pBtnSave->Enabled = true;
	m_bCond = true;
}

void __fastcall TFormInitSet::OnChbChamAutoClick(TObject *Sender)
{
    m_pBtnSave->Enabled = true;
	m_bCond = true;
}

void __fastcall TFormInitSet::OnBtnUpClick(TObject *Sender)
{
	int iSelIndex = m_pLstbTempSeq->ItemIndex;
    // 高亮显示
    if(-1 != iSelIndex)
    {
		m_pLstbTempSeq->Selected[iSelIndex] = true;
    }
    // 向上移动
	if (0 != iSelIndex && -1 != iSelIndex)
	{
		m_pLstbTempSeq->Items->Move(iSelIndex, iSelIndex - 1);
		m_pLstbTempSeq->Selected[iSelIndex - 1] = true;
	}
}

void __fastcall TFormInitSet::OnBtnDownClick(TObject *Sender)
{
	int iSelIndex = m_pLstbTempSeq->ItemIndex;
    int iCount = m_pLstbTempSeq->Count;
    // 高亮显示
	if(-1 != iSelIndex)
    {
		m_pLstbTempSeq->Selected[iSelIndex] = true;
    }

    // 向上移动
	if (iCount - 1 != iSelIndex && -1 != iSelIndex)
	{
		m_pLstbTempSeq->Items->Move(iSelIndex, iSelIndex + 1);
		m_pLstbTempSeq->Selected[iSelIndex + 1] = true;
	}
}

void __fastcall TFormInitSet::OnBtnOKClick(TObject *Sender)
{
	struct time t;
	struct date d;
	String		strFileName = "";
	gettime(&t);
	getdate(&d);
	strFileName = IntToStr(d.da_year) + IntToStrAdjust(d.da_mon) + IntToStrAdjust(d.da_day) +
		IntToStrAdjust(t.ti_hour) + IntToStrAdjust(t.ti_min) + ".tps";

	String	strTip = "";
	int		i, j, iRet = 0;
	m_pSaveDlg->Title = "Save As";
	m_pSaveDlg->FileName = strFileName;
	m_pSaveDlg->InitialDir = g_strAppPath + "\\tps\\";
	m_pSaveDlg->Filter = "Text Files(*.tps)|*.tps";

    // 界面有变化
	if (m_bLedt || m_bItem || m_bCond)
	{
		strTip = "Settings is modified, do you want to save it?";
		iRet = MessageDlg(strTip, mtWarning, TMsgDlgButtons() << mbYes << mbNo << mbCancel, 0);
		if (iRet == mrYes)				// Yes
		{
			if (m_pSaveDlg->Execute())	// 用来提供文件名
			{
				SaveLatest();
                RefreshStructFromForm();

				m_bLedt = false;		// 防止存盘退出后，再进入该界面点击OK键
				m_bItem = false;
				m_bCond = false;
				m_pBtnSave->Enabled = false;

				Close();
			}
		}

        // No ，表示不作为文件备份但还是认可此次设置
		if (iRet == mrNo)
		{
			RefreshStructFromForm();
			m_pBtnSave->Enabled = true;
			Close();
		}

        // 确定后又放弃，m_bLedt,m_bItem,m_bCond不变，不退出
		return;
	}

    // 界面无变化
    // 什么都没变，退出，必须更新全局结构
    RefreshStructFromForm();
	m_pBtnSave->Enabled = false;
	Close();
}

// 与原先代码比较一下
// Davidion 081213 原先代码是多余的
void __fastcall TFormInitSet::OnBtnCancelClick(TObject * Sender)
{
	Close();
}

// 保存完毕 下次再选步骤算新一轮选择
void __fastcall TFormInitSet::OnBtnSaveClick(TObject * Sender)
{
	struct time t;
	struct date d;
	String		strFileName = "";
	gettime(&t);
	getdate(&d);
	strFileName = IntToStr(d.da_year) + IntToStrAdjust(d.da_mon) + IntToStrAdjust(d.da_day)+
    	IntToStrAdjust(t.ti_hour) + IntToStrAdjust(t.ti_min) + ".tps";

	m_pSaveDlg->Title = "Save As";
	m_pSaveDlg->FileName = strFileName;
	m_pSaveDlg->InitialDir = g_strAppPath + "\\tps\\";
	m_pSaveDlg->Filter = "Text Files(*.tps)|*.tps";
	if (m_pSaveDlg->Execute())
	{
		SaveLatest();
        RefreshStructFromForm();

        m_bLedt = false;
		m_bItem = false;
		m_bCond = false;
		m_pBtnSave->Enabled = false;
	}
}

void __fastcall TFormInitSet::OnBtnLoadClick(TObject * Sender)
{
	m_pOpenDlg->Title = "Open";
	m_pOpenDlg->InitialDir = g_strAppPath + "\\tps\\";
	m_pOpenDlg->Filter = "Text Files(*.tps)|*.tps";
	if (m_pOpenDlg->Execute())
	{
		String strPath = "";
		strPath = m_pOpenDlg->FileName;
		StructFromFile(strPath);
        RefreshFormFromStruct();

		m_bLedt = false;
		m_bItem = false;
		m_bCond = false;
	}
}

///////////////////////////////////////////////////////////////////////////////
// U:01 ACU板卡统一选择或取消
void __fastcall TFormInitSet::EnableACUID(bool bEnable)
{
	m_pChbACU1->Checked = bEnable;
	m_pChbACU2->Checked = bEnable;
	m_pChbACU3->Checked = bEnable;
	m_pChbACU4->Checked = bEnable;
	m_pChbACU5->Checked = bEnable;
	m_pChbACU6->Checked = bEnable;
}

///////////////////////////////////////////////////////////////////////////////
// U:02 TV条件统一选择或取消
void __fastcall TFormInitSet::EnableCond(bool bEnable)
{
	m_pChbLTLV->Checked = bEnable;
	m_pChbLTNV->Checked = bEnable;
	m_pChbLTHV->Checked = bEnable;
	m_pChbNTLV->Checked = bEnable;
	m_pChbNTNV->Checked = bEnable;
	m_pChbNTHV->Checked = bEnable;
	m_pChbHTLV->Checked = bEnable;
	m_pChbHTNV->Checked = bEnable;
	m_pChbHTHV->Checked = bEnable;
}

///////////////////////////////////////////////////////////////////////////////
// U:03 初始化时导入配置文件到界面和g_STestInfo
// 		程序上电运行，从最新设置文件读取初始化信息，使初始化界面出现初始化信息
void __fastcall TFormInitSet::LoadLatest()
{
	TIniFile	*pIniFile = NULL;
	String		strTps = "", strTemp = "";

	pIniFile = new TIniFile(g_strAppPath + "\\config\\config.ini"); // 打开ini文件,没有就创建
	strTps = pIniFile->ReadString("Latest Settings", "FILENAME", "\0");
	delete pIniFile;

    // 以前没有记录，程序第一次运行
 	if (strTps == "\0")
	{
    	ShowMessage("Unable to open the config file to get tps file path!");
		GetTargetValue();
		ResetFormAndStruct();
        RefreshStructFromForm();
		m_pBtnDel->Enabled = false;
		m_pBtnSave->Enabled = false;
		m_pRgCouttype->Enabled = true;
		return;
	}

	strTps = g_strAppPath + "\\tps\\" + strTps;
    StructFromFile(strTps);
    RefreshFormFromStruct();
}

///////////////////////////////////////////////////////////////////////////////
// U:04 仅仅保存到配置文件
// 		将当前初始化设置存盘到文件，下次运行时该文件为最新设置文件
void __fastcall TFormInitSet::SaveLatest()
{
	TIniFile	*pIniFile;
	FILE		*pFile;
	String		strPath = "";

	strPath = m_pSaveDlg->FileName;
	pFile = fopen(strPath.c_str(), "w");
	if (pFile == NULL)
	{
		ShowMessage("Unable to open or create the setting file you want to save to!");
		return;
	}

    strPath = m_pSaveDlg->FileName.SubString(m_pSaveDlg->FileName.Length()-15, 16);
	pIniFile = new TIniFile(g_strAppPath + "\\config\\config.ini");	// 打开ini文件,没有就创建
	pIniFile->WriteString("Latest Settings", "FILENAME", strPath);
	delete pIniFile;

    // A: Target Value Setting
    // 温度
    String strTemp = "";
	strTemp = m_pLedtLowTemp->Text;
    fputs(strTemp.c_str(), pFile);
    fputc('\n', pFile);
	strTemp = m_pLedtNorTemp->Text;
    fputs(strTemp.c_str(), pFile);
    fputc('\n', pFile);
	strTemp = m_pLedtHighTemp->Text;
    fputs(strTemp.c_str(), pFile);
    fputc('\n', pFile);
    // 电压
	strTemp = m_pLedtLowVolt->Text;
    fputs(strTemp.c_str(), pFile);
    fputc('\n', pFile);
	strTemp = m_pLedtNorVolt->Text;
    fputs(strTemp.c_str(), pFile);
    fputc('\n', pFile);
	strTemp = m_pLedtHighVolt->Text;
    fputs(strTemp.c_str(), pFile);
    fputc('\n', pFile);
    // 延迟和写温度时间
    strTemp = m_pLedtLTDelay->Text;
    fputs(strTemp.c_str(), pFile);
    fputc('\n', pFile);
	strTemp = m_pLedtNTDelay->Text;
    fputs(strTemp.c_str(), pFile);
    fputc('\n', pFile);
	strTemp = m_pLedtHTDelay->Text;
    fputs(strTemp.c_str(), pFile);
    fputc('\n', pFile);
	strTemp = m_pLedtSampleInterval->Text;
    fputs(strTemp.c_str(), pFile);
    fputc('\n', pFile);
    // 温度操作顺序
    for(int i = 0; i < 3; i++)
    {
		strTemp = m_pLstbTempSeq->Items->Strings[i];
        if(strTemp == "Low Temp")		strTemp = "0";
        else if(strTemp == "Nor Temp") 	strTemp = "1";
        else if(strTemp == "High Temp") strTemp = "2";
        fputs(strTemp.c_str(), pFile);
    	fputc('\n', pFile);
    }

	// B: Test Information
	// C: ECU ID
	strTemp = m_pLedtTestName->Text;
    fputs(strTemp.c_str(), pFile);
    fputc('\n', pFile);
	strTemp = m_pLedtModelName->Text;
    fputs(strTemp.c_str(), pFile);
    fputc('\n', pFile);
	strTemp = m_pLedtRemark->Text;
    fputs(strTemp.c_str(), pFile);
    fputc('\n', pFile);

	strTemp = m_pLedtID1->Text;
    if(strTemp == "") 	strTemp = "000";
    fputs(strTemp.c_str(), pFile);
    fputc('\n', pFile);
	strTemp = m_pLedtID2->Text;
    if(strTemp == "") 	strTemp = "000";
    fputs(strTemp.c_str(), pFile);
    fputc('\n', pFile);
	strTemp = m_pLedtID3->Text;
    if(strTemp == "") 	strTemp = "000";
    fputs(strTemp.c_str(), pFile);
    fputc('\n', pFile);
	strTemp = m_pLedtID4->Text;
    if(strTemp == "") 	strTemp = "000";
    fputs(strTemp.c_str(), pFile);
    fputc('\n', pFile);
	strTemp = m_pLedtID5->Text;
    if(strTemp == "") 	strTemp = "000";
    fputs(strTemp.c_str(), pFile);
    fputc('\n', pFile);
	strTemp = m_pLedtID6->Text;
    if(strTemp == "") 	strTemp = "000";
    fputs(strTemp.c_str(), pFile);
    fputc('\n', pFile);

	// D: Test Condition Setting
	// m_iTVCond[9]
    int iTemp = 0;
    if (true == m_pChbLTLV->Checked)	iTemp = 1;
    else iTemp = 0;
    strTemp = "test_cond[" + IntToStr(0) + "]=" + IntToStr(iTemp);
    fputs(strTemp.c_str(), pFile);
    fputc('\n', pFile);

    if (true == m_pChbLTNV->Checked)	iTemp = 1;
    else iTemp = 0;
    strTemp = "test_cond[" + IntToStr(1) + "]=" + IntToStr(iTemp);
    fputs(strTemp.c_str(), pFile);
    fputc('\n', pFile);

    if (true == m_pChbLTHV->Checked)	iTemp = 1;
    else iTemp = 0;
    strTemp = "test_cond[" + IntToStr(2) + "]=" + IntToStr(iTemp);
    fputs(strTemp.c_str(), pFile);
    fputc('\n', pFile);

    if (true == m_pChbNTLV->Checked)	iTemp = 1;
    else iTemp = 0;
    strTemp = "test_cond[" + IntToStr(3) + "]=" + IntToStr(iTemp);
    fputs(strTemp.c_str(), pFile);
    fputc('\n', pFile);

    if (true == m_pChbNTNV->Checked)	iTemp = 1;
    else iTemp = 0;
    strTemp = "test_cond[" + IntToStr(4) + "]=" + IntToStr(iTemp);
    fputs(strTemp.c_str(), pFile);
    fputc('\n', pFile);

    if (true == m_pChbNTHV->Checked)	iTemp = 1;
    else iTemp = 0;
    strTemp = "test_cond[" + IntToStr(5) + "]=" + IntToStr(iTemp);
    fputs(strTemp.c_str(), pFile);
    fputc('\n', pFile);

    if (true == m_pChbHTLV->Checked)	iTemp = 1;
    else iTemp = 0;
    strTemp = "test_cond[" + IntToStr(6) + "]=" + IntToStr(iTemp);
    fputs(strTemp.c_str(), pFile);
    fputc('\n', pFile);

    if (true == m_pChbHTNV->Checked)	iTemp = 1;
    else iTemp = 0;
    strTemp = "test_cond[" + IntToStr(7) + "]=" + IntToStr(iTemp);
    fputs(strTemp.c_str(), pFile);
    fputc('\n', pFile);

    if (true == m_pChbHTHV->Checked)	iTemp = 1;
    else iTemp = 0;
    strTemp = "test_cond[" + IntToStr(8) + "]=" + IntToStr(iTemp);
    fputs(strTemp.c_str(), pFile);
    fputc('\n', pFile); 

	// E: ACU Setting
    // F: 备选测试项
    for (int j = 0; j < MES_ITEM_REAL; j++)
    {
        strTemp = "test_step[" + IntToStr(j) + "]=" + IntToStr(m_iItemCurr[j]);
		fputs(strTemp.c_str(), pFile);
		fputc('\n', pFile);
    }

    // G Crash & Chamber Control
    if(1 == m_pRgCouttype->ItemIndex)	iTemp = 1;
    else	iTemp = 0;
    strTemp = IntToStr(iTemp);
    fputs(strTemp.c_str(), pFile);
    fputc('\n', pFile);
    if(true == m_pChbChamAuto->Checked)	iTemp = 1;
    else	iTemp = 0;
    strTemp = IntToStr(iTemp);
    fputs(strTemp.c_str(), pFile);
    fputc('\n', pFile);

	fclose(pFile);
}

///////////////////////////////////////////////////////////////////////////////
// U:05 文件不存在或第一次运行程序：
// 	温度、电压、温度延迟、温度操作顺序、温度记录时间间隔控件获取缺省值
void __fastcall TFormInitSet::GetTargetValue()
{
	TIniFile	*ini;
	ini = new TIniFile(g_strAppPath + "\\config\\config.ini");
    // 温度设置
    String strTemp = FloatToStr(ini->ReadFloat("Default TargetValue", "TEMP_LOW", 0));
	m_pLedtLowTemp->Text = strTemp;
    strTemp = FloatToStr(ini->ReadFloat("Default TargetValue", "TEMP_NOML", 0));
	m_pLedtNorTemp->Text = strTemp;
    strTemp = FloatToStr(ini->ReadFloat("Default TargetValue", "TEMP_HIGH", 0));
	m_pLedtHighTemp->Text = strTemp;

    // 电压设置
    strTemp = FloatToStr(ini->ReadFloat("Default TargetValue", "VOLT_LOW", 0));
	m_pLedtLowVolt->Text = strTemp;
    strTemp = FloatToStr(ini->ReadFloat("Default TargetValue", "VOLT_NOML", 0));
	m_pLedtNorVolt->Text = strTemp;
    strTemp = FloatToStr(ini->ReadFloat("Default TargetValue", "VOLT_HIGH", 0));
	m_pLedtHighVolt->Text = strTemp;

    // 温度延迟设置
    strTemp = FloatToStr(ini->ReadFloat("Default TargetValue", "TEMP_LOW_DELAY", 0));
	m_pLedtLTDelay->Text = strTemp;
    strTemp = FloatToStr(ini->ReadFloat("Default TargetValue", "TEMP_NOML_DELAY", 0));
	m_pLedtNTDelay->Text = strTemp;
    strTemp = FloatToStr(ini->ReadFloat("Default TargetValue", "TEMP_HIGH_DELAY", 0));
	m_pLedtHTDelay->Text = strTemp;

    // 温度操作顺序设置
    int iaTempSeq[3] = {0};
    strTemp = FloatToStr(ini->ReadFloat("Default TargetValue", "TempSeq_1", 0));
	iaTempSeq[0] = strTemp.ToInt();
    strTemp = FloatToStr(ini->ReadFloat("Default TargetValue", "TempSeq_2", 0));
	iaTempSeq[1] = strTemp.ToInt();
    strTemp = FloatToStr(ini->ReadFloat("Default TargetValue", "TempSeq_3", 0));
	iaTempSeq[2] = strTemp.ToInt();
    m_pLstbTempSeq->Items->Clear();
    int iTempMode = 0;
    for(int i = 0; i < 3; i++)
    {
        iTempMode = iaTempSeq[i];
        if(0 == iTempMode)		m_pLstbTempSeq->Items->Add("Low Temp");
        else if(1 == iTempMode)	m_pLstbTempSeq->Items->Add("Nor Temp");
        else if(2 == iTempMode)	m_pLstbTempSeq->Items->Add("High Temp");
    }

    // 温度记录时间间隔
    strTemp = IntToStr(ini->ReadInteger("Default TargetValue", "Interval_Sample", 10));
	m_pLedtSampleInterval->Text = strTemp;
	delete ini;
}

///////////////////////////////////////////////////////////////////////////////
// U:06 读文件一行
void __fastcall TFormInitSet::GetLineFromFile(FILE * pFile, String & strTemp, int &iEqual)
{
	char	cTemp[50] = { "\0" };
	for (int m = 0; m < 50; m++)
	{
		cTemp[m] = '\0';
	}

	int j = 0;
	strTemp = "\0";
	fgets(cTemp, 50, pFile);
	while (cTemp[j] != '\n' && (j < 49))
	{
		if (cTemp[j] == '=') iEqual = j + 1;
		j++;	// j记录\n的位置
	}

	for (int k = 0; k < j; k++)
	{
		strTemp = strTemp + cTemp[k];
	}
}

///////////////////////////////////////////////////////////////////////////////
// U:07 TListBox的移动
void __fastcall TFormInitSet::SetItem(TListBox * pList, int iIndex)
{
	int iMaxItem = 0;

	pList->SetFocus();
	iMaxItem = pList->Items->Count - 1;

	if (iIndex == LB_ERR) iIndex = 0;
	else if (iIndex > iMaxItem)
		iIndex = iMaxItem;
	pList->Selected[iIndex] = true;

    while (iMaxItem != -1 && pList->Items->Strings[iIndex].operator[](1) == 'A')
    {
        iIndex--;
        if(iIndex < 0)
        	break;
        pList->Selected[iIndex] = true;
    }

    bool bSrcEmpty = false, bMidEmpty = false;

	bSrcEmpty = (m_pLstbOri->Items->Count == 0);
	bMidEmpty = (m_pLstbMid->Items->Count == 0);
	m_pSpBtnOMRight->Enabled = !bSrcEmpty;
	m_pSpBtnOMRightAll->Enabled = !bSrcEmpty;
	m_pSpBtnOMLeft->Enabled = !bMidEmpty;
	m_pSpBtnOMLeftAll->Enabled = !bMidEmpty;
    m_pBtnMSRight->Enabled=!bMidEmpty;
}

///////////////////////////////////////////////////////////////////////////////
// U:08 TListBox的移动
int __fastcall TFormInitSet::GetFirstSelInList(TCustomListBox * pList)
{
	int i;

	for (i = 0; i < pList->Items->Count; i++)
	{
		if (pList->Selected[i]) return i;
	}

	return LB_ERR;
}

///////////////////////////////////////////////////////////////////////////////
// U:09 TListBox的移动
void __fastcall TFormInitSet::MoveInList(TCustomListBox * pList, TStrings * pItems)
{
	for (int i = pList->Items->Count - 1; i >= 0; i--)
	{
		if (pList->Selected[i])
		{
			pItems->AddObject(pList->Items->Strings[i], pList->Items->Objects[i]);
			pList->Items->Delete(i);
		}
	}
}

///////////////////////////////////////////////////////////////////////////////
// U:10 清空Form和g_STestInfo
// 		当程序上电运行从文件读取初始化信息或载入以前的设置时，如果文件打开失败，
// 		清空除温度、电压、温度延迟、记录时间、操作顺序外的初始化界面和相应的全
//		局变量值
void __fastcall TFormInitSet::ResetFormAndStruct()
{
    int i = 0;
    // 测试信息
	m_pLedtTestName->Text = "\0";
	m_pLedtModelName->Text = "\0";
	m_pLedtRemark->Text = "\0";
    for (i = 0; i < 3; i++)
	{
		g_STestInfo.m_strInfo[i] = "\0";
	}

    // ID信息
    m_pLedtID1->Text = "\0";
	m_pLedtID2->Text = "\0";
	m_pLedtID3->Text = "\0";
	m_pLedtID4->Text = "\0";
	m_pLedtID5->Text = "\0";
	m_pLedtID6->Text = "\0";
	for (i = 0; i < 6; i++)
	{
		g_STestInfo.m_strEcuId[i] = "\0";
	}

    // TV信息
    EnableCond(false);
	for (i = 0; i < 9; i++)
	{
		g_STestInfo.m_iTVCond[i] = 0;
	}

    // Item选择信息
    EnableACUID(false);
    m_pLstbSel->Items->Clear();
	for (i = 0; i < MES_ITEM_REAL; i++)
	{
		g_STestInfo.m_iItem[i] = 0;
	}
	g_STestInfo.m_iSelACU = 0;

    // 温箱和Crash
	m_pChbChamAuto->Checked = true;
	m_pRgCouttype->ItemIndex = 0;
	g_STestInfo.m_iChamType = 1;
	g_STestInfo.m_iDaqType = 0;
}

///////////////////////////////////////////////////////////////////////////////
// U:11 文件到全局变量g_STestInfo
void __fastcall TFormInitSet::StructFromFile(String strPath)
{
	FILE *pFile = NULL;
    String strTemp = "";
	pFile = fopen(strPath.c_str(), "r");
	if (pFile == NULL)
	{
		ShowMessage("Unable to open the latest setting file to get initial information!");
		GetTargetValue();
		ResetFormAndStruct();
        RefreshStructFromForm();
		m_pBtnDel->Enabled = false;
		m_pBtnSave->Enabled = false;
		m_pRgCouttype->Enabled = true;

		return;
	}

	// 各种温度3、电压3、采样率1、延迟时间3、ACU信息3、ACU ID 6共19个
    int iEqual = 0;
    // 温度信息
	for (int i = 0; i < 3; i++)
	{
    	iEqual = 0;
    	strTemp = "\0";
		GetLineFromFile(pFile, strTemp, iEqual);

        g_STestInfo.m_strTemp[i] = strTemp;
	}

    // 电压信息
    for (int i = 0; i < 3; i++)
	{
    	iEqual = 0;
    	strTemp = "\0";
		GetLineFromFile(pFile, strTemp, iEqual);

        g_STestInfo.m_strVolt[i] = strTemp;
	}

    // 时间延迟LT、NT、HT
    for (int i = 0; i < 3; i++)
	{
    	iEqual = 0;
    	strTemp = "\0";
		GetLineFromFile(pFile, strTemp, iEqual);

        g_STestInfo.m_strDelay[i] = strTemp;
	}

    // 时间间隔
    for (int i = 0; i < 1; i++)
	{
    	iEqual = 0;
    	strTemp = "\0";
		GetLineFromFile(pFile, strTemp, iEqual);

        g_STestInfo.m_strInterval = strTemp;
	}

    // 温度操作顺序
    for (int i = 0; i < 3; i++)
	{
    	iEqual = 0;
    	strTemp = "\0";
		GetLineFromFile(pFile, strTemp, iEqual);
        int iTempSeq = StrToInt(strTemp);

        g_STestInfo.m_iTempSeq[i] = iTempSeq;
	}

    // 测试信息
    for (int i = 0; i < 3; i++)
	{
    	iEqual = 0;
    	strTemp = "\0";
		GetLineFromFile(pFile, strTemp, iEqual);

        g_STestInfo.m_strInfo[i] = strTemp;
	}

    // ECU号设置
    for (int i = 0; i < 6; i++)
	{
    	iEqual = 0;
    	strTemp = "\0";
		GetLineFromFile(pFile, strTemp, iEqual);

		if (strTemp == "000")
        	g_STestInfo.m_strEcuId[i] = "\0";
		else
        	g_STestInfo.m_strEcuId[i] = strTemp;
	}

    // 放到 ThreadMain中去
	// float m_fPrecision[4]; // 对应test.ini文件中的测试精度
	// float m_fAmendRes[8][4]; // 接触线等电阻，进行修正处理

	// m_iTVCond[9] 测试条件：温度电压的九种组合情况
	for (int i = 0; i < 9; i++)
	{
    	iEqual = 0;
    	strTemp = "\0";
		GetLineFromFile(pFile, strTemp, iEqual);
        strTemp = strTemp.operator [](iEqual + 1);	// based 1
		g_STestInfo.m_iTVCond[i] = StrToInt(strTemp);
	}

	// m_iItem[MES_ITEM_REAL] 测试项信息
	for (int i = 0; i < MES_ITEM_REAL; i++)
	{
    	iEqual = 0;
    	strTemp = "\0";
		GetLineFromFile(pFile, strTemp, iEqual);

        int iLength = strTemp.Length();
        String strItem = "";
		for(; iEqual < iLength; iEqual++)
        {
        	strItem = strItem + strTemp.operator [](iEqual + 1);
        }

		g_STestInfo.m_iItem[i] = StrToInt(strItem);
        m_iItemCurr[i] = StrToInt(strItem);
	}

    // m_iSelACU 被选ACU子板号
	g_STestInfo.m_iSelACU = 0x00;
	for (int i = 0; i < MES_ITEM_REAL; i++)
	{
		g_STestInfo.m_iSelACU = g_STestInfo.m_iSelACU | g_STestInfo.m_iItem[i];
	}

	// m_iDaqType DAQ输出类型 0 advanced 1 conventional
    iEqual = 0;
	strTemp = "\0";
    GetLineFromFile(pFile, strTemp, iEqual);
	strTemp = strTemp.Trim();
	g_STestInfo.m_iDaqType = StrToInt(strTemp);

	// m_iChamType 温箱标识 0 不启动温箱 1 启动温箱
    iEqual = 0;
	strTemp = "\0";
    GetLineFromFile(pFile, strTemp, iEqual);
	strTemp = strTemp.Trim();
	g_STestInfo.m_iChamType = StrToInt(strTemp);

	fclose(pFile);
}

///////////////////////////////////////////////////////////////////////////////
// U:12 全局变量g_STestInfo到控件
void __fastcall TFormInitSet::RefreshFormFromStruct()
{
	// A: Target Value Setting
	m_pLedtLowTemp->Text = g_STestInfo.m_strTemp[0];
	m_pLedtNorTemp->Text = g_STestInfo.m_strTemp[1];
	m_pLedtHighTemp->Text = g_STestInfo.m_strTemp[2];
	m_pLedtLowVolt->Text = g_STestInfo.m_strVolt[0];
	m_pLedtNorVolt->Text = g_STestInfo.m_strVolt[1];
	m_pLedtHighVolt->Text = g_STestInfo.m_strVolt[2];
	m_pLedtLTDelay->Text = g_STestInfo.m_strDelay[0];
	m_pLedtNTDelay->Text = g_STestInfo.m_strDelay[1];
	m_pLedtHTDelay->Text = g_STestInfo.m_strDelay[2];
    // 温度操作顺序
    m_pLstbTempSeq->Items->Clear();
    int iTempMode = 0;
    for(int i = 0; i < 3; i++)
    {
        iTempMode = g_STestInfo.m_iTempSeq[i];
        if(0 == iTempMode)		m_pLstbTempSeq->Items->Add("Low Temp");
        else if(1 == iTempMode)	m_pLstbTempSeq->Items->Add("Nor Temp");
        else if(2 == iTempMode)	m_pLstbTempSeq->Items->Add("High Temp");
    }
	m_pLedtSampleInterval->Text = g_STestInfo.m_strInterval;

	// B: Test Information
	// C: ECU ID
	m_pLedtTestName->Text = g_STestInfo.m_strInfo[0];
	m_pLedtModelName->Text = g_STestInfo.m_strInfo[1];
	m_pLedtRemark->Text = g_STestInfo.m_strInfo[2];
	m_pLedtID1->Text = g_STestInfo.m_strEcuId[0];
	m_pLedtID2->Text = g_STestInfo.m_strEcuId[1];
	m_pLedtID3->Text = g_STestInfo.m_strEcuId[2];
	m_pLedtID4->Text = g_STestInfo.m_strEcuId[3];
	m_pLedtID5->Text = g_STestInfo.m_strEcuId[4];
	m_pLedtID6->Text = g_STestInfo.m_strEcuId[5];

	// D: Test Condition Setting
	// m_iTVCond[9]
	if (g_STestInfo.m_iTVCond[0]) m_pChbLTLV->Checked = true;
	else m_pChbLTLV->Checked = false;

	if (g_STestInfo.m_iTVCond[1]) m_pChbLTNV->Checked = true;
	else m_pChbLTNV->Checked = false;

	if (g_STestInfo.m_iTVCond[2]) m_pChbLTHV->Checked = true;
	else m_pChbLTHV->Checked = false;

	if (g_STestInfo.m_iTVCond[3]) m_pChbNTLV->Checked = true;
	else m_pChbNTLV->Checked = false;

	if (g_STestInfo.m_iTVCond[4]) m_pChbNTNV->Checked = true;
	else m_pChbNTNV->Checked = false;

	if (g_STestInfo.m_iTVCond[5]) m_pChbNTHV->Checked = true;
	else m_pChbNTHV->Checked = false;

	if (g_STestInfo.m_iTVCond[6]) m_pChbHTLV->Checked = true;
	else m_pChbHTLV->Checked = false;

	if (g_STestInfo.m_iTVCond[7]) m_pChbHTNV->Checked = true;
	else m_pChbHTNV->Checked = false;

	if (g_STestInfo.m_iTVCond[8]) m_pChbHTHV->Checked = true;
	else m_pChbHTHV->Checked = false;

	// E: ACU Setting
	if (0x01 == (g_STestInfo.m_iSelACU & 0x01)) m_pChbACU1->Checked = true;
    else	m_pChbACU1->Checked = false;
	if (0x02 == (g_STestInfo.m_iSelACU & 0x02)) m_pChbACU2->Checked = true;
    else	m_pChbACU2->Checked = false;
	if (0x04 == (g_STestInfo.m_iSelACU & 0x04)) m_pChbACU3->Checked = true;
    else	m_pChbACU3->Checked = false;
	if (0x08 == (g_STestInfo.m_iSelACU & 0x08)) m_pChbACU4->Checked = true;
    else	m_pChbACU4->Checked = false;
	if (0x10 == (g_STestInfo.m_iSelACU & 0x10)) m_pChbACU5->Checked = true;
    else	m_pChbACU5->Checked = false;
	if (0x20 == (g_STestInfo.m_iSelACU & 0x20)) m_pChbACU6->Checked = true;
    else m_pChbACU6->Checked = false;

    // F: 备选测试项
	int iCount = 0, iShift = 0x01;
	m_pLstbSel->Items->Clear();
	for (int i = 1; i <= MES_SUBBOARD_NUM; i++)
	{
		m_pLstbSel->Items->Strings[iCount] = "ACU" + IntToStr(i) + ":";
		iCount++;
		for (int j = 0; j < MES_ITEM_REAL; j++)
		{
			if ((g_STestInfo.m_iItem[j] & iShift << (i - 1)) == iShift << (i - 1))
			{
				m_pLstbSel->Items->Strings[iCount] = g_StepText[j];
				iCount++;
			}
		}
	}

    // G Crash & Chamber Control
	m_pRgCouttype->ItemIndex = g_STestInfo.m_iDaqType;
	if (g_STestInfo.m_iChamType) m_pChbChamAuto->Checked = true;
	else m_pChbChamAuto->Checked = false;

    m_pBtnSave->Enabled = false;
}

///////////////////////////////////////////////////////////////////////////////
// U:13 控件到全局变量g_STestInfo
void __fastcall TFormInitSet::RefreshStructFromForm()
{
	// A: Target Value Setting
    String strTemp = "";
    g_STestInfo.m_strTemp[0] = m_pLedtLowTemp->Text;
    g_STestInfo.m_strTemp[1] = m_pLedtNorTemp->Text;
    g_STestInfo.m_strTemp[2] = m_pLedtHighTemp->Text;
    g_STestInfo.m_strVolt[0] = m_pLedtLowVolt->Text;
    g_STestInfo.m_strVolt[1] = m_pLedtNorVolt->Text;
    g_STestInfo.m_strVolt[2] = m_pLedtHighVolt->Text;
    g_STestInfo.m_strDelay[0] = m_pLedtLTDelay->Text;
    g_STestInfo.m_strDelay[1] = m_pLedtNTDelay->Text;
    g_STestInfo.m_strDelay[2] = m_pLedtHTDelay->Text;
    for(int i = 0; i < 3; i++)
    {
		strTemp = m_pLstbTempSeq->Items->Strings[i];
        if(strTemp == "Low Temp")		g_STestInfo.m_iTempSeq[i] = 0;
        else if(strTemp == "Nor Temp") 	g_STestInfo.m_iTempSeq[i] = 1;
        else if(strTemp == "High Temp") g_STestInfo.m_iTempSeq[i] = 2;
    }
    g_STestInfo.m_strInterval = m_pLedtSampleInterval->Text;

	// B: Test Information
	// C: ECU ID
    g_STestInfo.m_strInfo[0] = m_pLedtTestName->Text;
    g_STestInfo.m_strInfo[1] = m_pLedtModelName->Text;
    g_STestInfo.m_strInfo[2] = m_pLedtRemark->Text;

	strTemp = m_pLedtID1->Text;
    if(strTemp == "") 	strTemp = "000";
    g_STestInfo.m_strEcuId[0] = strTemp;
	strTemp = m_pLedtID2->Text;
    if(strTemp == "") 	strTemp = "000";
    g_STestInfo.m_strEcuId[1] = strTemp;
	strTemp = m_pLedtID3->Text;
    if(strTemp == "") 	strTemp = "000";
    g_STestInfo.m_strEcuId[2] = strTemp;
	strTemp = m_pLedtID4->Text;
    if(strTemp == "") 	strTemp = "000";
    g_STestInfo.m_strEcuId[3] = strTemp;
	strTemp = m_pLedtID5->Text;
    if(strTemp == "") 	strTemp = "000";
    g_STestInfo.m_strEcuId[4] = strTemp;
	strTemp = m_pLedtID6->Text;
    if(strTemp == "") 	strTemp = "000";
    g_STestInfo.m_strEcuId[5] = strTemp;

	// D: Test Condition Setting
	// m_iTVCond[9]
    int iTemp = 0;
    if (true == m_pChbLTLV->Checked)	iTemp = 1;
    else iTemp = 0;
    g_STestInfo.m_iTVCond[0] = iTemp;

    if (true == m_pChbLTNV->Checked)	iTemp = 1;
    else iTemp = 0;
    g_STestInfo.m_iTVCond[1] = iTemp;

    if (true == m_pChbLTHV->Checked)	iTemp = 1;
    else iTemp = 0;
    g_STestInfo.m_iTVCond[2] = iTemp;

    if (true == m_pChbNTLV->Checked)	iTemp = 1;
    else iTemp = 0;
    g_STestInfo.m_iTVCond[3] = iTemp;

    if (true == m_pChbNTNV->Checked)	iTemp = 1;
    else iTemp = 0;
    g_STestInfo.m_iTVCond[4] = iTemp;

    if (true == m_pChbNTHV->Checked)	iTemp = 1;
    else iTemp = 0;
    g_STestInfo.m_iTVCond[5] = iTemp;

    if (true == m_pChbHTLV->Checked)	iTemp = 1;
    else iTemp = 0;
    g_STestInfo.m_iTVCond[6] = iTemp;

    if (true == m_pChbHTNV->Checked)	iTemp = 1;
    else iTemp = 0;
    g_STestInfo.m_iTVCond[7] = iTemp;

    if (true == m_pChbHTHV->Checked)	iTemp = 1;
    else iTemp = 0;
    g_STestInfo.m_iTVCond[8] = iTemp;

	// E: ACU Setting
    // F: 备选测试项
    g_STestInfo.m_iSelACU = 0x00;
    for (int j = 0; j < MES_ITEM_REAL; j++)
    {
        g_STestInfo.m_iItem[j] = m_iItemCurr[j];
        g_STestInfo.m_iSelACU = g_STestInfo.m_iSelACU | g_STestInfo.m_iItem[j];
    }

    // G Crash & Chamber Control
    if(1 == m_pRgCouttype->ItemIndex)	iTemp = 1;
    else	iTemp = 0;
    g_STestInfo.m_iDaqType = iTemp;

    if(true == m_pChbChamAuto->Checked)	iTemp = 1;
    else	iTemp = 0;
    g_STestInfo.m_iChamType = iTemp;
}

///////////////////////////////////////////////////////////////////////////////
// U:14 测试项m_pLstbSel到m_iItemCurr
void __fastcall TFormInitSet::SetItemCurrInfo()
{
	int		iItem = 0;
	int		iSelACU = 0;
	String	strItem = "";

	for (int i = 0; i < MES_ITEM_REAL; i++)
    {
    	m_iItemCurr[i] = 0;
    }

    if (m_pChbACU1->Checked) iSelACU = iSelACU | 0x01;
	if (m_pChbACU2->Checked) iSelACU = iSelACU | 0x02;
	if (m_pChbACU3->Checked) iSelACU = iSelACU | 0x04;
	if (m_pChbACU4->Checked) iSelACU = iSelACU | 0x08;
	if (m_pChbACU5->Checked) iSelACU = iSelACU | 0x10;
	if (m_pChbACU6->Checked) iSelACU = iSelACU | 0x20;

	for (int i = 0; i < m_pLstbMid->Items->Count; i++)
	{
		strItem = m_pLstbMid->Items->Strings[i].SubString(1, 2);
		iItem = StrToInt(strItem);
		m_iItemCurr[iItem - 1] = m_iItemCurr[iItem - 1] | iSelACU;
	}
}


