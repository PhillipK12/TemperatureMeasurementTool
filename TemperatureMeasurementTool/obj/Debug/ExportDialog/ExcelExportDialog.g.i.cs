﻿#pragma checksum "..\..\..\ExportDialog\ExcelExportDialog.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "BDA35D4CB7670BF351FE81165238445AA5889884"
//------------------------------------------------------------------------------
// <auto-generated>
//     Dieser Code wurde von einem Tool generiert.
//     Laufzeitversion:4.0.30319.42000
//
//     Änderungen an dieser Datei können falsches Verhalten verursachen und gehen verloren, wenn
//     der Code erneut generiert wird.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;
using TemperatureMeasurementTool;


namespace TemperatureMeasurementTool {
    
    
    /// <summary>
    /// ExportDialog
    /// </summary>
    public partial class ExportDialog : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 22 "..\..\..\ExportDialog\ExcelExportDialog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid GridHeader;
        
        #line default
        #line hidden
        
        
        #line 51 "..\..\..\ExportDialog\ExcelExportDialog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DatePicker DtpMockupFromDate;
        
        #line default
        #line hidden
        
        
        #line 53 "..\..\..\ExportDialog\ExcelExportDialog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DatePicker DtpMockupToDate;
        
        #line default
        #line hidden
        
        
        #line 57 "..\..\..\ExportDialog\ExcelExportDialog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox TxtDateipfad;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/TemperatureMeasurementTool;component/exportdialog/excelexportdialog.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\ExportDialog\ExcelExportDialog.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.GridHeader = ((System.Windows.Controls.Grid)(target));
            
            #line 22 "..\..\..\ExportDialog\ExcelExportDialog.xaml"
            this.GridHeader.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.GridHeader_OnMouseLeftButtonDown);
            
            #line default
            #line hidden
            return;
            case 2:
            
            #line 32 "..\..\..\ExportDialog\ExcelExportDialog.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.BtnClose_OnClick);
            
            #line default
            #line hidden
            
            #line 32 "..\..\..\ExportDialog\ExcelExportDialog.xaml"
            ((System.Windows.Controls.Button)(target)).MouseEnter += new System.Windows.Input.MouseEventHandler(this.UIElement_OnMouseEnter);
            
            #line default
            #line hidden
            
            #line 32 "..\..\..\ExportDialog\ExcelExportDialog.xaml"
            ((System.Windows.Controls.Button)(target)).MouseLeave += new System.Windows.Input.MouseEventHandler(this.UIElement_OnMouseLeave);
            
            #line default
            #line hidden
            return;
            case 3:
            this.DtpMockupFromDate = ((System.Windows.Controls.DatePicker)(target));
            return;
            case 4:
            this.DtpMockupToDate = ((System.Windows.Controls.DatePicker)(target));
            return;
            case 5:
            this.TxtDateipfad = ((System.Windows.Controls.TextBox)(target));
            return;
            case 6:
            
            #line 62 "..\..\..\ExportDialog\ExcelExportDialog.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.BtnExcelExport_OnClick);
            
            #line default
            #line hidden
            
            #line 62 "..\..\..\ExportDialog\ExcelExportDialog.xaml"
            ((System.Windows.Controls.Button)(target)).MouseEnter += new System.Windows.Input.MouseEventHandler(this.UIElement_OnMouseEnter);
            
            #line default
            #line hidden
            
            #line 62 "..\..\..\ExportDialog\ExcelExportDialog.xaml"
            ((System.Windows.Controls.Button)(target)).MouseLeave += new System.Windows.Input.MouseEventHandler(this.UIElement_OnMouseLeave);
            
            #line default
            #line hidden
            return;
            case 7:
            
            #line 66 "..\..\..\ExportDialog\ExcelExportDialog.xaml"
            ((System.Windows.Controls.TextBlock)(target)).MouseEnter += new System.Windows.Input.MouseEventHandler(this.UIElement_OnMouseEnter);
            
            #line default
            #line hidden
            
            #line 66 "..\..\..\ExportDialog\ExcelExportDialog.xaml"
            ((System.Windows.Controls.TextBlock)(target)).MouseLeave += new System.Windows.Input.MouseEventHandler(this.UIElement_OnMouseLeave);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

