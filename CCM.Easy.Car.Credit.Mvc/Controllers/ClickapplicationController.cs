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
    public class ClickapplicationController : Controller
    {
        HttpClientHelper Http = new HttpClientHelper("https://localhost:44386/Api/Clickapplication2/");
        HttpClientHelper Http1 = new HttpClientHelper("https://localhost:44386/Api/Clickapplication1/");
        HttpClientHelper Http2 = new HttpClientHelper("https://localhost:44386/api/Clickapplication/");
        HttpClientHelper Http3 = new HttpClientHelper("https://localhost:44386/api/Clickapplication/");
        // GET: Default
        /// <summary>
        /// 显示，需要车辆编号和车辆名字
        /// 贷款方案
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            CarInfo model = new CarInfo();
            model.CarId = 0;
            model.CarName = "";
            string dt = Http.Get("GetCha?join=" + JsonConvert.SerializeObject(model));
            string gong = Http1.Get("Chacha?join=" + JsonConvert.SerializeObject(model));
            
           ViewBag.shu = JsonConvert.DeserializeObject<DataTable>(dt);
            ViewBag.gong= JsonConvert.DeserializeObject<DataTable>(gong);
            return View();
        }
        /// <summary>
        /// 显示，需要用户邮箱和密码
        /// 个人信息
        /// </summary>
        /// <returns></returns>
        public ActionResult Chaxi()
        {
            UserInfo model = new UserInfo();
            model.UserEmail = "";
            model.UserPwd = "";
            string dt = Http2.Get("GetUserXin?join=" + JsonConvert.SerializeObject(model));
            DataTable data = JsonConvert.DeserializeObject<DataTable>(dt);
            return View(data);
        }
        /// <summary>
        /// 添加用户的信息
        /// 额度评估/审批
        /// </summary>
        /// <returns></returns>
        public ActionResult Chayong()
        {
            return View();
        }
        [HttpPost]
        public void Chayong(UserInfo mdoel)
        {
            UserInfo user = new UserInfo();
            user = mdoel;
            string dt = Http3.Post("PostUserdetailed" ,JsonConvert.SerializeObject(mdoel));
        }
    }
}