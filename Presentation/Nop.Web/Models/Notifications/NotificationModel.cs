using System;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Notifications
{
    public class NotificationModel : BaseNopEntityModel
    {
        public int EntityId { get; set; }

        public string EntityName { get; set; }

        public int CreatedBy { get; set; }
        public string CreatedByName { get; set; }

        public int CreatedFor { get; set; }

        public string CreatedForName { get; set; }

        public bool IsRead { get; set; }

        public string Description { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
