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
            var opassmd5 = Hash.GetMd5Hash((oldpass + "-sol"));
            var npassmd5 = Hash.GetMd5Hash((newpass + "-sol"));
            List<User> user = (from re in db.Users where re.UserToken==token && re.UserHesh == opassmd5 select re).ToList();
            List<UserAdmin> admins = (from re in db.UserAdmins where re.Token == token && re.Password == opassmd5 select re).ToList();

            ApiChangePass changePass = new ApiChangePass() {success = false};

            if (user.Count != 0 && admins.Count != 0)
            {
                throw new Exception("Пользователь не найден");
            }

            string newtoken = Guid.NewGuid().ToString();
            var tokens = (from re in db.Users select re.UserToken).ToList<string>();
            tokens.AddRange((from re in db.UserAdmins select re.Token).ToList());
            while ((from re in tokens where newtoken == re select re).ToList().Count != 0)
            {
                newtoken = Guid.NewGuid().ToString();
            }


            if (user.Count != 0)
            {
                if (newpass.Length < 8)
                {
                    changePass.token = "";
                    changePass.reason = "Пароль меньше 8 символов";
                }
                else
                {
                    user[0].UserHesh = npassmd5;
                    changePass.token = newtoken;
                    user[0].UserToken = changePass.token;
                    db.SubmitChanges();
                    changePass.success = true;
                    changePass.reason = "Пароль успешно изменен.";
                }
            }
            else
                if (admins.Count != 0)
                {
                    if (newpass.Length < 8)
                    {
                        changePass.token = "";
                        changePass.reason = "Пароль меньше 8 символов";
                    }
                    else
                    { 
                        admins[0].Password = npassmd5;
                        changePass.token = newtoken;
                        admins[0].Token = changePass.token;
                        db.SubmitChanges();
                        changePass.success = true;
                        changePass.reason = "Пароль успешно изменен.";
                    }
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