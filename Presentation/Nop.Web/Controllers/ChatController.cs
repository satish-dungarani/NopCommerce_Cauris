using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Messages;
using Nop.Core.Infrastructure;
using Nop.Services.Customers;
using Nop.Services.Messages;
using Nop.Services.Vendors;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Models.Chat;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Nop.Web.Controllers
{
    [AutoValidateAntiforgeryToken]
    public partial class ChatController : BasePublicController
    {
        private readonly ICustomerService _customerService;
        private readonly IConversationMessagesService _conversationMessagesService;
        private readonly IConversationsService _conversationService;
        private readonly IWorkContext _workContext;
        private readonly IVendorService _vendorService;
        public ChatController(ICustomerService customerService,
            IConversationsService conversationsService,
            IWorkContext workContext,
            IConversationMessagesService conversationMessagesService , IVendorService vendorService)
        {
            _customerService = customerService;
            _conversationService = conversationsService;
            _workContext = workContext;
            _conversationMessagesService = conversationMessagesService;
            _vendorService = vendorService;
        }
        public PartialViewResult _Chat(int partnerId)
        {
            Customer currentConsumer = _workContext.CurrentCustomer;
            Conversation conversation = _conversationService.GetByUserId(currentConsumer.Id, partnerId);
            var receiver = _customerService.GetCustomerById( partnerId);
            ConversationModel vMConversation = new ConversationModel { ConversationId = conversation.Id, SenderId = currentConsumer.Id, PartnerId = partnerId, PartnerName= _customerService.GetCustomerFullName(receiver) , HasUnreadMessages = _conversationMessagesService.GetUnreadMessageByConversation(currentConsumer.Id, conversation.Id).Any()};
            return PartialView(vMConversation);
        }

        [HttpsRequirement]
        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        public IActionResult Conversations()
        {
            if (!_customerService.IsRegistered(_workContext.CurrentCustomer))
                return Challenge();
            Customer currentConsumer = _workContext.CurrentCustomer;
            IEnumerable<Conversation> conversations = _conversationService.GetAllByUserId(currentConsumer.Id);
            var vm = conversations.OrderByDescending(x => x.LastMessageDate).Select(x => new ConversationModel { 
             ConversationId =x.Id,
             SenderId = currentConsumer.Id,
             PartnerId = x.FirstSenderId == currentConsumer.Id? x.SecondSenderId :x.FirstSenderId,
             PartnerName = _customerService.GetCustomerFullName(_customerService.GetCustomerById(x.FirstSenderId == currentConsumer.Id ? x.SecondSenderId : x.FirstSenderId))
            });
            return View(vm);
        }
        public IEnumerable<MessageChatModel> GetOldMessages(int? conversationId = null, int? receiverId = null)
        {
            Customer currentConsumer = _workContext.CurrentCustomer;
            Conversation conversion = null;
            if (!conversationId.HasValue && receiverId.HasValue)
            {
                conversion = _conversationService.GetByUserId(currentConsumer.Id, receiverId.Value);
            }
            else if (conversationId.HasValue)
            {
                conversion = _conversationService.Get(conversationId.Value);
            }
            List<MessageChatModel> mMessageChats = new List<MessageChatModel>();

            if (conversion != null)
            {
                var fisrtSender = _customerService.GetCustomerById(conversion.FirstSenderId);
                var secondSender = _customerService.GetCustomerById(conversion.SecondSenderId);

                IEnumerable<ConversationMessage> messages = _conversationMessagesService.GetByConversationId(conversion.Id);
                foreach (var message in messages)
                {
                    int partnerId = message.ReceiverId == currentConsumer.Id ? message.SenderId : message.ReceiverId;
                    mMessageChats.Add(new MessageChatModel
                    {
                        FromPartner = message.ReceiverId == currentConsumer.Id,
                        PartnerId = partnerId,
                        PartnerName = partnerId == conversion.FirstSenderId ? _customerService.GetCustomerFullName(fisrtSender) : _customerService.GetCustomerFullName(secondSender),
                        Message = message.Text,
                        IsRead = message.SenderId == currentConsumer.Id ? true : message.IsRead
                    });
                }
            }
            return mMessageChats;
        }
        public void SetReadMessagesConversation(int conversationId)
        {
            Customer currentConsumer = _workContext.CurrentCustomer;
            Conversation conversation = _conversationService.Get(conversationId);
            if (conversation != null)
            {
                EngineContext.Current.Resolve<IConversationMessagesService>().SetReadMessages(conversation.Id, currentConsumer.Id);
            }
        }
        public List<ConversationModel> GetUnreadMessagesConversation()
        {
            Customer currentConsumer = _workContext.CurrentCustomer;
            List<ConversationModel> vm = new List<ConversationModel>();
            if (_customerService.IsRegistered(currentConsumer))
            {
               
                IEnumerable<ConversationMessage> unreadConversationMessages = new List<ConversationMessage>();
                if (_customerService.IsRegistered(currentConsumer))
                {
                    if (_workContext.CurrentVendor != null)
                    {
                        unreadConversationMessages = _conversationMessagesService.GetUnreadMessageConversation(_workContext.CurrentVendor.Id).ToList();

                    }
                    else
                    {
                        unreadConversationMessages = _conversationMessagesService.GetUnreadMessageConversation(currentConsumer.Id).ToList();
                    }
                    foreach (var unreadConversationMessage in unreadConversationMessages)
                    {
                        ConversationModel vMConversation = new ConversationModel
                        {
                            IsConnected = true
                        };
                        if (unreadConversationMessage != null)
                        {
                            vMConversation.HasUnreadMessages = true;
                            vMConversation.PartnerId = unreadConversationMessage.SenderId;
                        }
                        vm.Add(vMConversation);
                    }
                }
            }
            return vm;
        }
        public ConversationModel GetConversation(int receiverId)
        {
            Customer currentConsumer = _workContext.CurrentCustomer;
            Conversation conversation = _conversationService.GetOrCreateConversation(receiverId, 0, currentConsumer);
           var receiverasCustomer = _customerService.GetCustomerById(receiverId);
            return new ConversationModel { ConversationId = conversation.Id, SenderId = currentConsumer.Id, PartnerId = receiverId, PartnerName = _customerService.GetCustomerFullName(receiverasCustomer) };
        }
    }

    internal class ConversationComparer : IEqualityComparer<ConversationMessage>
    {
        public bool Equals([AllowNull] ConversationMessage x, [AllowNull] ConversationMessage y)
        {
            return x.ConversationId == y.ConversationId;
        }

        public int GetHashCode([DisallowNull] ConversationMessage obj)
        {
            return obj.ConversationId.GetHashCode();
        }
    }
}