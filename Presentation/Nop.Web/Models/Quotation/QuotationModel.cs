using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Models.Catalog;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Models.Quotation
{
    public partial class QuotationModel : BaseNopModel
    {
        public QuotationModel()
        {
            AvailableCountries = new List<SelectListItem>();
            AllowedQuantities = new List<SelectListItem>();
        }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int CategroyName { get; set; }
        public int VendorId { get; set; }
        public int CustomerVendorId { get; set; }
        public int CustomerId { get; set; }
        public int CountryId { get; set; }
        public IList<SelectListItem> AvailableCountries { get; set; }
        public decimal UnityPrice { get; set; }  
        public float Quantity { get; set; }
        public decimal Price { get; set; }
        public int Status { get; set; }
        public string Result { get;  set; }
        public string Specification { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime LeadTime { get; set; }
        public DateTime? NewLeadTime { get; set; }
        public List<SelectListItem> AllowedQuantities { get;  set; }
        public IEnumerable<FeesModel> Fees { get; set; }
       
        public bool AlreadyExist { get; internal set; }
        public string StatusText { get; internal set; }
        public string Country { get; internal set; }
        public bool IsVendor { get; internal set; }
        public int Id { get; internal set; }
        public int QuotationId { get; set; }
        public string CustomerFullName { get; internal set; }
        public object CreationDate { get; internal set; }
        
        public IList<SelectListItem> PreviousQuotationStatus { get; set; }
        public IList<SelectListItem> NextQuotationStatus { get; set; }
    }
}
