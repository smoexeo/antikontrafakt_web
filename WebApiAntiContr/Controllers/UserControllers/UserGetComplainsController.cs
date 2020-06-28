using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using AntiContr_Lib;
using DBContext;

namespace WebApiAntiContr.Controllers.UserControllers
{
    public class UserGetComplainsController : ApiController
    {
        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        public object Get(string token, int count, int page, string status = null)
        {
            //page - страницы начинаются с 1;
            /*проверка токена*/
            DBDataContext db = new DBDataContext();
            var users = (from re in db.Users where re.UserToken == token select re).ToList();
            if (users.Count != 0)
            {
                if (status == null)
                { /*админ есть, пускаем формирование списка*/
                    var records = (from re in users[0].Requests orderby re.Date select new RecordComlains() { id = re.Id.ToString(), date = re.Date.ToString(), status = re.Status } ).ToList();
                    int i = records.Count / (count * page);
                    List<RecordComlains> list = new List<RecordComlains>(records.GetRange(count * (page - 1), (count * (page - 1) + count)>records.Count? records.Count - (count * (page - 1)) : count));
                    return list;
                }
                else
                {
                    var records = (from re in users[0].Requests where re.Status == status orderby re.Date select new RecordComlains() { id = re.Id.ToString(), date = re.Date.ToString(), status = re.Status }).ToList();
                    int i = records.Count / (count * page);
                    List<RecordComlains> list = new List<RecordComlains>(records.GetRange(count * (page - 1), (count * (page - 1) + count) > records.Count ? records.Count-(count * (page - 1)) : count));
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