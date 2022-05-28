using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Models.Quotation
{
    public class QuotationSearchModel: BaseSearchModel
    {
        public QuotationSearchModel()
        {
            AvailableCountries = new List<SelectListItem>();
            AvailableQuotationStatus = new List<SelectListItem>();
        }
        public bool Received { get; set; }
        public string Keyword { get; set; }
        public string LeadDateSearch { get; set; }
        public string Email { get; set; }
        public string Product { get; set; }
        public string Name { get; set; }
        public int Status { get; set; }
        public string Country { get; set; }
        public IList<SelectListItem> AvailableCountries { get; set; }
        public IList<SelectListItem> AvailableQuotationStatus { get; set; }
        public int? Id { get; set; }
    }
}
