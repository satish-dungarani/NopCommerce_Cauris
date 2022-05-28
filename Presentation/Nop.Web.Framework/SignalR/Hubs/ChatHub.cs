using Microsoft.AspNetCore.SignalR;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Messages;
using Nop.Core.Infrastructure;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Framework.SignalR.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ICustomerService _customerService;
        private readonly IGenericAttributeService _genericAttributeService;
        public ChatHub(ICustomerService customerService, IGenericAttributeService genericAttributeService)
        {
            _customerService = customerService;
            _genericAttributeService = genericAttributeService;
        }
        
        #region Chat methodes
        public string GetConnectionId()
        {
            Customer customer = EngineContext.Current.Resolve<IWorkContext>().CurrentCustomer;
          
            if (customer != null && _customerService.IsRegistered(customer))
            {
                    CustomerConnection customerConnection = EngineContext.Current.Resolve<ICustomerConnectionService>().GetByCustomerId(customer.Id);
                    if (customerConnection == null)
                    {
                        EngineContext.Current.Resolve<ICustomerConnectionService>().Add(new CustomerConnection { ConnectionId = Context.ConnectionId, CustomerId = customer.Id });
                        customerConnection = EngineContext.Current.Resolve<ICustomerConnectionService>().GetByCustomerId(customer.Id);
                    }
                    if (customerConnection.ConnectionId != Context.ConnectionId)
                    {
                        customerConnection.ConnectionId = Context.ConnectionId;
                        EngineContext.Current.Resolve<ICustomerConnectionService>().Update(customerConnection);
                    }
                    return Context.ConnectionId;
            }
            return string.Empty;
        }
        public async Task SendMessage(string senderConnectionId, string message, string receiverId, string conversationId = "")
        {
            int.TryParse(receiverId, out int receiverIdValue);
            int.TryParse(conversationId, out int conversationIdValue);


            if (receiverIdValue == 0)
            {
                //Send to all connected users
                await Clients.All.SendAsync("ReceiveMessage", true, senderConnectionId, message, "");
            }
            else
            {
                Customer sender = EngineContext.Current.Resolve<IWorkContext>().CurrentCustomer;
                Customer receiver = _customerService.GetCustomerById(receiverIdValue);

                if (sender != null && receiver != null)
                {
                    Conversation conversation = EngineContext.Current.Resolve<IConversationsService>().GetOrCreateConversation(receiverIdValue, conversationIdValue, sender);

                    SaveMessage(message, receiverIdValue, sender, conversation);

                    CustomerConnection customerConnectionReceiver = GetOrCreateCustomerConnection(receiver);

                    await Clients.Client(senderConnectionId.ToString()).SendAsync("ReceiveMessage", false, _customerService.GetCustomerFullName(receiver), conversation?.Id, receiverId.ToString(), message);
                    await Clients.Client(customerConnectionReceiver.ConnectionId.ToString()).SendAsync("ReceiveMessage", true, _customerService.GetCustomerFullName(sender), conversation?.Id, sender.Id.ToString(), message);
                }
            }
        }


        #endregion

        #region Privates methodes
        private CustomerConnection GetOrCreateCustomerConnection(Customer customer)
        {
            CustomerConnection customerConnectionReceiver = EngineContext.Current.Resolve<ICustomerConnectionService>().GetByCustomerId(customer.Id);
            if (customerConnectionReceiver == null)
            {
                EngineContext.Current.Resolve<ICustomerConnectionService>().Add(new CustomerConnection { ConnectionId = Context.ConnectionId, CustomerId = customer.Id });
                customerConnectionReceiver = EngineContext.Current.Resolve<ICustomerConnectionService>().GetByCustomerId(customer.Id);
            }

            return customerConnectionReceiver;
        }

        private static void SaveMessage(string message, int receiverIdValue, Customer customer, Conversation conversation)
        {
            if (conversation != null)
            {
                EngineContext.Current.Resolve<IConversationMessagesService>().Save(
                    new ConversationMessage
                    {
                        ConversationId = conversation.Id,
                        CreationDate = DateTime.Now,
                        ReceiverId = receiverIdValue,
                        SenderId = customer.Id,
                        Text = message
                    }
                );
            }
        }

        #endregion
    }
}
