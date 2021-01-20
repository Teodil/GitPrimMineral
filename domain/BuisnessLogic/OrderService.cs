using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BuisnessLogic
{
    public class OrderService 
    {
        private readonly IProductRepository productRepository;
        private readonly IOrderRepository orderRepository;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IMessageService messageService;

        protected ISession Session => httpContextAccessor.HttpContext.Session;

        public OrderService(IProductRepository productRepository,
                    IOrderRepository orderRepository,
                    IHttpContextAccessor httpContextAccessor,
                    IMessageService messageService)
        {
            this.productRepository = productRepository;
            this.orderRepository = orderRepository;
            this.httpContextAccessor = httpContextAccessor;
            this.messageService = messageService;
        }

        public bool TryGetOrder(out Order order)
        {
            if (Session.TryGetCart(out Cart cart))
            {
                order = orderRepository.GetById(cart.OrderId);
                return true;
            }

            order = null;
            return false;
        }

        internal IEnumerable<Product> GetProducts(Order order)
        {
            var ProductIds = order.Items.Select(item => item.ProductId);

            return productRepository.GetAllByIds(ProductIds);
        }

        public Order AddProduct(int productId, int count)
        {
            if (count < 1)
                throw new InvalidOperationException("Too few books to add");

            if (!TryGetOrder(out Order order))
                order = orderRepository.Create();

            AddOrUpdateProduct(order, productId, count);
            UpdateSession(order);

            return order;
        }

        internal void AddOrUpdateProduct(Order order, int productId, int count)
        {
            var product = productRepository.GetById(productId);
            if (product.Count > count)
            {
                if (order.TryGetItem(productId, out OrderItem orderItem))
                {
                    if(product.Count > orderItem.Count)
                    {
                        orderItem.Count += count;

                    }
                }
                else
                {
                    string image = null;
                    if (product.Images.Count > 0)
                    {
                        image = product.Images[0];
                    }
                    order.AddItem(product.Id, product.Tittle, image, product.Price, count, product.Waight);
                }
            }

            orderRepository.Update(order);
        }

        internal void UpdateSession(Order order)
        {
            var cart = new Cart(order.Id, order.TotalCount, order.TotalPrice);
            Session.Set(cart);
        }


        public Order RemoveProduct(int productId,int count)
        {
            var order = GetOrder();
            order.RemoveItem(productId,count);

            orderRepository.Update(order);
            UpdateSession(order);

            return order;
        }

        public Order GetOrder()
        {
            if (TryGetOrder(out Order order))
                return order;

            throw new InvalidOperationException("Empty session.");
        }


        public Order SendConfirmation(string email, string cellPhone)
        {
            var order = GetOrder();
            order.Errors.Clear();

            if (CheckEmail(email))
            {
                var confirmationCode = new Random().Next(1000,9999);
                order.CellPhone = cellPhone;
                order.Email = email;
                Session.SetInt32(email, confirmationCode);
                messageService.SendCode(confirmationCode, email);
            }
            else
                order.Errors["email"] = "Неверно указана почта";

            return order;
        }

        private bool CheckEmail(string email)
        {
            string cond = @"(\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*)";
            if (Regex.IsMatch(email, cond))
                return true;
            else
                return false;
        }

        public Order ConfirmCellPhone(string cellPhone,string email, int confirmationCode)
        {
            int? storedCode = Session.GetInt32(email);
            var order = GetOrder();
            order.Errors.Clear();

            if (storedCode == null)
            {
                order.Errors["email"] = "Что-то случилось. Попробуйте получить код ещё раз.";
                return order;
            }

            if (storedCode != confirmationCode)
            {
                order.Errors["confirmationCode"] = "Неверный код. Проверьте и попробуйте ещё раз.";
                return order;
            }

            order.Email = email;
            order.CellPhone = cellPhone;
            orderRepository.Update(order);

            Session.Remove(email);

            return order;
        }

        public Order SetDelivery(Delivery delivery)
        {
            var order = GetOrder();
            order.Delivery = delivery;

            orderRepository.Update(order);
            UpdateSession(order);

            return order;
        }

        public Order SetPayment(Payment payment)
        {
            var order = GetOrder();
            order.Payment = payment;

            orderRepository.Update(order);
            UpdateSession(order);

            return order;
        }

        public void Finish(int orderId)
        {
            var order = orderRepository.GetById(orderId);
            orderRepository.FinishOrder(order);
        }
    }
}
