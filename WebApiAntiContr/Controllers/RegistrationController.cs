using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AntiContr_Lib;
using DBContext;

namespace WebApiAntiContr.Controllers
{
    public class RegistrationController : ApiController
    {
        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        public object Post([FromBody]string email, [FromBody]string code, [FromBody]string pass)
        {
          
            if (pass.Length < 8)
            {
                return new SuccessMess() { success = false, reason = "Пароль меньше 8 символов" };
            }
            DBDataContext db = new DBDataContext();
            List<User> user = (from re in db.Users where re.Email == email && re.UserHesh == code select re).ToList();

            if (user.Count != 0)
            {
                user[0].UserHesh = pass;
                db.SubmitChanges();
                return new SuccessMess() {success=true, reason = "Регистрация прошла успешно" };
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