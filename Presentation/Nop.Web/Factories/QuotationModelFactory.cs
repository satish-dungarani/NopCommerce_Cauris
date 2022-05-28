using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Notifications;
using Nop.Core.Domain.Quotations;
using Nop.Services;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Notifications;
using Nop.Services.Tax;
using Nop.Services.Vendors;
using Nop.Web.Framework.Models.Extensions;
using Nop.Web.Models.Catalog;
using Nop.Web.Models.Notifications;
using Nop.Web.Models.Quotation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Factories
{
    public partial class QuotationModelFactory : IQuotationModelFactory
    {
        #region Fields

        private readonly ICountryService _countryService;
        private readonly ICurrencyService _currencyService;
        private readonly ICustomerService _customerService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IFeesService _feesService;
        private readonly ILocalizationService _localizationService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IProductService _productService;
        private readonly IVendorService _vendorService;
        private readonly IWorkContext _workContext;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IProductModelFactory _productModelFactory;
        private readonly ICustomNotificationService _customNotificationService;

        #endregion

        #region Ctor

        public QuotationModelFactory(ICountryService countryService,
            ICurrencyService currencyService,
            ICustomerService customerService,
            IDateTimeHelper dateTimeHelper,
            IFeesService feesService,
            ILocalizationService localizationService,
            IPriceFormatter priceFormatter,
            IProductService productService,
            IVendorService vendorService,
            IWorkContext workContext,
            IPriceCalculationService priceCalculationService,
            IGenericAttributeService genericAttributeService,
            IProductModelFactory productModelFactory,
            ICustomNotificationService customNotificationService)
        {
            _countryService = countryService;
            _currencyService = currencyService;
            _customerService = customerService;
            _dateTimeHelper = dateTimeHelper;
            _feesService = feesService;
            _localizationService = localizationService;
            _priceFormatter = priceFormatter;
            _productService = productService;
            _vendorService = vendorService;
            _workContext = workContext;
            _priceCalculationService = priceCalculationService;
            _genericAttributeService = genericAttributeService;
            _productModelFactory = productModelFactory;
            _customNotificationService = customNotificationService;
        }

        #endregion

        #region Quotation Methods

        public QuotationModel PrepareCreateModel(int productId)
        {
            Customer customer = _workContext.CurrentCustomer;
            Product product = _productService.GetProductById(productId);
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            int countryId = _genericAttributeService.GetAttribute<int>(customer, NopCustomerDefaults.CountryIdAttribute);
            decimal price = _priceCalculationService.GetPriceWithCurrentCurrency(_priceCalculationService.GetFinalPrice(product, customer, includeDiscounts: false));

            QuotationModel quotationModel = new QuotationModel()
            {
                ProductId = productId,
                ProductName = product.Name,
                UnityPrice = price,
                CountryId = countryId,
                CustomerId = customer.Id,
                CustomerVendorId = _customerService.GetCustomerByVendorId(product.VendorId)?.Id ?? 0,
                VendorId = product.VendorId,
                Price = _priceCalculationService.GetPriceWithFees(price, 1),
                Quantity = 1,
                Status = (int)QuotationStatus.CustomerSend,
                Fees = _feesService.GetAll().Select(f => new FeesModel { Percent = f.Percent, Label = f.Label, Description = f.Description }),
                LeadTime = DateTime.Now,
               
            };
            var allowedQuantities = _productService.ParseAllowedQuantities(product);
            foreach (var qty in allowedQuantities)
            {
                quotationModel.AllowedQuantities.Add(new SelectListItem
                {
                    Text = qty.ToString(),
                    Value = qty.ToString(),
                    Selected = true
                });
            }

            quotationModel.AvailableCountries.Add(new SelectListItem { Text = _localizationService.GetResource("Address.SelectCountry"), Value = "0" });
            foreach (var c in _countryService.GetAllCountries(_workContext.WorkingLanguage.Id))
            {
                quotationModel.AvailableCountries.Add(new SelectListItem
                {
                    Text = _localizationService.GetLocalized(c, x => x.Name),
                    Value = c.Id.ToString(),
                    Selected = c.Id == countryId
                });
            }

            return quotationModel;
        }

        public QuotationListModel PrepareListModel(QuotationSearchModel searchModel, IEnumerable<Quotation> quotations, bool isVendor)
        {
            var currentCustomer = _workContext.CurrentCustomer;
            var quotationModels = new List<QuotationViewModel>();

            foreach (Quotation quotation in quotations)
            {
                var product = _productService.GetProductById(quotation.ProductId);
                var price = _priceCalculationService.GetPriceWithCurrentCurrency(_priceCalculationService.GetFinalPrice(product, currentCustomer, includeDiscounts: false));
                var country = _countryService.GetCountryById(quotation.CountryId);
                var customer = _customerService.GetCustomerById(quotation.CustomerId);
                var productDetails = _productModelFactory.PrepareProductDetailsModel(product);
                var vendor = _vendorService.GetVendorById(quotation.VendorId);
                var viewModel = new QuotationViewModel
                {
                    QuotationId = quotation.Id,
                    ProductDetails = productDetails,
                    CustomerFirstName = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.FirstNameAttribute),
                    CustomerLastName = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.LastNameAttribute),
                    CustomerEmail = customer.Email,
                    Country = _localizationService.GetLocalized(country, x => x.Name),
                    Quantity = quotation.Quantity,
                    UnityPrice = price,
                    Price = _priceCalculationService.GetPriceWithCurrentCurrency(_priceCalculationService.GetPriceWithFees(price, quotation.Quantity)),
                    LeadTime = quotation.LeadTime.ToShortDateString(),
                    Status = ParseQuotationStaus(quotation.Status, isVendor),
                    ProductName = productDetails.Name,
                    StatusId = (int)quotation.Status,
                    VendorName = vendor.Name
                };
                quotationModels.Add(viewModel);
            }
            if (searchModel.Id.HasValue && searchModel.Id.Value> 0)
            {
                quotationModels = quotationModels.Where(x => x.Id == searchModel.Id).ToList();
            }

            if (!string.IsNullOrEmpty(searchModel.Name?.Trim().ToLower()) && !searchModel.Received)
            {
                quotationModels = quotationModels.Where(x => x.VendorName?.ToLower().Contains(searchModel.Name) == true).ToList() ;
            }
            if (!string.IsNullOrEmpty(searchModel.Name?.Trim().ToLower()) && searchModel.Received)
            {
                quotationModels = quotationModels.Where(x => x.CustomerFullName?.ToLower().Contains(searchModel.Name) == true).ToList();
            }
            if (!string.IsNullOrEmpty(searchModel.LeadDateSearch?.Trim().ToLower()))
            {
                

                quotationModels = quotationModels.Where(x => DateTime.Parse(x.LeadTime)  == DateTime.Parse(searchModel.LeadDateSearch)).ToList();
            }
            if (!string.IsNullOrEmpty(searchModel.Product?.Trim().ToLower()))
            {
                quotationModels = quotationModels.Where(x => x.ProductName?.ToLower().Contains(searchModel.Product) == true).ToList();
            }
            if (searchModel.Status>0)
            {
                quotationModels = quotationModels.Where(x => x.StatusId == searchModel.Status).ToList();
            }

            //prepare grid model
            var model = new QuotationListModel().PrepareToGrid(searchModel, quotationModels.ToPagedList(searchModel), () =>
            {
                //fill in model values from the entity
                return quotationModels;
            });
            return model;
        }

        public QuotationModel PrepareEditModel(Quotation quotation)
        {
            bool isVendor = _workContext.CurrentVendor != null && _workContext.CurrentVendor.Id == quotation.VendorId;
            Product product = _productService.GetProductById(quotation.ProductId);
            decimal price = _priceCalculationService.GetPriceWithCurrentCurrency(_priceCalculationService.GetFinalPrice(product, _workContext.CurrentCustomer, includeDiscounts: false));
            Customer customer = _customerService.GetCustomerById(quotation.CustomerId);
            string firstName = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.FirstNameAttribute);
            string lastName = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.LastNameAttribute);
            QuotationModel quotationModel = new QuotationModel()
            {
                QuotationId = quotation.Id,
                ProductId = quotation.ProductId,
                ProductName = product.Name,
                UnityPrice = price,
                CountryId = quotation.CountryId,
                Country = _countryService.GetCountryById(quotation.CountryId)?.Name,
                CustomerId = quotation.CustomerId,
                CustomerFullName = firstName + " " + lastName,
                VendorId = product.VendorId,
                CustomerVendorId = _customerService.GetCustomerByVendorId(product.VendorId)?.Id ?? 0,
                Price = _priceCalculationService.GetPriceWithCurrentCurrency(quotation.Price),
                Quantity = quotation.Quantity,
                Status = (int)quotation.Status,
                StatusText = ParseQuotationStaus(quotation.Status, isVendor),
                Fees = _feesService.GetAll().Select(f => new FeesModel { Percent = f.Percent, Label = f.Label, Description = f.Description }),
                LeadTime = quotation.LeadTime,
               
                IsVendor = isVendor,
                CreationDate = quotation.CreationDate.ToShortDateString(),
                Specification = quotation.Specification
            };
            var allowedQuantities = _productService.ParseAllowedQuantities(product);
            foreach (var qty in allowedQuantities)
            {
                quotationModel.AllowedQuantities.Add(new SelectListItem
                {
                    Text = qty.ToString(),
                    Value = qty.ToString(),
                    Selected = true
                });
            }

            quotationModel.PreviousQuotationStatus = GetPreviousStatus(quotationModel.Status);

            quotationModel.NextQuotationStatus = GetNexStatus(quotationModel.Status);

          


            return quotationModel;
        }

        private IList<SelectListItem> GetNexStatus(int status)
        {
            var nextStatus = new List<SelectListItem>();
          
                switch (status)
                {
                    case (int)QuotationStatus.CustomerSend:
                        nextStatus.Add(new SelectListItem
                        {
                            Text = QuotationStatus.VendorSend.GetDescription(),
                            Value = ((int)status).ToString()
                        });
                        nextStatus.Add(new SelectListItem
                        {
                            Text = QuotationStatus.CustomerAccept.GetDescription(),
                            Value = ((int)status).ToString()
                        });
                        break;
                    case (int)QuotationStatus.ModerationPending:
                        nextStatus.Add(new SelectListItem
                        {
                            Text = QuotationStatus.VendorSend.GetDescription(),
                            Value = ((int)status).ToString()
                        });
                        break;
                    case (int)QuotationStatus.VendorSend:
                        nextStatus.Add(new SelectListItem
                        {
                            Text = QuotationStatus.CustomerAccept.GetDescription(),
                            Value = ((int)status).ToString()
                        });
                        break;
                    case (int)QuotationStatus.CustomerAccept:
                       
                        break;

                }

            return nextStatus;
        }

        private IList<SelectListItem> GetPreviousStatus(int status)
        {
            var previous = new List<SelectListItem>();
            switch (status)
            {
                case (int)QuotationStatus.CustomerSend:
                    break;
                case (int)QuotationStatus.ModerationPending:
                case (int)QuotationStatus.VendorSend:
                    previous.Add(new SelectListItem
                    {
                        Text = QuotationStatus.CustomerSend.GetDescription(),
                        Value = ((int)status).ToString()
                    });
                    break;
                case (int)QuotationStatus.CustomerAccept:
                    previous.Add(new SelectListItem
                    {
                        Text = QuotationStatus.CustomerSend.GetDescription(),
                        Value = ((int)status).ToString()
                    });
                   
                    previous.Add(new SelectListItem
                    {
                        Text = QuotationStatus.VendorSend.GetDescription(),
                        Value = ((int)status).ToString()
                    });
                    break;

            }
            return previous;

        }

        public string ParseQuotationStaus(int status, bool isVendor)
        {
            switch (status)
            {
                case (int)QuotationStatus.CustomerSend:
                    return _localizationService.GetResource(isVendor ? "Quote.Request.Received" : "Quote.Request.Sended");
                case (int)QuotationStatus.ModerationPending:
                    return _localizationService.GetResource("Quote.BeingProcessed");
                case (int)QuotationStatus.VendorSend:
                    return _localizationService.GetResource(isVendor ? "Quote.Response.Sended" : "Quote.Response.Received");
                case (int)QuotationStatus.CustomerRefuse:
                    return _localizationService.GetResource("Quote.Refused");
                case (int)QuotationStatus.CustomerAccept:
                    return _localizationService.GetResource("Quote.Accepted");
                case (int)QuotationStatus.ModeratorRefuse:
                    return _localizationService.GetResource("Quote.Moderator.Refused");
                default:
                    return string.Empty;
            }
        }

        public QuotationListModel PrepareListModelModeration(QuotationSearchModel searchModel, IEnumerable<Quotation> quotations)
        {
            Customer currentCustomer = _workContext.CurrentCustomer;
            List<QuotationViewModel> quotationModels = new List<QuotationViewModel>();

            foreach (Quotation quotation in quotations)
            {
                Product product = _productService.GetProductById(quotation.ProductId);
                decimal price = _priceCalculationService.GetPriceWithCurrentCurrency(_priceCalculationService.GetFinalPrice(product, currentCustomer, includeDiscounts: false));
                Country country = _countryService.GetCountryById(quotation.CountryId);
                Customer customer = _customerService.GetCustomerById(quotation.CustomerId);
                ProductDetailsModel productDetails = _productModelFactory.PrepareProductDetailsModel(product);
                QuotationViewModel viewModel = new QuotationViewModel
                {
                    QuotationId = quotation.Id,
                    ProductDetails = productDetails,
                    CustomerFirstName = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.FirstNameAttribute),
                    CustomerLastName = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.LastNameAttribute),
                    CustomerEmail = customer.Email,
                    Country = _localizationService.GetLocalized(country, x => x.Name),
                    Quantity = quotation.Quantity,
                    UnityPrice = price,
                    Price = _priceCalculationService.GetPriceWithCurrentCurrency(_priceCalculationService.GetPriceWithFees(price, quotation.Quantity)),
                    LeadTime = quotation.LeadTime.ToShortDateString(),
                    ProductName = productDetails.Name,
                    Specification = quotation.Specification
                };
                quotationModels.Add(viewModel);
            }
            string keyword = searchModel.Keyword?.Trim().ToLower();
            if (!string.IsNullOrEmpty(keyword))
            {
                quotationModels = quotationModels.Where(x => x.CustomerFullName?.ToLower().Contains(keyword) == true
                                                           || x.ProductName?.ToLower().Contains(keyword) == true
                                                           || x.Status?.ToLower().Contains(keyword) == true
                                                           || x.Specification?.ToLower().Contains(keyword) == true).ToList();
            }
            //prepare grid model
            var model = new QuotationListModel().PrepareToGrid(searchModel, quotationModels.ToPagedList(searchModel), () =>
            {
                //fill in model values from the entity
                return quotationModels;
            });
            return model;
        }

        public QuotationSearchModel PrepareSearchModeration(QuotationSearchModel quotationSearchModel)
        {
            if(quotationSearchModel == null)
            {
                quotationSearchModel = new QuotationSearchModel();
            }
            quotationSearchModel.SetGridPageSize();
            return quotationSearchModel;
        }


        #endregion

        #region Notification

        public List<NotificationModel> PrepareNotificationModel()
        {
            var customer = _workContext.CurrentCustomer;
            var notifications = _customNotificationService.GetAllNotification()
                .Where(x => x.IsRead == false && x.CreatedFor == customer.Id)
                .ToList()
                .OrderByDescending(x=>x.Id).Take(3);

            List<NotificationModel> model = new List<NotificationModel>();

            foreach (var item in notifications)
            {
                var notificationModel = new NotificationModel();

                notificationModel.Id = item.Id;
                notificationModel.EntityId = item.EntityId;
                notificationModel.EntityName = item.EntityName;
                notificationModel.CreatedBy = item.CreatedBy;
                notificationModel.CreatedByName = _customerService.GetCustomerFullName(_customerService.GetCustomerById(item.CreatedBy));
                notificationModel.CreatedForName = _customerService.GetCustomerFullName(_customerService.GetCustomerById(item.CreatedFor));
                notificationModel.CreatedFor = item.CreatedFor;
                notificationModel.IsRead = item.IsRead;
                notificationModel.Description = item.Description;
                notificationModel.CreatedDate = Convert.ToDateTime(item.CreatedDate.ToString("dd-MMM-yyyy"));
                

                model.Add(notificationModel);
            }
            return model;
        }

        public NotificationListModel PrepareNotificationListModel(NotificationSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            var customer = _workContext.CurrentCustomer;

            //get categories
            var notifications = _customNotificationService.GetAllNotification().Where(x => x.IsRead == false && x.CreatedFor == customer.Id).ToList().ToPagedList(searchModel);

            //prepare grid model
            var model = new NotificationListModel().PrepareToGrid(searchModel, notifications, () =>
            {
                return notifications.Select(notification =>
                {
                    //fill in model values from the entity
                    var notificationModel = new NotificationModel();

                    notificationModel.Id = notification.Id;
                    notificationModel.EntityId = notification.EntityId;
                    notificationModel.EntityName = notification.EntityName;
                    notificationModel.CreatedBy = notification.CreatedBy;
                    notificationModel.CreatedFor = notification.CreatedFor;
                    notificationModel.CreatedByName = _customerService.GetCustomerFullName(_customerService.GetCustomerById(notification.CreatedBy));
                    notificationModel.CreatedForName = _customerService.GetCustomerFullName(_customerService.GetCustomerById(notification.CreatedFor));
                    notificationModel.Description = notification.Description;
                    notificationModel.IsRead = notification.IsRead;
                    //notificationModel.CreatedDate = notification.CreatedDate;
                    notificationModel.CreatedDate = Convert.ToDateTime(notification.CreatedDate.ToString("dd-MMM-yyyy"));

                    return notificationModel;
                });
            });

            return model;
        }

        public NotificationSearchModel PrepareNotificationSearchModel(NotificationSearchModel searchModel)
        {
            NotificationSearchModel model = new NotificationSearchModel();

            model.SetGridPageSize();

            return model;
        }

        public List<QuotationViewModel> PrepareListModel(IOrderedEnumerable<Quotation> quotations , bool isVendor)
        {
            var currentCustomer = _workContext.CurrentCustomer;
            var quotationModels = new List<QuotationViewModel>();

            foreach (Quotation quotation in quotations)
            {
                var product = _productService.GetProductById(quotation.ProductId);
                var price = _priceCalculationService.GetPriceWithCurrentCurrency(_priceCalculationService.GetFinalPrice(product, currentCustomer, includeDiscounts: false));
                var country = _countryService.GetCountryById(quotation.CountryId);
                var customer = _customerService.GetCustomerById(quotation.CustomerId);
                var vendor = _vendorService.GetVendorById(quotation.VendorId);
                var productDetails = _productModelFactory.PrepareProductDetailsModel(product);
                var viewModel = new QuotationViewModel
                {
                    QuotationId = quotation.Id,
                    ProductDetails = productDetails,
                    CustomerFirstName = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.FirstNameAttribute),
                    CustomerLastName = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.LastNameAttribute),
                    CustomerEmail = customer.Email,
                    Country = _localizationService.GetLocalized(country, x => x.Name),
                    Quantity = quotation.Quantity,
                    UnityPrice = price,
                    Price = _priceCalculationService.GetPriceWithCurrentCurrency(_priceCalculationService.GetPriceWithFees(price, quotation.Quantity)),
                    LeadTime = quotation.LeadTime.ToShortDateString(),
                    Status = ParseQuotationStaus(quotation.Status, isVendor),
                    ProductName = productDetails.Name,
                    StatusId = (int)quotation.Status,
                    VendorName = vendor.Name
                };
                quotationModels.Add(viewModel);
            }
          
            return quotationModels;
        }

        #endregion
    }
}
