using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Notifications;
using Nop.Core.Domain.Quotations;
using Nop.Core.Domain.Transactions;
using Nop.Services;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Notifications;
using Nop.Services.Quotations;
using Nop.Services.Transactions;
using Nop.Web.Factories;
using Nop.Web.Framework.Validators;
using Nop.Web.Models.Catalog;
using Nop.Web.Models.Notifications;
using Nop.Web.Models.Quotation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class QuotationController : BasePublicController
    {
        #region Fields
        private readonly IQuotationService _quotationService;
        private readonly IWorkContext _workContext;
        private readonly ILocalizationService _localizationService;
        private readonly IQuotationModelFactory _quotationModelFactory;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly IProductService _productService;
        private readonly ISettingService _settingSetvice;
        private readonly ICustomerService _customerService;
        private readonly ICurrencyService _currencyService;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly ITransactionService _transactionService;
        private readonly ICustomNotificationService _customNotificationService;
        private readonly ICountryService _countryService;

        #endregion

        #region Ctor
        public QuotationController(IQuotationService quotationService,
            IWorkContext workContext,
            ILocalizationService localizationService,
            IQuotationModelFactory quotationModelFactory,
            IPriceCalculationService priceCalculationService,
            IProductService productService,
            ISettingService settingSetvice,
            ICustomerService customerService,
            ICurrencyService currencyService,
            IWorkflowMessageService workflowMessageService,
            ITransactionService transactionService,
            ICustomNotificationService customNotificationService,
            ICountryService countryService)
        {
            _quotationService = quotationService;
            _workContext = workContext;
            _localizationService = localizationService;
            _quotationModelFactory = quotationModelFactory;
            _priceCalculationService = priceCalculationService;
            _productService = productService;
            _settingSetvice = settingSetvice;
            _workflowMessageService = workflowMessageService;
            _customerService = customerService;
            _currencyService = currencyService;
            _transactionService = transactionService;
            _customNotificationService = customNotificationService;
            _countryService = countryService;
        }
        #endregion

        #region Methodes
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Details(int id)
        {
            return View();
        }

        [HttpGet]
        public ActionResult RequestQuotation(int productId)
        {
            if (!_customerService.IsRegistered(_workContext.CurrentCustomer))
                return Challenge();
            QuotationModel quotationModel = _quotationModelFactory.PrepareCreateModel(productId);
            var quotation = _quotationService.GetByCustomerIdProductId(_workContext.CurrentCustomer.Id, productId);
            if (quotation != null && quotation.Status != (int)QuotationStatus.CustomerRefuse && quotation.Status != (int)QuotationStatus.CustomerAccept)
            {
                quotationModel.Result = _localizationService.GetResource("Quotation.RequestQuotation.RequestQuotationAlreadySended");
                quotationModel.AlreadyExist = true;
            }
            return PartialView("_RequestQuotationButton", quotationModel);

        }


        public JsonResult CreateRequestQuotation(int productId, int quantity, DateTime leadDate, int countryId, string msgDetails, string unityPrice, int vendorId)
        {
            var productDetail = _productService.GetProductById(productId);
            var price = Convert.ToDecimal(unityPrice.Replace(":", ""));
            var unityPriceWithStoreCurrency = _priceCalculationService.GetFinalPrice(productDetail, _workContext.CurrentCustomer, includeDiscounts: false);
            bool hasBlackListTerms = !string.IsNullOrEmpty(msgDetails) && _settingSetvice.GetSettingByKey<string>("blacklisttext").Contains(msgDetails);
            Quotation quotation = new Quotation
            {
                CountryId = countryId,
                CreationDate = DateTime.Now,
                CustomerId = _workContext.CurrentCustomer.Id,
                LeadTime = leadDate,
                Price = _priceCalculationService.GetPriceWithFees(unityPriceWithStoreCurrency, quantity),
                ProductId = productId,
                Quantity = quantity,
                Specification = msgDetails,
                Status = hasBlackListTerms ? (int)QuotationStatus.ModerationPending : (int)QuotationStatus.CustomerSend,//TODO Moderation
                UnitPrice = price,
                StatusDate = DateTime.Now,
                VendorId = vendorId

            };
            _quotationService.Save(quotation);

            var notification = new Notification
            {
                EntityId = quotation.Id,
                EntityName = "Quotation",
                CreatedBy = _workContext.CurrentCustomer.Id,
                CreatedFor = _customerService.GetCustomerByVendorId(quotation.VendorId).Id,
                IsRead = false,
                Description = string.Format(_localizationService.GetResource("Custom.Code.Notification.Customer.Send.Quotation.Request"), _workContext.CurrentCustomer.Email),
                CreatedDate = DateTime.UtcNow
            };
            _customNotificationService.InsertNotification(notification);

            if (quotation != null && quotation.Id > 0)
            {
                QuotationMessage quotationMessage = new QuotationMessage
                {
                    Id = quotation.Id,
                    CustomerId = quotation.CustomerId,
                    ProductId = quotation.ProductId,
                    VendorId = quotation.VendorId
                };
                //send request quote customer notification
                _workflowMessageService.SendRequestQuoteCustomerNotification(quotationMessage, _workContext.WorkingLanguage.Id);

                //send request quote vendor notification
                _workflowMessageService.SendRequestQuoteVendorNotification(quotationMessage, _workContext.WorkingLanguage.Id);
            }
            return Json(new
            {
                success = true,

                message = string.Format(_localizationService.GetResource("Quotation.Request.SentLink"), Url.RouteUrl("SendedQuotes"))
            });
        }

        public ActionResult Delete(int id)
        {
            return View();
        }


        public ActionResult Received()
        {
            if (!_customerService.IsRegistered(_workContext.CurrentCustomer))
                return Challenge();

            var model = _quotationModelFactory.PrepareSearchModeration(new QuotationSearchModel()
            {
                Received = true
            });
            model.AvailableCountries.Add(new SelectListItem { Text = _localizationService.GetResource("Address.SelectCountry"), Value = "0" });
            foreach (var c in _countryService.GetAllCountries(_workContext.WorkingLanguage.Id))
            {
                model.AvailableCountries.Add(new SelectListItem
                {
                    Text = _localizationService.GetLocalized(c, x => x.Name),
                    Value = c.Id.ToString()
                });
            }
            model.AvailableQuotationStatus.Add(new SelectListItem { Text = "Select Status", Value = "0" });
            foreach (QuotationStatus status in Enum.GetValues(typeof(QuotationStatus)))
            {
                var statusTransformation = _quotationModelFactory.ParseQuotationStaus((int)status, model.Received);
                model.AvailableQuotationStatus.Add(new SelectListItem
                {
                    Text = statusTransformation,
                    Value = ((int)status).ToString()
                });
            }
            return View("List", model);
        }

        public ActionResult Sended()
        {
            if (!_customerService.IsRegistered(_workContext.CurrentCustomer))
                return Challenge();

            var model = _quotationModelFactory.PrepareSearchModeration(new QuotationSearchModel()
            {
                Received =false
            });
            model.AvailableCountries.Add(new SelectListItem { Text = _localizationService.GetResource("Address.SelectCountry"), Value = "0" });
            foreach (var c in _countryService.GetAllCountries(_workContext.WorkingLanguage.Id))
            {
                model.AvailableCountries.Add(new SelectListItem
                {
                    Text = _localizationService.GetLocalized(c, x => x.Name),
                    Value = c.Id.ToString()
                });
            }
            model.AvailableQuotationStatus.Add(new SelectListItem { Text = "Select Status", Value = "0" });
            foreach (QuotationStatus status in Enum.GetValues(typeof(QuotationStatus)))
            {
                var statusTransformation = _quotationModelFactory.ParseQuotationStaus((int)status, model.Received);
                model.AvailableQuotationStatus.Add(new SelectListItem
                {
                    Text = statusTransformation,
                    Value = ((int)status).ToString()
                });
            }
            return View("List", model) ;
        }
       
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public IActionResult ListReceived([Validate] QuotationSearchModel searchModel)
        {
            var quotations = _quotationService.GetAllByVendorId(_workContext.CurrentVendor.Id).Where(x => x.CustomerId != _workContext.CurrentCustomer.Id);
            var model = _quotationModelFactory.PrepareListModel(searchModel, quotations.Where(x => x.Status != (int)QuotationStatus.ModerationPending).OrderBy(x => x.Status), true);
            return Json(model);
        }
        
        
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public virtual IActionResult ListSended([Validate] QuotationSearchModel searchModel)
        {
            var quotations = _quotationService.GetAllByCustomerId(_workContext.CurrentCustomer.Id);
            var model = _quotationModelFactory.PrepareListModel(searchModel, quotations.OrderBy(x => x.Status), searchModel.Received);
            return Json(model);
        }

         public IActionResult ListSended()
        {
            var quotations = _quotationService.GetAllByCustomerId(_workContext.CurrentCustomer.Id);
            var model = _quotationModelFactory.PrepareListModel(quotations.OrderBy(x => x.Status),false);
            return Json(model);
        }
        public IActionResult ListReceived()
        {
            var quotations = _quotationService.GetAllByVendorId(_workContext.CurrentVendor.Id).Where(x => x.CustomerId != _workContext.CurrentCustomer.Id);
            var model = _quotationModelFactory.PrepareListModel(quotations.OrderBy(x => x.Status), true);
            return Json(model);
        }
        public JsonResult CalculatePriceForQuotation(int productId, string quantity)
        {
            Product product = _productService.GetProductById(productId);
            decimal price = _priceCalculationService.GetPriceWithCurrentCurrency(_priceCalculationService.GetFinalPrice(product, _workContext.CurrentCustomer, includeDiscounts: false));
            int.TryParse(quantity, out int quantityValue);
            return Json(new { TotalPriceHT = $"{Math.Round(price * quantityValue, 2)}", TotalPriceTTC = $"{_priceCalculationService.GetPriceWithFees(price, quantityValue)}" });
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            Quotation quotation = _quotationService.Get(id);
            if (quotation.CustomerId != _workContext.CurrentCustomer.Id && quotation.VendorId != _workContext.CurrentVendor?.Id)
            {
                return AccessDeniedView();
            }
            return View(_quotationModelFactory.PrepareEditModel(quotation));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult VendorResponse(QuotationModel model)
        {
            try
            {
                Quotation quotation = _quotationService.Get(model.QuotationId);
                bool isVendor = _workContext.CurrentVendor != null && _workContext.CurrentVendor.Id == quotation.VendorId;
                if (isVendor)
                {
                    quotation.Status = (int)QuotationStatus.VendorSend;
                    quotation.Price = _currencyService.ConvertToPrimaryStoreCurrency(model.Price, _workContext.WorkingCurrency);
                    if (model.NewLeadTime.HasValue && model.NewLeadTime != DateTime.MinValue)
                    {
                        quotation.LeadTime = model.NewLeadTime.Value;
                    }
                    _quotationService.Save(quotation);

                    var notification = new Notification
                    {
                        EntityId = quotation.Id,
                        EntityName = "Quotation",
                        CreatedBy = _customerService.GetCustomerByVendorId(quotation.VendorId).Id,
                        CreatedFor = quotation.CustomerId,
                        IsRead = false,
                        Description = _localizationService.GetResource("Custom.Code.Notification.Vendor.Send.Quotation.Response"),
                        CreatedDate = DateTime.UtcNow
                    };
                    _customNotificationService.InsertNotification(notification);

                    return RedirectToAction(nameof(Received));
                }
                return RedirectToAction(nameof(Sended));
            }
            catch
            {
                return View(model);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CustomerAccept(QuotationModel model)
        {
            try
            {
                var quotation = _quotationService.Get(model.QuotationId);
                if (quotation.CustomerId == _workContext.CurrentCustomer.Id)
                {
                    quotation.Status = (int)QuotationStatus.CustomerAccept;
                    _quotationService.Save(quotation);
                    var transaction = new Transaction()
                    {
                        TransactionNumber = Guid.NewGuid(),
                        QuotationId = quotation.Id,
                        ModeratorIndentity = "XI" + CommonHelper.GenerateRandomDigitCode(7),
                        DeliveryTermId = (int)DeliveryTerm.Free_Alongside_Ship,
                        TransactionStatusId = (int)TransactionStatus.Waiting_Upload_Contract,
                        Quantity = Convert.ToInt32(quotation.Quantity),
                        Amount = quotation.Price,
                        CreatedOnUtc = DateTime.UtcNow,
                        ContractSignatureDate= DateTime.UtcNow,
                        LastDateOfShipment = DateTime.UtcNow
                    };
                    _transactionService.SaveTransaction(transaction);

                    var notification = new Notification
                    {
                        EntityId = transaction.Id,
                        EntityName = "Transaction",
                        CreatedBy = _workContext.CurrentCustomer.Id,
                        CreatedFor = _customerService.GetCustomerByVendorId(quotation.VendorId).Id,
                        IsRead = false,
                        Description = string.Format(_localizationService.GetResource("Custom.Code.Notification.Customer.Accept.Response"), _workContext.CurrentCustomer.Email),
                        CreatedDate = DateTime.UtcNow
                    };
                    _customNotificationService.InsertNotification(notification);

                    var quotationMessage = new QuotationMessage
                    {
                        Id = quotation.Id,
                        CustomerId = quotation.CustomerId,
                        ProductId = quotation.ProductId,
                        VendorId = quotation.VendorId,
                        CustomerFullName = _customerService.GetCustomerById(quotation.CustomerId).Username
                    };

                    //send accept quote customer notification
                    //_workflowMessageService.SendAcceptQuoteCustomerNotification(quotationMessage, _workContext.WorkingLanguage.Id);

                    ////send accept quote vendor notification
                    //_workflowMessageService.SendAcceptQuoteVendorNotification(quotationMessage, _workContext.WorkingLanguage.Id);
                }
                return RedirectToAction(nameof(Sended));
            }
            catch (Exception ex)
            {
                throw ex;

            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CustomerRefuse(QuotationModel model)
        {
            try
            {
                Quotation quotation = _quotationService.Get(model.QuotationId);
                if (quotation.CustomerId == _workContext.CurrentCustomer.Id)
                {
                    quotation.Status = (int)QuotationStatus.CustomerRefuse;
                    _quotationService.Save(quotation);
                }
                return RedirectToAction(nameof(Sended));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Notification List

        public ActionResult NotificationList()
        {
            var model = _quotationModelFactory.PrepareNotificationSearchModel(new NotificationSearchModel());

            return View(model);
        }

        [HttpPost]
        public ActionResult NotificationList(NotificationSearchModel searchModel)
        {
            var model = _quotationModelFactory.PrepareNotificationListModel(searchModel);

            return Json(model);
        }

        public ActionResult ReadNotification(int id, string name)
        {
            var notification = _customNotificationService.GetNotificationByEntityIdAndEntityName(id, name);

            if (notification == null)
                return RedirectToAction("NotificationList");

            notification.IsRead = true;

            _customNotificationService.UpdateNotification(notification);

            if (name == "Quotation")
                return RedirectToAction("Edit", new { id = notification.EntityId });
            else
                return RedirectToAction("Edit","Transaction", new { id = notification.EntityId });

        }

        #endregion
    }
}
