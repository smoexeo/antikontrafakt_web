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

        // отправка заявлений в бд
        // POST api/<controller>
        public object Post([FromBody] Newtonsoft.Json.Linq.JToken value)
        {
            var api = JsonConvert.DeserializeObject<ApiComplain_product>(value.ToString());

            if (api == null)
                return new SuccessMess()
                {
                    success = false,
                    reason = "Неверный формат запроса"
                };

            DBDataContext db = new DBDataContext();
            var users = (from user in db.Users
                         where user.UserToken == api.token
                         select user)
                         .ToList();

            if (api.text_request == string.Empty)
                return new SuccessMess()
                {
                    success = false,
                    reason = "Введены не все данные."
                };

            if (users.Count != 0)
            {
                if (string.IsNullOrEmpty(api.id)) // вставка
                {
                    db.Requests.InsertOnSubmit(new Request()
                    {
                        Address = api.adress,
                        Date = DateTime.Now,
                        IdUser = users[0].IdUser,
                        Status = api.status,
                        TextRequest = api.text_request,
                        Type = api.type,
                        Unit = api.unit
                    });
                }
                else // апдейт
                {
                    var requests = (from request in db.Requests
                                    where request.Id == Convert.ToInt32(api.id)
                                    select request)
                                    .ToList();
                    if (requests.Count > 1 || requests.Count == 0)
                        return new SuccessMess()
                        {
                            success = false,
                            reason = "Дубликат или 0 результатов"
                        };

                    var req = requests[0];
                    req.Address = api.adress;
                    req.Date = DateTime.Now;
                    req.IdUser = users[0].IdUser;
                    req.Status = api.status;
                    req.TextRequest = api.text_request;
                    req.Type = api.type;
                    req.Unit = api.unit;
                }

                db.SubmitChanges();

                return new SuccessMess()
                {
                    success = true,
                    reason = null
                };
            }
            else
            {
                return new SuccessMess()
                {
                    success = false,
                    reason = "Токен не действителен."
                };
            }
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}