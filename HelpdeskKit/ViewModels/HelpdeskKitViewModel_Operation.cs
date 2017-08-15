using HelpdeskKit.AD;
using HelpdeskKit.Commands;
using System;

namespace HelpdeskKit.ViewModels
{
    public partial class HelpdeskKitViewModel
    {
        private IActiveDirectory _ad;

        private User _user;
        public User CurrentUser
        {
            get => _user;
            set
            {
                _user = value;
                OnPropertyChanged(nameof(CurrentUser));
            }
        }

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

        public RelayCommand SearchAdCommand => new RelayCommand(SearchAdMethod);
        private void SearchAdMethod(object param)
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

        public RelayCommand UnlockCommand => new RelayCommand(UnlockMethod, (o)=>(o != null));
        private void UnlockMethod(object param)
        {
            if (param == null) throw new ArgumentNullException();
            if (param.GetType() != typeof(User)) throw new ArgumentException();
            _ad.Unlock(param as User);
            OnPropertyChanged(nameof(CurrentUser));
        }


    }
}