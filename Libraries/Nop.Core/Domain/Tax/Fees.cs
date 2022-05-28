using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Core.Domain.Tax
{
    public class Fees : BaseEntity
    {
        public decimal Percent { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }
    }
}
