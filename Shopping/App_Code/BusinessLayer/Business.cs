﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Shopping.App_Code.BusinessLayer
{
    public class Business : IBusinessAuth
    {
        Business()
        {

        }

        #region IBusinessAuth members
        public string GetRolesForUser(string uname)
        {
            throw new NotImplementedException();
        }

        public bool SignIn(string userName, string password, bool createPersistentCookie)
        {
            throw new NotImplementedException();
        }

        public bool ChangePassword(string userName, string password, string newPassword)
        {
            throw new NotImplementedException();
        }

        public void SignOut()
        {
            throw new NotImplementedException();
        }

        public bool ValidateUser(string userName, string password)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}