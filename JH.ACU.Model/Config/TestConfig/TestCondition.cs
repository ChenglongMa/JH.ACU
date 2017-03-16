﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace JH.ACU.Model.Config.TestConfig
{
    [XmlType("TestCondition")]
    public class TestCondition
    {
        public TestCondition()
        {
            Temperature = new Temperature();
            Voltage = new Voltage();
            AcuItems = new List<AcuItems>();
        }

        [XmlElement]
        public Temperature Temperature { get; set; }
        [XmlElement]
        public Voltage Voltage { get; set; }
        [XmlArrayItem("AcuItem",typeof(AcuItems))]
        public List<AcuItems> AcuItems { get; set; }
    }
}