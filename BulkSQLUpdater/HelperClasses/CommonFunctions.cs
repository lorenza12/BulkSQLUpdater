using BulkSQLUpdater;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;

namespace HelperClasses.CommonFunctions
{
    public class CommonFunctions
    {
        public static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static DataTable ConvertToDataTable(List<string> columnNames, List<List<string>> columnValues)
        {
            log.Debug("Converting to DataTable");

            try
            {
                DataTable dt = new DataTable();

                foreach (string column in columnNames)
                {
                    dt.Columns.Add(column);
                }

                foreach (List<string> listItem in columnValues)
                {
                    var row = dt.NewRow();

                    for (int valueCounter = 0; valueCounter < listItem.Count; valueCounter++)
                    {
                        row[columnNames[valueCounter].ToString()] = listItem[valueCounter];
                    }

                    dt.Rows.Add(row);

                }

                return dt;

            }
            catch (Exception ex)
            {

                StringBuilder sbInParams = new StringBuilder();
                sbInParams.Append("<Method>ConvertToDataTable</Method>");
                log.Error(sbInParams, ex);

            }

            return null;
        }

        public static string CreateTempTableString(List<TableColumn> columns)
        {
            log.Debug("Creating Temp Table String");

            try
            {
                string tempTableString = "CREATE TABLE #TmpTable(";


                for (int i = 0; i < columns.Count; i++)
                {
                    string columnName = columns[i].ColumnName;
                    string dataType = columns[i].DataType;

                    tempTableString += $"{columnName} {dataType},";
                }

                //Remove last comma
                tempTableString = tempTableString.TrimEnd(',') + ")";

                return tempTableString;

            }
            catch (Exception ex)
            {

                StringBuilder sbInParams = new StringBuilder();
                sbInParams.Append("<Method>CreateTempTableString</Method>");
                log.Error(sbInParams, ex);

            }

            return string.Empty;
        }

        public static string CreateUpdateTableString(List<TableColumn> columns, string tableName)
        {
            log.Debug("Creating Update Table String");

            try
            {
                string updateTableString = $"UPDATE {tableName} SET ";

                for (int i = 0; i < columns.Count; i++)
                {
                    //Skip the primary key as we wont need to add update statement for that
                    if (i != 0)
                    {
                        string columnName = columns[i].ColumnName;

                        updateTableString += $"{columnName} = tmp.{columnName},";
                    }
                }

                //Remove last comma
                updateTableString = updateTableString.TrimEnd(',');
                updateTableString += $" FROM {tableName} main  INNER JOIN #TmpTable tmp ON tmp.{columns[0].ColumnName} = main.{columns[0].ColumnName}; DROP TABLE #TmpTable;";

                return updateTableString;

            }
            catch (Exception ex)
            {

                StringBuilder sbInParams = new StringBuilder();
                sbInParams.Append("<Method>CreateTempTableString</Method>");
                log.Error(sbInParams, ex);

            }

            return string.Empty;
        }

        public static string CreateConnectionString(string server, string database, string user, string password)
        {
            log.Debug("Creating Connection String");

            SqlConnectionStringBuilder connectionString = new SqlConnectionStringBuilder();
            try
            {
                //connectionString = $"Data Source={server};Initial Catalog={database};Persist Security Info=True;User ID={user};Password={password}";
                connectionString.DataSource = server;
                connectionString.InitialCatalog = database;

                if (!string.IsNullOrEmpty(user))
                    connectionString.UserID = user;

                if (!string.IsNullOrEmpty(password))
                    connectionString.Password = password;
            }
            catch (Exception ex)
            {
                StringBuilder sbInParams = new StringBuilder();
                sbInParams.Append("<Method>CreateConnectionString</Method>");
                sbInParams.Append($"<Server>{server}</Server>");
                sbInParams.Append($"<Database>{database}</Database>");
                sbInParams.Append($"<User>{user}</User>");
                log.Error(sbInParams, ex);
            }
            return connectionString.ConnectionString;
        }

        public static DataTable ConvertCSVToDataTable(string csvFilePath, char delimiter, bool hasHeader)
        {
            log.Debug("Converting CSV to DataTable");

            bool createHeaders = false;
            DataTable dt = new DataTable();

            try
            {
                using (StreamReader sr = new StreamReader(csvFilePath))
                {
                    if (hasHeader)
                    {
                        string[] headers = sr.ReadLine().Split(delimiter);
                        foreach (string header in headers)
                        {
                            dt.Columns.Add(header);
                        }
                    }
                    else
                    {
                        createHeaders = true;
                    }

                    while (!sr.EndOfStream)
                    {
                        string[] rows = sr.ReadLine().Split(delimiter);
                        DataRow dr = dt.NewRow();
                        for (int i = 0; i < rows.Length; i++)
                        {
                            if (createHeaders)
                            {
                                dt.Columns.Add(i.ToString()); //If file doesnt have headers create them manually
                            }
                            dr[i] = rows[i];
                        }
                        dt.Rows.Add(dr);

                        createHeaders = false;
                    }

                }
            }
            catch (Exception ex)
            {
                StringBuilder sbInParams = new StringBuilder();
                sbInParams.Append("<Method>ConvertCSVToDataTable</Method>");
                sbInParams.Append($"<CsvFilePath>{csvFilePath}</CsvFilePath>");
                sbInParams.Append($"<Delimiter>{delimiter}</Delimiter>");
                sbInParams.Append($"<HasHeader>{hasHeader}</HasHeader>");
                log.Error(sbInParams, ex);
            }

            return dt;
        }

        public static DataTable UpdateDataTableColumnNames(DataTable dataTable, List<TableColumn> columnNames)
        {
            log.Debug("Updating DataTable Column Names");

            try
            {
                for (int i = 0; i < columnNames.Count; i++)
                {
                    dataTable.Columns[i].ColumnName = columnNames[i].ColumnName.ToString();
                }

            }
            catch (Exception ex)
            {
                StringBuilder sbInParams = new StringBuilder();
                sbInParams.Append("<Method>UpdateDataTableColumnNames</Method>");
                log.Error(sbInParams, ex);

                //So the insert does a hard stop
                throw new DataException("Error updating column names");
            }

            return dataTable;
        }
    }
}
