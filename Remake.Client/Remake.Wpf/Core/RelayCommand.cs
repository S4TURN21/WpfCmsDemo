using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Remake.Wpf.Core
{
    public class RelayCommand : ICommand
    {
        private Action execute;
        private Func<bool>? canExecute;
        private ICommand? logoutCommand;

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public RelayCommand(Action execute, Func<bool>? canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public RelayCommand(ICommand? logoutCommand)
        {
            this.logoutCommand = logoutCommand;
        }

        public bool CanExecute(object? parameter)
        {
            return this.canExecute == null || this.canExecute();
        }

        public void Execute(object? parameter)
        {
            this.execute();
        }
    }

    public class RelayCommand<T> : ICommand
    {
        private Action<T?> execute;
        private Func<T?, bool>? canExecute;

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public RelayCommand(Action<T?> execute, Func<T?, bool>? canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public bool CanExecute(object? parameter)
        {
            return this.canExecute == null || this.canExecute((T?)parameter);
        }

        public void Execute(object? parameter)
        {
            execute((T?)parameter);
        }
    }
}
