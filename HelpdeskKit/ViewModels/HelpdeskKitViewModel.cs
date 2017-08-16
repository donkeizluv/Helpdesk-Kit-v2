using System.ComponentModel;
using System.Runtime.CompilerServices;
using HelpdeskKit.Annotations;
using HelpdeskKit.Views;
using HelpdeskKit.Views.ContentControls;
using System;
using HelpdeskKit.Commands;
using System.Windows.Threading;
using MaterialDesignThemes.Wpf;

namespace HelpdeskKit.ViewModels
{
    public partial class HelpdeskKitViewModel : INotifyPropertyChanged
    {
        public HelpdeskKitViewModel()
        {
            Init();
            InitItems();
            ShowLoginDialog();
            ShowSnackBar = false;
            InnitSnackBarTimer();
        }

        private void ShowLoginDialog()
        {
            var view = new LoginDialog();
            var vm = new LoginDialogViewModel(ContinuedLogin);
            view.DataContext = vm;
            vm.RequestCloseEventArgs += CloseDialog;
            DialogContent = view;
            ShowDialog = true;
        }


        private void CloseDialog(object sender, RequestCloseEventArgs e)
        {
            ShowDialog = false;
            //should i?
            //or should i cache all the dialogs?
            //since dialog gets opened not that often, maybe cache isnt needed
            var dialog = (ICloseable)sender;
            dialog.RequestCloseEventArgs -= CloseDialog;
        }

        internal string Username;
        internal string Password;

        private void ContinuedLogin(object context)
        {
            if (context.GetType() != typeof(LoginDialogViewModel)) throw new InvalidOperationException();
            var vm = (LoginDialogViewModel)context;
            //Ad should be instanciate otherwise should not have escaped login dialog
            _ad = vm.Ad ?? throw new InvalidProgramException();
            Username = vm.Username;
            Password = vm.Password;
        }

        private DispatcherTimer _snackbarTimer;
        //public SnackbarMessageQueue MainMessageQueue = new SnackbarMessageQueue(TimeSpan.FromSeconds(1.5));
        private void InnitSnackBarTimer()
        {
            _snackbarTimer = new DispatcherTimer();
            _snackbarTimer.Interval = TimeSpan.FromSeconds(1.7);
            _snackbarTimer.Tick += ((o, e) =>
            {
                ShowSnackBar = false;
                SnackBarMessage = string.Empty;
                _snackbarTimer.Stop();
            });
        }
        public void DisplaySnackBar(string content)
        {
            //MainMessageQueue.Enqueue(content);
            if (ShowSnackBar) //in case already shown, update mess then reset timer
            {
                SnackBarMessage = content;
                if (_snackbarTimer.IsEnabled) //reset timer
                {
                    _snackbarTimer.Stop();
                    _snackbarTimer.Start();
                }
                return;
            }
            SnackBarMessage = content;
            ShowSnackBar = true;
            _snackbarTimer.Start();
        }

        public RelayCommand CloseSnackBar => new RelayCommand(new Action<object>(o => { ShowSnackBar = false; }));
        private bool _showSnackbar;
        public bool ShowSnackBar
        {
            get => _showSnackbar;
            set
            {
                _showSnackbar = value;
                OnPropertyChanged(nameof(ShowSnackBar));
            }
        }

        private string _snackBarMess;
        public string SnackBarMessage
        {
            get => _snackBarMess;
            set
            {
                _snackBarMess = value;
                OnPropertyChanged(nameof(SnackBarMessage));
            }
        }

        private bool _showDialog;
        public bool ShowDialog
        {
            get => _showDialog;
            set
            {
                _showDialog = value;
                OnPropertyChanged(nameof(ShowDialog));

            }
        }

        private object _dialogContent;
        public object DialogContent
        {
            get => _dialogContent;
            set
            {
                //if (Equals(value, _dialogContent)) return;
                _dialogContent = value;
                OnPropertyChanged(nameof(DialogContent));
            }
        }

        private void Init()
        {
            //default search option
            SearchByAd = true;
        }

        public MenuItem[] MenuItems { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        private void InitItems()
        {
            MenuItems = new[]
            {
                new MenuItem("Account", new GeneralPageControl()),
                new MenuItem("Automation", new AutomationPageControl())
            };
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}