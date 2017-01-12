using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JH.ACU.DAL;
using JH.ACU.Model;
using NationalInstruments.Visa;

namespace JH.ACU.BLL
{
    public class BllPwr
    {
        public BllPwr()
        {
            switch (_config.Type)
            {
                case "GPIB":
                    _mbSession=new GpibSession(_config.PortNumber);
                    break;
                case "Serial":
                    break;
                    
            }
        }

        #region 字段属性

        private readonly MessageBasedSession _mbSession;

        private Instr _config
        {
            get { return DalConfig.GetInstrConfig("Power"); }
        }

        #endregion

        #region 私有方法

        #endregion

        #region 公有方法

        #endregion

    }
}
