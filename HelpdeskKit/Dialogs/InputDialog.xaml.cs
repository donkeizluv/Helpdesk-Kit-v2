using HelpdeskKit.Commands;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace HelpdeskKit.Dialogs
{
    /// <summary>
    ///     Interaction logic for InputDialog.xaml
    /// </summary>
    public partial class InputDialog : UserControl
    {
        public RelayCommand CancelCommand;
        public InputDialog(string inputLabel)
        {
            InitializeComponent();
            InputLabel = inputLabel;
            DataContext = this;
        }
        public string InputLabel { get; set; }
        public string Input { get; set; }
    }
}