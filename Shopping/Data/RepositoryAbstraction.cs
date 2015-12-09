using Shopping.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Shopping.Models;

namespace Shopping.Data
{
    public class RepositoryAbstraction : IRepositoryAbstraction
    {
        private IAuthenticate iAuth = null;
        private IRepositoryShop iRepositoryShop = null;

        public RepositoryAbstraction()
        {
            iAuth = GenericFactory<RepositorySql, IAuthenticate>.CreateInstance();
            iRepositoryShop = GenericFactory<RepositorySql, IRepositoryShop>.CreateInstance();
        }

        #region IAuthenticate Members
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

        public RegistrationModel GetCustomerInfo(string userName)
        {
            return iAuth.GetCustomerInfo(userName);
        }
        #endregion

        #region IRepositoryShop Members
        public List<ProductModel> GetProducts(string catID)
        {
            return iRepositoryShop.GetProducts(catID);
        }

        public ProductModel GetProduct(string prodId)
        {
            return iRepositoryShop.GetProduct(prodId);
        }

        public bool AddProduct(ProductModel product)
        {
            return iRepositoryShop.AddProduct(product);
        }

        public bool UpdateProduct(ProductModel product)
        {
            return iRepositoryShop.UpdateProduct(product);
        }
        #endregion
    }
}