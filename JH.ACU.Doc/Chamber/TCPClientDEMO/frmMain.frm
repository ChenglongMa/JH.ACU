VERSION 5.00
Object = "{248DD890-BB45-11CF-9ABC-0080C7E7B78D}#1.0#0"; "MSWINSCK.OCX"
Begin VB.Form frmMain 
   Caption         =   " ModbusTCP Sample"
   ClientHeight    =   4560
   ClientLeft      =   60
   ClientTop       =   435
   ClientWidth     =   6465
   LinkTopic       =   "Form1"
   ScaleHeight     =   4560
   ScaleWidth      =   6465
   StartUpPosition =   3  '窗口缺省
   Begin VB.Timer Timer2 
      Interval        =   500
      Left            =   7800
      Top             =   2400
   End
   Begin VB.CommandButton Command9 
      Caption         =   "OFF"
      Height          =   375
      Left            =   5040
      TabIndex        =   18
      Top             =   2520
      Width           =   855
   End
   Begin VB.CommandButton Command8 
      Caption         =   "ON"
      Height          =   375
      Left            =   5040
      TabIndex        =   17
      Top             =   2040
      Width           =   855
   End
   Begin VB.TextBox Text2 
      Height          =   375
      Left            =   5040
      TabIndex        =   16
      Top             =   1560
      Width           =   855
   End
   Begin VB.CommandButton Command7 
      Caption         =   "设置"
      Height          =   375
      Left            =   5040
      TabIndex        =   15
      Top             =   1080
      Width           =   855
   End
   Begin VB.CommandButton Command6 
      Caption         =   "实时"
      Height          =   375
      Left            =   4080
      TabIndex        =   14
      Top             =   2040
      Width           =   855
   End
   Begin VB.CommandButton Command5 
      Caption         =   "实时"
      Height          =   375
      Left            =   4080
      TabIndex        =   13
      Top             =   1080
      Width           =   855
   End
   Begin VB.TextBox txtValueS 
      Height          =   375
      Left            =   2280
      TabIndex        =   10
      Top             =   2520
      Width           =   2655
   End
   Begin VB.TextBox txtIndexS 
      Alignment       =   2  'Center
      Height          =   375
      Left            =   2280
      TabIndex        =   9
      Text            =   "0"
      Top             =   2040
      Width           =   735
   End
   Begin VB.CommandButton Command4 
      Caption         =   "查找"
      Height          =   375
      Left            =   3120
      TabIndex        =   8
      Top             =   2040
      Width           =   855
   End
   Begin VB.CommandButton Command3 
      Caption         =   "查找"
      Height          =   375
      Left            =   3120
      TabIndex        =   6
      Top             =   1080
      Width           =   855
   End
   Begin VB.TextBox txtIndexA 
      Alignment       =   2  'Center
      Height          =   375
      Left            =   2280
      TabIndex        =   5
      Text            =   "0"
      Top             =   1080
      Width           =   735
   End
   Begin VB.Timer Timer1 
      Interval        =   1000
      Left            =   8400
      Top             =   2400
   End
   Begin MSWinsockLib.Winsock wskClient 
      Left            =   8520
      Top             =   1680
      _ExtentX        =   741
      _ExtentY        =   741
      _Version        =   393216
   End
   Begin VB.TextBox txtValueA 
      Height          =   375
      Left            =   2280
      TabIndex        =   2
      Top             =   1560
      Width           =   2655
   End
   Begin VB.CommandButton Command1 
      Caption         =   "连接控制器:端口号默认为 3000"
      Height          =   375
      Left            =   240
      TabIndex        =   1
      Top             =   600
      Width           =   5655
   End
   Begin VB.TextBox Text1 
      Height          =   375
      Left            =   1560
      TabIndex        =   0
      Text            =   "200.200.200.190"
      Top             =   120
      Width           =   4335
   End
   Begin VB.Label Label8 
      Caption         =   "若当前开关量在协议地址表中标识为读写,则[ON][OFF]按钮来改变当前状态"
      Height          =   495
      Left            =   240
      TabIndex        =   21
      Top             =   3840
      Width           =   7335
   End
   Begin VB.Label Label7 
      Caption         =   "若当前数据在协议地址表中标识为读写,则[设置]按钮用来改变当前值"
      Height          =   255
      Left            =   240
      TabIndex        =   20
      Top             =   3480
      Width           =   7335
   End
   Begin VB.Label Label6 
      Caption         =   "注:数据索引号为协议地址表的序号减1"
      Height          =   255
      Left            =   240
      TabIndex        =   19
      Top             =   3120
      Width           =   7335
   End
   Begin VB.Label Label5 
      Caption         =   "开关量数据索引(0-116)"
      Height          =   255
      Left            =   240
      TabIndex        =   12
      Top             =   2160
      Width           =   2055
   End
   Begin VB.Label Label4 
      Caption         =   "开关量数据数值显示："
      Height          =   255
      Left            =   240
      TabIndex        =   11
      Top             =   2640
      Width           =   1935
   End
   Begin VB.Label Label3 
      Caption         =   "模拟量数据数值显示："
      Height          =   255
      Left            =   240
      TabIndex        =   7
      Top             =   1680
      Width           =   1935
   End
   Begin VB.Label Label2 
      Caption         =   "模拟量数据索引(0-56)"
      Height          =   255
      Left            =   240
      TabIndex        =   4
      Top             =   1200
      Width           =   2055
   End
   Begin VB.Label Label1 
      Caption         =   "服务器IP地址:"
      Height          =   255
      Left            =   240
      TabIndex        =   3
      Top             =   240
      Width           =   1215
   End
End
Attribute VB_Name = "frmMain"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
Option Explicit

Dim rFlag As Byte
Dim DataValueA() As Single
Dim DataValueS() As Boolean
Dim RTDataA As Boolean
Dim RTDataS As Boolean

Private Sub Command1_Click()
    wskClient.Close
    wskClient.Connect Text1.Text, 3000
End Sub

Private Sub Command2_Click()
    If wskClient.State = 7 Then
        rFlag = 0
        ModBusTCPSendEx 0, 3, 0, 57
    End If
End Sub

Private Sub Command3_Click()
    On Error GoTo errhandle
    txtValueA.Text = iDataA(txtIndexA)
    Exit Sub
errhandle:
        MsgBox "索引超出范围!", vbInformation, "提示"
End Sub

Private Sub Command4_Click()
    On Error GoTo errhandle
    txtValueS.Text = iDataS(txtIndexS)
    Exit Sub
errhandle:
        MsgBox "索引超出范围!", vbInformation, "提示"
End Sub

Private Sub Command5_Click()
    RTDataA = Not RTDataA
End Sub

Private Sub Command6_Click()
    RTDataS = Not RTDataS
End Sub

Private Sub Command7_Click()
    Dim pData As Long
    Dim Value As Integer
    Value = Val(Text2.Text)
    pData = VarPtr(Value)
    ModBusTCPSendEx 0, 16, txtIndexA.Text, 1, pData
End Sub

Private Sub Command8_Click()
    Dim s As Byte
    Dim pData As Long
    s = 1
    pData = VarPtr(s)
    ModBusTCPSendEx 0, 15, txtIndexS.Text, 1, pData

End Sub

Private Sub Command9_Click()
    Dim s As Byte
    Dim pData As Long
    s = 0
    pData = VarPtr(s)
    ModBusTCPSendEx 0, 15, txtIndexS.Text, 1, pData
End Sub

Private Sub Form_Load()
    Timer1.Enabled = True
    Command3.Enabled = False
    Command4.Enabled = False
    Command5.Enabled = False
    Command6.Enabled = False
End Sub

Private Sub Timer1_Timer()
    Select Case wskClient.State
        Case 0
            Me.Caption = " ModbusTCP Sample - state: " + " Connect Close"
        Case 1
            Me.Caption = " ModbusTCP Sample - state: " + " Connect Open"
        Case 2
            Me.Caption = " ModbusTCP Sample - state: " + " Connect Listen"
        Case 3
            Me.Caption = " ModbusTCP Sample - state: " + " Connect Pending"
        Case 4
            Me.Caption = " ModbusTCP Sample - state: " + " Resolving Host"
        Case 5
            Me.Caption = " ModbusTCP Sample - state: " + " Host Resolved"
        Case 6
            Me.Caption = " ModbusTCP Sample - state: " + " Now Connecting"
        Case 7
            Me.Caption = " ModbusTCP Sample - state: " + " Connect Success"
            ModBusTCPSendEx 0, 3, 0, 57
        Case 8
            Me.Caption = " ModbusTCP Sample - state: " + " Closeing Connect"
        Case 9
            Me.Caption = " ModbusTCP Sample - state: " + " Err"
    End Select
End Sub


Private Sub wskClient_DataArrival(ByVal bytesTotal As Long)
    Dim bData() As Byte
    Dim func_code As Byte
    
    ReDim bData(bytesTotal - 1) As Byte
    
    wskClient.GetData bData, vbByte
    
    func_code = bData(7)
    
    Select Case func_code
    Case 1
        
        SwitchDataRead bData

        If RTDataS Then
            txtValueS.Text = iDataS(txtIndexS)
            txtIndexS.Enabled = False
        Else
            txtIndexS.Enabled = True
        End If
        If Command4.Enabled = False Then Command4.Enabled = True
        If Command6.Enabled = False Then Command6.Enabled = True

    Case 3
        AnalogDataRead bData
                     
        If RTDataA Then
            txtValueA.Text = iDataA(txtIndexA)
            txtIndexA.Enabled = False
        Else
            txtIndexA.Enabled = True
        End If
        If Command3.Enabled = False Then Command3.Enabled = True
        If Command5.Enabled = False Then Command5.Enabled = True

        If wskClient.State = 7 Then ModBusTCPSendEx 0, 1, 0, 117
    End Select
End Sub


