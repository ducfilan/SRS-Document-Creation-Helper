namespace SpecHelper
{
    partial class FormSpecHelper
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.tvTable = new System.Windows.Forms.TreeView();
            this.tabStoreProcedureFunction = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tvProgramItem = new System.Windows.Forms.TreeView();
            this.rtbStoredProcedureContent = new System.Windows.Forms.RichTextBox();
            this.tabTableView = new System.Windows.Forms.TabPage();
            this.btSetConnection = new System.Windows.Forms.Button();
            this.tabPageView = new System.Windows.Forms.TabControl();
            this.btBrowseSPFilePath = new System.Windows.Forms.Button();
            this.tbSPFilePath = new System.Windows.Forms.TextBox();
            this.btExportToExcel = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btUpdateSchema = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.tabStoreProcedureFunction.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabTableView.SuspendLayout();
            this.tabPageView.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            this.openFileDialog1.Filter = "xls file|*.xls;*.xlsx";
            // 
            // tvTable
            // 
            this.tvTable.FullRowSelect = true;
            this.tvTable.Location = new System.Drawing.Point(6, 6);
            this.tvTable.Name = "tvTable";
            this.tvTable.Size = new System.Drawing.Size(1135, 580);
            this.tvTable.TabIndex = 0;
            // 
            // tabStoreProcedureFunction
            // 
            this.tabStoreProcedureFunction.Controls.Add(this.splitContainer1);
            this.tabStoreProcedureFunction.Location = new System.Drawing.Point(4, 22);
            this.tabStoreProcedureFunction.Name = "tabStoreProcedureFunction";
            this.tabStoreProcedureFunction.Padding = new System.Windows.Forms.Padding(3);
            this.tabStoreProcedureFunction.Size = new System.Drawing.Size(1147, 592);
            this.tabStoreProcedureFunction.TabIndex = 1;
            this.tabStoreProcedureFunction.Text = "Stored Procedures/Functions/Views";
            this.tabStoreProcedureFunction.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(2, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tvProgramItem);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.rtbStoredProcedureContent);
            this.splitContainer1.Size = new System.Drawing.Size(1145, 586);
            this.splitContainer1.SplitterDistance = 352;
            this.splitContainer1.TabIndex = 6;
            // 
            // tvProgramItem
            // 
            this.tvProgramItem.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvProgramItem.Location = new System.Drawing.Point(0, 0);
            this.tvProgramItem.Name = "tvProgramItem";
            this.tvProgramItem.Size = new System.Drawing.Size(352, 586);
            this.tvProgramItem.TabIndex = 0;
            this.tvProgramItem.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvProgramItem_AfterSelect);
            // 
            // rtbStoredProcedureContent
            // 
            this.rtbStoredProcedureContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbStoredProcedureContent.Location = new System.Drawing.Point(0, 0);
            this.rtbStoredProcedureContent.Name = "rtbStoredProcedureContent";
            this.rtbStoredProcedureContent.Size = new System.Drawing.Size(789, 586);
            this.rtbStoredProcedureContent.TabIndex = 0;
            this.rtbStoredProcedureContent.Text = "";
            // 
            // tabTableView
            // 
            this.tabTableView.Controls.Add(this.tvTable);
            this.tabTableView.Location = new System.Drawing.Point(4, 22);
            this.tabTableView.Name = "tabTableView";
            this.tabTableView.Padding = new System.Windows.Forms.Padding(3);
            this.tabTableView.Size = new System.Drawing.Size(1147, 592);
            this.tabTableView.TabIndex = 0;
            this.tabTableView.Text = "Tables";
            this.tabTableView.UseVisualStyleBackColor = true;
            // 
            // btSetConnection
            // 
            this.btSetConnection.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btSetConnection.Location = new System.Drawing.Point(937, 70);
            this.btSetConnection.Name = "btSetConnection";
            this.btSetConnection.Size = new System.Drawing.Size(230, 23);
            this.btSetConnection.TabIndex = 8;
            this.btSetConnection.Text = "Setup Connection";
            this.btSetConnection.UseVisualStyleBackColor = true;
            this.btSetConnection.Click += new System.EventHandler(this.btSetConnection_Click);
            // 
            // tabPageView
            // 
            this.tabPageView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabPageView.Controls.Add(this.tabTableView);
            this.tabPageView.Controls.Add(this.tabStoreProcedureFunction);
            this.tabPageView.Location = new System.Drawing.Point(12, 111);
            this.tabPageView.Name = "tabPageView";
            this.tabPageView.SelectedIndex = 0;
            this.tabPageView.Size = new System.Drawing.Size(1155, 618);
            this.tabPageView.TabIndex = 9;
            // 
            // btBrowseSPFilePath
            // 
            this.btBrowseSPFilePath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btBrowseSPFilePath.Location = new System.Drawing.Point(1113, 17);
            this.btBrowseSPFilePath.Name = "btBrowseSPFilePath";
            this.btBrowseSPFilePath.Size = new System.Drawing.Size(30, 23);
            this.btBrowseSPFilePath.TabIndex = 3;
            this.btBrowseSPFilePath.Text = "...";
            this.btBrowseSPFilePath.UseVisualStyleBackColor = true;
            this.btBrowseSPFilePath.Click += new System.EventHandler(this.btBrowseSPFilePath_Click);
            // 
            // tbSPFilePath
            // 
            this.tbSPFilePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbSPFilePath.Location = new System.Drawing.Point(105, 19);
            this.tbSPFilePath.Name = "tbSPFilePath";
            this.tbSPFilePath.Size = new System.Drawing.Size(1002, 20);
            this.tbSPFilePath.TabIndex = 2;
            // 
            // btExportToExcel
            // 
            this.btExportToExcel.Location = new System.Drawing.Point(12, 70);
            this.btExportToExcel.Name = "btExportToExcel";
            this.btExportToExcel.Size = new System.Drawing.Size(230, 23);
            this.btExportToExcel.TabIndex = 6;
            this.btExportToExcel.Text = "Export To Excel";
            this.btExportToExcel.UseVisualStyleBackColor = true;
            this.btExportToExcel.Click += new System.EventHandler(this.btExportToExcel_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.btBrowseSPFilePath);
            this.groupBox1.Controls.Add(this.tbSPFilePath);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1155, 52);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Database Information";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(4, 22);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(66, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "EXCEL FILE";
            // 
            // btUpdateSchema
            // 
            this.btUpdateSchema.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btUpdateSchema.Location = new System.Drawing.Point(415, 70);
            this.btUpdateSchema.Name = "btUpdateSchema";
            this.btUpdateSchema.Size = new System.Drawing.Size(230, 23);
            this.btUpdateSchema.TabIndex = 7;
            this.btUpdateSchema.Text = "Update Schema";
            this.btUpdateSchema.UseVisualStyleBackColor = true;
            this.btUpdateSchema.Click += new System.EventHandler(this.btUpdateSchema_Click);
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Location = new System.Drawing.Point(415, 107);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(230, 20);
            this.textBox1.TabIndex = 7;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // FormSpecHelper
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1179, 741);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.btSetConnection);
            this.Controls.Add(this.tabPageView);
            this.Controls.Add(this.btUpdateSchema);
            this.Controls.Add(this.btExportToExcel);
            this.Controls.Add(this.groupBox1);
            this.Name = "FormSpecHelper";
            this.Text = "FormSpecHelper";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormSpecHelper_FormClosing);
            this.Load += new System.EventHandler(this.FormSpecHelper_Load);
            this.tabStoreProcedureFunction.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.tabTableView.ResumeLayout(false);
            this.tabPageView.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.TreeView tvTable;
        private System.Windows.Forms.TabPage tabStoreProcedureFunction;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TreeView tvProgramItem;
        private System.Windows.Forms.RichTextBox rtbStoredProcedureContent;
        private System.Windows.Forms.TabPage tabTableView;
        private System.Windows.Forms.Button btSetConnection;
        private System.Windows.Forms.TabControl tabPageView;
        private System.Windows.Forms.Button btBrowseSPFilePath;
        private System.Windows.Forms.TextBox tbSPFilePath;
        private System.Windows.Forms.Button btExportToExcel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btUpdateSchema;
        private System.Windows.Forms.TextBox textBox1;
    }
}