using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnessLogic
{
    public interface IOrderRepositoryAdmin
    {
        Order[] GetFullOrdersInfo();
        Order GetFullOrderInfoById(int orderId);
        void RemoveItem(int orderId, int productId, int count);
        void AddItem(int orderId, int productId, int count);
        void DeleteOrder(int orderId);
    }
}
