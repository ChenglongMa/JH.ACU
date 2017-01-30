using System.Xml.Serialization;

namespace JH.ACU.Model
{
    public class Gpib 
    {

        private int _port = 1;

        /// <summary>
        /// 端口号或设备地址
        /// </summary>
        [XmlAttribute("Address")]
        public int Port
        {
            get
            {
                return _port;
            }
            set { _port = value; }
        }
 
    }
}