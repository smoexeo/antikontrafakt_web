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
    public class UpsertUserDataController : ApiController
    {
        //вставка / обновление данных пользователя
        public object Post([FromBody] Newtonsoft.Json.Linq.JToken value)
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

            if (users.Count == 0)
                throw new Exception("Пользователь не найден");

            if (users.Count > 1)
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
    }
}