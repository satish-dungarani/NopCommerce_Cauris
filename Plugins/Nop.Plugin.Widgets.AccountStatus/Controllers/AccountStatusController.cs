using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Widgets.AccountStatus.Models;
using Nop.Services.Customers;
using Nop.Services.Security;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.Widgets.AccountStatus.Controllers
{
    public class AccountStatusController: BasePluginController
    {
        #region Fields
        private readonly ICustomerService _customerService;
        private readonly IWorkContext _workContext;
        private readonly IPermissionService _permissionService;

        #endregion
        public AccountStatusController(IPermissionService permissionService , ICustomerService customerService , IWorkContext workContext)
        {
            _permissionService = permissionService;
            _customerService = customerService;
            _workContext = workContext;
        }
        public IActionResult Configure()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageWidgets))
                return AccessDeniedView();
            var model = new ConfigurationModel();
            return View("~/Plugins/Widgets.AccountStatus/Views/Configure.cshtml", model);
        }

        [HttpPost]
        public IActionResult Configure(ConfigurationModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageWidgets))
                return AccessDeniedView();
            return Configure();
        }
    }
}
