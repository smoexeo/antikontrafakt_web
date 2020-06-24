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
    public class AdminNewStatusController : ApiController
    {
        // GET api/<controller>
        public object Get(string token,int id,string status)
        {
            DBDataContext db = new DBDataContext();
            var admins = (from re in db.UserAdmins where re.Token == token select re).ToList();
            if (admins.Count != 0)
            {
               
                var item = (from re in db.Requests where re.Id == id select re).ToList();
                if (item.Count != 0)
                {
                    item[0].Status = status;
                    db.SubmitChanges();
                    return new SuccessMess() { success = true, reason = "Изменение прошло успешно." };
                }
                
            }
            return new SuccessMess() {success=false,reason = "Запись не обнаруженна, или произошла ошибка." };
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
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