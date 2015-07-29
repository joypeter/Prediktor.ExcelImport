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
    public interface IExcelColumn
    {
        string Name { get; }
        uint Col { get; }
    }

    public class ExportExcelDialogViewModel : NotificationObject
    {
        //private readonly IInteractionService _interactionService;

        public ExportExcelDialogViewModel()
        {
            //_interactionService = interactionService;
            _isIncludeTimestamps = true;
            _isTimestampsInFirstCol = true;
        }

        private bool _isIncludeTimestamps;
        public bool IsIncludeTimestamps
        {
            get { return _isIncludeTimestamps; }
            set
            {
                _isIncludeTimestamps = value;
                RaisePropertyChanged(() => _isIncludeTimestamps);
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
                RaisePropertyChanged(() => _isTimestampsInFirstCol);
            }
        }

        private bool _isTimestampsInLocalZone;
        public bool IsTimestampsInLocalZone
        {
            get { return _isTimestampsInLocalZone; }
            set
            {
                _isTimestampsInLocalZone = value;
                RaisePropertyChanged(() => _isTimestampsInLocalZone);
            }
        }

        private bool _isQuelityInSeperateCol;
        public bool IsQuelityInSeperateCol
        {
            get { return _isQuelityInSeperateCol; }
            set
            {
                _isQuelityInSeperateCol = value;
                RaisePropertyChanged(() => _isQuelityInSeperateCol);
            }
        }

        public ObservableCollection<IExcelColumn> StartInColumn
        {
            get;
            private set;
        }

        private IExcelColumn _selectedStartInColumn = null;
        public IExcelColumn SelectedStartInColumn
        {
            get { return _selectedStartInColumn; }
            set
            {
                _selectedStartInColumn = value;
                RaisePropertyChanged(() => _selectedStartInColumn);
            }
        }
    }
}
