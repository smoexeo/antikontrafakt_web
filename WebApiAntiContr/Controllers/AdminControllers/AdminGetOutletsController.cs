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
    public class AdminGetOutletsController : ApiController
    {
        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        public string Get(string token,string list,string items)
        {
            //подключение бд;
            DBDataContext db = new DBDataContext();
            /*проверка токена*/
            List<UserAdmin> user = (from re in db.UserAdmins where re.Token == token  select re).ToList();
            /*тип жалобы{1, 2}порядковый номер*/
            if (user.Count != 0)
            {
               // List<RequestPO> requests = (from re in db.RequestProds select ).ToList();

                
            }


            /*формирование листа*/
            /*закгрузка на лист элементов*/

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