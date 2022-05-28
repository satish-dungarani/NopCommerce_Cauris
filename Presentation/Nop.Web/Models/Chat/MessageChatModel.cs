using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Models.Chat
{
    public class MessageChatModel
    {
        public bool FromPartner { get; set; }
        public string PartnerName { get; set; }
        public int PartnerId { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; }
    }
}
