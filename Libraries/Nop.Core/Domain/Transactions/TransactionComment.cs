using System;

namespace Nop.Core.Domain.Transactions
{
    public partial class TransactionComment:BaseEntity
    {
        public int TransactionId { get; set; }
        public string ModeratorId { get; set; }
        public DateTime CreatedDate { get; set; }

        public string ModeratorComment { get; set; }
    }
}
