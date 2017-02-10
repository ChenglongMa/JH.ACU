#ifndef GpibDeviceH
#define GpibDeviceH

#include "Adgpib_bc.h"
#include "Global.h"
#include "GpibInterface.h"

class TGpibDevice
{
private:

protected:
	int m_iInterfaceHandle;				// �ӿھ��
	int m_iHandle;						// �豸���
    int m_iDevice;						// �豸��ַ
    String m_strMark;
    String m_strRetCode;

public:
  __fastcall TGpibDevice();
  __fastcall ~TGpibDevice();

public:
	bool __fastcall SetInterfaceHandle(TGpibInterface gpibInterface);   // ��ȡ�ӿھ��
	void __fastcall SetPA(int iDevice); 		 	    // �趨�豸����ַ
  	void __fastcall GetPA(int &iDevice);			    // ��ȡ�豸����ַ
  	bool __fastcall SetHandle();                        // ͨ������ַ�趨���
  	bool __fastcall GetHandle(int &iHandle); 		    // ��ȡ���

    // ��������
  	bool __fastcall GetIDN();
  	bool __fastcall SetRST();
  	bool __fastcall SetTST();

  	// ͨ���������̳�ʵ�֣�������߼��ɳ����߼�����
  	virtual bool __fastcall Init(TGpibInterface gpibInterface, int iDevice);		    // ���SetPA��SetHandle
    virtual bool __fastcall Close();

    // ���������
    void __fastcall SetRetCode();
};

#endif
