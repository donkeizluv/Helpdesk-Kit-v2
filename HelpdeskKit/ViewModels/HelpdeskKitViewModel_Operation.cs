using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HelpdeskKit.Commands;

namespace HelpdeskKit.ViewModels
{
    public partial class HelpdeskKitViewModel
    {
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

        public RelayCommand SearchAdCommand => new RelayCommand((o) => SearchAdMethod(), (o) => CanExecuteSearchAd);

        public void SearchAdMethod()
        {
            
        }

        public bool CanExecuteSearchAd => SearchString.Length > 0;
    }
}
