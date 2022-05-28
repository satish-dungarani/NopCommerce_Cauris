using Nop.Core.Domain.Tax;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Services.Tax
{
    public interface IFeesService
    {
        IEnumerable<Fees> GetAll();
        Fees Get(int id);
        Fees Save(Fees fees);
        void Delete(Fees fees);
    }
}
