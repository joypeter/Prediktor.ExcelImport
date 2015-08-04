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

    public class UpdateExcelDialogViewModel : NotificationObject
    {
        //private readonly IInteractionService _interactionService;

        public UpdateExcelDialogViewModel()
        {
            //_interactionService = interactionService;
            _isUseCurrentTime = false;
            _isAppendNewData = false;
        }

        private bool _isUseCurrentTime;
        public bool IsUseCurrentTime
        {
            get { return _isUseCurrentTime; }
            set
            {
                _isUseCurrentTime = value;
            }
        }

        private bool _isAppendNewData;
        public bool IsAppendNewData
        {
            get { return _isAppendNewData; }
            set
            {
                _isAppendNewData = value;
            }
        }
    }
}
