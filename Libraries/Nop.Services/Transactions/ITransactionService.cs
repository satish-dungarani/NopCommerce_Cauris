using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Domain.Transactions;

namespace Nop.Services.Transactions
{
    public interface ITransactionService
    {
        IPagedList<Transaction> SearchTransactions(string keyword = "", int vendorId = 0, int moderatorId = 0, int customerId = 0,
            int deliveryTermId = 0, int transactionStatusId = 0,
            int pageIndex = 0, int pageSize = int.MaxValue);

        IList<Transaction> GetTransactionByQuotationId(int quotationId);

        IList<Transaction> GetTransactionByModeratorId(int moderatorId);

        Transaction GetTransactionById(int id);

        IList<Transaction> GetAllTransactions();

        void DeleteTransaction(Transaction transactionDto);

        void SaveTransaction(Transaction transactionDto);

        void UpdateTransaction(Transaction transactionDto);

        /// <summary>
        /// Search Transaction
        /// </summary>

        /// <param name="tsIds">Transaction status identifiers; null to load all orders</param>
        /// <param name="psIds">Payment status identifiers; null to load all orders</param>
        /// <param name="billingEmail">Billing email. Leave empty to load all records.</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="getOnlyTotalCount">A value in indicating whether you want to load only total number of records. Set to "true" if you don't want to load data from database</param>
        /// <returns>Orders</returns>
        IPagedList<Transaction> SearchAdminTransaction(
            List<int> tsIds = null, int productId = 0, int transactionNumber = 0, int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false);

        IList<Transaction> SchedulerTransactionList();


        #region Delivery Transaction Comment 

        TransactionComment GetTransactionCommentById(int id);

        TransactionComment GetTransactionCommentByTransactionId(int transacationById);

        IList<TransactionComment> GetAllTransactionsCommentByTransactionId(int transacationById);

        IList<TransactionComment> GetAllTransactionsComment();

        void DeleteTransactionComment(TransactionComment transactionComment);

        void SaveTransactionComment(TransactionComment transactionComment);

        void UpdateTransactionComment(TransactionComment transactionComment);

        #endregion

        IPagedList<Transaction> SearchTransactions2(string name = null,
            int country = 0, string date = null, string transnumber = null, string productname = null,
            int transactionStatusId = 0,
            int vendorId = 0,
            int moderatorId = 0,
            int customerId = 0,
            int columnOrder = 0,
            string orderDir = "desc",
            int pageIndex = 0, int pageSize = int.MaxValue);
    }
}