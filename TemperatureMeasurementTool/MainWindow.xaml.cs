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
            BorHinweis.Visibility = Visibility.Collapsed;
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

            TempInput.Text = Settings.Default.RecentTemperatureLimit.ToString(CultureInfo.CurrentCulture);
            if (string.IsNullOrWhiteSpace(Settings.Default.ExcelFilePath) ||
                File.Exists(Settings.Default.ExcelFilePath) == false)
            {
                //Opens the Settings Dialog for choosing an existing excel file or creating one so its possible to save an temperature
                //TODO There are 2 Options 1.)make Settings Dialog in this scenario modal, so user can't save any entry 2.)Catch users action to save file without any file existing
                _settingsDialog = new SettingsDialog();
                _settingsDialog.Show();
                _settingsDialog.NeedExcelFilePath(true);
                _settingsDialog.MainWindow = this;
                WindowState = WindowState.Minimized;
            }

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
        public void ShowInformationText(string message)
        {
            var timer = new Timer();
            timer.Interval = 5000;
            timer.Elapsed += Timer_Elapsed;
            BorHinweis.Visibility = Visibility.Visible;
            TxtHinweis.Text = message;
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
        /// Saves the entry in the Excel File
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnDone_OnClick(object sender, RoutedEventArgs e)
        {
            using (ExcelPackage excelFile = new ExcelPackage(new FileInfo(Settings.Default.ExcelFilePath)))
            {
                var pickedDate = DateTime.Parse(DtpActualDate.Text);
                var actWorksheet = excelFile.Workbook.Worksheets.FirstOrDefault(a => a.Name.Equals(pickedDate.Year.ToString()));
                if (actWorksheet == null)
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

                var rowCount = actWorksheet.Dimension.End.Row;
                string lastValue = string.Empty;
                var isLastDateEqualPickedDate = false;
                if (actWorksheet.GetValue(rowCount, 1) != null && rowCount > 1)
                {
                    lastValue = actWorksheet.GetValue(rowCount, 1).ToString();
                    var lastdate = DateTime.Parse(lastValue);

                    // Wenn der letzte Eintrag nicht gestern war dann nachtragen
                    if (lastdate != pickedDate.AddDays(-1))
                    {
                        //Ausgewählte
                        if (lastdate < pickedDate)
                        {
                            var nextdate = lastdate.AddDays(1);
                            while (nextdate <= pickedDate.AddDays(-1))
                            {
                                rowCount = ++rowCount;
                                actWorksheet.Cells["A" + rowCount].Value = nextdate.ToShortDateString();
                                string Text = string.Empty;

                                //Wenn der fehlende Eintrag ein Feiertag war
                                if (DateSystem.IsOfficialPublicHolidayByCounty(nextdate, CountryCode.DE, Settings.Default.CountryCode))
                                {
                                    Text = p.Resources.Holiday;
                                    actWorksheet.Cells[$"B{rowCount}:G{rowCount}"].Merge = true;
                                    actWorksheet.Cells[$"B{rowCount}"].Value = Text;
                                    actWorksheet.Cells[$"B{rowCount}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                }
                                //wenn der fehlende Eintrag ein Tag des Wochenende ist
                                else if (nextdate.DayOfWeek == DayOfWeek.Sunday || nextdate.DayOfWeek == DayOfWeek.Saturday)
                                {
                                    Text = p.Resources.Weekend;
                                    actWorksheet.Cells[$"B{rowCount}:G{rowCount}"].Merge = true;
                                    actWorksheet.Cells[$"B{rowCount}"].Value = Text;
                                    actWorksheet.Cells[$"B{rowCount}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                }

                                //TODO: V2.0 - Sollte es kein Feiertag oder Tag des Wochenende sein, dann sollte hier ein fehlender Eintrag registriert werden

                                nextdate = nextdate.AddDays(1);
                            }
                        }
                        //Zähler für den nächsten Eintrag vom letzten Eintrag herunterzählen
                        else if (lastdate > pickedDate)
                        {
                            var previouseDate = lastdate;
                            while (previouseDate >= pickedDate)
                            {
                                rowCount = --rowCount;
                                previouseDate = previouseDate.AddDays(-1);
                            }
                        }
                    }
                    else if (lastdate.Equals(pickedDate))
                    {
                        isLastDateEqualPickedDate = true;
                        lastValue = lastdate.ToShortDateString();
                    }
                }

                var edited = false;
                if (ToggleVacationEntry.IsChecked == true)
                {
                    var dtFrom = DateTime.Parse(DtpVacationFromDate.Text);
                    var dtTo = DateTime.Parse(DtpVacationToDate.Text);

                    while (dtFrom <= dtTo)
                    {
                        rowCount = ++rowCount;
                        actWorksheet.Cells["A" + rowCount].Value = dtFrom.ToShortDateString();
                        string Text = string.IsNullOrWhiteSpace(VacationDescriptionInput.Text) ? p.Resources.Vacation : VacationDescriptionInput.Text;
                        actWorksheet.Cells[$"B{rowCount}:G{rowCount}"].Merge = true;
                        actWorksheet.Cells[$"B{rowCount}"].Value = Text;
                        actWorksheet.Cells[$"B{rowCount}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        dtFrom = dtFrom.AddDays(1);
                    }
                }
                else
                {
                    if (isLastDateEqualPickedDate || !string.IsNullOrWhiteSpace(lastValue) && Regex.IsMatch(lastValue, "^[0-9][0-9]?"))
                    {
                        var dateLastEntry = DateTime.Parse(lastValue);
                        var datePicked = DateTime.Parse(DtpActualDate.Text);
                        if (dateLastEntry.ToShortDateString().Equals(datePicked.ToShortDateString()))
                        {
                            actWorksheet.Cells["E" + rowCount + ":G" + rowCount].LoadFromArrays(new List<string[]>()
                                {
                                    new[] {TimePicker.GetValueAsString(), TempInput.Text + p.Resources.DegreeSymbol, LstAssignedEmployees.Text}
                                });
                            edited = true;
                        }
                    }
                    if (string.IsNullOrWhiteSpace(lastValue) || !edited)
                    {
                        rowCount = ++rowCount;
                        actWorksheet.Cells["A" + rowCount + ":D" + rowCount].LoadFromArrays(new List<string[]>()
                        {
                            new[]
                            {
                                DtpActualDate.Text, TimePicker.GetValueAsString(), TempInput.Text + p.Resources.DegreeSymbol,
                                LstAssignedEmployees.Text
                            }
                        });
                    }
                }

                var isSaved = false;
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
                }
                if (isSaved) ShowInformationText(p.Resources.MainWindow_Message_SuccessfullyEntered);
                if (Settings.Default.IsCloseAfterSaveEnabled) Close();

            }
        }
        
        /// <summary>
        /// Gets called when timer runs out to hide the information text again
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_Elapsed(object sender, EventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke((Action)(() => BorHinweis.Visibility = Visibility.Collapsed));
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
}