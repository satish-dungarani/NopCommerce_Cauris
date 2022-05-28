using Nop.Core.Domain.Quotations;
using Nop.Core.Domain.Transactions;
using Nop.Web.Models.Transactions;

namespace Nop.Web.Factories
{
    public interface ITransactionModelFactory
    {
        TransactionSearchModel PrepareTransactionSearchModel(TransactionSearchModel searchModel);
        TransactionListModel PrepareTransactionListModel(TransactionSearchModel searchModel);
        TransactionOverviewModel PrepareTransactionOverviewModel(Transaction transaction, Quotation quotation);

        TransactionListModel PrepareAjaxTransactionListModel(DataTableAjaxPostModel ajaxModel, TransactionSearchModel searchModel);
    }
}