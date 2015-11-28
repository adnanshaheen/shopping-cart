using Shopping.Data;
using Shopping.Models;
using Shopping.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Shopping.Business
{
    public class BusinessShop : IBusinessAuth, IBusinessShop
    {
        private IAuthenticate iAuthData = null;
        private IRepositoryShop iRepositoryShop = null;

        public BusinessShop()
        {
            iAuthData = GenericFactory<RepositoryAbstraction, IAuthenticate>.CreateInstance();
            iRepositoryShop = GenericFactory<RepositoryAbstraction, IRepositoryShop>.CreateInstance();
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

        #region IBusinessShop members
        public List<ProductModel> GetProducts(string catID)
        {
            List<ProductModel> TList = new List<ProductModel>();
            TList = iRepositoryShop.GetProducts(catID);
            return TList;
        }
        #endregion
    }
}