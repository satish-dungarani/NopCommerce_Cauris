using Nop.Core.Domain.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Services.Messages
{
    public interface IConversationMessagesService
    {
        IEnumerable<ConversationMessage> GetByConversationId(int conversationId);
        IEnumerable<ConversationMessage> GetByConversationByUsers(int firstId, int secondId);
        ConversationMessage Get(int conversationMessageId);
        IEnumerable<ConversationMessage> GetUnreadMessageConversation(int receiverId);
        IEnumerable<ConversationMessage> GetUnreadMessageByConversation(int receiverId, int conversationId);
        void Save(ConversationMessage conversationMessage);
        void Delete(int conversationMessageId);
        void DeleteAllOfConversation(int conversationId);
        void SetReadMessages(int conversationId, int receiverId);
        
    }
}
