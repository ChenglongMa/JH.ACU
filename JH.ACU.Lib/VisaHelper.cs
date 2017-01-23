using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Ivi.Visa;
using JH.ACU.Model;
using NationalInstruments.Visa;

namespace JH.ACU.Lib
{
    /// <summary>
    /// 通讯帮助类
    /// </summary>
    public static class VisaHelper
    {
        /// <summary>
        /// 根据仪器类型查找本机资源
        /// </summary>
        /// <param name="instrType"></param>
        public static List<string> FindResources(InstrType instrType)
        {
            var list = new List<string>();
            try
            {
                using (var rm = new ResourceManager())
                {
                    string filter;
                    switch (instrType)
                    {
                        case InstrType.Gpib:
                            filter = "GPIB?*INSTR";
                            break;
                        case InstrType.Serial:
                            filter = "ASRL?*INSTR";
                            break;
                        case InstrType.Tcp:
                            filter = "TCPIP?*";
                            break;
                        default:
                            throw new ArgumentOutOfRangeException("instrType", instrType, null);
                    }
                    IEnumerable<string> resources = rm.Find(filter);
                    foreach (var s in resources)
                    {
                        var parseResult = rm.Parse(s);
                        //TODO：待验证GPIB地址
                        list.Add(parseResult.InterfaceNumber.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            return list.Count == 0 ? /*new List<string>{"1","2","3","4","5","6"}*/null : list;
        }
        /// <summary>
        /// 将端口号转换为相应的filter
        /// </summary>
        /// <param name="instr"></param>
        /// <returns></returns>
        public static string GetPortNumber(Instr instr)
        {
            switch (instr.Type)
            {
                case InstrType.Gpib:
                    return string.Format("GPIB0::{0}::INSTR", instr.PortNumber);
                case InstrType.Serial:
                    return string.Format("ASRL{0}::INSTR", instr.PortNumber);
                case InstrType.Tcp:
                    throw new NotImplementedException();
                    break;
                default:
                    throw new ArgumentNullException("Type", "端口类型设置无效");
            }
        }
    }
}
