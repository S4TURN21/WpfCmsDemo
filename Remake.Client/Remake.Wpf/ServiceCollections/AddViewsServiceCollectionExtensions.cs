using Microsoft.Extensions.DependencyInjection;
using Remake.Wpf.Services;
using Remake.Wpf.Views;

namespace Remake.Wpf.ServiceCollections
{
    public static class AddViewsServiceCollectionExtensions
    {
        public static void AddViews(this IServiceCollection services)
        {
            //services.AddSingleton(provider => new MainWindow
            //{
            //    DataContext = provider.GetRequiredService<MainViewModel>()
            //});

            //services.AddSingleton(provider => new LoginWindow
            //{
            //    DataContext = provider.GetRequiredService<LoginViewModel>()
            //});
            services.AddSingleton<WindowMapper>();
            services.AddSingleton<IWindowManager, WindowManager>();
        }
    }
}
