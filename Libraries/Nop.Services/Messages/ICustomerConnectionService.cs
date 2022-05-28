using Nop.Core.Domain.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Services.Messages
{
    public interface ICustomerConnectionService
    {
        CustomerConnection GetByConnectionId(string connectionId);
        CustomerConnection GetByCustomerId(int customerId);
        void Add(CustomerConnection customerConnection);
        void Update(CustomerConnection customerConnection);
        void Delete(int customerConnection);
    }
}
