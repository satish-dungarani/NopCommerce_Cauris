using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Messages;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Services.Caching.Extensions;
using Nop.Services.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nop.Services.Messages
{
    public class ConversationService : IConversationsService
    {
        #region Fields
        private readonly IRepository<Conversation> _conversationRepository;
        private readonly IEventPublisher _eventPublisher;
        #endregion

        #region Ctor
        public ConversationService(IRepository<Conversation> conversationRepository, IEventPublisher eventPublisher)
        {
            _conversationRepository = conversationRepository;
            _eventPublisher = eventPublisher;
        }
        #endregion

        #region Methodes
        public void AddConversation(Conversation conversation)
        {
            if (conversation == null)
                throw new ArgumentNullException(nameof(conversation));

            _conversationRepository.Insert(conversation);

            //event notification
            _eventPublisher.EntityInserted(conversation);
        }

        public void Delete(int conversationId)
        {
            Conversation conversation = Get(conversationId);
            if (conversation == null)
                throw new ArgumentNullException(nameof(conversation));

            _conversationRepository.Delete(conversation);

            //event notification
            _eventPublisher.EntityDeleted(conversation);
        }

        public Conversation Get(int conversationId)
        {
            if (conversationId == 0)
                return null;

            return _conversationRepository.ToCachedGetById(conversationId);
        }

        public IEnumerable<Conversation> GetAll()
        {
            var query = _conversationRepository.Table;

            var conversations = query.ToList();

            return conversations;
        }

        public IEnumerable<Conversation> GetAllByUserId(int userId)
        {
            var query = _conversationRepository.Table;

            if (userId > 0)
            {
                query = query.Where(x => x.FirstSenderId == userId || x.SecondSenderId == userId);
            }

            var conversations = query.ToList();

            return conversations;
        }

        public Conversation GetByUserId(int firstSenderId, int secondSenderId)
        {

            try
            {
                var query = _conversationRepository.Table;

                if (firstSenderId > 0 && secondSenderId > 0)
                {
                    query = query.Where(x => (x.FirstSenderId == firstSenderId && x.SecondSenderId == secondSenderId)
                                           || (x.FirstSenderId == secondSenderId && x.SecondSenderId == firstSenderId));
                }
                var conversation = query.FirstOrDefault();

                return conversation;
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        public Conversation GetOrCreateConversation(int receiverIdValue, int conversationIdValue, Customer customer)
        {
            Conversation conversation = conversationIdValue > 0 ? Get(conversationIdValue) : GetByUserId(customer.Id, receiverIdValue);
            if (conversation == null || conversation.Id == 0)
            {
                conversation = new Conversation
                {
                    FirstSenderId = customer.Id,
                    SecondSenderId = receiverIdValue,
                    CreationDate = DateTime.Now
                };
                AddConversation(conversation);
                conversation = GetByUserId(customer.Id, receiverIdValue);
            }

            return conversation;
        }
        #endregion
    }
}
