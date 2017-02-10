#ifndef ComChamberH
#define ComChamberH

#define CRC16  0xA001

#include "Global.h"
#include "Com.h"

#define MES_COM_CHAMBER 	"COM2"

class TComChamber : public TCom
{
private:
    static unsigned char m_sucTableInit[256], m_sucTableResult[256];
    static unsigned char m_ucaTableF[256];		// CRC Table First
    static unsigned char m_ucaTableS[256];		// CRC Table Second
    static void __fastcall CRCInit(void); 		// Initialize the CRC table and compute the CRC bytes

private:
    static unsigned int __fastcall CRCCalc(unsigned char *buf, unsigned char size);	// Calculate the CRC
    bool __fastcall SetCommand(Byte *pcWriteBuff, int iSetTemp, float fReadTemp);

protected:

public:
  __fastcall TComChamber();
  __fastcall ~TComChamber();

public:
  virtual bool __fastcall Open();
  virtual bool __fastcall Close();

public:
    bool __fastcall SetTemp(float fTemp);
    bool __fastcall ReadTemp(float & fTemp);
    void __fastcall Wait(float fTemp);
    bool __fastcall Stop();
};

extern TComChamber g_comChamber;

#endif
