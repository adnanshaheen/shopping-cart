﻿using Shopping.Data;
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
        private IRepositoryAbstraction Repo = null;

        public BusinessShop()
        {
            Repo = GenericFactory<RepositoryAbstraction, IRepositoryAbstraction>.CreateInstance();
        }

        #region IBusinessAuth members
        public string GetRolesForUser(string uname)
        {
            return Repo.GetRolesForUser(uname);
        }

        public bool SignIn(string userName, string password, bool createPersistentCookie)
        {
            return Repo.SignIn(userName, password, createPersistentCookie);
        }

        public bool ChangePassword(string userName, string password, string newPassword)
        {
            return Repo.ChangePassword(userName, password, newPassword);
        }

        public void SignOut()
        {
            Repo.SignOut();
        }

        public bool ValidateUser(string userName, string password)
        {
            return Repo.ValidateUser(userName, password);
        }

        public RegistrationModel GetCustomerInfo(string userName)
        {
            return Repo.GetCustomerInfo(userName);
        }

        public bool UpdateCustomer(RegistrationModel Info)
        {
            return Repo.UpdateCustomer(Info);
        }
        #endregion

        #region IBusinessShop members
        public List<ProductModel> GetProducts(string catID)
        {
            List<ProductModel> TList = new List<ProductModel>();
            TList = Repo.GetProducts(catID);
            return TList;
        }

        public ProductModel GetProduct(string prodId)
        {
            return Repo.GetProduct(prodId);
        }

        public bool AddProduct(ProductModel product)
        {
            return Repo.AddProduct(product);
        }

        public bool UpdateProduct(ProductModel product)
        {
            return Repo.UpdateProduct(product);
        }

        public bool PlaceOrder(CheckoutModel order)
        {
            return Repo.PlaceOrder(order);
        }
        #endregion
    }
}