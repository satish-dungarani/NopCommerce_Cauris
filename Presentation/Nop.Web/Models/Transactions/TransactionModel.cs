using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Transactions;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Transactions
{
    public class TransactionOverviewModel : BaseNopEntityModel
    {
        public string TransactionNumber { get; set; }
        public int QuotationId { get; set; }
        public int? CaurisModeratorId { get; set; }
        public string CaurisModeratorName { get; set; }
        public string VendorName { get; set; }
        public string CustomerName { get; set; }
        public string ProductName { get; set; }
        public string Country { get; set; }
        public string VendorCountry { get; set; }
        public decimal Amount { get; set; }

        public string AmountFormat { get; set; }

        public float Quantity { get; set; }
        public string DispQuotationDate { get; set; }
        public int PaymentTypeId { get; set; }
        public string PaymentType { get; set; }
        public int DeliveryTypeId { get; set; }
        public string DeliveryType { get; set; }
        public int? IncostetorDeliveryId { get; set; }
        public int DeliveryLimitDays { get; set; }
        public int? VendorDocumentId { get; set; }
        public int? CaurisDocumentId { get; set; }
        public int? CustomerDocumentId { get; set; }
        public int? PaymentProofDocumentId { get; set; }
        public int TransactionStatusId { get; set; }
        public string TransactionStatus { get; set; }

        public string TransactionAdminComment { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public DateTime? UpdatedOnUtc { get; set; }

        public string ModeratorComment { get; set; }
        
        //Donwload Contract 
        public Guid DownloadContractGuid { get; set; }
    }
    public class TransactionListModel : BasePagedListModel<TransactionOverviewModel>
    {

    }
    public class TransactionSearchModel : BaseSearchModel
    {
        public TransactionSearchModel()
        {
            AvailableCountries = new List<SelectListItem>();
            AvailableTransactionStatus = new List<SelectListItem>();
        }
        public bool IsReceived { get; set; }
        public string Keyword { get; set; }
        public string Name { get; set; }
        public int Country { get; set; }
        public string Date { get; set; }
        public string Product { get; set; }
        public string TransNumber { get; set; }
        public int Status { get; set; }
        public IList<SelectListItem> AvailableCountries { get; set; }
        public IList<SelectListItem> AvailableTransactionStatus { get; set; }


    }
}
