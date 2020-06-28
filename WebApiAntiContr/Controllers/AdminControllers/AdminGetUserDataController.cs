using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using AntiContr_Lib;
using DBContext;

namespace WebApiAntiContr.Controllers.AdminControllers
{
    public class AdminGetUserDataController : ApiController
    {
        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        public object Get(string token,int id)
        {
            if (token == null)
                return null;

            DBDataContext db = new DBDataContext();
            var admins = (from re in db.UserAdmins where token == re.Token select re).ToList();
            var result = (from re in db.Requests where re.Id== id select re).ToList();

            if (result.Count != 0 && admins.Count!=0)
            {
                return new UserInfo() {Email = result[0].User.Email,Phone=result[0].User.Phone,FIO=result[0].User.FIO};
            }

            return null;
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