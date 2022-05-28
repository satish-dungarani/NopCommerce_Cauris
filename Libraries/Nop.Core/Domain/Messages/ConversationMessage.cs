using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Core.Domain.Messages
{
   public class ConversationMessage : BaseEntity
    {
        public int ConversationId { get; set; }
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public string Text { get; set; }
        public DateTime CreationDate { get; set; }
        public bool IsRead { get; set; }
    }
}
