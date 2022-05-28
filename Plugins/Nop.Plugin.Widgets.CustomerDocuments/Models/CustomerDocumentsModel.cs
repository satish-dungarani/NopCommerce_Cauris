using System.Collections.Generic;
using Nop.Plugin.Widgets.CustomerDocuments.Domain;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Widgets.CustomerDocuments.Models
{
    public class CustomerDocumentsModel : BaseNopModel
    {
        public CustomerDocumentsModel()
        {
            
        }
        public IList<CustomerDocumentsItem> Documents { get; set; }
    }
}
