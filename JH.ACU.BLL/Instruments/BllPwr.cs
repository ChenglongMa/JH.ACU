using System;
using JH.ACU.BLL.Abstract;
using JH.ACU.Model;

namespace JH.ACU.BLL.Instruments
{
    /// <summary>
    /// 程控电源操作类
    /// </summary>
    public class BllPwr :BllVisa//暂时先不调用IDalVisa
    {
        public BllPwr(InstrName instr=InstrName.PWR)
            : base(instr)
        {
        }
        #region 私有字段属性


        private const string Chanel = ":CHANnel1:"; //通道号:PSS & PSH can only be 1.
        private const string Output = ":OUTPut:"; 

        #endregion

        #region 公有属性



        /// <summary>
        /// 获取或设置输出电流
        /// </summary>
        public double OutputCurrent
        {
            get { return Convert.ToDouble(WriteAndRead(Chanel + "CURRent?")); }
            set { WriteNoRead(Chanel + "CURRent " + value); }
        }

        /// <summary>
        /// 获取或设置输出电压
        /// </summary>
        public double OutputVoltage
        {
            get { return Convert.ToDouble(WriteAndRead(Chanel + "VOLTage?")); }
            set { WriteNoRead(Chanel + "VOLTage " + value); }
        }

        /// <summary>
        /// 获取实际输出电流
        /// </summary>
        public double ActualCurrent
        {
            get { return Convert.ToDouble(WriteAndRead(Chanel + "MEASure:CURRent?")); }
        }

        /// <summary>
        /// 获取实际输出电压
        /// </summary>
        public double ActualVoltage
        {
            get { return Convert.ToDouble(WriteAndRead(Chanel + "MEASure:VOLTage?")); }
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
        public double Ovp
        {
            get { return Convert.ToDouble(WriteAndRead(Chanel + "PROTection:VOLTage?")); }
            set { WriteNoRead(Chanel + "PROTection:VOLTage " + value); }

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



        #endregion

        #region 私有方法


        #endregion

        #region 公有方法




        /// <summary>
        /// Clears over-voltage and over-current and
        /// over temperature protection error message.
        /// </summary>
        public void ClearProduction()
        {
            WriteNoRead(Output + "PROTection:CLEar");
        }



        #endregion

        public override void Initialize()
        {
            Reset();
            if (!SelfTest())
            {
                throw new Exception("程控电源自检失败");
            }
        }
    }
}
