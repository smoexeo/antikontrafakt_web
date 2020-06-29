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
    public class GetUserDataController : ApiController
    {
        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        public object Get(string token)
        {
            
            if (token == null)
                return null;

            DBDataContext db = new DBDataContext();

            var result = (from re in db.Users where re.UserToken == token select new UserInfo() { FIO = re.FIO, Email = re.Email, Phone = re.Phone }).ToList();

            if (result.Count != 0)
            {
                return result[0];
            }

            return null;
        }

        // POST api/<controller>
        public object Post([FromBody]Newtonsoft.Json.Linq.JToken value)
        {
            Token token = JsonConvert.DeserializeObject<Token>(value.ToString());
            if (token == null)
                return null;

            DBDataContext db = new DBDataContext();

            var result = (from re in db.Users where re.UserToken == token.token select new UserInfo() {FIO=re.FIO, Email=re.Email, Phone=re.Phone}).ToList();

            if (result.Count!=0)
            {
                return result[0];
            }

            return null;

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