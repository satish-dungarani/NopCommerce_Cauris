using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Domain.Customers;
using Nop.Data;
using Nop.Plugin.Widgets.CustomerDocuments.Domain;
using Nop.Services.Events;

namespace Nop.Plugin.Widgets.CustomerDocuments.Services
{
    public class CustomerDocumentsService : ICustomerDocumentsService
    {
        private readonly IRepository<CustomerDocumentsItem> _customerDocumentsItemRepository;
        private readonly IEventPublisher _eventPublisher;

        public CustomerDocumentsService(IRepository<CustomerDocumentsItem> customerDocumentsItemRepository, IEventPublisher eventPublisher)
        {
            _customerDocumentsItemRepository = customerDocumentsItemRepository;
            _eventPublisher = eventPublisher;
        }
        public void DeleteDocument(CustomerDocumentsItem documentItem)
        {
            if (documentItem == null)
                throw new ArgumentNullException(nameof(documentItem));

            _customerDocumentsItemRepository.Delete(documentItem);

            //event notification
            _eventPublisher.EntityDeleted(documentItem);
        }

        public IList<CustomerDocumentsItem> GetCustomerDocuments(Customer customer)
        {
            if (customer == null)
                return null;

            return (from doc in _customerDocumentsItemRepository.Table

                    where doc.CustomerId == customer.Id
                    select doc).ToList();
        }

        public CustomerDocumentsItem GetDocumentItemById(int id)
        {
            return (from doc in _customerDocumentsItemRepository.Table

                    where doc.Id == id
                    select doc).FirstOrDefault();
        }

        public void InsertDocument(CustomerDocumentsItem documentItem)
        {
            if (documentItem == null)
                throw new ArgumentNullException(nameof(documentItem));

            _customerDocumentsItemRepository.Insert(documentItem);

            //event notification
            _eventPublisher.EntityInserted(documentItem);
        }
    }
}
