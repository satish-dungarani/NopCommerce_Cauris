using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Core.Domain.Quotations
{
    public class Quotation : BaseEntity
    {
       
        public int CustomerId { get; set; }
       
        public int VendorId { get; set; }
      
        public int ProductId { get; set; }
      
        public float Quantity { get; set; }
        public int CountryId { get; set; }
        public DateTime CreationDate { get; set; }
        public int Status { get; set; }
        public QuotationStatus QuotationStatus
        {
            get => (QuotationStatus)Status;
            set => Status = (int)value;
        }
        public DateTime StatusDate { get; set; }
        public string Specification { get; set; }
        public DateTime LeadTime { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Price { get; set; }
    }
}
