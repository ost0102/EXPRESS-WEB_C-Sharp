using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Text;

namespace EXPRESS_JWTNL.Models.Query
{
    public class Sql_Master
    {
        private static string sSql = "";
        private static bool rtnBool = false;
        private static DataTable dt = new DataTable();



        #region ★★★★★Order화면 조회(튜닝)★★★★★


        #region  Order 기본 조회
        public static DataTable SearchNewMblData(DataRow dr)
        {
            sSql = "";

            sSql += "SELECT MBL_NO , TRANS_TYPE , IE_TYPE , ETD , ETA , POL , POD , VOY ";
            sSql += " FROM EXCEL_MBL_MST ";
            sSql += " WHERE 1=1 ";
            if(dr["SEARCH_TYPE"].ToString() == "MBL_NO")
            {
                sSql += "   AND " + dr["SEARCH_TYPE"].ToString() + " = '" + dr["SEARCH_VALUE"].ToString() + "' ";
            }
            else
            {//하우스나 LOC로 검색시
                sSql += " AND MBL_NO = (SELECT MBL_NO FROM EXCEL_HBL_MST WHERE 1=1 ";
                sSql += "   AND "+ dr["SEARCH_TYPE"].ToString() + " = '"+ dr["SEARCH_VALUE"].ToString() + "') ";
            }
            
            sSql += "AND TO_DATE(ETA, 'YYYYMMDDHH24MISS') BETWEEN TO_DATE('" + dr["STRT_YMD"].ToString() + "', 'YYYYMMDD') AND TO_DATE('" + dr["END_YMD"].ToString() + "', 'YYYYMMDD') +1 ";

            dt = _DataHelper.ExecuteDataTable(sSql, CommandType.Text);

            return dt;
        }

        public static DataTable SearchNewHblData(DataRow dr)
        {
            sSql = "";

            sSql += "SELECT API.STATE ";
            sSql += "   , API.KR ";
            sSql += "   , NVL(API.CL_MD,HBL.CLEAER_MODE)AS CLEAER_MODE ";
            sSql += "   , HBL.HBL_NO ";
            sSql += "   , HBL.MBL_NO ";
            sSql += "   , HBL.LOGISTIC_CD ";
            sSql += "   , HBL.COP_NO ";
            sSql += "   , HBL.UNIPASS_CARGO_INFO_ID ";
            sSql += "   , HBL.STATUS ";
            sSql += "   , HBL.UNI_YN ";
            sSql += "   , HBL.PARTNER_CD ";
            sSql += "   , HBL.BIG_BAG ";

            sSql += "   , MBL.TRANS_TYPE ";
            sSql += "   , MBL.IE_TYPE ";
            sSql += "   , MBL.ETD ";
            sSql += "   , MBL.ETA ";
            sSql += "   , MBL.POL ";
            sSql += "   , MBL.POD ";
            sSql += "   , MBL.VOY ";

            sSql += "   , HBL.CURR_NM ";
            sSql += "   , HBL.TOT_PRICE ";
            sSql += "   , SKU.SKU_PRICE ";
            sSql += "   , SKU.GRS_WGT ";
            sSql += "   , SKU.TAX_PRICE ";
            sSql += "   , SKU.POST_PRICE ";
            sSql += "   , SKU.SEQ AS SEQ ";
            sSql += "   , SKU.ITEM_NM ";
            sSql += "   , SKU.ITEM_ID ";
            sSql += "   , SKU.HS_CD ";
            sSql += "   , SKU.P_URL ";
            sSql += "   , SKU.QTY ";
            

            sSql += "   , HBL.SHP_NM ";
            sSql += "   , HBL.SHP_ADDR ";
            sSql += "   , HBL.SHP_MO ";

            sSql += "   , HBL.CNE_NM ";
            sSql += "   , HBL.CNE_ADDR ";
            sSql += "   , HBL.CNE_MO ";
            sSql += "   , HBL.CNE_ZIP ";
            sSql += "   , HBL.CNE_MAIL ";
            sSql += "   , HBL.PASS_PORT ";

            
            sSql += " FROM EXCEL_HBL_MST HBL ";
            #region 상품정보 JOIN
            sSql += "   LEFT JOIN EXCEL_SKU_MST SKU ";
            sSql += "       ON HBL.HBL_NO = SKU.HBL_NO ";
            #endregion
            sSql += "   LEFT JOIN (SELECT HBL_NO , MBL_NO , MAX(CLEAR_MODE)AS CL_MD , MAX(RTN_STATUS) KEEP(DENSE_RANK LAST ORDER BY SEQ) AS STATE , MAX(RTN_KR) KEEP(DENSE_RANK LAST ORDER BY SEQ) KR ";
            sSql += "               FROM CAINIAO_SEND_DATA";
            sSql += "               WHERE 1=1 ";
            sSql += "               GROUP BY HBL_NO, MBL_NO ) API ";
            sSql += "       ON HBL.HBL_NO = API.HBL_NO ";

            sSql += "   LEFT JOIN EXCEL_MBL_MST MBL ";
            sSql += "       ON HBL.MBL_NO  = MBL.MBL_NO ";

            sSql += " WHERE 1=1 ";
            //조건별 조회조건 변환
            sSql += "AND HBL." + dr["SEARCH_TYPE"].ToString() + "= '" + dr["SEARCH_VALUE"].ToString() + "' ";

            if (dr["SEARCH_TYPE"].ToString() == "MBL_NO")
            {
                #region 멀티건 처리
                string[] bl_list = dr["MULTI_NO"].ToString().Split(',');
                bl_list = bl_list.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray(); //빈배열 제거
                int blseq = 1;

                if (bl_list.Length > 0)
                {
                    sSql += "AND ( ";
                    foreach (var blone in bl_list)
                    {

                        if (!String.IsNullOrEmpty(blone.ToString()))
                        {
                            if (blseq == 1)
                            {
                                sSql += "   HBL.HBL_NO = '" + blone + "' ";
                                if (bl_list.Length == 1)
                                {
                                    sSql += ")";
                                }
                            }
                            else
                            {
                                sSql += "  OR  HBL.HBL_NO = '" + blone + "' ";
                                if (bl_list.Length == blseq)
                                {
                                    sSql += ")";
                                }
                            }
                        }
                        blseq += 1;
                    }
                }
                #endregion
            }

            //기간 조회조건
            sSql += "AND TO_DATE(MBL.ETA, 'YYYYMMDDHH24MISS') BETWEEN TO_DATE('" + dr["STRT_YMD"].ToString() + "', 'YYYYMMDD') AND TO_DATE('" + dr["END_YMD"].ToString() + "', 'YYYYMMDD') +1";

            sSql += " ORDER BY HBL.HBL_NO ";


            dt = _DataHelper.ExecuteDataTable(sSql, CommandType.Text);

            return dt;
        }
        #endregion


        #region 유니패스 팝업 조회
        //최상단 데이터
        //TABLE NAME : MST
        public static DataTable SearchNewUniMst(DataRow dr)
        {

            sSql = "";
            sSql += "SELECT UM.HBLNO , UM.CSCLPRGSSTTS , UM.PRCSDTTM , UM.CARGMTNO , H.COP_NO , H.LOGISTIC_CD ";
            sSql += " FROM UNIPASS_CARGO_INFO_MST UM";
            sSql += "   LEFT JOIN EXCEL_HBL_MST H ";
            sSql += "       ON UM.MNGT_ID = H.UNIPASS_CARGO_INFO_ID ";
            sSql += " WHERE 1=1 ";
            sSql += "  AND MNGT_ID = '" + dr["SEARCH_VALUE2"].ToString() + "' ";


            dt = _DataHelper.ExecuteDataTable(sSql, CommandType.Text);

            return dt;
        }


        //중단 마스터 관련 데이터
        //TABLE NAME : DTL
        public static DataTable SearchNewUniMbl(DataRow dr)
        {
            sSql = "";
            sSql += "SELECT H.MBL_NO AS ORG_MBL , H.CNE_NM , H.SHP_NM , MM.VOY , MM.ETD , MM.ETA , MM.POL , MM.POD , REPLACE(H.MBL_NO,'-','') AS MBL_NO , H.PARTNER_CD ";
            sSql += "   FROM EXCEL_HBL_MST H ";
            sSql += "   LEFT JOIN EXCEL_MBL_MST MM ";
            sSql += "       ON H.MBL_NO = MM.MBL_NO ";
            sSql += "   WHERE H.UNIPASS_CARGO_INFO_ID = '"+dr["SEARCH_VALUE2"].ToString()+"' ";

            dt = _DataHelper.ExecuteDataTable(sSql, CommandType.Text);

            return dt;
        }

        //디테일 데이터
        //TABLE NAME : UDTL
        public static DataTable SearchNewUniDtl(DataRow dr)
        {
            sSql = "";
            sSql += "SELECT UD.SEQ ";
            sSql += "       , UD.MNGT_ID ";
            sSql += "       , UD.SHEDNM ";
            sSql += "       , UD.PRCSDTTM ";
            sSql += "       , UD.CARGTRCNRELABSOPTPCD ";
            //sSql += "       , UD.SEND_YN ";
            sSql += "       , CASE WHEN CA.SEND_YN = 'Y' THEN '전송 완료' ";
            sSql += "         WHEN CA.SEND_YN = 'N' THEN '미전송' END AS SEND_YN ";
            sSql += "       , CA2.STATUS AS L_STATUS ";
            sSql += "       , EXPT1.EXPT AS EXPT1 ";
            sSql += "       , EXPT2.EXPT AS EXPT2";
            sSql += "   FROM UNIPASS_CARGO_INFO_DTL UD ";
            sSql += "   LEFT JOIN (SELECT GUID , RTN_KR , MAX(SEND_YN)AS SEND_YN , MAX(RTN_STATUS)KEEP(DENSE_RANK LAST ORDER BY SEQ) MY_STA FROM CAINIAO_SEND_DATA WHERE GUID = '"+dr["SEARCH_VALUE2"].ToString()+"' GROUP BY GUID , RTN_KR) CA";
            sSql += "       ON UD.MNGT_ID = CA.GUID ";
            sSql += "       AND UD.CARGTRCNRELABSOPTPCD = CA.RTN_KR ";
            
            sSql += "   LEFT JOIN (SELECT MAX(RTN_STATUS)KEEP(DENSE_RANK LAST ORDER BY SEQ) STATUS , GUID FROM CAINIAO_SEND_DATA WHERE GUID = '"+dr["SEARCH_VALUE2"].ToString()+"' GROUP BY GUID) CA2 ";
            sSql += "   ON UD.MNGT_ID = CA.GUID ";

            sSql += "   LEFT JOIN (SELECT MAX(RTN_STATUS)KEEP(DENSE_RANK LAST ORDER BY SEQ) EXPT , GUID FROM CAINIAO_SEND_DATA WHERE GUID = '" + dr["SEARCH_VALUE2"].ToString() + "' AND RTN_STATUS LIKE '30%' AND RTN_STATUS != '3025' AND RTN_STATUS != '3006' GROUP BY GUID) EXPT1 ";
            sSql += "   ON UD.MNGT_ID = CA.GUID ";

            sSql += "   LEFT JOIN (SELECT MAX(RTN_STATUS)KEEP(DENSE_RANK LAST ORDER BY SEQ) EXPT , GUID FROM CAINIAO_SEND_DATA WHERE GUID = '" + dr["SEARCH_VALUE2"].ToString() + "' AND RTN_STATUS LIKE '40%' AND RTN_STATUS != '4010' GROUP BY GUID) EXPT2 ";
            sSql += "   ON UD.MNGT_ID = CA.GUID ";

            sSql += "   WHERE 1=1  ";
            sSql += "   AND MNGT_ID = '"+dr["SEARCH_VALUE2"].ToString()+"' ";

            sSql += " ORDER BY UD.SEQ ASC ";

            dt = _DataHelper.ExecuteDataTable(sSql, CommandType.Text);

            return dt;
        }


        #endregion



        #region 예외 저장
        public static bool SaveSendExpt(DataTable dt)
        {
            sSql = "";
            StringBuilder sqlsb = new StringBuilder();
            DateTime ntime = DateTime.Now;
            string nullguid = ntime.ToString("yyyyMMddHHmmssfff");
            try
            {
                for(int i = 0; i < dt.Rows.Count; i++) {
                    sqlsb.AppendLine();
                    sqlsb.AppendLine("MERGE INTO  CAINIAO_SEND_DATA CA");
                    if (dt.Rows[i]["guid"].ToString() != "") {
                        sqlsb.AppendLine(" USING DUAL ON (CA.GUID = '" + dt.Rows[i]["guid"].ToString() + "' ");
                    }
                    else
                    {
                        
                        sqlsb.AppendLine(" USING DUAL ON (CA.GUID = '" + nullguid +"1" + i.ToString().PadLeft(5,'0') + "' ");
                    }
                    sqlsb.AppendLine("                 AND CA.HBL_NO = '" + dt.Rows[i]["wayBillNo"].ToString() + "' ");
                    sqlsb.AppendLine("                  AND CA.RTN_STATUS = '" + dt.Rows[i]["returnStatus"].ToString() + "' )");
                    sqlsb.AppendLine("  WHEN NOT MATCHED THEN ");
                    sqlsb.AppendLine(" INSERT (CA.GUID , CA.COP_NO, CA.HBL_NO , CA.RTN_KR , CA.RTN_STATUS , CA.RTN_TIME , CA.CLEAR_MODE , CA.SEQ , CA.SEND_YN , CA.API_YMD , CA.API_HM , CA.UP_SEQ, CA.MBL_NO ) ");
                    if (dt.Rows[i]["guid"].ToString() != "")
                    {
                        sqlsb.AppendLine(" VALUES ( '" + dt.Rows[i]["guid"].ToString() + "' ");
                    }
                    else
                    {
                        sqlsb.AppendLine(" VALUES ( '" + nullguid + "1" + i.ToString().PadLeft(5, '0') + "' ");
                    }
                    sqlsb.AppendLine("         , '" + dt.Rows[i]["copNo"].ToString() + "' ");
                    sqlsb.AppendLine("          , '" + dt.Rows[i]["wayBillNo"].ToString() + "' ");
                    sqlsb.AppendLine("          , (SELECT SATAUS_KR FROM MAP_CAINIAO_CODE WHERE SATAUS_CD = '" + dt.Rows[i]["returnStatus"].ToString() + "') ");
                    sqlsb.AppendLine("          , '" + dt.Rows[i]["returnStatus"].ToString() + "' ");
                    sqlsb.AppendLine("          , '" + dt.Rows[i]["RTN_TIME"].ToString() + "' ");
                    if (dt.Columns.Contains("CLEAR_MODE"))
                    {
                        sqlsb.AppendLine("          , '"+dt.Rows[i]["CLEAR_MODE"].ToString() +"' ");
                    }
                    else
                    {
                        sqlsb.AppendLine("          , '' ");
                    }
                    sqlsb.AppendLine("          , (SELECT SEQ FROM MAP_CAINIAO_CODE WHERE SATAUS_CD = '" + dt.Rows[i]["returnStatus"].ToString() + "') ");
                    sqlsb.AppendLine("          , 'Y' ");
                    sqlsb.AppendLine("          , TO_CHAR(SYSDATE,'YYYYMMDD') ");
                    sqlsb.AppendLine("          , TO_CHAR(SYSDATE,'HH24MISS') ");
                    sqlsb.AppendLine("          , '' ");
                    sqlsb.AppendLine("          , '" + dt.Rows[i]["MBL_NO"].ToString() + "' ) ");

                    sqlsb.AppendLine("  WHEN  MATCHED THEN ");
                    sqlsb.AppendLine("   UPDATE SET ");
                    sqlsb.AppendLine("      CA.COP_NO ='" + dt.Rows[i]["copNo"].ToString() + "' , ");
                    sqlsb.AppendLine("      CA.RTN_KR =(SELECT SATAUS_KR FROM MAP_CAINIAO_CODE WHERE SATAUS_CD = '" + dt.Rows[i]["returnStatus"].ToString() + "')  , ");
                    sqlsb.AppendLine("      CA.RTN_TIME ='" + dt.Rows[i]["RTN_TIME"].ToString() + "' , ");
                    if (dt.Columns.Contains("CLEAR_MODE"))
                    {
                        sqlsb.AppendLine("      CA.CLEAR_MODE ='"+dt.Rows[i]["CLEAR_MODE"].ToString() +"' , ");
                    }
                    else
                    {
                        sqlsb.AppendLine("      CA.CLEAR_MODE ='' , ");
                    }
                    sqlsb.AppendLine("      CA.SEQ =(SELECT SEQ FROM MAP_CAINIAO_CODE WHERE SATAUS_CD = '" + dt.Rows[i]["returnStatus"].ToString() + "') , ");
                    sqlsb.AppendLine("      CA.SEND_YN ='Y' , ");
                    sqlsb.AppendLine("      CA.API_YMD = TO_CHAR(SYSDATE,'YYYYMMDD') , ");
                    sqlsb.AppendLine("      CA.API_HM = TO_CHAR(SYSDATE,'HH24MISS') , ");
                    sqlsb.AppendLine("      CA.UP_SEQ = '' , ");
                    sqlsb.AppendLine("      CA.MBL_NO = '" + dt.Rows[i]["MBL_NO"].ToString() + "'; ");

                    if(dt.Rows[i]["returnStatus"].ToString().Substring(0,2) == "40") //실패건 
                    {
                        sqlsb.AppendLine("UPDATE UNIPASS_CARGO_INFO_MST SET MIG_YN = 'Y' , SEARCH_YN = 'N' ");
                        sqlsb.AppendLine(" WHERE MNGT_ID = '" + dt.Rows[i]["guid"].ToString() + "'  AND HBL_NO = '" + dt.Rows[i]["wayBillNo"].ToString() + "'; ");
                    }

                    if(dt.Rows[i]["guid"].ToString() == "")
                    {
                        sqlsb.AppendLine(" UPDATE EXCEL_HBL_MST SET UNIPASS_CARGO_INFO_ID = '" + nullguid + "1" + i.ToString().PadLeft(5, '0') + "'  WHERE HBL_NO = '"+ dt.Rows[i]["wayBillNo"].ToString() + "'; ");
                        
                    }

                }
                

                sSql = "BEGIN" + sqlsb.ToString() + " END;";

                int cnt = _DataHelper.ExecuteNonQuery(sSql, CommandType.Text);
                if (cnt == -1) rtnBool = true;
                return rtnBool;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
           
        }

        #endregion

        #endregion


        #region★★★★★ Exception 화면(튜닝) ★★★★★
        public static DataTable SearchNewExcept(DataRow dr)
        {
            sSql = "";

            sSql += "SELECT API.STATE, ";
            sSql += "       API.KR, ";
            sSql += "       NVL (API.CL_MD, HBL.CLEAER_MODE) AS CLEAER_MODE, ";
            sSql += "       HBL.HBL_NO, ";
            sSql += "       HBL.MBL_NO, ";
            sSql += "       HBL.LOGISTIC_CD, ";
            sSql += "       HBL.COP_NO, ";
            sSql += "       HBL.UNIPASS_CARGO_INFO_ID, ";
            //sSql += "       HBL.STATUS, ";
            //sSql += "       HBL.UNI_YN, ";
            sSql += "       HBL.PARTNER_CD, ";
            sSql += "       HBL.BIG_BAG, ";
            //sSql += "       MBL.TRANS_TYPE, ";
            //sSql += "       MBL.IE_TYPE, ";
            sSql += "       MBL.ETD, ";
            sSql += "       MBL.ETA, ";
            sSql += "       MBL.POL, ";
            sSql += "       MBL.POD, ";
            sSql += "       MBL.VOY, ";
            sSql += "       HBL.CURR_NM, ";
            sSql += "       HBL.TOT_PRICE, ";
            sSql += "       HBL.SHP_NM, ";
            sSql += "       HBL.SHP_ADDR, ";
            sSql += "       HBL.SHP_MO, ";
            sSql += "       HBL.CNE_NM, ";
            sSql += "       HBL.CNE_ADDR, ";
            sSql += "       HBL.CNE_MO, ";
            sSql += "       HBL.CNE_ZIP, ";
            sSql += "       HBL.CNE_MAIL, ";
            sSql += "       HBL.PASS_PORT ";
            sSql += "  FROM EXCEL_HBL_MST HBL ";
            sSql += "       LEFT JOIN (  SELECT HBL_NO, ";
            sSql += "                           MBL_NO, ";
            sSql += "                           MAX (CLEAR_MODE) AS CL_MD, ";
            sSql += "                           MAX (RTN_STATUS) KEEP (DENSE_RANK LAST ORDER BY SEQ) ";
            sSql += "                              AS STATE, ";
            sSql += "                           MAX (RTN_KR) KEEP (DENSE_RANK LAST ORDER BY SEQ) KR ";
            sSql += "                      FROM CAINIAO_SEND_DATA ";
            sSql += "                     WHERE 1 = 1 ";
            sSql += "                     AND SEND_YN = 'Y' ";
            sSql += "                  GROUP BY HBL_NO, MBL_NO ";
            sSql += "                  ) API ";
            sSql += "          ON HBL.HBL_NO = API.HBL_NO ";
            sSql += "          AND HBL.MBL_NO = API.MBL_NO ";
            sSql += "       INNER JOIN EXCEL_MBL_MST MBL ";
            sSql += "          ON HBL.MBL_NO = MBL.MBL_NO ";
            sSql += "          AND MBL.LINE_CD IS NULL ";
            sSql += "          AND TO_DATE (MBL.ETA, 'YYYYMMDDHH24MISS') BETWEEN TO_DATE ('"+ dr["STRT_YMD"].ToString() + "', 'YYYYMMDD') AND TO_DATE ('"+dr["END_YMD"].ToString()+"','YYYYMMDD') +1";
            
            //인천공항
            if(dr["PARTNER_CD"].ToString() == "GATE_31182453")
            {
                sSql += "          AND TRANS_TYPE = '5' ";
            }
            else // 해운
            {
                sSql += "          AND TRANS_TYPE = '2' ";
            }
            
            sSql += " WHERE 1 = 1 ";
            if(dr["SEARCH_VALUE"].ToString() != "")
            {
                sSql += "AND HBL." + dr["SEARCH_TYPE"].ToString() + "= '" + dr["SEARCH_VALUE"].ToString() + "' ";
            }
            if(dr["MULTI"].ToString() != "")
            {
                sSql += " AND (";
                string[] bl_list = dr["MULTI"].ToString().Split('\n');
                if (bl_list.Length > 0)
                {
                    for (int j = 0; j < bl_list.Length; j++)
                    {
                        if (j != 0) { sSql += " OR "; }
                        sSql += " HBL.HBL_NO = '"+bl_list[j].ToString()+"' ";
                        if (j == bl_list.Length - 1) { sSql += ") "; }
                    }
                }
            }

            //게이트 번호
            sSql += " AND HBL.PARTNER_CD = '"+dr["PARTNER_CD"].ToString()+"' ";
            //보류 멈춰있는 건들 
            if(dr["EXCEPT_TYPE"].ToString() == "CC")
            {
                sSql += " AND ( STATE = '3006') ";
            }
            if (dr["EXCEPT_TYPE"].ToString() == "TRS")
            {
                sSql += " AND ( STATE = '3041') ";
            }
            if (dr["EXCEPT_TYPE"].ToString() == "EXCEP")
            {
                sSql += " AND (STATE != '3025'  AND STATE != '3006' AND STATE != '3007' AND STATE LIKE '30%') ";
                //sSql += " AND (STATE != '3025'  AND STATE != '3006' AND STATE LIKE '30%') ";
            }
            if (dr["EXCEPT_TYPE"].ToString() == "TAX")
            {
                sSql += " AND (STATE = '3007') ";
            }
            if(dr["EXCEPT_TYPE"].ToString() == "NONE")
            {
                sSql += " AND (STATE = '1000') ";
            }

            if (dr["EXCEPT_TYPE"].ToString() == "NONE2")
            {
                sSql += " AND (STATE = '3025') ";
            }


            if (dr["EXCEPT_TYPE"].ToString() == "NOCS")
            {
                sSql += " ";
            }

            if (dr["EXCEPT_TYPE"].ToString() == "CARRIN")
            {
                sSql += " AND (STATE = '2030') ";
            }

            if (dr["EXCEPT_TYPE"].ToString() == "SIGNIN")
            {
                sSql += " AND (STATE = '2060') ";
            }

            if (dr["EXCEPT_TYPE"].ToString() == "CARROUT")
            {
                sSql += " AND (STATE = '2070') ";
            }

            dt = _DataHelper.ExecuteDataTable(sSql, CommandType.Text);

            return dt;
        }
        #endregion


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

            if (dr["SEARCH_TYPE"].ToString() == "MBL_NO")
            {
                sSql += "AND MBL_NO ='" + dr["SEARCH_VALUE"].ToString() + "' ";
            }

            sSql += "               GROUP BY HBL_NO  ";
            sSql += " ) EXCEP";
            sSql += "       ON HBL.HBL_NO = EXCEP.HBL_NO ";


                
            sSql += " LEFT JOIN EXCEL_MBL_MST MBL";
            sSql += " ON  MBL.MBL_NO = HBL.MBL_NO";
            sSql += " WHERE 1=1";
            if (dr["SEARCH_TYPE"].ToString() == "MBL_NO")
            {
                sSql += "AND HBL.MBL_NO = '" + dr["SEARCH_VALUE"].ToString() + "'";
                sSql += "AND TO_DATE(MBL.ETA, 'YYYYMMDDHH24MISS') BETWEEN TO_DATE('" + dr["STRT_YMD"].ToString() + "', 'YYYYMMDD') AND TO_DATE('" + dr["END_YMD"].ToString() + "', 'YYYYMMDD') + 1";
                //sSql += "    AND HBL.INS_YMD >= '" + dr["STRT_YMD"].ToString() + "'";
                //sSql += "    AND HBL.INS_YMD <= '" + dr["END_YMD"].ToString() + "'";

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
                if (dr["SEARCH_TYPE"].ToString() == "HBL_NO")
                {
                    sSql += "AND HBL.HBL_NO = (";
                    sSql += "  SELECT HBL_NO";
                    sSql += "  FROM EXCEL_HBL_MST";
                    sSql += "  WHERE 1=1";
                    sSql += "AND HBL_NO = '" + dr["SEARCH_VALUE"].ToString() + "'";
                }
                else if (dr["SEARCH_TYPE"].ToString() == "LOGISTIC_CD")
                {
                    sSql += "AND HBL.LOGISTIC_CD = (";
                    sSql += "  SELECT LOGISTIC_CD";
                    sSql += "  FROM EXCEL_HBL_MST";
                    sSql += "  WHERE 1=1";
                    sSql += "AND LOGISTIC_CD = '" + dr["SEARCH_VALUE"].ToString() + "'";
                }

                sSql += " ) ";
                sSql += "AND TO_DATE(MBL.ETA, 'YYYYMMDDHH24MISS') BETWEEN TO_DATE('" + dr["STRT_YMD"].ToString() + "', 'YYYYMMDD') AND TO_DATE('" + dr["END_YMD"].ToString() + "', 'YYYYMMDD') +1";
                //sSql += "    AND HBL.INS_YMD >= '" + dr["STRT_YMD"].ToString() + "'";
                //sSql += "    AND HBL.INS_YMD <= '" + dr["END_YMD"].ToString() + "'";
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
            sSql += "      , A.CARGMTNO";
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

        /// <summary>
        /// 상태값 조회
        /// </summary>
        /// <param name="dr"></param>
        /// <returns></returns>
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

        #region Exception 사유
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

        #endregion

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

        #region Exception Page

        //조회
        public static DataTable SearchExDtlData(DataRow dr)
        {
            string sSql = "";

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
            //sSql += "      , SKU.SKU_PRICE";
            //sSql += "      , SKU.TAX_PRICE";
            //sSql += "      , SKU.POST_PRICE";
            //sSql += "      , SKU.GRS_WGT";
            sSql += "      , HBL.CURR_NM";
            sSql += "      , HBL.STATUS";
            sSql += "      , HBL.UNI_YN";
            sSql += "      , HBL.INS_YMD";
            sSql += "      , HBL.INS_HM";
            sSql += "      , HBL.CLEAER_MODE";
            sSql += "      , HBL.UNIPASS_CARGO_INFO_ID";
            //sSql += "      , HBL.SEQ";
            //sSql += "      , SKU.SEQ AS SEQ";
            //sSql += "      , SKU.ITEM_NM";
            //sSql += "      , SKU.HS_CD";
            //sSql += "      , SKU.ITEM_ID";
            sSql += "      , NVL2(API.RTN_STATUS,'Y','N') AS  FAIL_YN ";  //보류 체크
            sSql += "      , NVL2(API2.RTN_STATUS,'Y','N') AS  RESON_YN "; // 
            sSql += "      , NVL2(API3.RTN_STATUS,'Y','N') AS  END_YN ";
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
            sSql += " FROM EXCEL_HBL_MST HBL";
            //sSql += " LEFT JOIN EXCEL_SKU_MST SKU";
            //sSql += " ON  HBL.HBL_NO = SKU.HBL_NO";
            /*2040 여부*/
            sSql += " LEFT JOIN  CAINIAO_SEND_DATA API";
            sSql += " ON HBL.HBL_NO = API.HBL_NO";
            sSql += " AND (API.RTN_STATUS = '2040')";
            /*보류목록 사항 체크*/
            sSql += " LEFT JOIN  CAINIAO_SEND_DATA API2";
            sSql += " ON HBL.HBL_NO = API2.HBL_NO";
            sSql += "   AND (API2.RTN_STATUS IN (SELECT SATAUS_CD FROM MAP_CAINIAO_CODE  WHERE STD_CD = 'EXPT1') OR API2.RTN_STATUS = '3007')";
            //sSql += " AND (API2.RTN_STATUS LIKE '30%') ";
            //sSql += " AND (API2.RTN_STATUS != '3025' AND AP12.RTN_STATUS != '3006') ";
            /*오류 완료여부*/
            sSql += " LEFT JOIN  CAINIAO_SEND_DATA API3";
            sSql += " ON HBL.HBL_NO = API3.HBL_NO";
            //sSql += " AND (API3.RTN_STATUS LIKE '40%'OR API3.RTN_STATUS = '2070' )";
            sSql += " AND (API3.RTN_STATUS LIKE '40%' OR API3.RTN_STATUS = '2070'  OR API3.RTN_STATUS = '2050')"; // 2050 인 건도 제외


            /*기존 NEXT_SEND 조건 변경 및 JOIN 사용 */
            sSql += " LEFT JOIN ( SELECT HBL_NO , MAX(SEQ)AS API_SEQ , MAX(RTN_STATUS) KEEP (DENSE_RANK FIRST ORDER BY SEQ DESC) STAT ";
            sSql += "               FROM CAINIAO_SEND_DATA ";
            sSql += "               WHERE 1=1 AND SEND_YN = 'Y' ";
            sSql += "               GROUP BY HBL_NO ) EXCEP ";
            sSql += "       ON HBL.HBL_NO = EXCEP.HBL_NO ";

            sSql += " LEFT JOIN EXCEL_MBL_MST MBL";
            sSql += " ON  MBL.MBL_NO = HBL.MBL_NO";
            sSql += " WHERE 1=1";

            //조회 조건값이 있을 때 
            if(dr["SEARCH_VALUE"].ToString() != "")
            {
                if (dr["SEARCH_TYPE"].ToString() == "Master B/L")
                {
                    sSql += "AND HBL.MBL_NO = '" + dr["SEARCH_VALUE"].ToString() + "'";
                    //sSql += "    AND HBL.INS_YMD >= '" + dr["STRT_YMD"].ToString() + "'";
                    //sSql += "    AND HBL.INS_YMD <= '" + dr["END_YMD"].ToString() + "'";
                    sSql += "   AND SUBSTR(MBL.ETA,0,8) BETWEEN '" + dr["STRT_YMD"].ToString() + "' AND '" + dr["END_YMD"].ToString() + "' ";
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
                    if (dr["SEARCH_TYPE"].ToString() == "House B/L")
                    {
                        sSql += "AND HBL.HBL_NO = (";
                        sSql += "  SELECT HBL_NO";
                        sSql += "  FROM EXCEL_HBL_MST";
                        sSql += "  WHERE 1=1";
                        sSql += "AND HBL_NO = '" + dr["SEARCH_VALUE"].ToString() + "'";
                    }
                    else if (dr["SEARCH_TYPE"].ToString() == "Location ID")
                    {
                        sSql += "AND HBL.LOGISTIC_CD = (";
                        sSql += "  SELECT LOGISTIC_CD";
                        sSql += "  FROM EXCEL_HBL_MST";
                        sSql += "  WHERE 1=1";
                        sSql += "AND LOGISTIC_CD = '" + dr["SEARCH_VALUE"].ToString() + "'";
                    }

                    sSql += " ) ";

                    //sSql += "    AND HBL.INS_YMD >= '" + dr["STRT_YMD"].ToString() + "'";
                    //sSql += "    AND HBL.INS_YMD <= '" + dr["END_YMD"].ToString() + "'";
                    //기간 조회로 변경
                    sSql += "   AND SUBSTR(MBL.ETA,0,8) BETWEEN '" + dr["STRT_YMD"].ToString() + "' AND '" + dr["END_YMD"].ToString() + "' ";

                }
            }
            
            if(dr["EXCEPT_TYPE"].ToString() != "")
            {
                //sSql += "    AND EXCEP.STAT LIKE '" + dr["EXCEPT_TYPE"].ToString() + "%' ";
                switch (dr["EXCEPT_TYPE"].ToString())
                {
                    case "2040":
                        sSql += " AND (API.RTN_STATUS IS NOT NULL AND API2.RTN_STATUS IS NULL AND API3.RTN_STATUS IS NULL)";
                        break;
                    case "40":
                        sSql += " AND (API3.RTN_STATUS IS NOT NULL)";
                        sSql += "    AND EXCEP.STAT LIKE '" + dr["EXCEPT_TYPE"].ToString() + "%' ";
                        break;
                    case "30":
                        sSql += " AND (API2.RTN_STATUS IS NOT NULL )";
                        sSql += "    AND EXCEP.STAT LIKE '" + dr["EXCEPT_TYPE"].ToString() + "%' ";
                        break;
                }
            }

 //미완성 테스트 필요            
            if(dr["REQ_SVC"].ToString() != "") //해운항공 조회조건 있을 때
            {
                if(dr["REQ_SVC"].ToString() == "SEA")
                {//GATE_31182453 : 항공 게이트번호
                    sSql += " AND HBL.PARTNER_CD != 'GATE_31182453' ";
                }
                else{
                    sSql += " AND HBL.PARTNER_CD = 'GATE_31182453' "; 
                }
            }

            //sSql += "    AND HBL.INS_YMD >= '" + dr["STRT_YMD"].ToString() + "'";
            //sSql += "    AND HBL.INS_YMD <= '" + dr["END_YMD"].ToString() + "'";
            dt = _DataHelper.ExecuteDataTable(sSql, CommandType.Text);

            return dt;
        }

        /// <summary>
        /// 단일 종류 업데이트용 팝업 바인딩
        /// </summary>
        /// <param name="dr"></param>
        /// <returns></returns>
        public static DataTable SearchExcepList(DataRow dr)
        {

            string sType = dr["SEARCH_TYPE"].ToString();
            sSql = "";

            if(sType == "EXCEP" || sType == "TAX")
            {
                sSql += "SELECT '0000' AS SATAUS_CD , '--'||'FINAL STATE'||'--' AS NAME  , '"+sType+"' AS TYPE ";
                sSql += "FROM MAP_CAINIAO_CODE ";
                sSql += " UNION ";
            }

            sSql += "SELECT  REAL_CD AS SATAUS_CD, '(' || REAL_CD || ') ' || STATUS_US AS NAME  , '" + sType + "' AS TYPE  ";
            sSql += " FROM MAP_CAINIAO_CODE ";
            sSql += "    WHERE 1=1 ";

            

            switch (sType)
            {
                case "CC": // 2040 & 3006
                    sSql += " AND STD_CD = 'EXPT1' ";
                    break;
                case "EXCEP": // 30XX
                    sSql += " AND STD_CD = 'EXPT2' ";
                    break;
                case "TRS":
                    sSql += " AND STD_CD = 'EXPT1'  AND REAL_CD != '3041' ";
                    break;
                case "TAX": //3007
                    sSql += " AND REAL_CD = '4006' ";
                    break;
                case "INFO": //2020 & 2030
                    sSql += " AND REAL_CD = '3001' ";
                    break;
            }


            sSql += " ORDER BY SATAUS_CD ";

            dt = _DataHelper.ExecuteDataTable(sSql, CommandType.Text);

            return dt;
        }

        /// <summary>
        /// 플래그값 일괄 업데이트 쿼리
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static bool AddExceptFlag(DataRow dr , DataTable dt)
        {

            sSql = "";
            sSql += " BEGIN "; 

            sSql += "MERGE INTO CAINIAO_SEND_DATA A";
            sSql += "   USING DUAL ";
            sSql += "   ON (A.HBL_NO = '"+dr["WAYBILL_NO"].ToString()+"' ";
            sSql += "       AND A.RTN_STATUS = '"+ dt.Rows[0]["CODE1"].ToString()+"') ";
            sSql += "   WHEN NOT MATCHED THEN ";

            sSql += " INSERT (A.GUID, A.COP_NO , A.HBL_NO , A.RTN_KR , A.RTN_STATUS , A.RTN_TIME , A.SEQ ) ";
            sSql += "   VALUES(";
            sSql += "           '" +dr["CARGO_ID"].ToString()+ "'";
            sSql += "           , '"+dr["COP_NO"].ToString()+"' ";
            sSql += "           , '" + dr["WAYBILL_NO"].ToString() + "' ";
            sSql += "           , '" + dt.Rows[0]["NAME1"].ToString() + "' ";
            sSql += "           , '" + dt.Rows[0]["CODE1"].ToString() + "' ";
            //sSql += "           , TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS') ";
            //2040 보다 1초 늦게 셋팅 요청
            sSql += "           , (SELECT (RTN_TIME+1)AS RTN_NEW FROM CAINIAO_SEND_DATA WHERE HBL_NO= '"+ dr["WAYBILL_NO"].ToString() + "' AND RTN_STATUS = '2040')";
            sSql += "           , '7' ";
            sSql += "); ";

            if(dt.Rows[0]["CODE1"].ToString() == "3001")
            {
                sSql += "MERGE INTO CAINIAO_SEND_DATA A";
                sSql += "   USING DUAL ";
                sSql += "   ON (A.HBL_NO = '" + dr["WAYBILL_NO"].ToString() + "' ";
                sSql += "       AND A.RTN_STATUS = '3002') ";
                sSql += "   WHEN NOT MATCHED THEN ";

                sSql += " INSERT (A.GUID, A.COP_NO , A.HBL_NO , A.RTN_KR , A.RTN_STATUS , A.RTN_TIME , A.SEQ ) ";
                sSql += "   VALUES(";
                sSql += "           '" + dr["CARGO_ID"].ToString() + "'";
                sSql += "           , '" + dr["COP_NO"].ToString() + "' ";
                sSql += "           , '" + dr["WAYBILL_NO"].ToString() + "' ";
                sSql += "           , '관련문자전송' ";
                sSql += "           , '3002' ";
                //sSql += "           , TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS') ";
                //2040 보다 1초 늦게 셋팅 요청
                sSql += "           , (SELECT (RTN_TIME+1)AS RTN_NEW FROM CAINIAO_SEND_DATA WHERE HBL_NO= '" + dr["WAYBILL_NO"].ToString() + "' AND RTN_STATUS = '2040')";
                sSql += "           , '8' ";
                sSql += "); ";
            }


            //4000 번 항목 선택 됐을 떄만 추가
            if(dt.Rows[0]["CODE2"].ToString() != "0000")
            {

                string seq_state = "8";
                if (dt.Rows[0]["CODE2"].ToString() == "4004")
                {
                    seq_state = "10";
                }

                //4000번 항목 추가 

                sSql += " "; 
                sSql += " MERGE INTO CAINIAO_SEND_DATA A ";
                sSql += "   USING DUAL ";
                sSql += "   ON (A.HBL_NO = '"+dr["WAYBILL_NO"].ToString()+"' ";
                sSql += "       AND A.RTN_STATUS = '" + dt.Rows[0]["CODE2"].ToString() + "') ";
                sSql += "   WHEN NOT MATCHED THEN ";
                sSql += " INSERT (A.GUID, A.COP_NO , A.HBL_NO , A.RTN_KR , A.RTN_STATUS , A.RTN_TIME , A.SEQ ) ";
                sSql += "   VALUES(";
                sSql += "           '" + dr["CARGO_ID"].ToString() + "'";
                sSql += "           , '" + dr["COP_NO"].ToString() + "' ";
                sSql += "           , '" + dr["WAYBILL_NO"].ToString() + "' ";
                sSql += "           , '" + dt.Rows[0]["NAME2"].ToString() + "' ";
                sSql += "           , '" + dt.Rows[0]["CODE2"].ToString() + "' ";
                sSql += "           , TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS') ";
                sSql += "           , '"+ seq_state + "' ";
                sSql += "); ";

                //수집 종료 처리
                sSql += "    UPDATE UNIPASS_CARGO_INFO_MST SET MIG_YN = 'Y' , SEARCH_YN = 'N' ";
                sSql += "       WHERE HBL_NO ='"+ dr["WAYBILL_NO"].ToString() + "'; ";

                sSql += "   UPDATE EXCEL_HBL_MST SET UNI_YN = 'Y' ";
                sSql += "   WHERE HBL_NO ='"+ dr["WAYBILL_NO"].ToString() + "'; ";
                //수집 종료 처리
            }

            sSql += " END; ";

            int cnt = _DataHelper.ExecuteNonQuery(sSql, CommandType.Text);
            if (cnt == -1 ) rtnBool = true;


            return rtnBool;

        }

        #endregion

        #region CallBack Page 

        /// <summary>
        /// 하우스 리스트 조회
        /// </summary>
        /// <param name="dr"></param>
        /// <returns></returns>
        public static DataTable SearchNewCallBack(DataRow dr)
        {
            sSql = "";

            sSql += "SELECT API.STATE, ";
            sSql += "       API.KR, ";
            sSql += "       NVL (API.CL_MD, HBL.CLEAER_MODE) AS CLEAER_MODE, ";
            sSql += "       HBL.HBL_NO, ";
            sSql += "       HBL.MBL_NO, ";
            sSql += "       HBL.LOGISTIC_CD, ";
            sSql += "       HBL.COP_NO, ";
            sSql += "       HBL.UNIPASS_CARGO_INFO_ID, ";
            //sSql += "       HBL.STATUS, ";
            //sSql += "       HBL.UNI_YN, ";
            sSql += "       HBL.PARTNER_CD, ";
            sSql += "       HBL.BIG_BAG, ";
            if (dr["EXCEPT_TYPE"].ToString() == "NOCS")
            {
                //sSql += "       HBL.MBL_NO, ";
                sSql += "       (SELECT IE_TYPE FROM EXCEL_MBL_MST M WHERE M.MBL_NO = HBL.MBL_NO)AS IE_TYPE, ";
                sSql += "       (SELECT ETD FROM EXCEL_MBL_MST M WHERE M.MBL_NO = HBL.MBL_NO)AS ETD, ";
                sSql += "       (SELECT ETA FROM EXCEL_MBL_MST M WHERE M.MBL_NO = HBL.MBL_NO)AS ETA, ";
                sSql += "       (SELECT POL FROM EXCEL_MBL_MST M WHERE M.MBL_NO = HBL.MBL_NO)AS POL, ";
                sSql += "       (SELECT POD FROM EXCEL_MBL_MST M WHERE M.MBL_NO = HBL.MBL_NO)AS POD, ";
                sSql += "       (SELECT VOY FROM EXCEL_MBL_MST M WHERE M.MBL_NO = HBL.MBL_NO)AS VOY, ";
            }
            //else
            //{
            //    sSql += "       MBL.TRANS_TYPE, ";
            //    sSql += "       MBL.IE_TYPE, ";
            //    sSql += "       MBL.ETD, ";
            //    sSql += "       MBL.ETA, ";
            //    sSql += "       MBL.POL, ";
            //    sSql += "       MBL.POD, ";
            //    sSql += "       MBL.VOY, ";
            //}

            sSql += "       HBL.CURR_NM, ";
            sSql += "       HBL.TOT_PRICE, ";
            sSql += "       HBL.SHP_NM, ";
            sSql += "       HBL.SHP_ADDR, ";
            sSql += "       HBL.SHP_MO, ";
            sSql += "       HBL.CNE_NM, ";
            sSql += "       HBL.CNE_ADDR, ";
            sSql += "       HBL.CNE_MO, ";
            sSql += "       HBL.CNE_ZIP, ";
            sSql += "       HBL.CNE_MAIL, ";
            sSql += "       HBL.PASS_PORT ";
            sSql += "  FROM EXCEL_HBL_MST HBL ";
            sSql += "       LEFT JOIN (  SELECT HBL_NO, ";
            sSql += "                           MBL_NO, ";
            sSql += "                           GUID, ";
            sSql += "                           MAX (CLEAR_MODE) AS CL_MD, ";
            sSql += "                           MAX (RTN_STATUS) KEEP (DENSE_RANK LAST ORDER BY SEQ) ";
            sSql += "                              AS STATE, ";
            sSql += "                           MAX (RTN_KR) KEEP (DENSE_RANK LAST ORDER BY SEQ) KR ";
            sSql += "                      FROM CAINIAO_SEND_DATA ";
            sSql += "                     WHERE 1 = 1 ";
            sSql += "                     AND SEND_YN = 'Y' ";
            sSql += "                  GROUP BY HBL_NO, MBL_NO, GUID ";
            sSql += "                  ) API ";
            sSql += "          ON HBL.HBL_NO = API.HBL_NO ";
            sSql += "           AND HBL.UNIPASS_CARGO_INFO_ID = API.GUID ";
            //sSql += "          AND HBL.MBL_NO = API.MBL_NO "; //마스터 없는 건들도 있기때문 ..
            
            //if(dr["EXCEPT_TYPE"].ToString() != "NOCS") //미전송 건이 아닐때
            //{
            //    sSql += "       INNER JOIN EXCEL_MBL_MST MBL ";
            //    sSql += "          ON HBL.MBL_NO = MBL.MBL_NO ";
            //    sSql += "          AND MBL.LINE_CD IS NULL ";
            //    sSql += "          AND TO_DATE (MBL.ETA, 'YYYYMMDDHH24MISS') BETWEEN TO_DATE ('" + dr["STRT_YMD"].ToString() + "', 'YYYYMMDD') AND TO_DATE ('" + dr["END_YMD"].ToString() + "','YYYYMMDD') +1";

            //    //인천공항
            //    if (dr["PARTNER_CD"].ToString() == "GATE_31182453")
            //    {
            //        sSql += "          AND TRANS_TYPE = '5' ";
            //    }
            //    else // 해운
            //    {
            //        sSql += "          AND TRANS_TYPE = '2' ";
            //    }
            //}



            sSql += " WHERE 1 = 1 ";
            if (dr["SEARCH_VALUE"].ToString() != "")
            {
                sSql += "AND HBL." + dr["SEARCH_TYPE"].ToString() + "= '" + dr["SEARCH_VALUE"].ToString() + "' ";
            }
            if (dr["MULTI"].ToString() != "")
            {
                sSql += " AND (";
                string[] bl_list = dr["MULTI"].ToString().Split('\n');
                if (bl_list.Length > 0)
                {
                    for (int j = 0; j < bl_list.Length; j++)
                    {
                        if (j != 0) { sSql += " OR "; }
                        sSql += " HBL.HBL_NO = '" + bl_list[j].ToString() + "' ";
                        if (j == bl_list.Length - 1) { sSql += ") "; }
                    }
                }
            }

            //게이트 번호
            sSql += " AND HBL.PARTNER_CD = '" + dr["PARTNER_CD"].ToString() + "' ";
            //보류 멈춰있는 건들 
            if (dr["EXCEPT_TYPE"].ToString() == "CC")
            {
                sSql += " AND ( STATE = '3006') ";
            }
            if (dr["EXCEPT_TYPE"].ToString() == "EXCEP")
            {
                sSql += " AND (STATE != '3025'  AND STATE != '3006' AND STATE != '3007' AND STATE LIKE '30%') ";
            }
            if (dr["EXCEPT_TYPE"].ToString() == "PRICE")
            {
                sSql += " AND (STATE = '3007') ";
            }
            if (dr["EXCEPT_TYPE"].ToString() == "NONE")
            {
                sSql += " AND (STATE = '1000') ";
            }

            if (dr["EXCEPT_TYPE"].ToString() == "NONE2")
            {
                sSql += " AND (STATE = '3025') ";
            }


            if (dr["EXCEPT_TYPE"].ToString() == "NOCS")
            {
                sSql += "AND  STATE IS NULL ";
            }

            if (dr["EXCEPT_TYPE"].ToString() == "CARRIN")
            {
                sSql += " AND (STATE = '2030') ";
            }

            if (dr["EXCEPT_TYPE"].ToString() == "SIGNIN")
            {
                sSql += " AND (STATE = '2060') ";
            }

            if (dr["EXCEPT_TYPE"].ToString() == "CARROUT")
            {
                sSql += " AND (STATE = '2070') ";
            }

            sSql += "          AND TO_DATE (HBL.INS_YMD||HBL.INS_HM, 'YYYYMMDDHH24MISS') BETWEEN TO_DATE ('" + dr["STRT_YMD"].ToString() + "', 'YYYYMMDD') AND TO_DATE ('" + dr["END_YMD"].ToString() + "','YYYYMMDD') +1";

            dt = _DataHelper.ExecuteDataTable(sSql, CommandType.Text);

            return dt;
        }

        /// <summary>
        /// 업데이트 항목값
        /// </summary>
        /// <param name="dr"></param>
        /// <returns></returns>
        public static DataTable SearchCallBackList(DataRow dr)
        {

            string sType = dr["SEARCH_TYPE"].ToString();
            sSql = "";

            if (sType == "EXCEP" || sType == "TAX" || sType == "PRICE")
            {
                sSql += "SELECT '0000' AS SATAUS_CD , '--'||'FINAL STATE'||'--' AS NAME  , '" + sType + "' AS TYPE ";
                sSql += "FROM MAP_CAINIAO_CODE ";
                sSql += " UNION ";
            }

            sSql += "SELECT  REAL_CD AS SATAUS_CD, '(' || REAL_CD || ') ' || STATUS_US AS NAME  , '" + sType + "' AS TYPE  ";
            sSql += " FROM MAP_CAINIAO_CODE ";
            sSql += "    WHERE 1=1 ";



            switch (sType)
            {
                case "CC": // 2040 & 3006
                    sSql += " AND STD_CD = 'EXPT1' OR REAL_CD = '2050' OR REAL_CD = '3007' ";
                    break;
                case "EXCEP": // 30XX
                    sSql += " AND  REAL_CD = '2050'  ";
                    break;
                case "TAX": //3007
                    sSql += " AND REAL_CD = '4006' OR REAL_CD = '2050' ";
                    break;
                case "PRICE": //3007
                    sSql += " AND REAL_CD = '4006' OR REAL_CD = '2050' ";
                    break;
                case "NONE": //1000 일때
                    sSql += "AND SATAUS_CD = '3025' OR  SATAUS_CD = '2020' ";
                    break;
                case "NONE2": //3025 일때
                    sSql += "AND SATAUS_CD = '4010' OR  SATAUS_CD = '2020' ";
                    break;
                case "CARRIN": // 2020&2030일 때
                    sSql += "AND SATAUS_CD = '2040'  OR REAL_CD = '2050'  OR REAL_CD = '3001' ";
                    break;
                case "SIGNIN":
                    sSql += "AND SATAUS_CD = '2070' ";
                    break;
                case "NOCS":
                    sSql += "AND SATAUS_CD = '3025' OR  SATAUS_CD = '2020' ";
                    break;
            }


            sSql += " ORDER BY SATAUS_CD ";

            dt = _DataHelper.ExecuteDataTable(sSql, CommandType.Text);

            return dt;
        }

        #endregion

        #region Rate Page
        public static DataTable fnSearchRateData(DataRow dr)
        {
            sSql = "";

            sSql += " SELECT MB.MBL_NO ,MB.ETD , MB.ETA , MB.PARCEL_CNT ,ROUND((AV.\"'2020'\"/MB.PARCEL_CNT*100),2) AS RATE , AV.\"'2020'\" AS CARRY_IN,AV.\"'2040'\" AS WAIT , AV.\"'2050'\" AS DECLAR ,AV.\"'2070'\" AS CARRY_OUT ";
            sSql += " FROM EXCEL_MBL_MST MB ";
            sSql += " LEFT JOIN ( ";
            sSql += " SELECT * ";
            sSql += " FROM( ";
            sSql += " SELECT  RTN_STATUS , MBL_NO ";
            sSql += " FROM CAINIAO_SEND_DATA ";
            sSql += " WHERE 1=1 ";
            sSql += " AND (RTN_STATUS = '2020' OR RTN_STATUS = '2040' OR RTN_STATUS = '2050' OR RTN_STATUS = '2070') ";
            sSql += " ) ";
            sSql += " PIVOT (COUNT(RTN_STATUS) FOR RTN_STATUS IN('2020','2040','2050','2070')) ";
            sSql += " ) AV ";
            sSql += " ON MB.MBL_NO = AV.MBL_NO ";
            sSql += " WHERE 1=1 ";
            if (dr["MBL_NO"].ToString() != "") {
                sSql += " AND (";
                string[] bl_list = dr["MBL_NO"].ToString().Split('\n');
                for(int i = 0; i< bl_list.Length; i++)
                {
                    if(i != 0){sSql += " OR ";}

                    sSql += " REPLACE(MB.MBL_NO,'-','') = REPLACE('" + bl_list[i].ToString() + "','-','') ";
                    if (i == bl_list.Length - 1) { sSql += " ) "; }
                }
                
            //sSql += " AND MB.MBL_NO ='"+ dr["MBL_NO"].ToString() + "' ";
            }
            sSql += " AND TO_DATE(MB.ETA,'YYYYMMDDHH24MISS') BETWEEN TO_DATE('" + dr["STRT_YMD"].ToString() + "', 'YYYYMMDD') AND TO_DATE('" + dr["END_YMD"].ToString() + "', 'YYYYMMDD') +1";
            sSql += " ORDER BY ETA DESC ";

            dt = _DataHelper.ExecuteDataTable(sSql, CommandType.Text);

            return dt;

        }
        #endregion

    }
}
