using System;
using System.Linq;
using Nop.Core;
using Nop.Services;
using Nop.Core.Domain.Quotations;
using Nop.Core.Domain.Transactions;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Quotations;
using Nop.Services.Transactions;
using Nop.Services.Vendors;
using Nop.Web.Framework.Models.Extensions;
using Nop.Web.Models.Transactions;
using Nop.Services.Common;

namespace Nop.Web.Factories
{
    public partial class TransactionModelFactory : ITransactionModelFactory
    {
        #region Field
        private readonly ITransactionService _transactionService;
        private readonly IQuotationService _quotationService;
        private readonly IProductService _productService;
        private readonly ICustomerService _customerService;
        private readonly IVendorService _vendorService;
        private readonly ICountryService _countryService;
        private readonly IDownloadService _downloadService;
        private readonly IWorkContext _workContext;
        private readonly ILocalizationService _localizationService;
        private readonly IAddressService _addressService;
        private readonly IPriceFormatter _priceFormatter;
        #endregion

        #region Ctor
        public TransactionModelFactory(ITransactionService transactionService, IQuotationService quotationService, 
            IProductService productService, ICustomerService customerService,
            IVendorService vendorService, ICountryService countryService, 
            IDownloadService downloadService, IWorkContext workContext, 
            ILocalizationService localizationService, IAddressService addressService,
            IPriceFormatter priceFormatter)
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
            _addressService = addressService;
            _priceFormatter = priceFormatter;
        }
        #endregion

        #region Methods
        public virtual TransactionSearchModel PrepareTransactionSearchModel(TransactionSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        public virtual TransactionListModel PrepareTransactionListModel(TransactionSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get polls
            var transactions = _transactionService.SearchTransactions(searchModel.Keyword,
                vendorId: (searchModel.IsReceived && _workContext.CurrentVendor != null) ? _workContext.CurrentVendor.Id : 0,
                customerId: (!searchModel.IsReceived) ? _workContext.CurrentCustomer.Id : 0,
                pageIndex: searchModel.Page - 1,
                pageSize: searchModel.PageSize);

            //prepare list model
            var model = new TransactionListModel().PrepareToGrid(searchModel, transactions, () =>
            {
                return transactions.Select(transaction =>
                {
                    var quote = _quotationService.Get(transaction.QuotationId);
                    //fill in model values from the entity
                    var transModel = new TransactionOverviewModel()
                    {
                        Id = transaction.Id,
                        QuotationId = transaction.QuotationId,
                        ProductName = _productService.GetProductById(quote.ProductId)?.Name,
                        CustomerName = _customerService.GetCustomerFullName(_customerService.GetCustomerById(quote.CustomerId)),
                        VendorName = _vendorService.GetVendorById(quote.VendorId)?.Name,
                        Country = _countryService.GetCountryById(quote.CountryId)?.Name,
                        Amount = transaction.Amount,
                        Quantity = quote.Quantity,
                        DispQuotationDate = quote.CreationDate.ToString("dd/MM/yyyy"),
                        TransactionNumber = transaction.TransactionNumber.ToString(),
                        CaurisModeratorId = transaction.CaurisModeratorId,
                        DeliveryTypeId = transaction.DeliveryTermId,
                        DeliveryType = _localizationService.GetLocalizedEnum(transaction.DeliveryTerm),

                        //PaymentTypeId = transaction.PaymentTypeId,
                        //PaymentType = _localizationService.GetLocalizedEnum(transaction.PaymentType),
                        //IncostetorDeliveryId = transaction.IncostetorDeliveryId,
                        //DeliveryLimitDays = transaction.DeliveryLimitDays,
                        //PaymentTypeId = transaction.PaymentTypeId,
                        //PaymentType = transaction.PaymentType.GetDescription(),
                        //DeliveryTypeId = transaction.DeliveryTypeId,
                        //DeliveryType = transaction.DeliveryType.GetDescription(),
                        //IncostetorDeliveryId = transaction.IncostetorDeliveryId,
                        //DeliveryLimitDays = transaction.DeliveryLimitDays,

                        VendorDocumentId = transaction.VendorDocumentId,
                        CaurisDocumentId = transaction.CaurisDocumentId,
                        CustomerDocumentId = transaction.CustomerDocumentId,
                        PaymentProofDocumentId = transaction.PaymentProofDocumentId,
                        TransactionStatusId = transaction.TransactionStatusId,
                        TransactionStatus = transaction.TransactionStatus.GetDescription(),
                        CreatedOnUtc = transaction.CreatedOnUtc.Date,
                        UpdatedOnUtc = transaction.UpdatedOnUtc?.Date
                    };

                    return transModel;
                });
            });

            return model;
        }

        public virtual TransactionOverviewModel PrepareTransactionOverviewModel(Transaction transaction, Quotation quotation)
        {
            var vendor = _vendorService.GetVendorById(quotation.VendorId);
            //fill in model values from the entity
            var trans = new TransactionOverviewModel()
            {
                Id = transaction.Id,
                QuotationId = transaction.QuotationId,
                ProductName = _productService.GetProductById(quotation.ProductId)?.Name,
                CustomerName = _customerService.GetCustomerFullName(_customerService.GetCustomerById(quotation.CustomerId)),
                VendorName = vendor?.Name,
                VendorCountry = _countryService.GetCountryByAddress(_addressService.GetAddressById(vendor.AddressId))?.Name,
                Country = _countryService.GetCountryById(quotation.CountryId)?.Name,
                Amount = transaction.Amount,
                Quantity = transaction.Quantity,
                DispQuotationDate = quotation.CreationDate.ToString("dd/MM/yyyy"),
                TransactionNumber = transaction.TransactionNumber.ToString(),
                CaurisModeratorId = transaction.CaurisModeratorId,
                DeliveryTypeId = transaction.DeliveryTermId,
                DeliveryType = _localizationService.GetLocalizedEnum(transaction.DeliveryTerm),

                //PaymentTypeId = transaction.PaymentTypeId,
                //PaymentType = _localizationService.GetLocalizedEnum(transaction.PaymentType),
                //IncostetorDeliveryId = transaction.IncostetorDeliveryId,
                //DeliveryLimitDays = transaction.DeliveryLimitDays,
                //PaymentTypeId = transaction.PaymentTypeId,
                //PaymentType = transaction.PaymentType.GetDescription(),
                //DeliveryTypeId = transaction.DeliveryTypeId,
                //DeliveryType = transaction.DeliveryType.GetDescription(),
                //IncostetorDeliveryId = transaction.IncostetorDeliveryId,
                //DeliveryLimitDays = transaction.DeliveryLimitDays,

                VendorDocumentId = transaction.VendorDocumentId,
                CaurisDocumentId = transaction.CaurisDocumentId,
                CustomerDocumentId = transaction.CustomerDocumentId,
                PaymentProofDocumentId = transaction.PaymentProofDocumentId,
                TransactionStatusId = transaction.TransactionStatusId,
                TransactionStatus = transaction.TransactionStatus.GetDescription(),
                CreatedOnUtc = transaction.CreatedOnUtc.Date,
                UpdatedOnUtc = transaction.UpdatedOnUtc?.Date,
                AmountFormat = _priceFormatter.FormatPrice(transaction.Amount),
                ModeratorComment = _transactionService.GetTransactionCommentByTransactionId(transaction.Id).ModeratorComment

            };

            //var transactionList = _transactionService.GetAllTransactionsCommentByTransactionId(transaction.Id);
            //if (transactionList.Any())
            //    trans.TransactionAdminComment = transactionList.LastOrDefault()?.ModeratorComment;

            if (transaction.CustomerDocumentId != null)
                trans.DownloadContractGuid = _downloadService.GetDownloadById(transaction.CustomerDocumentId ?? 0).DownloadGuid;

            return trans;
        }

        public virtual TransactionListModel PrepareAjaxTransactionListModel(DataTableAjaxPostModel ajaxModel, TransactionSearchModel searchModel)
        {

            var transactions = _transactionService.SearchTransactions2(searchModel.Name, searchModel.Country, searchModel.Date, searchModel.TransNumber, searchModel.Product, searchModel.Status
                , (searchModel.IsReceived/* && _workContext.CurrentVendor != null*/) ? _workContext.CurrentVendor.Id : 0, 0,
                (!searchModel.IsReceived) ? _workContext.CurrentCustomer.Id : 0, ajaxModel.Order[0].column, ajaxModel.Order[0].dir,
                pageIndex: (ajaxModel.Start / ajaxModel.Length),
                pageSize: ajaxModel.Length);

            //prepare model
            //prepare list model
            var model = new TransactionListModel().PrepareToGrid(searchModel, transactions, () =>
            {
                var tranList = transactions.Select(transaction =>
                {
                    var quote = _quotationService.Get(transaction.QuotationId);
                    var vendor = _vendorService.GetVendorById(quote.VendorId);
                    var transModel = new TransactionOverviewModel()
                    {
                        Id = transaction.Id,
                        TransactionNumber = transaction.TransactionNumber.ToString(),
                        DispQuotationDate = quote.CreationDate.ToString("dd/MM/yyyy"),
                        VendorName = vendor?.Name,
                        VendorCountry = _countryService.GetCountryByAddress(_addressService.GetAddressById(vendor.AddressId))?.Name,
                        CustomerName = _customerService.GetCustomerFullName(_customerService.GetCustomerById(quote.CustomerId)),
                        Country = _countryService.GetCountryById(quote.CountryId)?.Name,
                        ProductName = _productService.GetProductById(quote.ProductId)?.Name,
                        TransactionStatus = transaction.TransactionStatus.GetDescription(),
                        TransactionStatusId = transaction.TransactionStatusId
                    };
                    return transModel;
                });


                ////Search feature
                //if (!string.IsNullOrEmpty(searchModel.Name) && searchModel.IsReceived)
                //    tranList = tranList.Where(x => x.VendorName.Contains(searchModel.Name));

                //if (!string.IsNullOrEmpty(searchModel.Name) && !searchModel.IsReceived)
                //    tranList = tranList.Where(x => x.CustomerName.Contains(searchModel.Name));

                //if (!string.IsNullOrEmpty(searchModel.Country) && searchModel.IsReceived)
                //    tranList = tranList.Where(x => searchModel.Country.Contains(x.VendorCountry.ToLower()));

                //if (!string.IsNullOrEmpty(searchModel.Country) && !searchModel.IsReceived)
                //    tranList = tranList.Where(x => x.Country.Contains(searchModel.Country));

                //if (!string.IsNullOrEmpty(searchModel.Product))
                //    tranList = tranList.Where(x => x.ProductName.Contains(searchModel.Product));

                //if (!string.IsNullOrEmpty(searchModel.Date))
                //    tranList = tranList.Where(x => x.DispQuotationDate.Contains(searchModel.Date));

                //if (!string.IsNullOrEmpty(searchModel.TransNumber))
                //    tranList = tranList.Where(x => x.TransactionNumber.Contains(searchModel.TransNumber));

                //if (searchModel.Status != null && searchModel.Status > 0)
                //    tranList = tranList.Where(x => x.TransactionStatusId == Convert.ToInt32(searchModel.Status));

                ////Sorting feature
                //tranList = ajaxModel.Order[0].column switch
                //{
                //    1 => ajaxModel.Order[0].dir.Equals("asc") ? tranList.OrderBy(x => x.TransactionNumber) : tranList.OrderByDescending(x => x.TransactionNumber),
                //    2 => ajaxModel.Order[0].dir.Equals("asc") ? tranList.OrderBy(x => x.DispQuotationDate) : tranList.OrderByDescending(x => x.DispQuotationDate),
                //    3 => ajaxModel.Order[0].dir.Equals("asc") ? tranList.OrderBy(x => x.VendorName) : tranList.OrderByDescending(x => x.VendorName),
                //    4 => ajaxModel.Order[0].dir.Equals("asc") ? tranList.OrderBy(x => x.VendorCountry) : tranList.OrderByDescending(x => x.VendorCountry),
                //    5 => ajaxModel.Order[0].dir.Equals("asc") ? tranList.OrderBy(x => x.CustomerName) : tranList.OrderByDescending(x => x.CustomerName),
                //    6 => ajaxModel.Order[0].dir.Equals("asc") ? tranList.OrderBy(x => x.Country) : tranList.OrderByDescending(x => x.Country),
                //    7 => ajaxModel.Order[0].dir.Equals("asc") ? tranList.OrderBy(x => x.ProductName) : tranList.OrderByDescending(x => x.ProductName),
                //    8 => ajaxModel.Order[0].dir.Equals("asc") ? tranList.OrderBy(x => x.TransactionStatus) : tranList.OrderByDescending(x => x.TransactionStatus),
                //    _ => tranList.OrderByDescending(x => x.Id),
                //};
                return tranList;
            });

            return model;
        }
        #endregion
    }
}
