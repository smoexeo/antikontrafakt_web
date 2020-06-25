﻿using System;
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
        public object Get(string idStr)
        {
            int id = Convert.ToInt32(idStr);

            DBDataContext db = new DBDataContext();
            var requests = (from req in db.Requests
                            where req.Id == id
                            select req)
                            .ToList();

            if (requests.Count == 0 || requests.Count > 1)
                return null;

            var request = requests[0];
            return new RecordComplainFullInfo
            {
                adress = request.Address,
                status = request.Status,
                textRequest = request.TextRequest,
                type = request.Type,
                unit = request.Unit
            };
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
                    Status = api.status,
                    TextRequest = api.text_request,
                    Type = api.type,
                    Unit = api.unit
                });
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
                    success = true,
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