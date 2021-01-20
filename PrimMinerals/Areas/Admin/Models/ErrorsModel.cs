using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrimMinerals.Areas.Admin.Models
{
    public class ErrorsModel
    {
        public Dictionary<string,string> Errors { get; set; }

        public ErrorsModel()
        {
            Errors = new Dictionary<string, string>();
        }
    }
}
