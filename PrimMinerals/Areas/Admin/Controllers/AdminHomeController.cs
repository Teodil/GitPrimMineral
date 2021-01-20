using BuisnessLogic;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PrimMinerals.Areas.Admin.Models;
using Store.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using System.Net.Http.Headers;

namespace PrimMinerals.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminHomeController : Controller
    {

        private ErrorsModel errorsModel = new ErrorsModel();
        private readonly UserRepository userRepository;
        private readonly IOrderRepositoryAdmin orderRepository;
        private readonly IProductRepository productRepository;

        IWebHostEnvironment _appEnvironment;

        public AdminHomeController(UserRepository userRepository,
                                   IProductRepository productRepository,
                                   IOrderRepositoryAdmin orderRepository,
                                   IWebHostEnvironment webHostEnvironment)
        {
            this.userRepository = userRepository;
            this.productRepository = productRepository;
            this.orderRepository = orderRepository;
            _appEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            return View(errorsModel);
        }
        [HttpPost]
        public IActionResult TryLogIn(string username, string password)
        {
            errorsModel.Errors.Clear();

            if (userRepository.GetUser(username, password))
            {
                return View("Main");
            }

            errorsModel.Errors["failLogIn"] = "Пользователь не найден";

            return View("Index", errorsModel);
        }


        #region ProductMethods
        [HttpPost]
        public IActionResult Products()
        {
            var products = productRepository.GetAll();
            return View(products);
        }

        [HttpPost]
        public IActionResult ProductsSearch(string query)
        {
            var products = productRepository.GetBySearch(query);
            return View("Products", products);
        }

        [HttpPost]
        public IActionResult ProductEdit(int productId)
        {
            var products = productRepository.GetById(productId);
            return View(products);
        }

        [HttpPost]
        public IActionResult ProductAddPage()
        {
            return View(errorsModel);
        }

        [HttpPost]
        public IActionResult AddProduct(string Tittle, string Category, int Count, string Description, string Size, float Waight, decimal Price, string IsFavourite)
        {
            errorsModel.Errors.Clear();
            bool _IsFavourtie = false;

            if (IsFavourite == "on")
            {
                _IsFavourtie = true;
            }

            if (string.IsNullOrEmpty(Tittle))
            {
                errorsModel.Errors["Tittle"] = "Поле должно быть заполнено";
                return View("ProductAddPage", errorsModel);
            }
            if (string.IsNullOrEmpty(Category))
            {
                errorsModel.Errors["Category"] = "Поле должно быть заполнено";
                return View("ProductAddPage", errorsModel);
            }
            if (Count<1)
            {
                errorsModel.Errors["Count"] = "Поле должно быть заполнено";
                return View("ProductAddPage", errorsModel);
            }
            if (Waight==0)
            {
                errorsModel.Errors["Waight"] = "Поле должно быть заполнено";
                return View("ProductAddPage", errorsModel);
            }
            if (Price==0)
            {
                errorsModel.Errors["Price"] = "Поле должно быть заполнено";
                return View("ProductAddPage", errorsModel);
            }

            productRepository.AddProduct(Tittle, Category, Count, Description, Size, Waight, Price, _IsFavourtie);

            var newFileName = string.Empty;

            if (HttpContext.Request.Form.Files != null)
            {
                var fileName = string.Empty;
                string PathDB = string.Empty;

                var files = HttpContext.Request.Form.Files;

                foreach (var file in files)
                {
                    if (file.Length > 0)
                    {
                        //Getting FileName
                        fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');

                        //Assigning Unique Filename (Guid)
                        var myUniqueFileName = Convert.ToString(Guid.NewGuid());

                        //Getting file Extension
                        var FileExtension = Path.GetExtension(fileName);

                        // concating  FileName + FileExtension
                        newFileName = myUniqueFileName + FileExtension;

                        // Combines two strings into a path.
                        fileName = Path.Combine(_appEnvironment.WebRootPath, "demoImages") + $@"\{newFileName}";

                        // if you want to store path of folder in database
                        PathDB = "demoImages/" + newFileName;

                        using (FileStream fs = System.IO.File.Create(fileName))
                        {
                            file.CopyTo(fs);
                            fs.Flush();
                        }

                        int AddedProductId = productRepository.GetLastId();

                        productRepository.UploadImage(AddedProductId, PathDB);
                    }
                }
            }

            errorsModel.Errors["Added"] = "Товар успешно добавлен";

            return View("ProductAddPage", errorsModel);
        }

        [HttpPost]
        public IActionResult UpdateProduct(int id, string Tittle, string Category, int Count, string Description, string Size, float Waight,decimal Price,string IsFavourite)
        {
            bool _IsFavourtie = false;

            if (IsFavourite == "on")
            {
                _IsFavourtie = true;
            }

            productRepository.UpdateProduct(id, Tittle, Category, Count, Description, Size, Waight, Price, _IsFavourtie);
            return RedirectPreserveMethod($"/Admin/AdminHome/ProductEdit?productId={id}");
        }

        [HttpPost]
        public IActionResult Upload(int productId,string name)
        {
            var newFileName = string.Empty;

            if (HttpContext.Request.Form.Files != null)
            {
                var fileName = string.Empty;
                string PathDB = string.Empty;

                var files = HttpContext.Request.Form.Files;

                foreach (var file in files)
                {
                    if (file.Length > 0)
                    {
                        //Getting FileName
                        fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');

                        //Assigning Unique Filename (Guid)
                        var myUniqueFileName = Convert.ToString(Guid.NewGuid());

                        //Getting file Extension
                        var FileExtension = Path.GetExtension(fileName);

                        // concating  FileName + FileExtension
                        newFileName = myUniqueFileName + FileExtension;

                        // Combines two strings into a path.
                        fileName = Path.Combine(_appEnvironment.WebRootPath, "demoImages") + $@"\{newFileName}";

                        // if you want to store path of folder in database
                        PathDB = "demoImages/" + newFileName;

                        using (FileStream fs = System.IO.File.Create(fileName))
                        {
                            file.CopyTo(fs);
                            fs.Flush();
                        }

                        productRepository.UploadImage(productId, PathDB);
                    }
                }


            }
            return RedirectPreserveMethod($"/Admin/AdminHome/ProductEdit?productId={productId}");
        }

        [HttpPost]

        public IActionResult DeleteImage(int productId, string path)
        {

            string fullPath = _appEnvironment.WebRootPath + path;
            System.IO.File.Delete(fullPath);
            productRepository.DeleteImage(productId, path);

            return RedirectPreserveMethod($@"/Admin/AdminHome/ProductEdit?productId={productId}");
        }

        #endregion

        [HttpPost]
        public IActionResult Orders()
        {
            var order = orderRepository.GetFullOrdersInfo();

            return View(order);
        }

        [HttpPost]
        public IActionResult DeleteOrder(int orderId,string redirectUrl)
        {
            orderRepository.DeleteOrder(orderId);

            return RedirectPreserveMethod(redirectUrl);
        }

        [HttpPost]
        public IActionResult Users()
        {
            return View();
        }
    }
}
