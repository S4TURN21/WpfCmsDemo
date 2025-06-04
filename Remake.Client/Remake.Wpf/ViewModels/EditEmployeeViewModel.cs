using AutoMapper;
using Microsoft.Win32;
using Remake.Wpf.Core;
using Remake.Wpf.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Remake.Wpf.ViewModels
{
    public class EditEmployeeViewModel : ViewModelBase
    {
        private readonly IApiService _apiService;
        private readonly IDialogService _dialogService;
        private readonly IMapper _mapper;
        private readonly IMessageService _messageService;

        //public Employee? Employee { get; init; }
        public Guid Id { get; set; }
        public string? LastName { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public byte[]? Image { get; set; }

        public ICommand FileUploadCommand { get; }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public EditEmployeeViewModel(IApiService apiService, IDialogService dialogService, IMapper mapper, IMessageService messageService)
        {
            _apiService = apiService;
            _dialogService = dialogService;
            _mapper = mapper;
            _messageService = messageService;

            //Employee = new Employee();

            FileUploadCommand = new RelayCommand(FileUpload);

            SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(Cancel);
        }

        private void FileUpload()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                var fileName = openFileDialog.FileName;

                Image = File.ReadAllBytes(fileName);
            }
        }

        //public async void SetEmployee(Guid id)
        //{
        //    var employee = await _apiService.GetEmployeeByIdAsync(id);

        //    if (employee != null)
        //    {
        //        Id = employee.Id;
        //        LastName = employee.LastName;
        //        FirstName = employee.FirstName;
        //        MiddleName = employee.MiddleName;
        //    }
        //}

        private async void Save()
        {
            //var employee = new Employee
            //{
            //    Id = Id,
            //    LastName = LastName,
            //    FirstName = FirstName,
            //    MiddleName = MiddleName,
            //};
            //_mapper.Map(this, Employee);
            //_ = await _apiService.UpdateEmployeeAsync(Employee);


            var request = new ApiRequestBuilder(_apiService, _messageService).ShowSuccess("Информация о сотруднике обновлена!");

            var employee = await request.RunAsync(async (api) =>
            {
                var employee = _mapper.Map<Employee>(this);

                return await api.UpdateEmployeeAsync(employee);
            });

            if (employee != null)
            {
                _dialogService.Close();
            }
        }

        private void Cancel()
        {
            _dialogService.Close(DialogResult.Cancel);
        }
    }
}
