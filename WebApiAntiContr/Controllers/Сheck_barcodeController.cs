using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using AntiContr_Lib;


namespace WebApiAntiContr.Controllers
{
    public class Сheck_barcodeController : ApiController
    {
        // GET api/<controller>/5
        public object Get(string barcode)
        {
            //WebBrowser webBrowser = new WebBrowser();
            //webBrowser.Navigate(@"http://ru.disai.org");
            //while (webBrowser.ReadyState != WebBrowserReadyState.Complete)
            //    Application.DoEvents();
            //var elementCollection = webBrowser.Document.GetElementsByTagName("a");
           
            //var inpute = webBrowser.Document.GetElementsByTagName("input");
            //inpute[0].InnerText = barcode;
            //elementCollection[37].InvokeMember("click");
            
            //while (webBrowser.ReadyState != WebBrowserReadyState.Complete)
            //    Application.DoEvents();
            //var front = webBrowser.Document.GetElementsByTagName("front");
            



            //return "value";
            MessBarCode messBarCode = new AntiContr_Lib.MessBarCode();
            messBarCode.result = "Указанный товар не существует";
            messBarCode.info = "В разработке, сервис не найден";
            return messBarCode;
        }
    }
}