using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace EXPRESS_JWTNL.Models.Query
{
    public class Sql_Common
    {
        private static string sSql = "";
        private static DataTable dt = new DataTable();


        public static DataTable SearchCommonCode(DataRow dr)
        {
            string sSql = "";

            sSql += "SELECT COMN_CD , CD_NM FROM MDM_COM_CODE WHERE GRP_CD= '" + dr["COMM_CD"] + "' AND USE_YN = 'Y' ";

            dt = _DataHelper.ExecuteDataTable(sSql, CommandType.Text);

            return dt;

        }

        public static DataTable SearchUserInfo(DataRow dr)
        {
            string sSql = "";

            sSql += "SELECT * FROM MDM_EXT_USR_MST WHERE USR_ID = '" + dr["USR_ID"] + "'";
            sSql += " AND PSWD = '" + YJIT.Utils.StringUtils.Md5Hash((string)dr["PSWD"]) + "' ";

            dt = _DataHelper.ExecuteDataTable(sSql, CommandType.Text);

            return dt;

        }


        public static int SaveMasterData(DataRow dr)
        {
            string sSql = "";
            int i = 0;

            sSql += "           BEGIN";
            sSql += "           UPDATE EXCEL_MBL_MST SET POL_CD = '" + dr["POL_CD"] + "'";
            sSql += "                        ,POD_CD = '" + dr["POD_CD"] + "'";
            sSql += "                        ,ETA = '" + dr["ETA"] + "'";
            sSql += "                        ,LINE_CD = '" + dr["LINE_CD"] + "'";
            sSql += "                        ,MBL_NO = '" + dr["NEW_MBL_NO"] + "'";
            sSql += "                        ,VSL = '" + dr["VSL"] + "'";
            sSql += "                        ,VOY = '" + dr["VOY"] + "'";
            sSql += "           WHERE MBL_NO = '" + dr["MBL_NO"] + "';";

            sSql += "           UPDATE EXCEL_HBL_MST SET MBL_NO = '" + dr["NEW_MBL_NO"] + "'";
            sSql += "           WHERE MBL_NO = '" + dr["MBL_NO"] + "';";


            sSql += "           UPDATE EXCEL_HBL_SKU SET MBL_NO = '" + dr["NEW_MBL_NO"] + "'";
            sSql += "           WHERE MBL_NO = '" + dr["MBL_NO"] + "';";

            sSql += "           END;";
            //}
            i = _DataHelper.ExecuteNonQuery(sSql, CommandType.Text);

            return i;
        }

            public static int SaveHouseData(DataRow dr)
        {
            string sSql = "";
            int i = 0;
            //if (dr["WEB_TYPE"].ToString() == "W")
            //{
            //    sSql += "UPDATE EXP_HBL_MST SET WURL_ID = '" + dr["WEB_ID"] + "'";
            //    sSql += "                      ,POL_CD = '" + dr["POL_CD"] + "'";
            //    sSql += "                      ,POD_CD = '" + dr["POD_CD"] + "'";
            //    sSql += "                      ,POL_NM = (SELECT LOC_NM FROM MDM_PORT_MST WHERE LOC_CD = '" + dr["POL_CD"] + "')";
            //    sSql += "                      ,POD_NM = (SELECT LOC_NM FROM MDM_PORT_MST WHERE LOC_CD = '" + dr["POD_CD"] + "')";
            //    sSql += "                      ,CNE_REG_NO = '" + dr["CNE_REG_NO"] + "'";
            //    sSql += "                      ,PKG = '" + dr["PKG"] + "'";
            //    sSql += "                      ,GRS_WGT = " + dr["GRS_WGT"];
            //    sSql += "                      ,MSRMT = " + dr["MSRMT"];
            //    sSql += "                      ,PKG_UNIT_CD = '" + dr["PKG_UNIT_CD"] + "'";
            //    sSql += "                      ,CURR_CD = '" + dr["CURR_CD"] + "'";
            //    sSql += "                      ,CUSTOMS_CI_VALUE = '" + dr["CUSTOMS_CI_VALUE"] + "'";
            //    sSql += "                      ,CUSTOM_IMP_HS_CD = '" + dr["PKG_UNIT_CD"] + "'";
            //    sSql += "                      ,MAIN_ITEM_NM = '" + dr["MAIN_ITEM_NM"] + "'";
            //    sSql += "                      ,CUSTOM_IMP_TYPE = '" + dr["CUSTOM_IMP_TYPE"] + "'";
            //    sSql += "                      ,WURL = '" + dr["WURL"] + "'";
            //    sSql += "                      ,NFY_NM_ENG = '" + dr["NFY_NM"] + "'";
            //    sSql += "                      ,SHP_NM_ENG = '" + dr["SHP_NM_ENG"] + "'";
            //    sSql += "                      ,SHP_ADDR = '" + dr["SHP_ADDR"] + "'";
            //    sSql += "                      ,SHP_TEL_NO = '" + dr["SHP_TEL_NO"] + "'";
            //    sSql += "                      ,SHP_CTRY_CD = '" + dr["SHP_CTRY_CD"] + "'";
            //    sSql += "                      ,CNE_NM_ENG = '" + dr["CNE_NM_ENG"] + "'";
            //    sSql += "                      ,CNE_ADDR = '" + dr["CNE_ADDR"] + "'";
            //    sSql += "                      ,CNE_TEL_NO = '" + dr["CNE_TEL_NO"] + "'";
            //    sSql += "                      ,CNE_ZIP_NO = '" + dr["CNE_ZIP_NO"] + "'";
            //    sSql += "                      ,CNE_EMAIL = '" + dr["CNE_EMAIL"] + "'";
            //    sSql += "                      ,FRGN_PROXY_CD = '" + dr["FRGN_PROXY_CD"] + "'";
            //    sSql += "                      ,FRGN_PROXY_NM = '" + dr["FRGN_PROXY_NM"] + "'";
            //    sSql += "                      ,BUY_PROXY_CD = '" + dr["BUY_PROXY_CD"] + "'";
            //    sSql += "                      ,BUY_PROXY_NM = '" + dr["BUY_PROXY_NM"] + "'";
            //    sSql += "                      ,SELL_PROXY_CD = '" + dr["SELL_PROXY_CD"] + "'";
            //    sSql += "                      ,SELL_PROXY_NM = '" + dr["SELL_PROXY_NM"] + "'";
            //    sSql += "                      ,RMK = '" + dr["RMK"] + "'";
            //    sSql += "                      ,TRUCK_CD = '" + dr["TRUCK_CD"] + "'";
            //    sSql += "                      ,ENTRY_TYPE = '" + dr["ENTRY_TYPE"] + "'";
            //    sSql += "                      ,HWH_NM = '" + dr["HWH_NM"] + "'";
            //    sSql += "                      ,HWH_CD = '" + dr["HWH_CD"] + "'";
            //    sSql += "                      ,SHIPPING_DATE = '" + dr["SHIPPING_DATE"] + "'";
            //    sSql += "                      ,SHIPPING_NAME = '" + dr["SHIPPING_NAME"] + "'";
            //    sSql += "                      ,CUSTOM_ISSUE_DATE = '" + dr["CUSTOM_ISSUE_DATE"] + "'";
            //    sSql += "                      ,CONSIGNEE_ADDR = '" + dr["CONSIGNEE_ADDR"] + "'";
            //    sSql += "                      ,REPLACE_REMARK = '" + dr["REPLACE_REMARK"] + "'";
            //    sSql += "                      ,UPD_USR = '" + dr["USR_ID"] + "'";
            //    sSql += "                      ,UPD_YMD = UFN_DATE_FORMAT('DATE') ";
            //    sSql += "                      ,UPD_HM = UFN_DATE_FORMAT('TIME') ";
            //    sSql += "           WHERE HBL_ID = '" + dr["HBL_ID"] + "'";
            //}
            //else {
                sSql += "           BEGIN";
                sSql += "           UPDATE EXCEL_MBL_MST SET POL_CD = '" + dr["POL_CD"] + "'";
                sSql += "                        ,POD_CD = '" + dr["POD_CD"] + "'";
                sSql += "                        ,ETA = '" + dr["ETA"] + "'";
                sSql += "           WHERE MBL_NO = '" + dr["MBL_NO"] + "';";
                //sSql += "                      ,POL_NM = (SELECT LOC_NM FROM MDM_PORT_MST WHERE LOC_CD = '" + dr["POL_CD"] + "')";
                //sSql += "                      ,POD_NM = (SELECT LOC_NM FROM MDM_PORT_MST WHERE LOC_CD = '" + dr["POD_CD"] + "')";
                sSql += "           UPDATE EXCEL_HBL_MST SET CNE_REG_NO = '" + dr["CNE_REG_NO"] + "'";
                sSql += "                      ,PKG = '" + dr["PKG"] + "'";
                sSql += "                      ,GRS_WGT = " + dr["GRS_WGT"];
                sSql += "                      ,VOL_WGT = " + dr["MSRMT"];
                sSql += "                      ,PKG_UNIT_CD = '" + dr["PKG_UNIT_CD"] + "'";
                //sSql += "                      ,CURR_EXCEL_HBL_MSTCD = '" + dr["CURR_CD"] + "'";
                sSql += "                      ,CUSTOMS_CI_VALUE = '" + dr["CUSTOMS_CI_VALUE"] + "'";
                sSql += "                      ,CUSTOM_IMP_HS_CD = '" + dr["PKG_UNIT_CD"] + "'";
                sSql += "                      ,MAIN_ITEM_NM = '" + dr["MAIN_ITEM_NM"] + "'";
                sSql += "                      ,CUSTOM_IMP_TYPE = '" + dr["CUSTOM_IMP_TYPE"] + "'";
                sSql += "                      ,WURL = '" + dr["WURL"] + "'";
                sSql += "                      ,NFY_NM_ENG = '" + dr["NFY_NM"] + "'";
                sSql += "                      ,SHP_NM_ENG = '" + dr["SHP_NM_ENG"] + "'";
                sSql += "                      ,SHP_ADDR = '" + dr["SHP_ADDR"] + "'";
                sSql += "                      ,SHP_TEL_NO = '" + dr["SHP_TEL_NO"] + "'";
                sSql += "                      ,SHP_CTRY_CD = '" + dr["SHP_CTRY_CD"] + "'";
                sSql += "                      ,CNE_NM_ENG = '" + dr["CNE_NM_ENG"] + "'";
                sSql += "                      ,CNE_ADDR = '" + dr["CNE_ADDR"] + "'";
                sSql += "                      ,CNE_TEL_NO = '" + dr["CNE_TEL_NO"] + "'";
                //sSql += "                      ,CNE_ZIP_NO = '" + dr["CNE_ZIP_NO"] + "'";
                sSql += "                      ,CNE_EMAIL = '" + dr["CNE_EMAIL"] + "'";
                sSql += "                      ,FRGN_PROXY_CD = '" + dr["FRGN_PROXY_CD"] + "'";
                sSql += "                      ,FRGN_PROXY_NM = '" + dr["FRGN_PROXY_NM"] + "'";
                sSql += "                      ,BUY_PROXY_CD = '" + dr["BUY_PROXY_CD"] + "'";
                sSql += "                      ,BUY_PROXY_NM = '" + dr["BUY_PROXY_NM"] + "'";
                sSql += "                      ,SELL_PROXY_CD = '" + dr["SELL_PROXY_CD"] + "'";
                sSql += "                      ,SELL_PROXY_NM = '" + dr["SELL_PROXY_NM"] + "'";
                sSql += "                      ,RMK = '" + dr["RMK"] + "'";
                sSql += "           WHERE HBL_NO = '" + dr["HBL_NO"] + "';";
                sSql += "           END;";
            //}
            i = _DataHelper.ExecuteNonQuery(sSql, CommandType.Text);

            return i;

        }
        public static int DeleteHouseData(DataRow dr)
        {
            string sSql = "";
            int i = 0;

            sSql += "           DELETE EXCEL_HBL_MST ";
            sSql += "           WHERE HBL_NO = '" + dr["HBL_NO"] + "'";
            i = _DataHelper.ExecuteNonQuery(sSql, CommandType.Text);

            return i;

        }


        public static int SaveHouseAll(DataRow dr)
        {
            string sSql = "";
            int i = 0;

            sSql += "           UPDATE EXCEL_HBL_MST SET ";
            if (dr.Table.Columns.Contains("CNE_USE_TYPE"))
            {
                sSql += "                      CNE_USE_TYPE = '" + dr["CNE_USE_TYPE"] + "',";
            }
            if (dr.Table.Columns.Contains("CUSTOM_IMP_TYPE"))
            {
                sSql += "                      CUSTOM_IMP_TYPE = '" + dr["CUSTOM_IMP_TYPE"] + "',";
            }
            if (dr.Table.Columns.Contains("SHP_NM_ENG"))
            {
                sSql += "                      SHP_NM_ENG = '" + dr["SHP_NM_ENG"] + "',";
            }
            if (dr.Table.Columns.Contains("WURL"))
            {
                sSql += "                      WURL = '" + dr["WURL"] + "',";
            }
            if (dr.Table.Columns.Contains("SHP_ADDR_ENG"))
            {
                sSql += "                      SHP_ADDR_ENG = '" + dr["SHP_ADDR_ENG"] + "',";
            }
            //if (dr.Table.Columns.Contains("SHP_NM_LOC"))
            //{
            //    sSql += "                      SHP_NM_LOC = '" + dr["SHP_NM_LOC"] + "',";
            //}
            if (dr.Table.Columns.Contains("CUSTOM_EXP_HS_CD"))
            {
                sSql += "                      CUSTOM_IMP_HS_CD = '" + dr["CUSTOM_EXP_HS_CD"] + "',";
            }
            //if (dr.Table.Columns.Contains("SHP_ADDR_LOC"))
            //{
            //    sSql += "                      SHP_ADDR_LOC = '" + dr["SHP_ADDR_LOC"] + "',";
            //}
            if (dr.Table.Columns.Contains("CNE_NM_LOC"))
            {
                sSql += "                      CNE_NM_LOC = '" + dr["CNE_NM_LOC"] + "',";
            }
            if (dr.Table.Columns.Contains("CNE_ADDR"))
            {
                sSql += "                      CNE_ADDR = '" + dr["CNE_ADDR"] + "',";
            }
            if (dr.Table.Columns.Contains("CUSTOM_EXP_HS_CD"))
            {
                sSql += "                      CUSTOM_EXP_HS_CD = '" + dr["CUSTOM_EXP_HS_CD"] + "',";
            }
            if (dr.Table.Columns.Contains("PKG"))
            {
                sSql += "                      PKG = " + dr["PKG"] + ",";
            }
            if (dr.Table.Columns.Contains("GRS_WGT"))
            {
                sSql += "                      GRS_WGT = " + dr["GRS_WGT"] + ",";
            }
            if (dr.Table.Columns.Contains("MSRMT"))
            {
                sSql += "                      MSRMT = " + dr["MSRMT"] + ",";
            }
            if (dr.Table.Columns.Contains("MAIN_ITEM_NM"))
            {
                sSql += "                      MAIN_ITEM_NM = '" + dr["MAIN_ITEM_NM"] + "',";
            }
            if (dr.Table.Columns.Contains("STATEMENT_CD"))
            {
                sSql += "                      STATEMENT_CD = '" + dr["STATEMENT_CD"] + "',";
            }
            if (dr.Table.Columns.Contains("CNE_ADDR"))
            {
                sSql += "                      CNE_ADDR = '" + dr["CNE_ADDR"] + "',";
            }
            if (dr.Table.Columns.Contains("CNE_TEL_NO"))
            {
                sSql += "                      CNE_TEL_NO = '" + dr["CNE_TEL_NO"] + "',";
            }
            if (dr.Table.Columns.Contains("CNE_EMAIL"))
            {
                sSql += "                      CNE_EMAIL = '" + dr["CNE_EMAIL"] + "',";
            }
            if (dr.Table.Columns.Contains("FRGN_PROXY_CD"))
            {
                sSql += "                      FRGN_PROXY_CD = '" + dr["FRGN_PROXY_CD"] + "',";
            }
            if (dr.Table.Columns.Contains("FRGN_PROXY_NM"))
            {
                sSql += "                      FRGN_PROXY_NM = '" + dr["FRGN_PROXY_NM"] + "',";
            }
            if (dr.Table.Columns.Contains("BUY_PROXY_CD"))
            {
                sSql += "                      BUY_PROXY_CD = '" + dr["BUY_PROXY_CD"] + "',";
            }
            if (dr.Table.Columns.Contains("BUY_PROXY_NM"))
            {
                sSql += "                      BUY_PROXY_NM = '" + dr["BUY_PROXY_NM"] + "',";
            }
            if (dr.Table.Columns.Contains("SELL_PROXY_CD"))
            {
                sSql += "                      SELL_PROXY_CD = '" + dr["SELL_PROXY_CD"] + "',";
            }
            if (dr.Table.Columns.Contains("SELL_PROXY_NM"))
            {
                sSql += "                      SELL_PROXY_NM = '" + dr["SELL_PROXY_NM"] + "',";
            }
            if (dr.Table.Columns.Contains("PTN_ORD_NO"))
            {
                sSql += "                      PTN_ORD_NO = '" + dr["PTN_ORD_NO"] + "',";
            }
            if (dr.Table.Columns.Contains("CNTR_AGT_CD"))
            {
                sSql += "                      CNTR_AGT_CD = '" + dr["CNTR_AGT_CD"] + "',";
            }
            if (dr.Table.Columns.Contains("WURL_ID"))
            {
                sSql += "                      WURL_ID = '" + dr["WURL_ID"] + "',";
            }
            sSql += "                      UPD_USR = '" + dr["USR_ID"] + "',";
            sSql += "                      UPD_YMD = UFN_DATE_FORMAT('DATE'),";
            sSql += "                      UPD_HM = UFN_DATE_FORMAT('TIME')";
            if (dr["BL_TYPE"].ToString() == "M")
            {
                sSql += "           WHERE MBL_NO IN('" + dr["MBL_NO"] + "')";
            }
            else {
                sSql += "           WHERE HBL_NO IN('" + dr["MBL_NO"] + "')";
            }
                //}
                i = _DataHelper.ExecuteNonQuery(sSql, CommandType.Text);

            return i;

        }
    }
}
