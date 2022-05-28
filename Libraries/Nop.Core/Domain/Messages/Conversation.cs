using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Core.Domain.Messages
{
    public class Conversation : BaseEntity
    {
        public int FirstSenderId { get; set; }
        public int SecondSenderId { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? LastMessageDate { get; set; }
    }
}
