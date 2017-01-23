using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Ivi.Visa;

namespace JH.ACU.Model
{
    /// <summary>
    /// 仪器配置信息
    /// </summary>
    [XmlType("InstrConfig")]
    public class InstrConfig : List<Instr>
    {
    }

    public class Instr
    {
        /// <summary>
        /// 仪器名称
        /// </summary>
        [XmlAttribute("Name")]
        public InstrName Name { get; set; }

        /// <summary>
        /// 连接类型
        /// </summary>
        [XmlAttribute("Type")]
        public InstrType Type { get; set; }

        private int _portNumber = 1;

        /// <summary>
        /// 端口号或设备地址
        /// </summary>
        [XmlAttribute("PortNumber")]
        public int PortNumber
        {
            get
            {
                return _portNumber;
            }
            set { _portNumber = value; }
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
        private SerialStopBitsMode _stopBits=SerialStopBitsMode.One;
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
