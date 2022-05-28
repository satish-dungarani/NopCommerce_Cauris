using System;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Plugin.Widgets.AccountStatus.Models;
using Nop.Services.Caching;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Media;
using Nop.Web.Framework.Components;
namespace Nop.Plugin.Widgets.AccountStatus.Components
{
    [ViewComponent(Name = "WidgetsAccountStatus")]
   public class WidgetsAccountStatusViewComponent : NopViewComponent
    {
        private readonly ICacheKeyService _cacheKeyService;
        private readonly IStoreContext _storeContext;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly ISettingService _settingService;
        private readonly IPictureService _pictureService;
        private readonly IWebHelper _webHelper; 
        private readonly ICustomerService _customerService;
        private readonly IWorkContext _workContext;
        private readonly IGenericAttributeService _genericAttributeService;
        public WidgetsAccountStatusViewComponent(ICacheKeyService cacheKeyService,
            IStoreContext storeContext,
            IStaticCacheManager staticCacheManager,
            ISettingService settingService,
            IPictureService pictureService,
            IWebHelper webHelper, 
            ICustomerService customerService, 
            IWorkContext workContext,
            IGenericAttributeService genericAttributeService)
        {
            _cacheKeyService = cacheKeyService;
            _storeContext = storeContext;
            _staticCacheManager = staticCacheManager;
            _settingService = settingService;
            _pictureService = pictureService;
            _webHelper = webHelper;
            _customerService = customerService;
            _workContext = workContext;
            _genericAttributeService = genericAttributeService;
        }
        public IViewComponentResult Invoke(string widgetZone, object additionalData)
        {
            var customer = _workContext.CurrentCustomer;
            var storeId = _storeContext.CurrentStore.Id;
            if (customer != null && _customerService.IsRegistered(customer))
            {
                var model = new AccountStatusModel();
                var uploadIdentityDocuments=  _genericAttributeService.GetAttribute<string>(customer, "UploadIdentityDocuments", storeId);
                var uploadCompagnyDocuments= _genericAttributeService.GetAttribute<string>(customer, "UploadCompanyDocuments", storeId);
                var validateDocuments = _genericAttributeService.GetAttribute<string>(customer, "ValidateDocuments",storeId);
                var choiceSubscriptionPlan = _genericAttributeService.GetAttribute<string>(customer, "ChoiceSubscriptionPlan", storeId);
                 if(string.IsNullOrEmpty(uploadIdentityDocuments))
                {
                    model.UploadIdentityDocumens = "Waiting";
                }
                 else
                {
                    model.UploadIdentityDocumens = "Done";
                }

                if (string.IsNullOrEmpty(uploadCompagnyDocuments))
                {
                    model.UploadCompagnyDocuments = "Waiting";
                }
                else
                {
                    model.UploadCompagnyDocuments = "Done";
                }

                if (string.IsNullOrEmpty(validateDocuments))
                {
                    model.ValidateDocuments = "Waiting";
                }
                else
                {
                    model.ValidateDocuments = "Done";
                }

                if (string.IsNullOrEmpty(choiceSubscriptionPlan))
                {
                    model.ChoiceSubscriptionPlan = "Waiting";
                }
                else
                {
                    model.ChoiceSubscriptionPlan = "Done";
                }
                return View("~/Plugins/Widgets.AccountStatus/Views/PublicInfo.cshtml", model);
            }
            return Content("");
        }
    }
}
