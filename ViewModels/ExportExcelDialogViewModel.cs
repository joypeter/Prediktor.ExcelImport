using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using Prediktor.Carbon.Infrastructure.Definitions;
using System.Collections.ObjectModel;

namespace Prediktor.ExcelImport.ViewModels
{
    public class ExcelColumn
    {
        public string Name { get; set; }
        public int Col { get; set; }
    }

    public class ExportExcelDialogViewModel : NotificationObject
    {
        //private readonly IInteractionService _interactionService;

        public ExportExcelDialogViewModel(
            int startInColomn,
            bool isIncludeTimestamps,
            bool isTimestampsInFirstCol,
            bool isTimestampsInLocalZone,
            bool isQualityInSeperateCol)
        {
            //_interactionService = interactionService;
            _isIncludeTimestamps = isIncludeTimestamps;
            _isTimestampsInFirstCol = isTimestampsInFirstCol;
            _isTimestampsInLocalZone = isTimestampsInLocalZone;
            _isQualityInSeperateCol = isQualityInSeperateCol;

            if (_startInColumn == null)
            {
                _startInColumn = new ObservableCollection<ExcelColumn>();
                for (int i = 0; i < 26; i++)
                {
                    char a = (char)(i + 65);
                    ExcelColumn ec = new ExcelColumn() { Name = a.ToString(), Col = i + 1 };
                    StartInColumn.Add(ec);
                }
                _selectedStartInColumn = _startInColumn[startInColomn - 1];
            }
            //StartInColumn = new ObservableCollection<ExcelColumn>();
            //for (uint i = 0; i<26; i++)
            //{
            //    char a = (char)(i+65);
            //    ExcelColumn ec = new ExcelColumn(a.ToString(), i + 1);
            //    StartInColumn.Add(ec); 
            //}
        }

        private bool _isIncludeTimestamps;
        public bool IsIncludeTimestamps
        {
            get { return _isIncludeTimestamps; }
            set
            {
                _isIncludeTimestamps = value;
                if (!value)
                {
                    IsTimestampsInLocalZone = false;
                }
            }
        }

        private bool _isTimestampsInFirstCol;
        public bool IsTimestampsInFirstCol
        {
            get { return _isTimestampsInFirstCol; }
            set
            {
                _isTimestampsInFirstCol = value;
                //RaisePropertyChanged(() => _isTimestampsInFirstCol);
            }
        }

        private bool _isTimestampsInLocalZone;
        public bool IsTimestampsInLocalZone
        {
            get { return _isTimestampsInLocalZone; }
            set
            {
                _isTimestampsInLocalZone = value;
            }
        }

        private bool _isQualityInSeperateCol;
        public bool IsQuelityInSeperateCol
        {
            get { return _isQualityInSeperateCol; }
            set
            {
                _isQualityInSeperateCol = value;
            }
        }

        private ObservableCollection<ExcelColumn> _startInColumn;
        public ObservableCollection<ExcelColumn> StartInColumn
        {
            get
            {
                
                return _startInColumn;
            }
            private set
            {
                _startInColumn = value;
            }
        }

        private ExcelColumn _selectedStartInColumn = null;
        public ExcelColumn SelectedStartInColumn
        {
            get { return _selectedStartInColumn; }
            set
            {
                _selectedStartInColumn = value;
                //RaisePropertyChanged(() => _selectedStartInColumn);
            }
        }

        //private ObservableCollection<string> _startColumn;
        //public ObservableCollection<string> StartColumn
        //{
        //    get 
        //    {
        //        if (_startColumn == null)
        //        {
        //            for (int i = 0; i < 26; i++)
        //            {
        //                char a = (char)(i+65);
        //                _startColumn.Add(a.ToString());
        //            }
        //        }
        //        return _startColumn;
        //    }
        //    set
        //    {
        //        this._startColumn = value;
        //    }
        //}

        //private string _selectedStartColumn = null;
        //public string SelectedStartColumn
        //{
        //    get { return _selectedStartColumn; }
        //    set { 
        //        _selectedStartColumn = value;
        //        RaisePropertyChanged(() => _selectedStartInColumn);
        //    }
        //}
    }
}
