using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Cauris.Transaction
{
    public partial class TransactionModel : BaseNopEntityModel
    {
        public TransactionModel()
        {
            AvailableTransactionStatus = new List<SelectListItem>();
        }
      
        public IList<SelectListItem> AvailableTransactionStatus { get; set; }
        public int CustomerId { get; set; }

        [NopResourceDisplayName("Corus.Admin.Transaction.Field.ModeratorIdentity")]
        public string ModeratorIdentity { get; set; }

        [UIHint("DateNullable")]
        [NopResourceDisplayName("Corus.Admin.Transaction.Field.ContractSignatureDate")]
        public DateTime ContractSignatureDate { get; set; }

        [NopResourceDisplayName("Corus.Admin.Transaction.Field.BuyerName")]
        public string BuyerName { get; set; }

        [NopResourceDisplayName("Corus.Admin.Transaction.Field.SellerName")]
        public string SellerName { get; set; }

        [NopResourceDisplayName("Corus.Admin.Transaction.Field.ProductName")]
        public string ProductName { get; set; }

        [NopResourceDisplayName("Corus.Admin.Transaction.Field.ProductId")]
        public int ProductId { get; set; }

        [NopResourceDisplayName("Corus.Admin.Transaction.Field.Quantity")]
        public int Quantity { get; set; }

        [NopResourceDisplayName("Corus.Admin.Transaction.Field.Price")]
        public decimal Price { get; set; }

        [NopResourceDisplayName("Corus.Admin.Transaction.Field.Tax")]
        public decimal Tax { get; set; }
       
       [NopResourceDisplayName("Corus.Admin.Transaction.Field.PaymentTerm")]
        public string PaymentTerm { get; set; }

        [NopResourceDisplayName("Corus.Admin.Transaction.Field.Inspection")]
        public bool Inspection { get; set; }

        [NopResourceDisplayName("Corus.Admin.Transaction.Field.Insurance")]
        public bool Insurance { get; set; }

        [NopResourceDisplayName("Corus.Admin.Transaction.Field.PartialShipping")]
        public bool PartialShipping { get; set; }

        [NopResourceDisplayName("Corus.Admin.Transaction.Field.DeliveryTermId")]
        public int DeliveryTermId { get; set; }
        [NopResourceDisplayName("Corus.Admin.Transaction.Field.DeliveryTerm")]
        public string DeliveryTerm { get; set; }

        [NopResourceDisplayName("Corus.Admin.Transaction.Field.LoadingOrigin")]
        public string LoadingOrigin { get; set; }

        [NopResourceDisplayName("Corus.Admin.Transaction.Field.Destination")]
        public string Destination { get; set; }

        [UIHint("DateNullable")]
        [NopResourceDisplayName("Corus.Admin.Transaction.Field.LastDateOfShipment")]
        public DateTime LastDateOfShipment { get; set; }


        [NopResourceDisplayName("Corus.Admin.Transaction.Field.DocumentsRequirement")]
        public string DocumentsRequirement { get; set; }

        

         [NopResourceDisplayName("Corus.Admin.Transaction.Field.TransactionNumber")]
        public string TransactionNumber { get; set; }

        [NopResourceDisplayName("Corus.Admin.Transaction.Field.QuotationId")]
        public int QuotationId { get; set; }
        [NopResourceDisplayName("Corus.Admin.Transaction.Field.CaurisModeratorId")]
        public int? CaurisModeratorId { get; set; }
        [NopResourceDisplayName("Corus.Admin.Transaction.Field.CaurisModeratorName")]
        public string CaurisModeratorName { get; set; }
       
        
        [NopResourceDisplayName("Corus.Admin.Transaction.Field.Amount")]
        public decimal Amount { get; set; }
        public string PrimaryStoreCurrencyCode { get; set; }


        [NopResourceDisplayName("Corus.Admin.Transaction.Field.VendorDocumentId")]
        public int? VendorDocumentId { get; set; }
        [NopResourceDisplayName("Corus.Admin.Transaction.Field.CaurisDocumentId")]
        public int? CaurisDocumentId { get; set; }
        [NopResourceDisplayName("Corus.Admin.Transaction.Field.CustomerDocumentId")]
        public int? CustomerDocumentId { get; set; }
        [NopResourceDisplayName("Corus.Admin.Transaction.Field.PaymentProofDocumentId")]
        public int? PaymentProofDocumentId { get; set; }
        [NopResourceDisplayName("Corus.Admin.Transaction.Field.TransactionStatusId")]

        public int TransactionStatusId { get; set; }
        [NopResourceDisplayName("Corus.Admin.Transaction.Field.TransactionStatus")]
        public string TransactionStatus { get; set; }
        [NopResourceDisplayName("Corus.Admin.Transaction.Field.CreatedOnUtc")]
        public DateTime CreatedOnUtc { get; set; }
        [NopResourceDisplayName("Corus.Admin.Transaction.Field.UpdatedOnUtc")]
        public DateTime? UpdatedOnUtc { get; set; }


        [NopResourceDisplayName("Corus.Admin.Transaction.Field.ModeratorComment")]
        public string ModeratorComment { get; set; }


        [NopResourceDisplayName("Corus.Admin.Transaction.Field.DownloadLink")]
        public string DownloadLink { get; set; }

        [NopResourceDisplayName("Corus.Admin.Transaction.Field.ModeratorLastComment")]
        public string ModeratorLastComment { get; set; }
        //List Model
        public Guid DownloadGuid { get; set; }
        public string OrderStatus { get; set; }

        public string PaymentStatus { get; set; }
    }
}