using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using Newtonsoft.Json;
using System.Net;
using System.IO;
using System.Xml;
using EXPRESS_JWTNL.Models.Query;
using EXPRESS_JWTNL.Models;
using Formatting = Newtonsoft.Json.Formatting;
using System.Text;

namespace EXPRESS_JWTNL.Controllers
{
    public class OrderController : Controller
    {
        string strResult = "";
        string rtnJson = "";
        bool rtnStatus = false;
        public ActionResult order()
        {
            ViewBag.MENU1 = "Order";
            return View();
        }
        public ActionResult exception()
        {
            ViewBag.MENU2 = "Exception";
            return View();
        }
        public ActionResult recall()
        {
            ViewBag.MENU5 = "Recall";
            return View();
        }

        public ActionResult consol()
        {
            ViewBag.MENU3 = "Console";
            return View();
        }
        public ActionResult rate()
        {
            ViewBag.MENU4 = "Rate";
            return View();
        }



        public class JsonData
        {
            public string vJsonData { get; set; }
        }

        #region ★★★★★ 최종 사용 컨트롤러 ★★★★★

        #region Order New Controller

        /// <summary>
        /// 조회 쿼리
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public ActionResult fnNewOrderSearch(JsonData value)
        {
            string strResult = "";
            string Code = "";
            string Msg = "";

            try
            {
                string vJsonData = value.vJsonData.ToString();

                DataTable dt = new DataTable();
                DataTable Mdt = new DataTable();
                DataTable Ddt = new DataTable();
                DataTable Rdt = new DataTable();

                DataSet ds = new DataSet();
                dt = JsonConvert.DeserializeObject<DataTable>(vJsonData);
                Mdt = Sql_Master.SearchNewMblData(dt.Rows[0]);
                Mdt.TableName = "MBL_DATA";
                Ddt = Sql_Master.SearchNewHblData(dt.Rows[0]);
                Ddt.TableName = "HBL_DATA";

                if (Mdt.Rows.Count == 0 || Ddt.Rows.Count==0)
                {
                    Code = "N";
                    Msg = "No Data";
                }
                else
                {
                    Code = "Y";
                    Msg = "Success";
                }


                Rdt.Columns.Add("trxCode");
                Rdt.Columns.Add("trxMsg");
                DataRow row1 = Rdt.NewRow();
                row1["trxCode"] = Code;
                row1["trxMsg"] = Msg;
                Rdt.Rows.Add(row1);
                Rdt.TableName = "Result";
                

                //strResult = _common.MakeJson("Y", "Success", dt);
                strResult = _common.MakeJson(Rdt, Mdt,Ddt);

                OrderList model = new OrderList
                {
                    OrderDt = strResult.ToString()
                };

                return View("order", model);
            }
            catch(Exception e)
            {
                strResult = e.Message;
                return Json(strResult);
            }
        }

        /// <summary>
        /// 유니패스 팝업 관련 쿼리
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public ActionResult fnNewUnipassSearch(JsonData value)
        {
            string strResult = "";
            string Code = "";
            string Msg = "";
            try
            {
                string vJsonData = value.vJsonData.ToString();

                DataTable dt = new DataTable();
                DataTable Mdt = new DataTable();
                DataTable Hdt = new DataTable();
                DataTable Udt = new DataTable();

                DataSet ds = new DataSet();


                dt = JsonConvert.DeserializeObject<DataTable>(vJsonData);

                Mdt = Sql_Master.SearchNewUniMst(dt.Rows[0]);
                Mdt.TableName = "MST";
                ds.Tables.Add(Mdt);

                Hdt = Sql_Master.SearchNewUniMbl(dt.Rows[0]);
                Hdt.TableName = "DTL";
                ds.Tables.Add(Hdt);

                Udt = Sql_Master.SearchNewUniDtl(dt.Rows[0]);
                Udt.TableName = "UDTL";
                ds.Tables.Add(Udt);

                if (Mdt.Rows.Count == 0 || Hdt.Rows.Count == 0 || Udt.Rows.Count == 0)
                {
                    Code = "N";
                    Msg = "No Data";
                }
                else
                {
                    Code = "Y";
                    Msg = "Success";
                }


                dt = new DataTable();
                dt.Columns.Add("trxCode");
                dt.Columns.Add("trxMsg");
                DataRow row1 = dt.NewRow();
                row1["trxCode"] = Code;
                row1["trxMsg"] = Msg;
                dt.Rows.Add(row1);
                dt.TableName = "Result";
                ds.Tables.Add(dt);
                strResult = JsonConvert.SerializeObject(ds);

                return Json(strResult);
            }
            catch(Exception e)
            {
                strResult = e.Message;
                return Json(strResult);
            }
        }

        #endregion


        #region 예외 처리 30xx & 40xx


        public string fnExceptSoloSendSave(JsonData value)
        {
            string strResult = "";
            string vJsonData = value.vJsonData.ToString();
            DataTable dt = JsonConvert.DeserializeObject<DataTable>(vJsonData);
            DataTable CopyDt = new DataTable();
            DataTable S1Dt = new DataTable();
            DataTable S2Dt = new DataTable();
            DataSet ds = new DataSet();


            //전송

            #region 전송 데이터 만들기
            //리스트
            S1Dt = dt.Copy();
            S1Dt.TableName = "LIST";
            S1Dt.Columns.Remove("MBL_NO");
            S1Dt.Columns.Remove("RTN_TIME");
            S1Dt.Columns.Remove("PARTNER_CD");

            //불필요 컬럼 제거


            //마스터
            S2Dt.Columns.Add("bizKey", typeof(string));
            S2Dt.Columns.Add("partnerCd", typeof(string));
            //S2Dt.Columns.Add("mblNo", typeof(string));
            S2Dt.TableName = "MAIN";

            DataRow dr = S2Dt.NewRow();
            dr["bizKey"] = dt.Rows[0]["copNo"].ToString();
            dr["partnerCd"] = dt.Rows[0]["PARTNER_CD"].ToString();
            S2Dt.Rows.Add(dr);
            //dr["mblNo"] = dt.Rows[0]["MBL_NO"].ToString();

            ds.Tables.Add(S2Dt);
            ds.Tables.Add(S1Dt);

            string SendJsonData= "";
            SendJsonData = JsonConvert.SerializeObject(ds, Formatting.Indented);
            //string Url = "http://localhost:49876/";
            string Url = "http://api.elvisexp.co.kr/";
            string resultVal = fnGetAPIData("POST", Url + "/api/JWTNL/requestParcelCallBack", SendJsonData);

            #endregion

            //api call
            if (resultVal.Contains("Success"))
            {

                #region 저장
                rtnStatus = Sql_Master.SaveSendExpt(dt);

                if (rtnStatus)
                {
                    strResult = _common.MakeJson("Y", "Success");
                }
                else
                {
                    strResult = _common.MakeJson("N", "No Data");
                }
                #endregion
            }
            else
            {
                strResult = _common.MakeJson("N", "Fail");
            }



            return strResult;
        }

        /// <summary>
        /// 코드 바인딩
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public string fnSearchHold(JsonData value)
        {
            string strResult = "";
            string vJsonData = value.vJsonData.ToString();
            DataTable dt = JsonConvert.DeserializeObject<DataTable>(vJsonData);
            dt = Sql_Master.SearchHoldData(dt.Rows[0]);
            dt.TableName = "HOLD";

            if (dt.Rows.Count == 0)
            {
                strResult = _common.MakeJson("N", "", dt);
            }
            else
            {
                strResult = _common.MakeJson("Y", "", dt);
            }





            return strResult;
        }


        /// <summary>
        /// 예외 저장
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public string fnInsertHold(JsonData value)
        {
            string strResult = "";
            try
            {
                string vJsonData = value.vJsonData.ToString();
                DataTable dt = JsonConvert.DeserializeObject<DataTable>(vJsonData);

                rtnStatus = Sql_Master.LeftInsertHold(dt.Rows[0]);
                if (dt.Rows[0]["RIGHT_SEARCH_CD"].ToString() != "0000")
                {
                    rtnStatus = Sql_Master.RightInsertHold(dt.Rows[0]);
                }


                if (rtnStatus)
                {
                    strResult = _common.MakeJson("Y", "Success");
                }
                else
                {
                    strResult = _common.MakeJson("N", "No Data");
                }


                return strResult;
            }
            catch (Exception e)
            {
                strResult = e.Message;
                return strResult;
            }
        }
        #endregion

        #region Exception 화면 
        /// <summary>
        /// 조회
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public ActionResult fnSearchNewExcept(JsonData value)
        {
            string strResult = "";
            string Code = "";
            string Msg = "";

            try
            {
                string vJsonData = value.vJsonData.ToString();

                DataTable dt = new DataTable();
                DataTable Mdt = new DataTable();
                DataTable Ddt = new DataTable();
                DataTable Rdt = new DataTable();

                DataSet ds = new DataSet();
                dt = JsonConvert.DeserializeObject<DataTable>(vJsonData);

                // 조회조건 조건값 있을때 
                if (!string.IsNullOrEmpty(dt.Rows[0]["SEARCH_VALUE"].ToString()))
                {
                    Mdt = Sql_Master.SearchNewMblData(dt.Rows[0]);
                    Mdt.TableName = "MBL_DATA";

                }

                Ddt = Sql_Master.SearchNewExcept(dt.Rows[0]);
                Ddt.TableName = "EXCEP";


                if (Mdt.Rows.Count == 0 || Ddt.Rows.Count == 0)
                {
                    Code = "N";
                    Msg = "No Data";
                }
                else
                {
                    Code = "Y";
                    Msg = "Success";
                }


                Rdt.Columns.Add("trxCode");
                Rdt.Columns.Add("trxMsg");
                DataRow row1 = Rdt.NewRow();
                row1["trxCode"] = Code;
                row1["trxMsg"] = Msg;
                Rdt.Rows.Add(row1);
                Rdt.TableName = "Result";


                //strResult = _common.MakeJson("Y", "Success", dt);
                strResult = _common.MakeJson(Rdt, Mdt, Ddt);

                OrderList model = new OrderList
                {
                    OrderDt = strResult.ToString()
                };

                return View("order", model);
            }
            catch (Exception e)
            {
                strResult = e.Message;
                return Json(strResult);
            }
        }


        /// <summary>
        /// 리스트 형식으로 센딩
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public  string fnSendMulti(JsonData value)
        {
            string strResult = "";
            string vJsonData = value.vJsonData.ToString();
            string bizkey = "";
            string partner_cd = "";

            DataSet dst = JsonConvert.DeserializeObject<DataSet>(vJsonData);

            DataTable dt = dst.Tables["LIST"];
            DataTable CopyDt = new DataTable();
            DataTable S1Dt = new DataTable();
            DataTable S2Dt = new DataTable();
            DataTable MultiDt = null;
            DataSet ds = new DataSet();

            try
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (MultiDt == null)
                    {
                        MultiDt = dt.Clone(); // 테이블 형식 복사
                    }

                    MultiDt.ImportRow(dt.Rows[i]);

                    if (MultiDt.Rows.Count > 300)
                    {
                        #region 전송 가공
                        S1Dt = MultiDt.Copy();

                        bizkey = MultiDt.Rows[0]["copNo"].ToString();
                        partner_cd = MultiDt.Rows[0]["PARTNER_CD"].ToString();

                        S1Dt.TableName = "LIST";
                        S1Dt.Columns.Remove("MBL_NO");
                        S1Dt.Columns.Remove("RTN_TIME");
                        S1Dt.Columns.Remove("PARTNER_CD");
                        if (S1Dt.Columns.Contains("CLEAR_MODE"))
                        {
                            S1Dt.Columns.Remove("CLEAR_MODE");
                        }

                        
                        S2Dt.Columns.Add("bizKey", typeof(string));
                        S2Dt.Columns.Add("partnerCd", typeof(string));
                        S2Dt.TableName = "MAIN";

                        DataRow dr1 = S2Dt.NewRow();
                        dr1["bizKey"] = bizkey;
                        dr1["partnerCd"] = partner_cd;
                        S2Dt.Rows.Add(dr1);
                        S2Dt.TableName = "MAIN";

                        ds.Tables.Add(S2Dt);
                        ds.Tables.Add(S1Dt);
                        #endregion

                        string SendJsonData = "";
                        SendJsonData = JsonConvert.SerializeObject(ds, Formatting.Indented);
                        //string Url = "http://localhost:49876/";
                        string Url = "http://api.elvisexp.co.kr/";
                        string resultVal = fnGetAPIData("POST", Url + "/api/JWTNL/requestParcelCallBack", SendJsonData);

                        //api call
                        if (resultVal.Contains("Success"))
                        {
                            #region 저장
                            rtnStatus = Sql_Master.SaveSendExpt(MultiDt);

                            if (rtnStatus)
                            {
                                strResult = _common.MakeJson("Y", "Success");
                            }
                            else
                            {
                                strResult = _common.MakeJson("N", "No Data");
                            }
                            #endregion
                        }
                        else
                        {
                            strResult = _common.MakeJson("N", "Fail");
                        }
                        MultiDt = null;
                    }
                }
                #region 전송 가공
                if(MultiDt != null)
                {
                    S1Dt = MultiDt.Copy();

                    bizkey = MultiDt.Rows[0]["copNo"].ToString();
                    partner_cd = MultiDt.Rows[0]["PARTNER_CD"].ToString();

                    S1Dt.TableName = "LIST";
                    S1Dt.Columns.Remove("MBL_NO");
                    S1Dt.Columns.Remove("RTN_TIME");
                    S1Dt.Columns.Remove("PARTNER_CD");
                    if (S1Dt.Columns.Contains("CLEAR_MODE"))
                    {
                        S1Dt.Columns.Remove("CLEAR_MODE");
                    }


                    S2Dt.Columns.Add("bizKey", typeof(string));
                    S2Dt.Columns.Add("partnerCd", typeof(string));
                    S2Dt.TableName = "MAIN";

                    DataRow dr1 = S2Dt.NewRow();
                    dr1["bizKey"] = bizkey;
                    dr1["partnerCd"] = partner_cd;
                    S2Dt.Rows.Add(dr1);
                    S2Dt.TableName = "MAIN";

                    ds.Tables.Add(S2Dt);
                    ds.Tables.Add(S1Dt);
                    #endregion

                    string SendJsonData = "";
                    SendJsonData = JsonConvert.SerializeObject(ds, Formatting.Indented);
                    //string Url = "http://localhost:49876/";
                    string Url = "http://api.elvisexp.co.kr/";
                    string resultVal = fnGetAPIData("POST", Url + "/api/JWTNL/requestParcelCallBack", SendJsonData);

                    //api call
                    if (resultVal.Contains("Success"))
                    {
                        #region 저장
                        rtnStatus = Sql_Master.SaveSendExpt(MultiDt);

                        if (rtnStatus)
                        {
                            strResult = _common.MakeJson("Y", "Success");
                        }
                        else
                        {
                            strResult = _common.MakeJson("N", "No Data");
                        }
                        #endregion
                    }
                    else
                    {
                        strResult = _common.MakeJson("N", "Fail");
                    }
                    MultiDt = null;
                }
              
            }
            catch(Exception e)
            {
                strResult = _common.MakeJson("E", "Error");
            }
            //전송



            return strResult;
        }


        #endregion

        #region 콘솔화면
        public ActionResult fnGetList(JsonData value)
        {
            string strResult = "";

            bool rtnbool = false;

            string vJsonData = value.vJsonData.ToString();

            //string Mngt_No = DateTime.Now.ToString("yyyyMMddHH24mmssfff");

            DataTable dt = new DataTable();
            DataTable Mdt = new DataTable();
            DataTable Ddt = new DataTable();


            DataSet ds = new DataSet();
            dt = JsonConvert.DeserializeObject<DataTable>(vJsonData);

            rtnbool = Sql_Consol.insertTemp(dt);

            if (rtnbool)
            {
                string tableFlag = "";
                // 주 의 위에 INSERTTEMP에서 HBL_NO로 컬럼명 변경해서 이렇게 써야댐
                if (dt.Columns.Contains("HBL_NO"))
                {
                    tableFlag = "HBL";
                }
                else
                {
                    tableFlag = "LOC";
                }
                string Param_Mngt_No = dt.Rows[0]["MNGT_NO"].ToString();
                //조회
                dt = Sql_Consol.SelectUsrHbl(Param_Mngt_No, tableFlag);
                //삭제 
                rtnbool = Sql_Consol.deleteTemp(Param_Mngt_No, tableFlag);
            }
            strResult = _common.MakeJson("Y", "Success", dt);

            ConsoleList model = new ConsoleList
            {
                ConsoleDt = strResult.ToString()
            };
            return View("consol", model);
        }

        #endregion

        #region CallBack 화면


        public ActionResult fnSearchNewCallBack(JsonData value)
        {
            string strResult = "";
            string Code = "";
            string Msg = "";

            try
            {
                string vJsonData = value.vJsonData.ToString();

                DataTable dt = new DataTable();
                DataTable Mdt = new DataTable();
                DataTable Ddt = new DataTable();
                DataTable Rdt = new DataTable();

                DataSet ds = new DataSet();
                dt = JsonConvert.DeserializeObject<DataTable>(vJsonData);

                // 조회조건 조건값 있을때 
                if (!string.IsNullOrEmpty(dt.Rows[0]["SEARCH_VALUE"].ToString()))
                {
                    Mdt = Sql_Master.SearchNewMblData(dt.Rows[0]);
                    Mdt.TableName = "MBL_DATA";

                }

                Ddt = Sql_Master.SearchNewCallBack(dt.Rows[0]);
                Ddt.TableName = "CallBack";


                if (Mdt.Rows.Count == 0 || Ddt.Rows.Count == 0)
                {
                    Code = "N";
                    Msg = "No Data";
                }
                else
                {
                    Code = "Y";
                    Msg = "Success";
                }


                Rdt.Columns.Add("trxCode");
                Rdt.Columns.Add("trxMsg");
                DataRow row1 = Rdt.NewRow();
                row1["trxCode"] = Code;
                row1["trxMsg"] = Msg;
                Rdt.Rows.Add(row1);
                Rdt.TableName = "Result";


                //strResult = _common.MakeJson("Y", "Success", dt);
                strResult = _common.MakeJson(Rdt, Mdt, Ddt);

                OrderList model = new OrderList
                {
                    OrderDt = strResult.ToString()
                };

                return View("order", model);
            }
            catch (Exception e)
            {
                strResult = e.Message;
                return Json(strResult);
            }
        }

        #endregion


        #region Rate 화면
        public ActionResult fnSearchRateData(JsonData value)
        {
            string strResult = "";
            try
            {
                string vJsonData = value.vJsonData.ToString();

                DataTable dt = new DataTable();


                dt = JsonConvert.DeserializeObject<DataTable>(vJsonData);

                dt = Sql_Master.fnSearchRateData(dt.Rows[0]);
                strResult = _common.MakeJson("Y", "Success", dt);

                Rate model = new Rate
                {
                    RateDt = strResult.ToString()
                };
                return View("rate", model);
            }
            catch (Exception e)
            {
                strResult = e.Message;
                return Json(strResult);
            }
        }

        #endregion

        #endregion



        #region Order 화면
        public ActionResult fnSearchData(JsonData value)
        {
            string strResult = "";
            try
            {
                string vJsonData = value.vJsonData.ToString();

                DataTable dt = new DataTable();
                DataTable Mdt = new DataTable();
                DataTable Ddt = new DataTable();


                DataSet ds = new DataSet();
                dt = JsonConvert.DeserializeObject<DataTable>(vJsonData);

                dt = Sql_Master.SearchDtlData(dt.Rows[0]);
                strResult = _common.MakeJson("Y", "Success", dt);

                OrderList model = new OrderList
                {
                    OrderDt = strResult.ToString()
                };
                return View("order", model);
            }
            catch (Exception e)
            {
                strResult = e.Message;
                return Json(strResult);
            }
        }


        [HttpPost]
        public string fnSaveData(JsonData value)
        {
            string strResult = "";
            try
            {
                string vJsonData = value.vJsonData.ToString();
                DataTable dt = JsonConvert.DeserializeObject<DataTable>(vJsonData);
                string status = Sql_Excel.TestExcelData(dt);
                if(status == "Y" && dt.Rows[0]["SORT"].ToString() == "M")
                {
                    strResult = "Y";
                }
                else if (status == "Y" && dt.Rows[0]["SORT"].ToString() == "H")
                {
                    strResult = "Y";
                }
                else if (status == "Y" && dt.Rows[0]["SORT"].ToString() == "S")
                {
                    //MBL, HBL 저장된 뒤 SKU 저장할때 프로시져 태우기
                    strResult = Sql_Excel.SaveExcelData(dt);
                }
                else
                {
                    strResult = "N";
                }


            }
            catch (Exception e)
            {
                //return e.Message;
            }
            return strResult;
        }


        public string fnDeleteExcel(JsonData value)
        {
            string strResult = "";
            int nResult = 0;
            int nResult1 = 0;
            int nResult2 = 0;

            string vJsonData = value.vJsonData.ToString();
            DataTable dt = JsonConvert.DeserializeObject<DataTable>(vJsonData);
            DataTable Mdt = new DataTable();
            DataTable Hdt = new DataTable();
            try { 
                for(int i = 0; dt.Rows.Count > i; i++)
                {
                    nResult = _DataHelper.ExecuteNonQuery(Sql_Excel.DeleteSkuDetail(dt.Rows[i]), CommandType.Text);
                    Mdt = Sql_Excel.SearchSkuData(dt.Rows[i]);
                   if (Mdt.Rows.Count == 0)
                     {
                        nResult1 = _DataHelper.ExecuteNonQuery(Sql_Excel.DeleteHblMst(dt.Rows[i]), CommandType.Text);
                     }
                }

                Hdt = Sql_Excel.SearchHblData(dt.Rows[0]);
                if (Hdt.Rows.Count == 0)
                {
                    nResult2 = _DataHelper.ExecuteNonQuery(Sql_Excel.DeleteMblMst(dt.Rows[0]), CommandType.Text);
                }

            }
            catch (Exception e)
            {
                strResult = _common.MakeJson("E", e.Message);
                return strResult;
            }
            if (nResult > 0)
            {
                strResult = _common.MakeJson("Y", "값 불러오기 성공");

            }
            else
            {
                strResult = _common.MakeJson("N", "값 불러오기 실패");
                return strResult;
            }



            return strResult;

        }



        public ActionResult fnUniSearchData(JsonData value)
        {
            string strResult = "";
            try
            {
                string vJsonData = value.vJsonData.ToString();

                DataTable dt = new DataTable();
                DataTable Mdt = new DataTable();
                DataTable Ddt = new DataTable();
                DataTable UMdt = new DataTable();
                DataTable UDdt = new DataTable();


                DataSet ds = new DataSet();
                dt = JsonConvert.DeserializeObject<DataTable>(vJsonData);

                Mdt = Sql_Master.SearchMstData(dt.Rows[0]); //MBL 정보
                Mdt.TableName = "MST";
                ds.Tables.Add(Mdt);

                Ddt = Sql_Master.SearchDtlData(dt.Rows[0]); //DTL
                Ddt.TableName = "DTL";
                ds.Tables.Add(Ddt);

                UMdt = Sql_Master.SearchUniMstData(dt.Rows[0]); //유니패스쪽 MST
                UMdt.TableName = "UMST";
                ds.Tables.Add(UMdt);

                UDdt = Sql_Master.SearchUniDtlData(dt.Rows[0]); // 유니패스 DTL
                UDdt.TableName = "UDTL";
                ds.Tables.Add(UDdt);


                dt = new DataTable();
                dt.Columns.Add("trxCode");
                dt.Columns.Add("trxMsg");
                DataRow row1 = dt.NewRow();
                row1["trxCode"] = "Y";
                row1["trxMsg"] = "Success";
                dt.Rows.Add(row1);
                dt.TableName = "Result";
                ds.Tables.Add(dt);
                strResult = JsonConvert.SerializeObject(ds);
                return Json(strResult);
            }
            catch (Exception e)
            {
                strResult = e.Message;
                return Json(strResult);
            }
        }

        #endregion

        #region Exception 조회

        public ActionResult fnSearchExData(JsonData value)
        {
            string strResult = "";
            try
            {
                string vJsonData = value.vJsonData.ToString();

                DataTable dt = new DataTable();
                DataTable Mdt = new DataTable();
                DataTable Ddt = new DataTable();


                DataSet ds = new DataSet();
                dt = JsonConvert.DeserializeObject<DataTable>(vJsonData);

                dt = Sql_Master.SearchExDtlData(dt.Rows[0]);
                strResult = _common.MakeJson("Y", "Success", dt);

                OrderList model = new OrderList
                {
                    OrderDt = strResult.ToString()
                };
                return View("order", model);

            }
            catch (Exception e)
            {
                strResult = e.Message;
                return Json(strResult);
            }
        }


        public string fnInsertExcept(JsonData value)
        {
            string strResult = "";
            string vJsonData = value.vJsonData.ToString();

            DataSet ds = JsonConvert.DeserializeObject<DataSet>(vJsonData);

            //DataTable dt = JsonConvert.DeserializeObject<DataTable>(vJsonData);
            //DataTable dt = new DataTable();

            #region Update
            //일괄 업데이트 항목들
            foreach(DataRow dr in ds.Tables["LIST"].Rows) 
            {
                rtnStatus = Sql_Master.AddExceptFlag(dr , ds.Tables["EXCEPT"]);
            }


            #endregion


            if (rtnStatus)
            {
                strResult = _common.MakeJson("Y", "Success");
            }
            else
            {
                strResult = _common.MakeJson("N", "No Data");
            }


            return strResult;

        }

        #endregion





        #region Common Logic


        public string fnGetAPIData(string strType, string strURL, /*string auth_type, string strToken,*/ string strParam)
        {
            string strJson = "";
            try
            {
                string URL = strURL;
                if (strType == "GET")
                {
                    URL = String.Format("{0}?{1}", strURL, strParam);
                }

                HttpWebRequest request = WebRequest.Create(URL) as HttpWebRequest;
                //if (!string.IsNullOrEmpty(strToken))
                //{
                //    request.Headers.Add("Authorization-Type", auth_type);
                //    request.Headers.Add("Authorization-Token", strToken);
                //}
                request.Method = strType.ToUpper();
                request.ContentType = "application/json;charset=UTF-8";

                if (strType == "POST")
                {
                    Byte[] byteDataParams = Encoding.UTF8.GetBytes(strParam);
                    request.ContentLength = byteDataParams.Length;

                    Stream st = request.GetRequestStream();
                    st.Write(byteDataParams, 0, byteDataParams.Length);
                    st.Close();
                }

                HttpWebResponse response;
                response = request.GetResponse() as HttpWebResponse;

                Stream stream = response.GetResponseStream();
                StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                string strResult = reader.ReadToEnd();
                //strResult = strResult.ToString().Replace("\\r", "/&r").Replace("\\n", "/&n").Replace("\\", String.Empty).Replace("/&r", "\\r").Replace("/&n", "\\n").Remove(0, 1).Substring(0,strResult.Length-1);
                strResult = strResult.ToString().Replace("\\r", "/&r").Replace("\\n", "/&n").Replace("\\", String.Empty).Replace("/&r", "\\r").Replace("/&n", "\\n");
                strResult = strResult.Remove(0, 1);
                strResult = strResult.Substring(0, strResult.Length - 1);
                //DataSet ds = JsonConvert.DeserializeObject<DataSet>(strResult);
                stream.Close();
                response.Close();
                reader.Close();

                return strResult;
            }
            catch (Exception e)
            {
                strJson = e.Message;
                return strJson;
            }
        }

        /// <summary>
        /// 한번에 두건 조회할때
        /// </summary>
        /// <returns></returns>
        public ActionResult LoadExcept()
        {
            string strResult = "";

            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            try
            {
                DataTable dt1 = Sql_Master.SearchExcep1();
                dt1.TableName = "LEFT_EXCEPT";
                ds.Tables.Add(dt1);

                DataTable dt2 = Sql_Master.SearchExcep2();
                dt2.TableName = "RIGHT_EXCEPT";
                ds.Tables.Add(dt2);

                dt = new DataTable();
                dt.Columns.Add("trxCode");
                dt.Columns.Add("trxMsg");
                DataRow row1 = dt.NewRow();
                row1["trxCode"] = "Y";
                row1["trxMsg"] = "Success";
                dt.Rows.Add(row1);
                dt.TableName = "Result";
                ds.Tables.Add(dt);
                strResult = JsonConvert.SerializeObject(ds);

                
                return Json(strResult);
            }
            catch(Exception e)
            {
                strResult = e.Message;
                return Json(strResult);
            }

        }

        /// <summary>
        /// 단건 익셉션 조회
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public ActionResult BindExcept(JsonData value)
        {
            string strResult = "";
            DataTable dt = new DataTable();
            string vJsonData = value.vJsonData.ToString();

            DataTable rdt = JsonConvert.DeserializeObject<DataTable>(vJsonData);

            try
            {
                dt = Sql_Master.SearchExcepList(rdt.Rows[0]);
                dt.TableName = "EXCEP_LIST";
                strResult = _common.MakeJson("Y", "Success", dt);

                return Json(strResult);
            }
            catch(Exception e)
            {
                strResult = e.Message;
                return Json(strResult);
            }
        }

        /// <summary>
        /// 단건 강제전송 항목 조회
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public ActionResult BindCallbackSend(JsonData value)
        {
            string strResult = "";
            DataTable dt = new DataTable();
            string vJsonData = value.vJsonData.ToString();

            DataTable rdt = JsonConvert.DeserializeObject<DataTable>(vJsonData);

            try
            {
                dt = Sql_Master.SearchCallBackList(rdt.Rows[0]);
                dt.TableName = "NEXT_LIST";
                strResult = _common.MakeJson("Y", "Success", dt);

                return Json(strResult);
            }
            catch (Exception e)
            {
                strResult = e.Message;
                return Json(strResult);
            }
        }

        #endregion

        #region 유니패스 api 수신 테스트(미사용) 
        [HttpPost]
        public string GetUnipass()
        {
            string strResult = "";

            try
            {
                
                DataTable dt = new DataTable();
                
                dt = Sql_Master.SearchData();

                for (int i = 0; dt.Rows.Count > i; i++)
                {

                    string hbl_no = dt.Rows[i]["HBL_NO"].ToString();
                    string blYy = dt.Rows[i]["INS_TIME"].ToString().Substring(0, 4);

                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://unipass.customs.go.kr:38010/ext/rest/cargCsclPrgsInfoQry/retrieveCargCsclPrgsInfo?crkyCn=x290k179s019l179p060t090h0&hblNo=" + hbl_no + "&blYy=" + blYy);
                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    using (Stream stream = response.GetResponseStream())
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        strResult = reader.ReadToEnd();



                        // XML 문자열을 XmlDocument 객체로 파싱
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.LoadXml(strResult);

                        // cargCsclPrgsInfoQryVo 요소의 모든 하위 노드를 가져오기
                        XmlNodeList nodes = xmlDoc.SelectNodes("//cargCsclPrgsInfoQryVo/*");

                        // cargCsclPrgsInfoDtlQryVo 요소의 모든 하위 노드를 가져오기
                        XmlNodeList dtlNodes = xmlDoc.SelectNodes("//cargCsclPrgsInfoQryRtnVo/cargCsclPrgsInfoDtlQryVo");
                        // cargCsclPrgsInfoDtlQryVo 요소의 모든 하위 노드를 가져오기
                        //XmlNodeList dtlNodesdtl = xmlDoc.SelectSingleNode("//cargCsclPrgsInfoQryRtnVo/cargCsclPrgsInfoDtlQryVo").ChildNodes;

                        //XmlNodeList dtlNodes2 = xmlDoc.SelectNodes("//cargCsclPrgsInfoQryRtnVo/cargCsclPrgsInfoDtlQryVo/*");
                        XmlNode tCntNode = xmlDoc.SelectSingleNode("//tCnt");

                        // 빈 데이터 테이블 생성
                        DataTable MstTable = new DataTable();
                        DataTable DetaileTable = new DataTable();

                        // XML 노드 순회하며 데이터 테이블에 추가
                        MstTable.Rows.Add();
                        foreach (XmlNode node in nodes)
                        {
                            MstTable.Columns.Add(node.Name);
                            MstTable.Rows[0][node.Name] = node.InnerText;
                            //dataTable.Rows.Add(node.Name, node.InnerText);
                        }
                        rtnStatus = Sql_Master.InsUnipass(MstTable.Rows[0]);
                        string HBL_NO = MstTable.Rows[0]["hblNo"].ToString();
                        string MBL_NO = MstTable.Rows[0]["mblNo"].ToString();
                        string CNTR_NO = MstTable.Rows[0]["cntrNo"].ToString();
                        string SEQ = tCntNode.InnerText;
                        // 상세 테이블 행 추가 및 각 행에 해당하는 노드 값 설정
                        foreach (XmlNode detailNode in dtlNodes)
                        {
                            DataRow newRow = DetaileTable.NewRow();
                            foreach (XmlNode node in detailNode.ChildNodes)
                            {
                                if (!DetaileTable.Columns.Contains(node.Name))
                                {
                                    DetaileTable.Columns.Add(node.Name);
                                }
                                newRow[node.Name] = node.InnerText;
                            }
                            DetaileTable.Rows.Add(newRow);
                        }

                        rtnStatus = Sql_Master.InsDtlUnipass(DetaileTable, HBL_NO,MBL_NO, CNTR_NO, SEQ);

                    }

                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return strResult;
        }
        #endregion
    }
}