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
    /// <summary>
    /// 抽象类：用于各仪器通用方法及属性等
    /// </summary>
    public abstract class BllVisa
    {
        protected BllVisa()
        {
            switch (Config.Type)
            {
                case "GPIB":
                    MbSession = new GpibSession(Config.PortNumber);
                    break;
                case "Serial":
                    MbSession = new SerialSession(Config.PortNumber)
                    {
                        BaudRate = Config.BaudRate,
                        Parity = Config.Parity,
                        DataBits = Config.DataBits
                    };
                    break;

            }
        }

        #region 属性字段

        private MessageBasedSession MbSession { get; set; }

        private IMessageBasedRawIO RawIo
        {
            get { return MbSession == null ? null : MbSession.RawIO; }
        }

        protected abstract Instr Config { get; }

        #endregion

        #region 私有方法

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

        #endregion

        #region 公有方法

        /// <summary>
        /// Return the unique identification code of the power supply.
        /// </summary>
        public string Idn
        {
            get { return WriteAndRead("*IDN?"); }
        }

        /// <summary>
        /// Set all control settings of power supply to their default values but does
        /// not purge stored setting. 
        /// </summary>
        public void Reset()
        {
            WriteNoRead("*RST");
        }
        /// <summary>
        /// 清除所有的事件寄存器
        /// </summary>
        public void Clear()
        {
            WriteNoRead("*CLS");
        }

        /// <summary>
        /// Self-test and test the RAM, ROM
        /// </summary>
        /// <returns></returns>
        public bool SelfTest()
        {
            var res = WriteAndRead("*TST?");
            return res == "0";
        }
        #endregion

    }
}
