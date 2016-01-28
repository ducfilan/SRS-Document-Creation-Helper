using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SpecHelper
{
    // Todo: What happen with Re-Init, Re-Analyze...

    public partial class FormSpecHelper : Form
    {
        private List<ClassItem> _classItems;
        private List<TableItem> _tableItems;
        private List<string> _ignoredItems;
        public static string FileName = "";
        private string _folderPath = "";

        public FormSpecHelper()
        {
            InitializeComponent();
        }

        private void FormSpecHelper_Load(object sender, EventArgs e)
        {
            tbSPFilePath.Text = Properties.Settings.Default.SPPath;

            _folderPath = Path.GetDirectoryName(tbSPFilePath.Text);
            FileName = Path.GetFileName(tbSPFilePath.Text);
            // Check for connection.
            if (ConnectionManager.GetConnection() == null)
            {
                new FormSetupDatabase().ShowDialog();
            }

            // Insert base localization tables
            var connection = ConnectionManager.GetConnection();
            if (connection != null)
            {
                var query = @"  --Create culture master table AH_MASTER_SUPPORTED_CULTURES
							IF OBJECT_ID('dbo.AH_MASTER_SUPPORTED_CULTURES') IS NULL
							  BEGIN
								  CREATE TABLE AH_MASTER_SUPPORTED_CULTURES(
								   Culture_ID int,
								   IsSupported bit,
								   CultureName nvarchar(5) NOT NULL,
								   Description nvarchar(50) NOT NULL,

                                   Created_By nvarchar(100),
                                   Created_Date datetime,
                                   Updated_By nvarchar(100),
                                   Updated_Date datetime,
                                   rowguid uniqueidentifier NOT NULL,

								   PRIMARY KEY( Culture_ID )
								  );

								  INSERT INTO AH_MASTER_SUPPORTED_CULTURES
								  VALUES 
									(3081,1, 'en-AU', 'English-Australia', GETDATE(),GETDATE(),GETDATE(),GETDATE(),NEWID()),
									(1046,1, 'pt-BR', 'Portugese-Brazil', GETDATE(),GETDATE(),GETDATE(),GETDATE(),NEWID()),
									(1033,1, 'en-US', 'English-America', GETDATE(),GETDATE(),GETDATE(),GETDATE(),NEWID()),
									(1031,1, 'de-DE', 'deutch-Deutch', GETDATE(),GETDATE(),GETDATE(),GETDATE(),NEWID()),
									(1036,1, 'fr-FR', 'French-France', GETDATE(),GETDATE(),GETDATE(),GETDATE(),NEWID());
							--Add more culture here
							  END;

							
							IF OBJECT_ID('dbo.AH_MASTER_OUID_CULTURE_XREF') IS NULL
							  BEGIN
								  CREATE TABLE AH_MASTER_OUID_CULTURE_XREF(
								   OU_Culture_ID int NOT NULL IDENTITY(1,1),
								   OUID int,
								   Culture_ID int NOT NULL,
								   DefaultCulture bit NOT NULL,
								   OU_CULTURE_KEY int NOT NULL,
								   
                                   Created_By nvarchar(100),
                                   Created_Date datetime,
                                   Updated_By nvarchar(100),
                                   Updated_Date datetime,
                                   rowguid uniqueidentifier NOT NULL,

                                   PRIMARY KEY(OU_Culture_ID)
								  );

								  INSERT INTO AH_MASTER_OUID_CULTURE_XREF
								  VALUES 
									(200, 1046, 1, 5555, GETDATE(),GETDATE(),GETDATE(),GETDATE(),NEWID()),
									(201, 1046, 1, 5556, GETDATE(),GETDATE(),GETDATE(),GETDATE(),NEWID()),
									(202, 3081, 1, 444, GETDATE(),GETDATE(),GETDATE(),GETDATE(),NEWID()),
									(203, 3081, 1, 444, GETDATE(),GETDATE(),GETDATE(),GETDATE(),NEWID());
							--Add more culture here
							  END;


							IF OBJECT_ID('dbo.AH_MASTER_CULTURE_KEY') IS NULL
							  BEGIN
								  CREATE TABLE AH_MASTER_CULTURE_KEY(
								   OU_CULTURE_KEY int,
								   Description nvarchar(150) NOT NULL,

                                   Created_By nvarchar(100),
                                   Created_Date datetime,
                                   Updated_By nvarchar(100),
                                   Updated_Date datetime,
                                   rowguid uniqueidentifier NOT NULL,

								   PRIMARY KEY(OU_CULTURE_KEY)
								  );

								  INSERT INTO AH_MASTER_CULTURE_KEY
								  VALUES 
									(5555, 'Imperial', GETDATE(),GETDATE(),GETDATE(),GETDATE(),NEWID()),
									(5556, 'Metric', GETDATE(),GETDATE(),GETDATE(),GETDATE(),NEWID()),
									(444, 'Metric', GETDATE(),GETDATE(),GETDATE(),GETDATE(),NEWID());
							--Add more culture here
							  END;";

                try
                {
                    var command = new SqlCommand(query) { Connection = connection };
                    command.ExecuteNonQuery();
                }
                catch { }
            }
        }

        private void Init()
        {
            SqlItemManager.Init();
            _tableItems = new List<TableItem>();

            _classItems = ImportExportExcelHelper.Instance.GetClassDataFromExcel(this.tbSPFilePath.Text);
            ImportExportExcelHelper.Instance.GetTableDataFromExcel(this.tbSPFilePath.Text);
            _ignoredItems = ImportExportExcelHelper.Instance.GetIgnoredListFromExcel(this.tbSPFilePath.Text);

            var connection = ConnectionManager.GetConnection();
            if (connection != null)
            {
                var query = @"  SELECT DISTINCT TABLE_NAME 
                                FROM INFORMATION_SCHEMA.TABLES 
                                WHERE TABLE_TYPE = 'BASE TABLE' 
                                    AND TABLE_NAME NOT LIKE '%_LOCALIZED'
                                    AND (TABLE_NAME LIKE 'AH_MASTER%' OR TABLE_NAME = 'AH_MEMBER_WELLNESS_TIPS')   ";

                try
                {
                    var command = new SqlCommand(query) { Connection = connection };
                    var dataAdapter = new SqlDataAdapter(command);
                    var dataTable = new DataTable();

                    dataAdapter.Fill(dataTable);
                    foreach (DataRow row in dataTable.Rows)
                    {
                        var tableName = row["TABLE_NAME"].ToString();
                        if (SqlItemManager.GetRegisteredItem(tableName) != null)
                        {
                            _tableItems.Add(SqlItemManager.GetRegisteredItem(tableName) as TableItem);
                        }
                        else
                        {
                            _tableItems.Add(new TableItem(tableName));
                        }
                    }
                }
                catch { }
            }
        }

        private void UpdateReferences()
        {
            foreach (var classItem in _classItems)
            {
                foreach (var programItem in classItem.ProgramItems)
                {
                    programItem.UpdateReferences();
                }
            }

            foreach (var tableItem in _tableItems)
            {
                if (tableItem.IsReferenced)
                {
                    tableItem.UpdateReferences();
                }
            }
        }

        private void ExportConfirmTableSpec()
        {
            var confirmList = new List<TableItem>();
            foreach (var tableItem in _tableItems)
            {
                if (tableItem.IsReferenced)
                {
                    confirmList.Add(tableItem);
                }
            }

            // Export confirmList.
            ImportExportExcelHelper.Instance.ExportDataToExcel(_folderPath, confirmList);
        }

        private void ExportSpec()
        {
            ImportExportExcelHelper.Instance.ExportDataToExcel(Application.StartupPath, _classItems);
        }

        private void FormSpecHelper_FormClosing(object sender, FormClosingEventArgs e)
        {
            ConnectionManager.CloseConnection();
            Properties.Settings.Default.SPPath = this.tbSPFilePath.Text;
            Properties.Settings.Default.TablePath = this.tbSPFilePath.Text;
            Properties.Settings.Default.Save();
        }

        private void btBrowseSPFilePath_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.tbSPFilePath.Text = openFileDialog1.FileName;
                _folderPath = Path.GetDirectoryName(tbSPFilePath.Text);
                FileName = Path.GetFileName(tbSPFilePath.Text);
            }
        }

        private void btExportToExcel_Click(object sender, EventArgs e)
        {
            Init();
            UpdateReferences();
            Display();
            ExportConfirmTableSpec();
            ExportSpec();
        }

        private void btUpdateSchema_Click(object sender, EventArgs e)
        {
            Init();
            UpdateReferences();
            var resultForm = new UpdateProgramResult();

            foreach (var tableItem in _tableItems)
            {
                tableItem.UpdateSchema();
            }

            foreach (var programItem in SqlItemManager.ProgramItems)
            {
                if (_ignoredItems.Any(p => programItem.Name.IndexOf(p, StringComparison.InvariantCultureIgnoreCase) >= 0))
                {
                    resultForm.AddToList(programItem.Name, Status.NoChange);
                    continue; 
                }
                
                var result = programItem.UpdateSchema();
                resultForm.AddToList(programItem.Name, result);
            }

            Display();
            resultForm.Show();
        }

        private void Display()
        {
            tvTable.Nodes.Clear();
            tvProgramItem.Nodes.Clear();

            foreach (var tableItem in _tableItems)
            {
                if (tableItem.IsReferenced
                    && tableItem.IsBeingLocalized)
                {
                    TreeNode rootNode = new TreeNode(tableItem.Name);
                    foreach (var columnItem in tableItem.Columns)
                    {
                        if (columnItem.IsLocalized)
                        {
                            TreeNode subNode = new TreeNode(columnItem.Name);
                            rootNode.Nodes.Add(subNode);
                        }
                    }

                    tvTable.Nodes.Add(rootNode);
                }
            }

            foreach (var programItem in SqlItemManager.ProgramItems)
            {
                TreeNode rootNode = new TreeNode(programItem.Name);

                tvProgramItem.Nodes.Add(rootNode);
            }
        }

        private void tvProgramItem_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var item = SqlItemManager.GetRegisteredItem(e.Node.Text) as ProgramItem;
            if (item != null)
            {
                rtbStoredProcedureContent.Text = item.Content;
            }
        }

        private void btSetConnection_Click(object sender, EventArgs e)
        {
            new FormSetupDatabase().ShowDialog();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            foreach (var programItem in SqlItemManager.ProgramItems)
            {
                if (programItem.Name.IndexOf(textBox1.Text) != -1)
                {
                    rtbStoredProcedureContent.Text = programItem.Content;
                }
            }
        }
    }
}
