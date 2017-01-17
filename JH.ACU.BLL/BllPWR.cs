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
    public class BllPwr :BllVisa//暂时先不调用IDalVisa
    {
        #region 私有字段属性

        protected sealed override Instr Config
        {
            get { return BllConfig.GetInstrConfig(InstrName.PWR); }
        }

        private const string Chanel = ":CHANnel1:"; //通道号:PSS & PSH can only be 1.
        private const string Output = ":OUTPut:"; //QUES:有没有冒号不确定

        #endregion

        #region 公有属性



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
        public decimal ActualCurrent
        {
            get { return Convert.ToDecimal(WriteAndRead(Chanel + "MEASure:CURRent?")); }
        }

        /// <summary>
        /// 获取实际输出电压
        /// </summary>
        public decimal ActualVoltage
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



        #endregion

        #region 私有方法


        #endregion

        #region 公有方法



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



        #endregion

    }
}
