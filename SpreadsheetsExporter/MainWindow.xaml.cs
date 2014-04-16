using Google.GData.Spreadsheets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SpreadsheetsExporter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SpreadsheetOperations _operations;
        private SpreadsheetEntry _selectedSpreadsheet;
        private WorksheetEntry _selectedWorksheet;
        private IWritersProviders _writersProvider = new SimpleWritersProvider();

        public MainWindow()
        {
            InitializeComponent();
            authCode.Visibility = Visibility.Hidden;
            goButton.Visibility = Visibility.Hidden;
            spreadsheetsStackPanel.Visibility = Visibility.Hidden;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            connectButton.IsEnabled = false;
            _operations = new SpreadsheetOperations();
            var authorizationUrl = _operations.GetAuthorizationUrl();
            System.Diagnostics.Process.Start(authorizationUrl);
            authCode.Visibility = Visibility.Visible;
            goButton.Visibility = Visibility.Visible;

        }

        private void goButton_Click(object sender, RoutedEventArgs e)
        {
            _operations.SetAccessCode(authCode.Text);
            _operations.Connect();
            connectionStackPanel.Visibility = Visibility.Hidden;

            GetSpreadsheets();
        }

        private void GetSpreadsheets()
        {
            var spreadsheets = _operations.GetSpreadsheetList();
            spreadsheetsStackPanel.Visibility = Visibility.Visible;
            spreadsheetsList.ItemsSource = spreadsheets;
            selectSpreadsheetButton.IsEnabled = true;
            useWorksheetButton.IsEnabled = false;
            useWriterButton.IsEnabled = false;
        }

        private void authCode_GotFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            authCode.SelectAll();
        }

        private void authCode_GotFocus(object sender, MouseEventArgs e)
        {
            authCode.SelectAll();
        }

        private void reloadSpreadsheetsButton_Click(object sender, RoutedEventArgs e)
        {
            GetSpreadsheets();
        }

        private void selectSpreadsheetButton_Click(object sender, RoutedEventArgs e)
        {
            if (spreadsheetsList.SelectedItem == null)
            {
                return;
            }
            _selectedSpreadsheet = (SpreadsheetEntry)spreadsheetsList.SelectedItem;
            WorksheetFeed wsFeed = _selectedSpreadsheet.Worksheets;
            spreadsheetsList.ItemsSource = wsFeed.Entries;
            selectSpreadsheetButton.IsEnabled = false;
            useWorksheetButton.IsEnabled = true;
            useWriterButton.IsEnabled = false;
        }

        private void useWorksheetButton_Click(object sender, RoutedEventArgs e)
        {
            if (spreadsheetsList.SelectedItem == null)
            {
                return;
            }

            _selectedWorksheet = (WorksheetEntry)spreadsheetsList.SelectedItem;
            spreadsheetsList.ItemsSource = _writersProvider.GetWriters();
            selectSpreadsheetButton.IsEnabled = false;
            useWorksheetButton.IsEnabled = false;
            useWriterButton.IsEnabled = true;
        }

        private void useWriterButton_Click(object sender, RoutedEventArgs e)
        {
            if (spreadsheetsList.SelectedItem == null)
            {
                return;
            }
            var values = _operations.GetList(_selectedWorksheet);
            var writer = _writersProvider.GetWriter(((WriterInfo)spreadsheetsList.SelectedItem).WriterName);

            var result = writer.Write(values);
            Clipboard.SetText(result);
        }
    }
}
