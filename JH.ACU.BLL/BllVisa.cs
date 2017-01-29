using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Ivi.Visa;
using JH.ACU.Lib;
using JH.ACU.Model;
using NationalInstruments.Visa;

namespace JH.ACU.BLL
{
    /// <summary>
    /// 抽象类：用于各仪器通用方法及属性等
    /// </summary>
    public abstract class BllVisa
    {
        protected BllVisa(InstrName instr)
        {
            CreateSession(instr);
        }

        #region 属性字段

        protected MessageBasedSession MbSession { get; set; }

        protected IMessageBasedRawIO RawIo
        {
            get { return MbSession == null ? null : MbSession.RawIO; }
        }

        protected Instr Config { get; set; }

        /// <summary>
        /// Return the unique identification code of the instrument supply.
        /// </summary>
        public string Idn
        {
            get { return WriteAndRead("*IDN?"); }
        }

        /// <summary>
        /// 获取错误信息
        /// </summary>
        public string Error
        {
            get { return WriteAndRead("SYSTem:ERRor?"); }
        }

        #endregion

        #region 私有方法

        private void CreateSession(InstrName name)
        {
            Config = BllConfig.GetInstr(name);
            switch (Config.Type)
            {
                case InstrType.Gpib:
                    MbSession = new GpibSession(VisaHelper.GetPortNumber(Config));
                    break;
                case InstrType.Serial:
                    MbSession = new SerialSession(VisaHelper.GetPortNumber(Config))
                    {
                        BaudRate = Config.BaudRate,
                        Parity = Config.Parity,
                        DataBits = Config.DataBits,
                        StopBits = Config.StopBits,
                    };
                    break;
                case InstrType.Tcp:
                    throw new NotImplementedException();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

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

        public abstract bool Initialize();
        /// <summary>
        /// Set all control settings of instrument supply to their default values but does
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

        /// <summary>
        /// 暂停命令执行或查询，直到完成所有挂起操作
        /// </summary>
        public void Wait()
        {
            WriteNoRead("*WAI");
        }

        /// <summary>
        /// 等待操作完成
        /// QUES:使用方法待确定
        /// </summary>
        public void WaitComplete(int timeout = 30000)
        {
            var t = Environment.TickCount;
            do
            {
                if (string.IsNullOrEmpty(Error))
                {
                    Thread.Sleep(100);
                }
                else
                {
                    return;
                }
            } while (WriteAndRead("*OPC?") == "0" && Environment.TickCount - t < timeout);
        }

        #endregion
    }
}
