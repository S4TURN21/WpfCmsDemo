using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Remake.Wpf.ViewModels
{
    public abstract class ViewModelBase : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        public virtual string? Title { get; set; }

        public bool HasErrors => _errors.Any();

        private readonly Dictionary<string, List<string>> _errors = new Dictionary<string, List<string>>();

        public ViewModelBase()
        {
            PropertyChanged += (o, e) => ValidateProperty(e.PropertyName);
        }

        private void ValidateProperty(string? propertyName)
        {
            if (string.IsNullOrWhiteSpace(propertyName)) return;

            var prop = this.GetType().GetProperty(propertyName);

            if (prop is null) return;

            var value = prop.GetValue(this);
            var context = new ValidationContext(this)
            {
                MemberName = propertyName
            };
            var results = new List<ValidationResult>();

            if (Validator.TryValidateProperty(value, context, results))
            {
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            }
        }

        public IEnumerable GetErrors(string? propertyName)
        {
            if (propertyName != null && _errors.TryGetValue(propertyName, out var errors))
            {
                return errors;
            }

            return Enumerable.Empty<string>();
        }
    }
}
