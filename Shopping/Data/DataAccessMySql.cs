using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Web;

namespace Shopping.Data
{
    public class DataAccessMySql : IDataAccess
    {
        private string CONNSTR = ConfigurationManager.ConnectionStrings["MYSQLDB"].ConnectionString;

        public DataAccessMySql()
        {

        }

        #region IDataAccess Members
        public object GetSingleAnswer(string sql, List<DbParameter> PList)
        {
            object obj = null;
            MySqlConnection conn = new MySqlConnection(CONNSTR);
            try
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                foreach (DbParameter p in PList)
                    cmd.Parameters.Add(p);
                obj = cmd.ExecuteScalar();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                conn.Close();
            }
            return obj;
        }

        public DataTable GetDataTable(string sql, List<DbParameter> PList)
        {
            DataTable dt = new DataTable();
            MySqlConnection conn = new MySqlConnection(CONNSTR);
            try
            {
                conn.Open();
                MySqlDataAdapter da = new MySqlDataAdapter();
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                foreach (DbParameter p in PList)
                    cmd.Parameters.Add(p);
                da.SelectCommand = cmd;
                da.Fill(dt);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                conn.Close();
            }
            return dt;
        }

        public int InsOrUpdOrDel(string sql, List<DbParameter> PList)
        {
            int rows = 0;
            MySqlConnection conn = new MySqlConnection(CONNSTR);
            try
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                foreach (DbParameter p in PList)
                    cmd.Parameters.Add(p);
                rows = cmd.ExecuteNonQuery();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                conn.Close();
            }
            return rows;
        }
        #endregion
    }
}