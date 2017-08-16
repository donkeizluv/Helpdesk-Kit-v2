using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HelpdeskKit.AD
{
    public class MockMyAd : IActiveDirectory
    {
        public MockMyAd()
        {
            
        }
        public bool Authenticate(string username, string pwd)
        {
            //mock login ad
            Thread.Sleep(1000);
            return string.Compare(username, "admin", true) == 0 && string.Compare(pwd, "admin", true) == 0;
        }

        public bool SearchByUsername(string username, out User user)
        {
            if (username.Contains("foo"))
            {
                user = new User()
                {
                    Ad = "foo.me",
                    Batch = string.Empty,
                    Description = "foobar!",
                    ExpireDate = DateTime.Today,
                    Lock = true,
                    Active = false,

                };
                return true;
            }
            if (username.Contains("bar"))
            {
                user = new User()
                {
                    Ad = "bar.you",
                    Batch = string.Empty,
                    Description = "bar!",
                    ExpireDate = DateTime.Today,
                    Lock = false,
                    Active = true,

                };
                return true;
            }
            user = null;
            return false;
        }

        public bool SearchByHr(string hr, out User user)
        {
            user = null;
            return false;
        }

        public void Unlock(User user)
        {
            user.Lock = false;
        }

        public void ChangePassword(User user, string pwd)
        {
            user.ExpireDate = user.ExpireDate.AddDays(45);
        }

        public void ChangeBatch(User user, string batch)
        {
            user.Batch = batch;
        }

        public void Disable(User user)
        {
            user.Active = false;
        }
    }
}
