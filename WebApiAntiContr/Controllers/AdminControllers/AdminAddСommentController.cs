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
    public class AdminAddСommentController : ApiController
    {
        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        public object Get(string token, int id,string comment)
        {
            DBDataContext db = new DBDataContext();
            var admins = (from re in db.UserAdmins where re.Token == token select re).ToList();
            if (admins.Count != 0)
            {
                try
                {
                    Feedback feedback = new Feedback() { FeedbackText = comment, IdReq = id };
                    db.Feedbacks.InsertOnSubmit(feedback);
                    db.SubmitChanges();
                    return new SuccessMess() { success = true, reason = "Комментарий добавлен." };
                }
                catch
                { }
            }
            return new SuccessMess() { success = false, reason = "Запись не обнаруженна, или произошла ошибка." };
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