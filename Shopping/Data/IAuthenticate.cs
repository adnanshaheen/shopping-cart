using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shopping.Data
{
    public interface IAuthenticate
    {
        string GetRolesForUser(string uname);
        bool SignIn(string userName, string password, bool createPersistentCookie);
        bool ChangePassword(string userName, string password, string newPassword);
        void SignOut();
        bool ValidateUser(string userName, string password);
    }
}
