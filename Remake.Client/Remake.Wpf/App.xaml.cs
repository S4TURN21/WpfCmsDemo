using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using Remake.Wpf.ServiceCollections;
using Remake.Wpf.Services;
using Remake.Wpf.Views;
using System.Windows;
using System.Windows.Media;

namespace Remake.Wpf
{
    public partial class App : Application
    {
        private readonly ServiceProvider _serviceProvider;

        public App()
        {
            ShutdownMode = ShutdownMode.OnExplicitShutdown;
            AppContext.SetSwitch("Switch.System.Windows.Controls.Text.UseAdornerForTextboxSelectionRendering", false);

            IServiceCollection services = new ServiceCollection();
            services.AddViews();
            services.AddViewModels();
            services.AddServices();
            services.AddAutoMapper(typeof(App));

            _serviceProvider = services.BuildServiceProvider();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            var authService = _serviceProvider.GetRequiredService<AuthService>();
            var windowManager = _serviceProvider.GetRequiredService<IWindowManager>();
            var viewModelLocator = _serviceProvider.GetRequiredService<ViewModelLocator>();

            if (await IsUserAuthenticated())
            {
                var mainWindow = windowManager.CreateWindow(viewModelLocator.MainViewModel);
                mainWindow.Show();
            }
            else
            {
                var loginWindow = (LoginWindow)windowManager.CreateWindow(viewModelLocator.LoginViewModel);
                loginWindow.Show();
            }

            base.OnStartup(e);
        }

        public async Task<bool> IsUserAuthenticated()
        {
            var _localStorage = _serviceProvider.GetRequiredService<ILocalStorage>();
            var _authService = _serviceProvider.GetRequiredService<AuthService>();
            var _apiService = _serviceProvider.GetRequiredService<IApiService>();

            string? token = _localStorage.GetItem("token");

            if (token != null && !_authService.IsTokenExpired(token))
            {
                _authService.User = await _apiService.UserInfoAsync();

                return true;
            }

            var isRefreshSuccess = await _authService.TryRefreshingTokens(token);

            return isRefreshSuccess;
        }

        private void Window_Mininmize(object sender, RoutedEventArgs e)
        {
            var window = GetParentWindow((DependencyObject)sender);
            if (window != null)
            {
                window.WindowState = WindowState.Minimized;
            }
        }
        private void Window_Maximize(object sender, RoutedEventArgs e)
        {
            var window = GetParentWindow((DependencyObject)sender);
            if (window != null)
            {
                window.WindowState = window.WindowState == WindowState.Normal ? WindowState.Maximized : WindowState.Normal;
            }
        }

        private void Window_Close(object sender, RoutedEventArgs e)
        {
            var window = GetParentWindow((DependencyObject)sender);
            if (window != null)
            {
                window.Close();
            }
        }

        private static Window? GetParentWindow(DependencyObject child)
        {
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            if (parentObject == null)
            {
                return null;
            }

            Window? parent = parentObject as Window;
            if (parent != null)
            {
                return parent;
            }
            else
            {
                return GetParentWindow(parentObject);
            }
        }
    }
}
