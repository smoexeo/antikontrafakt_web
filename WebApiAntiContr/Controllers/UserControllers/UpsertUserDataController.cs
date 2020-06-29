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

        public object Get(string fio,string email, string phone, string token)
        {
            var userApi = new UserInfoToken() {fio=fio,email=email,phone=phone,token=token };

            if (userApi == null)
                return new SuccessMess()
                {
                    success = false,
                    reason = "Неверный формат запроса"
                };

            DBDataContext db = new DBDataContext();
            var users = (from u in db.Users
                         where u.UserToken == userApi.token
                         select u)
                         .ToList();

            if (users.Count == 0)
                throw new Exception("Пользователь не найден");

            if (users.Count > 1)
                throw new Exception("Дублирование пользователей");

            var user = users[0];

            if (user.Email != userApi.email)
                user.Email = userApi.email;
            if (user.Phone != userApi.phone)
                user.Phone = userApi.phone;
            if (user.FIO != userApi.fio)
                user.FIO = userApi.fio;

            db.SubmitChanges();

            return new SuccessMess()
            {
                success = true,
                reason = "Данные пользователя обновлены"
            };
        }
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
            var users = (from u in db.Users
                         where u.UserToken == userApi.token
                         select u)
                         .ToList();

            if (users.Count == 0)
                throw new Exception("Пользователь не найден");

            if (users.Count > 1)
                throw new Exception("Дублирование пользователей");

            var user = users[0];

            if (user.Email != userApi.email)
                user.Email = userApi.email;
            if (user.Phone != userApi.phone)
                user.Phone = userApi.phone;
            if (user.FIO != userApi.fio)
                user.FIO = userApi.fio;

            db.SubmitChanges();

            return new SuccessMess()
            {
                success = true,
                reason = "Данные пользователя обновлены"
            };
        }
    }
}