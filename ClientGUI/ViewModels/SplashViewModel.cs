using ClientGUI.Commands;
using ClientGUI.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ClientGUI.ViewModels
{


    public class SplashViewModel : BaseViewModel, IPageViewModel
    {
        private ICommand _goToMain;



        private string _username;
        private string _ip;


        public string Username
        {
            get
            {
                return _username;
            }
            set
            {
                _username = value;
                OnPropertyChanged("Username");
            }
        }
        public string IP
        {
            get
            {
                return _ip;
            }
            set
            {
                _ip = value;
                OnPropertyChanged("IP");
            }
        }

        public SplashViewModel()
        {
            Username = "user";
            IP = "127.0.0.1";
        }

        public ICommand GoToMain
        {
            get
            {
                Connection connection = new();
                connection.Username = Username;
                connection.IP = IP;

                return _goToMain ?? (_goToMain = new RelayCommand(x =>
                {
                    Mediator.Notify("GoToMainScreen", connection);
                }));
            }
        }
    }
}
