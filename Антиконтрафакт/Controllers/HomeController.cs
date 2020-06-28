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
    public class HomeController : Controller
    {
        private static HttpClient client = new HttpClient();

        private static string url = @"http://godnext-001-site1.btempurl.com/api/";
        //private static string url = @"http://localhost:51675/api/";

        #region Главная страница
        public ActionResult Index()
        {
            SetUserNameHeader();
            return View();
        }
        [HttpPost]
        public ActionResult Index(string Barcode, string Tin)
        {
            if (Barcode != null)
                return RedirectToAction("Barcode", "Home", new { barcode = Barcode });
            if (Tin != null)
                return RedirectToAction("Outlet", "Home", new { tin = Tin });

            return View();
        }
        #endregion

        #region Вывод по штрих-коду и ИНН
        [HttpGet]
        public ActionResult Outlet(string tin)
        {
            SetUserNameHeader();
            string sget = RequestGet(url + "Check_outlet", new List<KeyValuePair<string, string>>() { new KeyValuePair<string, string>("tin", tin) });
            ApiGetMessCheckTin checkTin = JsonConvert.DeserializeObject<ApiGetMessCheckTin>(sget);
            @ViewBag.Tin = tin;
            @ViewBag.Name = "Информация отсутсвует.";
            @ViewBag.Address = "Информация отсутсвует.";
            if (checkTin.result == "Указанный ИНН является подлинным.")
            {
                @ViewBag.Color = "green";
                @ViewBag.Good = "Да";
                @ViewBag.Name = checkTin.Name;
                @ViewBag.Address = checkTin.Address;
            }
            else
            {
                @ViewBag.Color = "red";
                @ViewBag.Good = "Нет";
            }


            return View();
        }

        [HttpGet]
        public ActionResult Barcode(string barcode)
        {
            SetUserNameHeader();
            string sget = RequestGet(url + "Check_barcode", new List<KeyValuePair<string, string>>() { new KeyValuePair<string, string>("barcode", barcode) });
            MessBarCode messBarCode = JsonConvert.DeserializeObject<MessBarCode>(sget);
            ViewBag.Barcode = barcode;
            ViewBag.Country = "Информация отсутсвует.";
            ViewBag.DopInfo = "Информация отсутсвует.";

            if (messBarCode.result == "Указанный товар не существует(не найден)")
            {
               @ViewBag.Color = "red";
                ViewBag.Good = "Да";
            }
            else
            {
                @ViewBag.Color = "green";
                ViewBag.Good = "Нет";
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
                if (JsonConvert.DeserializeObject<TypeUser>(result) != TypeUser.None)
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
            bool result = false;
            SetUserNameHeader();
            try
            {
              result = SetUserData();
            }
            catch
            { }
            
            if (result)
                return View();
            else
                return RedirectToAction("Authorization");
        }
        [HttpGet]
        public ActionResult Account(string page)
        {
            bool result = false;
            SetUserNameHeader();
            try
            {
                int i = 1;
                if (!int.TryParse(page, out i)) i = 1;
                
                result = SetUserData(i);
            }
            catch
            { }

            if (result)
                return View();
            else
                return RedirectToAction("Authorization");
        }
        [HttpPost]
        public ActionResult Account(string submitButton, string oldPass, string newPass, string new2Pass)
        {
            SetUserData();
            if (submitButton == "NewPass")
            {
                if (newPass != new2Pass)
                {
                    @ViewBag.ErrorPass = "Пароли не совпали.";
                    //return View();
                }
                else
                {
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
                        else
                        Request.Cookies.Set(cookie);
                    }
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
        {
            HttpCookie cookie = Request.Cookies["token"];
            if (cookie != null)
            {
                var values = new NameValueCollection();
                values.Add("token", cookie.Value);
                var result = RequestPost(url + "istrytoken", values);
                if (JsonConvert.DeserializeObject<TypeUser>(result) != TypeUser.None)
                {
                    Request.Cookies.Set(cookie);
                    return RedirectToAction("Account");
                }
            }
            return View();
        }

        [HttpPost]
        public ActionResult Registration(string submitButton, string Email, string Code, string Pass, string TwoPass)
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
            SetUserNameHeader();
            TypeToken typeToken = GetTypeUser();
            if (typeToken.typeUser == TypeUser.User)
            {
                SetUserDataInRequestsPage(typeToken,null);
            }
            else
            { RedirectToAction("Authorization"); }
            return View();
        }
        [HttpGet]
        public ActionResult RequestsPage(string id)
        {
            SetUserNameHeader();
            TypeToken typeToken = GetTypeUser();
            @ViewBag.TypeUser = typeToken.typeUser;
            if (typeToken.typeUser != TypeUser.None)
            {
                var idContainer = new List<KeyValuePair<string, string>>
                {
                        new KeyValuePair<string, string>( "id", id ),
                        new KeyValuePair<string, string>( "token", typeToken.token )
                };
                var result = RequestGet(url + "ShowRequest", idContainer);
                var resRequest = JsonConvert.DeserializeObject<RecordComplainFullInfo>(result);
                if (resRequest != null)  // если получили данные о заявлении
                    {
                        
                        ViewBag.Adress = resRequest.adress;         // заполняем поля заявления
                        ViewBag.Message = resRequest.textRequest;
                        ViewBag.Unit = resRequest.unit;
                        ViewBag.Type = resRequest.type;

                        ViewBag.RequestId = id;

                    if (typeToken.typeUser == TypeUser.User)
                    {
                        bool isDisabled = false;    // отключаем поля в зависимости от статуса заявления
                        if (resRequest.status == "В рассмотрении" || resRequest.status == "Архивирована" || resRequest.status == "Отправлено в Роспотребнадзор")
                        {
                            isDisabled = true;
                        }

                        ViewBag.SaveDisabled = isDisabled;
                        ViewBag.PostDisabled = isDisabled;
                        ViewBag.DeleteDisabled = isDisabled;

                        ViewBag.ReadOnly = isDisabled;
                        ViewBag.Disabled = isDisabled;
                    }
                    if (typeToken.typeUser == TypeUser.Admin)
                    {
                        if (resRequest.status == "Архивирована")
                        {
                            @ViewBag.ReturnArhivvalue = "adminrecover";
                            @ViewBag.ReturnArhiv = "Восстановить";
                        }
                        else
                        {
                            @ViewBag.ReturnArhivvalue = "adminarchive";
                            @ViewBag.ReturnArhiv = "Архивировать";
                        }
                        bool isDisabled = true;    // отключаем поля в зависимости от статуса заявления
                        if (resRequest.status == "В рассмотрении")
                        {
                            isDisabled = false;
                        }

                        ViewBag.SaveDisabled = isDisabled;
                        ViewBag.PostDisabled = isDisabled;
                        ViewBag.DeleteDisabled = false;
                        if (resRequest.status == "Черновик")
                        {
                            ViewBag.DeleteDisabled = isDisabled;
                        }
                       

                        ViewBag.ReadOnly = true;
                        ViewBag.Disabled = true;
                    }
                }
                if (!SetUserDataInRequestsPage(typeToken, id))
                {
                    return RedirectToAction("Account");
                }
                
            }
            else
            { return RedirectToAction("Authorization"); }

            return View();
        }
        // просмотр выбранного заявления - надо передать id заявления
        // если id пустой или null, то создание нового заявления
        //[HttpGet]
        //public ActionResult RequestsPage(string id)
        //{
           
        //    HttpCookie tokenCookie = Request.Cookies["token"];
        //    if (tokenCookie != null)
        //    {
        //        var tvalues = new NameValueCollection();
        //        tvalues.Add("token", tokenCookie.Value);
        //        //тут проверяем токен
        //        var json = RequestPost(url + "istrytoken", tvalues);
        //        var tresult = JsonConvert.DeserializeObject<TypeUser>(json);
        //        if (tresult != TypeUser.None)
        //        {
        //            if (tresult == TypeUser.User)
        //            {
        //                var values = new NameValueCollection { { "token", tokenCookie.Value } };
        //                var result = RequestPost(url + "GetUserData", values);

        //                var userInfo = JsonConvert.DeserializeObject<UserInfo>(result);

        //                if (userInfo != null)   // заполняем поля пользовательских данных
        //                {
        //                    Request.Cookies.Set(tokenCookie);

        //                    if (!string.IsNullOrEmpty(userInfo.FIO))
        //                    {
        //                        var words = userInfo.FIO.Split(' ');
        //                        ViewBag.Surname = words[0];
        //                        ViewBag.Firstname = words[1];

        //                        if (words.Length == 3)
        //                            ViewBag.Patronymic = words[2];
        //                    }

        //                    ViewBag.Email = userInfo.Email;
        //                    ViewBag.Phone = userInfo.Phone;
        //                }
        //                else    // иначе просим авторизоваться - не получили данных с сервера
        //                {
        //                    return RedirectToAction("Authorization");
        //                }
        //            }

                
        //            if (!string.IsNullOrEmpty(id))  // если получили какой-то id, значит надо посмотреть на уже созданную запись
        //            {
        //                var idContainer = new List<KeyValuePair<string, string>>
        //                { new KeyValuePair<string, string>( "id", id ),
        //                new KeyValuePair<string, string>( "token", tokenCookie.Value )};
        //                var result = RequestGet(url + "ShowRequest", idContainer);

        //                var resRequest = JsonConvert.DeserializeObject<RecordComplainFullInfo>(result);

        //                if (resRequest != null)  // если получили данные о заявлении
        //                {
        //                    ViewBag.Adress = resRequest.adress;         // заполняем поля заявления
        //                    ViewBag.Message = resRequest.textRequest;
        //                    ViewBag.Unit = resRequest.unit;
        //                    ViewBag.Type = resRequest.type;

        //                    ViewBag.RequestId = id;

        //                    bool isDisabled = false;    // отключаем поля в зависимости от статуса заявления
        //                    if (resRequest.status == "В рассмотрении" || resRequest.status == "Архивирована")
        //                    {
        //                        isDisabled = true;
        //                    }

        //                    ViewBag.SaveDisabled = isDisabled;
        //                    ViewBag.PostDisabled = isDisabled;
        //                    ViewBag.DeleteDisabled = isDisabled;

        //                    ViewBag.ReadOnly = isDisabled;
        //                    ViewBag.Disabled = isDisabled;
        //                }
        //            }
        //            else  // не получили id - значит создаем новое заявление - надо отключить кнопку удаления
        //            {
        //                ViewBag.SaveDisabled = false;
        //                ViewBag.PostDisabled = false;
        //                ViewBag.DeleteDisabled = true;

        //                ViewBag.ReadOnly = false;
        //                ViewBag.Disabled = false;
        //            }
        //        }
        //        else
        //        {
        //            return RedirectToAction("Authorization");
        //        }
        //    }

        //    return View();
        //}

        // для отправки в бд
        [HttpPost]
        public ActionResult RequestsPage(string btn, string surname, string firstname, string patronymic,
            string mailAdress, string phoneNumber, string adress, string unit, string requestType, string message, string requestId)
        {
           
            TypeToken typeToken = GetTypeUser();
            @ViewBag.TypeUser = typeToken.typeUser;
            if(typeToken.typeUser==TypeUser.None)
                return RedirectToAction("Authorization");
            SetUserNameHeader();
            if (typeToken.typeUser == TypeUser.User)
            {
                string fio = surname + " " + firstname + " " + patronymic;
                fio = fio.Trim(); // убрать пробел, если не было отчества

                // отправляем данные пользователя на сервер, чтобы они обновились, если были изменены
                var userValues = new NameValueCollection
                {
                    { "fio",  fio},
                    { "email", mailAdress },
                    { "phone", phoneNumber },
                    { "token", typeToken.token }
                };
                var resultPost = RequestPost(url + "UpsertUserData", userValues);
                SuccessMess mess = JsonConvert.DeserializeObject<SuccessMess>(resultPost);

                // в зависимости от нажатой кнопки, формируем статус заявления
                string status = "В рассмотрении";
                switch (btn)
                {
                    case "postform": status = "В рассмотрении"; break;
                    case "save": status = "Черновик"; break;
                    case "archive": status = "Архивирована"; break;
                }
            
                // отправляем данные заявления на сервер
                var requestValues = new NameValueCollection
                {
                    { "token", typeToken.token },
                    { "text_request", message},
                    { "adress", adress },
                    { "unit", unit },
                    { "type", requestType },
                    { "status", status },
                    { "id", requestId }
                };
                resultPost = RequestPost(url + "Complain_product", requestValues);
                mess = JsonConvert.DeserializeObject<SuccessMess>(resultPost);
            }
            if (typeToken.typeUser == TypeUser.Admin)
            {
                switch (btn)
                {
                    case "adminpostform":
                        RequestGet(url + "AdminNewStatus",
                            new List<KeyValuePair<string, string>>()
                            {
                                new KeyValuePair<string, string>("token",typeToken.token),
                                new KeyValuePair<string, string>("id",requestId),
                                new KeyValuePair<string, string>("status","Отправлено в Роспотребнадзор")
                            });
                        break;
                    case "adminrecover":
                        RequestGet(url + "AdminNewStatus",
                           new List<KeyValuePair<string, string>>()
                           {
                                new KeyValuePair<string, string>("token",typeToken.token),
                                new KeyValuePair<string, string>("id",requestId),
                                new KeyValuePair<string, string>("status","Черновик")
                           });
                        break;
                    case "adminarchive":
                        RequestGet(url + "AdminNewStatus",
                            new List<KeyValuePair<string, string>>()
                            {
                                new KeyValuePair<string, string>("token",typeToken.token),
                                new KeyValuePair<string, string>("id",requestId),
                                new KeyValuePair<string, string>("status","Архивирована")
                            });
                        break;
                    case "adminedit":
                        RequestGet(url + "AdminNewStatus",
                            new List<KeyValuePair<string, string>>()
                            {
                                new KeyValuePair<string, string>("token",typeToken.token),
                                new KeyValuePair<string, string>("id",requestId),
                                new KeyValuePair<string, string>("status","Черновик")
                            });
                        break;
                    default:
                        break;
                }
            }
               

            

            return RedirectToAction("Account");
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

        #region Вспомогательные функции
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }

        void SetUserNameHeader()
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
                    if (!string.IsNullOrEmpty(userInfo.FIO))
                        @ViewBag.UserName =(userInfo.FIO).Split(' ')[1];
                    
                }
            }
        }
        //на странице жалобы устанавливает данные пользователя или заявителя, так же позволяет заполнять все для администратора
        bool SetUserDataInRequestsPage(TypeToken typeToken, string id)
        {
            UserInfo userInfo=null;
            if (typeToken.typeUser == TypeUser.User)
            {
                var values = new NameValueCollection { { "token", typeToken.token } };
                var result = RequestPost(url + "GetUserData", values);
                 userInfo = JsonConvert.DeserializeObject<UserInfo>(result);
                
            }
            if (typeToken.typeUser == TypeUser.Admin)
            {
                if (id == null)
                { return false; }
                var json = RequestGet(url + "AdminGetUserData", new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("token",typeToken.token),
                    new KeyValuePair<string, string>("id",id),
                });
                userInfo = JsonConvert.DeserializeObject<UserInfo>(json);

            }
            if (userInfo != null)   // заполняем поля пользовательских данных
                {
                    if (!string.IsNullOrEmpty(userInfo.FIO))
                    {
                        var words = userInfo.FIO.Split(' ');
                        ViewBag.Surname = words[0];
                        ViewBag.Firstname = words[1];

                        if (words.Length == 3)
                            ViewBag.Patronymic = words[2];
                    }
                    ViewBag.Email = userInfo.Email;
                    ViewBag.Phone = userInfo.Phone;
                }
            return true;
        }
        
        //перевод кодировок из utf8 в win1251
        string Utf8ToWin1251(byte[] utf8Bytes)
        {
            Encoding utf8 = Encoding.GetEncoding("UTF-8");
            Encoding win1251 = Encoding.GetEncoding("Windows-1251");
            byte[] win1251Bytes = Encoding.Convert(utf8, win1251, utf8Bytes);
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
        string RequestGet(string url, List<KeyValuePair<string, string>> values)
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

        void TableComplain(int i,TypeUser typeUser,string token,int count)
        {
            string json="";
            if (typeUser == TypeUser.User)
            {
                json = RequestGet(url + "UserGetComplains", new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("token",token),
                    new KeyValuePair<string, string>("count",count.ToString()),
                    new KeyValuePair<string, string>("page",i.ToString())
                });
                
            }
            if (typeUser == TypeUser.Admin)
            {
                json = RequestGet(url + "AdminGetComplains", new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("token",token),
                    new KeyValuePair<string, string>("count",count.ToString()),
                    new KeyValuePair<string, string>("page",i.ToString())
                });
            }
            List<RecordComlains> records = JsonConvert.DeserializeObject<List<RecordComlains>>(json);
            
            ViewBag.datas = records;
        }
        
        //заполняет данными личный кабинет пользователя
        bool SetUserData(int currPage=1)
        {
            HttpCookie cookie = Request.Cookies["token"];
            bool resultreturn = false;
            if (cookie != null)
            {
                var values = new NameValueCollection();
                values.Add("token", cookie.Value);
                var json = RequestPost(url + "istrytoken", values);
                var result = JsonConvert.DeserializeObject<TypeUser>(json);
                if (result != TypeUser.None)
                {
                    var minInfoRecords_json = RequestGet(url + "GetMinInfoRecords", new List<KeyValuePair<string, string>>() { new KeyValuePair<string, string>("token", cookie.Value) });
                    var minInfoRecords = JsonConvert.DeserializeObject<ApiGetMinInfoRecords>(minInfoRecords_json);
                    @ViewBag.ShowInfo = minInfoRecords.show;
                    @ViewBag.NotShowInfo = minInfoRecords.notshow;
                    @ViewBag.SendInfo = minInfoRecords.arhiv;
                    @ViewBag.DraftInfo = minInfoRecords.draft;
                    int sum = minInfoRecords.show + minInfoRecords.notshow + minInfoRecords.arhiv + minInfoRecords.draft;
                    int count = 16;//заявок на странице
                    int page = sum / count + 1;//количество страниц
                    if (!(currPage <= page && currPage >= 1)) currPage = 1;
                    var array = new List<int>();
                    int j = currPage - 2;
                    for (int i = 0,c=0; i < 5; i++)
                    {
                        c = j + i;
                        if (c >= 1 && c<=page) array.Add(c);
                    }
                    @ViewBag.EndPage = page;
                    @ViewBag.Array = array;
                    TableComplain(currPage, result, cookie.Value,count);
                    @ViewBag.TypeUser = result;
                    resultreturn = true;
                }
            }
            Request.Cookies.Set(cookie);
            return resultreturn;
        }

        //Проверяет тип пользователя по токену и возвращает токен с типом.
        TypeToken GetTypeUser()
        {
            HttpCookie tokenCookie = Request.Cookies["token"];
            if (tokenCookie != null)
            {
                var tvalues = new NameValueCollection();
                tvalues.Add("token", tokenCookie.Value);
                var json = RequestPost(url + "istrytoken", tvalues);
                var tresult = JsonConvert.DeserializeObject<TypeUser>(json);
                Request.Cookies.Set(tokenCookie);
                return new TypeToken() { typeUser = tresult, token = tokenCookie.Value };
                
            }
            return new TypeToken() { typeUser = TypeUser.None};
        }
        #endregion

    }
}