using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Text.RegularExpressions;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using AntiContr_Lib;
using DBContext;


namespace WebApiAntiContr.Controllers
{
    public class Check_barcodeController : ApiController
    {
        //http://localhost:51675/api/Сheck_barcode?barcode=4605246009969
        // GET api/<controller>/5
        public object Get(string barcode)
        {
            if (string.IsNullOrEmpty(barcode))
            {
                return new MessBarCode()
                {
                    result = "Указанный товар не существует(не найден)",
                    info = null
                };
            }
            DBDataContext db = new DBDataContext();

            var barcodes = (from re in db.Products where barcode == re.Barcode select re).ToList();
            MessBarCode messBarCode;
            if (barcodes.Count!=0)
            {
                messBarCode = new MessBarCode()
                {
                    result = "Указанный товар существует",
                    info = new FullInfoBarcode()
                    {
                        article = barcodes[0].Article,
                        brend = barcodes[0].Brand,
                        country = barcodes[0].Country,
                        group = barcodes[0].InventoryGroup,
                        name = barcodes[0].ProductGroupName,
                        unit_value = barcodes[0].UnitValue
                    }
                };
            }
            else
            {
                messBarCode = GetInfoOnSite(barcode);
            }

            return messBarCode;
        }
        MessBarCode GetInfoOnSite(string barcode)
        {
            WebClient webClient = new WebClient();
            webClient.Headers.Add(HttpRequestHeader.ContentType, "text/html; charset=utf-8");
            var webresult = webClient.DownloadString(@"https://service-online.su/text/shtrih-kod/cod.php?cod=" + barcode);
            Regex regex = new Regex(@"<p>\D*<br />\D*</p>");
            MessBarCode messBarCode = new MessBarCode();
            var result = regex.Match(webresult);

            if (result.Success)
            {
                messBarCode.result = "Указанный товар существует";
                messBarCode.info = new FullInfoBarcode() { country = result.Value.Replace("<p>Код верный<br />", "").Replace("</p>", "").Replace("Cтрана производитель", "") };
            }
            else
            {
                messBarCode.result = "Указанный товар не существует(не найден)";
            }

            return messBarCode;
        }
    }
}