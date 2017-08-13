using System.Windows;
using System.Windows.Input;

namespace HelpdeskKit.Dialogs
{
    /// <summary>
    ///     Interaction logic for InputDialog.xaml
    /// </summary>
    public partial class InputDialog : Window
    {
        public InputDialog(string inputLabel)
        {
            InitializeComponent();
            InputLabel = inputLabel;
            DataContext = this;
        }

        public string InputLabel { get; set; }
        public string Input { get; set; }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.ImeProcessedKey == Key.Escape)
                Close();
        }
    }
}