using BuisnessLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrimMinerals.Models
{
    public class ChooseDeliveryModel
    {
        public Order Order { get; }
        public Delivery[] Delivers { get; }

        public ChooseDeliveryModel(Order order, Delivery[] deliveries)
        {
            Order = order;
            Delivers = deliveries;
        }
    }
}
