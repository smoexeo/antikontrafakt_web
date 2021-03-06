﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Text.RegularExpressions;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using AntiContr_Lib;

using System.Configuration;

namespace WebApiAntiContr.Controllers
{
    public class Сheck_barcodeController : ApiController
    {
        //http://localhost:51675/api/Сheck_barcode?barcode=4605246009969
        // GET api/<controller>/5
        public object Get(string barcode)
        {
          

            WebClient webClient = new WebClient();
            var webresult = webClient.DownloadString(@"https://service-online.su/text/shtrih-kod/cod.php?cod=" + barcode);
            Encoding utf8 = Encoding.GetEncoding("UTF-8");
            Encoding win1251 = Encoding.GetEncoding("Windows-1251");
            byte[] utf8Bytes = win1251.GetBytes(webresult);
            byte[] win1251Bytes = Encoding.Convert(utf8, win1251, utf8Bytes);

            webresult=win1251.GetString(win1251Bytes);
            Regex regex = new Regex(@"<p>\D*<br />\D*</p>");
            MessBarCode messBarCode = new MessBarCode();
            var result = regex.Match(webresult);

            if (result.Success)
            {
                
                messBarCode.result = "Указанный товар существует";
                messBarCode.info = result.Value.Replace("<p>Код верный<br />","").Replace("</p>","");
            }
            else
            {
                messBarCode.result = "Указанный товар не существует(не найден)";
                
            }

            return messBarCode;
        }
    }
}