using System.ComponentModel;
using System.DirectoryServices;
using System.Runtime.CompilerServices;
using HelpdeskKit.AD;
using HelpdeskKit.Annotations;
using HelpdeskKit.Dialogs;
using HelpdeskKit.Views;
using HelpdeskKit.Views.ContentControls;

namespace HelpdeskKit.ViewModels
{
    public partial class HelpdeskKitViewModel : INotifyPropertyChanged
    {

     
        public HelpdeskKitViewModel()
        {
            _ad = new MockMyAd();
            Init();
            InitItems();
            ShowLoginDialog();
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