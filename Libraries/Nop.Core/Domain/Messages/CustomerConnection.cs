using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Core.Domain.Messages
{
    public class CustomerConnection : BaseEntity
    {
        public string ConnectionId { get; set; }
        public int CustomerId { get; set; }
    }
}
