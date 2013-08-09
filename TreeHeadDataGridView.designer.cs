namespace CellPaintingDataGridView
{
    partial class TreeHeadDataGridView
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的資源。
        /// </summary>
        /// <param name="disposing">如果應釋放託管資源 為true 否則為false</param>
        protected override void Dispose(bool disposing)
        {
            if (this.treeView1 != null)
            {
                this.treeView1.Dispose();
            }
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 控制項控制工具產生的代碼

        /// <summary>
        /// 設計工具所需方法 - 不要
        /// 不要使用程式碼編輯器修改此內容的方法。
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
        }

        #endregion
    }
}
