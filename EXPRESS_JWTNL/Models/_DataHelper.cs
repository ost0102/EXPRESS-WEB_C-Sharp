using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Configuration;
using Oracle.ManagedDataAccess;
using Oracle.ManagedDataAccess.Client;
namespace EXPRESS_JWTNL.Models
{
    public class _DataHelper
    {
        private static string _connectionString;        
        private static object _connectionStringLockObject = new object();        
        public static string ConnectionString
        {
            get
            {
                if (string.IsNullOrEmpty(_connectionString))
                {
                    lock (_connectionStringLockObject)
                    {
                        _connectionString = ConfigurationManager.ConnectionStrings["ELVIS_ORACLE"].ConnectionString;
                    }
                }

                return _connectionString;
            }
        }
        public static string copyExcelData(DataTable dt)
        {
            string strResult = "";
            //DataTable Mdt = new DataTable();
            //DataTable Hdt = new DataTable();
            //DataTable Sdt = new DataTable();
            try
            {
                using (var bulkCopy = new OracleBulkCopy(ConfigurationManager.ConnectionStrings["ELVIS_ORACLE"].ConnectionString))
                {
                    if (dt.Rows[0]["SORT"].ToString() == "M")
                    {
                        bulkCopy.DestinationTableName = "TEMP_MBL_MST_UL";
                        bulkCopy.WriteToServer(dt);
                    }
                    else if (dt.Rows[0]["SORT"].ToString() == "H")
                    {
                        bulkCopy.DestinationTableName = "TEMP_HBL_MST_UL";
                        bulkCopy.WriteToServer(dt);
                    }
                    else
                    {
                        bulkCopy.DestinationTableName = "TEMP_SKU_MST_UL";
                        bulkCopy.WriteToServer(dt);
                    }

                    return "Y";
                }
            }
            catch (Exception e)
            {
                strResult = e.Message;
                return "N";
            }
        }

        public static bool CheckDataBaseConnecting()
        {            
            try
            {
                OracleConnection conn = new OracleConnection(ConnectionString);
                conn.Open();
                conn.Clone();
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static void test(DataSet ds)
        {
            DataTable dt = new DataTable();
            using (var bulkCopy = new OracleBulkCopy(ConfigurationManager.ConnectionStrings["ELVIS_ORACLE"].ConnectionString))
            {
                dt = ds.Tables["MST"];
                bulkCopy.DestinationTableName = "TEMP_MBL_MST";
                bulkCopy.WriteToServer(dt);

                dt = ds.Tables["HBL"];
                bulkCopy.DestinationTableName = "TEMP_HBL_MST";
                bulkCopy.WriteToServer(dt);

                dt = ds.Tables["SKU"];
                bulkCopy.DestinationTableName = "TEMP_HBL_SKU";
                bulkCopy.WriteToServer(dt);
            }
        }

        /// <summary>
        /// Insert Query 전용
        /// </summary>
        /// <param name="Sql">Oracle Query</param>
        /// <param name="cmdType">Query Type</param>
        /// <returns></returns>
        public static int ExecuteNonQuery(string Sql, CommandType cmdType)
        {
            int result;

            using (OracleConnection conn = new OracleConnection(ConnectionString))
            {

                conn.Open();
                OracleTransaction tran = conn.BeginTransaction();
                OracleCommand cmd = new OracleCommand(Sql, conn);
                cmd.CommandType = cmdType;
                cmd.Connection = conn;
                cmd.Transaction = tran;
                try
                {
                    result = cmd.ExecuteNonQuery();
                    tran.Commit();
                    conn.Close();
                }
                catch (Exception)
                {
                    tran.Rollback();
                    if (conn.State != ConnectionState.Closed)
                    {
                        conn.Close();
                    }
                    throw;
                }
            }
            return result;
        }

        public static int ExecuteNonCommit(string Sql, CommandType cmdType)
        {
            int result;

            using (OracleConnection conn = new OracleConnection(ConnectionString))
            {

                conn.Open();
                OracleTransaction tran = conn.BeginTransaction();
                OracleCommand cmd = new OracleCommand(Sql, conn);
                cmd.CommandType = cmdType;
                cmd.Connection = conn;
                cmd.Transaction = tran;
                try
                {
                    result = cmd.ExecuteNonQuery();
                    conn.Close();
                }
                catch (Exception)
                {
                    if (conn.State != ConnectionState.Closed)
                    {
                        conn.Close();
                    }
                    throw;
                }
            }
            return result;
        }

        public static int ExecuteNonQuery(string _ConnStr, string Sql, CommandType cmdType)
        {
            int result;

            using (OracleConnection conn = new OracleConnection(_ConnStr))
            {

                conn.Open();
                OracleTransaction tran = conn.BeginTransaction();
                OracleCommand cmd = new OracleCommand(Sql, conn);
                cmd.CommandType = cmdType;
                cmd.Connection = conn;
                cmd.Transaction = tran;
                try
                {
                    result = cmd.ExecuteNonQuery();

                    tran.Commit();

                    conn.Close();
                }
                catch (Exception)
                {
                    tran.Rollback();
                    if (conn.State != ConnectionState.Closed)
                    {
                        conn.Close();
                    }
                    throw;
                }
            }

            return result;
        }

        public static object ExecuteScalar(string Sql, CommandType cmdType)
        {
            object result = null;

            using (OracleConnection conn = new OracleConnection(ConnectionString))
            {
                try
                {
                    conn.Open();

                    OracleCommand cmd = new OracleCommand(Sql, conn);
                    cmd.CommandType = cmdType;

                    result = cmd.ExecuteScalar();

                    conn.Close();
                }
                catch (Exception)
                {
                    if (conn.State != ConnectionState.Closed)
                        conn.Close();

                    throw;
                }
            }

            return result;
        }

        public static object ExecuteScalar(string _ConnStr, string Sql, CommandType cmdType)
        {
            object result = null;

            using (OracleConnection conn = new OracleConnection(_ConnStr))
            {
                try
                {
                    conn.Open();

                    OracleCommand cmd = new OracleCommand(Sql, conn);
                    cmd.CommandType = cmdType;

                    result = cmd.ExecuteScalar();

                    conn.Close();
                }
                catch (Exception)
                {
                    if (conn.State != ConnectionState.Closed)
                        conn.Close();

                    throw;
                }
            }

            return result;
        }

        public static DataSet ExecuteDataSet(string Sql, CommandType cmdType)
        {
            DataSet dsResult = null;

            using (OracleConnection conn = new OracleConnection(ConnectionString))
            {
                try
                {
                    conn.Open();
                    OracleCommand cmd = new OracleCommand(Sql, conn);

                    OracleDataAdapter da = new OracleDataAdapter(cmd);
                    dsResult = new DataSet();
                    da.Fill(dsResult);
                    da.Dispose();
                    conn.Close();

                }
                catch (Exception)
                {
                    if (conn.State != ConnectionState.Closed)
                    {
                        conn.Close();
                    }
                    throw;
                }
            }

            return dsResult;
        }

        public static DataSet ExecuteDataSet(string _ConnStr, string Sql, CommandType cmdType)
        {
            DataSet dsResult = null;

            using (OracleConnection conn = new OracleConnection(_ConnStr))
            {
                try
                {
                    conn.Open();
                    OracleCommand cmd = new OracleCommand(Sql, conn);

                    OracleDataAdapter da = new OracleDataAdapter(cmd);
                    dsResult = new DataSet();
                    da.Fill(dsResult);
                    da.Dispose();
                    conn.Close();

                }
                catch (Exception)
                {
                    if (conn.State != ConnectionState.Closed)
                    {
                        conn.Close();
                    }
                    throw;
                }
            }

            return dsResult;
        }

        public static DataTable ExecuteDataTable(string Sql, CommandType cmdType)
        {
            DataTable dtResult = null;

            using (OracleConnection conn = new OracleConnection(ConnectionString))
            {
                try
                {
                    conn.Open();

                    OracleCommand cmd = new OracleCommand(Sql, conn);
                    cmd.CommandType = cmdType;

                    dtResult = new DataTable();

                    OracleDataAdapter da = new OracleDataAdapter(cmd);
                    da.Fill(dtResult);
                    da.Dispose();
                    conn.Close();
                }
                catch (Exception e)
                {
                    if (conn.State != ConnectionState.Closed)
                    {
                        conn.Close();
                    }
                    throw;
                }
            }

            return dtResult;
        }

        public static DataTable ExecuteDataTable(string _ConnStr, string Sql, CommandType cmdType)
        {
            DataTable dtResult = null;

            using (OracleConnection conn = new OracleConnection(_ConnStr))
            {
                try
                {
                    conn.Open();

                    OracleCommand cmd = new OracleCommand(Sql, conn);
                    cmd.CommandType = cmdType;

                    dtResult = new DataTable();

                    OracleDataAdapter da = new OracleDataAdapter(cmd);
                    da.Fill(dtResult);
                    da.Dispose();
                    conn.Close();
                }
                catch (Exception e)
                {
                    if (conn.State != ConnectionState.Closed)
                    {
                        conn.Close();
                    }
                    throw;
                }
            }

            return dtResult;
        }

        public static OracleDataReader ExecuteDataReader(string Sql, CommandType cmdType)
        {
            OracleDataReader rs = null;
            OracleConnection conn = null;

            try
            {
                conn = new OracleConnection(ConnectionString);
                conn.Open();

                OracleCommand cmd = new OracleCommand(Sql, conn);
                cmd.CommandType = cmdType;
                rs = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (Exception)
            {
                if (rs != null)
                {
                    rs.Close();
                }

                if (conn != null && conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                }

                throw;
            }

            
            return rs;
        }

       
        public static DataTable CallPRoc(string mngt_no, string mbl_no)
        {
            int result = 0;
            DataTable ht = new DataTable();
            string procName = "USP_CAINIAO_INSERT";
            using (OracleConnection conn = new OracleConnection(ConnectionString))
            {

                try
                {
                    conn.Open();

                    OracleCommand cmd = new OracleCommand(procName, conn);
                    if (mbl_no != null)
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("P_MNGT_NO", OracleDbType.Varchar2, 32767).Value = mngt_no;
                        cmd.Parameters.Add("P_MBL_NO", OracleDbType.Varchar2, 32767).Value = mbl_no;
                        cmd.Parameters.Add("R_RTNCD", OracleDbType.Varchar2, 32767).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add("R_RTNMSG", OracleDbType.Varchar2, 32767).Direction = ParameterDirection.Output;
                        //OracleDataAdapter da = new OracleDataAdapter(cmd);
                        //da.Fill(dtResult);
                        //da.Dispose();

                        result = cmd.ExecuteNonQuery();
                        conn.Close();

                        ht.Columns.Add("R_RTNCD");
                        ht.Columns.Add("R_RTNMSG");

                        DataRow dr1 = ht.NewRow();

                        dr1["R_RTNCD"] = cmd.Parameters["R_RTNCD"].Value.ToString();
                        dr1["R_RTNMSG"] = cmd.Parameters["R_RTNMSG"].Value.ToString();

                        ht.Rows.Add(dr1);

                        //ht.Add("R_RTNCD", cmd.Parameters["R_RTNCD"].Value.ToString());
                        //ht.Add("R_RTNMSG", cmd.Parameters["R_RTNMSG"].Value.ToString());

                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    if (conn.State != ConnectionState.Closed)
                    {
                        conn.Close();
                    }
                    throw;
                }
            }
            return ht;
        }




    }
}