using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JH.ACU.Lib;
using JH.ACU.Model;
using JH.ACU.Model.Config.InstrumentConfig;

namespace JH.ACU.BLL.Config
{
    /// <summary>
    /// 仪器配置类
    /// </summary>
    public static class BllConfig
    {
        private static readonly string SettingFileName = Environment.CurrentDirectory +
                                                         "\\Config\\InstrConfig.xml";

        //private static readonly List<Instr> InstrConfig = GetInstrConfigs();

        public static List<Instr> GetInstrConfigs()
        {
            return XmlHelper.XmlDeserializeFromFile<InstrConfig>(SettingFileName, Encoding.UTF8);
        }

        /// <summary>
        /// 保存配置
        /// </summary>
        /// <param name="instr"></param>
        /// <param name="instrs"></param>
        public static void Save(Instr instr,List<Instr> instrs=null)
        {
            var list = instrs ?? GetInstrConfigs();
            var i = list.Find(ins => ins.Name == instr.Name);
            if (i == null)
            {
                list.Add(instr);
            }
            else
            {
                list.Remove(i);
                list.Add(instr);
            }
            var resList=new InstrConfig();
            resList.AddRange(list.OrderBy(o => o.Name));
            XmlHelper.XmlSerializeToFile(resList, SettingFileName, Encoding.UTF8);
        }

        /// <summary>
        /// 获取配置
        /// </summary>
        /// <param name="name"></param>
        /// <param name="instrs"></param>
        /// <returns></returns>
        public static Instr GetInstr(InstrName name,List<Instr> instrs=null )
        {
            var list = instrs ?? GetInstrConfigs();
            var instr = list.Find(i => i.Name == name);
            return instr ?? new Instr { Name = name };
        }
    }
}
