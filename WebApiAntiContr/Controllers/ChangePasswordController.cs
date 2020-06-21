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
    public class ChangePasswordController : ApiController
    {
       

        // GET api/<controller>/5
        public object Get(string token, string oldpass,string newpass)
        {
            DBDataContext db = new DBDataContext();
            List<User> user = (from re in db.Users where re.UserToken==token && re.UserHesh == oldpass select re).ToList();
            ApiChangePass changePass = new ApiChangePass() {success = false};
            if (user.Count != 0)
            {
                user[0].UserHesh = newpass;
                changePass.token = Guid.NewGuid().ToString();
                user[0].UserToken = changePass.token;
                db.SubmitChanges();
                changePass.success = true;
                changePass.reason = "Пароль успешно изменен.";
            }
            else
            {
                changePass.token="";
                changePass.reason = "Старый пароль неверный";
            }
            return changePass;
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