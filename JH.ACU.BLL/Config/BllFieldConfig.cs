/* ==============================================================================
 * 功能描述：Grid格式类Field读取  
 * 创 建 者：马成龙
 * 创建日期：2017/4/11 16:12:24
 * ==============================================================================*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using JH.ACU.Lib;
using JH.ACU.Lib.GridConfig;


namespace JH.ACU.BLL.Config
{
    /// <summary>
    /// BllFieldConfig
    /// </summary>
    public static class BllFieldConfig
    {
        #region 构造函数

        static BllFieldConfig()
        {
        }

        #endregion

        #region 属性字段

        private static readonly string FileName = Environment.CurrentDirectory + "\\GridFields";

        #endregion

        #region 私有方法

        #endregion

        #region 公有方法

        /// <summary>
        /// 加载字段信息
        /// </summary>
        /// <param name="configFile"></param>
        /// <returns></returns>
        public static List<FieldMetaInfo> LoadFieldsInfo(string configFile)
        {
            var fields = new List<FieldMetaInfo>();
            //判断配置文件是否存在
            var fileName = Path.Combine(FileName, configFile);
            if (!File.Exists(fileName))
            {
                return fields;
            }
            var list = XmlHelper.XmlDeserializeFromFile<FieldList>(fileName, Encoding.UTF8);
            fields.AddRange(list);
            return fields;
        }

        #endregion

    }
}