using BuisnessLogic;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PrimMinerals.Models;
using Store.Data;

namespace PrimMinerals.Controllers
{
    public class OrderController : Controller
    {
        private readonly OrderService orderService;
        private readonly DeliveryRepository deliveryRepository;
        private readonly PaymentRepository paymentRepository;

        public OrderController(OrderService orderService, DeliveryRepository deliveryRepository, PaymentRepository paymentRepository)
        {
            this.deliveryRepository = deliveryRepository;
            this.orderService = orderService;
            this.paymentRepository = paymentRepository;
        }

        [HttpGet]
        public IActionResult Index()
        {
            if (orderService.TryGetOrder(out Order order) && order.Items.Count>0)
                return View(order);

            return View("Empty");
        }

        [HttpPost]
        public IActionResult AddItem(int productId, string redirectPage, int count)
        {
            orderService.AddProduct(productId, count);
            return Redirect(redirectPage);
        }
        [HttpPost]
        public IActionResult RemoveItem(int productId, string redirectPage,int count)
        {
            orderService.RemoveProduct(productId,count);
            return Redirect(redirectPage);
        }
        [HttpPost]
        public IActionResult StartConfirmation()
        {
            var order = orderService.GetOrder();
            order.Errors.Clear();
            return View("StartConfirmation", order);
        }

        [HttpPost]
        public IActionResult SendConfirmation(string email, string cellPhone)
        {
            var model = orderService.SendConfirmation(email,cellPhone);

            if (model.Errors.Count > 0)
                return View("StartConfirmation",model);


            return View("Confirmation", model);
        }

        [HttpPost]
        public IActionResult ConfirmEmail(string cellPhone,string email, int confirmationCode)
        {
            var order = orderService.ConfirmCellPhone(cellPhone,email, confirmationCode);

            if (order.Errors.Count > 0)
                return View("Confirmation", order);




            var deliveries = deliveryRepository.GetAll();
            ChooseDeliveryModel model = new ChooseDeliveryModel(order, deliveries);

            return View("ChooseDelivery",model);
        }
        [HttpPost]
        public IActionResult SetDelivery(int deliveryId, string CityTo, string StreetTo, string HouseTo, string FlatTo, string IndexTo, decimal deliveryPrice)
        {
            Delivery delivery = deliveryRepository.GetById(deliveryId);
            delivery.CityTo = CityTo;
            delivery.StreetTo = StreetTo;
            delivery.HouseTo = HouseTo;
            delivery.FlatTo = FlatTo;
            delivery.IndexTo = IndexTo;
            delivery.Price = deliveryPrice;
            var order = orderService.SetDelivery(delivery);
            var payments = paymentRepository.GetAll();

            PaymentModel paymentModel = new PaymentModel(order, payments);

            return View("Payment",paymentModel);
        }
        [HttpPost]
        public IActionResult SetPayment(int PaymentId)
        {
            var payment = paymentRepository.GetById(PaymentId);
            var order = orderService.SetPayment(payment);

            if(payment.Id == 2)
            {
                return Redirect($"/YandexKassa?orderId={order.Id}&price={order.TotalPrice}");
            }

            return RedirectToAction("Finish", "Order", new { orderId=order.Id}); ;
        }

        public IActionResult Finish(int orderId)
        {
            orderService.Finish(orderId);

            HttpContext.Session.RemoveCart();

            return View();
        }
    }
}
