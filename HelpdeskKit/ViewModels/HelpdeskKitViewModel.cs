using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.DirectoryServices;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using HelpdeskKit.AD;
using HelpdeskKit.Annotations;
using HelpdeskKit.Dialogs;
using HelpdeskKit.Views.ContentControls;
using HelpdeskKit.Views;

namespace HelpdeskKit.ViewModels
{
    public partial class HelpdeskKitViewModel : INotifyPropertyChanged
    {
        public object CurrentDialogContent
        {
            get => _dialogContent;
            set
            {
                //if (Equals(value, _dialogContent)) return;
                _dialogContent = value;
                OnPropertyChanged(nameof(CurrentDialogContent));
            }
        }

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

        public MenuItem[] MenuItems { get; set; }

        private AdController _controller;
        private DirectoryEntry _currentEntry;
        private object _dialogContent;
        private bool _authenticated = false;

        public HelpdeskKitViewModel()
        {
            CurrentDialogContent = new LoginDialog();
            InitItems();
        }

        private void InitItems()
        {
            MenuItems = new[]
            {
                new MenuItem("Account", new GeneralPageControl()),
                new MenuItem("Automation", new AutomationPageControl())
            };
        }
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
