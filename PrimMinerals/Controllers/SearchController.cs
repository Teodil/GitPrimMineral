using BuisnessLogic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrimMinerals.Controllers
{
    public class SearchController : Controller
    {
        private readonly IProductRepository productRepository;

        public SearchController(IProductRepository productRepository)
        {
            this.productRepository = productRepository;
        }



        public ActionResult Index(string query)
        {
            var products = productRepository.GetBySearch(query);
            return View(products);
        }
    }
}
