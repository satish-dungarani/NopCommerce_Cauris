using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Transactions;
using Nop.Services;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Quotations;
using Nop.Services.Transactions;
using Nop.Services.Vendors;
using Nop.Web.Areas.Admin.Factories.Corus;
using Nop.Web.Areas.Admin.Models.Cauris.Transaction;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Web.Areas.Admin.Factories.Cours
{
    public class TransactionModelFactory : ITransactionModelFactory
    {
        private readonly ITransactionService _transactionService;
        private readonly IQuotationService _quotationService;
        private readonly IProductService _productService;
        private readonly ICustomerService _customerService;
        private readonly IVendorService _vendorService;
        private readonly ICountryService _countryService;
        private readonly IDownloadService _downloadService;
        private readonly IWorkContext _workContext;
        private readonly ILocalizationService _localizationService;
        private readonly ICurrencyService _currencyService;
        private readonly CurrencySettings _currencySettings;

        #region Ctor
        public TransactionModelFactory(ITransactionService transactionService, IQuotationService quotationService,
            IProductService productService, ICustomerService customerService, IVendorService vendorService, ICountryService countryService, IDownloadService downloadService, IWorkContext workContext, ILocalizationService localizationService
            , ICurrencyService currencyService, CurrencySettings currencySettings)
        {
            _transactionService = transactionService;
            _quotationService = quotationService;
            _productService = productService;
            _customerService = customerService;
            _vendorService = vendorService;
            _countryService = countryService;
            _downloadService = downloadService;
            _workContext = workContext;
            _localizationService = localizationService;
            _currencyService = currencyService;
            _currencySettings = currencySettings;
        }
        #endregion


        #region Utilites
        /// <summary>
        /// Prepare available order statuses
        /// </summary>
        /// <param name="items">Order status items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        public virtual void PrepareOrderStatuses(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //prepare available order statuses
            var availableStatusItems = TransactionStatus.Waiting_Upload_Contract.ToSelectList(false);
            foreach (var statusItem in availableStatusItems)
            {
                items.Add(statusItem);
            }

            //insert special item for the default value
            PrepareDefaultItem(items, withSpecialDefaultItem, defaultItemText);
        }

        /// <summary>
        /// Prepare default item
        /// </summary>
        /// <param name="items">Available items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use "All" text</param>
        protected virtual void PrepareDefaultItem(IList<SelectListItem> items, bool withSpecialDefaultItem, string defaultItemText = null)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //whether to insert the first special item for the default value
            if (!withSpecialDefaultItem)
                return;

            //at now we use "0" as the default value
            const string value = "0";

            //prepare item text
            defaultItemText ??= _localizationService.GetResource("Admin.Common.All");

            //insert this default item at first
            items.Insert(0, new SelectListItem { Text = defaultItemText, Value = value });
        }
        #endregion
        /// <summary>
        /// Prepare transaction search model
        /// </summary>
        /// <param name="searchModel">Transaction search model</param>
        /// <returns>Transaction search model</returns>
        public virtual TransactionSearchModel PrepareTransactionSearchModel(TransactionSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare available order, payment and shipping statuses
            //PrepareOrderStatuses(searchModel.AvailableTransactionStatus);
            //if (searchModel.AvailableTransactionStatus.Any())
            //{
            //    if (searchModel.TransactionStatusIds?.Any() ?? false)
            //    {
            //        var ids = searchModel.TransactionStatusIds.Select(id => id.ToString());
            //        searchModel.AvailableTransactionStatus.Where(statusItem => ids.Contains(statusItem.Value)).ToList()
            //            .ForEach(statusItem => statusItem.Selected = true);
            //    }
            //    else
            //        searchModel.AvailableTransactionStatus.FirstOrDefault().Selected = true;
            //}

            searchModel.AvailableTransactionStatus.Add(new SelectListItem { Text = "Select Status", Value = "0" });
            foreach (TransactionStatus ts in Enum.GetValues(typeof(TransactionStatus)))
            {
                searchModel.AvailableTransactionStatus.Add(new SelectListItem
                {
                    Text = ts.GetDescription(),
                    Value = ((int)ts).ToString()
                });
            }
            //if (searchModel.AvailablePaymentTypes.Any())
            //{
            //    if (searchModel.PaymentStatusIds?.Any() ?? false)
            //    {
            //        var ids = searchModel.PaymentStatusIds.Select(id => id.ToString());
            //        searchModel.AvailablePaymentTypes.Where(statusItem => ids.Contains(statusItem.Value)).ToList()
            //            .ForEach(statusItem => statusItem.Selected = true);
            //    }
            //    else
            //        searchModel.AvailablePaymentTypes.FirstOrDefault().Selected = true;
            //}
            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged transaction list model
        /// </summary>
        /// <param name="searchModel">Transaction search model</param>
        /// <returns>Transaction list model</returns>

        public virtual TransactionListModel PrepareTransactionListModel(TransactionSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));


            //get parameters to filter orders
            var transactionStatusIds = (searchModel.TransactionStatusIds?.Contains(0) ?? true) ? null : searchModel.TransactionStatusIds.ToList();
            var paymentStatusIds = (searchModel.PaymentStatusIds?.Contains(0) ?? true) ? null : searchModel.PaymentStatusIds.ToList();


            //get orders
            var transactions = _transactionService.SearchAdminTransaction(
                tsIds: transactionStatusIds,
                productId: searchModel.ProductId,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);


            var model = new TransactionListModel().PrepareToGrid(searchModel, transactions, () =>
            {
                return transactions.Select(transaction =>
                {
                    var quotation = _quotationService.Get(transaction.QuotationId);
                    var customer = _customerService.GetCustomerById(quotation.CustomerId);


                    var transModel = new TransactionModel();
                    transModel.QuotationId = transaction.QuotationId;
                    transModel.BuyerName = _customerService.GetCustomerFullName(customer);
                    transModel.ProductName = _productService.GetProductById(quotation.ProductId).Name;
                    transModel.TransactionNumber = transaction.TransactionNumber.ToString();
                    transModel.CaurisModeratorId = transaction.CaurisModeratorId;
                    transModel.DeliveryTermId = transaction.DeliveryTermId;
                    transModel.DeliveryTerm = _localizationService.GetLocalizedEnum(transaction.DeliveryTerm);
                    transModel.VendorDocumentId = transaction.VendorDocumentId;
                    transModel.CaurisDocumentId = transaction.CaurisDocumentId;
                    transModel.CustomerDocumentId = transaction.CustomerDocumentId;
                    transModel.PaymentProofDocumentId = transaction.PaymentProofDocumentId;
                    transModel.TransactionStatusId = transaction.TransactionStatusId;
                    transModel.TransactionStatus = transaction.TransactionStatus.GetDescription();
                    transModel.CreatedOnUtc = transaction.CreatedOnUtc;
                    transModel.UpdatedOnUtc = transaction.UpdatedOnUtc?.Date;
                    transModel.Id = transaction.Id;
                    transModel.ModeratorIdentity = transaction.ModeratorIndentity;

                    return transModel;
                });
            });

            return model;
        }


        /// <summary>
        /// Prepare transaction model
        /// </summary>
        /// <param name="model">transaction model</param>
        /// <param name="transaction">transaction</param>
        /// <returns>transaction model</returns>

        public virtual TransactionModel PrepareTransactionModel(TransactionModel model, Transaction transaction)
        {
            if (transaction != null)
            {
                var quotation = _quotationService.Get(transaction.QuotationId);
                var customer = _customerService.GetCustomerById(quotation.CustomerId);

                var transactionComment = _transactionService.GetTransactionCommentByTransactionId(transaction.Id);

                var transactionList = _transactionService.GetAllTransactionsCommentByTransactionId(transaction.Id);

                //fill in model values from the entity
                model ??= new TransactionModel();

                model.ModeratorIdentity = transaction.ModeratorIndentity;
                model.ContractSignatureDate = transaction.ContractSignatureDate;

                model.QuotationId = transaction.QuotationId;
                model.BuyerName = _customerService.GetCustomerFullName(customer);
                model.ProductName = _productService.GetProductById(quotation.ProductId).Name;
                model.SellerName = _vendorService.GetVendorById(quotation.VendorId)?.Name;

                model.Quantity = Convert.ToInt32(transaction.Quantity);
                model.Price = transaction.Amount;
                model.Tax = transaction.Tax;

                model.TransactionNumber = transaction.TransactionNumber.ToString();
                model.CaurisModeratorId = transaction.CaurisModeratorId;

                model.Inspection = transaction.Inspection;
                model.Insurance = transaction.Insurance;
                model.PartialShipping = transaction.PartialShipping;


                model.DeliveryTermId = transaction.DeliveryTermId;
                model.DeliveryTerm = _localizationService.GetLocalizedEnum(transaction.DeliveryTerm);

                model.VendorDocumentId = transaction.VendorDocumentId;
                model.CaurisDocumentId = transaction.CaurisDocumentId;
                model.CustomerDocumentId = transaction.CustomerDocumentId;
                model.PaymentProofDocumentId = transaction.PaymentProofDocumentId;

                model.TransactionStatusId = transaction.TransactionStatusId;
                model.TransactionStatus = _localizationService.GetLocalizedEnum(transaction.TransactionStatus);
                model.CreatedOnUtc = transaction.CreatedOnUtc.Date;
                model.UpdatedOnUtc = transaction.UpdatedOnUtc?.Date;


                model.Destination = transaction.Destination;
                model.LastDateOfShipment = transaction.LastDateOfShipment;
                model.Id = transaction.Id;
                model.ProductId = quotation.ProductId;

                model.PaymentTerm = transaction.PaymentTerm;
                model.LoadingOrigin = transaction.LoadingOrigin;
                model.DocumentsRequirement = transaction.DocumentsRequirement;

                model.ModeratorComment = transactionComment?.ModeratorComment;

                if (transactionList.Any())
                    model.ModeratorLastComment = transactionList?.LastOrDefault().ModeratorComment;
                if (transaction.CustomerDocumentId != null)
                    model.DownloadGuid = _downloadService.GetDownloadById(transaction.CustomerDocumentId ?? 0).DownloadGuid;

                model.PrimaryStoreCurrencyCode = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId).CurrencyCode;

            }
            model.AvailableTransactionStatus.Add(new SelectListItem { Text = "Select Delivery", Value = "0" });
            foreach (DeliveryTerm ts in Enum.GetValues(typeof(DeliveryTerm)))
            {
                model.AvailableTransactionStatus.Add(new SelectListItem
                {
                    Text = ts.GetDescription(),
                    Value = ((int)ts).ToString()
                });
            }
            return model;
        }
    }
}
