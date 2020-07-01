using System;
using System.Collections;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using System.Text;
using AntiContr_Lib;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using DBContext;
using System.Net.Mail;
using System.Collections.Specialized;

namespace WebApiAntiContr.Controllers.CommonControllers
{
    public class PassRecoverySendCodeController : ApiController
    {
        
        // GET api/<controller>/5
        public object Get(string email)
        {
            Regex r = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");

            if (email != "" && r.IsMatch(email))
            {
                var db = new DBDataContext();
                var users = (from re in db.Users where email == re.Email select re).ToList();

                if (users.Count != 0)
                {

                    using (MD5 md5Hash = MD5.Create())
                    {
                        DateTime now = DateTime.Now;
                        string hash = GetMd5Hash(md5Hash, now.ToString("dd-MM-yy-hh")+users[0].UserHesh);

                        bool result = SendCode(email, hash);
                        if (result)
                            return new SuccessMess { success = result, reason = "На почту было отправленно письмо для восстановления доступа." };
                        else
                            return new SuccessMess { success = result, reason = "Произошла ошибка. Попробучте отправить писмо еще раз." };
                    }
                }
                else
                {
                   return new SuccessMess() { success = false, reason = "Данная почта не зарегистрированна." };
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
                mail.Subject = "Восстановление доступа пользователя АнтиКонтрафакт";
                mail.Body = "Пройдите по ссылке и смените пароль: " + "flatren-001-site1.itempurl.com/Home/Recovery?email=" + email+"&hash="+ verificationcode + ". \nСсылка действует до конца текущего часа.\nЕсли вы считаете, что данное письмо пришло вам по ошибке - просто проигнорируйте его.";
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
        string GetMd5Hash(MD5 md5Hash, string input)
        {

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }
        bool VerifyMd5Hash(MD5 md5Hash, string input, string hash)
        {
            // Hash the input.
            string hashOfInput = GetMd5Hash(md5Hash, input);

            // Create a StringComparer an compare the hashes.
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            if (0 == comparer.Compare(hashOfInput, hash))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public object Post([FromBody]Newtonsoft.Json.Linq.JToken value)
        {
            var result = JsonConvert.DeserializeObject<SendMessRecovery>(value.ToString());
            var db = new DBDataContext();
            var users = (from re in db.Users where result.email == re.Email select re).ToList();

            if (users.Count != 0)
            {

                using (MD5 md5Hash = MD5.Create())
                {
                    DateTime now = DateTime.Now;
                    var bresult = VerifyMd5Hash(md5Hash, now.ToString("dd-MM-yy-hh") + users[0].UserHesh, result.hash);
                    if(bresult)
                        return new SuccessMess() { success = bresult, reason = users[0].Email};
                }
            }
            return new SuccessMess() {success = false, reason = "Ссылка устарела."};
        }
        // POST api/<controller>
       
    }
}