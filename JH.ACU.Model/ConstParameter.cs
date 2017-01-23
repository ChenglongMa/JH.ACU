using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JH.ACU.Model
{
    /// <summary>
    /// 常量参数
    /// </summary>
    public struct ConstParameter
    {
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
                    {"ACU", InstrName.ACU},
                    {"数字万用表", InstrName.DMM},
                    {"电阻箱#1", InstrName.PRS0},
                    {"电阻箱#2", InstrName.PRS1},
                    {"程控电源", InstrName.PWR},
                    {"温箱", InstrName.Chamber},
                };
            }
        }
    }

    /// <summary>
    /// 仪器名称枚举
    /// </summary>
    public enum InstrName
    {
        //None,
        ACU,

        /// <summary>
        /// 程控电源
        /// </summary>
        PWR,

        /// <summary>
        /// 程控电阻箱0
        /// </summary>
        PRS0,

        /// <summary>
        /// 程控电阻箱1
        /// </summary>
        PRS1,

        /// <summary>
        /// 数字万用表
        /// </summary>
        DMM,

        /// <summary>
        /// 温箱
        /// </summary>
        Chamber,
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

}
