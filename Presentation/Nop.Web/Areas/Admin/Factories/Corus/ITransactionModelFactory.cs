using Nop.Core.Domain.Transactions;
using Nop.Web.Areas.Admin.Models.Cauris.Transaction;

namespace Nop.Web.Areas.Admin.Factories.Corus
{
    public interface ITransactionModelFactory
    {
        /// <summary>
        /// Prepare transaction search model
        /// </summary>
        /// <param name="searchModel">Transaction search model</param>
        /// <returns>Transaction search model</returns>
        TransactionSearchModel PrepareTransactionSearchModel(TransactionSearchModel searchModel);

        /// <summary>
        /// Prepare paged transaction list model
        /// </summary>
        /// <param name="searchModel">Transaction search model</param>
        /// <returns>Transaction list model</returns>
        TransactionListModel PrepareTransactionListModel(TransactionSearchModel searchModel);

        /// <summary>
        /// Prepare transaction model
        /// </summary>
        /// <param name="model">transaction model</param>
        /// <param name="transaction">transaction</param>
        /// <returns>transaction model</returns>
        TransactionModel PrepareTransactionModel(TransactionModel model, Transaction transaction);
    }
}
