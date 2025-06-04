using Remake.Wpf.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Remake.Wpf.Services
{
    public enum DialogResult
    {
        None,
        OK,
        Cancel
    }

    public class DialogResultArgs : EventArgs
    {
        public DialogResult? Result { get; set; }
    }

    public class Dialog
    {
        public event EventHandler<DialogResultArgs>? OnClose;

        public ViewModelBase CurrentView { get; }

        public Dialog(ViewModelBase currentView)
        {
            CurrentView = currentView;
        }

        public void Close(DialogResult result = DialogResult.OK)
        {
            OnClose?.Invoke(this, new DialogResultArgs() { Result = result });
        }
    }

    public interface IDialogService
    {
        public ViewModelBase? CurrentView { get; }

        void Close(DialogResult result = DialogResult.OK);
        Dialog Open<TViewModel>(Action<TViewModel>? config = null) where TViewModel : ViewModelBase;
        Dialog Open<TViewModel>(TViewModel viewModel) where TViewModel : ViewModelBase;
    }

    public class DialogService : IDialogService, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly Func<Type, ViewModelBase> _viewModelFactory;

        public ViewModelBase? CurrentView
        {
            get => currentView;
            set => currentView = value;
        }

        private Dialog? _dialog;
        private ViewModelBase? currentView;

        public DialogService(Func<Type, ViewModelBase> viewModelFactory)
        {
            _viewModelFactory = viewModelFactory;
        }

        public Dialog Open<TViewModel>(Action<TViewModel>? config = null) where TViewModel : ViewModelBase
        {
            TViewModel viewModel = (TViewModel)_viewModelFactory.Invoke(typeof(TViewModel));

            config?.Invoke(viewModel);

            var dialog = this.Open(viewModel);

            return dialog;
        }

        public Dialog Open<TViewModel>(TViewModel viewModel) where TViewModel : ViewModelBase
        {
            _dialog?.Close();

            CurrentView = viewModel;

            _dialog = new Dialog(viewModel);
            _dialog.OnClose += Dialog_OnClose;

            return _dialog;
        }

        public void Close(DialogResult result = DialogResult.OK)
        {
            _dialog?.Close(result);
        }

        private void Dialog_OnClose(object? sender, EventArgs e)
        {
            _dialog = null;
            CurrentView = null;
        }
    }
}
