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
                using (MD5 md5Hash = MD5.Create())
                {
                    DateTime now = DateTime.Now;
                    var bresult = VerifyMd5Hash(md5Hash, now.ToString("dd-MM-yy-hh") + user[0].UserHesh, hash);
                    if (bresult)
                    {
                        user[0].UserHesh = pass + "-sol";
                        db.SubmitChanges();
                        return new SuccessMess() { success = bresult, reason = "Пароль успешно изменен." };
                    }
                  
                }
            }

            return new SuccessMess() { success = false, reason = "Пароль удачно не изменился." };
        }
        string GetMd5Hash(MD5 md5Hash, string input)
        {

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }
        bool VerifyMd5Hash(MD5 md5Hash, string input, string hash)
        {
            // Hash the input.
            string hashOfInput = GetMd5Hash(md5Hash, input);

            // Create a StringComparer an compare the hashes.
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            if (0 == comparer.Compare(hashOfInput, hash))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
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