using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Cauris.Transaction
{
    public class TransactionSearchModel : BaseSearchModel
    {
        #region Ctor

        public TransactionSearchModel()
        {
            AvailableTransactionStatus = new List<SelectListItem>();
            AvailablePaymentTypes = new List<SelectListItem>();
            PaymentStatusIds = new List<int>();
            TransactionStatusIds = new List<int>();
        }

        #endregion

        #region Properties


        [NopResourceDisplayName("Admin.Cauris.List.TransactionStatus")]
        public IList<int> TransactionStatusIds { get; set; }

        [NopResourceDisplayName("Admin.Cauris.List.PaymentStatus")]
        public IList<int> PaymentStatusIds { get; set; }

        [NopResourceDisplayName("Admin.Cauris.List.CustomerEmail")]
        public string CustomerEmail { get; set; }
        [NopResourceDisplayName("Admin.Orders.List.Product")]
        public int ProductId { get; set; }

        [NopResourceDisplayName("Admin.Cauris.List.TransactionNumber")]
        public int TransactionNumber { get; set; }
        public IList<SelectListItem> AvailableTransactionStatus { get; set; }

        public IList<SelectListItem> AvailablePaymentTypes { get; set; }

        #endregion
    }
}
