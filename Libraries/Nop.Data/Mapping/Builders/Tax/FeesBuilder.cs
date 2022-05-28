using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Tax;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Data.Mapping.Builders.Tax
{
    public class FeesBuilder : NopEntityBuilder<Fees>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                 .WithColumn(nameof(Fees.Label)).AsString().NotNullable()
                 .WithColumn(nameof(Fees.Percent)).AsDecimal().NotNullable()
                 .WithColumn(nameof(Fees.Description)).AsString().Nullable();
        }
    }
}
