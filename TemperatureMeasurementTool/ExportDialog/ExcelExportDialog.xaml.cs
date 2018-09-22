using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Nager.Date;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using TemperatureMeasurementTool.Properties;

namespace TemperatureMeasurementTool
{
    /// <summary>
    /// Interaktionslogik für ExportDialog.xaml
    /// </summary>
    public partial class ExportDialog : Window
    {
        public ExportDialog()
        {
            InitializeComponent();
            DtpMockupFromDate.SelectedDate = Settings.Default.RecentSinceDateTime;
            DtpMockupToDate.SelectedDate = Settings.Default.RecentTillDateTime;
            TxtDateipfad.Text = Path.GetDirectoryName(Settings.Default.ExcelFilePath).Replace("\\", "/") + "/" + Settings.Default.MockUpFileName;
            
        }

        /// <summary>
        /// Exportiert die Daten als Excel File
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnExcelExport_OnClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(DtpMockupFromDate.Text) || string.IsNullOrWhiteSpace(DtpMockupToDate.Text) || string.IsNullOrWhiteSpace(TxtDateipfad.Text)) return;
            using (ExcelPackage excel = new ExcelPackage())
            {
                if (DtpMockupToDate.SelectedDate == null || DtpMockupFromDate.SelectedDate == null) return;

                var calcYear = DtpMockupToDate.SelectedDate.Value.Year;
                while (calcYear >= DtpMockupFromDate.SelectedDate.Value.Year)
                {
                    excel.Workbook.Worksheets.Add(calcYear.ToString());
                    calcYear--;
                }

                // Target a worksheet
                //var worksheet = excel.Workbook.Worksheets[1]; //WHAT THE FUCK? WHY DOES THAT ARRAY STARTS WITH ONE?!?!?!?!?

                //Creates the Data
                foreach (var editWorksheet in excel.Workbook.Worksheets)
                {
                    WriteTheDate(editWorksheet);
                }


                var excelFile = new FileInfo(TxtDateipfad.Text);
                excel.SaveAs(excelFile);

                //Open ExcelFile if there is Excel installed
                bool isExcelInstalled = Type.GetTypeFromProgID("Excel.Application") != null ? true : false;
                if (isExcelInstalled)
                {
                    Process.Start(excelFile.ToString());
                }
            }
        }

        private void WriteTheDate(ExcelWorksheet excelWorksheet)
        {
            excelWorksheet.Cells["A1:G1"].LoadFromArrays(new List<string[]>
            {
                new[] {"Datum", "1. Zeit", "1. Temperatur", "Kürzel", "2. Zeit", "2.Temperatur", "Kürzel"}
            });
            // Cells args are first row, first col, last row, last col
            using (var rowRngHeader = excelWorksheet.Cells[1, 1, 1, 7])
            {
                rowRngHeader.Style.Font.Name = "Segoe UI";
                rowRngHeader.Style.Font.Bold = true;
                rowRngHeader.Style.Font.Size = 12;
                rowRngHeader.Style.Border.BorderAround(ExcelBorderStyle.Thin);
            }

            var rowCount = 2;
            var actDate = DateTime.Parse("01.01." + excelWorksheet.Name);


            var dateFrom = DateTime.Parse(DtpMockupFromDate.Text).Year;
            if (actDate.Year.ToString().Equals(dateFrom))
            {
                actDate = DateTime.Parse(DtpMockupFromDate.Text);
            }

            var rnd = new Random();
            while (actDate != DtpMockupToDate.SelectedDate.Value && actDate.Year.ToString() == excelWorksheet.Name)
            {
                if (!(actDate.DayOfWeek == DayOfWeek.Sunday || actDate.DayOfWeek == DayOfWeek.Saturday) && !DateSystem.IsOfficialPublicHolidayByCounty(actDate, CountryCode.DE, "DE-HE"))
                {
                    var firstassignedUser =
                        Settings.Default.AssignedUsersList[rnd.Next(0, Settings.Default.AssignedUsersList.Count)];
                    var secondassignedUser =
                        Settings.Default.AssignedUsersList[rnd.Next(0, Settings.Default.AssignedUsersList.Count)];

                    //Temperaturmessungen
                    var tempFromInt = Convert.ToInt32(Settings.Default.TemperatureLimitFrom);
                    var tempFromSecondDigit = Convert.ToInt32(Settings.Default.TemperatureLimitFrom.ToString().Substring(Settings.Default.TemperatureLimitFrom.ToString().Length - 1));
                    var tempToInt = Convert.ToInt32(Settings.Default.TemperatureLimitTo);
                    var tempToSecondDigit = Convert.ToInt32(Settings.Default.TemperatureLimitTo.ToString().Substring(Settings.Default.TemperatureLimitTo.ToString().Length - 1));

                    var firstrnd = rnd.Next(tempFromInt, tempToInt + 1);
                    var secondrnd = rnd.Next(tempFromInt, tempToInt + 1);
                    string firsttemperaturlimit = string.Empty;
                    string secondtemperaturlimit = string.Empty;

                    //Erste Temperaturmessung
                    if (firstrnd == tempToInt)
                    {
                        firsttemperaturlimit = firstrnd + "," +
                                               (tempToSecondDigit == 0 ? "0" : rnd.Next(0, tempToSecondDigit).ToString()) + "°";
                    }
                    else if (firstrnd < tempToInt && firstrnd > tempFromInt)
                    {
                        firsttemperaturlimit = firstrnd + "," + rnd.Next(0, 9) + "°";
                    }
                    else if (firstrnd == tempFromInt)
                    {
                        firsttemperaturlimit = firstrnd + "," +
                                               (tempFromSecondDigit == 0 ? "0" : rnd.Next(tempFromSecondDigit, 9).ToString()) + "°";
                    }

                    //Zweite Temperaturmessung
                    if (secondrnd == tempToInt)
                    {
                        secondtemperaturlimit = secondrnd + "," +
                                               (tempToSecondDigit == 0 ? "0" : rnd.Next(0, tempToSecondDigit).ToString()) + "°";
                    }
                    else if (secondrnd < tempToInt && secondrnd > tempFromInt)
                    {
                        secondtemperaturlimit = secondrnd + "," + rnd.Next(0, 9) + "°";
                    }
                    else if (secondrnd == tempFromInt)
                    {
                        secondtemperaturlimit = secondrnd + "," +
                                               (tempFromSecondDigit == 0 ? "0" : rnd.Next(tempFromSecondDigit, 9).ToString()) + "°";
                    }



                    const string firsttime = "7:30";
                    string secondtime;
                    switch (actDate.DayOfWeek)
                    {
                        case DayOfWeek.Monday:
                            secondtime = "17:00";
                            break;
                        case DayOfWeek.Tuesday:
                            secondtime = "12:00";
                            break;
                        case DayOfWeek.Wednesday:
                            secondtime = "12:00";
                            break;
                        case DayOfWeek.Thursday:
                            secondtime = "18:00";
                            break;
                        case DayOfWeek.Friday:
                            secondtime = "12:00";
                            break;
                        default:
                            secondtime = "12:00";
                            break;
                    }

                    excelWorksheet.Cells["A" + rowCount + ":G" + rowCount].LoadFromArrays(new List<object[]>()
                    {
                        new object[]
                        {
                            actDate.ToShortDateString(), firsttime, firsttemperaturlimit, firstassignedUser, secondtime,
                        secondtemperaturlimit, secondassignedUser
                        }
                    });
                }
                else
                {                                                           
                    excelWorksheet.Cells["A" + rowCount].Value = actDate.ToShortDateString();

                    string Text = string.Empty;
                    if(DateSystem.IsOfficialPublicHolidayByCounty(actDate, CountryCode.DE, "DE-HE"))
                    {
                        Text = "Feiertag ";
                    }
                    else if(actDate.DayOfWeek == DayOfWeek.Sunday || actDate.DayOfWeek == DayOfWeek.Saturday)
                    {
                        Text = "Wochenende";
                    }

                    excelWorksheet.Cells[$"B{rowCount}:G{rowCount}"].Merge = true;
                    excelWorksheet.Cells[$"B{rowCount}"].Value= Text;
                    excelWorksheet.Cells[$"B{rowCount}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }
                rowCount = ++rowCount;
                actDate = actDate.AddDays(1);
            }

                
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

        private void BtnClose_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void GridHeader_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}