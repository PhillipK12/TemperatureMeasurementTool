using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using OfficeOpenXml;
using TemperatureMeasurementTool.Properties;
using p = TemperatureMeasurementTool.Properties;
using Timer = System.Timers.Timer;
using System.Windows.Controls.Primitives;
using Nager.Date;
using OfficeOpenXml.Style;

namespace TemperatureMeasurementTool
{
    /// <summary>
    /// The Logic of the Main Window shown in the bottom right of the desktop
    /// The logic behind the MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        #region fields
        private SettingsDialog _settingsDialog;
        private ExportDialog _excelExportDialog;
        private string _previouseValue;
        #endregion

        public MainWindow()
        {
            InitializeComponent();
            Setup();
        }

        /// <summary>
        /// When starting the main window the Setup method loads users settings
        /// and importent data for the interaction with the user
        /// </summary>
        private void Setup()
        {
            //The input field of the temperature gets the focus for great useability
            TempInput.Focus();
            //Todays Date is the initial/default value of the input value
            DtpActualDate.Text = DateTime.Now.Date.ToString(Settings.Default.DateFormat);
            if (Settings.Default.AssignedUsersList != null)
            {
                foreach (var element in Settings.Default.AssignedUsersList)
                {
                    LstAssignedEmployees.Items.Add(element);
                }
                LstAssignedEmployees.SelectedIndex = Settings.Default.IndexRecentSelectedEmployee;
            }
            TempInput.Text = Settings.Default.RecentTemperatureInput.ToString(CultureInfo.CurrentCulture);
            CheckExcelFilePath();
            /*TODO Check if there are any missing entries from the last recorded time someone save a entry
            var lastRecord = Settings.Default.DateLastRecord.AddDays(1);
            var yesterdaysDate = DateTime.Today.Date.AddDays(-1);
            if (Settings.Default.DateLastRecord != null && lastRecord < yesterdaysDate)
            {
                var dateCount = 0;
                HinweisMissingEntry.Visibility = Visibility.Visible;
                while (lastRecord <= yesterdaysDate)
                {
                    if (Settings.Default.MissingDateCollection == null) Settings.Default.MissingDateCollection = new System.Collections.Specialized.StringCollection();
                    Settings.Default.MissingDateCollection.Add(lastRecord.ToShortDateString());
                    dateCount = ++dateCount;
                    lastRecord = lastRecord.AddDays(1);
                }
                HinweisTextMissingEntry.Text = dateCount + " Fehlende Tage";
            }
            */
        }

        /// <summary>
        /// Check if the entered value if over or under the temperature limits saved in the settings
        /// If its over or under the temperature limits (coming from the settings) the user gets an warning
        /// </summary>
        /// <param name="text"></param>
        private void CheckTemperatureInput(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return;
            var tempFrom = Settings.Default.TemperatureLimitFrom;
            var tempTo = Settings.Default.TemperatureLimitTo;
            var value = Convert.ToDecimal(text, new NumberFormatInfo() { NumberDecimalSeparator = "," });
            if (value < tempFrom || value > tempTo)
            {
                borTemp.BorderBrush = Brushes.Red;
                TxtWarning.Visibility = Visibility.Visible;
            }
            else
            {
                borTemp.BorderBrush = Brushes.Transparent;
                TxtWarning.Visibility = Visibility.Hidden;
            }
        }

        /// <summary>
        /// After Settings Dialog closes the is a fresh list of all assigned users 
        /// </summary>
        internal void SettingsChanged()
        {
            //get a fresh list of assigned users
            LstAssignedEmployees.Items.Clear();
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
        /// Shows an information text transmitted by calling the method 
        /// and it will be shown on the bottom right side of the main window
        /// </summary>
        /// <param name="message"></param>
        /// <param name="messageTyp">Information/Exclamation/Error</param>
        public void ShowInformationText(string message, MessageTyp messageTyp = MessageTyp.Information)
        {
            var timer = new Timer();
            timer.Interval = 5000;
            timer.Elapsed += Timer_Elapsed;
            BorHint.Visibility = Visibility.Visible;

            switch (messageTyp)
            {
                //Successfully -> Green Color / Symbol Hool 
                case MessageTyp.Success:
                    SolidColorBrush informationsBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#02DA5A"));
                    BorHint.BorderBrush = informationsBrush;
                    SymbolHint.Foreground = informationsBrush;
                    SymbolHint.Text = "\xE73E";
                    TxtHint.Foreground = informationsBrush;
                    break;

                //Information -> Blue Color / Symbol Exclamation 
                case MessageTyp.Information:
                    SolidColorBrush exclamationsBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6495ED"));
                    BorHint.BorderBrush = exclamationsBrush;
                    SymbolHint.Foreground = exclamationsBrush;
                    SymbolHint.Text = "\xE946";
                    TxtHint.Foreground = exclamationsBrush;
                    break;

                //Problem -> Red Color / Symbol Cross 
                case MessageTyp.Error:
                    SolidColorBrush errorsBrush = new SolidColorBrush(Colors.Red);
                    BorHint.BorderBrush = errorsBrush;
                    SymbolHint.Foreground = errorsBrush;
                    SymbolHint.Text = "\xEB90";
                    TxtHint.Foreground = errorsBrush;
                    break;

                //Warnung -> Yellow Color / Warning symbol
                case MessageTyp.Warning:
                    SolidColorBrush warningBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFD600"));
                    BorHint.BorderBrush = warningBrush;
                    SymbolHint.Foreground = warningBrush;
                    SymbolHint.Text = "\xE7BA";
                    TxtHint.Foreground = warningBrush;
                    break;
            }
            TxtHint.Text = message;
            timer.Start();
        }

        #region event methods
        private void CloseWindow_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void DragWindow_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void PositioningMainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            var desktopWorkingArea = SystemParameters.WorkArea;
            Left = desktopWorkingArea.Right - Width;
            Top = desktopWorkingArea.Bottom - Height;
        }

        /// <summary>
        /// When something typed backup the correct input
        /// for overwriting wrong input values with the last correct value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Temperaturinput_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            _previouseValue = TempInput.Text;
        }

        /// <summary>
        /// Opens a new instance of the SettingsDialog 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnOpenSettings_OnClick(object sender, RoutedEventArgs e)
        {
            //TODO Check if there are already a instance of the settings dialog open
            _settingsDialog = new SettingsDialog { MainWindow = this };
            _settingsDialog.Show();
        }

        /// <summary>
        /// Checks if Excel file path exist
        /// if not then opens the SettingsDialog
        /// </summary>
        private bool CheckExcelFilePath()
        {
            if (string.IsNullOrWhiteSpace(Settings.Default.ExcelFilePath) || File.Exists(Settings.Default.ExcelFilePath) == false)
            {
                ShowInformationText(p.Resources.MainWindow_Message_NoExcelFile, MessageTyp.Error);
                //Opens the Settings Dialog for choosing an existing excel file or creating one so its possible to save an temperature
                //TODO There are 2 Options 1.)make Settings Dialog in this scenario modal, so user can't save any entry 2.)Catch users action to save or close file without any file existing
                _settingsDialog = new SettingsDialog();
                _settingsDialog.Show();
                _settingsDialog.NeedExcelFilePath(true);
                _settingsDialog.MainWindow = this;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Saves the entry in the Excel File
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnDone_OnClick(object sender, RoutedEventArgs e)
        {
            //if Excel file path does not exist then show settings & dont save
            if (!CheckExcelFilePath()) return;

            var pickedDate = ToggleVacationEntry.IsChecked == true ? DateTime.Parse(DtpVacationFromDate.Text) : DateTime.Parse(DtpActualDate.Text);
            if (pickedDate == null)
            {
                ShowInformationText(p.Resources.MainWindow_Message_NoDatePicked, MessageTyp.Error);
            }

            using (ExcelPackage excelFile = new ExcelPackage(new FileInfo(Settings.Default.ExcelFilePath)))
            {
                //Search for the correct worksheet
                var actWorksheet = excelFile.Workbook.Worksheets.FirstOrDefault(a => a.Name.Equals(pickedDate.Year.ToString()));
                if (actWorksheet == null) CreateNewWorksheet(actWorksheet, excelFile, pickedDate);

                //Find out which position pickeddate needs to be
                var firstDate = DateTime.Parse("01.01." + pickedDate.Year);
                var rowCount = Settings.Default.EntriesStartsWith + (pickedDate - firstDate).TotalDays;

                //Get Value of that position for profing its correct row
                var valueOfRow = actWorksheet.Cells["A" + rowCount].Value as string;
                if (!string.IsNullOrWhiteSpace(valueOfRow) && DateTime.TryParse(valueOfRow, out DateTime dateOfRow))
                {
                    // If its a holiday then do nothing and return
                    if (DateSystem.IsOfficialPublicHolidayByCounty(dateOfRow, CountryCode.DE, Settings.Default.CountryCode))
                    {
                        ShowInformationText(p.Resources.MainWindow_Message_Holiday, MessageTyp.Warning);
                        return;
                    }
                    //  If its a day of the weekend then do nothing and return
                    if (dateOfRow.DayOfWeek == DayOfWeek.Sunday || dateOfRow.DayOfWeek == DayOfWeek.Saturday)
                    {
                        ShowInformationText(p.Resources.MainWindow_Message_Weekend + " " + dateOfRow.DayOfWeek, MessageTyp.Warning);
                        return;
                    }

                    //Check if the date of the position matches picked date
                    if (dateOfRow.Equals(pickedDate))
                    {
                        if (ToggleVacationEntry.IsChecked == true)
                        {
                            //Settings.Default.RecentSinceDateTime
                            var dateTo = DateTime.Parse(DtpVacationToDate.Text);
                            var index = rowCount;
                            while (pickedDate <= dateTo)
                            {
                                if ( !(pickedDate.DayOfWeek == DayOfWeek.Sunday || pickedDate.DayOfWeek == DayOfWeek.Saturday) && !DateSystem.IsOfficialPublicHolidayByCounty(pickedDate, CountryCode.DE, Settings.Default.CountryCode))
                                {
                                    var value = string.IsNullOrWhiteSpace(VacationDescriptionInput.Text) ? p.Resources.ExcelFile_EntryText_Vacation : VacationDescriptionInput.Text;
                                    actWorksheet.Cells[$"B{index}:G{index}"].Merge = true;
                                    actWorksheet.Cells[$"B{index}"].Value = value;
                                    actWorksheet.Cells[$"B{index}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                }
                                index = ++index;
                                pickedDate = pickedDate.AddDays(1);
                            }
                        }
                        //Check if 1. entry is open
                        else if (actWorksheet.Cells["B" + rowCount].Value == null)
                        {
                            //if so bring all values in that row
                            actWorksheet.Cells["B" + rowCount + ":D" + rowCount].LoadFromArrays(new List<string[]>()
                            {
                                new[]
                                {
                                    //Time | Temperature | Employee
                                    TimePicker.GetValueAsString(), TempInput.Text + p.Resources.DegreeSymbol,
                                    LstAssignedEmployees.Text
                                }
                             });
                        }
                        //Check if 2. entry is open
                        else if (actWorksheet.Cells["E" + rowCount].Value == null)
                        {
                            //if so bring all values in that row
                            actWorksheet.Cells["E" + rowCount + ":G" + rowCount].LoadFromArrays(new List<string[]>()
                            {
                                new[]
                                {
                                    //Time | Temperature | Employee
                                    TimePicker.GetValueAsString(), TempInput.Text + p.Resources.DegreeSymbol,
                                    LstAssignedEmployees.Text
                                }
                             });
                        }
                        //Already both entries saved
                        else
                        {
                            //ToDo create Dialog for asking User if the value should be overwritten
                            var result = MessageBox.Show("Bereits beide Einträge vorhanden!");
                        }
                    }
                }

                //try saving the excel file with all changes
                bool isSaved = false;
                try
                {
                    excelFile.Save();
                    isSaved = true;
                }
                catch (InvalidOperationException ee)
                {
                    var result = MessageBox.Show(p.Resources.ErrorMessageExcelFileIsStillOpen + "\n" + p.Resources.ErrorMessage + ": " + ee.Message, p.Resources.ErrorMessageTitle, MessageBoxButton.OKCancel, MessageBoxImage.Error, MessageBoxResult.Cancel);
                    while (result != MessageBoxResult.Cancel && !isSaved)
                    {
                        try
                        {
                            excelFile.Save();
                            isSaved = true;
                        }
                        catch (InvalidOperationException)
                        {
                            result = MessageBox.Show(p.Resources.ErrorMessageExcelFileIsStillOpen + "\n" + p.Resources.ErrorMessage + ": " + ee.Message, p.Resources.ErrorMessageTitle, MessageBoxButton.OKCancel, MessageBoxImage.Error, MessageBoxResult.Cancel);

                        }
                    }
                    if (result == MessageBoxResult.Cancel)
                    {
                        excelFile.Dispose();
                    }
                }

                Settings.Default.RecentTemperatureInput = Convert.ToDecimal(TempInput.Text, new NumberFormatInfo() { NumberDecimalSeparator = "," });
                Settings.Default.Save();
                ShowInformationText(p.Resources.MainWindow_Message_SuccessfullyEntered, MessageTyp.Success);
                if (Settings.Default.IsCloseAfterSaveEnabled) Close();

            }
        }

        /// <summary>
        /// Creates a new worksheet for the picked date
        /// </summary>
        /// <param name="actWorksheet"></param>
        /// <param name="excelFile"></param>
        /// <param name="pickedDate"></param>
        private void CreateNewWorksheet(ExcelWorksheet actWorksheet, ExcelPackage excelFile, DateTime pickedDate)
        {
            actWorksheet = excelFile.Workbook.Worksheets.Add(pickedDate.Year.ToString());
            excelFile.Workbook.Worksheets.MoveToStart(actWorksheet.Index);
            actWorksheet.Cells["A1:G1"].LoadFromArrays(new List<string[]>() { new[] {
                       p.Resources.ExcelFile_TitleColumn_Date ,
                       p.Resources.ExcelFile_TitleColumn_FirstTime,
                       p.Resources.ExcelFile_TitleColumn_FirstTemp,
                       p.Resources.ExcelFile_TitleColumn_FirstEmployee,
                       p.Resources.ExcelFile_TitleColumn_SecondTime,
                       p.Resources.ExcelFile_TitleColumn_SecondTemp,
                       p.Resources.ExcelFile_TitleColumn_SecondEmployee,
                    } });
            // Cells args are first row, first col, last row, last col
            using (var rowRngHeader = actWorksheet.Cells[1, 1, 1, 7])
            {
                rowRngHeader.Style.Font.Name = "Segoe UI";
                rowRngHeader.Style.Font.Bold = true;
                rowRngHeader.Style.Font.Size = 12;
                rowRngHeader.Style.Border.BorderAround(ExcelBorderStyle.Thin);
            }
            actWorksheet.Cells.AutoFitColumns();
        }

        /// <summary>
        /// Gets called when timer runs out to hide the information text again
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_Elapsed(object sender, EventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke((Action)(() => BorHint.Visibility = Visibility.Collapsed));
        }

        /// <summary>
        /// Substracts the value by 0.1
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnTempDown_OnClick(object sender, RoutedEventArgs e)
        {
            var value = Convert.ToDecimal(TempInput.Text, new NumberFormatInfo() { NumberDecimalSeparator = "," });
            value = decimal.Subtract(value, new decimal(0.1));
            TempInput.Text = value.ToString(CultureInfo.CurrentCulture);
            CheckTemperatureInput(TempInput.Text);
        }

        /// <summary>
        /// Adds to the value by 0.1
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnTempUp_OnClick(object sender, RoutedEventArgs e)
        {
            var value = Convert.ToDecimal(TempInput.Text, new NumberFormatInfo() { NumberDecimalSeparator = "," });
            value = decimal.Add(value, new decimal(0.1));
            TempInput.Text = value.ToString(CultureInfo.CurrentCulture);
            CheckTemperatureInput(TempInput.Text);
        }

        /// <summary>
        /// Checks the temperature input from the user and 
        /// puts the previous value into the text field in case the input is not valid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChecksTemperatureInput_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!Regex.IsMatch(TempInput.Text, "^-?[0-9]?[0-9][.,]?[0-9]?$"))
            {
                TempInput.Text = _previouseValue;
                return;
            }
            CheckTemperatureInput(TempInput.Text);
        }

        private void AnimationForegroundColor_OnMouseEnter(object sender, MouseEventArgs e)
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

        private void AnimationForegroundColor_OnMouseLeave(object sender, MouseEventArgs e)
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

        private void OpenExportDialog_OnClick(object sender, RoutedEventArgs e)
        {
            _excelExportDialog = new ExportDialog();
            _excelExportDialog.Show();
        }

        /// <summary>
        /// If user clicks on the button for entering a vacation / free day instead of an temperature input
        /// it will makes the correct panel visible and the current panel invisible and vice versa
        /// /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VacationEntry_CheckChanged(object sender, RoutedEventArgs e)
        {
            var toggleVacation = (ToggleButton)sender;

            if (toggleVacation.IsChecked == true)
            {
                NormalEntryContent.Visibility = Visibility.Collapsed;
                VacationEntryContent.Visibility = Visibility.Visible;
                labelVacation.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00A8DE"));
            }
            else
            {
                NormalEntryContent.Visibility = Visibility.Visible;
                VacationEntryContent.Visibility = Visibility.Collapsed;
                labelVacation.Foreground = Brushes.White;

            }

        }
        #endregion

        /* TODO Availeable soon in the next version
         */
        //private void HinweisMissingEntry_Click(object sender, RoutedEventArgs e)
        //{
        //    var missingEntryDialog = new MissingEntryDialog();
        //    missingEntryDialog.Show();
        //    missingEntryDialog.ShowMissingEntries();
        //}
    }

    public enum MessageTyp
    {
        Success,
        Information,
        Error,
        Warning
    }
}