using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using AntiContr_Lib;
using DBContext;
using System.Text.RegularExpressions;
using System.Text;
using System.Security.Cryptography;
namespace WebApiAntiContr.Controllers
{
    public class LoginController : ApiController
    {
        // GET api/<controller>/5
        public object Get(string email, string pass)
        {

            DBDataContext db = new DBDataContext();

            var passmd5 = Hash.GetMd5Hash((pass + "-sol"));
            List<User> user = (from re in db.Users where re.Email == email && re.UserHesh == passmd5 select re).ToList();
            Token token = new Token();
            if (user.Count != 0)
            {
                token.token = user[0].UserToken;
            }
            else
            {
                token.token = "";
            }

            return token;
        }

        // POST api/<controller>

        public object Post([FromBody]Newtonsoft.Json.Linq.JToken value)
        {
            /*
             Возможные проблемы:
             1) если есть одинаковый токен как у пользователя так и у администратора
             2) решение проблемы должно быть заложенно в формировании новых токенов
             */
            ApiLogin apiLogin=JsonConvert.DeserializeObject<ApiLogin>(value.ToString());

            TypeToken typeToken = new TypeToken() {token="", typeUser=TypeUser.None};

            if(apiLogin==null)
            {
                return typeToken;
            }

            DBDataContext db = new DBDataContext();
            var passmd5 = Hash.GetMd5Hash((apiLogin.code + "-sol"));
            var user = (from re in db.Users where re.Email == apiLogin.email && re.UserHesh == passmd5 select re).ToList();
            var admins = (from re in db.UserAdmins where re.Login == apiLogin.email && re.Password == passmd5 select re).ToList();

            if (admins.Count != 0)
            {
                typeToken.token = admins[0].Token;
                typeToken.typeUser = TypeUser.Admin;
            }

            if (user.Count != 0)
            {
                typeToken.token = user[0].UserToken;
                typeToken.typeUser = TypeUser.User;
            }

            return typeToken;
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