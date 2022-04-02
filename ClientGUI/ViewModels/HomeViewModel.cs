using ClientGUI.Commands;
using ClientGUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ClientGUI.ViewModels
{
    public class HomeViewModel : BaseViewModel, IPageViewModel
    {

        private ICommand _sendMessage;
        private string _message = "";
        private string _history = "";

        public HomeViewModel()
        {
            connection.client.SetAction(Listener);
            History += $"Connected to chat...";
        }

        private void Listener(string response)
        {
            History += $"\n[{string.Format("{0:HH:mm:ss tt}", DateTime.Now)}] " + response;
        }

        public string Message
        {
            get
            {
                return _message;
            }
            set
            {
                _message = value;
                OnPropertyChanged(nameof(Message));
            }
        }

        public string History
        {
            get
            {
                return _history;
            }
            set
            {
                _history = value;
                OnPropertyChanged(nameof(History));
            }
        }

        public ICommand SendMessage
        {
            get
            {
                    return _sendMessage ?? (_sendMessage = new RelayCommand(x =>
                    {
                        connection.client.SendMessage(_message);
                        History += Message != "" ? $"\n[{string.Format("{0:HH:mm:ss tt}", DateTime.Now)}] {Username}: {Message}" : "";
                        Message = "";
                    }));
            }
        }
    }
}
