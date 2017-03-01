using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace JH.ACU.Lib
{
    /// <summary>
    /// 日志帮助类
    /// </summary>
    public static class LogHelper
    {
        private static string _rootPath = AppDomain.CurrentDomain.BaseDirectory + "Log";
        /// <summary>
        /// 创建日志文件夹
        /// </summary>
        /// <returns></returns>
        private static bool CreateLog_Directory(string path, string strfile)
        {
            try
            {
                if (!Directory.Exists(path))
                {
                    //创建日志文件夹
                    Directory.CreateDirectory(path);
                }
                if (!File.Exists(path + strfile))
                {
                    //创建日志文件夹
                    File.Create(path + strfile).Dispose();
                }
            }
            catch
            {
                return false;
            }

            return true;
        }
        public static void WriteErrorLog(Exception ex,string description=null)
        {
            var path = _rootPath;
            var strFile = "\\ErrorLog" + DateTime.Now.ToString("yyyyMMdd") + ".log";
            if (!CreateLog_Directory(path, strFile)) return;
            using (var sw = new StreamWriter(path + strFile, true, Encoding.Default))
            {
                sw.WriteLine("*****************************************【"
                             + DateTime.Now
                             + "】*****************************************");
                var functionName = MethodBase.GetCurrentMethod().Name;
                if (ex != null)
                {
                    sw.WriteLine("【FunctionName】" + functionName);
                    sw.WriteLine("【ErrorType】" + ex.GetType());
                    sw.WriteLine("【TargetSite】" + ex.TargetSite);
                    sw.WriteLine("【Message】" + ex.Message);
                    sw.WriteLine("【Source】" + ex.Source);
                    sw.WriteLine("【StackTrace】" + ex.StackTrace);
                }
                else
                {
                    sw.WriteLine("Exception is NULL");
                }
                sw.WriteLine();
            }
        }       
        public static void WriteWarningLog(string message)
        {
            var path = _rootPath;
            var strFile = "\\WarningLog" + DateTime.Now.ToString("yyyyMMdd") + ".log";
            if (!CreateLog_Directory(path, strFile)) return;
            using (var sw = new StreamWriter(path + strFile, true, Encoding.Default))
            {
                sw.WriteLine("*****************************************【"
                             + DateTime.Now
                             + "】*****************************************");
                var functionName = MethodBase.GetCurrentMethod().Name;
                sw.WriteLine("【FunctionName】" + functionName);
                sw.WriteLine("【WraningMessage】" + message);
                sw.WriteLine();
            }
        }

    }
}
