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
            ReadOnly = false;
            Visible = true;
            ColumnStyle = ColumnStyle.Default;
        }

        #endregion

        #region 属性字段

        /// <summary>
        /// 列名
        /// </summary>
        [XmlAttribute]
        public string ColName { get; set; }

        /// <summary>
        /// 列显示名
        /// </summary>
        [XmlAttribute]
        public string ColDisplayName { get; set; }

        /// <summary>
        /// 列宽度
        /// </summary>
        [XmlAttribute]
        public int Width { get; set; }

        private ColumnStyle _columnStyle = ColumnStyle.Default;

        /// <summary>
        /// 列显示类型
        /// </summary>
        [XmlAttribute]
        public ColumnStyle ColumnStyle
        {
            get { return _columnStyle; }
            set { _columnStyle = value; }
        }

        private bool _visible = true;

        /// <summary>
        /// 列可见性
        /// </summary>
        [XmlAttribute]
        public bool Visible
        {
            get { return _visible; }
            set { _visible = value; }
        }

        /// <summary>
        /// 是否只读
        /// </summary>
        [XmlAttribute]
        public bool ReadOnly { get; set; }

        #endregion

        #region 私有方法

        #endregion

        #region 公有方法

        #endregion

    }
}