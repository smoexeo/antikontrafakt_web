using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using Dadata;
using AntiContr_Lib;
using Newtonsoft.Json;

namespace WebApiAntiContr.Controllers
{
    public class Check_outletController : ApiController
    {
        
        static string token = "1cbadd71c5ffaabbd82f45db0f4784dde59648ad";
        static string bartoken = "73cb0766-2aad-4ff5-90f1-80845146bd6a";
        static string secret = "15d77fff9d8d4bab683a091007fea7fe5629cb6f";
        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }


        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        /// <summary>
        /// Функция для проверки ИНН 
        /// </summary>
        /// <param name="tin"> ИНН </param>
        /// <returns>
        /// result: "Указанный ИНН не существует." , "Указанный ИНН является подлинным." ,  "Нет связи с сервисом Dadata.ru"
        /// Address: "...."
        /// Name: "имя организации или владелец"
        /// </returns>
        public object Get(string tin)
        {   
            ApiGetMessCheckTin result = new ApiGetMessCheckTin();
           
            try
            {
                var api = new SuggestClient(token);
                var response = api.FindParty(tin);
                result.result = "Указанный ИНН не существует.";
                if (response.suggestions.Count != 0)
                {
                    var party = response.suggestions[0].data;
                    result.Name = party.name.full_with_opf;
                    result.Address = "г. " + party.address.data.city + ", " + party.address.data.city_district_with_type + "р-н, д." + party.address.data.house;
                    result.result = "Указанный ИНН является подлинным.";
                }
                
            }
            catch
            {
                result.result = "Нет связи с сервисом Dadata.ru";
            }
            return result;
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