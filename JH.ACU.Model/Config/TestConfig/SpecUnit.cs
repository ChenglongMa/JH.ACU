using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JH.ACU.Model.Config.TestConfig
{
    /// <summary>
    /// 测试步骤名、测试规范、单位和对应故障码
    /// </summary>
    public class SpecUnit
    {
        #region 构造函数

        public SpecUnit()
        {

        }

        #endregion

        #region 属性字段

        /// <summary>
        /// 测试步骤
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// 步骤描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 规范
        /// </summary>
        public string Specification { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        public string Uom { get; set; }

        /// <summary>
        /// 故障码
        /// </summary>
        public byte Dtc { get; set; }

        /// <summary>
        /// 测试结果
        /// </summary>
        public object Result { get; set; }

        #endregion

    }
}