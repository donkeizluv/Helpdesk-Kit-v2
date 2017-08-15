using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpdeskKit.AD
{
    public interface IActiveDirectory
    {
        bool Authenticate(string username, string pwd);
        bool SearchByUsername(string username, out User user);
        bool SearchByHr(string hr, out User user);
        void Unlock(User user);
        void ChangePassword(User user, string pwd);
        void ChangeBatch(User user, string batch);
        void Disable(User user);
    }
}
