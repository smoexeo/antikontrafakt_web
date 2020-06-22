using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DBContext;
using Newtonsoft.Json;
using AntiContr_Lib;
namespace WebApiAntiContr.Controllers
{
    public class Get_product_complaintsController : ApiController
    {
        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        public object Get(string token)
        {
            try
            { 
                DBDataContext dBDataContext = new DBDataContext();
                var list_iduser = (from re in dBDataContext.Users where re.UserToken == token select re.IdUser).ToList();
                if (list_iduser.Count != 0)
                {
                    int id_user = list_iduser[0];
                    var list_req = (from re in dBDataContext.RequestProds
                                    where re.IdUser == id_user
                                    select new Product()
                                    { name = re.IdReq, barcode = re.Barcode, description = re.TextRequest, status = re.Status }).ToList();

                    return list_req;
                }

                return new Error() {error = 0, description = "Данный токен не действителен." };
            }
            catch(Exception e)
            {
                return new Error() { error = -1, description = e.Message };
            }
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