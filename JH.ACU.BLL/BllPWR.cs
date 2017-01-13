using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivi.Visa;
using JH.ACU.DAL;
using JH.ACU.Model;
using NationalInstruments.Visa;

namespace JH.ACU.BLL
{
    public class BllPwr//暂时先不调用IDalVisa
    {
        public BllPwr()
        {
            switch (Config.Type)
            {
                case "GPIB":
                    _mbSession=new GpibSession(Config.PortNumber);
                    break;
                case "Serial":
                    _mbSession = new SerialSession(Config.PortNumber)
                    {
                        BaudRate = Config.BaudRate,
                        Parity = Config.Parity,
                        DataBits = Config.DataBits
                    };
                    break;
                    
            }
            _rawIo = _mbSession.RawIO;
        }

        #region 私有字段属性

        private readonly MessageBasedSession _mbSession;
        private readonly IMessageBasedRawIO _rawIo;
        private static Instr Config
        {
            get { return DalConfig.GetInstrConfig("Power"); }
        }

        #endregion

        #region 公有属性

        #endregion

        #region 私有方法

        private void Write(string command)
        {
            _rawIo.Write(command+"\n");
        }

        private string Read()
        {
            return _rawIo.ReadString();
        }
        #endregion

        #region 公有方法

        #endregion

    }
}
