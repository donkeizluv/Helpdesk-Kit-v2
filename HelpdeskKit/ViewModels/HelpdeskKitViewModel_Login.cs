﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using HelpdeskKit.AD;
using HelpdeskKit.Commands;

namespace HelpdeskKit.ViewModels
{
    public partial class HelpdeskKitViewModel
    {
        public RelayCommand LoginCommand => new RelayCommand((o) => LoginMethod(), (o) => CanExecuteLogin);

        private string _username = string.Empty;
        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged(nameof(Username));
                //LoginCommand.OnCanExecuteChanged();
            }
        }

        private string _password = string.Empty;
        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
                //LoginCommand.OnCanExecuteChanged();
            }
        }
        private string _loginStatusMessage;

        public string LoginStatusMessage
        {
            get => _loginStatusMessage;
            set
            {
                _loginStatusMessage = value;
                OnPropertyChanged(nameof(LoginStatusMessage));
            }
        }

        public bool CanExecuteLogin => Username.Length > 0 && Password.Length > 0;

        private void LoginMethod()
        {
            try
            {
                _controller = new AdController(Username, Password);
                Authenticated = true;
            }
            catch (Exception e)
            {
                LoginStatusMessage = "Login failed.";
            }
        }
    }
}
