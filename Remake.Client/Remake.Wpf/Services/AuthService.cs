using Remake.Wpf.ViewModels;
using Remake.Wpf.Views;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Windows;

namespace Remake.Wpf.Services
{
    public class AuthService
    {
        private readonly IApiService _apiService;
        private readonly ILocalStorage _localStorage;
        private readonly IWindowManager _windowManager;
        private readonly ViewModelLocator _viewModelLocator;

        public User? User { get; set; }

        public AuthService(IApiService apiService, ILocalStorage localStorage, IWindowManager windowManager, ViewModelLocator viewModelLocator)
        {
            _apiService = apiService;
            _localStorage = localStorage;
            _windowManager = windowManager;
            _viewModelLocator = viewModelLocator;
        }

        public async Task<AuthResponseDto?> LoginAsync(string email, string password)
        {
            var userAuthDto = new UserForAuthenticationDto
            {
                Email = email,
                Password = password
            };

            var response = await _apiService.LoginAsync(userAuthDto);

            _localStorage.SetItem("token", response?.Token ?? "");
            _localStorage.SetItem("refreshToken", response?.RefreshToken ?? "");

            User = await _apiService.UserInfoAsync();

            return response;
        }

        public void Logout()
        {
            _localStorage.SetItem("token", "");
            _localStorage.SetItem("refreshToken", "");

            if (_windowManager.GetWindowForViewModel(_viewModelLocator.MainViewModel) is MainWindow mainWindow)
            {
                var loginWindow = _windowManager.CreateWindow(_viewModelLocator.LoginViewModel);
                loginWindow.Show();
                mainWindow.Close();
            }
        }

        public bool IsTokenExpired(string token)
        {
            try
            {
                var jwtToken = new JwtSecurityToken(token);
                return jwtToken == null || jwtToken.ValidFrom > DateTime.UtcNow || jwtToken.ValidTo < DateTime.UtcNow;
            }
            catch { }

            return true;
        }

        public async Task<bool> TryRefreshingTokens(string? token)
        {
            string? refreshToken = _localStorage.GetItem("refreshToken");

            if (token == null || refreshToken == null)
            {
                return false;
            }

            var tokenRefreshDto = new TokenRefreshDto
            {
                AccessToken = token,
                RefreshToken = refreshToken
            };

            try
            {
                var response = await _apiService.RefreshAsync(tokenRefreshDto);
                _localStorage.SetItem("token", response?.Token ?? "");
                _localStorage.SetItem("refreshToken", response?.RefreshToken ?? "");

                User = await _apiService.UserInfoAsync();

                return true;
            }
            catch (ApiException)
            {
                return false;
            }
        }
    }
}
