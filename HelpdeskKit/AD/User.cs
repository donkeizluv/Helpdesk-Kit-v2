using System;

namespace HelpdeskKit.AD
{
    /// <summary>
    /// just for displaying
    /// </summary>
    public class User
    {
        //property changed cant be raised with specific prop (ex User.Ad)
        //every raise of Users' props changed trigger recheck for all props
        //TWICE or even more (vm fields, view's converters...)
        //seems okay for small app but could be a problem with big app or big datagrid
        public string Ad { get; set; }
        public bool Active { get; set; }
        public bool Lock { get; set; }

        public DateTime ExpireDate { get; set; }
        public bool IsExpired => DateTime.Compare(DateTime.Today, ExpireDate.Date) > 0;

        public string Batch { get; set; }

        public string Description { get; set; }

        public User()
        {
            
        }
    }
}