// -----------------------------------------------------------------------
// <copyright file="TableItem.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using System.Data;
using System.Data.SqlClient;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

namespace SpecHelper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class TableItem : SqlItem
    {
        private string _localizedPostFix = "_LOCALIZED";

        private readonly List<ColumnItem> _columns = new List<ColumnItem>();
        public List<ColumnItem> Columns
        {
            get { return _columns; }
        }

        public bool IsLocalized { get; set; }
        public bool IsReferenced { get; set; }
        public bool IsBeingLocalized { get; set; }

        public TableItem(string itemName)
            : base(itemName)
        { }

        public override void UpdateReferences()
        {
            var connection = ConnectionManager.GetConnection();

            if (connection != null)
            {
                // Get table localized status
                try
                {
                    var command = new SqlCommand();
                    command.Connection = connection;
                    command.CommandText = string.Format(@"  SELECT TABLE_NAME 
                                                            FROM INFORMATION_SCHEMA.TABLES 
                                                            WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_NAME = '{0}_LOCALIZED'", Name);

                    var dataTable = new DataTable();
                    var dataAdapter = new SqlDataAdapter(command);
                    dataAdapter.Fill(dataTable);

                    if (dataTable.Rows.Count > 0)
                    {
                        IsLocalized = true;
                    }
                }
                catch { }

                // Add columns
                try
                {
                    var command = new SqlCommand();
                    command.Connection = connection;
                    command.CommandText = string.Format(@"	SELECT COLUMN_NAME, DATA_TYPE
																		FROM INFORMATION_SCHEMA.COLUMNS 
																		WHERE TABLE_NAME = '{0}'", this.Name);

                    var dataTable = new DataTable();
                    var dataAdapter = new SqlDataAdapter(command);
                    dataAdapter.Fill(dataTable);

                    foreach (DataRow row in dataTable.Rows)
                    {
                        var columnName = row[0].ToString();
                        var dataType = row[1].ToString();
                        var column = GetColumn(columnName);

                        if (column != null)
                        {
                            if (dataType.Equals("NVarchar", StringComparison.InvariantCultureIgnoreCase))
                            {
                                column.DataType = DataTypes.NVarchar;
                            }
                        }
                        else
                        {
                            Columns.Add(new ColumnItem(columnName)
                                            {
                                                DataType = dataType.Equals("NVarchar", StringComparison.InvariantCultureIgnoreCase) ? DataTypes.NVarchar : DataTypes.Undefined
                                            });
                        }
                    }
                }
                catch { }

                // Get PK columns
                try
                {
                    var command = new SqlCommand();
                    command.Connection = connection;
                    command.CommandText = string.Format(@"	SELECT COLUMN_NAME
																		FROM INFORMATION_SCHEMA.COLUMNS 
																		WHERE TABLE_NAME = '{0}'

																		INTERSECT

																		SELECT COLUMN_NAME
																		FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS [TC]
																		JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE [KU] ON TC.CONSTRAINT_NAME = KU.CONSTRAINT_NAME
                                                                            AND TC.CONSTRAINT_TYPE = 'PRIMARY KEY'
																			AND KU.TABLE_NAME = '{0}'", this.Name);

                    var dataTable = new DataTable();
                    var dataAdapter = new SqlDataAdapter(command);
                    dataAdapter.Fill(dataTable);

                    foreach (DataRow row in dataTable.Rows)
                    {
                        var columnName = row[0].ToString();

                        var column = GetColumn(columnName);

                        if (column != null)
                        {
                            column.IsPK = true;
                        }
                    }
                }
                catch { }

                // Get Localized columns
                if (this.IsLocalized)
                {
                    // Get all columns of _LOCALIZED table except Foreign key and Primary key
                    var command = new SqlCommand();
                    command.Connection = connection;
                    command.CommandText = string.Format(@"	SELECT COLUMN_NAME
																		FROM INFORMATION_SCHEMA.COLUMNS 
																		WHERE TABLE_NAME = '{0}_LOCALIZED'

																		EXCEPT

																		SELECT COLUMN_NAME
																		FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS [TC]
																		JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE [KU] ON TC.CONSTRAINT_NAME = KU.CONSTRAINT_NAME
                                                                            AND (tc.CONSTRAINT_TYPE = 'PRIMARY KEY' OR tc.CONSTRAINT_TYPE = 'FOREIGN KEY')
																			AND KU.TABLE_NAME = '{0}_LOCALIZED'", this.Name);

                    var dataTable = new DataTable();
                    var dataAdapter = new SqlDataAdapter(command);
                    dataAdapter.Fill(dataTable);

                    foreach (DataRow row in dataTable.Rows)
                    {
                        var columnName = row[0].ToString();

                        var column = GetColumn(columnName);

                        if (column != null)
                        {
                            column.IsLocalized = true;
                        }
                    }
                }
            }
        }

        public override Status UpdateSchema()
        {
            if (this.IsLocalized == false && this.IsBeingLocalized == true)
            {
                string content = null;
                var localizedTableName = this.Name + _localizedPostFix;

                content +=
                string.Format(@"
			    --Identify the database which will be used
			    USE [{0}]
			    GO", Properties.Settings.Default.InitialCatalog);

                // Create new localized table
                content +=
                string.Format(@"--Create new localized table, with primary key Question_ID,
			    --culture code Culture_ID, and all other localized columns
			    IF OBJECT_ID('dbo.{0}') IS NULL
			        BEGIN
			         SELECT * INTO {0} FROM (SELECT DISTINCT ", localizedTableName);

                foreach (var columnItem in Columns)
                {
                    if (columnItem.IsLocalized)
                    {
                        content += string.Format("sc.CultureName + CAST(localizedTable.[{0}] AS NVARCHAR(MAX)) as [{0}], ", columnItem.Name);
                    }

                    if (columnItem.IsPK)
                    {
                        content += string.Format("localizedTable.[{0}] as [{0}], ", columnItem.Name);
                    }
                }

                content +=
                    string.Format(
                     @" ouidCultureX.[OU_CULTURE_KEY] 
			         FROM [{1}].[dbo].[{0}] localizedTable CROSS JOIN  [{1}].[dbo].[AH_MASTER_CULTURE_KEY] ouidCultureX 
                        LEFT JOIN 
                        [AH_MASTER_OUID_CULTURE_XREF] xref 
                            ON ouidCultureX.OU_CULTURE_KEY = xref.OU_CULTURE_KEY
                            INNER JOIN AH_Master_Supported_cultures sc 
                            ON xref.Culture_ID = sc.Culture_ID) crossedTable ", this.Name, Properties.Settings.Default.InitialCatalog);

                var orignialPK = string.Empty;

                foreach (var columnItem in Columns)
                {
                    if (columnItem.IsPK)
                    {
                        orignialPK += ", " + columnItem.Name;
                        content += string.Format(@"	ALTER TABLE {0} ALTER COLUMN {1} INTEGER NOT NULL", localizedTableName, columnItem.Name);
                    }
                }
                orignialPK = orignialPK.TrimStart(',');

                content += string.Format(@"	ALTER TABLE {0} ALTER COLUMN OU_CULTURE_KEY INTEGER NOT NULL", localizedTableName);

                // Add PK and FK
                content +=
                string.Format(@"
                --Add column Culture_ID into the localized table
			    --IF NOT EXISTS (SELECT * FROM sys.columns
			    --           WHERE Name = N'OU_CULTURE_KEY' and Object_ID = Object_ID(N'dbo.{0}'))			
			    --    ALTER TABLE [dbo].[{0}]
				    --Add default Culture_ID ‘en-AU’
			    --    ADD OU_CULTURE_KEY int NOT NULL DEFAULT '444' --Add default OU_CULTURE_KEY ‘en-AU’
		
			    --Add primary key
			    IF OBJECT_ID('dbo.PK_{0}') IS NULL
				    ALTER TABLE {0}
				    ADD CONSTRAINT PK_{0} PRIMARY KEY ({1}, OU_CULTURE_KEY)

			    --Add foreign key to original table
				    ALTER TABLE {0}
				    ADD CONSTRAINT FK_{0}_ORIGIN FOREIGN KEY ({1}) REFERENCES  {2}({1})
 
			    --Add foreign key to culture table AH_MASTER_CULTURE_KEY
				    ALTER TABLE {0}
				    ADD CONSTRAINT FK_{0}_CULTURE FOREIGN KEY (OU_CULTURE_KEY) REFERENCES AH_MASTER_CULTURE_KEY(OU_CULTURE_KEY)
				    END", localizedTableName, orignialPK, this.Name);

                var connection = ConnectionManager.GetConnection();

                if (connection != null)
                {
                    try
                    {
                        var server = new Server(new ServerConnection(connection));
                        server.ConnectionContext.ExecuteNonQuery(content);
                        IsLocalized = true;
                        return Status.Success;
                    }
                    catch
                    {
                        return Status.Fail;
                    }
                }
            }

            return Status.Fail;
        }

        public bool ContainColumn(string columnName)
        {
            foreach (var item in Columns)
            {
                if (item.Name.Equals(columnName, StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        public ColumnItem GetColumn(string columnName)
        {
            foreach (var item in Columns)
            {
                if (item.Name.Equals(columnName, StringComparison.InvariantCultureIgnoreCase))
                {
                    return item;
                }
            }
            return null;
        }
    }
}
