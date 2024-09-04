using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace EXPRESS_JWTNL.Models.Query
{
    public class Sql_Excel
    {
        private static string sSql = "";
        private static DataTable dt = new DataTable();


        public static string TestExcelData(DataTable dt)
        {
            
            string rtnData = _DataHelper.copyExcelData(dt);

            return rtnData;

        }
        public static string SaveExcelData(DataTable dt)
        {
            //string a;
            //_DataHelper.test(ds);
            //DataTable SQLdt =  _DataHelper.CallPRoc(dt.Rows[0]["MNGT_NO"].ToString(), dt.Rows[0]["MBL_NO"].ToString());

            string rtnState = _DataHelper.CallPRoc(dt.Rows[0]["MNGT_NO"].ToString(), dt.Rows[0]["MBL_NO"].ToString()).Rows[0]["R_RTNCD"].ToString();

            //if(SQLdt.Rows[0]["R_RTNCD"] == "Y")
            //{
            //    a = "Y";
            //}
            //else
            //{
            //    a = "E";
            //}

            //SQLdt.Rows[0]["R_RTNCD"].ToString();

            return rtnState;

        }


        public static string DeleteSkuDetail(DataRow dr)
        {
            sSql = "";
            sSql += " DELETE ";
            sSql += "   FROM EXCEL_SKU_MST ";
            sSql += "  WHERE 1=1" ;
            sSql += "AND HBL_NO = '" + dr["WAYBILL_NO"] + "'";
            sSql += "AND SEQ = '" + dr["SEQ"] + "'";

            return sSql;
        }

        public static string DeleteHblMst(DataRow dr)
        {
            sSql = "";
            sSql += " DELETE ";
            sSql += "   FROM EXCEL_HBL_MST ";
            sSql += "  WHERE HBL_NO = '" + dr["WAYBILL_NO"] + "'";

            return sSql;
        }


        public static string DeleteMblMst(DataRow dr)
        {
            sSql = "";
            sSql += " DELETE ";
            sSql += "   FROM EXCEL_MBL_MST ";
            sSql += "  WHERE MBL_NO = '" + dr["MBL_NO"] + "'";

            return sSql;
        }


        public static DataTable SearchSkuData(DataRow dr)
        {
            sSql = "";
            sSql += " SELECT HBL_NO ";
            sSql += "   FROM EXCEL_SKU_MST ";
            sSql += "  WHERE HBL_NO = '" + dr["WAYBILL_NO"] + "'";

            dt = _DataHelper.ExecuteDataTable(sSql, CommandType.Text);
            return dt;
        }

        public static DataTable SearchHblData(DataRow dr)
        {
            sSql = "";
            sSql += " SELECT MBL_NO ";
            sSql += "   FROM EXCEL_HBL_MST ";
            sSql += "  WHERE MBL_NO = '" + dr["MBL_NO"] + "'";

            dt = _DataHelper.ExecuteDataTable(sSql, CommandType.Text);
            return dt;
        }


    }
}
