using Nop.Core.Domain.Messages;
using Nop.Data;
using Nop.Services.Caching.Extensions;
using Nop.Services.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nop.Services.Messages
{
    public class ConversationMessageService : IConversationMessagesService
    {
        #region Fields
        private readonly IRepository<ConversationMessage> _conversationMessageRepository;
        private readonly IRepository<Conversation> _conversationRepository;  
        private readonly IEventPublisher _eventPublisher;
        #endregion

        #region Ctor
        public ConversationMessageService(IRepository<ConversationMessage> conversationMessageRepository, IRepository<Conversation> conversationRepository, IEventPublisher eventPublisher)
        {
            _conversationMessageRepository = conversationMessageRepository;
            _conversationRepository = conversationRepository;
            _eventPublisher = eventPublisher;
        }
        #endregion
        #region Methodes
        public void Save(ConversationMessage conversationMessage)
        {
            if (conversationMessage == null)
                throw new ArgumentNullException(nameof(conversationMessage));
            if (conversationMessage.Id == 0)
            {
                _conversationMessageRepository.Insert(conversationMessage);
                //event notification
                _eventPublisher.EntityInserted(conversationMessage);
                UpdateLastMessageConversationDate(conversationMessage.ConversationId);
            }
            else
            {
                _conversationMessageRepository.Update(conversationMessage);
                //event notification
                _eventPublisher.EntityUpdated(conversationMessage);
            }
        }

        private void UpdateLastMessageConversationDate(int conversationId)
        {
           Conversation conversation = _conversationRepository.ToCachedGetById(conversationId);
            if (conversation != null)
            {
                conversation.LastMessageDate = DateTime.Now;
                _conversationRepository.Update(conversation);
            }
        }

        public void Delete(int conversationMessageId)
        {
            ConversationMessage conversation = Get(conversationMessageId);
            if (conversation == null)
                throw new ArgumentNullException(nameof(conversation));

            _conversationMessageRepository.Delete(conversation);

            //event notification
            _eventPublisher.EntityDeleted(conversation);
        }
        public void Delete(ConversationMessage conversationMessage)
        {
            if (conversationMessage == null)
                throw new ArgumentNullException(nameof(conversationMessage));

            _conversationMessageRepository.Delete(conversationMessage);

            //event notification
            _eventPublisher.EntityDeleted(conversationMessage);
        }

        public void DeleteAllOfConversation(int conversationId)
        {
            foreach (ConversationMessage conversationMessage in GetByConversationId(conversationId))
            {
                Delete(conversationMessage);
            }
        }
        public ConversationMessage Get(int conversationMessageId)
        {
            if (conversationMessageId == 0)
                return null;

            return _conversationMessageRepository.ToCachedGetById(conversationMessageId);
        }

        public IEnumerable<ConversationMessage> GetUnreadMessageConversation(int receiverId)
        {
            var query = _conversationMessageRepository.Table;

            if (receiverId > 0)
            {
                query = query.Where(x => x.ReceiverId == receiverId && x.IsRead == false);
            }

            return query.ToList();
        }

        public IEnumerable<ConversationMessage> GetUnreadMessageByConversation(int receiverId, int conversationId)
        {
            var query = _conversationMessageRepository.Table;

            if (receiverId > 0)
            {
                query = query.Where(x => x.ReceiverId == receiverId && x.ConversationId == conversationId && x.IsRead == false);
            }

            return query.ToList();
        }



        public IEnumerable<ConversationMessage> GetByConversationByUsers(int firstId, int secondId)
        {
            var query = _conversationMessageRepository.Table;

            if (firstId > 0 && secondId > 0)
            {
                query = query.Where(x => (x.SenderId == firstId && x.ReceiverId == secondId)
                                       ||(x.SenderId == secondId && x.ReceiverId == firstId));
            }

            var conversations = query.ToList();

            return conversations;
        }

        public IEnumerable<ConversationMessage> GetByConversationId(int conversationId)
        {
            var query = _conversationMessageRepository.Table;

            if (conversationId > 0)
            {
                query = query.Where(x => x.ConversationId == conversationId);
            }

            var conversations = query.ToList();

            return conversations;
        }

        public void SetReadMessages(int conversationId, int receiverId)
        {
            var query = _conversationMessageRepository.Table;

            if (receiverId > 0)
            {
                query = query.Where(x => x.ConversationId == conversationId && x.ReceiverId == receiverId && x.IsRead == false);
            }
            foreach (var item in query)
            {
                item.IsRead = true;
                Save(item);
            }
        }

        #endregion
    }
}
