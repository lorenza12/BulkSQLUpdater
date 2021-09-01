using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using HelperClasses.CommonFunctions;
using HelperClasses.DatabaseFunctions;
using Microsoft.Win32;

namespace BulkSQLUpdater
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public string connectionString;
        public enum BulkOperation { Update, Insert };

        public MainWindow()
        {
            InitializeComponent();

        }

        #region button events
        private void testConnection_btn_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                this.Dispatcher.Invoke(() =>
                {
                    columns_lstbox.ItemsSource = null;
                    columns_lstbox.Items.Clear();

                    columnsSelected_lstbox.ItemsSource = null;
                    columnsSelected_lstbox.Items.Clear();
                });

                string server = server_txtbox.Text.Trim();
                string database = database_txtbox.Text.Trim();
                string user = username_txtbox.Text.Trim();
                string password = password_txtbox.Text.Trim();

                TestConnection(server, database, user, password, BulkOperation.Update);
            }
            catch (Exception ex)
            {
                StringBuilder sbInParams = new StringBuilder();
                sbInParams.Append("<Method>testConnection_btn_Click</Method>");
                log.Error(sbInParams, ex);
            }
        }

        private void testConnectionBulkInsert_btn_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                this.Dispatcher.Invoke(() =>
                {
                    columnsBulkInsert_lstbox.ItemsSource = null;
                    columnsBulkInsert_lstbox.Items.Clear();

                    columnsSelectedBulkInsert_lstbox.ItemsSource = null;
                    columnsSelectedBulkInsert_lstbox.Items.Clear();
                });

                string server = serverBulkInsert_txtbox.Text.Trim();
                string database = databaseBulkInsert_txtbox.Text.Trim();
                string user = usernameBulkInsert_txtbox.Text.Trim();
                string password = passwordBulkInsert_txtbox.Text.Trim();

                TestConnection(server, database, user, password, BulkOperation.Insert);
            }
            catch (Exception ex)
            {
                StringBuilder sbInParams = new StringBuilder();
                sbInParams.Append("<Method>testConnectionBulkInsert_btn_Click</Method>");
                log.Error(sbInParams, ex);
            }
        }


        private async void selectTable_btn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string table = tables_cmbobox.SelectedItem.ToString();

                ShowLoadingBar(BulkOperation.Update);

                await Task.Run(() =>
                {
                    GetTableColumns(table, BulkOperation.Update);

                });

                HideLoadingBar(BulkOperation.Update);

            }
            catch (Exception ex)
            {
                StringBuilder sbInParams = new StringBuilder();
                sbInParams.Append("<Method>testConnection_btn_Click</Method>");
                log.Error(sbInParams, ex);
            }

        }

        private void addColumn_btn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TableColumn column = (TableColumn)columns_lstbox.SelectedItem;
                if (column != null)
                {
                    columnsSelected_lstbox.Items.Add(column);

                    columns_lstbox.Items.RemoveAt(columns_lstbox.Items.IndexOf(columns_lstbox.SelectedItem));
                }
            }
            catch (Exception ex)
            {
                StringBuilder sbInParams = new StringBuilder();
                sbInParams.Append("<Method>addColumn_btn_Click</Method>");
                log.Error(sbInParams, ex);
            }
        }

        private void removeColumn_btn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TableColumn column = (TableColumn)columnsSelected_lstbox.SelectedItem;
                if (column != null)
                {
                    columns_lstbox.Items.Add(column);
                    columnsSelected_lstbox.Items.RemoveAt(columnsSelected_lstbox.Items.IndexOf(columnsSelected_lstbox.SelectedItem));
                }
            }
            catch (Exception ex)
            {
                StringBuilder sbInParams = new StringBuilder();
                sbInParams.Append("<Method>removeColumn_btn_Click</Method>");
                log.Error(sbInParams, ex);
            }
        }

        private void browse_btn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BrowseForCSV(BulkOperation.Update);
            }
            catch (Exception ex)
            {
                StringBuilder sbInParams = new StringBuilder();
                sbInParams.Append("<Method>browse_btn_Click</Method>");
                log.Error(sbInParams, ex);
            }
        }

        private async void update_btn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await Task.Run(() =>
                {
                    RunUpdate();
                });

            }
            catch (Exception ex)
            {
                StringBuilder sbInParams = new StringBuilder();
                sbInParams.Append("<Method>update_btn_Click</Method>");
                log.Error(sbInParams, ex);
            }
        }

        private void tables_cmbobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                this.Dispatcher.Invoke(() =>
                {
                    columns_lstbox.ItemsSource = null;
                    columns_lstbox.Items.Clear();

                    columnsSelected_lstbox.ItemsSource = null;
                    columnsSelected_lstbox.Items.Clear();

                    addColumn_btn.IsEnabled = false;
                    removeColumn_btn.IsEnabled = false;
                    status_txtblock.Visibility = Visibility.Hidden;
                });

            }
            catch (Exception ex)
            {
                StringBuilder sbInParams = new StringBuilder();
                sbInParams.Append("<Method>tables_cmbobox_SelectionChanged</Method>");
                log.Error(sbInParams, ex);
            }
        }

        private void tablesBulkInsert_cmbobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                this.Dispatcher.Invoke(() =>
                {
                    columnsBulkInsert_lstbox.ItemsSource = null;
                    columnsBulkInsert_lstbox.Items.Clear();

                    columnsSelectedBulkInsert_lstbox.ItemsSource = null;
                    columnsSelectedBulkInsert_lstbox.Items.Clear();

                    addColumnBulkInsert_btn.IsEnabled = false;
                    removeColumnBulkInsert_btn.IsEnabled = false;
                    statusBulkInsert_txtblock.Visibility = Visibility.Hidden;
                });

            }
            catch (Exception ex)
            {
                StringBuilder sbInParams = new StringBuilder();
                sbInParams.Append("<Method>tablesBulkInsert_cmbobox_SelectionChanged</Method>");
                log.Error(sbInParams, ex);
            }
        }



        private async void selectTableBulkInsert_btn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string table = tablesBulkInsert_cmbobox.SelectedItem.ToString();

                ShowLoadingBar(BulkOperation.Insert);

                await Task.Run(() =>
                {
                    GetTableColumns(table, BulkOperation.Insert);

                });

                HideLoadingBar(BulkOperation.Insert);

            }
            catch (Exception ex)
            {
                StringBuilder sbInParams = new StringBuilder();
                sbInParams.Append("<Method>selectTableBulkInsert_btn_Click</Method>");
                log.Error(sbInParams, ex);
            }
        }

        private void addColumnBulkInsert_btn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TableColumn column = (TableColumn)columnsBulkInsert_lstbox.SelectedItem;
                if (column != null)
                {
                    columnsSelectedBulkInsert_lstbox.Items.Add(column);

                    columnsBulkInsert_lstbox.Items.RemoveAt(columnsBulkInsert_lstbox.Items.IndexOf(columnsBulkInsert_lstbox.SelectedItem));
                }
            }
            catch (Exception ex)
            {
                StringBuilder sbInParams = new StringBuilder();
                sbInParams.Append("<Method>addColumnBulkInsert_btn_Click</Method>");
                log.Error(sbInParams, ex);
            }
        }

        private void removeColumnBulkInsert_btn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TableColumn column = (TableColumn)columnsSelectedBulkInsert_lstbox.SelectedItem;
                if (column != null)
                {
                    columnsBulkInsert_lstbox.Items.Add(column);
                    columnsSelectedBulkInsert_lstbox.Items.RemoveAt(columnsSelectedBulkInsert_lstbox.Items.IndexOf(columnsSelectedBulkInsert_lstbox.SelectedItem));
                }
            }
            catch (Exception ex)
            {
                StringBuilder sbInParams = new StringBuilder();
                sbInParams.Append("<Method>removeColumnBulkInsert_btn_Click</Method>");
                log.Error(sbInParams, ex);
            }
        }

        private void browseBulkInsert_btn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BrowseForCSV(BulkOperation.Insert);
            }
            catch (Exception ex)
            {
                StringBuilder sbInParams = new StringBuilder();
                sbInParams.Append("<Method>browseBulkInsert_btn_Click</Method>");
                log.Error(sbInParams, ex);
            }
        }

        private async void insert_btn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await Task.Run(() =>
                {
                    RunInsert();
                });

            }
            catch (Exception ex)
            {
                StringBuilder sbInParams = new StringBuilder();
                sbInParams.Append("<Method>updateBulkInsert_btn_Click</Method>");
                log.Error(sbInParams, ex);
            }
        }

        #endregion

        #region methods

        private async void TestConnection(string server, string database, string user, string password, BulkOperation bulkOperation)
        {
            log.Debug($"Testing Connection - server: {server}, database: {database}, user: {user}, passowrd: {password}");

            try
            {
                ShowLoadingBar(bulkOperation);

                connectionString = CommonFunctions.CreateConnectionString(server, database, user, password);

                if (!string.IsNullOrEmpty(connectionString))
                {
                    await Task.Run(() =>
                    {
                        GetDatabaseTable(connectionString, bulkOperation);
                    });
                }

                HideLoadingBar(bulkOperation);
            }
            catch (Exception ex)
            {
                StringBuilder sbInParams = new StringBuilder();
                sbInParams.Append("<Method>TestConnection</Method>");
                log.Error(sbInParams, ex);
            }
        }

        private void GetDatabaseTable(string connectionString, BulkOperation bulkOperation)
        {
            DataTable tables = new DataTable("Tables");
            try
            {
                tables = DatabaseFunctions.GetTables(connectionString);

                if (tables != null && tables.Rows.Count > 0)
                {
                    List<string> tableList = new List<string>();
                    for (int i = 0; i < tables.Rows.Count; i++)
                    {
                        String table = Convert.ToString(tables.Rows[i]["Table"]);
                        tableList.Add(table);
                    }

                    this.Dispatcher.Invoke(() =>
                    {
                        switch (bulkOperation)
                        {
                            case BulkOperation.Insert:
                                tablesBulkInsert_cmbobox.ItemsSource = tableList;
                                break;
                            case BulkOperation.Update:
                                tables_cmbobox.ItemsSource = tableList;
                                break;
                        }
                        EnableControlsOnConnection(bulkOperation);
                    });

                }
                else
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        DisableControlsOnBadConnection(bulkOperation);
                    });

                }
            }
            catch (Exception ex)
            {
                StringBuilder sbInParams = new StringBuilder();
                sbInParams.Append("<Method>GetDatabaseTable</Method>");
                log.Error(sbInParams, ex);

                this.Dispatcher.Invoke(() =>
                {
                    DisableControlsOnBadConnection(bulkOperation);
                });

            }
        }

        private void GetTableColumns(string table, BulkOperation bulkOperation)
        {
            DataTable columns = new DataTable("Columns");
            try
            {
                this.Dispatcher.Invoke(() =>
                {
                    switch (bulkOperation)
                    {
                        case BulkOperation.Insert:
                            columnsBulkInsert_lstbox.ItemsSource = null;
                            columnsBulkInsert_lstbox.Items.Clear();
                            break;
                        case BulkOperation.Update:
                            columns_lstbox.ItemsSource = null;
                            columns_lstbox.Items.Clear();
                            break;
                    }

                });

                columns = DatabaseFunctions.GetTableColumns(connectionString, table);

                if (columns != null && columns.Rows.Count > 0)
                {
                    for (int i = 0; i < columns.Rows.Count; i++)
                    {
                        this.Dispatcher.Invoke(() =>
                        {
                            TableColumn col = new TableColumn(columns.Rows[i][0].ToString(), columns.Rows[i][1].ToString());


                            switch (bulkOperation)
                            {
                                case BulkOperation.Insert:
                                    columnsBulkInsert_lstbox.Items.Add(col);
                                    break;
                                case BulkOperation.Update:
                                    columns_lstbox.Items.Add(col);
                                    break;
                            }

                        });
                    }

                    this.Dispatcher.Invoke(() =>
                    {

                        switch (bulkOperation)
                        {
                            case BulkOperation.Insert:
                                addColumnBulkInsert_btn.IsEnabled = true;
                                removeColumnBulkInsert_btn.IsEnabled = true;
                                break;
                            case BulkOperation.Update:
                                addColumn_btn.IsEnabled = true;
                                removeColumn_btn.IsEnabled = true;
                                break;
                        }

                    });

                }
                else
                {
                    this.Dispatcher.Invoke(() =>
                    {

                        switch (bulkOperation)
                        {
                            case BulkOperation.Insert:
                                addColumnBulkInsert_btn.IsEnabled = false;
                                removeColumnBulkInsert_btn.IsEnabled = false;
                                break;
                            case BulkOperation.Update:
                                addColumn_btn.IsEnabled = false;
                                removeColumn_btn.IsEnabled = false;
                                break;
                        }

                    });
                }
            }
            catch (Exception ex)
            {
                StringBuilder sbInParams = new StringBuilder();
                sbInParams.Append("<Method>GetDatabaseTable</Method>");
                log.Error(sbInParams, ex);
            }
        }

        private void BrowseForCSV(BulkOperation bulkOperation)
        {
            try
            {
                OpenFileDialog fileDialog = new OpenFileDialog();
                fileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                if (fileDialog.ShowDialog() == true)
                {
                    switch (bulkOperation)
                    {
                        case BulkOperation.Insert:
                            csvFilePathBulkInsert_txtbox.Text = fileDialog.FileName;
                            break;
                        case BulkOperation.Update:
                            csvFilePath_txtbox.Text = fileDialog.FileName;
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                StringBuilder sbInParams = new StringBuilder();
                sbInParams.Append("<Method>BrowseForCSV</Method>");
                log.Error(sbInParams, ex);
            }
        }

        private void RunUpdate()
        {
            bool success;
            string message = string.Empty;
            string table = string.Empty;
            string csvFile = string.Empty;
            char delimiter = ',';
            bool hasHeader = false;

            try
            {
                ShowLoadingBar(BulkOperation.Update);

                this.Dispatcher.Invoke(() =>
                {
                    table = ((string)tables_cmbobox.SelectedItem).ToString();
                    csvFile = csvFilePath_txtbox.Text.ToString().Trim();
                    var delimiterTag = ((ComboBoxItem)csvDelimiter_cmbobox.SelectedItem).Tag.ToString();
                    delimiter = delimiterTag == "\\t" ? '\t' : delimiterTag.ToCharArray()[0];
                    hasHeader = (bool)hasHeaders_chckbox.IsChecked;
                });

                List<TableColumn> selectedColumns = new List<TableColumn>();

                foreach (TableColumn item in columnsSelected_lstbox.Items)
                {
                    selectedColumns.Add(item);
                }

                DataTable data = CommonFunctions.ConvertCSVToDataTable(csvFile, delimiter, hasHeader);

                if (data != null && data.Rows.Count > 0)
                {
                    string tempTable = CommonFunctions.CreateTempTableString(selectedColumns);
                    string updateStatement = CommonFunctions.CreateUpdateTableString(selectedColumns, table);

                    success = DatabaseFunctions.UpdateData(connectionString, tempTable, updateStatement, data);
                    message = success ? "Database successully updated" : "There was an issue updating the database";

                }
                else
                {
                    success = false;
                    message = "CSV file could not be loaded correctly - please verify the file is correct.";
                }

            }
            catch (Exception ex)
            {
                StringBuilder sbInParams = new StringBuilder();
                sbInParams.Append("<Method>RunUpdate</Method>");
                log.Error(sbInParams, ex);

                success = false;
                message = "An error occurred. Please verify the table, delimiter, connection string properties, and csv file are all correct.";
            }

            SetStatusText(message, success, BulkOperation.Update);
            HideLoadingBar(BulkOperation.Update);
        }


        private void RunInsert()
        {
            bool success;
            string message = string.Empty;
            string table = string.Empty;
            string csvFile = string.Empty;
            char delimiter = ',';
            bool hasHeader = false;

            try
            {
                ShowLoadingBar(BulkOperation.Insert);

                this.Dispatcher.Invoke(() =>
                {
                    table = ((string)tablesBulkInsert_cmbobox.SelectedItem).ToString();
                    csvFile = csvFilePathBulkInsert_txtbox.Text.ToString().Trim();
                    var delimiterTag = ((ComboBoxItem)csvDelimiterBulkInsert_cmbobox.SelectedItem).Tag.ToString();
                    delimiter = delimiterTag == "\\t" ? '\t' : delimiterTag.ToCharArray()[0];
                    hasHeader = (bool)hasHeadersBulkInsert_chckbox.IsChecked;
                });

                List<TableColumn> selectedColumns = new List<TableColumn>();

                foreach (TableColumn item in columnsSelectedBulkInsert_lstbox.Items)
                {
                    selectedColumns.Add(item);
                }

                DataTable data = CommonFunctions.ConvertCSVToDataTable(csvFile, delimiter, hasHeader);

                if (data != null && data.Rows.Count > 0)
                {

                    //Change DataTable column names to match the columns in the database
                    data = CommonFunctions.UpdateDataTableColumnNames(data, selectedColumns);

                    success = DatabaseFunctions.BulkInsert(connectionString, table, data);
                   
                    message = success ? "Database successully updated" : "There was an issue inserting into the database";

                }
                else
                {
                    success = false;
                    message = "CSV file could not be loaded correctly - please verify the file is correct.";
                }

            }
            catch (Exception ex)
            {
                StringBuilder sbInParams = new StringBuilder();
                sbInParams.Append("<Method>RunInsert</Method>");
                log.Error(sbInParams, ex);

                success = false;
                message = "An error occurred. Please verify the table, delimiter, connection string properties, and csv file are all correct and column counts match.";
            }

            SetStatusText(message, success, BulkOperation.Insert);
            HideLoadingBar(BulkOperation.Insert);
        }


        private void EnableControlsOnConnection(BulkOperation bulkOperation)
        {
            try
            {
                this.Dispatcher.Invoke(() =>
                {
                    switch (bulkOperation)
                    {
                        case BulkOperation.Insert:
                            badConnectionBulkInsert_lbl.Visibility = Visibility.Hidden;

                            tablesBulkInsert_cmbobox.IsEnabled = true;
                            selectTableBulkInsert_btn.IsEnabled = true;

                            statusBulkInsert_txtblock.Visibility = Visibility.Hidden;
                            break;

                        case BulkOperation.Update:
                            badConnection_lbl.Visibility = Visibility.Hidden;

                            tables_cmbobox.IsEnabled = true;
                            selectTable_btn.IsEnabled = true;

                            status_txtblock.Visibility = Visibility.Hidden;
                            break;
                    }

                });

            }
            catch (Exception ex)
            {
                StringBuilder sbInParams = new StringBuilder();
                sbInParams.Append("<Method>DisableControlsOnBadConnection</Method>");
                log.Error(sbInParams, ex);
            }
        }

        private void DisableControlsOnBadConnection(BulkOperation bulkOperation)
        {
            try
            {
                this.Dispatcher.Invoke(() =>
                {

                    switch (bulkOperation)
                    {
                        case BulkOperation.Insert:
                            badConnectionBulkInsert_lbl.Visibility = Visibility.Visible;

                            tablesBulkInsert_cmbobox.IsEnabled = false;
                            selectTableBulkInsert_btn.IsEnabled = false;

                            columnsBulkInsert_lstbox.ItemsSource = null;
                            columnsBulkInsert_lstbox.Items.Clear();

                            columnsSelectedBulkInsert_lstbox.ItemsSource = null;
                            columnsSelectedBulkInsert_lstbox.Items.Clear();

                            addColumnBulkInsert_btn.IsEnabled = false;
                            removeColumnBulkInsert_btn.IsEnabled = false;

                            statusBulkInsert_txtblock.Visibility = Visibility.Hidden;
                            break;

                        case BulkOperation.Update:
                            badConnection_lbl.Visibility = Visibility.Visible;

                            tables_cmbobox.IsEnabled = false;
                            selectTable_btn.IsEnabled = false;

                            columns_lstbox.ItemsSource = null;
                            columns_lstbox.Items.Clear();

                            columnsSelected_lstbox.ItemsSource = null;
                            columnsSelected_lstbox.Items.Clear();

                            addColumn_btn.IsEnabled = false;
                            removeColumn_btn.IsEnabled = false;

                            status_txtblock.Visibility = Visibility.Hidden;
                            break;
                    }



                });
            }
            catch (Exception ex)
            {
                StringBuilder sbInParams = new StringBuilder();
                sbInParams.Append("<Method>DisableControlsOnBadConnection</Method>");
                log.Error(sbInParams, ex);
            }
        }

        private void SetStatusText(string text, bool sucess, BulkOperation bulkOperation)
        {
            try
            {
                this.Dispatcher.Invoke(() =>
                {

                    switch (bulkOperation)
                    {
                        case BulkOperation.Insert:
                            statusBulkInsert_txtblock.Visibility = Visibility.Visible;
                            statusBulkInsert_txtblock.Text = text;
                            statusBulkInsert_txtblock.Foreground = sucess ? Brushes.Green : Brushes.Red;
                            break;
                        case BulkOperation.Update:
                            status_txtblock.Visibility = Visibility.Visible;
                            status_txtblock.Text = text;
                            status_txtblock.Foreground = sucess ? Brushes.Green : Brushes.Red;
                            break;
                    }

                });
            }
            catch (Exception ex)
            {
                StringBuilder sbInParams = new StringBuilder();
                sbInParams.Append("<Method>ShowLoadingBar</Method>");
                log.Error(sbInParams, ex);
            }
        }

        private void ShowLoadingBar(BulkOperation bulkOperation)
        {
            try
            {
                this.Dispatcher.Invoke(() =>
                {
                    switch (bulkOperation)
                    {
                        case BulkOperation.Insert:
                            loadingBulkInsert_progressBar.Visibility = Visibility.Visible;
                            break;
                        case BulkOperation.Update:
                            loading_progressBar.Visibility = Visibility.Visible;
                            break;
                    }

                });
            }
            catch (Exception ex)
            {
                StringBuilder sbInParams = new StringBuilder();
                sbInParams.Append("<Method>ShowLoadingBar</Method>");
                log.Error(sbInParams, ex);
            }
        }

        private void HideLoadingBar(BulkOperation bulkOperation)
        {
            try
            {
                this.Dispatcher.Invoke(() =>
                {
                    switch (bulkOperation)
                    {
                        case BulkOperation.Insert:
                            loadingBulkInsert_progressBar.Visibility = Visibility.Hidden;
                            break;
                        case BulkOperation.Update:
                            loading_progressBar.Visibility = Visibility.Hidden;
                            break;
                    }
                });
            }
            catch (Exception ex)
            {
                StringBuilder sbInParams = new StringBuilder();
                sbInParams.Append("<Method>HideLoadingBar</Method>");
                log.Error(sbInParams, ex);
            }
        }


        #endregion


    }
}
