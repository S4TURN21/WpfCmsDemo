using Remake.Wpf.ViewModels;
using System.ComponentModel;

namespace Remake.Wpf.Services
{
    public interface INavigationService
    {
        public ViewModelBase? CurrentView { get; }
        void NavigateTo<TViewModel>(Func<TViewModel, bool>? config = null) where TViewModel : ViewModelBase;
    }

    class NavigationService : INavigationService, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly Func<Type, ViewModelBase> _viewModelFactory;

        public ViewModelBase? CurrentView { get; private set; }

        public NavigationService(Func<Type, ViewModelBase> viewModelFactory)
        {
            _viewModelFactory = viewModelFactory;
        }

        public void NavigateTo<TViewModel>(Func<TViewModel, bool>? config = null) where TViewModel : ViewModelBase
        {
            TViewModel viewModel = (TViewModel)_viewModelFactory.Invoke(typeof(TViewModel));
            var cancel = config?.Invoke(viewModel);

            if (cancel is true) return;

            CurrentView = viewModel;
        }
    }
}
