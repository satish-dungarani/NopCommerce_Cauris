using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Quotations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Data.Mapping.Builders.Quotations
{
    public class QuotationBuilder : NopEntityBuilder<Quotation>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                 .WithColumn(nameof(Quotation.CustomerId)).AsInt32().NotNullable()
                 .WithColumn(nameof(Quotation.VendorId)).AsInt32().NotNullable()
                 .WithColumn(nameof(Quotation.ProductId)).AsInt32().NotNullable()
                 .WithColumn(nameof(Quotation.Quantity)).AsFloat().NotNullable()
                 .WithColumn(nameof(Quotation.CountryId)).AsInt32().NotNullable()
                 .WithColumn(nameof(Quotation.CreationDate)).AsDateTime().NotNullable()
                 .WithColumn(nameof(Quotation.Status)).AsInt32().NotNullable()
                 .WithColumn(nameof(Quotation.StatusDate)).AsDateTime().NotNullable()
                 .WithColumn(nameof(Quotation.Specification)).AsString().NotNullable()
                 .WithColumn(nameof(Quotation.LeadTime)).AsDateTime().NotNullable()
                 .WithColumn(nameof(Quotation.UnitPrice)).AsFloat().NotNullable()
                 .WithColumn(nameof(Quotation.Price)).AsFloat().Nullable();
        }
    }
}
