using Prime.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace Remake.Wpf.Services
{
    public class ApiRequestBuilder
    {
        private readonly IApiService _apiService;
        private readonly IMessageService _messageService;

        private bool _showSuccessMessage;
        private string? _successDetail;
        private Message? _successMessage;

        private bool _showErrorMessage = true;
        private string? _errorDetail;
        private Message? _errorMessage;

        public ApiRequestBuilder(IApiService apiService, IMessageService messageService)
        {
            _apiService = apiService;
            _messageService = messageService;
        }

        public ApiRequestBuilder ShowSuccess(string? detail = null)
        {
            this._showSuccessMessage = true;
            this._successDetail = detail;

            return this;
        }

        public ApiRequestBuilder ShowSuccess(Message message)
        {
            this._showSuccessMessage = true;
            this._successMessage = message;

            return this;
        }

        public ApiRequestBuilder ShowError(string? detail = null)
        {
            this._showErrorMessage = true;
            this._errorDetail = detail;

            return this;
        }

        public ApiRequestBuilder ShowError(Message message)
        {
            this._showErrorMessage = true;
            this._errorMessage = message;

            return this;
        }

        public async Task<TResult?> RunAsync<TResult>(Func<IApiService, Task<TResult>> callback)
        {
            try
            {
                var result = await callback.Invoke(_apiService);

                if (this._showSuccessMessage)
                {
                    ShowSuccess();
                }

                return result;
            }
            catch (ApiException)
            {
                if (this._showErrorMessage)
                {
                    ShowError();
                }

                return default;
            }
        }

        public async Task RunAsync(Func<IApiService, Task> callback)
        {
            try
            {
                await callback.Invoke(_apiService);

                if (this._showSuccessMessage)
                {
                    ShowSuccess();
                }
            }
            catch (ApiException)
            {
                if (this._showErrorMessage)
                {
                    ShowError();
                }
            }
        }

        private void ShowSuccess()
        {
            _messageService.Add(this._successMessage ?? new()
            {
                Severity = Severity.Success,
                Summary = "Успешно",
                Detail = this._successDetail ?? "Операция выполнена успешно",
                Sticky = false
            });
        }

        private void ShowError()
        {
            _messageService.Add(this._errorMessage ?? new()
            {
                Severity = Severity.Danger,
                Summary = "Ошибка",
                Detail = this._errorDetail ?? "Возникла ошибка при выполнении запроса",
                Sticky = true
            });
        }
    }
}
