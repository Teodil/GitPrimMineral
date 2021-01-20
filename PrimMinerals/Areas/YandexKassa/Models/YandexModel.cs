using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrimMinerals.Areas.YandexKassa.Models
{
    public class YandexModel
    {
        public int OrderId { get; }
        public decimal Price { get; }

        public YandexModel(int orderId,decimal price)
        {
            OrderId = orderId;
            Price = price;
        }
    }
}
