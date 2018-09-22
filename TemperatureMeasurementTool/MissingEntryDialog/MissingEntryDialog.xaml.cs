using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using Nager.Date;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using TemperatureMeasurementTool.Properties;

namespace TemperatureMeasurementTool.MissingEntryDialog
{
    /// <summary>
    /// Interaktionslogik für ExportDialog.xaml
    /// </summary>
    public partial class MissingEntryDialog : Window
    {
        private string _previouseValueAbends;
        private string _previouseValueMorgens;

        public MissingEntryDialog()
        {
            InitializeComponent();
            
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

        internal void ShowMissingEntries()
        {
            foreach (string item in Settings.Default.MissingDateCollection)
            {
                LstMissingEntries.Items.Add(item);
            }
            LstMissingEntries.SelectedIndex = 0;
        }

        private void BtnTempDownMorgens_OnClick(object sender, RoutedEventArgs e)
        {
            var value = Convert.ToDecimal(TempInputMorgens.Text, new NumberFormatInfo() { NumberDecimalSeparator = "," });
            value = decimal.Subtract(value, new decimal(0.1));
            TempInputMorgens.Text = value.ToString(CultureInfo.CurrentCulture);
            CheckTemperatureInputMorgens(TempInputMorgens.Text);
        }

        private void BtnTempUpMorgens_OnClick(object sender, RoutedEventArgs e)
        {
            var value = Convert.ToDecimal(TempInputMorgens.Text, new NumberFormatInfo() { NumberDecimalSeparator = "," });
            value = decimal.Add(value, new decimal(0.1));
            TempInputMorgens.Text = value.ToString(CultureInfo.CurrentCulture);
            CheckTemperatureInputMorgens(TempInputMorgens.Text);
        }

        private void BtnTempDownAbends_OnClick(object sender, RoutedEventArgs e)
        {
            var value = Convert.ToDecimal(TempInputAbends.Text, new NumberFormatInfo() { NumberDecimalSeparator = "," });
            value = decimal.Subtract(value, new decimal(0.1));
            TempInputAbends.Text = value.ToString(CultureInfo.CurrentCulture);
            CheckTemperatureInputAbends(TempInputAbends.Text);
        }

        private void BtnTempUpAbends_OnClick(object sender, RoutedEventArgs e)
        {
            var value = Convert.ToDecimal(TempInputAbends.Text, new NumberFormatInfo() { NumberDecimalSeparator = "," });
            value = decimal.Add(value, new decimal(0.1));
            TempInputAbends.Text = value.ToString(CultureInfo.CurrentCulture);
            CheckTemperatureInputAbends(TempInputAbends.Text);
        }

        private void CheckTemperatureInputMorgens(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return;
            var tempFrom = Settings.Default.TemperatureLimitFrom;
            var tempTo = Settings.Default.TemperatureLimitTo;
            var value = Convert.ToDecimal(text, new NumberFormatInfo() { NumberDecimalSeparator = "," });
            if (value < tempFrom || value > tempTo)
            {
                borTempAbends.BorderBrush = Brushes.Red;
                TxtWarningAbends.Visibility = Visibility.Visible;
            }
            else
            {
                borTempAbends.BorderBrush = Brushes.Transparent;
                TxtWarningAbends.Visibility = Visibility.Hidden;
            }
        }
        
        private void CheckTemperatureInputAbends(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return;
            var tempFrom = Settings.Default.TemperatureLimitFrom;
            var tempTo = Settings.Default.TemperatureLimitTo;
            var value = Convert.ToDecimal(text, new NumberFormatInfo() { NumberDecimalSeparator = "," });
            if (value < tempFrom || value > tempTo)
            {
                borTempAbends.BorderBrush = Brushes.Red;
                TxtWarningAbends.Visibility = Visibility.Visible;
            }
            else
            {
                borTempAbends.BorderBrush = Brushes.Transparent;
                TxtWarningAbends.Visibility = Visibility.Hidden;
            }
        }

        private void LstMissingEntries_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        private void temperaturinputMorgens_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            _previouseValueMorgens = TempInputMorgens.Text;
        }

        private void temperaturinputAbends_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            _previouseValueAbends = TempInputAbends.Text;
        }


        private void TempInputMorgens_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!Regex.IsMatch(TempInputMorgens.Text, "^-?[0-9]?[0-9][.,]?[0-9]?$"))
            {
                TempInputMorgens.Text = _previouseValueMorgens;
                return;
            }
            CheckTemperatureInputMorgens(TempInputMorgens.Text);
        }


        private void TempInputAbends_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!Regex.IsMatch(TempInputAbends.Text, "^-?[0-9]?[0-9][.,]?[0-9]?$"))
            {
                TempInputAbends.Text = _previouseValueAbends;
                return;
            }
            CheckTemperatureInputAbends(TempInputAbends.Text);
        }

        private void CbxTakeVacation_Checked(object sender, RoutedEventArgs e)
        {
            var checkboxVacation = sender as CheckBox;
            bool IsVacation = checkboxVacation.IsChecked != null ? (bool)checkboxVacation.IsChecked : false;
            TimePickerMorgens.IsEnabled = !IsVacation;
            TemperaturPanelMorgens.IsEnabled = !IsVacation;
            TimePickerAbends.IsEnabled = !IsVacation;
            TemperaturPanelAbends.IsEnabled = !IsVacation;
        }
    }
}