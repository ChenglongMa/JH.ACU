using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using JH.ACU.Lib;

namespace JH.ACU.Model.Config.TestConfig
{
    /// <summary>
    /// 用于保存ACU索引、命名、测试项
    /// </summary>
    public class AcuItems
    {
        /// <summary>
        /// ACU索引，取值范围：0~7
        /// </summary>
        [XmlAttribute]
        public int Index { get; set; }

        ///// <summary>
        ///// ACU别名
        ///// </summary>
        //[XmlAttribute]
        //public string Name { get; set; }

        /// <summary>
        /// 测试项
        /// </summary>
        [XmlIgnore]
        public List<int> Items { get; set; }

        /// <summary>
        /// 将测试项转换为字符串保存到xml文件中
        /// </summary>
        [XmlElement("Items")]
        public string ItemsString
        {
            get { return string.Join(",", Items.ConvertAll(i => i.ToString()).ToArray()); }
            set
            {
                if (value.IsNullOrEmpty())
                {
                    Items = new List<int>();
                }
                else
                {
                    Items = (from s in value.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
                        let n = int.Parse(s)
                        select n).ToList();

                }
            }
        }

    }
}