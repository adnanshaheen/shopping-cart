﻿using Shopping.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Web;
using System.Web.Security;
using Shopping.Models;
using Shopping.Cache;
using System.IO;
using System.Data.OleDb;

namespace Shopping.Data
{
    public class RepositoryOrcl : IAuthenticate, IRepositoryShop
    {
        private IDataAccess idataAccess = null;
        private CacheAbstraction cache = null;

        public RepositoryOrcl()
        {
            idataAccess = GenericFactory<DataAccessOrcl, IDataAccess>.CreateInstance();
            cache = new CacheAbstraction();
        }

        #region IAuthenticate Members
        public bool ChangePassword(string userName, string password, string newPassword)
        {
            bool bRes = false;
            if (String.IsNullOrEmpty(userName))
                throw new ArgumentException("Value cannot be null or empty.", "userName");
            if (String.IsNullOrEmpty(password))
                throw new ArgumentException("Value cannot be null or empty.", "password");
            if (String.IsNullOrEmpty(newPassword))
                throw new ArgumentException("Value cannot be null or empty.", "newPassword");

            try
            {
                // user is already validated in AuthController.cs
                // Hence we don't need to validate again
                string sql = "update Users set Password=:newPassword where " +
                     "Username=:userName and Password=:password";
                List<DbParameter> PList = new List<DbParameter>();
                DbParameter p1 = new OleDbParameter(":newPassword", OleDbType.VarChar, 50);
                p1.Value = newPassword;
                PList.Add(p1);
                DbParameter p2 = new OleDbParameter(":userName", OleDbType.VarChar, 50);
                p2.Value = userName;
                PList.Add(p2);
                DbParameter p3 = new OleDbParameter(":password", OleDbType.VarChar, 50);
                p3.Value = password;
                PList.Add(p3);
                bRes = idataAccess.InsOrUpdOrDel(sql, PList) >= 1 ? true : false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return bRes;
        }

        public string GetRolesForUser(string uname)
        {
            string roles = "";
            string connStr = ConfigurationManager.ConnectionStrings["ORACLE"].ConnectionString;
            OleDbConnection conn = new OleDbConnection(connStr);

            try
            {
                conn.Open();
                OleDbCommand cmd = new OleDbCommand("GetUserRoles", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new OleDbParameter(":Username", uname));
                OleDbDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                    roles += reader["Role"].ToString() + "|";
                if (roles != "")  // remove last "|"
                    roles = roles.Substring(0, roles.Length - 1);
                conn.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
            }

            return roles;
        }

        public bool SignIn(string userName, string password, bool createPersistentCookie)
        {
            bool bRes = false;
            if (String.IsNullOrEmpty(userName)) throw new ArgumentException("Value cannot be null or empty.", "userName");
            try
            {
                if (ValidateUser(userName, password))
                {
                    string roles = GetRolesForUser(userName);

                    FormsAuthenticationTicket authTicket     // cookie timeout is also set
                        = new FormsAuthenticationTicket(1, userName, DateTime.Now, DateTime.Now.AddMinutes(5), false, roles);

                    //  encrypt the ticket
                    string encryptedTicket = FormsAuthentication.Encrypt(authTicket);

                    // add the encrypted ticket to the cookie as data
                    HttpCookie authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);

                    System.Web.HttpContext.Current.Response.Cookies.Add(authCookie);

                    bRes = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return bRes;
        }

        public void SignOut()
        {
            FormsAuthentication.SignOut();
        }

        public bool ValidateUser(string userName, string password)
        {
            if (String.IsNullOrEmpty(userName)) throw new ArgumentException("Value cannot be null or empty.", "userName");
            if (String.IsNullOrEmpty(password)) throw new ArgumentException("Value cannot be null or empty.", "password");

            object obj = null;
            try
            {
                string sql = "select Username from Users where Username=:userName" +
                    " and Password=:password";
                List<DbParameter> PList = new List<DbParameter>();
                DbParameter p1 = new OleDbParameter(":userName", OleDbType.VarChar, 50);
                p1.Value = userName;
                PList.Add(p1);
                DbParameter p2 = new OleDbParameter(":password", OleDbType.VarChar, 50);
                p2.Value = password;
                PList.Add(p2);

                obj = idataAccess.GetSingleAnswer(sql, PList);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return obj != null ? true : false;
        }

        public RegistrationModel GetCustomerInfo(string userName)
        {
            RegistrationModel Customer = null;
            List<RegistrationModel> TList = null;
            try
            {
                string key = String.Format("Customer_{0}", userName);
                Customer = cache.Retrieve<RegistrationModel>(key);
                if (Customer == null)
                {
                    DataTable dataTable = GetCustomerDB(userName);
                    TList = RepositoryHelper.ConvertToList<RegistrationModel>(dataTable);

                    Customer = TList.First<RegistrationModel>();
                    cache.Insert(key, Customer);
                }
            }
            catch (Exception)
            {
                throw;
            }
            return Customer;
        }

        public bool UpdateCustomer(RegistrationModel Info)
        {
            bool bRes = false;
            try
            {
                string sql = "update Customers set Address=:address, ZipCode=:zipCode, " +
                    "City=:city, State=:state, CCNumber=:creditcard, CCExpiration=:expiration, CCType=:type,"
                    + "Email=:email";

                List<DbParameter> PList = new List<DbParameter>();

                DbParameter p1 = new OleDbParameter(":address", OleDbType.VarChar, 50);
                p1.Value = Info.StreetAddress;
                PList.Add(p1);

                DbParameter p2 = new OleDbParameter(":zipCode", OleDbType.VarChar, 50);
                p2.Value = Info.ZipCode;
                PList.Add(p2);

                DbParameter p3 = new OleDbParameter(":city", OleDbType.VarChar, 50);
                p3.Value = Info.City;
                PList.Add(p3);

                DbParameter p4 = new OleDbParameter(":state", OleDbType.VarChar, 50);
                p4.Value = Info.State;
                PList.Add(p4);

                DbParameter p5 = new OleDbParameter(":creditcard", OleDbType.VarChar, 50);
                p5.Value = Info.CreditCard;
                PList.Add(p5);

                DbParameter p6 = new OleDbParameter(":expiration", OleDbType.VarChar, 50);
                p6.Value = Info.Expiration;
                PList.Add(p6);

                DbParameter p7 = new OleDbParameter(":type", OleDbType.VarChar, 50);
                p7.Value = Info.CreditCardType;
                PList.Add(p7);

                DbParameter p8 = new OleDbParameter(":email", OleDbType.VarChar, 50);
                p8.Value = Info.Email;
                PList.Add(p8);

                bRes = idataAccess.InsOrUpdOrDel(sql, PList) > 0 ? true : false;
            }
            catch (Exception)
            {
                throw;
            }
            return bRes;
        }
        #endregion

        #region IRepositoryShop Members
        public List<ProductModel> GetProducts(string catID)
        {
            List<ProductModel> TList = null;
            try
            {
                string key = String.Format("Products_{0}", catID);
                TList = cache.Retrieve<List<ProductModel>>(key);
                if (TList == null)
                {
                    DataTable dataTable = GetProductsDB(catID);
                    TList = RepositoryHelper.ConvertToList<ProductModel>(dataTable);
                    if (TList != null)
                    {
                        List<ImageModel> ImageList = null;
                        foreach (var item in TList)
                        {
                            DataTable imageTable = GetImages(item.ProductID);
                            ImageList = RepositoryHelper.ConvertToList<ImageModel>(imageTable);
                            if (ImageList != null && ImageList.Count() > 0)
                                item.Image = ImageList.First<ImageModel>();
                        }

                        cache.Insert(key, TList);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return TList;
        }

        public ProductModel GetProduct(string prodId)
        {
            ProductModel product = null;
            List<ProductModel> TList = null;
            try
            {
                // we should be adding each product related to catId in cache
                // and here, we should be able to get a product in cache
                string key = String.Format("Product_{0}", prodId);
                product = cache.Retrieve<ProductModel>(key);
                if (product == null)
                {
                    DataTable dataTable = GetProductDB(prodId);
                    TList = RepositoryHelper.ConvertToList<ProductModel>(dataTable);

                    List<ImageModel> ImageList = null;
                    foreach (var item in TList)
                    {
                        DataTable imageTable = GetImages(item.ProductID);
                        ImageList = RepositoryHelper.ConvertToList<ImageModel>(imageTable);
                        if (ImageList != null && ImageList.Count() > 0)
                            item.Image = ImageList.First<ImageModel>();
                    }

                    // We should have only one item in the list
                    product = TList.First<ProductModel>();
                    cache.Insert(key, product);
                }
            }
            catch (Exception)
            {
                throw;
            }
            return product;
        }

        public bool AddProduct(ProductModel product)
        {
            bool bRes = true;
            string ConnectionString = ConfigurationManager.ConnectionStrings["ORACLE"].ConnectionString;
            OleDbConnection Connection = new OleDbConnection(ConnectionString);
            OleDbTransaction Transaction = null;

            Stream stream = null;
            FileInfo fileInfo = null;
            Byte[] ImageData = null;
            try
            {
                Connection.Open();
                Transaction = Connection.BeginTransaction();

                string sql1 = "insert into Products (CatID, ProductSDesc, ProductLDesc,"
                    + "Price, Inventory) values("
                    + ":catID, :ProductSDesc, :ProductLDesc, :Price, :Inventory)";

                OleDbCommand cmd1 = new OleDbCommand(sql1, Connection);

                DbParameter p1a = new OleDbParameter(":catID", OleDbType.Integer);
                p1a.Value = product.CatagoryID;
                cmd1.Parameters.Add(p1a);

                DbParameter p2a = new OleDbParameter(":ProductSDesc", OleDbType.VarChar, 50);
                p2a.Value = product.ShortDesc;
                cmd1.Parameters.Add(p2a);

                DbParameter p3a = new OleDbParameter(":ProductLDesc", OleDbType.VarChar);
                p3a.Value = product.LongDesc;
                cmd1.Parameters.Add(p3a);

                DbParameter p4a = new OleDbParameter(":Price", OleDbType.Decimal);
                p4a.Value = product.Price;
                cmd1.Parameters.Add(p4a);

                DbParameter p5a = new OleDbParameter(":Inventory", OleDbType.Integer);
                p5a.Value = product.Inventory;
                cmd1.Parameters.Add(p5a);

                cmd1.Transaction = Transaction;
                int rows = cmd1.ExecuteNonQuery();
                if (rows <= 0)
                    throw new Exception("Couldn't insert product");

                string sql2 = "select * from Products where ProductId = (select max(ProductId) from products)";
                List<DbParameter> PList2 = new List<DbParameter>();
                object obj = idataAccess.GetSingleAnswer(sql2, PList2);
                string ProdId = obj != null ? obj.ToString() : "";

                stream = product.Image.ImageFile.InputStream;
                fileInfo = new FileInfo(Path.GetFullPath(product.Image.ImageFile.FileName));
                ImageData = new Byte[product.Image.ImageFile.ContentLength];

                stream.Read(ImageData, 0, product.Image.ImageFile.ContentLength);

                string sql3 = "insert into Images(Name, Type, Image, ProductId) values(@Name, @Type, @ImageData, @ProductId)";

                OleDbCommand cmd2 = new OleDbCommand(sql3, Connection);

                DbParameter p1b = new OleDbParameter(":Name", OleDbType.VarChar, 50);
                p1b.Value = fileInfo.Name;
                cmd2.Parameters.Add(p1b);

                DbParameter p2b = new OleDbParameter(":Type", OleDbType.VarChar, 10);
                p2b.Value = fileInfo.Extension;
                cmd2.Parameters.Add(p2b);

                DbParameter p3b = new OleDbParameter(":ImageData", OleDbType.VarBinary);
                p3b.Value = ImageData;
                cmd2.Parameters.Add(p3b);

                DbParameter p4b = new OleDbParameter(":ProductId", OleDbType.Integer);
                p4b.Value = ProdId;
                cmd2.Parameters.Add(p4b);

                cmd2.Transaction = Transaction;
                rows = cmd2.ExecuteNonQuery();
                if (rows <= 0)
                    throw new Exception("Couldn't insert product image");

                Transaction.Commit();
            }
            catch (Exception)
            {
                bRes = false;
                if (Transaction != null)
                    Transaction.Rollback();
                throw;
            }
            finally
            {
                if (Transaction != null)
                    Transaction.Dispose();

                Connection.Close();
            }

            return bRes;
        }

        public bool UpdateProduct(ProductModel product)
        {
            bool bRes = false;
            try
            {
                string sql1 = "update Products set CatID=:catID, ProductSDesc=:ProductSDesc, ProductLDesc=@ProductLDesc,"
                    + "Price=:Price, Inventory=:Inventory where ProductId=:prodId";
                List<DbParameter> PList1 = new List<DbParameter>();
                DbParameter p1a = new OleDbParameter(":catID", OleDbType.Integer);
                p1a.Value = product.CatagoryID;
                PList1.Add(p1a);

                DbParameter p2a = new OleDbParameter(":ProductSDesc", OleDbType.VarChar, 50);
                p2a.Value = product.ShortDesc;
                PList1.Add(p2a);

                DbParameter p3a = new OleDbParameter(":ProductLDesc", OleDbType.VarChar);
                p3a.Value = product.LongDesc;
                PList1.Add(p3a);

                DbParameter p4a = new OleDbParameter(":Price", OleDbType.Decimal);
                p4a.Value = product.Price;
                PList1.Add(p4a);

                DbParameter p5a = new OleDbParameter(":Inventory", OleDbType.Integer);
                p5a.Value = product.Inventory;
                PList1.Add(p5a);

                DbParameter p6a = new OleDbParameter(":prodId", OleDbType.Integer);
                p6a.Value = product.ProductID;
                PList1.Add(p6a);

                bRes = idataAccess.InsOrUpdOrDel(sql1, PList1) > 0 ? true : false;
                if (bRes)
                {
                    string key = String.Format("Product_{0}", product.ProductID);
                    cache.Remove(key);
                    key = String.Format("Products_{0}", product.CatagoryID);
                    cache.Remove(key);
                    key = "Products_";
                    cache.Remove(key);
                }
            }
            catch (Exception)
            {
                throw;
            }
            return bRes;
        }

        public bool PlaceOrder(CheckoutModel order)
        {
            bool bRes = true;
            string ConnectionString = ConfigurationManager.ConnectionStrings["ORACLE"].ConnectionString;
            OleDbConnection Connection = new OleDbConnection(ConnectionString);
            OleDbTransaction Transaction = null;

            try
            {
                Connection.Open();
                Transaction = Connection.BeginTransaction();

                string sql1 = "select top 1 OrderNo from Orders order by OrderNo desc";
                OleDbCommand cmd1 = new OleDbCommand(sql1, Connection);
                cmd1.Transaction = Transaction;
                object obj = cmd1.ExecuteScalar();
                Int64 orderNo = 1;
                if (obj != null)
                    orderNo = int.Parse(obj.ToString()) + 1;

                obj = GetUserId(order.Customer.UserName);
                if (obj == null)
                    throw new Exception("Can't find user in database");

                foreach (var item in order.Cart.CartList)
                {
                    string sql2 = "insert into OrderDetails (OrderNo, ItemNo, Qty) values (" +
                        ":orderNo, :ProdId, :Quantity)";
                    OleDbCommand cmd2 = new OleDbCommand(sql2, Connection);
                    DbParameter p1a = new OleDbParameter(":orderNo", OleDbType.BigInt);
                    p1a.Value = orderNo;
                    cmd2.Parameters.Add(p1a);

                    DbParameter p2a = new OleDbParameter(":ProdId", OleDbType.Integer);
                    p2a.Value = item.ProductID;
                    cmd2.Parameters.Add(p2a);

                    DbParameter p3a = new OleDbParameter(":Quantity", OleDbType.Integer);
                    p3a.Value = item.ProductQuantity;
                    cmd2.Parameters.Add(p3a);
                    cmd2.Transaction = Transaction;

                    int rows1 = cmd2.ExecuteNonQuery();
                    if (rows1 <= 0)
                        throw new Exception("Couldn't place your details");
                }

                string sql3 = "insert into Orders (OrderNo,UserID, OrderDate) VALUES (" +
                    ":orderNo, :userId, :orderDate)";
                OleDbCommand cmd3 = new OleDbCommand(sql3, Connection);
                DbParameter p1b = new OleDbParameter(":orderNo", OleDbType.Integer);
                p1b.Value = orderNo;
                cmd3.Parameters.Add(p1b);

                DbParameter p2b = new OleDbParameter(":userId", OleDbType.VarChar, 50);
                p2b.Value = obj.ToString();
                cmd3.Parameters.Add(p2b);

                DbParameter p3b = new OleDbParameter(":orderDate", OleDbType.VarChar, 50);
                p3b.Value = System.DateTime.Now.ToString();
                cmd3.Parameters.Add(p3b);
                cmd3.Transaction = Transaction;

                int rows2 = cmd3.ExecuteNonQuery();
                if (rows2 <= 0)
                    throw new Exception("Couldn't insert order in database");

                Transaction.Commit();
            }
            catch (Exception)
            {
                bRes = false;
                if (Transaction != null)
                    Transaction.Rollback();
                throw;
            }
            finally
            {
                if (Transaction != null)
                    Transaction.Dispose();
            }
            return bRes;
        }
        #endregion

        #region Private Members
        private DataTable GetProductsDB(string catID)
        {
            DataTable dataTable = null;
            try
            {
                string sql = "";
                List<DbParameter> PList = new List<DbParameter>();
                if (String.IsNullOrEmpty(catID))
                {
                    sql = "select * from products";
                }
                else
                {
                    sql = "select * from  products where catid=:catID";
                    DbParameter p1 = new OleDbParameter(":catID", OleDbType.VarChar, 50);
                    p1.Value = catID;
                    PList.Add(p1);
                }
                dataTable = idataAccess.GetDataTable(sql, PList);
            }
            catch (Exception)
            {
                throw;
            }
            return dataTable;
        }

        private DataTable GetProductDB(string prodId)
        {
            DataTable dataTable = null;
            try
            {
                string sql = "select * from products where ProductId=:prodId";
                List<DbParameter> PList = new List<DbParameter>();
                DbParameter p1 = new OleDbParameter(":prodId", OleDbType.VarChar, 50);
                p1.Value = prodId;
                PList.Add(p1);

                dataTable = idataAccess.GetDataTable(sql, PList);
            }
            catch (Exception)
            {
                throw;
            }
            return dataTable;
        }

        private DataTable GetImages(int prodId)
        {
            DataTable dataTable = null;
            try
            {
                string sql = "select * from Images where ProductId=:prodId";
                List<DbParameter> PList = new List<DbParameter>();
                DbParameter p1 = new OleDbParameter(":prodId", OleDbType.Integer);
                p1.Value = prodId;
                PList.Add(p1);

                dataTable = idataAccess.GetDataTable(sql, PList);
            }
            catch (Exception)
            {
                throw;
            }
            return dataTable;
        }
        //get role
        public string Getrole(string uname)
        {
            string ConnectionString = ConfigurationManager.ConnectionStrings["ORACLE"].ConnectionString;
            OleDbConnection Connection = new OleDbConnection(ConnectionString);
            OleDbTransaction Transaction = null;
            string role;

            try
            {
                Connection.Open();
                Transaction = Connection.BeginTransaction();

                string sql1 = "select userid from users where username=:userName";
                DbParameter p1 = new OleDbParameter(":userName", OleDbType.VarChar, 50);
                p1.Value = uname;

                OleDbCommand cmd1 = new OleDbCommand(sql1, Connection);
                cmd1.Parameters.Add(p1);
                cmd1.Transaction = Transaction;
                object obj = cmd1.ExecuteScalar();
                string userid;
                if (obj != null)
                    userid = obj.ToString();
                else
                    throw new Exception("Couldn't get userid");

                string sql2 = "select role_id from userroles where userid=:userid";
                OleDbCommand cmd2 = new OleDbCommand(sql2, Connection);
                DbParameter p2 = new OleDbParameter(":userid", OleDbType.VarChar, 50);
                p2.Value = userid;
                cmd2.Parameters.Add(p2);
                cmd2.Transaction = Transaction;
                obj = cmd2.ExecuteScalar();
                string roleid;
                if (obj != null)
                    roleid = obj.ToString();
                else
                    throw new Exception("Couldn't get roleid");

                string sql3 = "select role from definerole where roleid=:roleid";
                OleDbCommand cmd3 = new OleDbCommand(sql3, Connection);
                DbParameter p3 = new OleDbParameter(":roleid", OleDbType.VarChar, 50);
                p3.Value = roleid;
                cmd3.Parameters.Add(p3);
                cmd3.Transaction = Transaction;
                obj = cmd3.ExecuteScalar();

                if (obj != null)
                    role = obj.ToString();
                else
                    throw new Exception("Couldn't get role from definedroles");

                Transaction.Commit();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Connection.Close();
            }
            return role;
        }

        //get role
        private DataTable GetCustomerDB(string userName)
        {
            DataTable Customer = null;
            try
            {
                object obj = GetUserId(userName);
                if (obj != null)
                {
                    string userId = obj.ToString();
                    string sql = "select * from CustomerInfos where UserID=:userId";
                    List<DbParameter> PList = new List<DbParameter>();
                    DbParameter p1 = new OleDbParameter(":userId", OleDbType.VarChar, 50);
                    p1.Value = userId;
                    PList.Add(p1);

                    Customer = idataAccess.GetDataTable(sql, PList);
                }
            }
            catch (Exception)
            {
                throw;
            }
            return Customer;
        }

        private object GetUserId(string userName)
        {
            object obj = null;
            try
            {
                string sql = "select UserID from Users where Username=:userName";
                List<DbParameter> PList = new List<DbParameter>();
                DbParameter p1 = new OleDbParameter(":userName", OleDbType.VarChar, 50);
                p1.Value = userName;
                PList.Add(p1);

                obj = idataAccess.GetSingleAnswer(sql, PList);
            }
            catch (Exception)
            {
                throw;
            }
            return obj;
        }
        #endregion
    }
}
