using ClientGUI.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientGUI.ViewModels
{
    public abstract class BaseViewModel : PropertyChangedViewModel
    {

        #region connectionInfo


        public Connection connection = new();

        public string Username
        {
            get
            {
                return connection.Username;
            }
            set
            {
                connection.Username = value;
            }
        }
        public string IP
        {
            get
            {
                return connection.IP;
            }
            set
            {
                connection.IP = value;
            }
        }

        #endregion

        
    }
}
