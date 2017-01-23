using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        public static void WriteLogInfo(Exception ex, string strMessType, string p_2, string p_3, string p_4)
        {
            var path = _rootPath;
            var strFile = "\\" + DateTime.Now.ToString("yyyyMMdd") + ".log";
            if (!CreateLog_Directory(path, strFile)) return;
            using (var sw = new StreamWriter(path + strFile, true, Encoding.Default))
            {
                sw.WriteLine("*****************************************【"
                             + strMessType + DateTime.Now
                             + "】*****************************************");
                if (ex != null)
                {
                    sw.WriteLine("【错误代码类】" + p_2);
                    sw.WriteLine("【错误方法】" + p_3);
                    sw.WriteLine("【执行的代码】" + p_4);
                    sw.WriteLine("【ErrorType】" + ex.GetType());
                    sw.WriteLine("【TargetSite】" + ex.TargetSite);
                    sw.WriteLine("【Message】" + ex.Message);
                    sw.WriteLine("【Source】" + ex.Source);
                    sw.WriteLine("【StackTrace】" + ex.StackTrace);
                    
                }
                else
                {
                    sw.WriteLine("【错误代码类】" + p_2);
                    sw.WriteLine("【错误方法】" + p_3);
                    sw.WriteLine("【执行的代码】" + p_4);
                    sw.WriteLine("Exception is NULL");
                }
                sw.WriteLine();
                sw.Close();
                sw.Dispose();
            }
        }

    }
}
