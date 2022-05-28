using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Data.Mapping.Builders.Messages
{
    public class CustomerConnectionBuilder : NopEntityBuilder<CustomerConnection>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(CustomerConnection.ConnectionId)).AsString(int.MaxValue).NotNullable()
                .WithColumn(nameof(CustomerConnection.CustomerId)).AsInt32().NotNullable();
        }
    }
}
