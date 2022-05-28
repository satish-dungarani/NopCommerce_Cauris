using Nop.Web.Framework.Models;
using Nop.Web.Models.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Models.Quotation
{
    public class QuotationListModel : BasePagedListModel<QuotationViewModel>
    {
       
    }
    public class QuotationViewModel : BaseNopEntityModel
    {
        public int QuotationId { get; set; }
        public string Country { get; internal set; }
        public string CustomerFirstName { get; internal set; }
        public string CustomerLastName { get; internal set; }
        public string CustomerEmail { get; internal set; }
        public ProductDetailsModel ProductDetails { get; internal set; }
        public float Quantity { get; internal set; }
        public decimal Price { get; internal set; }
        public string LeadTime { get; internal set; }
        public string Status { get; internal set; }
        public int StatusId { get; internal set; }
        public string CustomerFullName
        {
            get
            {
                return CustomerLastName + " " + CustomerFirstName;
            }
        }

        public string ProductName { get; internal set; }
        public decimal UnityPrice { get; internal set; }
        public string Specification { get; internal set; }
        public string VendorName { get; internal set; }
    }
}
