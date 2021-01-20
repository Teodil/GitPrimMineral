using BuisnessLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;


namespace Store.Data
{
    public class ProductRepositoryMySQL : IProductRepository
    {
        private MySqlConnection connection = new MySqlConnection("Server=localhost;Port=3307;Database=mydb;Uid=root;Pwd=123456yt;");
        MySqlCommand command;
        MySqlDataReader dataReader;
        //private string config = "server=localhost;user id=root; password=root; database=mydb";

        public ProductRepositoryMySQL()
        {
            //connection = new MySqlConnection(config);
        }

        #region IProductRepository

        public Product[] GetAll()
        {
            connection.Close();

            connection.Open();

            command = connection.CreateCommand();
            command.CommandText = "Select * from product order by id DESC";
            List<Product> products = new List<Product>();
            dataReader = command.ExecuteReader();

            while (dataReader.Read())
            {
                var id = Convert.ToInt32(dataReader["id"]);
                var Tittle = dataReader["Tittle"].ToString();
                var Category = dataReader["Category"].ToString();
                var Count = Convert.ToInt32(dataReader["Count"]);
                var Description = dataReader["Description"].ToString();
                var Size = dataReader["Size"].ToString();
                var Waight = (float)(double)dataReader["Waight"];
                var Price = (decimal)dataReader["Price"];
                var IsFavourite = (bool)dataReader["IsFavourite"];

                products.Add(new Product
                    (
                    id,
                    Tittle,
                    Category,
                    Count,
                    Description,
                    Size,
                    Waight,
                    Price,
                    IsFavourite
                    ));
            }
            dataReader.Close();

            foreach(var product in products)
            {
                List<string> imagesString = GetImages(product);
                product.SetImages(imagesString);
            }

            connection.Close();

            return products.ToArray();

        }

        public Product[] GetAllByIds(IEnumerable<int> productIds)
        {
            connection.Open();
            command = connection.CreateCommand();

            List<Product> products = new List<Product>();
            foreach(var id in productIds)
            {
                command.CommandText = $"Select * from 'product' where id={id}";
                dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    products.Add(new Product
                                (
                                Convert.ToInt32(dataReader["id"]),
                                dataReader["Tittle"].ToString(),
                                dataReader["Category"].ToString(),
                                Convert.ToInt32(dataReader["Count"]),
                                dataReader["Description"].ToString(),
                                dataReader["Size"].ToString(),
                                (float)dataReader["Waight"],
                                (decimal)dataReader["Price"],
                                (bool)dataReader["IsFavourite"]
                                ));
                }
            }
            dataReader.Close();

            foreach(var product in products)
            {
                List<string> imagesString = GetImages(product);
                product.SetImages(imagesString);
            }

            connection.Close();

            return products.ToArray();
        }

        public Product GetById(int id)
        {
            connection.Close();
            connection.Open();
            Product product = null;
            command = connection.CreateCommand();
            command.CommandText = $"Select * from product where id={id}";
            dataReader = command.ExecuteReader();

            while (dataReader.Read())
            {
                product = new Product
                                (
                                Convert.ToInt32(dataReader["id"]),
                                dataReader["Tittle"].ToString(),
                                dataReader["Category"].ToString(),
                                Convert.ToInt32(dataReader["Count"]),
                                dataReader["Description"].ToString(),
                                dataReader["Size"].ToString(),
                                (float)(double)dataReader["Waight"],
                                (decimal)dataReader["Price"],
                                (bool)dataReader["IsFavourite"]
                                );
            }
            dataReader.Close();
            if (product != null)
                product.SetImages(GetImages(product));

            connection.Close();

            return product;

        }

        public Product[] GetBySearch(string query)
        {
            connection.Open();
            List<Product> products = new List<Product>();
            command = connection.CreateCommand();
            command.CommandText = $"Select * from product where Tittle Like '%{query}%' or Category LIKE '%{query}%' or Size LIKE '%{query}%' or Waight LIKE '%{query}%' order by id DESC";
            dataReader = command.ExecuteReader();

            while (dataReader.Read())
            {
                products.Add(new Product(
                            Convert.ToInt32(dataReader["id"]),
                            dataReader["Tittle"].ToString(),
                            dataReader["Category"].ToString(),
                            Convert.ToInt32(dataReader["Count"]),
                            dataReader["Description"].ToString(),
                            dataReader["Size"].ToString(),
                            (float)(double)dataReader["Waight"],
                            (decimal)dataReader["Price"],
                            (bool)dataReader["IsFavourite"]
                            ));
            }
            dataReader.Close();
            
            foreach(var product in products)
            {
                product.SetImages(GetImages(product));
            }

            connection.Close();

            return products.ToArray();

        }

        public Product[] GetFavoutrite()
        {
            connection.Open();
            List<Product> products = new List<Product>();
            command = connection.CreateCommand();
            command.CommandText = "Select * from product where IsFavourite=1 order by id DESC";
            dataReader = command.ExecuteReader();

            while (dataReader.Read())
            {
                products.Add(new Product(
                            Convert.ToInt32(dataReader["id"]),
                            dataReader["Tittle"].ToString(),
                            dataReader["Category"].ToString(),
                            Convert.ToInt32(dataReader["Count"]),
                            dataReader["Description"].ToString(),
                            dataReader["Size"].ToString(),
                            (float)(double)dataReader["Waight"],
                            (decimal)dataReader["Price"],
                            (bool)dataReader["IsFavourite"]
                            ));
            }
            dataReader.Close();

            foreach (var product in products)
            {
                product.SetImages(GetImages(product));
            }

            connection.Close();

            return products.ToArray();


        }

        #endregion


        #region IProductRepositoryAdmin

        public void AddProduct(string Tittle, string Category, int Count, string Description, string Size, float Waight, decimal Price, bool IsFavourite)
        {
            connection.Close();
            connection.Open();

            string _Waight = Waight.ToString().Replace(',', '.');
            string _Price = Price.ToString().Replace(',', '.');
            int _IsFavourite = Convert.ToInt32(IsFavourite);

            command = connection.CreateCommand();
            command.CommandText = $"INSERT INTO product (Tittle, Category,Count,Description,Size,Waight,Price,IsFavourite) values("+
                                  $"'{Tittle}','{Category}',{Count},'{Description}','{Size}',{_Waight},{_Price},{_IsFavourite})";

            command.ExecuteNonQuery();

            connection.Close();
        }

        public void UpdateProduct(int id, string Tittle, string Category, int Count, string Description, string Size, float Waight, decimal Price, bool IsFavourite)
        {
            connection.Close();
            connection.Open();

            string _Waight = Waight.ToString().Replace(',', '.');
            string _Price = Price.ToString().Replace(',', '.');
            int _IsFavourite = Convert.ToInt32(IsFavourite);

            command = connection.CreateCommand();
            command.CommandText = $"UPDATE product SET Tittle='{Tittle}', Category='{Category}', Count='{Count}', Description='{Description}', Size='{Size}', Waight='{_Waight}', Price='{_Price}', IsFavourite='{_IsFavourite}' WHERE Id={id}";

            command.ExecuteNonQuery();

            connection.Close();
        }

        public int GetLastId()
        {
            connection.Close();
            connection.Open();
            command = connection.CreateCommand();
            command.CommandText = "SELECT Id FROM product ORDER BY Id DESC LIMIT 1";

            int lastId = (int)command.ExecuteScalar();

            connection.Close();

            return lastId;
        }

        public void UploadImage(int productId, string path)
        {
            connection.Close();
            connection.Open();

            command = connection.CreateCommand();
            command.CommandText = $"INSERT INTO image(Url,ProductId) values('/{path}',{productId})";

            command.ExecuteNonQuery();

            connection.Close();

        }

        public void DeleteImage(int productId, string path)
        {
            connection.Close();
            connection.Open();

            command = connection.CreateCommand();
            command.CommandText = $"DELETE FROM image WHERE Url='{path}' AND ProductId={productId}";

            command.ExecuteNonQuery();

            connection.Close();
        }

        #endregion

        private List<string> GetImages(Product product)
        {
            if (product != null)
            {
                List<string> imagesString = new List<string>();
                command.CommandText = $"Select COUNT(*) from image where productId={product.Id}";
                command.CommandText = $"Select Url from image where productId={product.Id}";
                dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    imagesString.Add(dataReader["Url"].ToString());
                }
                dataReader.Close();
                return imagesString;
            }
            else
            {
                return  new List<string>();
            }
        }

    }
}
