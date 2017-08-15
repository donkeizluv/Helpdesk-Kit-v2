using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpdeskKit.Views
{
    public interface ICloseable
    {
        event EventHandler<RequestCloseEventArgs> RequestCloseEventArgs;
    }

    public class RequestCloseEventArgs : EventArgs
    {
        public RequestCloseEventArgs()
        {
        }
    }
}
