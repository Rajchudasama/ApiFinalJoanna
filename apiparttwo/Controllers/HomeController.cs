using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Com.CloudRail.SI;
using Com.CloudRail.SI.ServiceCode.Commands.CodeRedirect;
using Com.CloudRail.SI.Services;
using Coinbase.ObjectModel;
using Coinbase.Serialization;
using Coinbase.Converters;
using Coinbase;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using apiparttwo.Models;
using Newtonsoft.Json.Linq;

namespace apiparttwo.Controllers
{
    
    public class HomeController : Controller
    {
        
        [Authorize]
        public ActionResult Index()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            List<SelectListItem> currencylist = new List<SelectListItem>();


            currencylist.Add(new SelectListItem { Text = "EUR", Value = "EUR" });
            currencylist.Add(new SelectListItem { Text = "AUD", Value = "AUD" });
            currencylist.Add(new SelectListItem { Text = "INR", Value = "INR" });
            currencylist.Add(new SelectListItem { Text = "USD", Value = "USD" });
            currencylist.Add(new SelectListItem { Text = "CAD", Value = "CAD" });
            currencylist.Add(new SelectListItem { Text = "GBP", Value = "GBP" });
            currencylist.Add(new SelectListItem { Text = "JPY", Value = "JPY" });
            

            items.Add(new SelectListItem { Text = "Bitcoin", Value = "BTC", Selected = true });
            items.Add(new SelectListItem { Text = "Etherum", Value = "ETH" });


            ViewBag.currencylist = currencylist;
            ViewBag.cryptos = items;
            return View();
        }
       
        public async Task<ActionResult> getPrice(string crypto= "BTC", int unit=1,string currency = "USD")
        {
            string GetForums = string.Format("https://min-api.cryptocompare.com/data/top/exchanges?fsym={0}&tsym={1}",crypto,currency);
            // string list;
            List<string> forums = new List<string>();
            List<ForumPrice> pricelist = new List<ForumPrice>();
            using (HttpClient client = new HttpClient())
            {

                client.BaseAddress = new Uri(GetForums);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync(GetForums);
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();

                    //List<ForumPrice> pricelist = new List<ForumPrice>();
                    var apidata = JsonConvert.DeserializeObject<JObject>(data);
                    JObject jObject = new JObject(apidata);
                    forums = jObject.SelectTokens("Data[*].exchange").Select(x => (string)x).ToList();
                    foreach(string forum in forums)
                    {
                        ForumPrice forumPrice = new ForumPrice();
                        forumPrice.Name = forum;
                        forumPrice.Price = await ForumPrice(forum,crypto, currency);
                        pricelist.Add(forumPrice);
                       

                    }



                }
                ViewBag.price = pricelist;

                //return View(pricelist);
                return Json(pricelist, JsonRequestBehavior.AllowGet);
            }
        }
        public async Task<double> ForumPrice(string forum,string crypto,string currency)
        {
           // string GetForums = "https://min-api.cryptocompare.com/data/price?fsym=BTC&tsyms=USD&e=Bitfinex&extraParams=your_app_name";
            string url = String.Format("https://min-api.cryptocompare.com/data/price?fsym={1}&tsyms={2}&e={0}&extraParams=your_app_name",forum,crypto,currency);
            //List<string> forums = new List<string>();
            Double price;
            using (HttpClient client = new HttpClient())
            {

                client.BaseAddress = new Uri(url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();

                    //List<ForumPrice> pricelist = new List<ForumPrice>();
                    var apidata = JsonConvert.DeserializeObject<JObject>(data);
                    JObject jObject = new JObject(apidata);
                    price = Convert.ToDouble(jObject.SelectToken(currency));




                }
                else
                {
                    price = 0;
                }

                return price;
            }

        }
        //public async Task<ActionResult> currency(bool? currencylistget,string getcurrencyrate)
        //{
        //    string listurl = "https://free.currencyconverterapi.com/api/v5/currencies";
        //    string converturl = string.Format("https://free.currencyconverterapi.com/api/v5/convert?q=USD_{0}&compact=ultra",getcurrencyrate);
        //    List<string> currencylist = new List<string>();
        //    if (currencylistget == true)
        //    {
        //        using (HttpClient client = new HttpClient())
        //        {
        //            client.BaseAddress = new Uri(listurl);
        //            client.DefaultRequestHeaders.Accept.Clear();
        //            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        //            HttpResponseMessage response = await client.GetAsync(listurl);
        //            if (response.IsSuccessStatusCode)
        //            {
        //                var data = await response.Content.ReadAsStringAsync();

        //                //List<ForumPrice> pricelist = new List<ForumPrice>();
        //                var apidata = JsonConvert.DeserializeObject<JObject>(data);
        //                JObject jObject = new JObject(apidata);
        //                //currencylist = jObject.SelectToken("results[*].id").Select(x => (string)x).ToList();
        //                return Json(jObject, JsonRequestBehavior.AllowGet);



        //            }
        //        }


        //    }
        //    else
        //    {
        //        using (HttpClient client = new HttpClient())
        //        {
        //            client.BaseAddress = new Uri(converturl);
        //            client.DefaultRequestHeaders.Accept.Clear();
        //            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        //            HttpResponseMessage response = await client.GetAsync(converturl);
        //            if (response.IsSuccessStatusCode)
        //            {
        //                var data = await response.Content.ReadAsStringAsync();

        //                //List<ForumPrice> pricelist = new List<ForumPrice>();
        //                var apidata = JsonConvert.DeserializeObject<JObject>(data);
        //                JObject jObject = new JObject(apidata);
        //               // currencylist = jObject.SelectToken("results[*].id").Select(x => (string)x).ToList();
        //                return Json(jObject, JsonRequestBehavior.AllowGet);



        //            }
        //        }
        //    }
        //    return View();

        //}
        

        
       

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}