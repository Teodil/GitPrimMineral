using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnessLogic
{
    public class Product
    {
        public int Id { get;}
        public string Tittle { get; }
        public string Category { get; }
        public int Count { get; }
        public string Description { get; }
        public string Size { get;}
        public float Waight { get;}
        public decimal Price { get;}
        public bool IsFavourite { get; }
        public IList<string> Images { get; set;}


        public Product(int Id,string Tittle,string Category,int Count,string Description,string Size, float Waight, decimal Price, bool IsFavourite)
        {
            this.Id = Id;
            this.Tittle = Tittle;
            this.Category = Category;
            this.Count = Count;
            this.Description = Description;
            this.Size = Size;
            this.Waight = Waight;
            this.Price = Price;
            this.IsFavourite = IsFavourite;
        }
        public void SetImages(IEnumerable<string> Images)
        {
            this.Images = new List<string>(Images.ToList());
        }
    }
}
