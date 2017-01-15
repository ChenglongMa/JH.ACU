using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Ivi.Visa;
using JH.ACU.Model;
using NationalInstruments.Visa;

namespace JH.ACU.BLL
{
    public abstract class BllVisa
    {
        protected MessageBasedSession MbSession { get; set; }

        protected IMessageBasedRawIO RawIo
        {
            get { return MbSession == null ? null : MbSession.RawIO; }
        }

        protected abstract Instr Config { get; }
        protected string WriteAndRead(string command, int delay = 50)
        {
            RawIo.Write(command + "\n");
            Thread.Sleep(delay);
            return RawIo.ReadString();
        }

        protected void WriteNoRead(string command)
        {
            RawIo.Write(command + "\n");
        }

    }
}
