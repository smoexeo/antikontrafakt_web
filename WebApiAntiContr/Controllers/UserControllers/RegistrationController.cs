using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AntiContr_Lib;
using DBContext;
using Newtonsoft.Json;

namespace WebApiAntiContr.Controllers
{
    public class RegistrationController : ApiController
    {
        

        // GET api/<controller>/{}
        public SuccessMess Get(string email,string code,string pass)
        {

            ApiRegistration registration = new ApiRegistration() {email=email,code=code,pass=pass };
            if (registration == null)
                return new SuccessMess() { success = false, reason = "Код не действителен" };

            if (registration.pass.Length < 8)
            {
                return new SuccessMess() { success = false, reason = "Пароль меньше 8 символов" };
            }
            DBDataContext db = new DBDataContext();
            List<User> user = (from re in db.Users where re.Email == registration.email && re.UserHesh == registration.code select re).ToList();

            if (user.Count != 0)
            {
                user[0].UserHesh =Hash.GetMd5Hash( registration.pass+"-sol");
                db.SubmitChanges();
                return new SuccessMess() { success = true, reason = "Регистрация прошла успешно" };
            }

            return new SuccessMess() { success = false, reason = "Код не действителен" };
        }

        // POST api/<controller>
        public object Post([FromBody]Newtonsoft.Json.Linq.JToken value)
        {
            ApiRegistration registration = JsonConvert.DeserializeObject<ApiRegistration>(value.ToString());
            if (registration == null)
                return false;

            if (registration.pass.Length < 8)
            {
                return new SuccessMess() { success = false, reason = "Пароль меньше 8 символов" };
            }
            DBDataContext db = new DBDataContext();
            List<User> user = (from re in db.Users where re.Email == registration.email && re.UserHesh == registration.code select re).ToList();

            if (user.Count != 0)
            {
                user[0].UserHesh = Hash.GetMd5Hash(registration.pass + "-sol");
                db.SubmitChanges();
                return new SuccessMess() { success = true, reason = "Регистрация прошла успешно" };
            }

            return new SuccessMess() { success = false, reason = "Код не действителен" };
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