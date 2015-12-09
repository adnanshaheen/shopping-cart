using MySql.Data.MySqlClient;
using Shopping.Utilities;
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

namespace Shopping.Data
{
    public class RepositoryMySql : IAuthenticate, IRepositoryShop
    {
        private IDataAccess idataAccess = null;
        private CacheAbstraction cache = null;

        public RepositoryMySql()
        {
            idataAccess = GenericFactory<DataAccessMySql, IDataAccess>.CreateInstance();
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
                string sql = "update Users set Password=@newPassword where " +
                     "Username=@userName and Password=@password";
                List<DbParameter> PList = new List<DbParameter>();
                DbParameter p1 = new MySqlParameter("@newPassword", MySqlDbType.VarChar, 50);
                p1.Value = newPassword;
                PList.Add(p1);
                DbParameter p2 = new MySqlParameter("@userName", MySqlDbType.VarChar, 50);
                p2.Value = userName;
                PList.Add(p2);
                DbParameter p3 = new MySqlParameter("@password", MySqlDbType.VarChar, 50);
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
            string connStr = ConfigurationManager.ConnectionStrings["MYSQLDB"].ConnectionString;
            MySqlConnection conn = new MySqlConnection(connStr);

            try
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("GetUserRoles", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new MySqlParameter("@Username", uname));
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                    roles += reader["RoleName"].ToString() + "|";
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
                string sql = "select Username from Users where Username=@userName" +
                    " and Password=@password";
                List<DbParameter> PList = new List<DbParameter>();
                DbParameter p1 = new MySqlParameter("@userName", MySqlDbType.VarChar, 50);
                p1.Value = userName;
                PList.Add(p1);
                DbParameter p2 = new MySqlParameter("@password", MySqlDbType.VarChar, 50);
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
            // FIXME: Add transaction
            bool bRes = false;
            Stream stream = null;
            FileInfo fileInfo = null;
            Byte[] ImageData = null;
            try
            {
                string sql1 = "insert into Products (CatID, ProductSDesc, ProductLDesc,"
                    + "Price, Inventory) values("
                    + "@catID, @ProductSDesc, @ProductLDesc, @Price, @Inventory)";
                List<DbParameter> PList1 = new List<DbParameter>();
                DbParameter p1a = new MySqlParameter("@catID", MySqlDbType.Int32);
                p1a.Value = product.CatagoryID;
                PList1.Add(p1a);

                DbParameter p2a = new MySqlParameter("@ProductSDesc", MySqlDbType.VarChar, 50);
                p2a.Value = product.ShortDesc;
                PList1.Add(p2a);

                DbParameter p3a = new MySqlParameter("@ProductLDesc", MySqlDbType.Text);
                p3a.Value = product.LongDesc;
                PList1.Add(p3a);

                DbParameter p4a = new MySqlParameter("@Price", MySqlDbType.Decimal);
                p4a.Value = product.Price;
                PList1.Add(p4a);

                DbParameter p5a = new MySqlParameter("@Inventory", MySqlDbType.Int32);
                p5a.Value = product.Inventory;
                PList1.Add(p5a);

                bRes = idataAccess.InsOrUpdOrDel(sql1, PList1) > 0 ? true : false;
                if (bRes)
                {
                    string sql2 = "select top 1 ProductId from Products order by ProductId desc";
                    List<DbParameter> PList2 = new List<DbParameter>();
                    object obj = idataAccess.GetSingleAnswer(sql2, PList2);
                    string ProdId = obj != null ? obj.ToString() : "";

                    stream = product.Image.ImageFile.InputStream;
                    fileInfo = new FileInfo(Path.GetFullPath(product.Image.ImageFile.FileName));
                    ImageData = new Byte[product.Image.ImageFile.ContentLength];

                    stream.Read(ImageData, 0, product.Image.ImageFile.ContentLength);

                    string sql3 = "insert into Images(Name, Type, Image, ProductId) values(@Name, @Type, @ImageData, @ProductId)";
                    List<DbParameter> PList = new List<DbParameter>();
                    DbParameter p1b = new MySqlParameter("@Name", MySqlDbType.VarChar, 50);
                    p1b.Value = fileInfo.Name;
                    PList.Add(p1b);

                    DbParameter p2b = new MySqlParameter("@Type", MySqlDbType.VarChar, 10);
                    p2b.Value = fileInfo.Extension;
                    PList.Add(p2b);

                    DbParameter p3b = new MySqlParameter("@ImageData", MySqlDbType.VarBinary);
                    p3b.Value = ImageData;
                    PList.Add(p3b);

                    DbParameter p4b = new MySqlParameter("@ProductId", MySqlDbType.Int32);
                    p4b.Value = ProdId;
                    PList.Add(p4b);

                    bRes = idataAccess.InsOrUpdOrDel(sql3, PList) > 0 ? true : false;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return bRes;
        }
        #endregion

        private DataTable GetProductsDB(string catID)
        {
            DataTable dataTable = null;
            try
            {
                string sql = "select * from  products where catid=@catID";
                List<DbParameter> PList = new List<DbParameter>();
                DbParameter p1 = new MySqlParameter("@catID", MySqlDbType.VarChar, 50);
                p1.Value = catID;
                PList.Add(p1);

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
                string sql = "select * from products where ProductId=@prodId";
                List<DbParameter> PList = new List<DbParameter>();
                DbParameter p1 = new MySqlParameter("@prodId", MySqlDbType.VarChar, 50);
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
                string sql = "select * from images where ProductId=@prodId";
                List<DbParameter> PList = new List<DbParameter>();
                DbParameter p1 = new MySqlParameter("@prodId", MySqlDbType.Int32);
                p1.Value = prodId;

                dataTable = idataAccess.GetDataTable(sql, PList);
            }
            catch (Exception)
            {
                throw;
            }
            return dataTable;
        }
    }
}
