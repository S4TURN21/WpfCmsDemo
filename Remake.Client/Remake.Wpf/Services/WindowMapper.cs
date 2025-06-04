using Remake.Wpf.ViewModels;
using Remake.Wpf.Views;
using System.Windows;

namespace Remake.Wpf.Services
{
    public class WindowMapper
    {
        private readonly Dictionary<Type, Type> _mappings;

        public WindowMapper()
        {
            _mappings = new Dictionary<Type, Type>();

            RegisterMapping<MainViewModel, MainWindow>();
            RegisterMapping<LoginViewModel, LoginWindow>();
        }

        public void RegisterMapping<TViewModel, TWindow>() where TViewModel : ViewModelBase where TWindow : Window
        {
            _mappings[typeof(TViewModel)] = typeof(TWindow);
        }

        public Type? GetWindowTypeForViewModel(Type viewModelType)
        {
            _mappings.TryGetValue(viewModelType, out Type? windowType);
            return windowType;
        }
    }
}
