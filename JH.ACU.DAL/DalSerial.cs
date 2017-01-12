using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Ivi.Visa;
using NationalInstruments.Visa;

namespace JH.ACU.DAL
{
    /// <summary>
    /// 串口基础类
    /// </summary>
    public class DalSerial : IDalVisa
    {
        public DalSerial(string name, int baudRate = 9600)
        {
            MbSession = new SerialSession(name) { BaudRate = baudRate };
            RawIo = MbSession.RawIO;
        }

        public MessageBasedSession MbSession { get; set; }
        public IMessageBasedRawIO RawIo { get; set; }
    }
}