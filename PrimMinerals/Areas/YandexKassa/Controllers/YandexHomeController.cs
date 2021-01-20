using Microsoft.AspNetCore.Mvc;
using PrimMinerals.Areas.YandexKassa.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Store.YandexKassa.Areas.YandexKassa.Controllers
{
    [Area("YandexKassa")]
    public class YandexHomeController : Controller
    {
        public static YandexModel YandexModel;

        public IActionResult Index()
        {
            int orderId = Convert.ToInt32(Request.Query.FirstOrDefault(p => p.Key == "orderId").Value);
            decimal price = Convert.ToDecimal(Request.Query.FirstOrDefault(p => p.Key == "price").Value);

            YandexModel = new YandexModel(orderId, price);

            return View(YandexModel);
        }

        public IActionResult CallBack()
        {
            return View("CallBack",YandexModel);
        }
    }
}
