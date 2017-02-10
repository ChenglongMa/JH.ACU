#pragma hdrstop

#include "Com.h"

#pragma package(smart_init)

__fastcall TCom::TCom()
{
	m_bComOpen = false;
	m_hCom = INVALID_HANDLE_VALUE;
	m_strMark = "";
}

__fastcall TCom::~TCom()
{

}

bool __fastcall TCom::Open()
{
	return false;
}

bool __fastcall TCom::Close()
{
	return false;
}




