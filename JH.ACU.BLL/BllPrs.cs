using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JH.ACU.Model;

namespace JH.ACU.BLL
{
    /// <summary>
    /// 电阻箱操作类
    /// </summary>
    public class BllPrs:BllVisa
    {
        #region 构造函数

        public BllPrs(InstrName instr) : base(instr)
        {
        }

        #endregion

        #region 属性 字段

        private string Type { get; set; }
        private string Version { get; set; }
        private float Tolerance { get; set; }
        private int DecadesNum { get; set; }
        private double MinDec { get; set; }
        private double MaxDec { get; set; }
        private float SlotLsd { get; set; }
        private int Circuit{ get; set; }

        #endregion

        #region 私有方法

        private static float GetTolerance(string tolerance)
        {
            switch (tolerance.ToUpper())
            {
                case "X":
                    return 0.0001F;
                case "Q":
                    return 0.0002F;
                case "A":
                    return 0.0005F;
                case "B":
                    return 0.0010F;
                case "C":
                    return 0.0050F;
                case "F":
                    return 0.01F;
                case "G":
                    return 0.02F;
                case "H":
                    return 0.04F;
                default:
                    throw new ArgumentException("容差值无效", "tolerance");
            }
        }

        private static double GetLsd(string lsd)
        {
            //注意lsd大小写
            switch (lsd)
            {
                case "1m":
                    return 0.001;
                case "10m":
                    return 0.01;
                case "100m":
                    return 0.1;
                case "1":
                    return 1;
                case "10":
                    return 10;
                case "100":
                    return 100;
                case "1K":
                    return 1000;
                case "10K":
                    return 10*1000;
                case "100K":
                    return 100*1000;
                case "1M":
                    return 1000*1000;
                case "10M":
                    return 10*1000*1000;
                default:
                    throw new ArgumentException("分辨率无效", "lsd");
            }
        }
        #endregion

        #region 公有方法

        public override bool Initialize()
        {
            Reset();
            if (!SelfTest()) return false;
            if (string.IsNullOrEmpty(Idn))
            {
                return false;
            }
            var models = Idn.Split(',');
            if (models.Length<2)
            {
                return false;
            }
            var model = models[1].Split('-');
            if (model.Length<7)
            {
                return false;
            }
            Type = model[0];
            Version = model[1];
            Tolerance = GetTolerance(model[2]);
            DecadesNum = Convert.ToInt32(model[3]);
            MinDec = GetLsd(model[4]);
            MaxDec = MinDec*Math.Pow(10,DecadesNum )- MinDec;
            SlotLsd = Convert.ToSingle(model[5]);
            Circuit = Convert.ToInt32(model[6]);
            return true;
        }
        /// <summary>
        /// 设置输出电压
        /// </summary>
        /// <param name="resValue"></param>
        public void SetResistance(float resValue)
        {
            var value = Convert.ToInt32(resValue*(1/MinDec))*(1*Math.Pow(10, SlotLsd));
            string valStr = null;
            switch (Type)
            {
                default:
                case "200":
                case "201":
                    valStr = value.ToString("0000000000");
                    break;
                case "202":
                    valStr = value.ToString("000000000000");
                    break;
            }
            var command = "SOURce:DATA " + valStr;
            WriteNoRead(command);
        }
        #endregion

    }
}
