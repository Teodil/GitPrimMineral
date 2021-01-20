using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnessLogic
{
    public class Payment
    {
        public int Id { get; }
        public string Tittle { get; }
        public Payment (int id, string tittle)
        {
            Id = id;
            Tittle = tittle;
        }
    }
}
