namespace SpecHelper
{
	partial class FormSetupDatabase
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
			this.label1 = new System.Windows.Forms.Label();
			this.tbDataSource = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.tbInitialCatalog = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.tbUserID = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.tbPassword = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.tbPersistSecurityInfo = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.tbWorkStationID = new System.Windows.Forms.TextBox();
			this.btSave = new System.Windows.Forms.Button();
			this.btTestConnection = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 15);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(67, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Data Source";
			// 
			// tbDataSource
			// 
			this.tbDataSource.Location = new System.Drawing.Point(121, 12);
			this.tbDataSource.Name = "tbDataSource";
			this.tbDataSource.Size = new System.Drawing.Size(258, 20);
			this.tbDataSource.TabIndex = 1;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(12, 41);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(70, 13);
			this.label2.TabIndex = 0;
			this.label2.Text = "Initial Catalog";
			// 
			// tbInitialCatalog
			// 
			this.tbInitialCatalog.Location = new System.Drawing.Point(121, 38);
			this.tbInitialCatalog.Name = "tbInitialCatalog";
			this.tbInitialCatalog.Size = new System.Drawing.Size(258, 20);
			this.tbInitialCatalog.TabIndex = 2;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(12, 67);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(43, 13);
			this.label3.TabIndex = 0;
			this.label3.Text = "User ID";
			// 
			// tbUserID
			// 
			this.tbUserID.Location = new System.Drawing.Point(121, 64);
			this.tbUserID.Name = "tbUserID";
			this.tbUserID.Size = new System.Drawing.Size(258, 20);
			this.tbUserID.TabIndex = 3;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(12, 93);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(53, 13);
			this.label4.TabIndex = 0;
			this.label4.Text = "Password";
			// 
			// tbPassword
			// 
			this.tbPassword.Location = new System.Drawing.Point(121, 90);
			this.tbPassword.Name = "tbPassword";
			this.tbPassword.Size = new System.Drawing.Size(258, 20);
			this.tbPassword.TabIndex = 4;
			this.tbPassword.UseSystemPasswordChar = true;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(12, 119);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(100, 13);
			this.label5.TabIndex = 0;
			this.label5.Text = "Persist Security Info";
			// 
			// tbPersistSecurityInfo
			// 
			this.tbPersistSecurityInfo.Location = new System.Drawing.Point(121, 116);
			this.tbPersistSecurityInfo.Name = "tbPersistSecurityInfo";
			this.tbPersistSecurityInfo.Size = new System.Drawing.Size(258, 20);
			this.tbPersistSecurityInfo.TabIndex = 5;
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(12, 145);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(83, 13);
			this.label6.TabIndex = 0;
			this.label6.Text = "Work Station ID";
			// 
			// tbWorkStationID
			// 
			this.tbWorkStationID.Location = new System.Drawing.Point(121, 142);
			this.tbWorkStationID.Name = "tbWorkStationID";
			this.tbWorkStationID.Size = new System.Drawing.Size(258, 20);
			this.tbWorkStationID.TabIndex = 6;
			// 
			// btSave
			// 
			this.btSave.Location = new System.Drawing.Point(201, 168);
			this.btSave.Name = "btSave";
			this.btSave.Size = new System.Drawing.Size(130, 23);
			this.btSave.TabIndex = 8;
			this.btSave.Text = "Save";
			this.btSave.UseVisualStyleBackColor = true;
			this.btSave.Click += new System.EventHandler(this.btSave_Click);
			// 
			// btTestConnection
			// 
			this.btTestConnection.Location = new System.Drawing.Point(65, 168);
			this.btTestConnection.Name = "btTestConnection";
			this.btTestConnection.Size = new System.Drawing.Size(130, 23);
			this.btTestConnection.TabIndex = 7;
			this.btTestConnection.Text = "Test Connection";
			this.btTestConnection.UseVisualStyleBackColor = true;
			this.btTestConnection.Click += new System.EventHandler(this.btTestConnection_Click);
			// 
			// FormSetupDatabase
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(391, 202);
			this.Controls.Add(this.btTestConnection);
			this.Controls.Add(this.btSave);
			this.Controls.Add(this.tbWorkStationID);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.tbPersistSecurityInfo);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.tbPassword);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.tbUserID);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.tbInitialCatalog);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.tbDataSource);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.Name = "FormSetupDatabase";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Setup Database";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormSetupDatabase_FormClosing);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox tbDataSource;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox tbInitialCatalog;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox tbUserID;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox tbPassword;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox tbWorkStationID;
		private System.Windows.Forms.Button btSave;
		private System.Windows.Forms.Button btTestConnection;
		private System.Windows.Forms.TextBox tbPersistSecurityInfo;
	}
}