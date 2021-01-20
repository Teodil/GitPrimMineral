using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnessLogic
{
    public class Delivery
    {
        public int Id { get; set; }
        public string Tittle { get; set; }
        public decimal Price { get; set; }
        public string CityTo { get; set; }
        public string StreetTo { get; set; }
        public string HouseTo { get; set; }
        public string FlatTo { get; set; }
        public string IndexTo { get; set; }

        public Delivery(int Id,string Tittle)
        {
            this.Id = Id;
            this.Tittle = Tittle;
        }
    }
}
