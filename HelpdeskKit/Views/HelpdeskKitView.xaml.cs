using HelpdeskKit.Views;
using HelpdeskKit.Views.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HelpdeskKit
{
    /// <summary>
    /// Interaction logic for HelpdeskKitView.xaml
    /// </summary>
    public partial class HelpdeskKitView : Window
    {
        public HelpdeskKitViewModel ViewModel { get; private set; } = new HelpdeskKitViewModel();
        public HelpdeskKitView()
        {
            Timeline.DesiredFrameRateProperty.OverrideMetadata(typeof(Timeline), new FrameworkPropertyMetadata { DefaultValue = 45 });
            InitializeComponent();
            this.DataContext = ViewModel;
        }
    }
}
