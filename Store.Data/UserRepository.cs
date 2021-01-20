using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Data
{
    public class UserRepository
    {
        private MySqlConnection connection = new MySqlConnection("Server=localhost;Port=3307;Database=mydb;Uid=root;Pwd=123456yt;");
        MySqlCommand command;
        MySqlDataReader dataReader;

        public UserRepository()
        {}

        public bool GetUser(string username,string password)
        {
            connection.Close();
            connection.Open();

            command = connection.CreateCommand();
            command.CommandText = $"SELECT * FROM superUser WHERE userName='{username}' AND password='{password}'";

            List<string> finds = new List<string>();

            dataReader = command.ExecuteReader();

            while (dataReader.Read())
            {
                finds.Add(dataReader["userName"].ToString());
            }

            if (finds.Count == 0)
            {
                return false;
            }

            return true;
        }
    }
}
