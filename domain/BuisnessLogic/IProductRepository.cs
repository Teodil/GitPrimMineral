using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnessLogic
{
    public interface IProductRepository
    {
        Product[] GetAll();
        Product[] GetBySearch(string query);
        Product[] GetFavoutrite();
        Product GetById(int id);
        Product[] GetAllByIds(IEnumerable<int> productIds);

        int GetLastId();
        void AddProduct(string Tittle, string Category, int Count, string Description, string Size, float Waight, decimal Price, bool IsFavourite);
        void UpdateProduct(int id, string Tittle, string Category, int Count, string Description, string Size, float Waight, decimal Price, bool IsFavourite);

        void UploadImage(int productId, string path);

        void DeleteImage(int productId, string path);
    }
}
