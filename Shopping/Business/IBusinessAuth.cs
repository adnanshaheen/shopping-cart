using Shopping.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shopping.Business
{
    public interface IBusinessAuth
    {
        string GetRolesForUser(string uname);
        bool SignIn(string userName, string password, bool createPersistentCookie);
        bool ChangePassword(string userName, string password, string newPassword);
        void SignOut();
        bool ValidateUser(string userName, string password);
        RegistrationModel GetCustomerInfo(string userName);
        bool UpdateCustomer(RegistrationModel Info);
    }
}
