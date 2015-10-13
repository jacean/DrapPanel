using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Windows.Forms;
using System.Drawing;
using System.IO;

namespace DrapPanel
{
    class sqlConn
    {


        private string _connString = null;
        public string _DBType = null;

        public SqlConnection cn_Sql = null;
        public SqlTransaction tx_Sql = null;

        public sqlConn(string DBString, string DBType)
        {
            _connString = DBString;
            _DBType = DBType;
            if (_DBType.Equals("SQL"))
            {
                if (cn_Sql == null)
                {
                    cn_Sql = new SqlConnection(_connString);
                    cn_Sql.Open();
                }
            }
        }
        public void Close()
        {
            if (cn_Sql != null)
            {
                cn_Sql.Close();
                cn_Sql = null;
            }
        }

        public int executeUpdate(string SQL)
        {
            int affected = -1;
            try
            {
                if (_DBType.Equals("SQL"))
                {
                    tx_Sql = cn_Sql.BeginTransaction();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = cn_Sql;
                    cmd.CommandText = SQL;
                    cmd.CommandType = CommandType.Text;
                    cmd.Transaction = tx_Sql;
                    affected = cmd.ExecuteNonQuery();
                    tx_Sql.Commit();
                }
            }
            catch (Exception e)
            {
            }
            return affected;
        }
        public DataTable getVector(string SQL)
        {
            DataTable dt = new DataTable();

            try
            {
                if (_DBType.Equals("SQL"))
                {
                    tx_Sql = cn_Sql.BeginTransaction();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = cn_Sql;
                    cmd.CommandText = SQL;
                    cmd.CommandType = CommandType.Text;
                    cmd.Transaction = tx_Sql;
                    SqlDataReader dr = cmd.ExecuteReader();
                    dt.Load(dr);
                    tx_Sql.Commit();
                    return dt;
                }
            }
            catch (Exception e)
            {
            }
            return null;
        }
        public string GetSingleResult(string SQL)
        {
            DataTable dt = this.getVector(SQL);
            if (dt == null)
            {
                return "";
            }
            else
            {
                return dt.Rows[0][0].ToString();
            }
        }


    }
}
