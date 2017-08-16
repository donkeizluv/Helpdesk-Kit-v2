using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using HelpdeskKit.Annotations;
using HelpdeskKit.Commands;
using HelpdeskKit.AD;
using HelpdeskKit.Views;

namespace HelpdeskKit.ViewModels
{
    public class LoginDialogViewModel : INotifyPropertyChanged, ICloseable
    {
        public IActiveDirectory Ad { get; private set; }
        private bool _isLoggingIn;
        private string _loginStatusMessage;
        private string _password;
        private string _username;
        public RelayCommand LoginCommand => new RelayCommand(o => LoginMethod(), o => CanExecuteLogin);
        private Action<object> _continueWith;
        public LoginDialogViewModel(Action<object> continueWith)
        {
            //continue execution after request close this dialog
            _continueWith = continueWith;
        }
        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged(nameof(Username));
                LoginStatusMessage = string.Empty;
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
                LoginStatusMessage = string.Empty;
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

        public bool CanExecuteLogin => !string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password);


        private async void LoginMethod()
        {
            if (IsLoggingIn) return;
            try
            {
                IsLoggingIn = true;
                LoginStatusMessage = string.Empty;
                if (await Task.Run(new Func<bool>(_login)).ConfigureAwait(false))
                {
                    InvokeRequestCloseDialog(new RequestCloseEventArgs());
                    //done this dialog's part
                    //continue with parent's context method
                    _continueWith?.Invoke(this);
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
                //TODO: use ninject
                Ad = new MockMyAd();
                return Ad.Authenticate(Username, Password);
            }
            catch (Exception e)
            {
                return false;
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private void InvokeRequestCloseDialog(RequestCloseEventArgs e)
        {
            RequestCloseEventArgs?.Invoke(this, e);
        }
        public event EventHandler<RequestCloseEventArgs> RequestCloseEventArgs;
    }
}
