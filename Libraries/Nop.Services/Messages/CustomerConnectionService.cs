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
    public class CustomerConnectionService : ICustomerConnectionService
    {
        #region Fields
        private readonly IRepository<CustomerConnection> _customerConnection;
        private readonly IEventPublisher _eventPublisher;
        #endregion

        #region Ctor
        public CustomerConnectionService(IRepository<CustomerConnection> customerConnection, IEventPublisher eventPublisher)
        {
            _customerConnection = customerConnection;
            _eventPublisher = eventPublisher;
        }


        #endregion

        #region Methodes
        public void Add(CustomerConnection customerConnection)
        {
            if (customerConnection == null)
                throw new ArgumentNullException(nameof(customerConnection));

            _customerConnection.Insert(customerConnection);

            //event notification
            _eventPublisher.EntityInserted(customerConnection);
        }

        public void Delete(int customerConnectionId)
        {
            CustomerConnection customerConnection = Get(customerConnectionId);
            if (customerConnection == null)
                throw new ArgumentNullException(nameof(customerConnection));

            _customerConnection.Delete(customerConnection);

            //event notification
            _eventPublisher.EntityDeleted(customerConnection);
        }

        public CustomerConnection Get(int conversationId)
        {
            if (conversationId == 0)
                return null;

            return _customerConnection.ToCachedGetById(conversationId);
        }

        public CustomerConnection GetByConnectionId(string connectionId)
        {
            var query = _customerConnection.Table;

            query = query.Where(x => x.ConnectionId == connectionId);
            return query.FirstOrDefault();
        }

        public CustomerConnection GetByCustomerId(int customerId)
        {
            var query = _customerConnection.Table;

            if (customerId > 0)
            {
                query = query.Where(x => x.CustomerId == customerId);
            }
            return query.FirstOrDefault();
        }

        public void Update(CustomerConnection customerConnection)
        {
            if (customerConnection == null)
                throw new ArgumentNullException(nameof(customerConnection));

            _customerConnection.Update(customerConnection);

            //event notification
            _eventPublisher.EntityInserted(customerConnection);
        }
        #endregion
    }
}
