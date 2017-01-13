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
        public string Name { get; set; }

        /// <summary>
        /// 连接类型
        /// </summary>
        [XmlAttribute("Type")]
        public string Type { get; set; }

        /// <summary>
        /// 端口号或设备地址
        /// </summary>
        [XmlAttribute("PortNumber")]
        public string PortNumber { get; set; }

        /// <summary>
        /// 波特率
        /// </summary>
        [XmlAttribute("BaudRate")]
        public int BaudRate { get; set; }

        /// <summary>
        /// 校检位
        /// </summary>
        [XmlAttribute("Parity")]
        public SerialParity Parity { get; set; }
    }
}
