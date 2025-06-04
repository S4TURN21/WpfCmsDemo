using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Prime.Wpf;
using Remake.Wpf.Services;
using Remake.Wpf.ViewModels;
using Remake.Wpf.Views;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Windows;

namespace Remake.Wpf.ServiceCollections
{
    public static partial class AddServicesServiceCollectionExtensions
    {
        public class ErrorHandler : DelegatingHandler
        {
            private readonly IMessageService _messageService;

            public ErrorHandler(IMessageService messageService)
            {
                InnerHandler = new HttpClientHandler();
                _messageService = messageService;
            }

            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                try
                {
                    return await base.SendAsync(request, cancellationToken);
                }
                catch (HttpRequestException ex)
                {
                    //switch (ex.HttpRequestError)
                    //{
                    //    case HttpRequestError.ConnectionError:
                    //        MessageBox.Show($"Время подключения к серверу истекло. Возможно, он отключен для обслуживания.\nПожалуйста, повторите попытку позже.",
                    //                        "Невозможно подключиться к серверу",
                    //                        MessageBoxButton.OK,
                    //                        MessageBoxImage.Error);
                    //        break;
                    //}

                    return new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)
                    {
                        ReasonPhrase = "Ошибка подключения",
                        RequestMessage = request,
                        Content = new StringContent(ex.Message)
                    };

                    //throw;
                }

                var response = await base.SendAsync(request, cancellationToken);

                //_messageService.Add(new Message
                //{
                //    Summary = $"Status Code: {response.StatusCode}",
                //    Detail = "Cock"
                //});
                return response;
            }
        }

        public class TokenRefresher : DelegatingHandler
        {
            private readonly ILocalStorage _localStorage;
            private readonly IWindowManager _windowManager;
            private readonly ViewModelLocator _viewModelLocator;

            public TokenRefresher(ErrorHandler errorHandler, ILocalStorage localStorage, IWindowManager windowManager, ViewModelLocator viewModelLocator)
            {
                InnerHandler = errorHandler;
                _localStorage = localStorage;
                _windowManager = windowManager;
                _viewModelLocator = viewModelLocator;
            }

            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                string[] ignored = ["/api/accounts/login", "/api/token/refresh", "/api/accounts/userinfo"];

                if (request.RequestUri != null && ignored.Contains(request.RequestUri.LocalPath.ToLower()))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _localStorage.GetItem("token"));
                    return await base.SendAsync(request, cancellationToken);
                }

                //var refresh = await ((App)Application.Current).TryRefresh();
                if (!await ((App)Application.Current).IsUserAuthenticated())
                {
                    var loginWindow = (LoginWindow)_windowManager.CreateWindow(_viewModelLocator.LoginViewModel);
                    if (loginWindow.ShowDialog() != true)
                    {
                        Application.Current.Shutdown();
                    }
                }

                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _localStorage.GetItem("token"));
                return await base.SendAsync(request, cancellationToken);
            }
        }

        public static void AddServices(this IServiceCollection services)
        {
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<IDialogService, DialogService>();
            services.AddSingleton<IMessageService, MessageService>();
            services.AddSingleton<ILocalStorage, LocalStorage>();

            services.AddSingleton(provider =>
            {
                return new HttpClient(provider.GetRequiredService<TokenRefresher>());
            });
            services.AddSingleton<IApiService>(provider => new ApiService("https://localhost:7092/", provider.GetRequiredService<HttpClient>()));
            services.AddSingleton<AuthService>();
            services.AddSingleton<TokenRefresher>();
            services.AddSingleton<ErrorHandler>();
        }
    }
}
