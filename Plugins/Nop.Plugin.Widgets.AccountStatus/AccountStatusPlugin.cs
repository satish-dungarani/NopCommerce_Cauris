using System;
using System.Collections.Generic;
using Nop.Core;
using Nop.Services.Cms;
using Nop.Services.Plugins;
using Nop.Web.Framework.Infrastructure;

namespace Nop.Plugin.Widgets.AccountStatus
{
    public class AccountStatusPlugin : BasePlugin, IWidgetPlugin
    {
        private readonly IWebHelper _webHelper;
        public AccountStatusPlugin(IWebHelper webHelper)
        {
           _webHelper = webHelper;
        }
        public bool HideInWidgetList => false;
        public string GetWidgetViewComponentName(string widgetZone)
        {
            return "WidgetsAccountStatus";
        }
        public IList<string> GetWidgetZones()
        {
            return new List<string> { PublicWidgetZones.ContentBefore };
        }
        /// <summary>
        /// Install plugin
        /// </summary>
        public override void Install()
        {
            base.Install();
        }
        /// <summary>
        /// Uninstall plugin
        /// </summary>
        public override void Uninstall()
        {
            base.Uninstall();
        }
    }
}
