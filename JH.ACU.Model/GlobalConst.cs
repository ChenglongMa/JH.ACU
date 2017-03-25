using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JH.ACU.Model
{
    /// <summary>
    /// 常量参数
    /// </summary>
    public struct GlobalConst
    {
        static GlobalConst()
        {
            Precision = 0.01;
        }

        /// <summary>
        /// 获取波特率集合
        /// </summary>
        public static int[] BaudRate
        {
            get { return new[] {300, 600, 1200, 9600, 10400}; }
        }

        /// <summary>
        /// 获取数据位集合
        /// </summary>
        public static short[] DataBits
        {
            get { return new short[] {7, 8}; }
        }

        /// <summary>
        /// 获取仪器字典
        /// </summary>
        public static Dictionary<string, InstrName> InstrNameString
        {
            get
            {
                return new Dictionary<string, InstrName>
                {
                    //{"----请选择----", (InstrName)0},
                    {"ACU", InstrName.Acu},
                    {"数字万用表", InstrName.Dmm},
                    {"电阻箱#1", InstrName.Prs0},
                    {"电阻箱#2", InstrName.Prs1},
                    {"程控电源", InstrName.Pwr},
                    {"温箱", InstrName.Chamber},
                };
            }
        }
        
        public static double[,] TempVoltCondition = new double[3, 3];

        #region FC测试
        /// <summary>
        /// 测试精度
        /// </summary>
        public static double Precision { get; set; }

        #endregion

    }

    /// <summary>
    /// 仪器名称枚举
    /// </summary>
    public enum InstrName
    {
        //None,
        Acu,

        /// <summary>
        /// 程控电源
        /// </summary>
        Pwr,

        /// <summary>
        /// 程控电阻箱0
        /// </summary>
        Prs0,

        /// <summary>
        /// 程控电阻箱1
        /// </summary>
        Prs1,

        /// <summary>
        /// 数字万用表
        /// </summary>
        Dmm,

        /// <summary>
        /// 温箱
        /// </summary>
        Chamber,

        /// <summary>
        /// 数据采集卡
        /// </summary>
        Daq,
    }

    /// <summary>
    /// 仪器类型枚举
    /// </summary>
    public enum InstrType
    {
        Gpib,
        Serial,
        Tcp,
    }

    /// <summary>
    /// ACU开关枚举
    /// </summary>
    public enum BeltSwitch
    {
        Dsb,
        Psb,
        Pads
    }

    ///// <summary>
    ///// 温度电压模式
    ///// </summary>
    //[Flags]
    //public enum TempVoltMode
    //{
    //    LowTempLowVolt = 0x01,
    //    LowTempNorVolt = 0x02,
    //    LowTempHighVolt = 0x04,
    //    NorTempLowVolt = 0x08,
    //    NorTempNorVolt = 0x10,
    //    NorTempHighVolt = 0x20,
    //    HighTempLowVolt = 0x40,
    //    HighTempNorVolt = 0x80,
    //    HighTempHighVolt = 0x100
    //}
    /// <summary>
    /// 二分法查找结果枚举
    /// </summary>
    public enum FindResult
    {
        Error,
        InBetween,
        UnderMin,
        AboveMax,
    }
}