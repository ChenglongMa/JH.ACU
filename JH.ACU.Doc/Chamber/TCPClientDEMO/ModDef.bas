Attribute VB_Name = "ModDef"
Option Explicit
Public Declare Sub CopyMemory Lib "kernel32" Alias "RtlMoveMemory" (Destination As Any, Source As Any, ByVal Length As Long)

'sckClosed 0 缺省的。关闭
'sckOpen 1 打开
'sckListening 2 侦听
'sckConnectionPending  3 连接挂起
'sckResolvingHost  4 识别主机
'sckHostResolved  5 已识别主机
'sckConnecting  6 正在连接
'sckConnected  7 已连接
'sckClosing  8 同级人员正在关闭连接
'sckError  9 错误

