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
        public object Post([FromBody]Newtonsoft.Json.Linq.JToken value)
        {
            ApiComplain_product apiComplain_Product = JsonConvert.DeserializeObject<ApiComplain_product>(value.ToString());

            if (apiComplain_Product == null)
                return new SuccessMess() { success = false, reason = "Неверный формат запроса" };


            DBDataContext db = new DBDataContext();
            var users = (from re in db.Users where re.UserToken == apiComplain_Product.token select re).ToList();
            if (apiComplain_Product.barcode == "" || apiComplain_Product.description == "") return new SuccessMess() { success = false, reason = "Введены не все данные." };
            if (users.Count != 0)
            {

                db.RequestProds.InsertOnSubmit(new RequestProd()
                {
                    Barcode = apiComplain_Product.barcode,
                    TextRequest = apiComplain_Product.description,
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