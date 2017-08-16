using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using HelpdeskKit.Annotations;
using HelpdeskKit.Views;
using HelpdeskKit.Commands;

namespace HelpdeskKit.ViewModels
{
    public class InputDialogViewModel : ICloseable
    {
        public RelayCommand OkCommand { get; set; }
        public RelayCommand CancelCommand { get; set; }

        public bool Result { get; set; }
        private  Action<object> _continueWith;
        public string InputLabel { get; set; }
        public string Input { get; private set; }
        public bool AllowEmpty { get; private set; }
        public InputDialogViewModel(string inputLabel, bool allowEmptyInput, Action<object> continueWith)
        {
            AllowEmpty = allowEmptyInput;
            OkCommand = new RelayCommand(OkMethod, CanExecuteOk);
            CancelCommand = new RelayCommand((o) => CancelMethod());
            InputLabel = inputLabel;
            _continueWith = continueWith;
        }

        public bool CanExecuteOk(object param)
        {
            if (AllowEmpty) return true;
            return !string.IsNullOrEmpty((string)param);
        }

        private void OkMethod(object param)
        {
            Input = (string) param;
            Result = true;
            InvokeRequestCloseDialog();
        }
        private void CancelMethod()
        {
            Result = false;
            InvokeRequestCloseDialog();
        }

        private void InvokeRequestCloseDialog()
        {
            RequestCloseEventArgs?.Invoke(this, new RequestCloseEventArgs());
            _continueWith.Invoke(this);
        }
        public event EventHandler<RequestCloseEventArgs> RequestCloseEventArgs;
    }
}
