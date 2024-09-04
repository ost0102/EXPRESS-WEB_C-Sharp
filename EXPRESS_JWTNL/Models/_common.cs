using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using Newtonsoft.Json;
using System.Text;
using System.Collections;

namespace EXPRESS_JWTNL.Models
{
    public static class _common
    {
        public static string jSon = "";
        public static string sec_jSon = "";
        private static string strPageCount = "100";
        public static void ThrowMsg(bool ErrorOccur, string Msg)
        {
            ErrorOccur = true;
            throw new Exception(Msg);
        }

        public static int DataPageCount        
        {
            get
            {
                int rtnCount = 0;

                if (System.Web.Configuration.WebConfigurationManager.AppSettings["DataPageCount"] != null)
                {
                    strPageCount = System.Web.Configuration.WebConfigurationManager.AppSettings["DataPageCount"].ToString();
                }

                rtnCount = int.Parse(strPageCount);
                return rtnCount;
            }
        }

        #region Make Json - Data Convert to jSon

        public static string MakeJson(string status, string Msg)
        {
            sec_jSon = "";
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            Dictionary<string, object> dic = new Dictionary<string, object>();
            try
            {
                //dt.Columns.Add("rec_cd");
                // dt.Columns.Add("res_msg");
                //DataRow row1 = dt.NewRow();
                dic["rec_cd"] = status;
                dic["res_msg"] = Msg;
                //dt.Rows.Add(row1);
                //sec_jSon = _Sec_Encrypt.encryptAES256(JsonConvert.SerializeObject(ds, Formatting.Indented));
                sec_jSon = JsonConvert.SerializeObject(dic);
                return sec_jSon;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public static string MakeJson(string status, string Msg, DataTable args)
        {
            sec_jSon = "";
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            try
            {                                
                dt.Columns.Add("trxCode");
                dt.Columns.Add("trxMsg");
                DataRow row1 = dt.NewRow();
                row1["trxCode"] = status;
                row1["trxMsg"] = Msg;
                dt.Rows.Add(row1);
                dt.TableName = "Result";
                ds.Tables.Add(dt);
                if (status != "E") ds.Tables.Add(args);
                //jSon = JsonConvert.SerializeObject(ds);
                //sec_jSon = _Sec_Encrypt.encryptAES256(JsonConvert.SerializeObject(ds));
                sec_jSon = JsonConvert.SerializeObject(ds);
                return sec_jSon;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public static string MakeJson(DataTable Result, DataTable DT1)
        {
            jSon = "";
            sec_jSon = "";
            try
            {
                DataSet ds = new DataSet();
                Result.TableName = "Master";
                ds.Tables.Add(Result);
                DT1.TableName = "Detail";
                ds.Tables.Add(DT1);
                jSon = JsonConvert.SerializeObject(ds, Formatting.Indented);
                //sec_jSon = _Sec_Encrypt.encryptAES256(jSon);
            }
            catch (Exception e)
            {
                return e.Message;
            }
            return jSon;
        }

        /// <summary>
        /// return DS
        /// </summary>
        /// <param name="Result">MakeResultDT를 써서 가져온 DataTable을 넣어주면 됨.</param>
        /// <param name="DT1">병합할 DataTable 첫번째</param>
        /// <param name="DT2">병합할 DataTable 두번째</param>
        /// <returns></returns>
        public static string MakeJson(DataTable Result, DataTable DT1, DataTable DT2)
        {
            jSon = "";
            sec_jSon = "";
            try
            {
                DataSet ds = new DataSet();
                if (DT1.TableName == DT2.TableName)
                {
                    DT1.TableName = DT1.TableName;
                    DT2.TableName = DT2.TableName;
                }

                ds.Tables.Add(Result);
                ds.Tables.Add(DT1);
                ds.Tables.Add(DT2);
                jSon = JsonConvert.SerializeObject(ds, Formatting.Indented);
                sec_jSon = jSon;
                return sec_jSon;
            }
            catch (Exception e)
            {
                return e.Message;
            }            
        }

        public static string MakeNonJson(string status, string Msg, DataTable args)
        {
            jSon = "";
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            try
            {                                
                dt.Columns.Add("trxCode");
                dt.Columns.Add("trxMsg");
                DataRow row1 = dt.NewRow();
                row1["trxCode"] = status;
                row1["trxMsg"] = Msg;
                dt.Rows.Add(row1);
                dt.TableName = "Result";
                ds.Tables.Add(dt);
                if (status != "E" && args.Rows.Count > 0) ds.Tables.Add(args);
                //string strValue = JsonConvert.SerializeObject(ds);
                jSon = JsonConvert.SerializeObject(ds);
                return jSon;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public static string MakeNonJson(DataTable Result, DataTable DT1)
        {

            jSon = "";            
            try
            {
                DataSet ds = new DataSet();
                ds.Tables.Add(Result);
                ds.Tables.Add(DT1);
                jSon = JsonConvert.SerializeObject(ds, Formatting.Indented);
            }
            catch (Exception e)
            {
                return e.Message;
            }

            return jSon;
        }
        #endregion

        /// <summary>
        /// 결과값 DT로 만들어 주는 함수
        /// </summary>
        /// <param name="status"></param>
        /// <param name="Msg"></param>
        /// <returns></returns>
        public static DataTable MakeResultDT(string status, string Msg)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("trxCode");
            dt.Columns.Add("trxMsg");
            DataRow row1 = dt.NewRow();
            row1["trxCode"] = status;
            row1["trxMsg"] = Msg;
            dt.Rows.Add(row1);
            dt.TableName = "Result";
            return dt;
        }

        /// <summary>
        /// 랜덤 텍스트 가지고 오기 + 숫자
        /// </summary>
        /// <param name="numLength">텍스트 길이</param>
        /// <returns></returns>
        public static string fnGetRandomString(int numLength)
        {
            string strResult = "";
            Random rand = new Random();
            string strRandomChar = "qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM0123456789";
            StringBuilder rs = new StringBuilder();
            for (int i = 0; i < numLength; i++)
            {
                rs.Append(strRandomChar[(int)(rand.NextDouble() * strRandomChar.Length)]);
            }
            strResult = rs.ToString();

            return strResult;
        }

        public static string fnGetConfirmKey()
        {
            string strResult = "";
            Random rand = new Random();
            string strRandomChar = "qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM";
            string strRandomNum = "1234567890";
            string strRandomSC = "!@$%^()";
            StringBuilder rs = new StringBuilder();
            //특수문자 1 + 문자 3 + 숫자 1 + 문자 3 + 특수문자 1
            rs.Append(strRandomSC[(int)(rand.NextDouble() * strRandomSC.Length)]);
            rs.Append(strRandomChar[(int)(rand.NextDouble() * strRandomChar.Length)]);
            rs.Append(strRandomChar[(int)(rand.NextDouble() * strRandomChar.Length)]);
            rs.Append(strRandomChar[(int)(rand.NextDouble() * strRandomChar.Length)]);
            rs.Append(strRandomNum[(int)(rand.NextDouble() * strRandomNum.Length)]);
            rs.Append(strRandomChar[(int)(rand.NextDouble() * strRandomChar.Length)]);
            rs.Append(strRandomChar[(int)(rand.NextDouble() * strRandomChar.Length)]);
            rs.Append(strRandomChar[(int)(rand.NextDouble() * strRandomChar.Length)]);
            rs.Append(strRandomSC[(int)(rand.NextDouble() * strRandomSC.Length)]);
            strResult = rs.ToString();
            return strResult;
        }
    }
}