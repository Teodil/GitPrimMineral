using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnessLogic
{
    public class OrderItem
    {
        public int ProductId { get; set; }
        public string Tittle { get; set; }
        public string Image { get; set; }
        public int Count { get; set; }
        public decimal Price { get; set; }

        public float Waight { get; set; }

        public OrderItem(int productId,string tittle,string image, decimal price, int count, float waight)
        {
            ProductId = productId;
            Tittle = tittle;
            Image = image;
            Price = price;
            Count = count;
            Waight = waight;

        }

        public OrderItem(int productId, string tittle, decimal price, int count, float waight)
        {
            ProductId = productId;
            Tittle = tittle;
            Price = price;
            Count = count;
            Waight = waight;

        }

    }
}
