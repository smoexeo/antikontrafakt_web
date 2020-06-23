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
    public class AdminGetComplainsController : ApiController
    {
        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        public object Get(string token, int count,int page)
        {
            /*проверка токена*/
            DBDataContext db = new DBDataContext();

            var admins = (from re in db.UserAdmins where re.Token == token select re).ToList();

            if (admins.Count != 0)
            {
                /*админ есть, пускаем формирование списка*/
                 //var 
                

            }


            return "value";
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