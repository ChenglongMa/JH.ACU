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
            ResultValueList = new List<object>(8);
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

        /// <summary>
        /// ACU结果集合
        /// </summary>
        [XmlIgnore]
        public List<object> ResultValueList { get; private set; }

        #region 各ACU结果

        /// <summary>
        /// ACU#1结果
        /// </summary>
        [XmlIgnore]
        public object AcuResult1
        {
            get { return GetResult(1); }
        }

        /// <summary>
        /// ACU#1结果
        /// </summary>
        [XmlIgnore]
        public object AcuResult2
        {
            get { return GetResult(2); }
        }

        /// <summary>
        /// ACU#1结果
        /// </summary>
        [XmlIgnore]
        public object AcuResult3
        {
            get { return GetResult(3); }
        }

        /// <summary>
        /// ACU#1结果
        /// </summary>
        [XmlIgnore]
        public object AcuResult4
        {
            get { return GetResult(4); }
        }

        /// <summary>
        /// ACU#1结果
        /// </summary>
        [XmlIgnore]
        public object AcuResult5
        {
            get { return GetResult(5); }
        }

        /// <summary>
        /// ACU#1结果
        /// </summary>
        [XmlIgnore]
        public object AcuResult6
        {
            get { return GetResult(6); }
        }

        /// <summary>
        /// ACU#1结果
        /// </summary>
        [XmlIgnore]
        public object AcuResult7
        {
            get { return GetResult(7); }
        }

        /// <summary>
        /// ACU#1结果
        /// </summary>
        [XmlIgnore]
        public object AcuResult8
        {
            get { return GetResult(8); }
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

        #region 私有方法

        /// <summary>
        /// 获取ACU结果
        /// </summary>
        /// <param name="acuIndex">从1开始</param>
        /// <returns></returns>
        private object GetResult(int acuIndex)
        {
            if (ResultValueList.IsNullOrEmpty() || ResultValueList.Count < acuIndex)
            {
                return null;
            }
            return ResultValueList[acuIndex - 1];
        }

        #endregion

    }
}