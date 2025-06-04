using Prime.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Remake.Wpf.Services
{
    //public struct Message
    //{
    //    public string? Summary { get; set; }
    //    public string? Detail { get; set; }
    //}

    public interface IMessageService
    {
        public ObservableCollection<Message> Messages { get; }
        public void Add(Message message);
    }

    public class MessageService : IMessageService, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public ObservableCollection<Message> Messages { get; }

        public MessageService()
        {
            Messages = new ObservableCollection<Message>();
        }

        public void Add(Message message)
        {
            Messages.Add(message);
        }
    }
}
