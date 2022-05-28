using System;
using System.Collections.Generic;
using System.Text;
using Nop.Core.Domain.Customers;
using Nop.Plugin.Widgets.CustomerDocuments.Domain;

namespace Nop.Plugin.Widgets.CustomerDocuments.Services
{
   public interface ICustomerDocumentsService
    {
        IList<CustomerDocumentsItem> GetCustomerDocuments(Customer customer);
        CustomerDocumentsItem GetDocumentItemById(int id);
        void DeleteDocument(CustomerDocumentsItem documentItem);
        void InsertDocument(CustomerDocumentsItem documentItem);
    }
}
