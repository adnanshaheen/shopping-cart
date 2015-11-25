using Shopping.Data;
using Shopping.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Shopping.Business
{
    public class BusinessAuth : IBusinessAuth
    {
        private IAuthenticate iAuthData = null;

        public BusinessAuth()
        {
            iAuthData = GenericFactory<RepositoryAbstraction, IAuthenticate>.CreateInstance();
        }

        #region IBusinessAuth members
        public string GetRolesForUser(string uname)
        {
            return iAuthData.GetRolesForUser(uname);
        }

        public bool SignIn(string userName, string password, bool createPersistentCookie)
        {
            return iAuthData.SignIn(userName, password, createPersistentCookie);
        }

        public bool ChangePassword(string userName, string password, string newPassword)
        {
            return iAuthData.ChangePassword(userName, password, newPassword);
        }

        public void SignOut()
        {
            iAuthData.SignOut();
        }

        public bool ValidateUser(string userName, string password)
        {
            return iAuthData.ValidateUser(userName, password);
        }
        #endregion
    }
}