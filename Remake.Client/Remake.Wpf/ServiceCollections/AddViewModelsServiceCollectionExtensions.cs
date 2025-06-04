using Microsoft.Extensions.DependencyInjection;
using Remake.Wpf.Services;
using Remake.Wpf.ViewModels;

namespace Remake.Wpf.ServiceCollections
{
    public static class AddViewModelsServiceCollectionExtensions
    {
        public static void AddViewModels(this IServiceCollection services)
        {
            services.AddSingleton<ViewModelLocator>();

            services.AddSingleton<Func<Type, ViewModelBase>>(serviceProvider => viewModelType =>
            {
                return (ViewModelBase)serviceProvider.GetRequiredService(viewModelType);
            });

            services.AddSingleton<MainViewModel>();
            services.AddTransient<LoginViewModel>();

            services.AddSingleton<EmployeesViewModel>();
            services.AddTransient<AddEmployeeViewModel>();
            services.AddTransient<EditEmployeeViewModel>();

            services.AddSingleton<ProjectsViewModel>();
            services.AddSingleton<TasksViewModel>();
            services.AddSingleton<CalculationsViewModel>();
            services.AddTransient<ConfirmViewModel>();
        }
    }
}
