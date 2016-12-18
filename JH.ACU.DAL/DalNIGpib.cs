using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NationalInstruments.NI4882;


namespace JH.ACU.DAL
{
    public class DalNIGpib:Device
    {
        public DalNIGpib(int boardNumber, Address address) : base(boardNumber, address)
        {
        }

        public DalNIGpib(int boardNumber, byte primaryAddress) : base(boardNumber, primaryAddress)
        {
        }

        public DalNIGpib(int boardNumber, byte primaryAddress, byte secondaryAddress) : base(boardNumber, primaryAddress, secondaryAddress)
        {
        }

        public DalNIGpib(int boardNumber, byte primaryAddress, byte secondaryAddress, TimeoutValue timeoutValue) : base(boardNumber, primaryAddress, secondaryAddress, timeoutValue)
        {
        }
    }
}
