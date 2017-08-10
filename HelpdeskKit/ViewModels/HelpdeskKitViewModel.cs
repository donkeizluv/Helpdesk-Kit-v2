using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HelpdeskKit.Views.ContentControls;

namespace HelpdeskKit.Views.ViewModels
{
    public class HelpdeskKitViewModel
    {
        public MenuItem[] MenuItems { get; set; }
        public HelpdeskKitViewModel()
        {
            InitItems();
        }
        private void InitItems()
        {
            MenuItems = new MenuItem[]
            {
                new MenuItem("Account", new GeneralPageControl()),
                new MenuItem("Automation", new AutomationPageControl())
            };
        }
    }
}
