﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18449
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Prediktor.ExcelImport.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "12.0.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("opchda://localhost/Prediktor.ApisOPCHDAServer.1")]
        public string LastUri {
            get {
                return ((string)(this["LastUri"]));
            }
            set {
                this["LastUri"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("pack://application:,,,/Prediktor.Carbon.Style;component/DarkTheme.xaml")]
        public string Theme {
            get {
                return ((string)(this["Theme"]));
            }
            set {
                this["Theme"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool DebugView {
            get {
                return ((bool)(this["DebugView"]));
            }
            set {
                this["DebugView"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool ResultView {
            get {
                return ((bool)(this["ResultView"]));
            }
            set {
                this["ResultView"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string CurrentFile {
            get {
                return ((string)(this["CurrentFile"]));
            }
            set {
                this["CurrentFile"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool HighPrecisionTime {
            get {
                return ((bool)(this["HighPrecisionTime"]));
            }
            set {
                this["HighPrecisionTime"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool SolutionExplorerView {
            get {
                return ((bool)(this["SolutionExplorerView"]));
            }
            set {
                this["SolutionExplorerView"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("100")]
        public uint ConnectionLimit {
            get {
                return ((uint)(this["ConnectionLimit"]));
            }
            set {
                this["ConnectionLimit"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool ConnectionView {
            get {
                return ((bool)(this["ConnectionView"]));
            }
            set {
                this["ConnectionView"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1")]
        public int StartInColum {
            get {
                return ((int)(this["StartInColum"]));
            }
            set {
                this["StartInColum"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool IncludeTimestamp {
            get {
                return ((bool)(this["IncludeTimestamp"]));
            }
            set {
                this["IncludeTimestamp"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool IncludeQuality {
            get {
                return ((bool)(this["IncludeQuality"]));
            }
            set {
                this["IncludeQuality"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool TimestampsInFirstCol {
            get {
                return ((bool)(this["TimestampsInFirstCol"]));
            }
            set {
                this["TimestampsInFirstCol"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool TimestampsInLocalZone {
            get {
                return ((bool)(this["TimestampsInLocalZone"]));
            }
            set {
                this["TimestampsInLocalZone"] = value;
            }
        }
    }
}
