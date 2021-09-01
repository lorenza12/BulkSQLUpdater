using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace HelperClasses.DatabaseFunctions
{
    public class DatabaseFunctions
    {
        public static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private const string QUERYPATH = @"Querries";

        public static DataTable GetTables(string connectionString)
        {
            log.Debug($"Getting Database Tables - {connectionString}");

            DataTable tablesDT = new DataTable();

            try
            {
                string query = LoadQuery("TableLookup.txt");

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("", conn))
                    {
                        try
                        {
                            conn.Open();
                            command.CommandTimeout = 300;
                            command.CommandText = query;
                            SqlDataAdapter adapter = new SqlDataAdapter(command);
                            adapter.Fill(tablesDT);

                        }
                        catch (Exception ex)
                        {
                            StringBuilder sbInParams = new StringBuilder();
                            sbInParams.Append("<Method>UpdateData</Method>");
                            sbInParams.Append($"<ConnectionString>{connectionString}</ConnectionString>");
                            log.Error(sbInParams, ex);

                        }
                        finally
                        {
                            conn.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                StringBuilder sbInParams = new StringBuilder();
                sbInParams.Append("<Method>UpdateData</Method>");
                sbInParams.Append($"<ConnectionString>{connectionString}</ConnectionString>");
                log.Error(sbInParams, ex);
            }
            return tablesDT;
        }

        public static DataTable GetTableColumns(string connectionString, string tableName)
        {
            log.Debug($"Getting Table Columns From: {tableName}");

            DataTable columnsDT = new DataTable();

            try
            {
                string query = LoadQuery("TableColumnsLookup.txt");
                query = query.Replace("#TABLENAME#", tableName);

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("", conn))
                    {
                        try
                        {

                            conn.Open();
                            command.CommandTimeout = 300;
                            command.CommandText = query;
                            SqlDataAdapter adapter = new SqlDataAdapter(command);
                            adapter.Fill(columnsDT);

                        }
                        catch (Exception ex)
                        {
                            StringBuilder sbInParams = new StringBuilder();
                            sbInParams.Append("<Method>UpdateData</Method>");
                            sbInParams.Append($"<ConnectionString>{connectionString}</ConnectionString>");
                            sbInParams.Append($"<TableName>{tableName}</TableName>");
                            log.Error(sbInParams, ex);

                        }
                        finally
                        {
                            conn.Close();
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                StringBuilder sbInParams = new StringBuilder();
                sbInParams.Append("<Method>UpdateData</Method>");
                sbInParams.Append($"<ConnectionString>{connectionString}</ConnectionString>");
                sbInParams.Append($"<TableName>{tableName}</TableName>");
                log.Error(sbInParams, ex);
            }

            return columnsDT;
        }

        public static bool UpdateData(string connectionString, string tempTableCommand, string updateCommand, DataTable updateData)
        {
            log.Debug($"Updating Database - connecntion: {connectionString}, tempTable: {tempTableCommand}, updateCommand: {updateCommand}");

            if (!string.IsNullOrEmpty(connectionString) && !string.IsNullOrEmpty(tempTableCommand) && !string.IsNullOrEmpty(updateCommand) && updateData.Rows.Count > 0)
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("", conn))
                    {
                        try
                        {
                            conn.Open();

                            //Creating temp table on database
                            command.CommandText = tempTableCommand;
                            command.ExecuteNonQuery();

                            //Bulk insert into temp table
                            using (SqlBulkCopy bulkcopy = new SqlBulkCopy(conn))
                            {
                                bulkcopy.BulkCopyTimeout = 660;
                                bulkcopy.DestinationTableName = "#TmpTable";

                                bulkcopy.WriteToServer(updateData);
                                bulkcopy.Close();
                            }

                            // Updating destination table, and dropping temp table
                            command.CommandTimeout = 300;
                            command.CommandText = updateCommand;
                            command.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                            StringBuilder sbInParams = new StringBuilder();
                            sbInParams.Append("<Method>UpdateData</Method>");
                            sbInParams.Append($"<UpdateCommand>{updateCommand}</UpdateCommand>");
                            sbInParams.Append($"<TempTableCommand>{tempTableCommand}</TempTableCommand>");
                            log.Error(sbInParams, ex);

                            return false;
                        }
                        finally
                        {
                            conn.Close();
                        }

                        return true;
                    }
                }
            }
            else
            {
                return false;
            }
        }

        public static bool BulkInsert(string connectionString, string destinationtable, DataTable insertData)
        {
            log.Debug($"Bulk Insert - connecntion: {connectionString}");

            if (!string.IsNullOrEmpty(connectionString) && insertData.Rows.Count > 0)
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        SqlBulkCopy bulkCopy = new SqlBulkCopy(connectionString);

                        bulkCopy.DestinationTableName = destinationtable;
                        conn.Open();

                        bulkCopy.WriteToServer(insertData);
                        conn.Close();
                    }
                }
                catch (Exception ex)
                {
                    StringBuilder sbInParams = new StringBuilder();
                    sbInParams.Append("<Method>BulkInsert</Method>");
                    sbInParams.Append($"<ConnectionString>{connectionString}</ConnectionString>");
                    sbInParams.Append($"<Destinationtable>{destinationtable}</Destinationtable>");
                    log.Error(sbInParams, ex);

                    return false;
                }
            }
            else
            {
                return false;
            }

            return true;
        }

        private static string LoadQuery(string queryFile)
        {
            log.Debug($"Loading Query: {queryFile}");

            string lines = string.Empty;

            try
            {
                string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), $@"{QUERYPATH}\{queryFile}");

                using (StreamReader sr = new StreamReader(path))
                {
                    lines = sr.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                StringBuilder sbInParams = new StringBuilder();
                sbInParams.Append("<Method>UpdateData</Method>");
                sbInParams.Append($"<QueryFile>{queryFile}</QueryFile>");
                log.Error(sbInParams, ex);
            }
            return lines;
        }
    }
}
