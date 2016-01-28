using System;
using System.Data;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace SpecHelper
{
	public partial class FormSetupDatabase : Form
	{
		public FormSetupDatabase()
		{
			InitializeComponent();

			tbDataSource.Text = Properties.Settings.Default.DataSource;
			tbInitialCatalog.Text = Properties.Settings.Default.InitialCatalog;
			tbUserID.Text = Properties.Settings.Default.UserID;
			tbPassword.Text = Properties.Settings.Default.Password;
			tbPersistSecurityInfo.Text = String.IsNullOrEmpty(Properties.Settings.Default.PersitSecurityInfo)
										? "false"
										: Properties.Settings.Default.PersitSecurityInfo;
			tbWorkStationID.Text = String.IsNullOrEmpty(Properties.Settings.Default.WorkStationID)
									   ? Environment.MachineName
									   : Properties.Settings.Default.WorkStationID;
		}

		private void btTestConnection_Click(object sender, EventArgs e)
		{
			var result = testConnection();

			if (result)
			{
				MessageBox.Show("Connect to database successfully!", "Embrace Portal Database Manager");
			}
			else
			{
				MessageBox.Show("Cannot connect to the database!", "Embrace Portal Database Manager");
			}
		}

		private bool testConnection()
		{
			string connectionString =
				"Data Source={0};Initial Catalog= {1};User Id={2};Password={3}; Persist Security Info={4}; Workstation ID={5}";

			//connectionString = "Server = FRD-THINHLD1-LA; Database = Embrace_AU_QA65; User Id = pwi; Password = Ready2Go";
			var connection = new SqlConnection
			                     {
			                         ConnectionString = string.Format(connectionString, tbDataSource.Text,
			                                                          tbInitialCatalog.Text,
			                                                          tbUserID.Text,
			                                                          tbPassword.Text,
			                                                          tbPersistSecurityInfo.Text,
			                                                          tbWorkStationID.Text)
			                     };
		    try
			{
				connection.Open();
				if (connection.State == ConnectionState.Open)
				{
					connection.Close();
					return true;
				}
			}
			catch (Exception exception)
			{
			}
            return false;
		}

		private void btSave_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void FormSetupDatabase_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (testConnection())
			{
				Properties.Settings.Default.DataSource = tbDataSource.Text;
				Properties.Settings.Default.InitialCatalog = tbInitialCatalog.Text;
				Properties.Settings.Default.UserID = tbUserID.Text;
				Properties.Settings.Default.Password = tbPassword.Text;
				Properties.Settings.Default.PersitSecurityInfo = tbPersistSecurityInfo.Text;
				Properties.Settings.Default.WorkStationID = tbWorkStationID.Text;
				Properties.Settings.Default.Save();
			}
		}
	}
}