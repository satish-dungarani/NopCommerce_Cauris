using Nop.Core.Domain.Quotations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Services.Quotations
{
    public interface IQuotationService
    {
        IEnumerable<Quotation> GetAllByCustomerId(int customerId);
        IEnumerable<Quotation> GetAllByVendorId(int vendorId);
        IEnumerable<Quotation> GetAllByStatus(int status);
        Quotation GetByCustomerIdProductId(int customerId, int productId);
        Quotation Get(int id);
        Quotation Save(Quotation quotation);
        void Delete(Quotation quotation);
    }
}
