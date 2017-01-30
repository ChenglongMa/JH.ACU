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

        private InstrType _type;

        /// <summary>
        /// 连接类型
        /// </summary>
        [XmlAttribute("Type")]
        public InstrType Type { get
        {
            return _type;
        }
            set
            {
                _type = value;
                switch (value)
                {
                    case InstrType.Gpib:
                        Gpib = Gpib ?? new Gpib();
                        break;
                    case InstrType.Serial:
                        Serial = Serial ?? new Serial();
                        break;
                    case InstrType.Tcp:
                        TcpIp = TcpIp ?? new TcpIp();
                        break;
                    //default:
                    //    throw new ArgumentOutOfRangeException();
                }
            }
        }

        [XmlElement]
        public Gpib Gpib { get; set; }

        [XmlElement]
        public Serial Serial { get; set; }

        [XmlElement]
        public TcpIp TcpIp { get; set; }
    }
}
