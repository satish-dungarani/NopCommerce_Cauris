using Nop.Data.Extensions;
using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Customers;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Widgets.CustomerDocuments.Domain;
using Nop.Core.Domain.Stores;
using System;

namespace Nop.Plugin.Widgets.CustomerDocuments.Data
{
    public class CustomerDocumentsItemBuilder : NopEntityBuilder<CustomerDocumentsItem>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(CustomerDocumentsItem.DownloadId)).AsInt32().NotNullable()
                .WithColumn(nameof(CustomerDocumentsItem.FileName)).AsString().NotNullable()
                .WithColumn(nameof(CustomerDocumentsItem.DocumentType)).AsInt32().NotNullable()
                .WithColumn(nameof(CustomerDocumentsItem.StoreId)).AsInt32().ForeignKey<Store>()
                .WithColumn(nameof(CustomerDocumentsItem.CustomerId)).AsInt32().ForeignKey<Customer>()
                .WithColumn(nameof(CustomerDocumentsItem.CreatedOnUtc)).AsDateTime().WithDefaultValue(DateTime.Now)
                .WithColumn(nameof(CustomerDocumentsItem.UpdatedOnUtc)).AsDateTime();
              
        }
    }
}
