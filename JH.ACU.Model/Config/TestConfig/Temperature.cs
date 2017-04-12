using System.Xml.Serialization;

namespace JH.ACU.Model.Config.TestConfig
{
    public class Temperature
    {
        [XmlAttribute]
        public bool Enable { get; set; }

        /// <summary>
        /// 持续时间（单位：min）
        /// </summary>
        [XmlAttribute]
        public double Duration { get; set; }

        [XmlElement]
        public Temp LowTemp { get; set; }

        [XmlElement]
        public Temp NorTemp { get; set; }

        [XmlElement]
        public Temp HighTemp { get; set; }
    }

    public class Temp
    {
        /// <summary>
        /// 温度值
        /// </summary>
        [XmlElement]
        public double Value { get; set; }

        /// <summary>
        /// 测试延迟时间
        /// </summary>
        [XmlAttribute]
        public double Delay { get; set; }
    }
}