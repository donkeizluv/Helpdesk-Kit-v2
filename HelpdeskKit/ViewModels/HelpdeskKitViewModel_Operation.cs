using System.DirectoryServices;
using HelpdeskKit.AD;
using HelpdeskKit.Commands;
using System;

namespace HelpdeskKit.ViewModels
{
    public partial class HelpdeskKitViewModel
    {
        private IActiveDirectory _ad;

        private User _user;

        private bool _seachByAd;

        public bool SearchByAd
        {
            get => _seachByAd;
            set
            {
                _seachByAd = value;
                OnPropertyChanged(nameof(SearchByAd));

            }
        }

        public RelayCommand SearchAdCommand => new RelayCommand(SearchAdMethod, o => true);

        public void SearchAdMethod(object param)
        {
            if (param == null) return;
            if (param.GetType() != typeof(string))
                throw new ArgumentException("Param must be string");
            var search = param as string;
            if (search.Length < 1) return;
            if (SearchByAd)
            {
                if (_ad.SearchByUsername(search, out var user))
                {
                    CurrentUser = user;
                    return;
                }
                CurrentUser = null;
            }
            else
            {
                if (_ad.SearchByHr(search, out var user))
                {
                    CurrentUser = user;
                    return;
                }
                CurrentUser = null;
            }
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