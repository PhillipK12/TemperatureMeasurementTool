﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Dieser Code wurde von einem Tool generiert.
//     Laufzeitversion:4.0.30319.42000
//
//     Änderungen an dieser Datei können falsches Verhalten verursachen und gehen verloren, wenn
//     der Code erneut generiert wird.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TemperatureMeasurementTool.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "15.8.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string ExcelFilePath {
            get {
                return ((string)(this["ExcelFilePath"]));
            }
            set {
                this["ExcelFilePath"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Phillip.Kalusek@hotmail.de")]
        public string RecentMailadresse {
            get {
                return ((string)(this["RecentMailadresse"]));
            }
            set {
                this["RecentMailadresse"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("TemperatureMeasurement")]
        public string FileName {
            get {
                return ((string)(this["FileName"]));
            }
            set {
                this["FileName"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool IsCloseAfterSaveEnabled {
            get {
                return ((bool)(this["IsCloseAfterSaveEnabled"]));
            }
            set {
                this["IsCloseAfterSaveEnabled"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n<ArrayOfString xmlns:xsi=\"http://www.w3." +
            "org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n  <s" +
            "tring>MJ</string>\r\n  <string>ES</string>\r\n</ArrayOfString>")]
        public global::System.Collections.Specialized.StringCollection AssignedUsersList {
            get {
                return ((global::System.Collections.Specialized.StringCollection)(this["AssignedUsersList"]));
            }
            set {
                this["AssignedUsersList"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("3.4")]
        public decimal TemperatureLimitFrom {
            get {
                return ((decimal)(this["TemperatureLimitFrom"]));
            }
            set {
                this["TemperatureLimitFrom"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("4.0")]
        public decimal TemperatureLimitTo {
            get {
                return ((decimal)(this["TemperatureLimitTo"]));
            }
            set {
                this["TemperatureLimitTo"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool IsShuffleTimeMockupEnabled {
            get {
                return ((bool)(this["IsShuffleTimeMockupEnabled"]));
            }
            set {
                this["IsShuffleTimeMockupEnabled"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("2016-01-01")]
        public global::System.DateTime RecentSinceDateTime {
            get {
                return ((global::System.DateTime)(this["RecentSinceDateTime"]));
            }
            set {
                this["RecentSinceDateTime"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("2018-09-18")]
        public global::System.DateTime RecentTillDateTime {
            get {
                return ((global::System.DateTime)(this["RecentTillDateTime"]));
            }
            set {
                this["RecentTillDateTime"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0")]
        public int IndexRecentSelectedEmployee {
            get {
                return ((int)(this["IndexRecentSelectedEmployee"]));
            }
            set {
                this["IndexRecentSelectedEmployee"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("3.4")]
        public decimal RecentTemperatureLimit {
            get {
                return ((decimal)(this["RecentTemperatureLimit"]));
            }
            set {
                this["RecentTemperatureLimit"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool IsCloseAfterSaveSettings {
            get {
                return ((bool)(this["IsCloseAfterSaveSettings"]));
            }
            set {
                this["IsCloseAfterSaveSettings"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("TemperatureMeasurementMockupData.xlsx")]
        public string MockUpFileName {
            get {
                return ((string)(this["MockUpFileName"]));
            }
            set {
                this["MockUpFileName"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("2018-09-10")]
        public global::System.DateTime DateLastRecord {
            get {
                return ((global::System.DateTime)(this["DateLastRecord"]));
            }
            set {
                this["DateLastRecord"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::System.Collections.Specialized.StringCollection MissingDateCollection {
            get {
                return ((global::System.Collections.Specialized.StringCollection)(this["MissingDateCollection"]));
            }
            set {
                this["MissingDateCollection"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Phillip.Kalusek@hotmail.de")]
        public string MailTransmitterAdress {
            get {
                return ((string)(this["MailTransmitterAdress"]));
            }
            set {
                this["MailTransmitterAdress"] = value;
            }
        }
    }
}