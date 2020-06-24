using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using AntiContr_Lib;
using DBContext;

namespace WebApiAntiContr.Controllers
{
    public class IsTryTokenController : ApiController
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
        public object Post([FromBody]Newtonsoft.Json.Linq.JToken value)
        {
            Token token = JsonConvert.DeserializeObject<Token>(value.ToString());
            if (token == null)
                return false;
            DBDataContext db = new DBDataContext();
            var resultuser = (from re in db.Users where re.UserToken == token.token select re).ToList();
            var resultadmin = (from re in db.UserAdmins where re.Token == token.token select re).ToList();
            TypeUser typeUser = TypeUser.None;

            if (resultuser.Count != 0) typeUser = TypeUser.User;
            if (resultadmin.Count != 0) typeUser = TypeUser.Admin;

            return typeUser;
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