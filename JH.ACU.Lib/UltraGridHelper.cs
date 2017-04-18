using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win;
using System.Windows.Forms;
using JH.ACU.Lib.GridConfig;

//using ColumnStyle = Infragistics.Win.UltraWinGrid.ColumnStyle;

namespace JH.ACU.Lib
{
    /// <summary>
    /// 格式化UltraGrid
    /// </summary>
    public static class UltraGridHelper
    {
        /// <summary>
        /// 设置网格控件的默认格式
        /// </summary>
        /// <param name="grid"></param>
        public static void SetGridDefaultStyle(this UltraGrid grid)
        {
            //设置UltraGrid样式
            grid.DisplayLayout.Override.AllowUpdate = DefaultableBoolean.True;
            grid.DisplayLayout.Override.AllowDelete = DefaultableBoolean.False;
            grid.DisplayLayout.EmptyRowSettings.ShowEmptyRows = true;
            grid.DisplayLayout.EmptyRowSettings.Style = EmptyRowStyle.ExtendRowSelector;
            grid.DisplayLayout.Override.HeaderAppearance.TextHAlign = HAlign.Center;
            grid.DisplayLayout.Override.RowAlternateAppearance.BackColor = Color.WhiteSmoke;
            grid.DisplayLayout.Override.RowAlternateAppearance.ForeColor = Color.Black;
            grid.DisplayLayout.Override.RowAppearance.BorderColor = Color.Silver;
            grid.DisplayLayout.Override.HeaderClickAction = HeaderClickAction.SortMulti;
            grid.StyleSetName = "StyleSetBlue";

            //行被选择时候的样式
            //Infragistics.Win.Appearance activeRowApperance = new Infragistics.Win.Appearance();
            //activeRowApperance.BackColor = System.Drawing.SystemColors.Highlight;
            //activeRowApperance.ForeColor = System.Drawing.SystemColors.HighlightText;
            //grid.DisplayLayout.Override.ActiveRowAppearance = activeRowApperance;

            //行号
            grid.DisplayLayout.Override.RowSelectorNumberStyle = RowSelectorNumberStyle.VisibleIndex;
            grid.DisplayLayout.Override.RowSelectors = DefaultableBoolean.True;
            grid.DisplayLayout.Override.RowSelectorWidth = 35;
            grid.DisplayLayout.Override.RowSelectorHeaderStyle = RowSelectorHeaderStyle.SeparateElement;
            grid.DisplayLayout.Override.RowSelectorAppearance.TextVAlign = VAlign.Middle;

            //grid.DisplayLayout.ViewStyleBand = ViewStyleBand.OutlookGroupBy;
            //grid.DisplayLayout.GroupByBox.Hidden = false;
            //grid.DisplayLayout.Override.SummaryDisplayArea = SummaryDisplayAreas.BottomFixed | SummaryDisplayAreas.GroupByRowsFooter | SummaryDisplayAreas.InGroupByRows;

            //汇总区域背景颜色
            grid.DisplayLayout.Override.SummaryFooterAppearance.BackColor = Color.LightYellow;

            //固定列头
            grid.DisplayLayout.UseFixedHeaders = true;

            //默认行高
            //grid.DisplayLayout.AutoFitStyle = AutoFitStyle.ExtendLastColumn;


            //列头样式 XP风格
            grid.DisplayLayout.Override.HeaderStyle = HeaderStyle.WindowsXPCommand;



            //System.Windows.Forms.Keys keys = System.Windows.Forms.Keys.Up;
            //UltraGridAction action = UltraGridAction.AboveCell;
            //UltraGridState requiredState = UltraGridState.Cell;
            //GridKeyActionMapping keyMapping = new GridKeyActionMapping(keys, action, 0, requiredState, 0, 0);
            //grid.KeyActionMappings.Add(keyMapping);


            //keys = System.Windows.Forms.Keys.Down;
            //action = UltraGridAction.BelowCell;
            //requiredState = UltraGridState.Cell;
            //keyMapping = new GridKeyActionMapping(keys, action, 0, requiredState, 0, 0);
            //grid.KeyActionMappings.Add(keyMapping);

            //grid.DisplayLayout.Override.FilterUIType = FilterUIType.FilterRow;
            //grid.DisplayLayout.Override.FilterEvaluationTrigger = FilterEvaluationTrigger.OnCellValueChange;
            //grid.DisplayLayout.Override.FilterOperatorLocation = FilterOperatorLocation.WithOperand;
            //grid.DisplayLayout.Override.FilterOperatorDefaultValue = FilterOperatorDefaultValue.StartsWith;
            //grid.DisplayLayout.Override.FilterClearButtonLocation = FilterClearButtonLocation.RowAndCell;
            //grid.DisplayLayout.Override.FilterRowAppearance.BackColor = Color.LightYellow;
            grid.DisplayLayout.Override.SummaryFooterAppearance.FontData.Bold = DefaultableBoolean.True;
            grid.DisplayLayout.Override.AllowMultiCellOperations = AllowMultiCellOperation.All;
            grid.KeyDown += grid_KeyDown;

        }

        private static void grid_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.Control && e.KeyCode == Keys.C)
            {
                ((UltraGrid) sender).PerformAction(UltraGridAction.Copy);

            }
            if (e.Control && e.KeyCode == Keys.Z)
            {
                (sender as UltraGrid).PerformAction(UltraGridAction.Undo);

            }
            if (e.Control && e.KeyCode == Keys.Y)
            {
                (sender as UltraGrid).PerformAction(UltraGridAction.Redo);

            }
        }


        public static void SetFilterUI(UltraGrid grid)
        {
            grid.DisplayLayout.Override.FilterUIType = FilterUIType.FilterRow;
            grid.DisplayLayout.Override.FilterEvaluationTrigger = FilterEvaluationTrigger.OnCellValueChange;
            grid.DisplayLayout.Override.FilterOperatorLocation = FilterOperatorLocation.WithOperand;
            grid.DisplayLayout.Override.FilterOperatorDefaultValue = FilterOperatorDefaultValue.StartsWith;
            grid.DisplayLayout.Override.FilterClearButtonLocation = FilterClearButtonLocation.RowAndCell;
            grid.DisplayLayout.Override.FilterRowAppearance.BackColor = Color.LightYellow;

        }

        /// <summary>
        /// 设置默认操作符
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="operation"></param>
        public static void SetColDefaultOperation(UltraGrid grid, FilterOperatorDefaultValue operation)
        {
            if (grid.DisplayLayout.Bands[0] == null)
                return;
            foreach (UltraGridColumn col in grid.DisplayLayout.Bands[0].Columns)
            {
                col.FilterOperatorDefaultValue = operation;
            }
        }

        public static void SetFilterUIforBand(UltraGridBand gridBand)
        {
            gridBand.Override.FilterUIType = FilterUIType.FilterRow;
            gridBand.Override.FilterEvaluationTrigger = FilterEvaluationTrigger.OnCellValueChange;
            gridBand.Override.FilterOperatorLocation = FilterOperatorLocation.WithOperand;
            gridBand.Override.FilterOperatorDefaultValue = FilterOperatorDefaultValue.StartsWith;
            gridBand.Override.FilterClearButtonLocation = FilterClearButtonLocation.RowAndCell;
            gridBand.Override.FilterRowAppearance.BackColor = Color.LightYellow;

        }
        /// <summary>
        /// 为Grid设置样式
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="fields"></param>
        public static void SetStyle(this UltraGrid grid, List<FieldMetaInfo> fields)
        {
            grid.DisplayLayout.Override.RowSelectorNumberStyle = RowSelectorNumberStyle.VisibleIndex;
            if (grid.DisplayLayout.Bands.Count == 0)
                return;
            foreach (var column in grid.DisplayLayout.Bands[0].Columns)
            {
                column.Hidden = true;
            }
            foreach (var field in fields)
            {
                //获取列
                if (grid.DisplayLayout.Bands[0].Columns.IndexOf(field.ColName) < 0)
                    continue;
                var col = grid.DisplayLayout.Bands[0].Columns[field.ColName];
                col.Style = field.ColumnStyle;
                col.Hidden = !field.Visible;
                col.Width = field.Width;
                col.Header.Caption = field.ColDisplayName;
                col.Header.VisiblePosition = fields.IndexOf(field);
                col.CellActivation = field.ReadOnly ? Activation.NoEdit : Activation.AllowEdit;
            }
            grid.AutoSizeAllCol();
        }

        public static void SetStyle<T>(this UltraGrid grid, List<T> dataSource, List<FieldMetaInfo> fields)
        {
            grid.DisplayLayout.Override.RowSelectorNumberStyle = RowSelectorNumberStyle.VisibleIndex;
            if (grid.DisplayLayout.Bands.Count == 0)
                return;
            var type = typeof (T);
            foreach (var field in fields)
            {
                var propertyName = field.ColName;
                var pro = type.GetProperties().FirstOrDefault(p => p.Name == propertyName);
                if (pro == null) continue;
                var value = pro.GetValue(t, null);
                var col = grid.DisplayLayout.Bands[0].Columns.Add(propertyName, field.ColDisplayName);
                col.Style = field.ColumnStyle;
                col.Hidden = !field.Visible;
                col.Width = field.Width;
                col.Header.Caption = field.ColDisplayName;
                col.Header.VisiblePosition = fields.IndexOf(field);
                col.CellActivation = field.ReadOnly ? Activation.NoEdit : Activation.AllowEdit;
            }

            foreach (var t in dataSource)
            {
            }
        }

/*
            /// <summary>
            /// 设置网格控件的列格式
            /// </summary>
            /// <param name="grid"></param>
            /// <param name="cols"></param>
            public static void ChangeColumnStyle(UltraGrid grid, List<FieldMetaInfo> cols)
            {
                if (grid.DisplayLayout.Bands.Count == 0)
                    return;

                grid.DisplayLayout.Bands[0].Summaries.Clear();
                foreach (FieldMetaInfo field in cols)
                {
                    //获取列
                    if (grid.DisplayLayout.Bands[0].Columns.IndexOf(field.ColName) < 0)
                        return;
                    UltraGridColumn col = grid.DisplayLayout.Bands[0].Columns[field.ColName];

                    if (field.ColType == "Decimal" || field.ColType == "Int32" || field.ColType == "Int")
                    {
                        col.CellAppearance.TextHAlign = HAlign.Right;
                    }

                    //if (field.ColType == "Decimal")
                    //{
                    //    col.Style = ColumnStyle.Double;
                    //}
                    //if ( field.ColType == "Int32" || field.ColType == "Int")
                    //{
                    //    col.Style = ColumnStyle.Integer;
                    //}
                    //col.Format = field.Format;

                    //是否允许按列过滤
                    if (field.Filter)
                        col.AllowRowFiltering = DefaultableBoolean.True;
                    if (field.Width > 0)
                    {
                        col.Width = field.Width;
                    }

                    //汇总
                    if (field.SumType != "")
                    {
                        SummarySettings summarySetting = grid.DisplayLayout.Bands[0].Summaries.Add(
                                   (SummaryType)Enum.Parse(typeof(SummaryType), field.SumType),
                                   col,
                                   SummaryPosition.UseSummaryPositionColumn);
                        //summarySetting.SummaryDisplayArea = SummaryDisplayAreas.BottomFixed | SummaryDisplayAreas.GroupByRowsFooter | SummaryDisplayAreas.InGroupByRows;
                        summarySetting.SummaryDisplayArea = SummaryDisplayAreas.BottomFixed;
                        summarySetting.Appearance.BackColor = Color.LightYellow;
                        summarySetting.DisplayFormat = "{0}";

                        if (field.ColType == "Decimal" || field.ColType == "Int32" || field.ColType == "Int")
                            summarySetting.Appearance.TextHAlign = HAlign.Right;
                    }

                    if (field.IsFixed)
                    {
                        col.Header.FixedHeaderIndicator = FixedHeaderIndicator.Button;
                        //col.Header.Fixed = true;
                    }
                    else
                    {
                        col.Header.FixedHeaderIndicator = FixedHeaderIndicator.None;
                    }

                    if (field.CellType != "")
                    {
                        ColumnStyle style = (ColumnStyle)Enum.Parse(typeof(ColumnStyle), field.CellType);
                        col.Style = style;
                    }

                    if (!field.Visible)
                    {
                        col.Hidden = true;
                    }
                    else
                    {
                        col.Hidden = false;
                    }

                }

                if (grid.Rows.Count > 0 && grid.Rows[0].Cells != null)
                {
                    //grid.Rows[0].Cells[0].Activate();
                }

            }

            /// <summary>
            /// 设置网格控件的列格式2
            /// </summary>
            /// <param name="grid"></param>
            /// <param name="cols"></param>
            public static void ChangeColumnStyle(UltraGrid grid, List<FieldMetaInfo>[] cols)
            {
                if (grid.DisplayLayout.Bands.Count == 0 || grid.DisplayLayout.Bands.Count < cols.Length)
                    return;
                int count = cols.Length;
                for (int i = 0; i < count; i++)
                {
                    foreach (UltraGridColumn ugc in grid.DisplayLayout.Bands[i].Columns)
                    {
                        ugc.Hidden = true;
                    }
                    grid.DisplayLayout.Bands[i].Summaries.Clear();
                    foreach (FieldMetaInfo field in cols[i])
                    {
                        //获取列
                        if (grid.DisplayLayout.Bands[i].Columns.IndexOf(field.ColName) >= 0)
                        {
                            UltraGridColumn col = grid.DisplayLayout.Bands[i].Columns[field.ColName];
                            col.Header.Caption = field.ColDisplayName;
                            col.Header.VisiblePosition = cols[i].IndexOf(field);
                            if (field.ColType == "Decimal" || field.ColType == "Int32" || field.ColType == "Int")
                                col.CellAppearance.TextHAlign = HAlign.Right;

                            //col.Format = field.Format;

                            //是否允许按列过滤
                            if (field.Filter)
                                col.AllowRowFiltering = DefaultableBoolean.True;
                            if (field.Width > 0)
                            {
                                col.Width = field.Width;
                            }

                            //汇总
                            if (field.SumType != "")
                            {
                                SummarySettings summarySetting = grid.DisplayLayout.Bands[i].Summaries.Add(
                                           (SummaryType)Enum.Parse(typeof(SummaryType), field.SumType),
                                           col,
                                           SummaryPosition.UseSummaryPositionColumn);
                                //summarySetting.SummaryDisplayArea = SummaryDisplayAreas.BottomFixed | SummaryDisplayAreas.GroupByRowsFooter | SummaryDisplayAreas.InGroupByRows;
                                summarySetting.SummaryDisplayArea = SummaryDisplayAreas.BottomFixed;
                                summarySetting.Appearance.BackColor = Color.LightYellow;
                                summarySetting.DisplayFormat = "{0}";

                                if (field.ColType == "Decimal" || field.ColType == "Int32" || field.ColType == "Int")
                                    summarySetting.Appearance.TextHAlign = HAlign.Right;
                            }

                            if (field.IsFixed)
                            {
                                col.Header.FixedHeaderIndicator = FixedHeaderIndicator.Button;
                            }
                            else
                            {
                                col.Header.FixedHeaderIndicator = FixedHeaderIndicator.None;
                            }

                            if (field.Format != "")
                            {
                                col.Format = field.Format;
                            }

                            //if (field.CellType != "")
                            //{
                            //    ColumnStyle style = (ColumnStyle)Enum.Parse(typeof(ColumnStyle), field.CellType);
                            //    col.Style = style;
                            //}

                            if (!field.Visible)
                            {
                                col.Hidden = true;
                            }
                            else
                            {
                                col.Hidden = false;
                            }
                        }
                    }
                }

                if (grid.Rows.Count > 0)
                {
                    grid.Rows[0].Cells[0].Activate();
                }
            }

            /// <summary>
            /// 设置网格控件的列格式2
            /// </summary>
            /// <param name="grid"></param>
            /// <param name="cols"></param>
            public static void ChangeColumnStyle(UltraGridBand gridband, List<FieldMetaInfo> cols)
            {

                for (int i = 0; i < cols.Count; i++)
                {
                    foreach (UltraGridColumn ugc in gridband.Columns)
                    {
                        ugc.Hidden = true;
                    }
                    gridband.Summaries.Clear();
                    foreach (FieldMetaInfo field in cols)
                    {
                        //获取列
                        if (gridband.Columns.IndexOf(field.ColName) >= 0)
                        {
                            UltraGridColumn col = gridband.Columns[field.ColName];
                            col.Header.Caption = field.ColDisplayName;
                            col.Header.VisiblePosition = cols.IndexOf(field);
                            if (field.ColType == "Decimal" || field.ColType == "Int32" || field.ColType == "Int")
                                col.CellAppearance.TextHAlign = HAlign.Right;

                            //col.Format = field.Format;

                            //是否允许按列过滤
                            if (field.Filter)
                                col.AllowRowFiltering = DefaultableBoolean.True;
                            if (field.Width > 0)
                            {
                                col.Width = field.Width;
                            }

                            //汇总
                            if (field.SumType != "")
                            {
                                SummarySettings summarySetting = gridband.Summaries.Add(
                                           (SummaryType)Enum.Parse(typeof(SummaryType), field.SumType),
                                           col,
                                           SummaryPosition.UseSummaryPositionColumn);
                                //summarySetting.SummaryDisplayArea = SummaryDisplayAreas.BottomFixed | SummaryDisplayAreas.GroupByRowsFooter | SummaryDisplayAreas.InGroupByRows;
                                summarySetting.SummaryDisplayArea = SummaryDisplayAreas.BottomFixed;
                                summarySetting.Appearance.BackColor = Color.LightYellow;
                                summarySetting.DisplayFormat = "{0}";

                                if (field.ColType == "Decimal" || field.ColType == "Int32" || field.ColType == "Int")
                                    summarySetting.Appearance.TextHAlign = HAlign.Right;
                            }

                            if (field.IsFixed)
                            {
                                col.Header.FixedHeaderIndicator = FixedHeaderIndicator.Button;
                            }
                            else
                            {
                                col.Header.FixedHeaderIndicator = FixedHeaderIndicator.None;
                            }

                            if (field.Format != "")
                            {
                                col.Format = field.Format;
                            }

                            //if (field.CellType != "")
                            //{
                            //    ColumnStyle style = (ColumnStyle)Enum.Parse(typeof(ColumnStyle), field.CellType);
                            //    col.Style = style;
                            //}

                            if (!field.Visible)
                            {
                                col.Hidden = true;
                            }
                            else
                            {
                                col.Hidden = false;
                            }
                        }
                    }
                }

                //if (grid.Rows.Count > 0)
                //{
                //    grid.Rows[0].Cells[0].Activate();
                //}
            }
        */

        /// <summary>
        /// 按照列内容调整列宽度
        /// </summary>
        /// <param name="grid"></param>
        public static void AutoSizeAllCol(this UltraGrid grid)
        {
            if (grid.DisplayLayout.Bands.Count == 0)
                return;
            if (grid.DisplayLayout.Bands[0].Columns == null)
                return;
            foreach (var col in grid.DisplayLayout.Bands[0].Columns.Cast<UltraGridColumn>().Where(col => !col.Hidden))
            {
                col.PerformAutoResize(PerformAutoSizeType.AllRowsInBand);
            }
        }

        /// <summary>
        /// 设置
        /// </summary>
        /// <param name="grid"></param>
        public static void AllowColumnChooser(UltraGrid grid)
        {
            grid.DisplayLayout.Override.RowSelectorHeaderStyle = RowSelectorHeaderStyle.ColumnChooserButton;
        }

        /// <summary>
        /// 设置网格的过滤方式
        /// </summary>
        /// <param name="grid"></param>
        public static void SetFilterType(UltraGrid grid)
        {
            grid.DisplayLayout.Override.FilterUIType = FilterUIType.FilterRow;
            grid.DisplayLayout.Override.FilterEvaluationTrigger = FilterEvaluationTrigger.OnCellValueChange;
            grid.DisplayLayout.Override.FilterOperatorLocation = FilterOperatorLocation.WithOperand;
            grid.DisplayLayout.Override.FilterOperatorDefaultValue = FilterOperatorDefaultValue.StartsWith;
            grid.DisplayLayout.Override.FilterClearButtonLocation = FilterClearButtonLocation.RowAndCell;
            grid.DisplayLayout.Override.FilterRowAppearance.BackColor = Color.LightYellow;
        }

        /// <summary>
        /// 取消行号显示
        /// </summary>
        /// <param name="grid"></param>
        public static void DisableRowNumber(UltraGrid grid)
        {
            grid.DisplayLayout.Override.RowSelectorNumberStyle = RowSelectorNumberStyle.None;
        }

        /// <summary>
        /// 隐藏子band的列标题
        /// </summary>
        /// <param name="grid"></param>
        public static void HideChildGridHead(UltraGrid grid)
        {
            //格式化子Band
            for (int i = 0; i < grid.DisplayLayout.Bands.Count; i++)
            {
                if (i != 0)
                {
                    grid.DisplayLayout.Bands[i].ColHeadersVisible = false;
                }
            }
        }

        /// <summary>
        /// 设置Grid不可编辑
        /// </summary>
        /// <param name="grid"></param>
        public static void SetGridReadOnly(UltraGrid grid)
        {
            grid.DisplayLayout.Override.AllowAddNew = AllowAddNew.No;
            grid.DisplayLayout.Override.AllowDelete = DefaultableBoolean.False;
            grid.DisplayLayout.Override.AllowUpdate = DefaultableBoolean.False;
        }


        /// <summary>
        /// 设置Grid可编辑
        /// </summary>
        /// <param name="grid"></param>
        public static void SetGridEditable(UltraGrid grid)
        {
            grid.DisplayLayout.Override.AllowAddNew = AllowAddNew.TemplateOnBottom;
            grid.DisplayLayout.Override.AllowDelete = DefaultableBoolean.True;
            grid.DisplayLayout.Override.AllowUpdate = DefaultableBoolean.True;
        }

        public static void SetGridAddRow(UltraGrid grid)
        {
            grid.DisplayLayout.Override.AllowUpdate = DefaultableBoolean.True;
            grid.DisplayLayout.Override.AllowAddNew = AllowAddNew.TemplateOnBottom;
            grid.DisplayLayout.Override.TemplateAddRowAppearance.ForeColor = SystemColors.GrayText;
            grid.DisplayLayout.Override.AllowDelete = DefaultableBoolean.True;
            grid.DisplayLayout.Override.TemplateAddRowPrompt = "点击添加记录";
        }

        /// <summary>
        /// 设置方向键
        /// </summary>
        /// <param name="grid"></param>
        public static void SetUpDownKey(UltraGrid grid)
        {

            Keys keys = Keys.Up;
            UltraGridAction action = UltraGridAction.AboveCell;
            UltraGridState requiredState = UltraGridState.Cell;
            GridKeyActionMapping keyMapping = new GridKeyActionMapping(keys, action, 0, requiredState, 0, 0);
            grid.KeyActionMappings.Add(keyMapping);

            keys = Keys.Up;
            action = UltraGridAction.EnterEditMode;
            requiredState = UltraGridState.Cell;
            keyMapping = new GridKeyActionMapping(keys, action, 0, requiredState, 0, 0);
            grid.KeyActionMappings.Add(keyMapping);

            keys = Keys.Down;
            action = UltraGridAction.BelowCell;
            requiredState = UltraGridState.Cell;
            keyMapping = new GridKeyActionMapping(keys, action, 0, requiredState, 0, 0);
            grid.KeyActionMappings.Add(keyMapping);

            keys = Keys.Down;
            action = UltraGridAction.EnterEditMode;
            requiredState = UltraGridState.Cell;
            keyMapping = new GridKeyActionMapping(keys, action, 0, requiredState, 0, 0);
            grid.KeyActionMappings.Add(keyMapping);

            //keys = System.Windows.Forms.Keys.Left;
            //action = UltraGridAction.PrevCellByTab;
            //requiredState = UltraGridState.Cell;
            //keyMapping = new GridKeyActionMapping(keys, action, 0, requiredState, 0, 0);
            //grid.KeyActionMappings.Add(keyMapping);


            //keys = System.Windows.Forms.Keys.Right;
            //action = UltraGridAction.NextCellByTab;
            //requiredState = UltraGridState.Cell;
            //keyMapping = new GridKeyActionMapping(keys, action, 0, requiredState, 0, 0);
            //grid.KeyActionMappings.Add(keyMapping);
        }


        /// <summary>
        /// 设置方向键
        /// </summary>
        /// <param name="grid"></param>
        public static void SetLeftRightKey(UltraGrid grid)
        {

            Keys keys = Keys.Left;
            UltraGridAction action = UltraGridAction.PrevCellByTab;
            UltraGridState requiredState = UltraGridState.Cell;
            GridKeyActionMapping keyMapping = new GridKeyActionMapping(keys, action, 0, requiredState, 0, 0);
            grid.KeyActionMappings.Add(keyMapping);


            keys = Keys.Right;
            action = UltraGridAction.NextCellByTab;
            requiredState = UltraGridState.Cell;
            keyMapping = new GridKeyActionMapping(keys, action, 0, requiredState, 0, 0);
            grid.KeyActionMappings.Add(keyMapping);
        }


        /// <summary>
        /// 数据绑定列表头设置 
        /// </summary>
        /// <param name="ColunmName">列名</param>
        /// <param name="VisiblePosition">初始位置</param>
        /// <param name="OriginX">x轴坐标</param>
        /// <param name="OriginY">y轴坐标</param>
        /// <param name="SpanY">y向占几个格</param>
        public static void DataColumn_Layout(UltraGridColumn ultraGridColumn, int VisiblePosition, int OriginX,
            int OriginY, int SpanY)
        {
            ultraGridColumn.Header.VisiblePosition = VisiblePosition;
            ultraGridColumn.RowLayoutColumnInfo.OriginX = OriginX;
            ultraGridColumn.RowLayoutColumnInfo.OriginY = OriginY;
            ultraGridColumn.RowLayoutColumnInfo.PreferredLabelSize = new Size(20, 20);
            ultraGridColumn.RowLayoutColumnInfo.SpanX = 2;
            ultraGridColumn.RowLayoutColumnInfo.SpanY = SpanY;
        }

        /// <summary>
        /// 无数据绑定列表头设置
        /// </summary>
        /// <param name="ultraGridColumn"></param>
        /// <param name="VisiblePosition">初始位置</param>
        /// <param name="OriginX">x轴坐标</param>
        /// <param name="childColumCount">子列数</param>
        public static void NoDataColumn_Layout(UltraGridColumn ultraGridColumn, int VisiblePosition, int OriginX,
            int childColumCount)
        {
            ultraGridColumn.Header.VisiblePosition = VisiblePosition;
            ultraGridColumn.RowLayoutColumnInfo.LabelPosition = LabelPosition.LabelOnly;
            ultraGridColumn.RowLayoutColumnInfo.OriginX = OriginX;
            ultraGridColumn.RowLayoutColumnInfo.OriginY = 0;
            ultraGridColumn.RowLayoutColumnInfo.PreferredCellSize = new Size(childColumCount*20, 0);
            ultraGridColumn.RowLayoutColumnInfo.SpanX = childColumCount*2;
            ultraGridColumn.RowLayoutColumnInfo.SpanY = 1;
        }

        /// <summary>
        /// 整个表格不可编辑
        /// </summary>
        /// <param name="grid"></param>
        public static void SetWholeGridNoEdit(ref UltraGrid grid)
        {
            grid.DisplayLayout.Override.AllowUpdate =
                DefaultableBoolean.False;
        }

        /// <summary>
        /// 列不可编辑 
        /// </summary>
        /// <param name="column">某列</param>
        public static void SetGridColumnNoEdit(UltraGridColumn column)
        {
            column.CellActivation =
                Activation.NoEdit;
        }

        /// <summary>
        /// 设置指定单元格进入编辑状态
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="activeCell">指定单元格</param>
        public static void FocusOnCell(ref UltraGrid grid, UltraGridCell activeCell)
        {
            grid.ActiveCell = activeCell;
            grid.PerformAction(UltraGridAction.EnterEditMode, false, false);
        }


    }
}