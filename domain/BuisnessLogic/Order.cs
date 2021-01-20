using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnessLogic
{
    public class Order
    {
        public int Id { get;}
        public string CellPhone { get; set; }
        public string Email { get; set; }
        public List<OrderItem> Items { get; }

        public Delivery Delivery { get; set; }

        public Payment Payment { get; set; }

        public Dictionary<string, string> Errors = new Dictionary<string, string>();

        public bool IsFinished { get; set; }

        public Order(int Id, IEnumerable<OrderItem> items)
        {
            this.Id = Id;
            Items = items.ToList();
        }

        public decimal TotalPrice => Items.Sum(product => product.Price * product.Count);
        public int TotalCount => Items.Sum(product => product.Count);

        public float TotalWaight => Items.Sum(product => product.Waight * product.Count);

        public bool TryGetItem(int productId,out OrderItem orderItem)
        {
            var index = Items.FindIndex(item => item.ProductId == productId);
            if (index == -1)
            {
                orderItem = null;
                return false;
            }

            orderItem = Items[index];
            return true;
        }

        public void AddItem(int productId,string tittle,string image,decimal price, int count,float waight)
        {
            Items.Add(new OrderItem(
                     productId,
                     tittle,
                     image,
                     price,
                     count,
                     waight
                   ));
        }
        
        public void RemoveItem(int productId,int count)
        {
            var index = Items.FindIndex(item => item.ProductId == productId);
            if (index == -1)
                throw new InvalidOperationException("Can't find book");

            if (Items[index].Count > count)
                Items[index].Count -= count;
            else
            {
                Items.RemoveAt(index);
                Delivery = null;
            }

        }

    }
}
