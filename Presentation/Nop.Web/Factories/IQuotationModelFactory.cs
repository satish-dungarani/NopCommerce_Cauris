using Nop.Core.Domain.Notifications;
using Nop.Core.Domain.Quotations;
using Nop.Web.Models.Notifications;
using Nop.Web.Models.Quotation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Factories
{
    public partial interface IQuotationModelFactory
    {
        QuotationModel PrepareCreateModel(int productId);
        QuotationListModel PrepareListModel(QuotationSearchModel searchModel, IEnumerable<Quotation> quotations, bool isVendor);
        QuotationListModel PrepareListModelModeration(QuotationSearchModel searchModel, IEnumerable<Quotation> quotations);
        QuotationModel PrepareEditModel(Quotation quotation);
        QuotationSearchModel PrepareSearchModeration(QuotationSearchModel quotationSearchModel);
        string ParseQuotationStaus(int statusId, bool isVendor);
        #region Notification

        List<NotificationModel> PrepareNotificationModel();

        NotificationListModel PrepareNotificationListModel(NotificationSearchModel searchModel);

        NotificationSearchModel PrepareNotificationSearchModel(NotificationSearchModel searchModel);
        List<QuotationViewModel> PrepareListModel(IOrderedEnumerable<Quotation> quotations , bool isVendor);

        #endregion

    }
}
