// -----------------------------------------------------------------------
// <copyright file="ConnectionManager.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using System.Data;
using System.Data.SqlClient;

namespace SpecHelper
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class ConnectionManager
    {
        private static SqlConnection connection;
        public static SqlConnection GetConnection()
        {
            if (connection == null
                || connection.State != ConnectionState.Open)
            {
                connection = new SqlConnection();
                string connectionString =
                "Data Source={0};Initial Catalog= {1};User Id={2};Password={3}; Persist Security Info={4}; Workstation ID={5}";

                connection.ConnectionString = string.Format(connectionString, Properties.Settings.Default.DataSource,
                                                    Properties.Settings.Default.InitialCatalog,
                                                    Properties.Settings.Default.UserID,
                                                    Properties.Settings.Default.Password,
                                                    Properties.Settings.Default.PersitSecurityInfo,
                                                    Properties.Settings.Default.WorkStationID);

                try
                {
                    connection.Open();
                    if (connection.State != ConnectionState.Open)
                    {
                        connection = null;
                    }
                }
                catch
                {
                    connection = null;
                }
            }

            return connection;
        }

        public static void CloseConnection()
        {
            if (connection != null)
            {
                try
                {
                    connection.Close();
                    connection = null;
                }
                catch
                {
                    connection = null;
                }
            }
        }
    }
}
