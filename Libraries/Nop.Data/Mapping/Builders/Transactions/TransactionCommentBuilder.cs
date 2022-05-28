using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Transactions;

namespace Nop.Data.Mapping.Builders.Transactions
{
    public partial class TransactionCommentBuilder : NopEntityBuilder<TransactionComment>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(TransactionComment.ModeratorComment)).AsString().NotNullable();
        }

        #endregion
    }
}
