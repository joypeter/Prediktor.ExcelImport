﻿using Prediktor.Carbon.Configuration.Definitions.ModuleServices;

namespace Prediktor.ExcelImport
{
    public class ApplicationProperties : IApplicationProperties
    {
        public string Theme
        {
            get
            {
                return Properties.Settings.Default.Theme;
            }
            set
            {
                Properties.Settings.Default.Theme = value;
            }
        }

        public bool HighPrecisionTimeEnabled
        {
            get
            {
                return Properties.Settings.Default.HighPrecisionTime;
            }
            set
            {
                Properties.Settings.Default.HighPrecisionTime = value;
            }
        }

        public bool IsConnectionViewVisible
        {
            get
            {
                return Properties.Settings.Default.ConnectionView;
            }
            set
            {
                Properties.Settings.Default.ConnectionView = value;
            }
        }

        public bool IsDebugViewVisible
        {
            get
            {
                return Properties.Settings.Default.DebugView;
            }
            set
            {
                Properties.Settings.Default.DebugView = value;
            }
        }

        public bool IsResultViewVisible
        {
            get
            {
                return Properties.Settings.Default.ResultView;
            }
            set
            {
                Properties.Settings.Default.ResultView = value;
            }
        }


        public string CurrentFile
        {
            get
            {
                return Properties.Settings.Default.CurrentFile;
            }
            set
            {
                Properties.Settings.Default.CurrentFile = value;
            }
        }

        public void Save()
        {
            Properties.Settings.Default.Save();
        }


        public bool IsSolutionExplorerVisible
        {
            get
            {
                return Properties.Settings.Default.SolutionExplorerView;
            }
            set
            {
                Properties.Settings.Default.SolutionExplorerView = value;
            }
        }


        public uint ConnectionViewLimit
        {
            get
            {
                return Properties.Settings.Default.ConnectionLimit;
            }
            set
            {
                Properties.Settings.Default.ConnectionLimit = value;
            }
        }

        public string LastUri
        {
            get
            {
                return Properties.Settings.Default.LastUri;
            }
            set
            {
                Properties.Settings.Default.LastUri = value;
            }
        }
    }
}