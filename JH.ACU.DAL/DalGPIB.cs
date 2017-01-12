using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivi.Visa;
using NationalInstruments.Visa;

namespace JH.ACU.DAL
{
    public class DalGpib:IDalVisa
    {
        public DalGpib(string name)
        {
            MbSession = new GpibSession(name);
            RawIo = MbSession.RawIO;
        }
        public MessageBasedSession MbSession { get; set; }
        public IMessageBasedRawIO RawIo { get; set; }
    }
}
