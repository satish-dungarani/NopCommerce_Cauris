using System;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Plugin.Widgets.CustomerDocuments.Models;
using Nop.Plugin.Widgets.CustomerDocuments.Services;
using Nop.Services.Caching;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Media;
using Nop.Web.Framework.Components;
namespace Nop.Plugin.Widgets.CustomerDocuments.Components
{
    [ViewComponent(Name = "WidgetsCustomerDocuments")]
   public class WidgetsCustomerDocumentsViewComponent : NopViewComponent
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
        private readonly ICustomerDocumentsService _customerDocumentsService;
        public WidgetsCustomerDocumentsViewComponent(ICacheKeyService cacheKeyService,
            IStoreContext storeContext,
            IStaticCacheManager staticCacheManager,
            ISettingService settingService,
            IPictureService pictureService,
            IWebHelper webHelper, 
            ICustomerService customerService, 
            IWorkContext workContext,
            IGenericAttributeService genericAttributeService,
            ICustomerDocumentsService customerDocumentsService)
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
            _customerDocumentsService = customerDocumentsService;
        }
        public IViewComponentResult Invoke(string widgetZone, object additionalData)
        {
            var customer = _workContext.CurrentCustomer;
            if (customer != null && _customerService.IsRegistered(customer))
            {
                var model = new CustomerDocumentsModel();
                model.Documents = _customerDocumentsService.GetCustomerDocuments(customer);
                return View("~/Plugins/Widgets.CustomerDocuments/Views/PublicInfo.cshtml", model);
            }
            return Content("");
        }
    }
}
