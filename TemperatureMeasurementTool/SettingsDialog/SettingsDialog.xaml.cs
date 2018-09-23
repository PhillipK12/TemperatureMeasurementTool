using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using TemperatureMeasurementTool.Properties;
using System.IO;
using Microsoft.Win32;
using System.ComponentModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Net.Mail;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Fsolutions.Fbase.Common.Mail;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Microsoft.Office.Interop.Excel;
using Button = System.Windows.Controls.Button;
using CheckBox = System.Windows.Controls.CheckBox;
using TextBox = System.Windows.Controls.TextBox;
using Window = System.Windows.Window;
using p = TemperatureMeasurementTool.Properties;

namespace TemperatureMeasurementTool
{

    public partial class SettingsDialog : Window
    {
        #region fields
        private bool _isSaved = true;
        private OpenFileDialog _openFileDialog;
        private string _previouseValueFrom;
        private string _previouseValueTo;
        #endregion

        #region properties
        public MainWindow MainWindow { get; set; }
        public Action<object, CancelEventArgs> FileOk { get; private set; }
        #endregion

        public SettingsDialog()
        {
            InitializeComponent();
            Setup();
        }

        /// <summary>
        /// Loads data from the settings puts them in the ui
        /// checks if file path is correct
        /// </summary>
        private void Setup()
        {
            ToggleDatei.IsChecked = true;
            CloseAfterSave.IsChecked = Settings.Default.IsCloseAfterSaveEnabled;
            if (!string.IsNullOrWhiteSpace(Settings.Default.ExcelFilePath))
            {
                var path = Settings.Default.ExcelFilePath;
                if (File.Exists(path))
                {
                    // path is a file
                    TbxFileConfigPath.Text = Settings.Default.ExcelFilePath;
                }
                else if (Directory.Exists(path))
                {
                    // path is a directory.
                    NeedExcelFilePath(true);
                }
                else
                {
                    // path doesn't exist.
                    NeedExcelFilePath(true);
                }
            }

            TempInputTo.Text = Settings.Default.TemperatureLimitTo.ToString(CultureInfo.CurrentCulture);
            TempInputFrom.Text = Settings.Default.TemperatureLimitFrom.ToString(CultureInfo.CurrentCulture);

            TxtMailFrom.Text = Settings.Default.MailTransmitterAdress;
            TxtMailTo.Text = Settings.Default.RecentMailadresse;


            if (Settings.Default.AssignedUsersList != null)
            {
                foreach (var element in Settings.Default.AssignedUsersList)
                {
                    LstAssignedEmployees.Items.Add(element);
                }

                LstAssignedEmployees.SelectedIndex = Settings.Default.IndexRecentSelectedEmployee;
            }
        }
        
        /// <summary>
        /// Creates an empty excel file, file-format is xlsx with a header 
        /// </summary>
        /// <param name="fileDialog"></param>
        private void InitializeExcelFile(SaveFileDialog fileDialog)
        {
            using (ExcelPackage excel = new ExcelPackage())
            {
                var excelWorksheet = excel.Workbook.Worksheets.Add(DateTime.Today.Year.ToString());
                //var excelWorksheet = excel.Workbook.Worksheets[1]; //WTF? ARRAY STARTS WITH ONE?!?!?!?!? 
                excelWorksheet.Cells["A1:G1"].LoadFromArrays(new List<string[]>() { new[] {
                p.Resources.ExcelFile_TitleColumn_Date , 
                        p.Resources.ExcelFile_TitleColumn_FirstTime,
                        p.Resources.ExcelFile_TitleColumn_FirstTemp,
                        p.Resources.ExcelFile_TitleColumn_FirstEmployee,
                        p.Resources.ExcelFile_TitleColumn_SecondTime,
                        p.Resources.ExcelFile_TitleColumn_SecondTemp,
                        p.Resources.ExcelFile_TitleColumn_SecondEmployee,
                    }
        });
                // Cells args are first row, first col, last row, last col
                using (var rowRngHeader = excelWorksheet.Cells[1, 1, 1, 7])
                {
                    rowRngHeader.Style.Font.Name = "Segoe UI";
                    rowRngHeader.Style.Font.Bold = true;
                    rowRngHeader.Style.Font.Size = 12;
                    rowRngHeader.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                }
                excelWorksheet.Cells.AutoFitColumns();
                var excelFile = new FileInfo(fileDialog.FileName);
                excel.SaveAs(excelFile);
            }
        }
        
        /// <summary>
        /// Gets called when there is no filepath to a valid excel file and vice versa if transmitted value is false
        /// So a warning-text is in/visible and other settings get en/disabled
        /// </summary>
        /// <param name="value"></param>
        public void NeedExcelFilePath(bool value)
        {
            if (!value && !string.IsNullOrWhiteSpace(Settings.Default.ExcelFilePath))
            {
                if (File.Exists(Settings.Default.ExcelFilePath))
                {
                    return;
                }
                Settings.Default.ExcelFilePath = string.Empty;
            }
            ToggleDatei.IsEnabled = !value;
            ToggleAllgemein.IsEnabled = !value;
            ToggleProgramm.IsEnabled = !value;
            BtnPanelFileOperations.Visibility = !value ? Visibility.Visible : Visibility.Collapsed;
            MailPanel.Visibility = !value ? Visibility.Visible : Visibility.Collapsed;
            borWarning.Visibility = !value ? Visibility.Collapsed : Visibility.Visible;
            Height = !value ? 660 : 400;
        }

        #region event methods
        private void DragMoveSettingsDialog_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void PrintDoc_OnClick(object sender, RoutedEventArgs e)
        {
            Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();

            // Open the Workbook:
            Microsoft.Office.Interop.Excel.Workbook wb = excelApp.Workbooks.Open(
                Settings.Default.ExcelFilePath,
                Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                Type.Missing, Type.Missing, Type.Missing, Type.Missing);

            // Get the first worksheet.
            // (Excel uses base 1 indexing, not base 0.)
            Microsoft.Office.Interop.Excel.Worksheet ws = (Microsoft.Office.Interop.Excel.Worksheet)wb.Worksheets[1];

            foreach (Microsoft.Office.Interop.Excel.Worksheet displayWorksheet in wb.Worksheets)
            {
               displayWorksheet.PrintOut(
                Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            }
            // Cleanup:
            GC.Collect();
            GC.WaitForPendingFinalizers();

            Marshal.FinalReleaseComObject(ws);

            wb.Close(false, Type.Missing, Type.Missing);
            Marshal.FinalReleaseComObject(wb);

            excelApp.Quit();
            Marshal.FinalReleaseComObject(excelApp);
        }

        private void CloseSettingsDialog_OnClick(object sender, RoutedEventArgs e)
        {
            bool _ShouldClose = true;
            if (!_isSaved)
            {
                var result = MessageBox.Show("Wollen Sie vorher alle Änderungen speichern?", "Abfrage",
                    MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    //ToDo Save
                    _ShouldClose = true;
                }
                else if (result == MessageBoxResult.No)
                {
                    _ShouldClose = true;
                }
                else if (result == MessageBoxResult.Cancel)
                {
                    _ShouldClose = false;
                }
            }

            if (_ShouldClose) Close();
        }

        private void SaveSettings_OnClick(object sender, RoutedEventArgs e)
        {
            Settings.Default.TemperatureLimitFrom = Convert.ToDecimal(TempInputFrom.Text, new NumberFormatInfo() { NumberDecimalSeparator = "," });
            Settings.Default.TemperatureLimitTo = Convert.ToDecimal(TempInputTo.Text, new NumberFormatInfo() { NumberDecimalSeparator = "," });
            Settings.Default.AssignedUsersList.Clear();
            foreach(var item in LstAssignedEmployees.Items)
            {
                Settings.Default.AssignedUsersList.Add((string)item);
            }
            Settings.Default.Save();
            if (MainWindow != null)
            {
                if (MainWindow.WindowState == WindowState.Minimized) MainWindow.WindowState = WindowState.Normal;
                MainWindow.ShowInformationText(p.Resources.SettingsDialog_Message_SuccessfullySaved);
                MainWindow.SettingsChanged();
            }
            Close();

        }

        private void BtnCreateFile_OnClick(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Excel Datei (*.xlsx)|*.xlsx",
                FileName = Settings.Default.FileName,
                FilterIndex = 2,
                RestoreDirectory = true
            };
            var result = saveFileDialog.ShowDialog();
            if (result == true)
            {
                InitializeExcelFile(saveFileDialog);
                TbxFileConfigPath.Text = saveFileDialog.FileName;
                Settings.Default.ExcelFilePath = saveFileDialog.FileName;
                SaveSettings_OnClick(BtnSaveSettings, null);
            }
        }

        private void ToggleMenu_OnChecked(object sender, RoutedEventArgs e)
        {
            var toggleButton = e.Source as ToggleButton;
            if (toggleButton == null || toggleButton.IsChecked == false)
            {
                e.Handled = false;
                return;
            }

            toggleButton.Background = Brushes.Transparent;


            if (Equals(toggleButton, ToggleDatei))
            {
                BordDatei.Visibility = Visibility.Visible;
                BordAllgemein.Visibility = Visibility.Hidden;
                BordProgramm.Visibility = Visibility.Hidden;
                ToggleAllgemein.IsChecked = false;
                ToggleProgramm.IsChecked = false;
            }
            else if (Equals(toggleButton, ToggleAllgemein))
            {
                BordAllgemein.Visibility = Visibility.Visible;
                BordProgramm.Visibility = Visibility.Hidden;
                BordDatei.Visibility = Visibility.Hidden;
                ToggleDatei.IsChecked = false;
                ToggleProgramm.IsChecked = false;
            }
            else if (Equals(toggleButton, ToggleProgramm))
            {
                BordProgramm.Visibility = Visibility.Visible;
                BordAllgemein.Visibility = Visibility.Hidden;
                BordDatei.Visibility = Visibility.Hidden;
                ToggleAllgemein.IsChecked = false;
                ToggleDatei.IsChecked = false;
            }
        }

        private void AddColleague_OnClick(object sender, RoutedEventArgs e)
        {           
            LstAssignedEmployees.Items.Add(TxtNewEmployee.Text);
            TxtNewEmployee.Text = string.Empty;
            TxtNewEmployee.Focus();
        }
        
        private void UIElement_OnMouseEnter(object sender, MouseEventArgs e)
        {

            if (sender is TextBlock block)
            {
                block.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00A8DE"));

            }
            else if (sender is Button button)
            {
                button.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00A8DE"));
            }
        }

        private void UIElement_OnMouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is TextBlock block)
            {
                block.Foreground = Brushes.White;

            }
            else if (sender is Button button)
            {
                button.Foreground = Brushes.White;
            }
        }

        private void ChooseExcelFile_Click(object sender, RoutedEventArgs e)
        {
            _openFileDialog = new OpenFileDialog();
            _openFileDialog.Filter = "Excel Dateien (*.xlsx)|*.xlsx|All files (*.*)|*.*";
            _openFileDialog.FileOk += ChooseExcelFile_Ok;
            _openFileDialog.ShowDialog();
        }

        private void ChooseExcelFile_Ok(object sender, CancelEventArgs e)
        {
            TbxFileConfigPath.Text = _openFileDialog.FileName;
            Settings.Default.ExcelFilePath = _openFileDialog.FileName;
            NeedExcelFilePath(false);
        }

        private void OpenDoc_OnClick(object sender, RoutedEventArgs e)
        {
            if (File.Exists(Settings.Default.ExcelFilePath))
            {
                var path = Path.GetFullPath(Settings.Default.ExcelFilePath);
                Process.Start(path);
            }
        }

        private void CloseAfterSave_OnChecked(object sender, RoutedEventArgs e)
        {
            Settings.Default.IsCloseAfterSaveEnabled = ((CheckBox)sender).IsChecked == true;
        }

        private void BtnTempDown_OnClick(object sender, RoutedEventArgs e)
        {
            var btnTempDown = (Button)sender;
            if (btnTempDown != null)
            {
                if (btnTempDown.Equals(BtnTempFromDown))
                {
                    var value = Convert.ToDecimal(TempInputFrom.Text, new NumberFormatInfo() { NumberDecimalSeparator = "," });
                    value = decimal.Subtract(value, new decimal(0.1));
                    TempInputFrom.Text = value.ToString(CultureInfo.CurrentCulture);
                }
                else if (btnTempDown.Equals(BtnTempToDown))
                {
                    var value = Convert.ToDecimal(TempInputTo.Text, new NumberFormatInfo() { NumberDecimalSeparator = "," });
                    value = decimal.Subtract(value, new decimal(0.1));
                    TempInputTo.Text = value.ToString(CultureInfo.CurrentCulture);
                }
            }
        }

        private void BtnTempUp_OnClick(object sender, RoutedEventArgs e)
        {
            var btnTempUp = (Button)sender;
            if (btnTempUp != null)
            {
                if (btnTempUp.Equals(BtnTempFromUp))
                {
                    var value = Convert.ToDecimal(TempInputFrom.Text, new NumberFormatInfo() { NumberDecimalSeparator = "," });
                    value = decimal.Add(value, new decimal(0.1));
                    TempInputFrom.Text = value.ToString(CultureInfo.CurrentCulture);
                }
                else if (btnTempUp.Equals(BtnTempToUp))
                {
                    var value = Convert.ToDecimal(TempInputTo.Text, new NumberFormatInfo() { NumberDecimalSeparator = "," });
                    value = decimal.Add(value, new decimal(0.1));
                    TempInputTo.Text = value.ToString(CultureInfo.CurrentCulture);
                }
            }
        }

        private void TempInput_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var tempInput = (TextBox)sender;
            if (tempInput != null)
            {
                if (tempInput.Equals(TempInputFrom))
                {
                    if (!Regex.IsMatch(TempInputFrom.Text, "^-?[0-9]?[0-9][.,]?[0-9]?$"))
                    {
                        TempInputFrom.Text = _previouseValueFrom;
                    }
                }
                else if (tempInput.Equals(TempInputTo))
                {
                    if (!Regex.IsMatch(TempInputTo.Text, "^-?[0-9]?[0-9][.,]?[0-9]?$"))
                    {
                        TempInputTo.Text = _previouseValueTo;
                    }
                }
            }
        }

        private void Temperaturinput_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var tempInput = (TextBox)sender;
            if (tempInput != null)
            {
                if (tempInput.Equals(TempInputFrom))
                {
                    _previouseValueFrom = TempInputFrom.Text;
                }
                else if (tempInput.Equals(TempInputTo))
                {
                    _previouseValueTo = TempInputTo.Text;
                }
            }
        }

        private void DeleteColleague_OnClick(object sender, RoutedEventArgs e)
        {
            LstAssignedEmployees.Items.Remove(LstAssignedEmployees.SelectedItem);
        }
               
        private void BtnSendMail_OnClick(object sender, RoutedEventArgs e)
        {
            var mailMessage = new MailMessage
            {
                From = new MailAddress(TxtMailFrom.Text),
                To = {new MailAddress(TxtMailTo.Text)},
                Subject = "Temperaturmessung Exceldatei Export",
                IsBodyHtml = true,
                Body = "<span style='font-size: 12pt; font-family:Calibri; color: black;'>Im Anhang befinden sich die Temperaturmessungen</span>"
            };
            mailMessage.Attachments.Add(new Attachment(Settings.Default.ExcelFilePath));
            var filename = Path.GetDirectoryName(Settings.Default.ExcelFilePath).Replace("\\", "/") + "/ExportExcelFileMessage.eml";

            //save the MailMessage to the filesystem
            mailMessage.Save(filename);

            //Open the file with the default associated application registered on the local machine
            Process.Start(filename);
        }

        private void TxtNewEmployee_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                AddColleague_OnClick(null,null);
            }else if (e.Key == Key.Delete)
            {
                DeleteColleague_OnClick(null,null);
            }
        }

        private void ConvertXlsxToPdf_OnClick(object sender, RoutedEventArgs e)
        {
            Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();

            // Open the Workbook:
            var wb = excelApp.Workbooks.Open(
                Settings.Default.ExcelFilePath,
                Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                Type.Missing, Type.Missing, Type.Missing, Type.Missing);

            // Get the first worksheet.
            // (Excel uses base 1 indexing, not base 0.)
            Worksheet ws = (Worksheet)wb.Worksheets[1];

            foreach (Worksheet displayWorksheet in wb.Worksheets)
            {
                displayWorksheet.PageSetup.Orientation = XlPageOrientation.xlLandscape;
                var filename = Path.GetFileNameWithoutExtension(Settings.Default.ExcelFilePath);
                displayWorksheet.ExportAsFixedFormat(XlFixedFormatType.xlTypePDF, Path.GetDirectoryName(Settings.Default.ExcelFilePath) +"\\" + filename + displayWorksheet.Name + ".pdf");
            }
            // Cleanup:
            GC.Collect();
            GC.WaitForPendingFinalizers();

            Marshal.FinalReleaseComObject(ws);

            wb.Close(false, Type.Missing, Type.Missing);
            Marshal.FinalReleaseComObject(wb);

            excelApp.Quit();
            Marshal.FinalReleaseComObject(excelApp);
        }
        #endregion
    }
}