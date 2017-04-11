using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace JH.ACU.Model.Config.TestConfig
{
    [XmlType("SpecUint")]
    public class SpecUnit : List<SpecItem>
    {

    }

    /// <summary>
    /// 测试步骤名、测试规范、单位和对应故障码
    /// </summary>
    [Serializable]
    public class SpecItem
    {

        #region 属性字段

        /// <summary>
        /// 测试步骤
        /// </summary>
        [XmlAttribute]
        public int Index { get; set; }

        /// <summary>
        /// 步骤描述
        /// </summary>
        [XmlAttribute]
        public string Description { get; set; }

        /// <summary>
        /// 规范
        /// </summary>
        [XmlAttribute]
        public string Specification { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        [XmlAttribute("UOM")]
        public string Uom { get; set; }

        /// <summary>
        /// 故障码
        /// </summary>
        [XmlIgnore]
        public byte Dtc
        {
            get { return Convert.ToByte(DtcString, 16); }
            set { DtcString = "0x" + value.ToString("X2"); }
        }

        [XmlAttribute("DTC")]
        public string DtcString { get; set; }

        /// <summary>
        /// 测试结果值
        /// </summary>
        [XmlIgnore]
        public object ResultValue { get; set; }

        /// <summary>
        /// 测试结果信息
        /// </summary>
        [XmlIgnore]
        public object ResultInfo{ get; set; }

        #endregion

    }
}