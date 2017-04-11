/* ==============================================================================
 * 功能描述：Grid格式类Field读取  
 * 创 建 者：马成龙
 * 创建日期：2017/4/11 16:12:24
 * ==============================================================================*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JH.ACU.Lib;
using JH.ACU.Model.Config.GridConfig;
using JH.ACU.Model.Config.InstrumentConfig;


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
            List<FieldMetaInfo> fields = new List<FieldMetaInfo>();
            //判断配置文件是否存在
            string fileName = System.IO.Path.Combine(FileName, configFile);
            if (!System.IO.File.Exists(fileName))
            {
                return fields;
            }
            XmlHelper.XmlDeserializeFromFile<InstrConfig>(InstrFileName, Encoding.UTF8);
        }

        #endregion

    }
}