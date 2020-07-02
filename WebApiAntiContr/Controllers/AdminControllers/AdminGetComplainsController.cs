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
        public object Get(string token, int count,int page, string status=null)
        {
            //page - страницы начинаются с 1;
            /*проверка токена*/
            DBDataContext db = new DBDataContext();
            var admins = (from re in db.UserAdmins where re.Token == token select re).ToList();
            if (admins.Count != 0)
            {
                if (status == null)
                { /*админ есть, пускаем формирование списка*/
                    var records = (from re in db.Requests orderby re.Date where re.Status != "Архивирована" && re.Status != "Черновик" select new RecordComlains() { id = re.Id.ToString(), date = re.Date.ToString(), status = re.Status }).ToList();
                    int i = records.Count / (count * page);
                    List<RecordComlains> list = new List<RecordComlains>(records.GetRange(count * (page - 1), (count * (page - 1) + count) > records.Count ? records.Count- (count * (page - 1)) : count));
                    return list;
                }
                else
                { 
                    var records = (from re in db.Requests where re.Status==status orderby re.Date select new RecordComlains() { id = re.Id.ToString(), date = re.Date.ToString(), status = re.Status }).ToList();
                    int i = records.Count / (count * page);
                    List<RecordComlains> list = new List<RecordComlains>(records.GetRange(count * (page - 1), (count * (page - 1) + count) > records.Count ? records.Count - (count * (page - 1)) : count));
                    return list;
                }
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