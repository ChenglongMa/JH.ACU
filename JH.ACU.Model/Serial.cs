using System.Xml.Serialization;
using Ivi.Visa;

namespace JH.ACU.Model
{
    public class Serial
    {
        private int _port = 1;

        /// <summary>
        /// 端口号或设备地址
        /// </summary>
        [XmlAttribute("COM")]
        public int Port
        {
            get
            {
                return _port;
            }
            set { _port = value; }
        }

        /// <summary>
        /// 波特率
        /// </summary>
        [XmlAttribute("BaudRate")]
        public int BaudRate { get; set; }

        private SerialParity _parity = SerialParity.None;

        /// <summary>
        /// 校检位
        /// </summary>
        [XmlIgnore]
        public SerialParity Parity
        {
            get { return _parity; }
            set { _parity = value; }
        }

        private short _dataBits = 8;

        /// <summary>
        /// 数据位
        /// </summary>
        [XmlAttribute("DataBits")]
        public short DataBits
        {
            get { return _dataBits; }
            set { _dataBits = value; }
        }
        private SerialStopBitsMode _stopBits = SerialStopBitsMode.One;
        /// <summary>
        /// 停止位
        /// </summary>
        [XmlAttribute("StopBits")]
        public SerialStopBitsMode StopBits
        {
            get { return _stopBits; }
            set { _stopBits = value; }
        }
 
    }
}