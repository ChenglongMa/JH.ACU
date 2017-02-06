using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Ivi.Visa;
using JH.ACU.Model;
using JH.ACU.Model.Config.InstrumentConfig;
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
                            filter = "TCPIP?*SOCKET";
                            break;
                        default:
                            throw new ArgumentOutOfRangeException("instrType", instrType, null);
                    }
                    var resources = rm.Find(filter);
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
            return list.Count == 0 ? null : list;
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
                    return string.Format("GPIB0::{0}::INSTR", instr.Gpib.Port);
                case InstrType.Serial:
                    return string.Format("ASRL{0}::INSTR", instr.Serial.Port);
                case InstrType.Tcp:
                    return string.Format("TCPIP0::{0}::{1}::SOCKET", instr.TcpIp.IpAddress, instr.TcpIp.Port);
                    break;
                default:
                    throw new ArgumentNullException("instr", "端口类型设置无效");
            }
        }
    }
}
