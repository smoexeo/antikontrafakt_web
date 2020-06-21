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
        public object Get(string email, string pass)
        {
            DBDataContext db = new DBDataContext();

            List<User> user = (from re in db.Users where re.Email == email && re.UserHesh == pass select re).ToList();
            Token token = new Token();
            if (user.Count != 0)
            {
                token.token = user[0].UserToken;
            }
            else
            {
                token.token = "";
            }

            return token;
        }

        // POST api/<controller>

        public object Post([FromBody]Newtonsoft.Json.Linq.JToken value)
        {
            ApiLogin apiLogin=JsonConvert.DeserializeObject<ApiLogin>(value.ToString());

            if(apiLogin==null)
            {
                return "";
            }

            DBDataContext db = new DBDataContext();

            List<User> user = (from re in db.Users where re.Email == apiLogin.email && re.UserHesh == apiLogin.code select re).ToList();

            if (user.Count != 0)
            {
                return user[0].UserToken;
            }

            return "";
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