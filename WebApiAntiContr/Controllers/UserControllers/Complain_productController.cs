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

        public object UpsertUserInfo([FromBody] Newtonsoft.Json.Linq.JToken value)
        {
            var userApi = JsonConvert.DeserializeObject<UserInfoToken>(value.ToString());

            if (userApi == null)
                return new SuccessMess()
                {
                    success = false,
                    reason = "Неверный формат запроса"
                };

            DBDataContext db = new DBDataContext();
            var users = (from user in db.Users
                         where user.UserToken == userApi.token
                         select user)
                         .ToList();

            if (users.Count > 1 || users.Count == 0)
                throw new Exception("Дублирование пользователей");

            users[0].Email = userApi.email;
            users[0].Phone = userApi.phone;
            users[0].FIO = userApi.fio;

            db.SubmitChanges();

            return new SuccessMess()
            {
                success = true,
                reason = "Данные пользователя обновлены"
            };
        }

        // POST api/<controller>
        public object Post([FromBody]Newtonsoft.Json.Linq.JToken value)
        {
            var api = JsonConvert.DeserializeObject<ApiComplain_product>(value.ToString());

            if (api == null)
                return new SuccessMess() 
                { 
                    success = false, 
                    reason = "Неверный формат запроса" 
                };

            DBDataContext db = new DBDataContext();
            var users = (from re in db.Users 
                         where re.UserToken == api.token 
                         select re)
                         .ToList();

            if (api.text_request == string.Empty) 
                return new SuccessMess() 
                { 
                    success = false, 
                    reason = "Введены не все данные." 
                };

            if (users.Count != 0)
            {
                db.Requests.InsertOnSubmit(new Request()
                {
                    Address = api.adress,
                    Date = DateTime.Now,
                    IdUser = users[0].IdUser,
                    Status = "В рассмотрении",
                    TextRequest = api.text_request,
                    Type = api.type,
                    Unit = api.unit
                });
                db.SubmitChanges();

                return new SuccessMess() { 
                    success = true, 
                    reason = null
                };
            }
            else
            {
                return new SuccessMess() { 
                    success = true, 
                    reason = "Токен не действителен." 
                };
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