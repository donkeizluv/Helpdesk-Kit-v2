using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HelpdeskKit.AD
{
    public class MyAd : IActiveDirectory
    {
        public bool Authenticate(string username, string pwd)
        {
            throw new NotImplementedException();
        }

        public bool SearchByUsername(string username, out User user)
        {
            throw new NotImplementedException();
        }

        public bool SearchByHr(string hr, out User user)
        {
            throw new NotImplementedException();
        }

        public void Unlock(User user)
        {
            throw new NotImplementedException();
        }

        public void ChangePassword(User user, string pwd)
        {
            throw new NotImplementedException();
        }

        public void ChangeBatch(User user, string batch)
        {
            throw new NotImplementedException();
        }

        public void Disable(User user)
        {
            throw new NotImplementedException();
        }
    }
}
