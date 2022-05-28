using System.Data;
using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Quotations;
using Nop.Core.Domain.Transactions;
using Nop.Data.Extensions;

namespace Nop.Data.Mapping.Builders.Transactions
{
    /// <summary>
    /// Represents a order entity builder
    /// </summary>
    public partial class TransactionBuilder : NopEntityBuilder<Transaction>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(Transaction.QuotationId)).AsInt32().ForeignKey<Quotation>(onDelete: Rule.None)
                .WithColumn(nameof(Transaction.CaurisModeratorId)).AsInt32().Nullable().ForeignKey<Customer>(onDelete: Rule.None)
                .WithColumn(nameof(Transaction.VendorDocumentId)).AsInt32().Nullable().ForeignKey<Download>(onDelete: Rule.None)
                .WithColumn(nameof(Transaction.CaurisDocumentId)).AsInt32().Nullable().ForeignKey<Download>(onDelete: Rule.None)
                .WithColumn(nameof(Transaction.CustomerDocumentId)).AsInt32().Nullable().ForeignKey<Download>(onDelete: Rule.None)
                .WithColumn(nameof(Transaction.PaymentProofDocumentId)).AsInt32().Nullable().ForeignKey<Download>(onDelete: Rule.None);
        }

        #endregion
    }
}