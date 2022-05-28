using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Core.Domain.Quotations
{
    public class QuotationMessage : Quotation
    {
        public string QuotationNumber { get { return Id.ToString("D8"); } }
        public string CustomerFullName { get; set; }
        public string VendorFullName { get; set; }
        public string ProductTitle { get; set; }
    }
}
