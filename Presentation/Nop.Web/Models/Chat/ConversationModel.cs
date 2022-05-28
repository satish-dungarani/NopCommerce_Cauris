using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Models.Chat
{
    public class ConversationModel
    {
        public int ConversationId { get; set; }
        public int SenderId { get; set; }
        public bool HasUnreadMessages { get; set; }
        public int PartnerId { get; set; }
        public bool IsConnected { get; internal set; }
        public string PartnerName { get; internal set; }
    }
}
