using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Net;
using System.IO;
using AntiContr_Lib;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Mvc;
using System.Collections.Specialized;
using Newtonsoft.Json;

namespace Антикотрафакт.Controllers
{
    public  class HomeController : Controller
    {
        private static HttpClient client = new HttpClient();

        //private static string url = @"http://godnext-001-site1.btempurl.com/api/";
        private static string url = @"http://localhost:51675/api/";

        #region Главная страница
            public ActionResult Index()
            {
                SetUserInfo();
                return View();
            }
            [HttpPost]
            public ActionResult Index(string Barcode,string Tin)
        { 
            if(Barcode!=null)
                return RedirectToAction("Barcode","Home",new { barcode = Barcode });
            if(Tin!=null)
                return RedirectToAction("Outlet", "Home", new { tin = Tin });

            return View();
        }
        #endregion

        #region Вывод по штрих-коду и ИНН
            [HttpGet]
            public ActionResult Outlet(string tin)
            {
                SetUserInfo();
                string sget = RequestGet(url + "Check_outlet", new List<KeyValuePair<string, string>>() { new KeyValuePair<string, string>("tin", tin)});
                ApiGetMessCheckTin checkTin = JsonConvert.DeserializeObject<ApiGetMessCheckTin>(sget);
                @ViewBag.Tin = tin;
                @ViewBag.Name = "Информация отсутсвует.";
                @ViewBag.Address = "Информация отсутсвует.";
                if (checkTin.result == "Указанный ИНН является подлинным.")
                {
                    @ViewBag.Good = "Да";
                    @ViewBag.Name = checkTin.Name;
                    @ViewBag.Address = checkTin.Address;
                }
                else
                {
                    @ViewBag.Good = "Нет";
                }
            

                return View();
            }

            [HttpGet]
            public ActionResult Barcode(string barcode)
            {
                SetUserInfo();
                string sget = RequestGet(url + "Check_barcode", new List<KeyValuePair<string, string>>() { new KeyValuePair<string, string>("barcode", barcode)});
                MessBarCode messBarCode = JsonConvert.DeserializeObject<MessBarCode>(sget);
                ViewBag.Barcode = barcode;
                ViewBag.Country = "Информация отсутсвует.";
                ViewBag.DopInfo = "Информация отсутсвует.";

                if (messBarCode.result == "Указанный товар не существует(не найден)")
                {
                    ViewBag.Good = "Да";
                }
                else
                {
                    ViewBag.Good =  "Нет";
                    ViewBag.Country = messBarCode.info.ToString().Replace("Cтрана производитель", "");
                }
            
            
                return View();
            }
        #endregion

        #region Авторизация
        public ActionResult Authorization()
        {
            HttpCookie cookie = Request.Cookies["token"];

            if (cookie != null)
            {
                var values = new NameValueCollection();
                values.Add("token", cookie.Value);
                var result = RequestPost(url + "istrytoken", values);
                if (JsonConvert.DeserializeObject<TypeUser>(result)!=TypeUser.None)
                {
                    Request.Cookies.Set(cookie);
                    return RedirectToAction("Account");
                }
            }

            return View();
        }

            [HttpPost]
            public ActionResult Authorization(string Email, string Password)
        {
            var values = new NameValueCollection();
            values.Add("email", Email);
            values.Add("code", Password);
            var res = JsonConvert.DeserializeObject<TypeToken>(RequestPost(url + "Login", values));
            if (res.typeUser == TypeUser.None)
            {
                @ViewBag.Name = "Неверная почта или пароль.";
                return View();
            }
            Response.Cookies.Add(new HttpCookie("token", res.token));
            return RedirectToAction("Index");
        }
        #endregion

        #region Личный кабинет
            public ActionResult Account()
        {
            HttpCookie cookie = Request.Cookies["token"];
            if (cookie != null)
            {
                var values = new NameValueCollection();
                values.Add("token", cookie.Value);
                var result = RequestPost(url + "istrytoken", values);
                if (JsonConvert.DeserializeObject<TypeUser>(result) == TypeUser.User)
                {
                    
                    Request.Cookies.Set(cookie);
                    SetUserInfo();
                    return View();
                }
                if (JsonConvert.DeserializeObject<TypeUser>(result) == TypeUser.Admin)
                {
                    Request.Cookies.Set(cookie);
                    return View();
                }
            }
            return RedirectToAction("Authorization");
        }
        [HttpPost]
        public ActionResult Account(string submitButton, string oldPass,string newPass,string new2Pass)
        {
            if (submitButton == "NewPass")
            {
                if (newPass != new2Pass)
                {
                    @ViewBag.ErrorPass = "Пароли не совпали.";
                    return View();
                }
                HttpCookie cookie = Request.Cookies["token"];
                if (cookie != null)
                {
                    var values = new List<KeyValuePair<string, string>>()
                    {
                        new KeyValuePair<string, string>("token", cookie.Value),
                        new KeyValuePair<string, string>("oldpass", oldPass),
                        new KeyValuePair<string, string>("newpass", newPass)
                    };
                    var result = RequestGet(url + "ChangePassword", values);
                    ApiChangePass pass = JsonConvert.DeserializeObject<ApiChangePass>(result);
                    @ViewBag.ErrorPass = pass.reason;
                    if (pass.success)
                    {
                        Response.Cookies.Add(new HttpCookie("token", pass.token));
                    }
                    //Request.Cookies.Set(cookie);
                }
            }
            if (submitButton == "Exit")
            {

                Response.Cookies.Add(new HttpCookie("token", ""));
                return RedirectToAction("Account");
            }
           return View();
        }
        #endregion

        #region Страница регистрации
            public ActionResult Registration()
            { return View(); }

            [HttpPost]
            public ActionResult Registration(string submitButton,string Email,string Code,string Pass,string TwoPass)
            {
                @ViewBag.Email = Email;
                if (submitButton == "postcode")
                {
                    var result_get = RequestGet(url + "Sign_up", new List<KeyValuePair<string, string>>() { new KeyValuePair<string, string>("email", Email) });
                    SuccessMess mess = JsonConvert.DeserializeObject<SuccessMess>(result_get);

                    if (mess.success)
                    {
                        @ViewBag.ReturnMess = "Код потверждения был отправлен на почту.";
                    }
                    else
                    { @ViewBag.ReturnMess = mess.reason; }
                }
                if (submitButton == "postform")
                {
                    ViewBag.Code = Code;
                    if (Pass != TwoPass)
                    {
                        @ViewBag.ReturnError = "Пароли не совпали.";
                        return View();
                    }
                
                    var result_get = RequestGet(url + "Registration", new List<KeyValuePair<string, string>>()
                    {
                        new KeyValuePair<string, string>("email", Email),
                        new KeyValuePair<string, string>("code", Code),
                        new KeyValuePair<string, string>("pass", Pass)
                    });
                    SuccessMess mess = JsonConvert.DeserializeObject<SuccessMess>(result_get);
                    @ViewBag.ReturnError = mess.reason;
               
                }
                return View();
            }
        #endregion

        #region Страница заявок

        public ActionResult RequestsPage()
        {
            HttpCookie tokenCookie = Request.Cookies["token"];
            if (tokenCookie != null)
            {
                var values = new NameValueCollection
                {
                    { "token", tokenCookie.Value }
                };
                var result = RequestPost(url + "GetUserData", values);

                UserInfo userInfo = JsonConvert.DeserializeObject<UserInfo>(result);

                if (userInfo != null)
                {
                    Request.Cookies.Set(tokenCookie);
                    var words = userInfo.FIO.Split(' ');
                    ViewBag.Surname = words[0];
                    ViewBag.Firstname = words[1];

                    if (words.Length == 3)
                        ViewBag.Patronymic = words[2];

                    ViewBag.Email = userInfo.Email;
                    ViewBag.Phone = userInfo.Phone;
                }
            }
            else
            {
                return RedirectToAction("Authorization");
            }

            return View(); 
        }

        [HttpPost]
        public ActionResult RequestsPage(string btn, string surname, string firstname, string patronymic, 
            string email, string phoneNumber, string adress, string unit, string requestType, string message)
        {
            HttpCookie tokenCookie = Request.Cookies["token"];
            
            if (tokenCookie == null)
                return RedirectToAction("Authorization");
            SetUserInfo();
            switch (btn)
            {
                case "postform":
                    {
                        string fio = surname + " " + firstname + " " + patronymic;
                        fio = fio.Trim(); // убрать пробел, если не было отчества

                        var userValues = new NameValueCollection
                        {
                            { "token", tokenCookie.Value },
                            { "fio",  fio},
                            { "phone", phoneNumber },
                            { "email", email }
                        };
                        var resultPost1 = RequestPost(url + "Complain_product/UpsertUserInfo", userValues);
                        SuccessMess mess1 = JsonConvert.DeserializeObject<SuccessMess>(resultPost1);
                        ViewBag.Mess += mess1.reason;

                        var requestValues = new NameValueCollection
                        {
                            { "token", tokenCookie.Value },
                            { "text_request", message},
                            { "adress", adress },
                            { "unit", unit },
                            { "type", requestType }
                        };

                        var resultPost = RequestPost(url + "Complain_product", requestValues);
                        SuccessMess mess = JsonConvert.DeserializeObject<SuccessMess>(resultPost);
                        ViewBag.Mess += " " + mess.reason;
                    }
                    break;
                case "cancel":
                    {
                        ;
                    }
                    break;
                case "save":
                    {
                        ;
                    }
                    break;
            }

            return View();
        }

        #endregion


        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult _Layout()
        {

            return View();
        }

       

       

        //перевод кодировок из utf8 в win1251
        string Utf8ToWin1251(byte[] utf8Bytes)
        {
            Encoding utf8 = Encoding.GetEncoding("UTF-8");
            Encoding win1251 = Encoding.GetEncoding("Windows-1251");
            byte[] win1251Bytes = Encoding.Convert(utf8,win1251,utf8Bytes);
            return win1251.GetString(win1251Bytes);
        }
      
        //отрправляет POST запрос с данными
        string RequestPost(string url, NameValueCollection values)
        {
            string result;
            using (var client = new WebClient())
            {
               // client.Headers.Add(HttpRequestHeader.ContentType, "text/html; charset=utf-8");
                var responseString = Encoding.UTF8.GetString(client.UploadValues(url, "POST", values));
                result = responseString;
            }
            return result;
        }
        //отрправляет GET запрос с данными
        string RequestGet(string url, List<KeyValuePair<string,string>> values)
        {
            string result;
            url = url + "?";
            foreach (var item in values)
            {
                url += item.Key + "=" + item.Value + "&";
            }
            using (var client = new WebClient())
            {
                client.Headers.Add(HttpRequestHeader.ContentType, "text/html; charset=utf-8");
                // var responseString = Utf8ToWin1251((Encoding.GetEncoding("Windows-1251")).GetBytes(client.DownloadString(url)));
                // result = responseString;
                result = client.DownloadString(url);
            }
            return result;
        }

        //устанавливает фамилию пользователя в хейдере
        void SetUserInfo()
        {
            HttpCookie cookie = Request.Cookies["token"];
            if (cookie != null)
            {
                var values = new NameValueCollection();
                values.Add("token", cookie.Value);
                var result = RequestPost(url + "GetUserData", values);
                UserInfo userInfo = JsonConvert.DeserializeObject<UserInfo>(result);
                if (userInfo != null)
                {
                    Request.Cookies.Set(cookie);
                    if (userInfo.FIO != null)
                        @ViewBag.UserName = userInfo.FIO.Split(' ')[0];
                    //return RedirectToAction("Index");
                }
            }
               
        }
           
        
    }
}