/* ==============================================================================
 * 功能描述：Report  
 * 创 建 者：Administrator
 * 创建日期：2017/3/23 15:07:16
 * ==============================================================================*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace JH.ACU.Model
{
    /// <summary>
    /// 需要报告给UI层的信息
    /// </summary>
    public class Report
    {
        #region 构造函数

        public Report()
        {
            ValueDic = new Dictionary<string, object>();

        }

        #endregion

        #region 属性字段

        public double Temp { get; set; }
        public double Volt { get; set; }
        public string Message { get; set; }

        /// <summary>
        /// 扩展属性
        /// </summary>
        public Dictionary<string, object> ValueDic { get; set; }

        #endregion

    }
}