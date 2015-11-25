using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Shopping.Business
{
    public class BusinessAuth : IBusinessAuth
    {
        public BusinessAuth()
        {

        }

        #region IBusinessAuth members
        public string GetRolesForUser(string uname)
        {
            return "";
        }

        public bool SignIn(string userName, string password, bool createPersistentCookie)
        {
            return false;
        }

        public bool ChangePassword(string userName, string password, string newPassword)
        {
            return false;
        }

        public void SignOut()
        {
        }

        public bool ValidateUser(string userName, string password)
        {
            return false;
        }
        #endregion
    }
}