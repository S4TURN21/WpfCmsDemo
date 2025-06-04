using AutoMapper;
using Prime.Wpf;
using Remake.Wpf.Core;
using Remake.Wpf.Services;
using System.Collections.ObjectModel;
using System.Net;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Remake.Wpf.ViewModels
{
    public class EmployeesViewModel : ViewModelBase
    {
        public override string? Title { get; set; } = "Сотрудники";

        private readonly IApiService _apiService;
        private readonly IDialogService _dialogService;
        private readonly IMapper _mapper;
        private readonly IMessageService _messageService;
        //private readonly IApiService2 _apiService2;
        public bool Loading { get; set; }
        public ObservableCollection<Employee> Employees { get; set; }

        public ICommand TestErrorCommand { get; }
        public ICommand TestSuccessCommand { get; }
        public ICommand AddEmployeeCommand { get; }
        public ICommand EditEmployeeCommand { get; }
        public ICommand DeleteEmployeeCommand { get; }

        public EmployeesViewModel(IApiService apiService, IDialogService dialogService, IMapper mapper, IMessageService messageService)
        {
            _apiService = apiService;
            _dialogService = dialogService;
            _mapper = mapper;
            _messageService = messageService;
            //_apiService2 = apiService2;

            Employees = new ObservableCollection<Employee>();

            AddEmployeeCommand = new RelayCommand(AddEmployee);
            EditEmployeeCommand = new RelayCommand<Guid>(EditEmployee);
            DeleteEmployeeCommand = new RelayCommand<Guid>(DeleteEmployee);
            TestErrorCommand = new RelayCommand(TestError);
            TestSuccessCommand = new RelayCommand(TestSuccess);

            ResetEmployees();
        }

        private async void TestSuccess()
        {
            var request = new ApiRequestBuilder(_apiService, _messageService).ShowSuccess("Получен список сотрудников!");
            var employees = await request.RunAsync(async (api) =>
            {
                var employees = await api.GetEmployeesAsync();
                return employees;
            });

            //if (employees != null)
            //{
            //    _messageService.Add(new()
            //    {
            //        Severity = Severity.Info,
            //        Summary = "Employees",
            //        Detail = $"Count: {employees.Count}",
            //        Sticky = true
            //    });
            //}
        }

        private async void TestError()
        {
            var request = new ApiRequestBuilder(_apiService, _messageService);

            await request.RunAsync(async (api) =>
            {
                await api.TestErrorAsync();
            });


            //try
            //{
            //    await _apiService.TestErrorAsync();

            //    _messageService.Add(new()
            //    {
            //        Severity = Severity.Success,
            //        Summary = "Успех!",
            //        Detail = $"Запрос успешно выполнен"
            //    });
            //}
            //catch (ApiException ex)
            //{
            //    _messageService.Add(new()
            //    {
            //        Severity = Severity.Danger,
            //        Summary = "Ошибка",
            //        Detail = $"Невозможно обработать запрос.\nHttpStatusCode {ex.StatusCode} ({(HttpStatusCode)ex.StatusCode})\n{ex.Data}",
            //        Sticky = true
            //    });
            //}
        }

        private void AddEmployee()
        {
            var dialog = _dialogService.Open<AddEmployeeViewModel>();

            dialog.OnClose += async (o, e) =>
            {
                if (e.Result == DialogResult.OK)
                {
                    await ResetEmployeesAsync();
                }
            };
        }

        private async void EditEmployee(Guid employeeId)
        {
            var employee = await _apiService.GetEmployeeByIdAsync(employeeId);

            if (employee == null) return;

            var dialog = _dialogService.Open<EditEmployeeViewModel>();
            var employeeViewModel = (EditEmployeeViewModel)dialog.CurrentView;
            _mapper.Map(employee, employeeViewModel);

            dialog.OnClose += async (o, e) =>
            {
                if (e.Result == DialogResult.OK)
                {
                    await ResetEmployeesAsync();
                }
            };
        }

        private void DeleteEmployee(Guid employeeId)
        {
            var dialog = _dialogService.Open<ConfirmViewModel>(viewModel =>
            {
                viewModel.Title = "Подтвердите удаление";
                viewModel.Description = "Вы действительно хотите удалить этого сотрудника?";
            });
            dialog.OnClose += async (o, e) =>
            {
                if (e.Result == DialogResult.OK)
                {
                    var request = new ApiRequestBuilder(_apiService, _messageService).ShowSuccess("Сотрудник удален!");
                    await request.RunAsync(async (api) =>
                    {
                        await api.DeleteEmployeeAsync(employeeId);
                        await ResetEmployeesAsync();
                    });
                }
            };
        }

        private async void ResetEmployees()
        {
            Loading = true;
            Employees.Clear();

            var request = new ApiRequestBuilder(_apiService, _messageService).ShowError();

            var employees = await request.RunAsync(async (api) =>
            {
                return await api.GetEmployeesAsync();
            });

            //var employees = await _apiService.GetEmployeesAsync();

            if (employees != null)
            {
                foreach (var employee in employees)
                {
                    Employees.Add(employee);
                }
                Loading = false;
            }
        }

        private async Task ResetEmployeesAsync()
        {
            Employees.Clear();

            var employees = await _apiService.GetEmployeesAsync();

            foreach (var employee in employees)
            {
                Employees.Add(employee);
            }
        }
    }
}
