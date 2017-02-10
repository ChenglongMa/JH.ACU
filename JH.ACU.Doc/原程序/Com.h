#ifndef ComH
#define ComH

#include <windows.h>
#include <vcl.h>

class TCom
{
private:

protected:
	bool m_bComOpen;
    HANDLE m_hCom;

public:
  __fastcall TCom();
  __fastcall ~TCom();

public:
	String m_strMark;

public:
  virtual bool __fastcall Open();
  virtual bool __fastcall Close();

};

#endif
 