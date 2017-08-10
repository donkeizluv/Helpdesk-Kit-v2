using System.Windows;

namespace HelpdeskKit.Dialogs
{
    /// <summary>
    /// Interaction logic for InputDialog.xaml
    /// </summary>
    public partial class InputDialog : Window
    {
        public string InputLabel { get; set; }
        public string Input { get; set; }
        public InputDialog(string inputLabel)
        {
            InitializeComponent();
            InputLabel = inputLabel;
            DataContext = this;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void Window_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.ImeProcessedKey == System.Windows.Input.Key.Escape)
            {
                Close();
            }
        }
    }
}
