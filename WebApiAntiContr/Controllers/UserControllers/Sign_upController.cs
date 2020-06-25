using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Text.RegularExpressions;
using AntiContr_Lib;
using Newtonsoft.Json;
using DBContext;
using System.Net.Mail;


namespace WebApiAntiContr.Controllers
{
    public class Sign_upController : ApiController
    {
        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        public object Get(string email)
        {
            Regex r = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            
            string verificationcode = "";
            Random rand = new Random();
            verificationcode = rand.Next(100000, 999999).ToString();
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
                        //users[0].UserHesh = verificationcode;
                        //users[0].UserToken = Guid.NewGuid().ToString();
                    }
                    else
                    {
                        if (SendCode(email, verificationcode))
                        {
                            dBDataContext.Users.InsertOnSubmit(new User()
                            {
                                Email = email,
                                UserToken = Guid.NewGuid().ToString(),
                                UserHesh = verificationcode,
                                Phone = "79222415756",
                                FIO = "Папа Карло"                               

                            });
                            dBDataContext.SubmitChanges();
                        }
                        else
                        {
                            return new SuccessMess() { success = false, reason = "Почта содержит русские символы." };
                        }

                        
                    }
                    
                    return  new SuccessMess() {success =true,reason="Регистрация прошла успешно" };
                }
                catch 
                {
                    return  new SuccessMess() { success = false, reason = "Ошибка регистрации." };
                }
               
                

            }
            else
            {
                return new SuccessMess() { success = false, reason = "Почта имеет неверный фомат." };
            }
        }

        private bool SendCode(string email,string verificationcode)
        {
            try
            {
                Random rand1 = new Random();
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("antickontrafakt@yandex.ru");
                mail.To.Add(new MailAddress(email));
                mail.Subject = "Верификация пользователя АнтиКонтрафакт" + " - Запрос номер " + rand1.Next(10000, 99999);
                mail.Body = "Запрос номер " + rand1.Next(10000, 99999) + ". Код верификации пользователя: " + verificationcode;
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
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}