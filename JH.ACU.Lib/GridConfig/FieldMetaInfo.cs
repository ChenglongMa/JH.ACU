/* ==============================================================================
 * 功能描述：设置UI Grid格式类  
 * 创 建 者：马成龙
 * 创建日期：2017/4/11 15:51:15
 * ==============================================================================*/

using System.Collections.Generic;
using System.Xml.Serialization;
using Infragistics.Win.UltraWinGrid;

namespace JH.ACU.Lib.GridConfig
{
    [XmlType("Fields")]
    public class FieldList : List<FieldMetaInfo>
    {
        
    }
    /// <summary>
    /// FieldMetaInfo
    /// </summary>
    [XmlType("Field")]
    public class FieldMetaInfo
    {
        #region 构造函数

        public FieldMetaInfo()
        {
        }

        #endregion

        #region 属性字段
        [XmlAttribute]
        public string ColName { get; set; }
        [XmlAttribute]
        public string ColDisplayName { get; set; }
        [XmlAttribute]
        public int Width { get; set; }
        [XmlAttribute]
        public ColumnStyle ColumnStyle { get; set; }
        [XmlAttribute]
        public bool Visible { get; set; }
        [XmlAttribute]
        public bool ReadOnly{ get; set; }

        #endregion

        #region 私有方法

        #endregion

        #region 公有方法

        #endregion

    }
}