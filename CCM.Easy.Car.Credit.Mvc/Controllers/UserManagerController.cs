using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mail;
using System.Web.Mvc;
using CCM.Easy.Car.Credit.DataModels;
using Newtonsoft.Json;

namespace CCM.Easy.Car.Credit.Mvc.Controllers
{
    public class UserManagerController : Controller
    {
        
        HttpClientHelper client = new HttpClientHelper("https://localhost:44386/api/UserManager/");
        
        // GET: UserManager
        /// <summary>
        /// 陈彤彤和顾烯墰注册页面 12.18
        /// </summary>
        /// <returns></returns>
        public ActionResult Register()
        {
            return View();
        }
        /// 陈彤彤和顾烯墰注册页面 12.18（提示注册成功）
        [HttpPost]
        public ActionResult Register(UserInfo model)
        {
            string json = client.Post("PostRegister", JsonConvert.SerializeObject(model));
            int n = Convert.ToInt32(json);
            if (n>0)
            {
                Response.Write("<script>alert('注册成功');location.href='/UserManager/UserLogin'</script>");
            }
            return View();
        }
        #region 普通话登录、邮箱登陆
        
        
        /// <summary>
        ///  陈彤彤和顾烯墰登录页面 12.18（普通登录）
        /// </summary>
        /// <returns></returns>
        public ActionResult UserLogin()
        {
            UserInfo users = new UserInfo();

            //判断当前cookie是否存在用户名和密码，若存在则传给前台，直接赋值

            HttpCookie cookie = System.Web.HttpContext.Current.Request.Cookies.Get("COOKIE_NAME_FOR_USER");
            users.UserEmail = (cookie == null ? "" : cookie["COOKIE_USER_NAME"].ToString().Trim());
            users.UserPwd = (cookie == null ? "" : cookie["COOKIE_USER_PASS"].ToString().Trim());
            if (cookie != null)
            {
                ViewData["checked"] = "checked";
            }
            else
            {
                ViewData["checked"] = "";
            }
            return View(users);
        }
        public ActionResult Check()
        {
            List<UserInfo> list = new List<UserInfo>();
            string messge;
            var remeber = Request.Form["Isre"];
            string LoginName = Request.Form["name"];
            string Password = Request.Form["password"];
            var user = (from u1 in list
                        where u1.UserEmail == LoginName && u1.UserPwd == Password
                        select u1).FirstOrDefault();
            if (remeber == "true")
            {

                //如果选中记住密码则把用户名和密码存入cookie否则存为空

                HttpCookie cookie = new HttpCookie("COOKIE_NAME_FOR_USER");

                cookie.Expires = DateTime.Now.AddYears(1);
                cookie["COOKIE_USER_NAME"] = LoginName;
                cookie["COOKIE_USER_PASS"] = Password;
                System.Web.HttpContext.Current.Response.Cookies.Add(cookie);
            }
            else
            {
                HttpCookie cookie = new HttpCookie("COOKIE_NAME_FOR_USER");
                cookie.Expires = DateTime.Now.AddYears(-1);
                Request.Cookies.Add(cookie);
                cookie["COOKIE_USER_NAME"] = null;
                cookie["COOKIE_USER_PASS"] = null;
                System.Web.HttpContext.Current.Response.Cookies.Add(cookie);
            }
            if (user == null)
            {
                messge = "error";
            }
            else
            {
                messge = "ok";
                HttpContext.Session["Name"] = LoginName;
            }
            return Content(messge);
        }
        /// <summary>
        /// 陈彤彤和顾烯墰登录页面 12.18（提示登录成功）（普通登录）
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UserLogin(UserInfo model)
        {
            Session["UserEmail"] = model.UserEmail;
            string json = client.Get("GetUserLogin?json=" + JsonConvert.SerializeObject(model));
            int n = Convert.ToInt32(json);
            if (n>0)
            {
                Response.Write("<script>alert('登录成功！');location.href='/Homepage/Index'</script>");
            }
            return View();
        }
        /// <summary>
        ///  陈彤彤和顾烯墰登录页面 12.18（邮箱验证登录）
        /// </summary>
        /// <returns></returns>
        public ActionResult EmailLogin()
        {
            Session["UserEmail"] = "";
            return View();
        }
        /// <summary>
        ///  陈彤彤和顾烯墰登录页面 12.18（邮箱验证登录）
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EmailLogin(UserInfo model)
        {
            return View();
        }
        public int EmailDenglu(string yzm)
        {
            
            if (Session["yzm"]!=null)
            {
                if (yzm.ToUpper()== Session["yzm"].ToString())
                {

                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }
        /// <summary>
        ///  陈彤彤和顾烯墰登录页面 12.18（邮箱验证登录）
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Email(UserInfo model)
        {
            string json = client.Get("GetUserLoginRe?json="+JsonConvert.SerializeObject(model));
            Session["UserEmail"] = model.UserEmail;
            return 1;
        }
        /// <summary>
        /// 验证邮箱
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns>
        public int MailEmail(string txt)
        {
            
            UserInfo info = new UserInfo()
            {
                UserEmail = txt
            };
            string json = client.Put("PutUserLoginRe", JsonConvert.SerializeObject(info));
            if (json!="")
            {
                Session["UserEmail"] = info.UserEmail;
                return  Mail1(txt);
            }
            else
            {
                return 0;
            }
        }
        /// <summary>
        /// 陈彤彤和顾烯墰登录页面 12.18（邮箱验证登录）
        /// </summary>
        ///<returns></returns>
        public int Mail1(string Txt)
        {
            int n = 1;
            string yanzheng = string.Empty;
            System.Net.Mail.MailMessage mailMessage = new System.Net.Mail.MailMessage();
            //发件人邮箱地址，方法重载不同，可以根据需求自行选择。
            mailMessage.From = new MailAddress("yjl2240998120@163.com");
            //收件人邮箱地址。
            mailMessage.To.Add(new MailAddress(Txt));
            //邮件标题。
            mailMessage.Subject = "这是你的验证码";
            string verificationcode = createrandom(6);
            yanzheng = verificationcode;
            //邮件内容。
            mailMessage.Body = "你的验证码是" + verificationcode;
            //实例化一个SmtpClient类。
            SmtpClient client = new SmtpClient();
            //在这里我使用的是qq邮箱，所以是smtp.qq.com，如果你使用的是126邮箱，那么就是smtp.126.com。
            client.Host = "smtp.163.com";
            //使用安全加密连接。
            client.EnableSsl = true;
            //不和请求一块发送。
            client.UseDefaultCredentials = false;
            //验证发件人身份(发件人的邮箱，邮箱里的生成授权码);
            client.Credentials = new NetworkCredential("yjl2240998120@163.com", "jinli123");
            Session["yzm"] = yanzheng;
            //发送

            client.Send(mailMessage);
            return n;

        }
        //生成6位数字和大写字母的验证码
        private string createrandom(int codelengh)
        {
            int rep = 0;
            string str = string.Empty;
            long num2 = DateTime.Now.Ticks + rep;
            rep++;
            Random random = new Random(((int)(((ulong)num2) & 0xffffffffL)) | ((int)(num2 >> rep)));
            for (int i = 0; i < codelengh; i++)
            {
                char ch;
                int num = random.Next();
                if ((num % 2) == 0)
                {
                    ch = (char)(0x30 + ((ushort)(num % 10)));
                }
                else
                {
                    ch = (char)(0x41 + ((ushort)(num % 0x1a)));
                }
                str = str + ch.ToString();
            }
            return str;
        }
        #endregion
    }
}