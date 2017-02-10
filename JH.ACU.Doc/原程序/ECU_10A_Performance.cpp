#include <vcl.h>
#pragma hdrstop

USEFORM("FormMainU.cpp", FormMain);
USEFORM("FormInitSetU.cpp", FormInitSet);
USEFORM("FormManualU.cpp", FormManual);
//---------------------------------------------------------------------------
WINAPI WinMain(HINSTANCE, HINSTANCE, LPSTR, int)
{
	try
	{
		Application->Initialize();
		Application->CreateForm(__classid(TFormMain), &FormMain);
		Application->CreateForm(__classid(TFormInitSet), &FormInitSet);
		Application->CreateForm(__classid(TFormManual), &FormManual);
		Application->Run();
	}
	catch (Exception &exception)
	{
		Application->ShowException(&exception);
	}
	catch (...)
	{
		try
		{
			throw Exception("");
		}
		catch (Exception &exception)
		{
			Application->ShowException(&exception);
		}
	}
	return 0;
}

