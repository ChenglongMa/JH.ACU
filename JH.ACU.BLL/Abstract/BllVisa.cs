using System;
using System.Threading;
using Ivi.Visa;
using JH.ACU.BLL.Config;
using JH.ACU.Lib;
using JH.ACU.Model;
using JH.ACU.Model.Config.InstrumentConfig;
using NationalInstruments.Visa;

namespace JH.ACU.BLL.Abstract
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
                        BaudRate = Config.Serial.BaudRate,
                        Parity = Config.Serial.Parity,
                        DataBits = Config.Serial.DataBits,
                        StopBits = Config.Serial.StopBits,
                    };
                    break;
                case InstrType.Tcp:
                    MbSession = new TcpipSocket(VisaHelper.GetPortNumber(Config)) { TimeoutMilliseconds = Config.TcpIp.Timeout };
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected string WriteAndRead(string command, int delay = 0)//TODO:待测试delay时间
        {
            RawIo.Write(command + "\n");
            Thread.Sleep(delay);
            var res = RawIo.ReadString().Replace("\n", "");
            if (command != "SYSTem:ERRor?")
            {
                ThrowException();
            }
            return res;
        }

        protected void WriteNoRead(string command)
        {
            RawIo.Write(command + "\n");
        }

        protected void ThrowException()
        {
            var errorGroup = Error.Split(',');
            var code = Convert.ToInt32(errorGroup[0]);
            var message = errorGroup[1];
            if (code == 0) return;
            throw new Exception(message);
        }
        //private string RemoveEscapeCharacter(string oldStr)
        //{
        //    return oldStr.Replace("\n", "");
        //}
        #endregion

        #region 公有方法
        /// <summary>
        /// 仪器初始化
        /// </summary>
        /// <returns></returns>
        public abstract void Initialize();
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
                if (string.IsNullOrEmpty(Error))//BUG:Error不为空
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
