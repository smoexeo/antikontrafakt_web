using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Dadata;
using Dadata.Model;
using System.Net.Mail;
using System.Net;
using System.Text.RegularExpressions;
using DBA612DFFlatrenContext;

namespace Антиконтрафакт2._1.Controllers
{
    public class HomeController : Controller
    {
        static string Message;static string ActionRedir;
        string email="";
        static string verificationcode="";
        static AntiKEntities1 db=new AntiKEntities1();
        string token = "1cbadd71c5ffaabbd82f45db0f4784dde59648ad";
        string bartoken = "73cb0766-2aad-4ff5-90f1-80845146bd6a";
        string secret = "15d77fff9d8d4bab683a091007fea7fe5629cb6f";
        List<string> forminfo;
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Info()
        {
            return View();
        }
        public ActionResult Barcode_LookUp()
        {
            return View();
        }
        public ActionResult CheckSeller()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CheckSeller(string INNcode)
        {
                ViewBag.Error = null; ViewBag.Error = "";
                ViewBag.Name = null;
                ViewBag.Address = null;
                try
                {
                    var api = new SuggestClient(token);
                    var response = api.FindParty(INNcode);
                    var party = response.suggestions[0].data;
                    ViewBag.Name = party.name.full_with_opf;
                    ViewBag.Address = "г. " + party.address.data.city + ", " + party.address.data.city_district_with_type + "р-н, д." + party.address.data.house;
                }
                catch
                {
                Message = "Для произведения поиска необходимо ввести ИНН в поле для ввода.";
                ActionRedir = "CheckSeller";
                return RedirectToAction("UserMessage");
                }
            return View();
        }
        public ActionResult FindSeller()
        {
            return View();
        }
        public ActionResult SendForm()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SendForm(string SPName, string SPAddress, string SPINN, string SPReason, string PName, string PBar, string PReason)
        {
            forminfo = new List<string>();
            if ((SPName != ""&&SPAddress!=""&&SPINN!=""&&SPReason!="")&&(SPName != null && SPAddress != null && SPINN != null && SPReason != null))
            {
                forminfo.Add(SPName);forminfo.Add(SPAddress);forminfo.Add(SPINN);forminfo.Add(SPReason);
                Request r = new Request();
                string code;
                List<string> codes = (from re in db.Requests select re.request_code).ToList();
                Random rand = new Random();
                do
                {
                    code = rand.Next(100000,999999).ToString()+ rand.Next(100000, 999999).ToString();
                } while (codes.Contains(code));
                r.request_code = code;
                if (email != null && email != "")
                {
                    r.email = email;
                }
                r.request_text = SPReason;
                r.request_salepoint = SPName + SPAddress + SPINN;
                r.request_status = "Submited";
                db.Requests.Add(r);
                db.SaveChanges();
                ActionRedir = "Index";
                Message = "Заявка была успешно отправлено.";
                return RedirectToAction("UserMessage");
            }
             else if ((PName!=""&&PBar!=""&&PReason!="")&& (PName != null && PBar != null && PReason != null))
            {
                forminfo.Add(PName); forminfo.Add(PBar); forminfo.Add(PReason);
                Request r = new Request();
                string code;
                List<string> codes = (from re in db.Requests select re.request_code).ToList();
                Random rand = new Random();
                do
                {
                    code = rand.Next(100000, 999999).ToString() + rand.Next(100000, 999999).ToString();
                } while (codes.Contains(code));
                r.request_code = code;
                if (email != null && email != "")
                {
                    r.email = email;
                }
                r.request_text = PReason;
                r.request_product=PName+PBar;
                r.request_status = "Submited";
                db.Requests.Add(r);
                db.SaveChanges();
                ActionRedir = "Index";
                Message = "Заявка была успешно отправлено.";
                return RedirectToAction("UserMessage");
            }
            else
            {
                ActionRedir = "SendForm";
                Message = "Заполните все поля одной из двух форм и нажмите на соответствующую кнопку отправки.";
                return RedirectToAction("UserMessage");
            }
        }
        public ActionResult Validation()
        {
            email = ""; verificationcode = "";
            return View();
        }
        [HttpPost]
        public ActionResult Validation(string Emailad)
        {
            Regex r = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            email = ""; verificationcode = "";
            Random rand = new Random();
            verificationcode = rand.Next(100000, 999999).ToString();
            if (Emailad != ""&&r.IsMatch(Emailad))
            {
                Random rand1 = new Random();
                email = Emailad;
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("antickontrafakt@yandex.ru");
                mail.To.Add(new MailAddress(Emailad));
                mail.Subject = "Верификация пользователя АнтиКонтрафакт"+" - Запрос номер "+rand1.Next(10000,99999);
                mail.Body = "Запрос номер "+ rand1.Next(10000, 99999) +". Код верификации пользователя: " + verificationcode;
                SmtpClient client = new SmtpClient();
                client.Host = "smtp.yandex.ru";
                client.Port = 587;
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential("antickontrafakt@yandex.ru", "123456qwerty");
                client.Send(mail);
                return RedirectToAction("ValidationCodeCheck");
            }
            else
            {
                ActionRedir = "Validation";
                Message = "Введите свой адрес электронной почты в поле ввода и нажмите кнопку отправки.";
                return RedirectToAction("UserMessage");
            }

        }
        public ActionResult ValidationCodeCheck()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ValidationCodeCheck(string VerCode)
        {
            if (verificationcode == VerCode)
            {
                return RedirectToAction("SendForm");
            }
            else
            {
                email = ""; verificationcode = "";
                ActionRedir = "Validation";
                Message = "Введенный неверный код валидации пользователя.";
                return RedirectToAction("UserMessage");
            }
        } 
        public ActionResult UserMessage()
        {
            ViewBag.Message = null;
            ViewBag.Message = Message;
            return View();
        }
        [HttpPost]
        public ActionResult UserMessage(string PassVal)
        {
            return RedirectToAction(ActionRedir);
        }
    }
}