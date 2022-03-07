using ClientGUI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientGUI.Models
{
    public class Connection : BaseViewModel
    {
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
    }
}
