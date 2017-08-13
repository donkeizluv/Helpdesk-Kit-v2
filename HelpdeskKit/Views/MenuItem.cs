using System.ComponentModel;

namespace HelpdeskKit.Views
{
    public class MenuItem : INotifyPropertyChanged
    {
        private object _content;

        private string _name;

        //private ScrollBarVisibility _horizontalScrollBarVisibilityRequirement;
        //private ScrollBarVisibility _verticalScrollBarVisibilityRequirement;
        //private Thickness _marginRequirement = new Thickness(16);
        public MenuItem(string name, object content)
        {
            _name = name;
            Content = content;
        }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                RaisePropertyChanged("Name");
            }
        }

        public object Content
        {
            get => _content;
            set
            {
                _content = value;
                RaisePropertyChanged("Content");
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}