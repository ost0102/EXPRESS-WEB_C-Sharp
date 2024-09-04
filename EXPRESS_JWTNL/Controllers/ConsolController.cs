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
    public class ConsolController : Controller
    {
        string rtnJson = "";
        bool rtnStatus = false;

        //public ActionResult consol()
        //{
        //    return View();
        //}

        public class JsonData
        {
            public string vJsonData { get; set; }
        }


    }
   
}