using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Ivi.Visa;
using JH.ACU.DAL;
using JH.ACU.Model;
using NationalInstruments.Visa;

namespace JH.ACU.BLL
{
    public class BllPwr //暂时先不调用IDalVisa
    {
        public BllPwr()
        {
            switch (Config.Type)
            {
                case "GPIB":
                    _mbSession = new GpibSession(Config.PortNumber);
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

        private const string Chanel = ":CHANnel1:"; //通道号:PSS & PSH can only be 1.
        private const string Output = ":OUTPut:"; //QUES:有没有冒号不确定

        #endregion

        #region 公有属性

        /// <summary>
        /// Return the unique identification code of the power supply.
        /// </summary>
        public string Idn
        {
            get { return WriteAndRead("*IDN?"); }
        }

        /// <summary>
        /// 获取或设置输出电流
        /// </summary>
        public decimal OutputCurrent
        {
            get { return Convert.ToDecimal(WriteAndRead(Chanel + "CURRent?")); }
            set { WriteAndRead(Chanel + "CURRent " + value); }
        }

        /// <summary>
        /// 获取或设置输出电压
        /// </summary>
        public decimal OutputVoltage
        {
            get { return Convert.ToDecimal(WriteAndRead(Chanel + "VOLTage?")); }
            set { WriteAndRead(Chanel + "VOLTage " + value); }
        }

        /// <summary>
        /// 获取实际输出电流
        /// </summary>
        public decimal ActualOutputCurrent
        {
            get { return Convert.ToDecimal(WriteAndRead(Chanel + "MEASure:CURRent?")); }
        }

        /// <summary>
        /// 获取实际输出电压
        /// </summary>
        public decimal ActualOutputVoltage
        {
            get { return Convert.ToDecimal(WriteAndRead(Chanel + "MEASure:VOLTage?")); }
        }

        /// <summary>
        /// 获取或设置过流保护
        /// </summary>
        public bool Ocp
        {
            get
            {
                var res = WriteAndRead(Chanel + "PROTection:CURRent?");
                return res == "1";
            }
            set
            {
                var v = value ? "1" : "0";
                WriteNoRead(Chanel + "PROTection:CURRent " + v);
            }
        }

        /// <summary>
        /// 获取或设置过电压保护数值
        /// </summary>
        public decimal Ovp
        {
            get { return Convert.ToDecimal(WriteAndRead(Chanel + "PROTection:VOLTage?")); }
            set { WriteAndRead(Chanel + "PROTection:VOLTage " + value); }

        }

        /// <summary>
        /// 获取或设置输出状态（是否修改为方法待定）
        /// </summary>
        public bool OutPutState
        {
            get
            {
                var res = WriteAndRead(Output + "STATe?");
                return res == "1";
            }
            set
            {
                var v = value ? "1" : "0";
                WriteNoRead(Output + "STATe " + v);
            }
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

        private string WriteAndRead(string command, int delay = 50)
        {
            _rawIo.Write(command + "\n");
            Thread.Sleep(delay);
            return _rawIo.ReadString();
        }

        private void WriteNoRead(string command)
        {
            _rawIo.Write(command + "\n");
        }

        #endregion

        #region 公有方法

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
        public void Suspend()
        {
            WriteNoRead("*WAI");
        }

        /// <summary>
        /// Clears over-voltage and over-current and
        /// over temperature protection error message.
        /// </summary>
        public void ClearProduction()
        {
            WriteNoRead(Output + "PROTection:CLEar");
        }

        /// <summary>
        /// Set all control settings of power supply to their default values but does
        /// not purge stored setting. 
        /// </summary>
        public void Reset()
        {
            WriteNoRead("*RST");
        }

        #endregion

    }
}
