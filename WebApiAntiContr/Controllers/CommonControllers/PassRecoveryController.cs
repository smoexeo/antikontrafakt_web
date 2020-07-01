using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using AntiContr_Lib;
using System.Text.RegularExpressions;
using DBContext;
using System.Net.Mail;
namespace WebApiAntiContr.Controllers.CommonControllers
{
    public class PassRecoverySendCodeController : ApiController
    {
        static List<KeyValuePair<int, string>> keys = new List<KeyValuePair<int, string>>();
        // GET api/<controller>/5
        public object Get(string email)
        {
            Regex r = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            string verificationcode = "";
            Random rand = new Random();
            verificationcode = rand.Next(10000000, 99999999).ToString();
            if (email != "" && r.IsMatch(email))
            {

                DBDataContext dBDataContext;
                try
                {
                    dBDataContext = new DBDataContext();

                    var users = (from re in dBDataContext.Users where re.Email == email select re).ToList();

                    if (users.Count != 0)
                    {
                        return new SuccessMess() { success = false, reason = "Почта уже занята." };
                        //users[0].UserHesh = verificationcode;vv
                        //users[0].UserToken = Guid.NewGuid().ToString();
                    }
                    else
                    {
                        if (SendCode(email, verificationcode))
                        {
                            string token = Guid.NewGuid().ToString();
                            var tokens = (from re in dBDataContext.Users select re.UserToken).ToList<string>();
                            tokens.AddRange((from re in dBDataContext.UserAdmins select re.Token).ToList());
                            while ((from re in tokens where token == re select re).ToList().Count != 0)
                            {
                                token = Guid.NewGuid().ToString();
                            }

                            dBDataContext.Users.InsertOnSubmit(new User()
                            {
                                Email = email,
                                UserToken = token,
                                UserHesh = verificationcode,
                                Phone = "",
                                FIO = ""

                            });
                            dBDataContext.SubmitChanges();
                        }
                        else
                        {
                            return new SuccessMess() { success = false, reason = "Почта содержит русские символы." };
                        }


                    }

                    return new SuccessMess() { success = true, reason = "Регистрация прошла успешно" };
                }
                catch
                {
                    return new SuccessMess() { success = false, reason = "Ошибка регистрации." };
                }



            }
            else
            {
                return new SuccessMess() { success = false, reason = "Почта имеет неверный фомат." };
            }
            
        }
        private bool SendCode(string email, string verificationcode)
        {
            try
            {
                Random rand1 = new Random();
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("antickontrafakt@yandex.ru");
                mail.To.Add(new MailAddress(email));
                mail.Subject = "Верификация пользователя АнтиКонтрафакт";
                mail.Body = "Код верификации пользователя: " + verificationcode + ". \n\nЕсли вы считаете, что данное письмо пришло вам по ошибке - просто проигнорируйте его.";
                SmtpClient client = new SmtpClient();
                client.Host = "smtp.yandex.ru";
                client.Port = 587;
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential("antickontrafakt@yandex.ru", "123456qwerty");
                client.Send(mail);
            }
            catch
            {
                return false;
            }
            return true;
        }
        // POST api/<controller>
       
    }
}