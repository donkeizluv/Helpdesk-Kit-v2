using System.Windows;
using System.Windows.Input;

namespace HelpdeskKit
{
    /// <summary>
    ///     Interaction logic for HelpdeskKitView.xaml
    /// </summary>
    public partial class HelpdeskKitView : Window
    {
        //public HelpdeskKitViewModel ViewModel { get; private set; }
        public HelpdeskKitView()
        {
            InitializeComponent();
        }

        private void MenuItemListBox_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MenuToggleButton.IsChecked = false;
        }
    }
}