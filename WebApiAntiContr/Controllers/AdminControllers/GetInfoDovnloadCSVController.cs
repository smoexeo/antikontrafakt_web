using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AntiContr_Lib;
using DBContext;

namespace WebApiAntiContr.Controllers.AdminControllers
{
    public class GetInfoDownloadCSVController : ApiController
    {
        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        public object Get(string token)
        {

            DBDataContext db = new DBDataContext();
            var admins = (from re in db.UserAdmins where re.Token == token select re).ToList();
            if (admins.Count != 0)
            {

                /*админ есть, пускаем формирование списка*/
                var records = (from re in db.Requests orderby re.Date select new CSVInfoRecord()
                {
                    FIO = re.User.FIO,
                    Phone = re.User.Phone,
                    Email = re.User.Email,
                    Address = re.Address,
                    ID = re.Id.ToString(),
                    Status = re.Status,
                    TextRequest = re.TextRequest,
                    Type = re.Type,
                    Unit = re.Unit                  
                }).ToList();
                   
                    return records;
               
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