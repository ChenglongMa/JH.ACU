using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JH.ACU.DAL;
using JH.ACU.Model;

namespace JH.ACU.BLL
{
    public class BllDmm:BllVisa
    {
        protected sealed override Instr Config
        {
            get { return BllConfig.GetInstrConfig(InstrName.DMM); }
        }
    }
}
