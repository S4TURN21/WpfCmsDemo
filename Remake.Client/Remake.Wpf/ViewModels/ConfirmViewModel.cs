using Remake.Wpf.Core;
using Remake.Wpf.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Remake.Wpf.ViewModels
{
    public class ConfirmViewModel : ViewModelBase
    {
        //public override string? Title { get; set; }
        public string? Description { get; set; }

        private readonly IDialogService _dialogService;

        public ICommand ConfirmCommand { get; }
        public ICommand CancelCommand { get; }

        public ConfirmViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;
            ConfirmCommand = new RelayCommand(Confirm);
            CancelCommand = new RelayCommand(Cancel);
        }

        private void Confirm()
        {
            _dialogService.Close(DialogResult.OK);
        }

        private void Cancel()
        {
            _dialogService.Close(DialogResult.Cancel);
        }
    }
}
