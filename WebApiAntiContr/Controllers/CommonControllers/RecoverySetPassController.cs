using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using AntiContr_Lib;
using DBContext;
using System.Security.Cryptography;
using System.Text;

namespace WebApiAntiContr.Controllers.CommonControllers
{
    public class RecoverySetPassController : ApiController
    {
        // GET api/<controller>
        public object Get(string hash,string email,string pass)
        {
            if(pass.Length<8)
                new SuccessMess() { success = false, reason = "Пароль меньше 8 символов." };

            DBDataContext db = new DBDataContext();

            List<User> user = (from re in db.Users where re.Email == email select re).ToList();

            if (user.Count != 0)
            {

                    DateTime now = DateTime.Now;
                    var bresult = Hash.VerifyMd5Hash( now.ToString("dd-MM-yy-hh") + user[0].UserHesh, hash);
                    if (bresult)
                    {
                        user[0].UserHesh =Hash.GetMd5Hash(pass + "-sol");
                        db.SubmitChanges();
                        return new SuccessMess() { success = bresult, reason = "Пароль успешно изменен." };
                    }
                  
                
            }

            return new SuccessMess() { success = false, reason = "Пароль удачно не изменился." };
        }
       
    }
}