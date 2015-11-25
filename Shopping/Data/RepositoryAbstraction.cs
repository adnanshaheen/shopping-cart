using Shopping.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Shopping.Data
{
    public class RepositoryAbstraction : IAuthenticate
    {
        private IAuthenticate iAuth = null;

        public RepositoryAbstraction()
        {
            iAuth = GenericFactory<RepositorySql, IAuthenticate>.CreateInstance();
        }

        public bool ChangePassword(string userName, string password, string newPassword)
        {
            return iAuth.ChangePassword(userName, password, newPassword);
        }

        public string GetRolesForUser(string uname)
        {
            return iAuth.GetRolesForUser(uname);
        }

        public bool SignIn(string userName, string password, bool createPersistentCookie)
        {
            return iAuth.SignIn(userName, password, createPersistentCookie);
        }

        public void SignOut()
        {
            iAuth.SignOut();
        }

        public bool ValidateUser(string userName, string password)
        {
            return iAuth.ValidateUser(userName, password);
        }
    }
}