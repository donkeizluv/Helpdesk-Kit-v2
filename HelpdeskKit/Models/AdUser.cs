using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpdeskKit.Models
{
    public class AdUser
    {
        private string _ad;
        public string Ad
        {
            get { return _ad; }
            set { _ad = value; }
        }
        private bool _isLocked;

        public bool Lock
        {
            get { return _isLocked; }
            set { _isLocked = value; }
        }
        private DateTime _ExpireDate;

        public DateTime ExpireDate
        {
            get { return _ExpireDate; }
            set { _ExpireDate = value; }
        }
        private string _batch;

        public string Batch
        {
            get { return _batch; }
            set { _batch = value; }
        }
        private string _desc;

        public string Description
        {
            get { return _desc; }
            set { _desc = value; }
        }


    }
}
