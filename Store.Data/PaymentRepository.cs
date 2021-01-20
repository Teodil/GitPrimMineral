using BuisnessLogic;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Data
{
    public class PaymentRepository
    {

        private MySqlConnection connection = new MySqlConnection("Server=localhost;Port=3307;Database=mydb;Uid=root;Pwd=123456yt;");
        MySqlCommand command;
        MySqlDataReader dataReader;

        public Payment[] GetAll()
        {
            connection.Close();
            connection.Open();
            command = connection.CreateCommand();


            List<Payment> payments = new List<Payment>();

            command.CommandText = "SELECT * FROM paymentinfo";

            dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                payments.Add(new Payment(
                    dataReader.GetInt32("Id"),
                    dataReader.GetString("Tittle")
                    ));
            }

            dataReader.Close();
            connection.Close();

            return payments.ToArray();
        }

        public Payment GetById(int id)
        {
            connection.Close();
            connection.Open();
            command = connection.CreateCommand();
            Payment payment = null;

            command.CommandText = $"SELECT * FROM paymentinfo where Id={id}";

            dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                payment = new Payment(
                    dataReader.GetInt32("Id"),
                    dataReader.GetString("Tittle")
                    );
            }

            dataReader.Close();
            connection.Close();

            return payment;
        }
    }
}
