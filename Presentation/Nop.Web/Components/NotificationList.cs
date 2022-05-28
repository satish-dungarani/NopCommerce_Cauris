using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components
{
    public class NotificationListViewComponent : NopViewComponent
    {
        #region Fields

        private readonly IQuotationModelFactory _quotationModelFactory;

        #endregion

        #region Ctor

        public NotificationListViewComponent(IQuotationModelFactory quotationModelFactory)
        {
            _quotationModelFactory = quotationModelFactory;
        }

        #endregion
        public IViewComponentResult Invoke()
        {
            var model = _quotationModelFactory.PrepareNotificationModel();
            return View(model);
        }
    }
}
