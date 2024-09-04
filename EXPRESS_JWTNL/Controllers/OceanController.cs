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

namespace EXPRESS_JWTNL.Controllers
{
    public class OceanController : Controller
    {
        string rtnJson = "";
        bool rtnStatus = false;
        public ActionResult order()
        {
            return View();
        }
        public ActionResult orderlist()
        {
            return View();
        }

        public class JsonData
        {
            public string vJsonData { get; set; }
        }


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
                //Mdt = Sql_Master.SearchMstData(dt.Rows[0]);
                //Mdt.TableName = "MST";
                //ds.Tables.Add(Mdt);

                //Ddt = Sql_Master.SearchDtlData(dt.Rows[0]);
                //Ddt.TableName = "DTL";
                //ds.Tables.Add(Ddt);


                //dt = new DataTable();
                //dt.Columns.Add("trxCode");
                //dt.Columns.Add("trxMsg");
                //DataRow row1 = dt.NewRow();
                //row1["trxCode"] = "Y";
                //row1["trxMsg"] = "Success";
                //dt.Rows.Add(row1);
                //dt.TableName = "Result";
                //ds.Tables.Add(dt);
                //strResult = JsonConvert.SerializeObject(ds);
                //return Json(strResult);
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
                DataSet ds = JsonConvert.DeserializeObject<DataSet>(vJsonData);
                string status = Sql_Excel.TestExcelData(ds);
                if (status == "Y")
                {
                    strResult = JsonConvert.SerializeObject(Sql_Excel.SaveExcelData(ds));
                }
                else
                {

                }


            }
            catch (Exception e)
            {
                //return e.Message;
            }
            return strResult;
        }


        public string fnInsertHold(JsonData value)
        {
            string strResult = "";
            try
            {
                string vJsonData = value.vJsonData.ToString();
                DataTable dt = JsonConvert.DeserializeObject<DataTable>(vJsonData);

                rtnStatus = Sql_Master.LeftInsertHold(dt.Rows[0]);
                if(dt.Rows[0]["RIGHT_SEARCH_CD"].ToString() != "0000")
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

        public string fnSearchHold(JsonData value)
        {
           string strResult = "";
           string vJsonData = value.vJsonData.ToString();
           DataTable dt = JsonConvert.DeserializeObject<DataTable>(vJsonData);
           dt = Sql_Master.SearchHoldData(dt.Rows[0]);
           dt.TableName = "HOLD";

            if (dt.Rows.Count == 0)
            {
                strResult = _common.MakeJson("N", "",dt);
            }
            else
            {
                strResult = _common.MakeJson("Y", "",dt);
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

                Mdt = Sql_Master.SearchMstData(dt.Rows[0]);
                Mdt.TableName = "MST";
                ds.Tables.Add(Mdt);

                Ddt = Sql_Master.SearchDtlData(dt.Rows[0]);
                Ddt.TableName = "DTL";
                ds.Tables.Add(Ddt);

                UMdt = Sql_Master.SearchUniMstData(dt.Rows[0]);
                UMdt.TableName = "UMST";
                ds.Tables.Add(UMdt);

                UDdt = Sql_Master.SearchUniDtlData(dt.Rows[0]);
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
    }
}