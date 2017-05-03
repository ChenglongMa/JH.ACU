using System;
using JH.ACU.BLL.Abstract;
using JH.ACU.Lib;
using JH.ACU.Model;

namespace JH.ACU.BLL.Instruments
{
    /// <summary>
    /// 电阻箱操作类
    /// </summary>
    public class BllPrs : BllVisa
    {
        #region 构造函数

        public BllPrs(InstrName instr) : base(instr)
        {
        }

        #endregion

        #region 属性 字段

        /// <summary>
        /// 类型
        /// </summary>
        private string Type { get; set; }

        /// <summary>
        /// 版本号
        /// </summary>
        private string Version { get; set; }

        /// <summary>
        /// 容差值
        /// </summary>
        private float Tolerance { get; set; }

        /// <summary>
        /// 以十为组的组的数量
        /// </summary>
        private int DecadesNum { get; set; }

        /// <summary>
        /// 最小分辨率
        /// </summary>
        private double MinDec { get; set; }

        /// <summary>
        /// 最大分辨率
        /// </summary>
        private double MaxDec { get; set; }

        private float SlotLsd { get; set; }

        /// <summary>
        /// 回路配置（开路、短路）
        /// </summary>
        private int Circuit { get; set; }

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

        public override void Initialize()
        {
            Reset();
            if (!SelfTest()) throw new Exception("电阻箱自检失败");
            if (Idn.IsNullOrEmpty())
            {
                throw new NullReferenceException("电阻箱型号读取异常");
            }
            var models = Idn.Split(',');
            if (models.Length < 2)
            {
                throw new NullReferenceException("电阻箱型号读取异常");
            }
            var model = models[1].Split('-');
            if (model.Length < 7)
            {
                throw new NullReferenceException("电阻箱型号读取异常");
            }
            Type = model[0];
            Version = model[1];
            Tolerance = GetTolerance(model[2]);
            DecadesNum = Convert.ToInt32(model[3]);
            MinDec = GetLsd(model[4]);
            MaxDec = MinDec*Math.Pow(10, DecadesNum) - MinDec;
            SlotLsd = Convert.ToSingle(model[5]);
            Circuit = Convert.ToInt32(model[6]);
        }

        /// <summary>
        /// 设置输出电阻
        /// </summary>
        /// <param name="resValue">单位：欧姆</param>
        public void SetResistance(double resValue)
        {
            var value = Convert.ToInt32(resValue*(1/MinDec))*(1*Math.Pow(10, SlotLsd));
            string valStr;
            switch (Version)
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
            Write(command);
        }

        #endregion

    }
}
