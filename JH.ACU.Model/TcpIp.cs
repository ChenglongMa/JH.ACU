using System.Xml.Serialization;

namespace JH.ACU.Model
{
    public class TcpIp
    {
        public TcpIp()
        {
            IpAddress = "127.0.0.1";
            Port = 502;
            Timeout = 2000;
        }

        /// <summary>
        /// IP地址
        /// </summary>
        [XmlAttribute("IPAddress")]
        public string IpAddress { get; set; }

        /// <summary>
        /// 端口号
        /// </summary>
        [XmlAttribute("Port")]
        public int Port { get; set; }

        /// <summary>
        /// 超时时间:ms
        /// </summary>
        [XmlAttribute("Timeout")]
        public int Timeout { get; set; }
    }
}