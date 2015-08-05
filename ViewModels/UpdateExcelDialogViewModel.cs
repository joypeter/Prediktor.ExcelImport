using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using Prediktor.Carbon.Infrastructure.Definitions;
using System.Collections.ObjectModel;
using Prediktor.Utilities;
using Prediktor.Configuration.BaseTypes.Definitions;

namespace Prediktor.ExcelImport.ViewModels
{
    public class UpdateExcelDialogViewModel : NotificationObject
    {
        public UpdateExcelDialogViewModel(IResult<IHistoricalTime> endtime,
            bool isUseCurrentTime,
            bool isAppendNewData)
        {
            _isUseCurrentTime = isUseCurrentTime;
            _isAppendNewData = isAppendNewData;
            _newTime = DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss t\\M");

            //if (endtime.Value.IsRelativeTime)
            //    _newTime = DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss t\\M");
            //else
            //    _newTime = endtime.Value.AbsoluteTime.ToString("MM/dd/yyyy hh:mm:ss t\\M");
        }

        private string _newTime;
        public string NewTime
        {
            get { return _newTime; }
            set { _newTime = value; }
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
