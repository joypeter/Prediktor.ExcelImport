using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.Events;
using Prediktor.Configuration.Definitions;
using Prediktor.Carbon.Configuration.ViewModels;
using Prediktor.Carbon.Configuration.Definitions.ViewModels;
using Prediktor.Carbon.Infrastructure.Definitions;
using Prediktor.Carbon.Configuration.Definitions.ModuleServices;

namespace Prediktor.ExcelImport
{
    public class SolutionExplorer2ViewModel : SolutionExplorerViewModel
    {
        private bool _hasSelection;
        public SolutionExplorer2ViewModel(IEventAggregator eventAggregator,
            ISolutionService solution,
            IServiceFactory serviceFactory,
            IServiceNodeFactory serviceNodeFactory,
            IInteractionService interactionService,
            IConnectDialogService connectDialogService,
            ISolutionState solutionState,
            IApplicationProperties applicationProperties,
            IExpansionPathService expansionPathService)
            : base(eventAggregator, solution, serviceFactory, serviceNodeFactory,
            interactionService, connectDialogService, solutionState, applicationProperties,
            expansionPathService
            )
        {

        }

        public bool HasSelection
        {
            get { return _hasSelection; }
            set 
            { 
                _hasSelection = value;
                RaisePropertyChanged(() => HasSelection);
            }
        }
    }
}
