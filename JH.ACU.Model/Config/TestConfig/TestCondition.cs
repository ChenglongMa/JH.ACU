using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace JH.ACU.Model.Config.TestConfig
{
    [XmlType("TestCondition")]
    public class TestCondition
    {
        public TestCondition()
        {
            Temperature = new Temperature();
            Voltage = new Voltage();
            AcuItems = new List<AcuItems>();
            TvItems = new Dictionary<TvType, double[]>();
        }

        [XmlElement]
        public Temperature Temperature { get; set; }

        [XmlElement]
        public Voltage Voltage { get; set; }

        [XmlElement]
        public CrashOutType CrashOutType { get; set; }

        private Dictionary<TvType, double[]> _tvItems;
        /// <summary>
        /// 温度、电压测试项 
        /// double[0]:TempValue;
        /// double[1]:VoltValue;
        /// double[2]:TempDelay(min);
        /// </summary>
        [XmlIgnore]
        public Dictionary<TvType, double[]> TvItems {
            get
            {
                if (_tvItems == null) return null;
                var newDic = _tvItems.OrderBy(t => t.Key).ToDictionary(t => t.Key, t => t.Value);
                return newDic;
            }set { _tvItems = value; } }

        [XmlArrayItem("AcuItem", typeof (AcuItems))]
        public List<AcuItems> AcuItems { get; set; }
    }

    /// <summary>
    /// 碰撞输出类型
    /// </summary>
    public enum CrashOutType
    {
        Advanced = 0,
        Conventional = 1,
    }

    /// <summary>
    /// 温度电压组合类型，其后数字为排序
    /// </summary>
    public enum TvType
    {
        LowTempLowVolt = 0,
        LowTempNorVolt = 1,
        LowTempHighVolt = 2,

        HighTempLowVolt = 3,
        HighTempNorVolt = 4,
        HighTempHighVolt = 5,

        NorTempLowVolt = 6,
        NorTempNorVolt = 7,
        NorTempHighVolt = 8,

    }
}







