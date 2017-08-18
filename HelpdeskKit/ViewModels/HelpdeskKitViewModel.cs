using System.ComponentModel;
using System.Runtime.CompilerServices;
using HelpdeskKit.Annotations;
using HelpdeskKit.Views;
using HelpdeskKit.Views;
using HelpdeskKit.Views.ContentControls;
using System;
using HelpdeskKit.Commands;

namespace HelpdeskKit.ViewModels
{
    public partial class HelpdeskKitViewModel : INotifyPropertyChanged
    {
        public HelpdeskKitViewModel()
        {
            Init();
            InitItems();
            ShowLoginDialog();
        }

        private bool _searchFieldFocus;
        public bool SearchFieldFocus
        {
            get => _searchFieldFocus;
            set
            {
                _searchFieldFocus = value;
                OnPropertyChanged(nameof(SearchFieldFocus));
            }
        }

        public RelayCommand FocusSearchTextbox => new RelayCommand((o) =>
        {
            SearchFieldFocus = true;
        });


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
            var vm = (LoginDialogViewModel) context;
            //Ad should be instanciate otherwise should not have escaped login dialog
            _ad = vm.Ad ?? throw new InvalidProgramException();
            Username = vm.Username;
            Password = vm.Password;
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