using System;

namespace Nop.Core.Domain.Notifications
{
    public partial class Notification : BaseEntity
    {
        public int EntityId { get; set; }

        public string EntityName { get; set; }

        public int CreatedBy { get; set; }

        public int CreatedFor { get; set; }

        public bool IsRead { get; set; }

        public string Description { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
