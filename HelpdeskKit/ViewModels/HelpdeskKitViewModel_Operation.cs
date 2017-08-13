using System.DirectoryServices;
using HelpdeskKit.AD;
using HelpdeskKit.Commands;

namespace HelpdeskKit.ViewModels
{
    public partial class HelpdeskKitViewModel
    {
        private AdController _controller;

        private User _user;

        private string _searchString;

        public string SearchString
        {
            get => _searchString;
            set
            {
                _searchString = value;
                SearchAdCommand.OnCanExecuteChanged();
            }
        }

        public RelayCommand SearchAdCommand => new RelayCommand(o => SearchAdMethod(), o => CanExecuteSearchAd);

        public bool CanExecuteSearchAd => SearchString.Length > 0;

        public void SearchAdMethod()
        {

        }

        public User CurrentUser
        {
            get => _user;
            set
            {
                _user = value;
                OnPropertyChanged(nameof(CurrentUser));
            }
        }
    }
}