using System;
using System.Threading;
using System.Threading.Tasks;
using HelpdeskKit.AD;
using HelpdeskKit.Commands;
using HelpdeskKit.Dialogs;

namespace HelpdeskKit.ViewModels
{
    public partial class HelpdeskKitViewModel
    {
        private bool _isLoggingIn;
        private string _loginStatusMessage;
        private string _password = string.Empty;
        private string _username = string.Empty;
        public RelayCommand LoginCommand => new RelayCommand(o => LoginMethod(), o => CanExecuteLogin);
        private bool _authenticated;

        public bool Authenticated
        {
            get => _authenticated;
            set
            {
                if (value == _authenticated) return;
                _authenticated = value;
                OnPropertyChanged(nameof(Authenticated));
            }
        }
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

        public bool IsLoggingIn
        {
            get => _isLoggingIn;
            set
            {
                _isLoggingIn = value;
                OnPropertyChanged(nameof(IsLoggingIn));
            }
        }

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
        private void ShowLoginDialog()
        {
            DialogContent = new LoginDialog();
            ShowDialog = true;
        }

        private async void LoginMethod()
        {
            if (IsLoggingIn || Authenticated) return;
            try
            {
                IsLoggingIn = true;
                LoginStatusMessage = string.Empty;
                if (await Task.Run(new Func<bool>(_login)).ConfigureAwait(false))
                {
                    Authenticated = true;
                    if (!(DialogContent is LoginDialog)) return;
                    //hide dialog
                    ShowDialog = false;
                    //remove from host
                    //DialogContent = null;
                    return;
                }
                LoginStatusMessage = "Login failed.";
            }
            finally
            {
                IsLoggingIn = false;
            }
        }

        private bool _login()
        {
            try
            {
                //simulation
#if DEBUG
                Thread.Sleep(1000);
                //mock ad controler
                return true;
#endif
                _controller = new AdController(Username, Password);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}