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
    public class GetMinInfoRecordsController : ApiController
    {
        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        public object Get(string token)
        {
            if (token == null)
                return null;

            DBDataContext db = new DBDataContext();

            var result = (from re in db.Users where re.UserToken == token select re).ToList();

            if (result.Count != 0)
            {
                return new ApiGetMinInfoRecords()
                {
                    notshow = (from re in result[0].Requests where re.Status == "В рассмотрении" select re).ToList().Count,
                    arhiv = (from re in result[0].Requests where re.Status == "Архивирована" select re).ToList().Count,
                    draft = (from re in result[0].Requests where re.Status == "Черновик" select re).ToList().Count
                };
            }
            return new ApiGetMinInfoRecords();
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