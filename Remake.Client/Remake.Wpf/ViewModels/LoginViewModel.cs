using Remake.Wpf.Core;
using Remake.Wpf.Services;
using Remake.Wpf.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Net;
using System.Security;

namespace Remake.Wpf.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private readonly AuthService _authService;
        private readonly IWindowManager _windowManager;
        private readonly ViewModelLocator _viewModelLocator;

        [Required]
        public string? Email { get; set; }

        [Required]
        public string? Password { get; set; }

        public ICommand LoginCommand { get; }

        public string? Errors { get; set; }

        public bool IsLoading { get; set; }

        public LoginViewModel(AuthService authService, IWindowManager windowManager, ViewModelLocator viewModelLocator)
        {
            _authService = authService;
            _windowManager = windowManager;
            _viewModelLocator = viewModelLocator;

            LoginCommand = new RelayCommand(Login, () => !IsLoading);
        }

        private async void Login()
        {
            IsLoading = true;
            Errors = null;

            try
            {
                var response = await _authService.LoginAsync(Email, Password);

                //_authService.UpdateTokens(response);

                var mainWindow = _windowManager.GetWindowForViewModel(_viewModelLocator.MainViewModel);

                if (mainWindow == null)
                {
                    mainWindow = _windowManager.CreateWindow(_viewModelLocator.MainViewModel);
                }

                if (!mainWindow.IsLoaded || !mainWindow.IsVisible)
                {
                    mainWindow.Show();
                }


                if (_windowManager.GetWindowForViewModel(this) is LoginWindow loginWindow)
                {
                    try
                    {
                        loginWindow.DialogResult = true;
                    }
                    catch (InvalidOperationException)
                    {
                        loginWindow.Close();
                    }
                }
            }
            catch (ApiException<ServerMessage> ex)
            {
                Errors = ex.Result.Message;
                return;
            }
            catch (ApiException ex)
            {
                if ((HttpStatusCode)ex.StatusCode == HttpStatusCode.ServiceUnavailable)
                {
                    MessageBox.Show($"Время подключения к серверу истекло. Возможно, он отключен для обслуживания.\nПожалуйста, повторите попытку позже.",
                    "Невозможно подключиться к серверу",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                    return;
                }

                throw;
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
