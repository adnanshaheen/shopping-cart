using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;

namespace Shopping.Data
{
    public class DataAccessOrcl : IDataAccess
    {
        private string CONNSTR = ConfigurationManager.ConnectionStrings["ORACLE"].ConnectionString;

        public DataAccessOrcl()
        {

        }

        #region IDataAccess Members
        public object GetSingleAnswer(string sql, List<DbParameter> PList)
        {
            object obj = null;
            OleDbConnection conn = new OleDbConnection(CONNSTR);
            //OracleConnection conn = new OracleConnection(CONNSTR);
            try
            {
                conn.Open();
                OleDbCommand cmd = new OleDbCommand(sql, conn);
                //OracleCommand cmd = new OracleCommand(sql, conn);
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
            OleDbConnection conn = new OleDbConnection(CONNSTR);
            try
            {
                conn.Open();
                OleDbDataAdapter da = new OleDbDataAdapter();
                OleDbCommand cmd = new OleDbCommand(sql, conn);
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
            OleDbConnection conn = new OleDbConnection(CONNSTR);
            try
            {
                conn.Open();
                OleDbCommand cmd = new OleDbCommand(sql, conn);
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
