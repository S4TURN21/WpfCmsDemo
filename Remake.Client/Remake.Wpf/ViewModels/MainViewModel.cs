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
    public class MainViewModel : ViewModelBase
    {
        public INavigationService Navigation { get; }
        public IDialogService DialogService { get; }
        public IMessageService MessageService { get; }
        public AuthService AuthService { get; }

        public ICommand NavigateToProjectsCommand { get; }
        public ICommand NavigateToTasksCommand { get; }
        public ICommand NavigateToCalculationsCommand { get; }
        public ICommand NavigateToEmployeesCommand { get; }
        public ICommand LogoutCommand { get; }

        public MainViewModel(AuthService authService, INavigationService navigation, IDialogService dialogService, IMessageService messageService)
        {
            AuthService = authService;
            Navigation = navigation;
            DialogService = dialogService;
            MessageService = messageService;

            Navigation.NavigateTo<EmployeesViewModel>();

            NavigateToProjectsCommand = CreateNavigationCommand<ProjectsViewModel>();
            NavigateToTasksCommand = CreateNavigationCommand<TasksViewModel>();
            NavigateToCalculationsCommand = CreateNavigationCommand<CalculationsViewModel>();
            NavigateToEmployeesCommand = CreateNavigationCommand<EmployeesViewModel>();

            LogoutCommand = new RelayCommand(Logout);
        }

        private ICommand CreateNavigationCommand<T>() where T : ViewModelBase
        {
            return new RelayCommand(() => Navigation.NavigateTo<T>(), () => Navigation.CurrentView is not T);
        }

        private void Logout()
        {
            AuthService.Logout();
        }
    }
}
