using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Media;
using Nop.Core.Infrastructure;
using Nop.Plugin.Widgets.CustomerDocuments.Domain;
using Nop.Plugin.Widgets.CustomerDocuments.Models;
using Nop.Plugin.Widgets.CustomerDocuments.Services;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Widgets.CustomerDocuments.Controllers
{
    public class CustomerDocumentsController : BasePluginController
    {
        #region Fields
        private readonly ICustomerService _customerService;
        private readonly IWorkContext _workContext;
        private readonly IPermissionService _permissionService;
        private readonly IDownloadService _downloadService;
        private readonly INopFileProvider _fileProvider;
        private readonly ICustomerDocumentsService _customerDocumentsService;
        private readonly IStoreContext _storeContext;
        private readonly IGenericAttributeService _genericAttributeService;
        #endregion
        public CustomerDocumentsController(IPermissionService permissionService ,
                                                  ICustomerService customerService , 
                                                  IWorkContext workContext , 
                                                  IDownloadService downloadService,
                                                  INopFileProvider fileProvider,
                                                  ICustomerDocumentsService customerDocumentsService,
                                                  IStoreContext storeContext,
                                                  IGenericAttributeService genericAttributeService)
        {
            _permissionService = permissionService;
            _customerService = customerService;
            _workContext = workContext;
            _downloadService = downloadService;
            _fileProvider = fileProvider;
            _customerDocumentsService = customerDocumentsService;
            _storeContext = storeContext;
            _genericAttributeService = genericAttributeService;
        }
        public IActionResult Configure()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageWidgets))
                return AccessDeniedView();
            var model = new ConfigurationModel();
            return View("~/Plugins/Widgets.CustomerDocuments/Views/Configure.cshtml", model);
        }

        [HttpPost]
        public IActionResult Configure(ConfigurationModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageWidgets))
                return AccessDeniedView();
            return Configure();
        }
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public virtual IActionResult UploadFileCustomerDocument(string qquuid, string qqfilename, int qqtotalfilesize, IFormFile qqfile)
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
            var fileName = qqfilename;
            if (string.IsNullOrEmpty(qqfilename))
            {
                var qqFileNameParameter = "qqfilename";
                fileName = httpPostedFile.FileName;
                if (string.IsNullOrEmpty(fileName) && Request.Form.ContainsKey(qqFileNameParameter))
                    fileName = Request.Form[qqFileNameParameter].ToString();
                //remove path (passed in IE)
                fileName = _fileProvider.GetFileName(fileName);
            }
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
                Filename = _fileProvider.GetFileNameWithoutExtension(qqfilename),
                Extension = fileExtension,
                IsNew = true
            };
            _downloadService.InsertDownload(download);
            var document = new CustomerDocumentsItem
            {
                CreatedOnUtc = DateTime.UtcNow,
                CustomerId = _workContext.CurrentCustomer.Id,
                FileName = fileName,
                DownloadId = download.Id,
                StoreId = _storeContext.CurrentStore.Id,
                DocumentType = (int)CustomerDocumentType.Identity
            };
            _customerDocumentsService.InsertDocument(document);
            // update status profile


            _genericAttributeService
                 .SaveAttribute<int?>(_workContext.CurrentCustomer, "UploadIdentityDocuments",1, _storeContext.CurrentStore.Id);
            return Ok(new { success = true });

        }
        [HttpPost]
        [HttpsRequirement]
        public virtual IActionResult SaveCustomerDocument()
        {
            var model = new CustomerDocumentsModel();
            var customer = _workContext.CurrentCustomer;
            if (customer != null && _customerService.IsRegistered(customer))
            {
                model.Documents = _customerDocumentsService.GetCustomerDocuments(customer);
            }
                    
            return Json(new
            {

                html = RenderPartialViewToString("~/Plugins/Widgets.CustomerDocuments/Views/_IdentityList.cshtml", model)

            });
        }

        [HttpPost]
        [HttpsRequirement]
        public virtual IActionResult DeleteDocument(int documentId)
        {
            var document = _customerDocumentsService.GetDocumentItemById(documentId);
            if (document != null)
            {
                var downlod = _downloadService.GetDownloadById(document.DownloadId);
                _customerDocumentsService.DeleteDocument(document);
                if (downlod != null)
                {
                    _downloadService.DeleteDownload(downlod);
                }
            }
            var model = new CustomerDocumentsModel();
            var customer = _workContext.CurrentCustomer;
            if (customer != null && _customerService.IsRegistered(customer))
            {
                model.Documents = _customerDocumentsService.GetCustomerDocuments(customer);
            }
            return Json(new
            {
                Count = model.Documents.Count,
                html = RenderPartialViewToString("~/Plugins/Widgets.CustomerDocuments/Views/_IdentityDocuments.cshtml", model)

            });

        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public virtual IActionResult UploadFileCustomerDocumentCompany(string qquuid, string qqfilename, int qqtotalfilesize, IFormFile qqfile)
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
            var fileName = qqfilename;
            if (string.IsNullOrEmpty(qqfilename))
            {
                var qqFileNameParameter = "qqfilename";
                fileName = httpPostedFile.FileName;
                if (string.IsNullOrEmpty(fileName) && Request.Form.ContainsKey(qqFileNameParameter))
                    fileName = Request.Form[qqFileNameParameter].ToString();
                //remove path (passed in IE)
                fileName = _fileProvider.GetFileName(fileName);
            }
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
                Filename = _fileProvider.GetFileNameWithoutExtension(qqfilename),
                Extension = fileExtension,
                IsNew = true
            };
            _downloadService.InsertDownload(download);
            var document = new CustomerDocumentsItem
            {
                CreatedOnUtc = DateTime.UtcNow,
                CustomerId = _workContext.CurrentCustomer.Id,
                FileName = fileName,
                DownloadId = download.Id,
                StoreId = _storeContext.CurrentStore.Id,
                DocumentType = (int)CustomerDocumentType.Compagny
            };
            _customerDocumentsService.InsertDocument(document);
            // update status profile


            _genericAttributeService
                 .SaveAttribute<int?>(_workContext.CurrentCustomer, "UploadCompanyDocuments", 1, _storeContext.CurrentStore.Id);
            return Ok(new { success = true });

        }
        [HttpPost]
        [HttpsRequirement]
        public virtual IActionResult SaveCustomerDocumentCompany()
        {
            var model = new CustomerDocumentsModel();
            var customer = _workContext.CurrentCustomer;
            if (customer != null && _customerService.IsRegistered(customer))
            {
                model.Documents = _customerDocumentsService.GetCustomerDocuments(customer);
            }
           
            return Json(new
            {

                html = RenderPartialViewToString("~/Plugins/Widgets.CustomerDocuments/Views/_CompagnyList.cshtml", model)

            });
        }
    }
}
