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
    public class Complain_productController : ApiController
    {
        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        public object Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        public object Post([FromBody]string token, [FromBody]string barcode, [FromBody]string description)
        {
            
            DBDataContext db = new DBDataContext();
            var users = (from re in db.Users where re.UserToken == token select re).ToList();
            if (barcode == "" || description == "") return new SuccessMess() { success = false, reason = "Введены не все данные." };
            if (users.Count != 0)
            {

                db.RequestProds.InsertOnSubmit(new RequestProd()
                {
                    Barcode = barcode,
                    TextRequest = description,
                    Status = "Принят",
                    IdUser = users[0].IdUser
                });
                db.SubmitChanges();
                return new SuccessMess() { success = true, reason = null};
            }
            else
            {
                return new SuccessMess() { success = true, reason = "Токен не действителен." };
            }

            
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