using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Notifications;
using Nop.Core.Domain.Transactions;
using Nop.Core.Infrastructure;
using Nop.Services;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.ExportImport;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Notifications;
using Nop.Services.Quotations;
using Nop.Services.Transactions;
using Nop.Web.Factories;
using Nop.Web.Models.Transactions;

namespace Nop.Web.Controllers
{
    public class TransactionController : BasePublicController
    {
        #region fields

        private readonly ICustomerService _customerService;
        private readonly ITransactionService _transactionService;
        private readonly IQuotationService _quotationService;
        private readonly IWorkContext _workContext;
        private readonly ITransactionModelFactory _transactionModelFactory;
        private readonly IDownloadService _downloadService;
        private readonly INopFileProvider _fileProvider;
        private readonly INotificationService _notificationService;
        private readonly ICountryService _countryService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly ICustomNotificationService _customNotificationService;
        private readonly IExportManager _exportManager;

        #endregion 

        #region ctor
        public TransactionController(ICustomerService customerService,
            IWorkContext workContext,
            ITransactionModelFactory transactionModelFactory,
            ITransactionService transactionService,
            IQuotationService quotationService,
            IDownloadService downloadService,
            INopFileProvider fileProvider,
            INotificationService notificationService,
            ICountryService countryService,
            ILocalizationService localizationService,
            IWorkflowMessageService workflowMessageService,
            ICustomNotificationService customNotificationService, IExportManager exportManager)
        {
            _customerService = customerService;
            _workContext = workContext;
            _transactionModelFactory = transactionModelFactory;
            _transactionService = transactionService;
            _quotationService = quotationService;
            _downloadService = downloadService;
            _fileProvider = fileProvider;
            _notificationService = notificationService;
            _countryService = countryService;
            _localizationService = localizationService;
            _workflowMessageService = workflowMessageService;
            _customNotificationService = customNotificationService;
            _exportManager = exportManager;
        }
        #endregion

        #region Methods

        public virtual IActionResult List(bool isReceived)
        {
            if (!_customerService.IsRegistered(_workContext.CurrentCustomer))
                return Challenge();

            //prepare model
            var model = _transactionModelFactory.PrepareTransactionSearchModel(new TransactionSearchModel()
            {
                IsReceived = isReceived
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
            model.AvailableTransactionStatus.Add(new SelectListItem { Text = "Select Status", Value = "0" });
            foreach (TransactionStatus ts in Enum.GetValues(typeof(TransactionStatus)))
            {
                model.AvailableTransactionStatus.Add(new SelectListItem
                {
                    Text = ts.GetDescription(),
                    Value = ((int)ts).ToString()
                });
            }

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult List(TransactionSearchModel searchModel)
        {
            if (!_customerService.IsRegistered(_workContext.CurrentCustomer))
                return Challenge();

            //prepare model
            var model = _transactionModelFactory.PrepareTransactionListModel(searchModel);

            return Json(model);
        }

        [HttpPost]
        public IActionResult DataSource(DataTableAjaxPostModel ajaxModel,
            bool isReceived,
            string name = null,
            int country = 0,
            string date = null,
            string product = null,
            string transnumber = null,
            int status = 0
            )
          {
            if (!_customerService.IsRegistered(_workContext.CurrentCustomer))
                return Challenge();

            var listModel = _transactionModelFactory.PrepareAjaxTransactionListModel(ajaxModel, new TransactionSearchModel()
            {
                IsReceived = isReceived,
                Keyword = ajaxModel.Search.value,
                Name = name,
                Country = country,
                Date = date,
                Product = product,
                TransNumber = transnumber,
                Status = status
            });

            return Json(new
            {
                // this is what datatables wants sending back
                draw = ajaxModel.Draw,
                recordsTotal = listModel.RecordsTotal,
                recordsFiltered = listModel.RecordsFiltered,
                data = listModel.Data,
                length = ajaxModel.Length,
                order = ajaxModel.Order,
                start = ajaxModel.Start
            });
        }
        public virtual IActionResult Edit(int id)
        {
            var transaction = _transactionService.GetTransactionById(id);
            var quotation = _quotationService.Get(transaction.QuotationId);

            if (quotation.CustomerId != _workContext.CurrentCustomer.Id && quotation.VendorId != _workContext.CurrentVendor?.Id)
                return AccessDeniedView();

            ViewBag.IsReceived = _workContext.CurrentCustomer.Id != quotation.CustomerId;

            return View(_transactionModelFactory.PrepareTransactionOverviewModel(transaction, quotation));
        }

        [HttpPost]
        public virtual IActionResult Edit(TransactionOverviewModel model, IFormCollection form)
        {
            var transaction = _transactionService.GetTransactionById(model.Id);
            var quotation = _quotationService.Get(transaction.QuotationId);

            if (quotation.CustomerId != _workContext.CurrentCustomer.Id && quotation.VendorId != _workContext.CurrentVendor?.Id)
                return AccessDeniedView();

            var httpPostedFile = Request.Form.Files["CustomerDocument"];
            if (!string.IsNullOrEmpty(httpPostedFile?.FileName))
            {
                //save an uploaded file
                var download = new Download
                {
                    DownloadGuid = Guid.NewGuid(),
                    UseDownloadUrl = false,
                    DownloadUrl = string.Empty,
                    DownloadBinary = _downloadService.GetDownloadBits(httpPostedFile),
                    ContentType = httpPostedFile.ContentType,
                    Filename = _fileProvider.GetFileNameWithoutExtension(httpPostedFile.FileName),
                    Extension = _fileProvider.GetFileExtension(httpPostedFile.FileName),
                    IsNew = true
                };
                _downloadService.InsertDownload(download);

                transaction.CustomerDocumentId = download.Id;
                transaction.TransactionStatusId = (int)TransactionStatus.Waiting_Validation_Contract_By_Moderator;
                _transactionService.SaveTransaction(transaction);

                _workflowMessageService.SellerUploadContractNotification(_workContext.CurrentCustomer, transaction.Id, quotation.CustomerId, _workContext.WorkingLanguage.Id);

                var notification = new Notification
                {
                    EntityId = transaction.Id,
                    EntityName = "Transaction",
                    CreatedBy = _workContext.CurrentCustomer.Id,
                    CreatedFor = quotation.CustomerId,
                    IsRead = false,
                    Description = _localizationService.GetResource("Custom.Code.Notification.Seller.Uploaded.Contract"),
                    CreatedDate = DateTime.UtcNow
                };

                _customNotificationService.InsertNotification(notification);

                _notificationService.SuccessNotification("Customer contrat uploaded successfully.");
            }
            else
            {
                _notificationService.ErrorNotification("Customer contrat file not found.");
            }
            ViewBag.IsReceived = _workContext.CurrentCustomer.Id == quotation.CustomerId ? false : true;
            return View(_transactionModelFactory.PrepareTransactionOverviewModel(transaction, quotation));
        }

        #endregion

        #region Fine Uploader

        [HttpPost]
        public virtual IActionResult SaveTransactionComment(string adminTransactionComment, int transactionId)
        {
            var transaction = _transactionService.GetTransactionById(transactionId);

            #region Transaction Comment
            var transComment = new TransactionComment()
            {
                TransactionId = transaction.Id,
                ModeratorComment = adminTransactionComment,
                CreatedDate = DateTime.UtcNow,
                ModeratorId = transaction.ModeratorIndentity
            };
            _transactionService.SaveTransactionComment(transComment);
            #endregion
            return Ok(new { success = true });
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public virtual IActionResult UploadFileContract(int transactionId)
        {
            var httpPostedFile = Request.Form.Files.FirstOrDefault();
            if (httpPostedFile == null)
            {
                return Json(new
                {
                    success = false,
                    message = "No file uploaded",
                    downloadGuid = Guid.Empty
                });
            }
            var fileBinary = _downloadService.GetDownloadBits(httpPostedFile);

            var qqFileNameParameter = "qqfilename";
            var fileName = httpPostedFile.FileName;
            if (string.IsNullOrEmpty(fileName) && Request.Form.ContainsKey(qqFileNameParameter))
                fileName = Request.Form[qqFileNameParameter].ToString();
            //remove path (passed in IE)
            fileName = _fileProvider.GetFileName(fileName);

            var contentType = httpPostedFile.ContentType;

            var fileExtension = _fileProvider.GetFileExtension(fileName);
            if (!string.IsNullOrEmpty(fileExtension))
                fileExtension = fileExtension.ToLowerInvariant();

            var download = new Download
            {
                DownloadGuid = Guid.NewGuid(),
                UseDownloadUrl = false,
                DownloadUrl = string.Empty,
                DownloadBinary = fileBinary,
                ContentType = contentType,
                //we store filename without extension for downloads
                Filename = _fileProvider.GetFileNameWithoutExtension(fileName),
                Extension = fileExtension,
                IsNew = true
            };
            _downloadService.InsertDownload(download);
            var transaction = _transactionService.GetTransactionById(transactionId);
            var quotation = _quotationService.Get(transaction.QuotationId);
            transaction.CustomerDocumentId = download.Id;
            transaction.TransactionStatusId = (int)TransactionStatus.Waiting_Validation_Contract_By_Moderator;
            _transactionService.SaveTransaction(transaction);

            _workflowMessageService.SellerUploadContractNotification(_workContext.CurrentCustomer, transaction.Id, quotation.CustomerId, _workContext.WorkingLanguage.Id);

            var notification = new Notification();

            notification.EntityId = transaction.Id;
            notification.EntityName = "Transaction";
            notification.CreatedBy = _workContext.CurrentCustomer.Id;
            notification.CreatedFor = quotation.CustomerId;
            notification.IsRead = false;
            notification.Description = _localizationService.GetResource("Custom.Code.Notification.Seller.Uploaded.Contract");
            notification.CreatedDate = DateTime.UtcNow;

            _customNotificationService.InsertNotification(notification);

            //when returning JSON the mime-type must be set to text/plain
            //otherwise some browsers will pop-up a "Save As" dialog.
            return Json(new
            {
                success = true,
                message = _localizationService.GetResource("ShoppingCart.FileUploaded"),
                downloadUrl = Url.Action("GetFileUpload", "Download", new { downloadId = download.DownloadGuid }),
                downloadGuid = download.DownloadGuid
            });

            //return Ok(new { success = true });
        }

        //[HttpPost]
        //[IgnoreAntiforgeryToken]
        //public virtual IActionResult UploadFileContract(int transactionId, string adminComment)
        //{
        //    var httpPostedFile = Request.Form.Files.FirstOrDefault();
        //    if (httpPostedFile == null)
        //    {
        //        return Json(new
        //        {
        //            success = false,
        //            message = "No file uploaded",
        //            downloadGuid = Guid.Empty
        //        });
        //    }
        //    return Json(new
        //    {
        //        success = false,
        //        downloadGuid = Guid.Empty
        //    });
        //}


        #endregion

        #region Export
        //[HttpPost, ActionName("ExportToExcel")]
        //[FormValueRequired("exportexcel-all")]
        //public virtual IActionResult ExportExcelAll(TransactionSearchModel searchModel)
        //{
        //    if (!_customerService.IsRegistered(_workContext.CurrentCustomer))
        //        return Challenge();

        //    var listModel = _transactionModelFactory.PrepareAjaxTransactionListModel(new DataTableAjaxPostModel(), searchModel);

        //    try
        //    {
        //        var bytes = [];//_exportManager.ExportProductsToXlsx(listModel.Data);
        //        return File(bytes, MimeTypes.TextXlsx, "products.xlsx");
        //    }
        //    catch (Exception exc)
        //    {
        //        _notificationService.ErrorNotification(exc);
        //        return RedirectToAction("List");
        //    }
        //}

        #endregion

        #region Notification Redirect

        public virtual IActionResult NotificationRedirect(int id, int entityId, string entityName)
        {
            if (id == 0)
                return RedirectToRoute("Homepage");

            var notification = _customNotificationService.GetNotificationById(id);
            if (notification == null)
                return RedirectToRoute("Homepage");

            notification.IsRead = true;
            _customNotificationService.UpdateNotification(notification);

            return RedirectToAction("Edit", entityName, new { id = entityId });
        }
        #endregion
    }
}
