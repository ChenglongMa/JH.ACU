Attribute VB_Name = "ModDef"
Option Explicit
Public Declare Sub CopyMemory Lib "kernel32" Alias "RtlMoveMemory" (Destination As Any, Source As Any, ByVal Length As Long)

'sckClosed 0 ȱʡ�ġ��ر�
'sckOpen 1 ��
'sckListening 2 ����
'sckConnectionPending  3 ���ӹ���
'sckResolvingHost  4 ʶ������
'sckHostResolved  5 ��ʶ������
'sckConnecting  6 ��������
'sckConnected  7 ������
'sckClosing  8 ͬ����Ա���ڹر�����
'sckError  9 ����

