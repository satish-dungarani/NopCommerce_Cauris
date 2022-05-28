using Nop.Web.Framework.Models;

namespace Nop.Plugin.Widgets.AccountStatus.Models
{
    public class AccountStatusModel : BaseNopModel
    {
        public AccountStatusModel()
        {
            
        }

        public string UploadIdentityDocumens { get; internal set; }
        public string UploadCompagnyDocuments { get; internal set; }
        public string ValidateDocuments { get; internal set; }
        public string ChoiceSubscriptionPlan { get; internal set; }
    }
}
