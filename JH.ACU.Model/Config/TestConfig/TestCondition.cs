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
            TvItems = new Dictionary<TvType, double[]>();
        }

        [XmlElement]
        public Temperature Temperature { get; set; }

        [XmlElement]
        public Voltage Voltage { get; set; }

        /// <summary>
        /// 温度、电压测试项
        /// </summary>
        [XmlIgnore]
        public Dictionary<TvType, double[]> TvItems { get; set; }

        [XmlArrayItem("AcuItem", typeof (AcuItems))]
        public List<AcuItems> AcuItems { get; set; }
    }

    public enum TvType
    {
        LowTempLowVolt,
        LowTempNorVolt,
        LowTempHighVolt,

        NorTempLowVolt,
        NorTempNorVolt,
        NorTempHighVolt,

        HighTempLowVolt,
        HighTempNorVolt,
        HighTempHighVolt,

    }
}