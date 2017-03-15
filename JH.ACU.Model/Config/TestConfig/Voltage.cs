using System.Xml.Serialization;

namespace JH.ACU.Model.Config.TestConfig
{
    public class Voltage
    {
        [XmlElement]
        public double LowVolt { get; set; }
        [XmlElement]
        public double NorVolt { get; set; }
        [XmlElement]
        public double HighVolt { get; set; }
    }
}