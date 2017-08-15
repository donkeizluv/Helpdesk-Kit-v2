using System;

namespace HelpdeskKit.AD
{
    /// <summary>
    /// just for displaying
    /// </summary>
    public class User
    {
        public string Ad { get; set; }
        public bool Active { get; set; }
        public bool Lock { get; set; }

        public DateTime ExpireDate { get; set; }
        public bool IsExpired => DateTime.Compare(DateTime.Today, ExpireDate.Date) < 0;

        public string Batch { get; set; }

        public string Description { get; set; }

        public User()
        {
            
        }
    }
}