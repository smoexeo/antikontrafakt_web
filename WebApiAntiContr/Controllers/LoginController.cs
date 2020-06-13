using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using AntiContr_Lib;
using DBContext;
using System.Text;
using System.Security.Cryptography;
namespace WebApiAntiContr.Controllers
{
    public class LoginController : ApiController
    {
        // GET api/<controller>/5
        public object Get(string email,string code)
        {
            DBDataContext db = new DBDataContext();
            
            List<User> user = (from re in db.Users where re.Email == email && re.UserHesh == code select re).ToList();

            if (user.Count != 0)
            {
                return user[0].UserToken;
            }

            return new Error() {error = 0, description = "Не найден пользователь" };
        }

        // POST api/<controller>
        public object Post([FromBody]string email, [FromBody]string code)
        {
            DBDataContext db = new DBDataContext();

            List<User> user = (from re in db.Users where re.Email == email && re.UserHesh == code select re).ToList();

            if (user.Count != 0)
            {
                return user[0].UserToken;
            }

            return new Error() { error = 0, description = "Не найден пользователь" };
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