using BuisnessLogic;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Data
{
    public class DeliveryRepository
    {
        private MySqlConnection connection = new MySqlConnection("Server=localhost;Port=3307;Database=mydb;Uid=root;Pwd=123456yt;");
        MySqlCommand command;
        MySqlDataReader dataReader;

        public DeliveryRepository()
        {

        }

        public Delivery[] GetAll()
        {
            connection.Close();
            connection.Open();

            command = connection.CreateCommand();

            command.CommandText = "SELECT * FROM deliveryinfo";
            dataReader = command.ExecuteReader();

            List<Delivery> deliveries = new List<Delivery>();
            while (dataReader.Read())
            {
                deliveries.Add(
                    new Delivery(dataReader.GetInt32("id"),dataReader.GetString("Tittle"))
                    );
            }
            dataReader.Close();
            connection.Close();

            return deliveries.ToArray();
        }

        public Delivery GetById(int deliveryId)
        {
            connection.Close();
            connection.Open();

            command = connection.CreateCommand();

            command.CommandText = $"SELECT * FROM deliveryinfo where id={deliveryId}";
            dataReader = command.ExecuteReader();

            Delivery delivery = null;
            while (dataReader.Read())
            {
                delivery = new Delivery(dataReader.GetInt32("id"), dataReader.GetString("Tittle"));
            }
            dataReader.Close();
            connection.Close();

            return delivery;
        }
    }
}
