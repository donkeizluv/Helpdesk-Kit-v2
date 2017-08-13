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
        public bool Authenticate(string username, string pwd)
        {
            //mock login ad
            Thread.Sleep(1000);
            return true;
        }

        public bool SearchByUsername(string username, out User user)
        {
            user = new User()
            {
                Ad = "test user",
                Batch = string.Empty,
                Description = "mocking bird",
                ExpireDate = DateTime.Today,
                Lock = false

            };
            return true;
        }

        public bool SearchByHr(string hr, out User user)
        {
            user = null;
            return false;
        }

        public void Unlock(User user)
        {
            throw new UnauthorizedAccessException("Cant change user pwd");
        }

        public void ChangePassword(User user, string pwd)
        {
            user.ExpireDate = DateTime.Today.AddDays(45);
        }

        public void ChangeBatch(User user, string batch)
        {
            user.Batch = batch;
        }

        public void Disable(User user)
        {
            //user.
        }
    }
}
