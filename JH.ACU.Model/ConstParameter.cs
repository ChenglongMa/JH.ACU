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
        /// 获取仪器字典
        /// </summary>
        public static Dictionary<string, InstrName> InstrNameString
        {
            get
            {
                return new Dictionary<string, InstrName>
                {
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
}
