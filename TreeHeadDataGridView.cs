using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Drawing;

namespace CellPaintingDataGridView
{
    public partial class TreeHeadDataGridView : DataGridView
    {
        private TreeView treeView1=new TreeView();
        public TreeHeadDataGridView()
        {
            InitializeComponent();
        }

        public TreeHeadDataGridView(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }


        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public TreeNodeCollection HeadSource
        {
            get { return this.treeView1.Nodes; }

        }


        private int _cellHeight = 17;
        private int _columnDeep = 1;
        [Description("設定或取得合併表頭數的深度")]
        public int ColumnDeep
        {
            get
            {
                if (this.Columns.Count == 0)
                    _columnDeep = 1;

                this.ColumnHeadersHeight = _cellHeight * _columnDeep;
                return _columnDeep;
            }

            set
            {
                if (value < 1)
                    _columnDeep = 1;
                else
                    _columnDeep = value;
                this.ColumnHeadersHeight = _cellHeight * _columnDeep;
            }
        }
        
        ///<summary>
        ///畫出合併表頭
        ///</summary>
        ///<param name="node">合併表頭節點</param>
        ///<param name="e">繪圖參數集</param>
        ///<param name="level">節點深度</param>
        ///<remarks></remarks>
        public void PaintUnitHeader(TreeNode node, DataGridViewCellPaintingEventArgs e, int level)
        {
            //根結點時離開遞迴
            if (level == 0)
                return;

            RectangleF uhRectangle;
            int uhWidth;
            SolidBrush gridBrush = new SolidBrush(this.GridColor);

            Pen gridLinePen = new Pen(gridBrush);
            StringFormat textFormat = new StringFormat();


            textFormat.Alignment = StringAlignment.Center;

            uhWidth = GetUnitHeaderWidth(node);

            if (node.Nodes.Count == 0)
            {
                uhRectangle = new Rectangle(e.CellBounds.Left,
                            e.CellBounds.Top + node.Level * _cellHeight,
                            uhWidth - 1,
                            _cellHeight * (_columnDeep - node.Level) - 1);
            }
            else
            {
                uhRectangle = new Rectangle(
                            e.CellBounds.Left,
                            e.CellBounds.Top + node.Level * _cellHeight,
                            uhWidth - 1,
                            _cellHeight - 1);
            }

            Color backColor = e.CellStyle.BackColor;
            if (node.BackColor != Color.Empty)
            {
                backColor = node.BackColor;
            }
            SolidBrush backColorBrush = new SolidBrush(backColor);
            //畫矩形
            e.Graphics.FillRectangle(backColorBrush, uhRectangle);

            //畫底線
            e.Graphics.DrawLine(gridLinePen
                                , uhRectangle.Left
                                , uhRectangle.Bottom
                                , uhRectangle.Right
                                , uhRectangle.Bottom);
            //畫右邊線
            e.Graphics.DrawLine(gridLinePen
                                , uhRectangle.Right
                                , uhRectangle.Top
                                , uhRectangle.Right
                                , uhRectangle.Bottom);

            ////寫字段文本
            Color foreColor = Color.Black;
            if (node.ForeColor != Color.Empty)
            {
                foreColor = node.ForeColor;
            }
            e.Graphics.DrawString(node.Text, this.Font
                                    , new SolidBrush(foreColor)
                                    , uhRectangle.Left + uhRectangle.Width / 2 -
                                    e.Graphics.MeasureString(node.Text, this.Font).Width / 2 - 1
                                    , uhRectangle.Top +
                                    uhRectangle.Height / 2 - e.Graphics.MeasureString(node.Text, this.Font).Height / 2);

            ////遞迴呼叫
            if (node.PrevNode == null)
                if (node.Parent != null)
                    PaintUnitHeader(node.Parent, e, level - 1);
        }

        /// <summary>
        /// 獲得合併標題字段的寬度
        /// </summary>
        /// <param name="node">字段節點</param>
        /// <returns>字段寬度</returns>
        /// <remarks></remarks>
        private int GetUnitHeaderWidth(TreeNode node)
        {
            //獲得非最底層字段的寬度

            int uhWidth = 0;
            //獲得最底層的寬度
            if (node.Nodes == null)
                return this.Columns[GetColumnListNodeIndex(node)].Width;

            if (node.Nodes.Count == 0)
                return this.Columns[GetColumnListNodeIndex(node)].Width;

            for (int i = 0; i <= node.Nodes.Count - 1; i++)
            {
                uhWidth = uhWidth + GetUnitHeaderWidth(node.Nodes[i]);
            }
            return uhWidth;
        }


        /// <summary>
        /// 獲得底層字段索引
        /// </summary>
        /// <param name="node">底層字段節點</param>
        /// <returns>索引</returns>
        /// <remarks></remarks>
        private int GetColumnListNodeIndex(TreeNode node)
        {
            for (int i = 0; i <= _columnList.Count - 1; i++)
            {
                if (((TreeNode)_columnList[i]).Equals(node))
                    return i;
            }
            return -1;
        }

        private List<TreeNode> _columnList = new List<TreeNode>();
        [Description("最底層結點集合")]
        public List<TreeNode> NadirColumnList
        {
            get
            {
                if (this.treeView1 == null)
                    return null;

                if (this.treeView1.Nodes == null)
                    return null;

                if (this.treeView1.Nodes.Count == 0)
                    return null;

                _columnList.Clear();
                foreach (TreeNode node in this.treeView1.Nodes)
                {
                    //GetNadirColumnNodes(_columnList, node, false);
                    GetNadirColumnNodes(_columnList, node);
                }
                return _columnList;
            }
        }

        private void GetNadirColumnNodes(List<TreeNode> alList, TreeNode node)
        {
            if (node.FirstNode == null)
            {
                alList.Add(node);
            }
            foreach (TreeNode n in node.Nodes)
            {
                GetNadirColumnNodes(alList, n);
            }
        }

        /// <summary>
        /// 獲得底層欄位集合
        /// </summary>
        /// <param name="alList">底層欄位集合</param>
        /// <param name="node">欄位節點</param>
        /// <param name="checked">向上搜索與否</param>
        /// <remarks></remarks>
        private void GetNadirColumnNodes(List<TreeNode> alList, TreeNode node, Boolean isChecked)
        {
            if (isChecked == false)
            {
                if (node.FirstNode == null)
                {
                    alList.Add(node);
                    if (node.NextNode != null)
                    {
                        GetNadirColumnNodes(alList, node.NextNode, false);
                        return;
                    }
                    if (node.Parent != null)
                    {
                        GetNadirColumnNodes(alList, node.Parent, true);
                        return;
                    }
                }
                else
                {
                    if (node.FirstNode != null)
                    {
                        GetNadirColumnNodes(alList, node.FirstNode, false);
                        return;
                    }
                }
            }
            else
            {
                if (node.FirstNode == null)
                {
                    return;
                }
                else
                {
                    if (node.NextNode != null)
                    {
                        GetNadirColumnNodes(alList, node.NextNode, false);
                        return;
                    }

                    if (node.Parent != null)
                    {
                        GetNadirColumnNodes(alList, node.Parent, true);
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// 單元格繪製(重寫)
        /// </summary>
        /// <param name="e"></param>
        /// <remarks></remarks>
        protected override void OnCellPainting(DataGridViewCellPaintingEventArgs e)
        {
            //行標題不重寫
            if (e.ColumnIndex < 0)
            {
                base.OnCellPainting(e);
                return;
            }

            if (_columnDeep == 1)
            {
                base.OnCellPainting(e);
                return;
            }

            //繪製表頭
            if (e.RowIndex == -1)
            {
                PaintUnitHeader((TreeNode)NadirColumnList[e.ColumnIndex], e, _columnDeep);
                e.Handled = true;
            }
        }

    }
}


        

