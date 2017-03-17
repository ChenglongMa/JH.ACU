using System.Xml.Serialization;

namespace JH.ACU.Model.Config.TestConfig
{
    public class Temperature
    {
        [XmlAttribute]
        public bool Enable { get; set; }
        [XmlAttribute]
        public double Duration { get; set; }
        [XmlElement]
        public double LowTemp { get; set; }
        [XmlElement]
        public double NorTemp { get; set; }
        [XmlElement]
        public double HighTemp { get; set; }
    }
}