using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace JH.ACU.Model.Config.TestConfig
{
    public class AcuItems
    {
        [XmlAttribute]
        public string Id { get; set; }

        [XmlIgnore]
        public List<int> Items
        {
            get
            {
                return (from s in ItemsString.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
                    let n = int.Parse(s)
                    select n).ToList();
            }
            set { ItemsString = string.Join(",", value.ConvertAll(i => i.ToString()).ToArray()); }
        }

        [XmlAttribute("Items")]
        public string ItemsString { get; set; }

    }
}