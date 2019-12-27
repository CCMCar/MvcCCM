using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CCM.Easy.Car.Credit.DataModels;
using Newtonsoft.Json;

namespace CCM.Easy.Car.Credit.Mvc.Controllers
{
    /// <summary>
    /// 12月27日 MVC控制器的显示
    /// </summary>
    public class StateInfoMVCController : Controller
    {
        HttpClientHelper helper = new HttpClientHelper("https://localhost:44386/api/StateInfo/");
        // GET: Default
        public ActionResult Index()
        {
            string json = helper.Get("Collect");
            DataTable table = JsonConvert.DeserializeObject<DataTable>(json);
            ViewBag.nnn = table;
            return View();
        }
    }
}