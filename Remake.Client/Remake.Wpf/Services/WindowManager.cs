using Remake.Wpf.ViewModels;
using Remake.Wpf.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Remake.Wpf.Services
{
    public interface IWindowManager
    {
        Window CreateWindow(ViewModelBase viewModel);
        //Window GetOrCreateWindow(ViewModelBase viewModel);
        void CloseWindow();
        Window? GetWindowForViewModel(ViewModelBase viewModel);
    }

    public class WindowManager : IWindowManager
    {
        private readonly Dictionary<ViewModelBase, Window> _mappings;
        private readonly WindowMapper _windowMapper;

        public WindowManager(WindowMapper windowMapper)
        {
            _mappings = new Dictionary<ViewModelBase, Window>();

            _windowMapper = windowMapper;
        }

        public Window? GetWindowForViewModel(ViewModelBase viewModel)
        {
            if (_mappings.TryGetValue(viewModel, out var window))
            {
                return window;
            }

            return null;
        }

        public Window CreateWindow(ViewModelBase viewModel)
        {
            var windowType = _windowMapper.GetWindowTypeForViewModel(viewModel.GetType());

            if (windowType == null)
            {
                throw new NotImplementedException($"Couldn't find mapping for {windowType}");
            }

            var window = Activator.CreateInstance(windowType) as Window;
            window.DataContext = viewModel;
            window.Closed += (o, e) => OnClosed(viewModel);

            _mappings[viewModel] = window;

            return window;
        }

        private void OnClosed(ViewModelBase viewModel)
        {
            _mappings.Remove(viewModel);
            var isLoaded = _mappings.Values.Any(window => window.IsLoaded);

            if (!isLoaded)
            {
                Application.Current.Shutdown();
            }
        }

        public void CloseWindow()
        {
            throw new NotImplementedException();
        }
    }
}
