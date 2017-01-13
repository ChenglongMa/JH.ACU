using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;

namespace JH.ACU.BLL.Interface
{
    public interface IAcuCommunicates
    {
        bool Start();
        bool Initialize();
        byte GetCheckum(byte[] data);
        byte[] SendandRead(byte[] sendData);
        bool Stop();
    }
}
