using BuisnessLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrimMinerals.Models
{
    public class PaymentModel
    {
        public Order Order;
        public Payment[] Paymenets;
        public PaymentModel(Order order, Payment[] payments)
        {
            Order = order;
            Paymenets = payments;
        }
    }
}
