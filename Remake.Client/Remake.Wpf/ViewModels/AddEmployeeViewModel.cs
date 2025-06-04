using AutoMapper;
using Microsoft.Win32;
using Remake.Wpf.Core;
using Remake.Wpf.Services;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace Remake.Wpf.ViewModels
{
    public class AddEmployeeViewModel : ViewModelBase
    {
        private readonly IApiService _apiService;
        private readonly IDialogService _dialogService;
        private readonly IMapper _mapper;
        private readonly IMessageService _messageService;

        //public Employee Employee { get; set; }

        //public byte[]? Image { get; set; }
        //public string? ImageName { get; set; } = "Файл не выбран";

        public string? LastName { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public byte[]? Image { get; set; }

        public ICommand FileUploadCommand { get; }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public AddEmployeeViewModel(IApiService apiService, IDialogService dialogService, IMapper mapper, IMessageService messageService)
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
                //ImageName = Path.GetFileNameWithoutExtension(fileName);
            }
        }

        private async void Save()
        {
            var request = new ApiRequestBuilder(_apiService, _messageService).ShowSuccess("Сотрудник добавлен!");

            var employee = await request.RunAsync(async (api) =>
            {
                var employee = _mapper.Map<Employee>(this);

                return await api.AddEmployeeAsync(employee);
            });

            _dialogService.Close();

            //try
            //{
            //    _ = await _apiService.AddEmployeeAsync(Employee);
            //    _dialogService.Close();
            //}
            //catch (ApiException ex)
            //{
            //    MessageBox.Show($"ApiException {ex.StatusCode}");
            //}
        }

        private void Cancel()
        {
            _dialogService.Close(DialogResult.Cancel);
        }
    }
}
