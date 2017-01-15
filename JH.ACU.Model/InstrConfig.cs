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

        private string _portNumber = "";

        /// <summary>
        /// 端口号或设备地址
        /// </summary>
        [XmlAttribute("PortNumber")]
        public string PortNumber
        {
            get
            {
                switch (Type.ToUpper())
                {
                    case "GPIB":
                        return String.Format("GPIB0::{0}::INSTR", _portNumber);
                    case "SERIAL":
                        return String.Format("ASRL{0}::INSTR", _portNumber);
                    default:
                        throw new ArgumentNullException("Type", "端口类型设置无效");
                }
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

    }
    /// <summary>
    /// 仪器名称枚举
    /// </summary>
    public enum InstrName
    {

        ACU,
        /// <summary>
        /// 程控电源
        /// </summary>
        PWR,
        /// <summary>
        /// 程控电阻箱0
        /// </summary>
        PRS0,
        /// <summary>
        /// 程控电阻箱1
        /// </summary>
        PRS1,
        /// <summary>
        /// 数字万用表
        /// </summary>
        DMM
    }

}
