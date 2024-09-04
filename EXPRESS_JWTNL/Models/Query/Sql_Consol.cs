using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using System.Configuration;

namespace EXPRESS_JWTNL.Models.Query
{
    public class Sql_Consol
    {
        private static string sSql = "";
        private static bool rtnBool = false;
        private static DataTable dt = new DataTable();



        #region 오더 조회 영역
        public static DataTable SearchMstData(DataRow dr)
        {
            sSql = "";

            sSql += " SELECT MBL_NO";
            sSql += "      , VOY";
            sSql += "      , POL";
            sSql += "      , POD";
            sSql += "      , ETD";
            sSql += "      , ETA";
            sSql += "      , IE_TYPE";
            sSql += "      , TRANS_TYPE";
            sSql += "      , INS_YMD";
            sSql += "      , INS_HM";
            sSql += " FROM EXCEL_MBL_MST A";
            sSql += " WHERE 1=1";
            if (dr["SEARCH_TYPE"].ToString() == "Master B/L")
            {
                sSql += "AND A.MBL_NO = '" + dr["SEARCH_VALUE"].ToString() + "'";
                sSql += "    AND A.INS_YMD >= '" + dr["STRT_YMD"].ToString() + "'";
                sSql += "    AND A.INS_YMD <= '" + dr["END_YMD"].ToString() + "'";
            }
            else if (dr["SEARCH_TYPE"].ToString() == "PopUp")
            {
                sSql += "AND A.MBL_NO = (";
                sSql += "  SELECT MBL_NO";
                sSql += "  FROM EXCEL_HBL_MST";
                sSql += "  WHERE 1=1";
                sSql += "AND HBL_NO = '" + dr["SEARCH_VALUE"].ToString() + "')";
            }
            else
            {
                sSql += "AND A.MBL_NO = (";
                sSql += "  SELECT MBL_NO";
                sSql += "  FROM EXCEL_HBL_MST";
                sSql += "  WHERE 1=1";
    
                if (dr["SEARCH_TYPE"].ToString() == "House B/L")
                {
                    sSql += "AND HBL_NO = '" + dr["SEARCH_VALUE"].ToString() + "'";
                }
                else if (dr["SEARCH_TYPE"].ToString() == "Location ID")
                {
                    sSql += "AND LOGISTIC_CD = '" + dr["SEARCH_VALUE"].ToString() + "'";
                }

                sSql += " ) ";
                sSql += "    AND A.INS_YMD >= '" + dr["STRT_YMD"].ToString() + "'";
                sSql += "    AND A.INS_YMD <= '" + dr["END_YMD"].ToString() + "'";
            }

            //sSql += "    AND A.INS_YMD >= '" + dr["STRT_YMD"].ToString() + "'";
            //sSql += "    AND A.INS_YMD <= '" + dr["END_YMD"].ToString() + "'";

            dt = _DataHelper.ExecuteDataTable(sSql, CommandType.Text);

            return dt;

        }
        public static DataTable SearchDtlData(DataRow dr)
        {
            sSql = "";

            sSql += " SELECT HBL.HBL_NO";
            sSql += "      , HBL.MBL_NO";
            sSql += "      , HBL.LOGISTIC_CD";
            sSql += "      , HBL.COP_NO";
            sSql += "      , HBL.SHP_NM";
            sSql += "      , HBL.SHP_ADDR";
            sSql += "      , HBL.CNE_NM";
            sSql += "      , HBL.CNE_ADDR";
            sSql += "      , HBL.RET_NM";
            sSql += "      , HBL.RET_ADDR";
            sSql += "      , HBL.TOT_PRICE";
            sSql += "      , SKU.SKU_PRICE";
            sSql += "      , SKU.TAX_PRICE";
            sSql += "      , SKU.POST_PRICE";
            sSql += "      , SKU.GRS_WGT";
            sSql += "      , HBL.CURR_NM";
            sSql += "      , HBL.STATUS";
            sSql += "      , HBL.UNI_YN";
            sSql += "      , HBL.INS_YMD";
            sSql += "      , HBL.INS_HM";
            sSql += "      , HBL.CLEAER_MODE";
            sSql += "      , HBL.UNIPASS_CARGO_INFO_ID";
            //sSql += "      , HBL.SEQ";
            sSql += "      , SKU.SEQ AS SEQ";
            sSql += "      , SKU.ITEM_NM";
            sSql += "      , SKU.HS_CD";
            sSql += "      , SKU.ITEM_ID";
            sSql += "      , NVL2(API.RTN_STATUS,'Y','N') AS  FAIL_YN";

            /*기존 NEXT_SEND 조건 변경 및 JOIN 사용 */
            //sSql += "      , (SELECT MIN(RTN_STATUS) FROM  CAINIAO_SEND_DATA WHERE GUID = HBL.UNIPASS_CARGO_INFO_ID AND SEND_YN = 'N') AS NEXT_SEND"; 
            sSql += "      , EXCEP.STAT AS NEXT_SEND ";

            sSql += "      , MBL.VOY";
            sSql += "      , MBL.POL";
            sSql += "      , MBL.POD";
            sSql += "      , MBL.ETD";
            sSql += "      , MBL.ETA";
            sSql += "      , MBL.IE_TYPE";
            sSql += "      , MBL.TRANS_TYPE";
            sSql += "      , HBL.PASS_PORT ";
            sSql += "      , HBL.PARTNER_CD ";
            sSql += "      , HBL.BIG_BAG ";
            sSql += "      , HBL.SHP_MO ";
            sSql += "      , HBL.CNE_MO ";
            sSql += "      , SKU.P_URL ";
            sSql += "      , SKU.QTY ";
            sSql += "      , HBL.CNE_ZIP ";
            sSql += "      , HBL.CNE_MAIL ";
            sSql += " FROM EXCEL_HBL_MST HBL";
            sSql += " LEFT JOIN EXCEL_SKU_MST SKU";
            sSql += " ON  HBL.HBL_NO = SKU.HBL_NO";
            sSql += " LEFT JOIN  CAINIAO_SEND_DATA API";
            sSql += " ON HBL.HBL_NO = API.HBL_NO";
            sSql += " AND (API.RTN_STATUS = '2040')";

            sSql += "LEFT JOIN (SELECT HBL_NO , MAX(RTN_STATUS)AS RTN_STATUS FROM CAINIAO_SEND_DATA WHERE RTN_STATUS ='2050' OR RTN_STATUS LIKE '40%' GROUP BY HBL_NO) API2 ";
            sSql += "   ON  HBL.HBL_NO = API2.HBL_NO ";


            /*기존 NEXT_SEND 조건 변경 및 JOIN 사용 */
            sSql += " LEFT JOIN ( SELECT HBL_NO , MAX(SEQ)AS API_SEQ , MAX(RTN_STATUS) KEEP (DENSE_RANK FIRST ORDER BY SEQ DESC) STAT ";
            sSql += "               FROM CAINIAO_SEND_DATA ";
            sSql += "               WHERE 1=1 AND SEND_YN = 'Y' ";

            if (dr["SEARCH_TYPE"].ToString() == "MBL")
            {
                sSql += "AND MBL_NO ='" + dr["SEARCH_VALUE"].ToString() + "' ";
            }

            sSql += "               GROUP BY HBL_NO  ";
            sSql += " ) EXCEP";
            sSql += "       ON HBL.HBL_NO = EXCEP.HBL_NO ";


                
            sSql += " LEFT JOIN EXCEL_MBL_MST MBL";
            sSql += " ON  MBL.MBL_NO = HBL.MBL_NO";
            sSql += " WHERE 1=1";
            if (dr["SEARCH_TYPE"].ToString() == "MBL")
            {
                sSql += "AND HBL.MBL_NO = '" + dr["SEARCH_VALUE"].ToString() + "'";
                sSql += "    AND HBL.INS_YMD >= '" + dr["STRT_YMD"].ToString() + "'";
                sSql += "    AND HBL.INS_YMD <= '" + dr["END_YMD"].ToString() + "'";

                //int len = dr["MULTI_NO"].ToString().Split(',').Length;


                //멀티 조회 기능 추가
                string[] bl_list = dr["MULTI_NO"].ToString().Split(',');
                bl_list = bl_list.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray(); //빈배열 제거
                int blseq = 1;

                if(bl_list.Length > 0)
                {
                    sSql += "AND ( ";
                    foreach (var blone in bl_list)
                    {
                        
                        if (!String.IsNullOrEmpty(blone.ToString()))
                        {
                            if(blseq == 1)
                            {
                                sSql += "   HBL.HBL_NO = '"+blone+"' ";
                                if(bl_list.Length == 1)
                                {
                                    sSql += ")";
                                }
                            }
                            else
                            {
                                sSql += "  OR  HBL.HBL_NO = '" + blone + "' ";
                                if(bl_list.Length == blseq)
                                {
                                    sSql += ")";
                                }
                            }
                        }
                        blseq += 1;
                    }

                }
            }
            else if (dr["SEARCH_TYPE"].ToString() == "PopUp")
            {
                sSql += "AND HBL.HBL_NO = (";
                sSql += "  SELECT HBL_NO";
                sSql += "  FROM EXCEL_HBL_MST";
                sSql += "  WHERE 1=1";
                sSql += "AND HBL_NO = '" + dr["SEARCH_VALUE"].ToString() + "')";
            }
            else
            {
                if (dr["SEARCH_TYPE"].ToString() == "HBL")
                {
                    sSql += "AND HBL.HBL_NO = (";
                    sSql += "  SELECT HBL_NO";
                    sSql += "  FROM EXCEL_HBL_MST";
                    sSql += "  WHERE 1=1";
                    sSql += "AND HBL_NO = '" + dr["SEARCH_VALUE"].ToString() + "'";
                }
                else if (dr["SEARCH_TYPE"].ToString() == "LOC")
                {
                    sSql += "AND HBL.LOGISTIC_CD = (";
                    sSql += "  SELECT LOGISTIC_CD";
                    sSql += "  FROM EXCEL_HBL_MST";
                    sSql += "  WHERE 1=1";
                    sSql += "AND LOGISTIC_CD = '" + dr["SEARCH_VALUE"].ToString() + "'";
                }

                sSql += " ) ";
                sSql += "    AND HBL.INS_YMD >= '" + dr["STRT_YMD"].ToString() + "'";
                sSql += "    AND HBL.INS_YMD <= '" + dr["END_YMD"].ToString() + "'";
            }

            //sSql += "    AND HBL.INS_YMD >= '" + dr["STRT_YMD"].ToString() + "'";
            //sSql += "    AND HBL.INS_YMD <= '" + dr["END_YMD"].ToString() + "'";
            dt = _DataHelper.ExecuteDataTable(sSql, CommandType.Text);

           return dt;

        }

        public static DataTable SearchUniMstData(DataRow dr)
        {
            sSql = "";

            sSql += " SELECT A.MBLNO";
            sSql += "      , A.HBLNO";
            sSql += "      , A.CSCLPRGSSTTS";
            sSql += "      , A.PRCSDTTM";
            sSql += "      , A.MNGT_ID";
            sSql += " FROM UNIPASS_CARGO_INFO_MST A";
            sSql += " WHERE 1=1";
            //if (dr["SEARCH_TYPE"].ToString() == "PopUp")
            //{
            //    sSql += "AND A.HBL_NO = '" + dr["SEARCH_VALUE"].ToString() + "'";
            //}
            sSql += " AND MNGT_ID  ='"+dr["SEARCH_VALUE2"].ToString() +"' ";

            dt = _DataHelper.ExecuteDataTable(sSql, CommandType.Text);

            return dt;

        }

        public static DataTable SearchHoldData(DataRow dr)
        {

            sSql = "";
            sSql += "SELECT  (SELECT MAX(HBL_NO) FROM CAINIAO_SEND_DATA WHERE GUID = '"+dr["MNGT_ID"].ToString()+"' ) AS \"HBL_NO\" ";
            sSql += " , (SELECT RTN_STATUS FROM CAINIAO_SEND_DATA WHERE GUID = '" + dr["MNGT_ID"].ToString() + "'";
            //sSql += " AND (RTN_STATUS= '3004' OR RTN_STATUS= '3012') ) AS \"RTN_30\" ";
            sSql += " AND RTN_STATUS IN (SELECT SATAUS_CD FROM MAP_CAINIAO_CODE WHERE STD_CD = 'EXPT1')) AS \"RTN_30\" ";
            sSql += " , (SELECT RTN_STATUS FROM CAINIAO_SEND_DATA WHERE GUID = '" + dr["MNGT_ID"].ToString() + "'";
            //sSql += " AND (RTN_STATUS= '4001' OR RTN_STATUS= '4004' OR RTN_STATUS= '4008') ) AS \"RTN_40\" ";
            sSql += " AND RTN_STATUS IN (SELECT SATAUS_CD FROM MAP_CAINIAO_CODE WHERE STD_CD = 'EXPT2')) AS \"RTN_40\" ";
            sSql += " FROM DUAL ";

            dt = _DataHelper.ExecuteDataTable(sSql, CommandType.Text);
            return dt;
        }

        public static DataTable SearchUniDtlData(DataRow dr)
        {
            sSql = "";

            sSql += " SELECT DTL.SHEDNM";
            sSql += "      , DTL.CARGTRCNRELABSOPTPCD";
            sSql += "      , DTL.PRCSDTTM";
            sSql += "      , DTL.SEQ";
            //sSql += "      , DTL.SEND_YN";
            sSql += "      , CASE WHEN  API.SEND_YN = 'N' THEN '미전송' ";
            sSql += "       WHEN API.SEND_YN = 'Y' THEN '전송완료'";
            sSql += "       WHEN API.SEND_YN = 'F' THEN '전송 실패'";
            sSql += "       WHEN API.SEND_YN = 'E' THEN '미반입 전송'";
            sSql += "       END AS SEND_YN";
            sSql += " FROM UNIPASS_CARGO_INFO_DTL DTL";
            sSql += " LEFT JOIN (SELECT RTN_KR, GUID, MAX(SEND_YN) AS SEND_YN, UP_SEQ FROM CAINIAO_SEND_DATA WHERE GUID ='"+ dr["SEARCH_VALUE2"].ToString() + "' GROUP BY RTN_KR, GUID,UP_SEQ) API";
            sSql += "   ON DTL.CARGTRCNRELABSOPTPCD = API.RTN_KR ";
            sSql += "   AND API.UP_SEQ = DTL.SEQ ";
            sSql += "   AND DTL.MNGT_ID = API.GUID ";
            sSql += " WHERE 1=1";
            //if (dr["SEARCH_TYPE"].ToString() == "PopUp")
            //{
            //    sSql += "AND DTL.HBL_NO = '" + dr["SEARCH_VALUE"].ToString() + "'";
            //}
            sSql += " AND MNGT_ID  ='" + dr["SEARCH_VALUE2"].ToString() + "' ";
            sSql += " ORDER BY DTL.SEQ ASC";
            dt = _DataHelper.ExecuteDataTable(sSql, CommandType.Text);

            return dt;

        }


        public static DataTable SearchExcep1()
        {
            string sSql = "";
            sSql += "SELECT SATAUS_CD, '(' || SATAUS_CD || ') ' || STATUS_US AS NAME ";
            sSql += "   FROM MAP_CAINIAO_CODE ";
            sSql += "   WHERE STD_CD = 'EXPT1' ";
            sSql += " ORDER BY SATAUS_CD ";

            dt = _DataHelper.ExecuteDataTable(sSql, CommandType.Text);

            return dt;

        }

        public static DataTable SearchExcep2()
        {
            string sSql = "";
            sSql += "SELECT '0000' AS SATAUS_CD , '--'||'FINAL STATE'||'--' AS NAME ";
            sSql += "FROM MAP_CAINIAO_CODE ";
            sSql += "UNION ";
            sSql += "SELECT SATAUS_CD, '(' || SATAUS_CD || ') ' || STATUS_US AS NAME ";
            sSql += "   FROM MAP_CAINIAO_CODE ";
            sSql += "   WHERE STD_CD = 'EXPT2' ";
            sSql += " ORDER BY SATAUS_CD ";

            dt = _DataHelper.ExecuteDataTable(sSql, CommandType.Text);

            return dt;

        }


        public static DataTable SearchData()
        {
            string sSql = "";

            sSql += " SELECT MBL_NO	";
            sSql += "      , HBL_NO	";
            sSql += "      , INS_TIME	";
            sSql += " FROM EXCEL_HBL_MST A	";

            dt = _DataHelper.ExecuteDataTable(sSql, CommandType.Text);

            return dt;

        }
        public static bool LeftInsertHold(DataRow dr)
        {
            string sSql = "";


            sSql += "BEGIN ";

            sSql += "MERGE INTO CAINIAO_SEND_DATA ";
            sSql += "USING DUAL ";
            sSql += "ON ( GUID = '" + dr["MNGT_ID"].ToString() + "' AND RTN_STATUS = '"+ dr["LEFT_SEARCH_CD"].ToString() + "') ";
            sSql += "WHEN NOT MATCHED THEN  ";
            sSql += "INSERT (GUID , SEQ , COP_NO, HBL_NO , RTN_KR , RTN_STATUS, RTN_TIME ,  SEND_YN ) ";
            sSql += "VALUES('" + dr["MNGT_ID"].ToString() + "','7' , '" + dr["COP_NO"].ToString() + "' , '" + dr["WAYBILL_NO"].ToString() + "', (SELECT SATAUS_KR FROM MAP_CAINIAO_CODE WHERE SATAUS_CD ='"+ dr["LEFT_SEARCH_CD"].ToString() + "'), '" + dr["LEFT_SEARCH_CD"].ToString() + "' ,(SELECT (RTN_TIME+1)AS RTN_NEW FROM CAINIAO_SEND_DATA WHERE HBL_NO= '" + dr["WAYBILL_NO"].ToString() + "' AND RTN_STATUS = '2040')  , 'N'); ";


            if(dr["LEFT_SEARCH_CD"].ToString() == "3001")
            {
                sSql += "MERGE INTO CAINIAO_SEND_DATA ";
                sSql += "USING DUAL ";
                sSql += "ON ( GUID = '" + dr["MNGT_ID"].ToString() + "' AND RTN_STATUS = '3002') ";
                sSql += "WHEN NOT MATCHED THEN  ";
                sSql += "INSERT (GUID , SEQ , COP_NO, HBL_NO , RTN_KR , RTN_STATUS, RTN_TIME ,  SEND_YN ) ";
                sSql += "VALUES('" + dr["MNGT_ID"].ToString() + "','8' , '" + dr["COP_NO"].ToString() + "' , '" + dr["WAYBILL_NO"].ToString() + "', '관련문자전송', '3002' ,(SELECT (RTN_TIME+1)AS RTN_NEW FROM CAINIAO_SEND_DATA WHERE HBL_NO= '" + dr["WAYBILL_NO"].ToString() + "' AND RTN_STATUS = '2040')  , 'N'); ";
            }

            sSql += "END; ";

            int cnt = _DataHelper.ExecuteNonQuery(sSql, CommandType.Text);
            if (cnt == -1) rtnBool = true;
            return rtnBool;
        }

        public static bool RightInsertHold(DataRow dr)
        {
            string sSql = "";
            string seq_state = "8";
            if(dr["RIGHT_SEARCH_CD"].ToString() == "4004")
            {
                seq_state = "10";
            }

            sSql += "BEGIN ";

            sSql += "MERGE INTO CAINIAO_SEND_DATA ";
            sSql += "USING DUAL ";
            sSql += "ON ( GUID = '" + dr["MNGT_ID"].ToString() + "' AND RTN_STATUS = '"+ dr["RIGHT_SEARCH_CD"].ToString() + "') ";
            sSql += "WHEN NOT MATCHED THEN  ";
            sSql += "INSERT (GUID , SEQ , COP_NO, HBL_NO , RTN_KR , RTN_STATUS, RTN_TIME , SEND_YN ) ";
            sSql += "VALUES('" + dr["MNGT_ID"].ToString() + "','"+seq_state+"' , '" + dr["COP_NO"].ToString() + "' , '" + dr["WAYBILL_NO"].ToString() + "', (SELECT SATAUS_KR FROM MAP_CAINIAO_CODE WHERE SATAUS_CD = '"+dr["RIGHT_SEARCH_CD"].ToString()+"') , '" + dr["RIGHT_SEARCH_CD"].ToString() + "' ,TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS')  , 'N'); ";

            //수집 종료 처리
            sSql += "    UPDATE UNIPASS_CARGO_INFO_MST SET MIG_YN = 'Y' , SEARCH_YN = 'N' ";
            sSql += "       WHERE HBL_NO ='" + dr["WAYBILL_NO"].ToString() + "'; ";

            sSql += "   UPDATE EXCEL_HBL_MST SET UNI_YN = 'Y' ";
            sSql += "   WHERE HBL_NO ='" + dr["WAYBILL_NO"].ToString() + "'; ";
            //수집 종료 처리

            sSql += "END; ";


            int cnt = _DataHelper.ExecuteNonQuery(sSql, CommandType.Text);
            if (cnt == -1) rtnBool = true;
            return rtnBool;
        }

        #region 미사용(테스트)
        public static bool InsUnipass(DataRow dr)
        {

            string sSql = "";

            sSql += " MERGE INTO EXP_UNIPASS_MST";
            sSql += "      USING DUAL";
            sSql += "         ON (HBL_NO = '" + dr["hblNo"].ToString()+ "' AND MBL_NO = '" + dr["mblNo"].ToString() + "')";
            sSql += " WHEN MATCHED";
            sSql += " THEN";
            sSql += "    UPDATE SET CARGO_MNGT_NO = '" + dr["cargMtNo"].ToString() + "',";
            sSql += "               STATUS = '" + dr["prgsStts"].ToString() + "',";
            sSql += "               CARR_NM = '" + dr["shcoFlco"].ToString() + "',";
            sSql += "               VSL_NM = '" + dr["shipNm"].ToString() + "',";
            sSql += "               CUSTOM_STATUS = '" + dr["csclPrgsStts"].ToString() + "',";
            sSql += "               PROC_YMD = '" + dr["prcsDttm"].ToString().Substring(0, 8) + "',";
            sSql += "               PROC_HM = '" + dr["prcsDttm"].ToString().Substring(8, 6) + "',";
            sSql += "               CTRY_NM = '" + dr["shipNatNm"].ToString() + "',";
            sSql += "               SHIP_AGENT = '" + dr["agnc"].ToString() + "',";
            sSql += "               ITEM_NM = '" + dr["prnm"].ToString() + "',";
            sSql += "               POL_NM = '" + dr["ldprCd"].ToString() + "',";
            sSql += "               PKG = '" + dr["pckGcnt"].ToString() + "',";
            sSql += "               GRS_WGT = '" + dr["ttwg"].ToString() + "',";
            sSql += "               POD_NM = '" + dr["dsprCd"].ToString() + "',";
            sSql += "               ENT_CUSTOMS = '" + dr["etprCstm"].ToString() + "',";
            sSql += "               VOL_WGT = '" + dr["msrm"].ToString() + "',";
            sSql += "               BL_TYPE = '" + dr["blPtNm"].ToString() + "',";
            sSql += "               LOAD_YMD = '" + dr["etprDt"].ToString() + "',";
            sSql += "               VOY = '" + dr["vydf"].ToString() + "',";
            sSql += "               CARE_YN = '" + dr["mtTrgtCargYnNm"].ToString() + "',";
            sSql += "               CNTR_CNT = '" + dr["cntrGcnt"].ToString() + "',";
            sSql += "               OUT_DUTY_PENALTY = '" + dr["rlseDtyPridPassTpcd"].ToString() + "',";
            sSql += "               DELAY_ADDTAX = '" + dr["dclrDelyAdtxYn"].ToString() + "',";
            sSql += "               SPECIAL_CARGO_CD = '" + dr["spcnCargCd"].ToString() + "',";
            sSql += "               CNTR_NO = '" + dr["cntrNo"].ToString() + "',";
            sSql += "               EXPRESS_CUST = '" + dr["frwrEntsConm"].ToString() + "',";
            sSql += "               UPD_YMD = TO_CHAR(SYSDATE, 'YYYYMMDD'),";
            sSql += "               UPD_HM = TO_CHAR(SYSDATE,'HH24MISS'),";
            //나중에 수정 예정 
            sSql += "               SCRAP_YN = 'Y'";
            sSql += " WHEN NOT MATCHED";
            sSql += " THEN";
            sSql += "    INSERT     (CARGO_MNGT_NO,";
            sSql += "                STATUS,";
            sSql += "                CARR_NM,";
            sSql += "                MBL_NO,";
            sSql += "                HBL_NO,";
            sSql += "                CARGO_TYPE,";
            sSql += "                VSL_NM,";
            sSql += "                CUSTOM_STATUS,";
            sSql += "                PROC_YMD,";
            sSql += "                PROC_HM,";
            sSql += "                CTRY_NM,";
            sSql += "                SHIP_AGENT,";
            sSql += "                ITEM_NM,";
            sSql += "                POL_NM,";
            sSql += "                PKG,";
            sSql += "                GRS_WGT,";
            sSql += "                POD_NM,";
            sSql += "                ENT_CUSTOMS,";
            sSql += "                VOL_WGT,";
            sSql += "                BL_TYPE,";
            sSql += "                LOAD_YMD,";
            sSql += "                VOY,";
            sSql += "                CARE_YN,";
            sSql += "                CNTR_CNT,";
            sSql += "                OUT_DUTY_PENALTY,";
            sSql += "                DELAY_ADDTAX,";
            sSql += "                SPECIAL_CARGO_CD,";
            sSql += "                CNTR_NO,";
            sSql += "                EXPRESS_CUST,";
            sSql += "                SCRAP_YN,";
            sSql += "                INS_YMD,";
            sSql += "                INS_HM)";
            sSql += "                VALUES (";
            sSql += "                '" + dr["cargMtNo"].ToString() + "',";
            sSql += "                '" + dr["prgsStts"].ToString() + "',";
            sSql += "                '" + dr["shcoFlco"].ToString() + "',";
            sSql += "                '" + dr["mblNo"].ToString() + "',";
            sSql += "                '" + dr["hblNo"].ToString() + "',";
            sSql += "                '" + dr["cargTp"].ToString() + "',";
            sSql += "                '" + dr["shipNm"].ToString() + "',";
            sSql += "                '" + dr["csclPrgsStts"].ToString() + "',";
            sSql += "                '" + dr["prcsDttm"].ToString().Substring(0, 8) + "',";
            sSql += "                '" + dr["prcsDttm"].ToString().Substring(8, 6) + "',";
            sSql += "                '" + dr["shipNatNm"].ToString() + "',";
            sSql += "                '" + dr["agnc"].ToString() + "',";
            sSql += "                '" + dr["prnm"].ToString() + "',";
            sSql += "                '" + dr["ldprCd"].ToString() + "',";
            sSql += "                '" + dr["pckGcnt"].ToString() + "',";
            sSql += "                '" + dr["ttwg"].ToString() + "',";
            sSql += "                '" + dr["dsprCd"].ToString() + "',";
            sSql += "                '" + dr["etprCstm"].ToString() + "',";
            sSql += "                '" + dr["msrm"].ToString() + "',";
            sSql += "                '" + dr["blPtNm"].ToString() + "',";
            sSql += "                '" + dr["etprDt"].ToString() + "',";
            sSql += "                '" + dr["vydf"].ToString() + "',";
            sSql += "                '" + dr["mtTrgtCargYnNm"].ToString() + "',";
            sSql += "                '" + dr["cntrGcnt"].ToString() + "',";
            sSql += "                '" + dr["rlseDtyPridPassTpcd"].ToString() + "',";
            sSql += "                '" + dr["dclrDelyAdtxYn"].ToString() + "',";
            sSql += "                '" + dr["spcnCargCd"].ToString() + "',";
            sSql += "                '" + dr["cntrNo"].ToString() + "',";
            sSql += "                '" + dr["frwrEntsConm"].ToString() + "',";
            //나중에 변경예정
            sSql += "                'Y',";
            sSql += "                TO_CHAR(SYSDATE,'YYYYMMDD'),";
            sSql += "                TO_CHAR(SYSDATE,'HH24MISS')";
            sSql += "                )";

            int cnt = _DataHelper.ExecuteNonQuery(sSql, CommandType.Text);
            if (cnt > 0) rtnBool = true;
            return rtnBool;
        }

        public static bool InsDtlUnipass(DataTable dt,string HBL_NO ,string MBL_NO,string CNTR_NO, string SEQ)
        {

            //dt.Rows[it][""]

            int seq = int.Parse(SEQ);



            foreach (DataRow dr in dt.Rows)
            {


                string sSql = "";

                sSql += " MERGE INTO EXP_UNIPASS_DTL";
                sSql += "      USING DUAL";
                sSql += "         ON (HBL_NO = '" + HBL_NO + "' AND MBL_NO = '" + MBL_NO + "'AND CNTR_NO = '" + CNTR_NO + "'AND SEQ = " + seq + ")";
                sSql += " WHEN MATCHED";
                sSql += " THEN";
                sSql += "    UPDATE SET ";
                sSql += "               PROC_YMD = '" + dr["prcsDttm"].ToString().Substring(0, 8) + "',";
                sSql += "               PROC_NM = '" + dr["cargTrcnRelaBsopTpcd"].ToString()+ "',";
                sSql += "               PROC_HM = '" + dr["prcsDttm"].ToString().Substring(8, 6) + "',";
                sSql += "               PROC_LOC = '" + dr["shedNm"].ToString() + "',";
                sSql += "               UPD_YMD = TO_CHAR(SYSDATE, 'YYYYMMDD'),";
                sSql += "               UPD_HM = TO_CHAR(SYSDATE,'HH24MISS')";
                sSql += " WHEN NOT MATCHED";
                sSql += " THEN";
                sSql += "    INSERT     (MBL_NO,";
                sSql += "                HBL_NO,";
                sSql += "                CNTR_NO,";
                sSql += "                SEQ,";
                sSql += "                PROC_NM,";
                sSql += "                PROC_YMD,";
                sSql += "                PROC_HM,";
                sSql += "                PROC_LOC,";
                sSql += "                INS_YMD,";
                sSql += "                INS_HM)";
                sSql += "                VALUES (";
                sSql += "                '" + MBL_NO + "',";
                sSql += "                '" + HBL_NO + "',";
                sSql += "                '" + CNTR_NO + "',";
                sSql += "                " + seq + ",";
                sSql += "                '" + dr["cargTrcnRelaBsopTpcd"].ToString() + "',";
                sSql += "                '" + dr["prcsDttm"].ToString().Substring(0, 8) + "',";
                sSql += "                '" + dr["prcsDttm"].ToString().Substring(8, 6) + "',";
                sSql += "                '" + dr["shedNm"].ToString() + "',";
                sSql += "                TO_CHAR(SYSDATE,'YYYYMMDD'),";
                sSql += "                TO_CHAR(SYSDATE,'HH24MISS')";
                sSql += "                )";
                int cnt = _DataHelper.ExecuteNonQuery(sSql, CommandType.Text);
                if (cnt > 0) rtnBool = true;

                seq--; 
            }

                return rtnBool;
        }
        #endregion


        #endregion



        #region 콘솔 영역 조회

        /// <summary>
        /// 데이터 벌크 인서트
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static bool insertTemp(DataTable dt)
        {
            DataTable rdt = new DataTable();
            try
            {
                using (var bulkCopy = new OracleBulkCopy(ConfigurationManager.ConnectionStrings["ELVIS_ORACLE"].ConnectionString))
                {
                    rdt = dt;

                    
                    
                    if (rdt.Columns.Contains("WayBillNo")) { 
                        rdt.Columns["WayBillNo"].ColumnName = "HBL_NO";
                        bulkCopy.DestinationTableName = "SEARCH_TEMP_HBL";
                    }
                    else
                    {
                        rdt.Columns["LOGISTIC_CD"].ColumnName = "LOGISTIC_CD";
                        bulkCopy.DestinationTableName = "SEARCH_TEMP_LOC";
                    }
                    
                    bulkCopy.WriteToServer(rdt);

                    rtnBool = true;

                }
            }
            catch(Exception e)
            {
                rtnBool = false;

            }
            return rtnBool;
        }

        /// <summary>
        /// 조회
        /// </summary>
        /// <param name="Param_Mngt_No"></param>
        /// <returns></returns>
        public static DataTable SelectUsrHbl(string Param_Mngt_No, string table)
        {
            sSql = "";

            sSql += "SELECT HBL.HBL_NO, ";
            sSql += "       HBL.MBL_NO, ";
            sSql += "       HBL.LOGISTIC_CD, ";
            sSql += "       HBL.COP_NO, ";
            sSql += "       HBL.SHP_NM, ";
            sSql += "       HBL.SHP_ADDR, ";
            sSql += "       HBL.CNE_NM, ";
            sSql += "       HBL.CNE_ADDR, ";
            sSql += "       HBL.RET_NM, ";
            sSql += "       HBL.RET_ADDR, ";
            sSql += "       HBL.TOT_PRICE, ";
            sSql += "       SKU.SKU_PRICE, ";
            sSql += "       SKU.TAX_PRICE, ";
            sSql += "       SKU.POST_PRICE, ";
            sSql += "       SKU.GRS_WGT, ";
            sSql += "       HBL.CURR_NM, ";
            sSql += "       HBL.STATUS, ";
            sSql += "       HBL.UNI_YN, ";
            sSql += "       HBL.INS_YMD, ";
            sSql += "       HBL.INS_HM, ";
            sSql += "       HBL.CLEAER_MODE, ";
            sSql += "       HBL.UNIPASS_CARGO_INFO_ID, ";
            sSql += "       SKU.SEQ AS SEQ, ";
            sSql += "       SKU.ITEM_NM, ";
            sSql += "       SKU.HS_CD, ";
            sSql += "       SKU.ITEM_ID, ";
            sSql += "       '' AS FAIL_YN, ";
            sSql += "       '' AS NEXT_SEND, ";
            sSql += "       HBL.PASS_PORT, ";
            sSql += "       HBL.PARTNER_CD, ";
            sSql += "       HBL.BIG_BAG, ";
            sSql += "       HBL.SHP_MO, ";
            sSql += "       HBL.CNE_MO, ";
            sSql += "       SKU.P_URL, ";
            sSql += "       SKU.QTY, ";
            sSql += "       HBL.CNE_ZIP, ";
            sSql += "       HBL.CNE_MAIL ";
            sSql += "  FROM    EXCEL_HBL_MST HBL ";
            sSql += "       LEFT JOIN ";
            sSql += "          EXCEL_SKU_MST SKU ";
            sSql += "       ON HBL.HBL_NO = SKU.HBL_NO ";
            if(table == "HBL")
            {
                sSql += "       INNER JOIN SEARCH_TEMP_HBL TEMP ";
                sSql += "       ON (TEMP.HBL_NO = HBL.HBL_NO) AND TEMP.MNGT_NO = '" + Param_Mngt_No + "' ";
            }
            if (table == "LOC")
            {
                sSql += "       INNER JOIN SEARCH_TEMP_LOC TEMP ";
                sSql += "       ON (TEMP.LOGISTIC_CD = HBL.LOGISTIC_CD) AND TEMP.MNGT_NO = '" + Param_Mngt_No + "' ";
            }

            sSql += " WHERE 1 = 1 ";

            dt = _DataHelper.ExecuteDataTable(sSql, CommandType.Text);


            return dt;
        }


        public static bool deleteTemp(string Param_Mngt_No, string table)
        {
            try
            {
                sSql = "";
                if(table == "HBL")
                {
                    sSql += "DELETE FROM SEARCH_TEMP_HBL ";
                }
                else
                {
                    sSql += "DELETE FROM SEARCH_TEMP_LOC ";
                }
                
                sSql += "WHERE MNGT_NO = '" + Param_Mngt_No + "' ";


                int cnt = _DataHelper.ExecuteNonQuery(sSql, CommandType.Text);
                if (cnt != -1) rtnBool = true;
                
            }
            catch(Exception e)
            {
                rtnBool = false;
            }
            return rtnBool;

        }


        #endregion

    }
}
