using System;
using System.ComponentModel;

namespace Nop.Core.Domain.Transactions
{
    public partial class Transaction : BaseEntity
    {
        public Transaction()
        {
            TransactionNumber = Guid.NewGuid();
        }
        public Guid TransactionNumber { get; set; }

        public string ModeratorIndentity { get; set; }

        public DateTime ContractSignatureDate { get; set; }

        public int QuotationId { get; set; }
     
        public int Quantity { get; set; }

        public int? CaurisModeratorId { get; set; }

        public decimal Amount { get; set; }

        public decimal Tax { get; set; }
        public string PaymentTerm { get; set; }

        public bool Inspection { get; set; }

        public bool Insurance { get; set; }

        public bool PartialShipping { get; set; }

        public int DeliveryTermId { get; set; }

        public string LoadingOrigin { get; set; }
        public string Destination { get; set; }

        public DateTime LastDateOfShipment { get; set; }

        public string DocumentsRequirement { get; set; }
     

        public int TransactionStatusId { get; set; }

        public int? VendorDocumentId { get; set; }
        public int? CaurisDocumentId { get; set; }
        public int? CustomerDocumentId { get; set; }
        public int? PaymentProofDocumentId { get; set; }

        public DateTime CreatedOnUtc { get; set; }
        public DateTime? UpdatedOnUtc { get; set; }

        public TransactionStatus TransactionStatus
        {
            get => (TransactionStatus)TransactionStatusId;
            set => TransactionStatusId = (int)value;
        }
        //public PaymentType PaymentType
        //{
        //    get => (PaymentType)PaymentTypeId;
        //    set => PaymentTypeId = (int)value;
        //}
        public DeliveryTerm DeliveryTerm
        {
            get => (DeliveryTerm)DeliveryTermId;
            set => DeliveryTermId = (int)value;
        }
    }

    public enum TransactionStatus
    {
        [Description("Waiting Upload Contract")]
        Waiting_Upload_Contract = 10,
        [Description("Waiting Validation Contract By Moderator")]
        Waiting_Validation_Contract_By_Moderator = 20,
        [Description("Waiting Seller Proof Of Payment")]
        Waiting_Seller_Proof_Of_Payment = 30,
        [Description("Waiting For Funds Available")]
        Waiting_For_Funds_Available = 40,
        [Description("Waiting Proof Vendor Delivery")]
        Waiting_Proof_Vendor_Delivery = 50,
        [Description("Waiting Proof Receipt")]
        Waiting_Proof_Receipt = 60,
        [Description("Closed")]
        Closed = 70,
        [Description("Waiting proof of payment Buyer")]
        Waiting_Proof_Payment_Buyer = 80,
        [Description("Waiting for cauris transfer")]
        Waiting_For_Cauris_Transfer = 90,
        
    }

    public enum PaymentType
    {
        BankTransfer = 10,

        Online = 20,

        Manually = 30,
    }

    public enum DeliveryTerm
    {
        [Description("Free Alongside Ship")]
        Free_Alongside_Ship = 10,
        [Description("Free On Board")]
        Free_On_Board = 20,
        [Description("Cost And Freight")]
        Cost_And_Freight = 30,
        [Description("Insurance And Freight")]
        Insurance_And_Freight = 40,
        [Description("Delivered At Place")]
        Delivered_At_Place = 50,
        [Description("Carriage And Insurance Paid To")]
        Carriage_And_Insurance_Paid_To = 60,
        [Description("Delivered At Place Unloaded")]
        Delivered_At_Place_Unloaded = 70,
        [Description("Delivered Duty Paid")]
        Delivered_Duty_Paid = 80
    }
}