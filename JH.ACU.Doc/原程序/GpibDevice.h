#ifndef GpibDeviceH
#define GpibDeviceH

#include "Adgpib_bc.h"
#include "Global.h"
#include "GpibInterface.h"

class TGpibDevice
{
private:

protected:
	int m_iInterfaceHandle;				// 接口句柄
	int m_iHandle;						// 设备句柄
    int m_iDevice;						// 设备地址
    String m_strMark;
    String m_strRetCode;

public:
  __fastcall TGpibDevice();
  __fastcall ~TGpibDevice();

public:
	bool __fastcall SetInterfaceHandle(TGpibInterface gpibInterface);   // 获取接口句柄
	void __fastcall SetPA(int iDevice); 		 	    // 设定设备主地址
  	void __fastcall GetPA(int &iDevice);			    // 获取设备主地址
  	bool __fastcall SetHandle();                        // 通过主地址设定句柄
  	bool __fastcall GetHandle(int &iHandle); 		    // 获取句柄

    // 基本操作
  	bool __fastcall GetIDN();
  	bool __fastcall SetRST();
  	bool __fastcall SetTST();

  	// 通过子类来继承实现，其控制逻辑由程序逻辑控制
  	virtual bool __fastcall Init(TGpibInterface gpibInterface, int iDevice);		    // 组合SetPA和SetHandle
    virtual bool __fastcall Close();

    // 填充错误代码
    void __fastcall SetRetCode();
};

#endif
