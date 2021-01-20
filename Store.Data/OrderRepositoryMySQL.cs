using BuisnessLogic;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Data
{
    public class OrderRepositoryMySQL : IOrderRepository, IOrderRepositoryAdmin
    {
        private readonly List<Order> orders = new List<Order>();
        private readonly List<Order> ordersAdmin = new List<Order>();
        private MySqlConnection connection = new MySqlConnection("Server=localhost;Port=3307;Database=mydb;Uid=root;Pwd=123456yt;");
        MySqlCommand command;
        MySqlDataReader dataReader;

        public Order Create()
        {
            connection.Close();
            connection.Open();
            command = connection.CreateCommand();
            command.CommandText = "insert into `order`(cellPhone, email, isFinished) values('-','-',0)";
            command.ExecuteNonQuery();
            command.CommandText = "SELECT id FROM `order` ORDER BY id DESC LIMIT 1";
            int lastId = (int)command.ExecuteScalar();
            var order = new Order(lastId, new OrderItem[0]);

            orders.Add(order);
            connection.Close();

            return order;
        }

        public Order GetById(int id)
        {
            return orders.Single(order => order.Id == id);
        }

        public void Update(Order order)
        {
            connection.Close();
            connection.Open();

            command = connection.CreateCommand();
            command.CommandText = $"DELETE FROM OrderProduct where Order_id={order.Id}";
            command.ExecuteNonQuery();

            command.CommandText = $"DELETE FROM orderdelivery where Order_id={order.Id}";
            command.ExecuteNonQuery();

            foreach (var item in order.Items)
            {
                command.CommandText = "INSERT INTO OrderProduct(product_id, order_id, count, price) " +
                    $"values({item.ProductId}, {order.Id}, {item.Count}, {item.Price})";
                command.ExecuteNonQuery();
            }

            if(order.CellPhone!=null && order.Email != null)
            {
                command.CommandText = $"UPDATE `order` SET cellPhone='{order.CellPhone}', email='{order.Email}' WHERE id={order.Id}";
                command.ExecuteNonQuery();
            }

            if (order.Delivery != null)
            {
                command.CommandText = "INSERT INTO orderdelivery(Order_id, DeliveryInfo_id, Price, CityTo, StreetTo, HouseTo, FlatTo, IndexTo) " +
                    $"VALUES({order.Id}, {order.Delivery.Id}, {order.Delivery.Price}, '{order.Delivery.CityTo}', '{order.Delivery.StreetTo}', '{order.Delivery.HouseTo}', '{order.Delivery.FlatTo}', '{order.Delivery.IndexTo}')";
                command.ExecuteNonQuery();
            }

            if (order.Payment != null)
            {
                command.CommandText = $"UPDATE `order` SET PaymentInfo_Id={order.Payment.Id} WHERE id={order.Id}";
                command.ExecuteNonQuery();
            }

            connection.Close();
        }

        public void FinishOrder(Order order)
        {
            connection.Close();
            connection.Open();

            command = connection.CreateCommand();
            command.CommandText = $"UPDATE `order` SET isFinished=1 WHERE id={order.Id}";
            command.ExecuteNonQuery();

            foreach(var item in order.Items)
            {
                command.CommandText = $"UPDATE product SET Count=Count-{item.Count} WHERE Id={item.ProductId}";
                command.ExecuteNonQuery();
            }


            orders.Remove(order);
        }

        public Order[] GetFullOrdersInfo()
        {
            ordersAdmin.Clear();

            connection.Close();
            connection.Open();


            command = connection.CreateCommand();
            List<Order> orders = new List<Order>();

            command.CommandText = "select * from `order` as o left join paymentinfo as p on o.PaymentInfo_Id = p.Id;";

            dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                int id = Convert.ToInt32(dataReader["id"]);
                string cellPhone = dataReader["cellPhone"].ToString();
                string email = dataReader["email"].ToString();
                bool isFinished = (bool)dataReader["isFinished"];

                orders.Add(new Order(id, new List<OrderItem>()));
                var order = orders.Single(o => o.Id == id);
                order.CellPhone = cellPhone;
                order.Email = email;
                order.IsFinished = isFinished;

                try
                {
                    int PaymentInfo_Id = Convert.ToInt32(dataReader["PaymentInfo_Id"]);
                    string Tittle = dataReader["Tittle"].ToString();
                    order.Payment = new Payment(PaymentInfo_Id, Tittle);
                }
                catch
                {
                }
            }
            dataReader.Close();

            foreach(var order in orders)
            {
                command.CommandText = $"select * from orderproduct as op join product as p on op.Product_Id = p.Id join image as i on p.id=i.ProductId where Order_id={order.Id} group by Product_Id";
                dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    order.Items.Add(new OrderItem(
                        (int)dataReader["Product_Id"],
                        dataReader["Tittle"].ToString(),
                        dataReader["Url"].ToString(),
                        Convert.ToDecimal(dataReader["price"]),
                        (int)dataReader["Count"],
                        (float)(double)dataReader["Waight"]
                        ));
                }
                dataReader.Close();
            }

            foreach(var order in orders)
            {
                command.CommandText = $"select * from orderdelivery as od join deliveryinfo as df on  od.DeliveryInfo_id = df.id where Order_id={order.Id}";
                dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    order.Delivery = new Delivery((int)dataReader["DeliveryInfo_id"],dataReader["Tittle"].ToString());
                    order.Delivery.Price = (decimal)dataReader["Price"];
                    order.Delivery.CityTo = dataReader["CityTo"].ToString();
                    order.Delivery.StreetTo = dataReader["StreetTo"].ToString();
                    order.Delivery.HouseTo = dataReader["HouseTo"].ToString();
                    order.Delivery.FlatTo = dataReader["FlatTo"].ToString();
                    order.Delivery.IndexTo = dataReader["IndexTo"].ToString();
                }
                dataReader.Close();
            }

            connection.Close();

            ordersAdmin.AddRange(orders);

            return orders.ToArray();

        }

        public void DeleteOrder(int orderId)
        {
            connection.Close();
            connection.Open();

            command = connection.CreateCommand();

            command.CommandText = $"DELETE FROM orderdelivery WHERE Order_id={orderId}";
            command.ExecuteNonQuery();

            command.CommandText = $"DELETE FROM orderproduct WHERE Order_id={orderId}";
            command.ExecuteNonQuery();

            command.CommandText = $"DELETE FROM `order` WHERE id={orderId}";
            command.ExecuteNonQuery();

            connection.Close();
        }

        public Order GetFullOrderInfoById(int orderId)
        {
            return ordersAdmin.Single(order => order.Id == orderId);
        }

        public void RemoveItem(int orderId, int productId, int count)
        {
            connection.Close();
            connection.Open();

            command = connection.CreateCommand();
            command.CommandText = $"SELECT count FROM orderproduct where Product_Id={productId} AND Order_id={orderId}";

            int productCount = (int)command.ExecuteScalar();

            if (productCount < count)
            {
                command.CommandText = $"DELETE FROM orderproduct where Product_Id={productId} AND Order_id={orderId}";
                command.ExecuteNonQuery();
            }
            else
            {
                command.CommandText = $"UPDATE orderproduct SET count=count-{count} where Product_Id={productId} AND Order_id={orderId}";
                command.ExecuteNonQuery();
            }

            connection.Close();

            GetFullOrdersInfo();

        }

        public void AddItem(int orderId, int productId, int count)
        {
            connection.Close();
            connection.Open();

            command = connection.CreateCommand();
            command.CommandText = $"SELECT count FROM product where Id={productId}";

            int productCount = (int)command.ExecuteScalar();

            command = connection.CreateCommand();
            command.CommandText = $"SELECT count FROM orderproduct WHERE Product_Id={productId} AND Order_id={orderId}";

            int CurrentCountInOrder = (int)command.ExecuteScalar();

            if (productCount > count)
            {
                if(productCount>= (CurrentCountInOrder + count))
                {
                    command.CommandText = $"UPDATE orderproduct SET count=count+{count} where Product_Id={productId} AND Order_id={orderId}";
                    command.ExecuteNonQuery();
                }
            }

            connection.Close();

            GetFullOrdersInfo();
        }
    }
}
