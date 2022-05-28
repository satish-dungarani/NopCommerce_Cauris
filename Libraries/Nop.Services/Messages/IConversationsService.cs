using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Services.Messages
{
    public interface IConversationsService
    {
        IEnumerable<Conversation> GetAll();
        IEnumerable<Conversation> GetAllByUserId(int userId);
        Conversation Get(int conversationId);
        Conversation GetByUserId(int firstSenderId, int secondSenderId);
        Conversation GetOrCreateConversation(int receiverIdValue, int conversationIdValue, Customer customer);
        void AddConversation(Conversation conversation);
        void Delete(int conversationId);
    }
}
