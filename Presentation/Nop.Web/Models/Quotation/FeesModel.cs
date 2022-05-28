using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Models.Quotation
{
    public class FeesModel
    {
        public decimal Percent { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }
    }
}
