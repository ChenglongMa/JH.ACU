Attribute VB_Name = "modMBTCP"
Option Explicit

Public iDataA() As Integer
Public iDataS() As Boolean

Public Sub ModBusTCPSendEx(ByVal addr As Byte, ByVal bFunCode As Byte, ByVal regaddr As Integer, ByVal regNum As Integer, Optional ByVal pdataValue As Long)
    Dim SendByte() As Byte
    Dim regaddr_bitH As Byte
    Dim regaddr_bitL As Byte
    Dim regNum_bitH As Byte
    Dim regNum_bitL As Byte
    Dim tempbyte() As Byte
    Dim DataTemp As Byte
    Dim n As Long
    
    Dim lpVar As Long
    'On Error Resume Next
    ReDim tempbyte(3) As Byte
    Select Case bFunCode
    Case 3, 1
        ReDim SendByte(11) As Byte
        '//帧头 固定6个字节
        SendByte(0) = 0
        SendByte(1) = 0
        SendByte(2) = 0
        SendByte(3) = 0
        SendByte(4) = 0
        SendByte(5) = 6
        
        '//设备地址 默认为0
        SendByte(6) = addr
        
        '//MODBUS功能码
        SendByte(7) = bFunCode
            
        '//寄存器地址,高位在前,低位在后
        lpVar = VarPtr(regaddr)
            
        CopyMemory regaddr_bitH, ByVal lpVar + 1, 1
        SendByte(8) = regaddr_bitH
        CopyMemory regaddr_bitL, ByVal lpVar, 1
        SendByte(9) = regaddr_bitL
        
        '//寄存器数量,高位在前,低位在后
        lpVar = VarPtr(regNum)
            
        CopyMemory regNum_bitH, ByVal lpVar + 1, 1
        SendByte(10) = regNum_bitH
        CopyMemory regNum_bitL, ByVal lpVar, 1
        SendByte(11) = regNum_bitL
    Case 5
        ReDim SendByte(11) As Byte
        
        '//帧头 固定6个字节
        SendByte(0) = 0
        SendByte(1) = 0
        SendByte(2) = 0
        SendByte(3) = 0
        SendByte(4) = 0
        SendByte(5) = 6
        
        '//设备地址 默认为0
        SendByte(6) = addr
        
        '//MODBUS功能码
        SendByte(7) = bFunCode
            
        '//寄存器地址,高位在前,低位在后
        lpVar = VarPtr(regaddr)
            
        CopyMemory regaddr_bitH, ByVal lpVar + 1, 1
        SendByte(8) = regaddr_bitH
        CopyMemory regaddr_bitL, ByVal lpVar, 1
        SendByte(9) = regaddr_bitL
        
        '//开时写&HFF 关时写&H0
        CopyMemory DataTemp, ByVal pdataValue, 1
        If DataTemp Then
            SendByte(10) = &HFF
            SendByte(11) = 0
        Else
            SendByte(10) = 0
            SendByte(11) = 0
        End If
    Case 15
        Dim bytAll As Long
        Dim i As Byte
        Dim m As Byte
        
        m = regNum Mod 8
        If m Then
            bytAll = regNum \ 8 + 1
        Else
            bytAll = regNum / 8
        End If
        
        ReDim SendByte(12 + bytAll) As Byte
        '//帧头 固定6个字节
        SendByte(0) = 0
        SendByte(1) = 0
        SendByte(2) = 0
        SendByte(3) = 0
        SendByte(4) = 0
        SendByte(5) = 6
        
        '//设备地址 默认为0
        SendByte(6) = addr
        
        '//MODBUS功能码
        SendByte(7) = bFunCode
            
        '//寄存器地址,高位在前,低位在后
        lpVar = VarPtr(regaddr)
            
        CopyMemory regaddr_bitH, ByVal lpVar + 1, 1
        SendByte(8) = regaddr_bitH
        CopyMemory regaddr_bitL, ByVal lpVar, 1
        SendByte(9) = regaddr_bitL
        
        '//寄存器数量,高位在前,低位在后
        lpVar = VarPtr(regNum)
            
        CopyMemory regNum_bitH, ByVal lpVar + 1, 1
        SendByte(10) = regNum_bitH
        CopyMemory regNum_bitL, ByVal lpVar, 1
        SendByte(11) = regNum_bitL
        
        SendByte(12) = bytAll
        
        ReDim tempbyte(1) As Byte
        For n = 0 To bytAll - 1 Step 1
            For i = 0 To 7 Step 1
                If (n = bytAll And i = m) Or i = regNum Then Exit For
                CopyMemory tempbyte(1), ByVal pdataValue, 1
                tempbyte(0) = (tempbyte(1) * 2 ^ (i)) Or tempbyte(0)
                pdataValue = pdataValue + 1
            Next i
            SendByte(n + 13) = tempbyte(0)
            tempbyte(0) = 0
        Next n
    Case 16
        ReDim SendByte(13 + 2 * regNum - 1) As Byte
        
        '//帧头 固定6个字节
        SendByte(0) = 0
        SendByte(1) = 0
        SendByte(2) = 0
        SendByte(3) = 0
        SendByte(4) = 0
        SendByte(5) = 6
        
        '//设备地址 默认为0
        SendByte(6) = addr
        
        '//MODBUS功能码
        SendByte(7) = bFunCode
            
        '//寄存器地址,高位在前,低位在后
        lpVar = VarPtr(regaddr)
            
        CopyMemory regaddr_bitH, ByVal lpVar + 1, 1
        SendByte(8) = regaddr_bitH
        CopyMemory regaddr_bitL, ByVal lpVar, 1
        SendByte(9) = regaddr_bitL
        
        '//寄存器数量,高位在前,低位在后
        lpVar = VarPtr(regNum)
            
        CopyMemory regNum_bitH, ByVal lpVar + 1, 1
        SendByte(10) = regNum_bitH
        CopyMemory regNum_bitL, ByVal lpVar, 1
        SendByte(11) = regNum_bitL
        
        SendByte(12) = regNum * 2
        ReDim tempbyte(regNum * 2 - 1) As Byte
        
        For n = 0 To regNum - 1 Step 1
        '//数据段:
            CopyMemory tempbyte(n * 2 + 1), ByVal pdataValue, 1
            pdataValue = pdataValue + 1
            CopyMemory tempbyte(n * 2), ByVal pdataValue, 1
            pdataValue = pdataValue + 1
        Next n
    
        CopyMemory SendByte(13), tempbyte(0), regNum * 2
    End Select
    frmMain.wskClient.SendData SendByte
End Sub


Public Function AnalogDataRead(bData() As Byte)
    Dim n As Long
    Dim i As Long
    Dim bytAll As Byte
    Dim lpValue As Long
    Dim tempbyte(3) As Byte
    Dim DataTemp As Byte
    
    bytAll = bData(8)
    ReDim iDataA(bytAll / 2 - 1) As Integer
        
    lpValue = VarPtr(bData(9))
    i = 0
    For n = 0 To bytAll / 2 - 1 Step 1
        CopyMemory tempbyte(0), ByVal lpValue + n * 2, 2
        DataTemp = tempbyte(0)
        tempbyte(0) = tempbyte(1)
        tempbyte(1) = DataTemp
        CopyMemory iDataA(n), tempbyte(0), 2
    Next
End Function

Public Function SwitchDataRead(bData() As Byte)
    Dim n As Long
    Dim i As Long
    Dim bytAll As Byte
    Dim lpValue As Long
    Dim tempbyte(3) As Byte
    Dim DataTemp As Byte
    Dim j As Long
    Dim bytTemp() As Byte
    
    bytAll = bData(8)
    ReDim iDataS(bytAll * 8 - 1) As Boolean
    ReDim bytTemp(bytAll - 1) As Byte
    
    CopyMemory bytTemp(0), bData(9), bytAll
    j = 0
    For n = 0 To bytAll - 1 Step 1
        For i = 0 To 7 Step 1
            If bytTemp(n) And 2 ^ i Then
                iDataS(j) = True
            Else
                iDataS(j) = False
            End If
            j = j + 1
        Next i
    Next n
End Function
