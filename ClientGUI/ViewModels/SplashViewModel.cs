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

        public SplashViewModel()
        {
            connection.Username = "user";
            connection.IP = "127.0.0.1";
        }

        public ICommand GoToMain
        {
            get
            {
                return _goToMain ?? (_goToMain = new RelayCommand(x =>
                {
                    Mediator.Notify("GoToMainScreen", connection);
                }));
            }
        }
    }
}
