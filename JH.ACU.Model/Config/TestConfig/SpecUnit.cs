using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using JH.ACU.Lib;

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
        public SpecItem()
        {
        }

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


        #region 各ACU结果

        /// <summary>
        /// ACU#1结果
        /// </summary>
        [XmlIgnore]
        public object AcuResult1 { get; set; }

        /// <summary>
        /// ACU#1结果
        /// </summary>
        [XmlIgnore]
        public object AcuResult2 { get; set; }

        /// <summary>
        /// ACU#1结果
        /// </summary>
        [XmlIgnore]
        public object AcuResult3 { get; set; }

        /// <summary>
        /// ACU#1结果
        /// </summary>
        [XmlIgnore]
        public object AcuResult4 { get; set; }

        /// <summary>
        /// ACU#1结果
        /// </summary>
        [XmlIgnore]
        public object AcuResult5 { get; set; }

        /// <summary>
        /// ACU#1结果
        /// </summary>
        [XmlIgnore]
        public object AcuResult6 { get; set; }

        /// <summary>
        /// ACU#1结果
        /// </summary>
        [XmlIgnore]
        public object AcuResult7 { get; set; }

        /// <summary>
        /// ACU#1结果
        /// </summary>
        [XmlIgnore]
        public object AcuResult8 { get; set; }

        /// <summary>
        /// 测试最大值
        /// </summary>
        [XmlIgnore]
        public double MaxValue
        {
            get { return AcuResultList.IsNullOrEmpty() ? double.NaN : AcuResultList.Max(); }
        }

        /// <summary>
        /// 测试平均值
        /// </summary>
        public double AverValue
        {
            get { return AcuResultList.IsNullOrEmpty() ? double.NaN : AcuResultList.Average(); }
        }

        /// <summary>
        /// 测试最小值
        /// </summary>
        public double MinValue
        {
            get { return AcuResultList.IsNullOrEmpty() ? double.NaN : AcuResultList.Min(); }
        }

        #endregion

        ///// <summary>
        ///// 测试结果值
        ///// </summary>
        //[XmlIgnore]
        //[Obsolete]
        //public object ResultValue { get; set; } 

        /// <summary>
        /// 测试结果信息
        /// </summary>
        [XmlIgnore]
        public object ResultInfo { get; set; }

        #endregion

        [XmlIgnore]
        private IEnumerable<double> AcuResultList
        {
            get
            {
                var list = new List<double>();
                double value;
                if (double.TryParse(AcuResult1 as string, out value))
                {
                    list.Add(value);
                }
                if (double.TryParse(AcuResult2 as string, out value))
                {
                    list.Add(value);
                }
                if (double.TryParse(AcuResult3 as string, out value))
                {
                    list.Add(value);
                }
                if (double.TryParse(AcuResult4 as string, out value))
                {
                    list.Add(value);
                }
                if (double.TryParse(AcuResult5 as string, out value))
                {
                    list.Add(value);
                }
                if (double.TryParse(AcuResult6 as string, out value))
                {
                    list.Add(value);
                }
                if (double.TryParse(AcuResult7 as string, out value))
                {
                    list.Add(value);
                }
                if (double.TryParse(AcuResult8 as string, out value))
                {
                    list.Add(value);
                }
                return list;
            }
        }
    }
}