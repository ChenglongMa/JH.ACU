using System;
using System.Collections.Generic;
using System.Text;
using JH.ACU.Model;
using JH.ACU.Lib;

namespace JH.ACU.BLL
{
    public static class BllConfig
    {
        private static readonly string SettingFileName = Environment.CurrentDirectory +
                                                         "\\InstrumentConfig\\InstrConfig.xml";

        private static readonly List<Instr> InstrConfig = GetInstrConfigs();

        private static List<Instr> GetInstrConfigs()
        {
            return XmlHelper.XmlDeserializeFromFile<InstrConfig>(SettingFileName, Encoding.UTF8);
        }

        /// <summary>
        /// 保存配置
        /// </summary>
        /// <param name="instr"></param>
        public static void Save(Instr instr)
        {
            var i = InstrConfig.Find(ins => ins.Name == instr.Name);
            if (i == null)
            {
                InstrConfig.Add(instr);
            }
            else
            {
                InstrConfig.Remove(i);
                InstrConfig.Add(instr);
            }
            XmlHelper.XmlSerializeToFile(InstrConfig, SettingFileName, Encoding.UTF8);
        }

        /// <summary>
        /// 获取配置
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Instr GetInstr(InstrName name)
        {
            return InstrConfig.Find(i => i.Name == name);
        }
    }
}
