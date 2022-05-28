using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Quotations;
using Nop.Core.Domain.Transactions;
using Nop.Core.Domain.Vendors;
using Nop.Data;
using Nop.Services.Customers;
using Nop.Services.Events;
using Nop.Services.Vendors;

namespace Nop.Services.Transactions
{
    public partial class TransactionService : ITransactionService
    {
        #region Fields
        private readonly IRepository<Quotation> _quotationRepository;
        private readonly IRepository<Transaction> _transactionRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Vendor> _vendorRepository;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<TransactionComment> _transactionCommentRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly IWorkContext _workContext;
        private readonly ICustomerService _customerService;
        private readonly IVendorService _vendorService;
        private readonly IRepository<GenericAttribute> _gaRepository;
        private readonly IRepository<Address> _addressRepository;
        #endregion

        #region Ctor

        public TransactionService(IRepository<Quotation> quotationRepository,
            IRepository<Transaction> transactionRepository,
            IEventPublisher eventPublisher,
            IRepository<Product> productRepository,
            IWorkContext workContext,
            IRepository<Vendor> vendorRepository,
            IRepository<Customer> customerRepository,
            IRepository<TransactionComment> transactionCommentRepository, ICustomerService customerService, IVendorService vendorService, IRepository<GenericAttribute> gaRepository = null, IRepository<Address> addressRepository = null)
        {
            _quotationRepository = quotationRepository;
            _transactionRepository = transactionRepository;
            _eventPublisher = eventPublisher;
            _productRepository = productRepository;
            _workContext = workContext;
            _vendorRepository = vendorRepository;
            _customerRepository = customerRepository;
            _transactionCommentRepository = transactionCommentRepository;
            _customerService = customerService;
            _vendorService = vendorService;
            _gaRepository = gaRepository;
            _addressRepository = addressRepository;
        }

        #endregion

        #region Methods

        public virtual void SaveTransaction(Transaction transactionDto)
        {
            if (transactionDto == null)
                throw new ArgumentNullException(nameof(transactionDto));

            if (transactionDto.Id == 0)
            {
                _transactionRepository.Insert(transactionDto);
                _eventPublisher.EntityInserted(transactionDto);
            }
            else
            {
                _transactionRepository.Update(transactionDto);
                _eventPublisher.EntityUpdated(transactionDto);
            }
        }

        public virtual void DeleteTransaction(Transaction transactionDto)
        {
            if (transactionDto == null)
                throw new ArgumentNullException(nameof(transactionDto));

            _transactionRepository.Delete(transactionDto);
            _eventPublisher.EntityDeleted(transactionDto);
        }

        public virtual IList<Transaction> GetAllTransactions()
        {
            return _transactionRepository.Table.ToList();
        }

        public virtual Transaction GetTransactionById(int id)
        {
            return _transactionRepository.GetById(id);
        }

        public virtual IList<Transaction> GetTransactionByModeratorId(int moderatorId)
        {
            return _transactionRepository.Table.Where(t => t.CaurisModeratorId == moderatorId).ToList();
        }

        public virtual IList<Transaction> GetTransactionByQuotationId(int quotationId)
        {
            return _transactionRepository.Table.Where(t => t.QuotationId == quotationId).ToList();
        }

        public virtual IPagedList<Transaction> SearchTransactions(string keyword = "",
            int vendorId = 0,
            int moderatorId = 0,
            int customerId = 0,
            int deliveryTermId = 0,
            int transactionStatusId = 0,
            int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _transactionRepository.Table;

            if (moderatorId > 0)
                query = query.Where(t => t.CaurisModeratorId == moderatorId);

            if (deliveryTermId > 0)
                query = query.Where(t => t.DeliveryTermId == deliveryTermId);

            //if (paymentTypeId > 0)
            //    query = query.Where(t => t.PaymentTypeId == paymentTypeId);

            if (transactionStatusId > 0)
                query = query.Where(t => t.TransactionStatusId == transactionStatusId);

            if (vendorId > 0)
            {
                query = from t in query
                        join q in _quotationRepository.Table on t.QuotationId equals q.Id
                        join p in _productRepository.Table on q.ProductId equals p.Id
                        where p.VendorId == vendorId
                        select t;
            }

            if (customerId > 0)
            {
                query = from t in query
                        join q in _quotationRepository.Table on t.QuotationId equals q.Id
                        where q.CustomerId == customerId
                        select t;
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                query = from t in query
                        join q in _quotationRepository.Table on t.QuotationId equals q.Id
                        join p in _productRepository.Table on q.ProductId equals p.Id
                        join v in _vendorRepository.Table on q.VendorId equals v.Id
                        join c in _customerRepository.Table on q.CustomerId equals c.Id
                        where p.Name.Contains(keyword) || (vendorId > 0 ? v.Name.Contains(keyword) : c.Email.Contains(keyword))
                        select t;
            }

            var transactions = query.OrderByDescending(x => x.CreatedOnUtc).ToList();
            //paging
            return new PagedList<Transaction>(transactions, pageIndex, pageSize);
        }

        public virtual IPagedList<Transaction> SearchTransactions2(string name = null,
            int country = 0, string date = null, string transnumber = null, string productname = null,
            int transactionStatusId = 0,
            int vendorId = 0,
            int moderatorId = 0,
            int customerId = 0,
            int columnOrder = 0,
            string orderDir = "desc",
            int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _transactionRepository.Table;

            if (transactionStatusId > 0)
                query = query.Where(t => t.TransactionStatusId == transactionStatusId);

            if (moderatorId > 0)
                query = query.Where(t => t.CaurisModeratorId == moderatorId);

            if (string.IsNullOrEmpty(date) == false)
            {
                DateTime dt = Convert.ToDateTime(date);
                query = query.Where(t => t.CreatedOnUtc > dt);
            }

            if (vendorId > 0)
            {
                query = from t in query
                        join q in _quotationRepository.Table on t.QuotationId equals q.Id
                        join p in _productRepository.Table on q.ProductId equals p.Id
                        where p.VendorId == vendorId
                        select t;

                if (country > 0)
                {
                    query = from t in query
                            join q in _quotationRepository.Table on t.QuotationId equals q.Id
                            where q.CountryId == country
                            select t;
                }

                if (!string.IsNullOrEmpty(name))
                {
                    query = from t in query
                            join q in _quotationRepository.Table on t.QuotationId equals q.Id
                            join c in _customerRepository.Table on q.CustomerId equals c.Id
                            join g in _gaRepository.Table on q.CustomerId equals g.EntityId
                            where g.KeyGroup == nameof(Customer)
                            && (g.Key == NopCustomerDefaults.FirstNameAttribute || g.Key == NopCustomerDefaults.LastNameAttribute)
                            && g.Value.Contains(name)
                            select t;
                }
            }

            if (customerId > 0)
            {
                query = from t in query
                        join q in _quotationRepository.Table on t.QuotationId equals q.Id
                        join p in _productRepository.Table on q.ProductId equals p.Id
                        where q.CustomerId == customerId
                        select t;

                if (!string.IsNullOrEmpty(name))
                {
                    query = from t in query
                            join q in _quotationRepository.Table on t.QuotationId equals q.Id
                            join p in _productRepository.Table on q.ProductId equals p.Id
                            join v in _vendorRepository.Table on p.VendorId equals v.Id
                            where v.Name.Contains(name)
                            select t;
                }

                if (country > 0)
                {
                    query = from t in query
                            join q in _quotationRepository.Table on t.QuotationId equals q.Id
                            join p in _productRepository.Table on q.ProductId equals p.Id
                            join v in _vendorRepository.Table on p.VendorId equals v.Id
                            join ad in _addressRepository.Table on v.AddressId equals ad.Id
                            where ad.CountryId == country
                            select t;
                }
            }

            if (!string.IsNullOrEmpty(productname))
            {
                query = from t in query
                        join q in _quotationRepository.Table on t.QuotationId equals q.Id
                        join p in _productRepository.Table on q.ProductId equals p.Id
                        where p.Name.Contains(productname)
                        select t;
            }
            if (!string.IsNullOrEmpty(transnumber))
            {
                query = from t in query
                        where t.Id.ToString().Contains(transnumber)
                        select t;
            }


            //Sorting feature
            query = columnOrder switch
            {
                1 => orderDir.Equals("asc") ? query.OrderBy(x => x.TransactionNumber) : query.OrderByDescending(x => x.TransactionNumber),
                2 => orderDir.Equals("asc") ? query.OrderBy(x => x.CreatedOnUtc) : query.OrderByDescending(x => x.CreatedOnUtc),
                //3 => orderDir.Equals("asc") ? query.OrderBy(x => x.VendorName) : tranList.OrderByDescending(x => x.VendorName),
                //4 => orderDir.Equals("asc") ? query.OrderBy(x => x.VendorCountry) : tranList.OrderByDescending(x => x.VendorCountry),
                //5 => orderDir.Equals("asc") ? query.OrderBy(x => x.CustomerName) : tranList.OrderByDescending(x => x.CustomerName),
                //6 => orderDir.Equals("asc") ? query.OrderBy(x => x.Country) : tranList.OrderByDescending(x => x.Country),
                //7 => orderDir.Equals("asc") ? query.OrderBy(x => x.ProductName) : tranList.OrderByDescending(x => x.ProductName),
                8 => orderDir.Equals("asc") ? query.OrderBy(x => x.TransactionStatus) : query.OrderByDescending(x => x.TransactionStatus),
                _ => orderDir.Equals("asc") ? query.OrderBy(x => x.Id) : query.OrderByDescending(x => x.Id)
            };
            //var transactions = query.OrderByDescending(x => x.CreatedOnUtc).ToList();
            //paging
            return new PagedList<Transaction>(query, pageIndex, pageSize);
        }

        public virtual IList<Transaction> SchedulerTransactionList()
        {
            var query = _transactionRepository.Table
                .Where(t => t.TransactionStatusId == (int)TransactionStatus.Waiting_For_Cauris_Transfer)
                .ToList();

            return query;
        }
        public void UpdateTransaction(Transaction transaction)
        {
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));

            _transactionRepository.Update(transaction);

            _eventPublisher.EntityUpdated(transaction);
        }

        #region Admin Factory

        public IPagedList<Transaction> SearchAdminTransaction(List<int> tsIds = null, int productId = 0, int transactionNumber = 0,
            int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false)
        {
            var query = _transactionRepository.Table;

            if (tsIds != null && tsIds.Any())
                query = query.Where(o => tsIds.Contains(o.TransactionStatusId));

            //if (transactionNumber > 0)
            //    query = query.Where(p => p.TransactionNumber == transactionNumber);

            if (productId > 0)
                query = from o in query
                        join oi in _quotationRepository.Table on o.Id equals oi.ProductId
                        where oi.ProductId == productId
                        select o;

            query = query.OrderByDescending(o => o.CreatedOnUtc);

            //database layer paging
            return new PagedList<Transaction>(query, pageIndex, pageSize, getOnlyTotalCount);

        }


        #endregion

        #endregion

        #region Delivery Transaction Comment
        public TransactionComment GetTransactionCommentById(int id)
        {
            return _transactionCommentRepository.GetById(id);
        }

        public IList<TransactionComment> GetAllTransactionsCommentByTransactionId(int transacationById)
        {
            var transactionList = (from tc in _transactionCommentRepository.Table
                                   where tc.TransactionId == transacationById
                                   orderby tc.Id descending
                                   select tc).ToList();

            return transactionList;
        }
        public TransactionComment GetTransactionCommentByTransactionId(int transacationById)
        {
            var comments = (from tc in _transactionCommentRepository.Table
                          where tc.TransactionId == transacationById
                          orderby tc.Id descending
                          select tc).Take(1).ToList();
                   
            return comments.LastOrDefault();    
        }

        public IList<TransactionComment> GetAllTransactionsComment()
        {
            return _transactionCommentRepository.Table.ToList();
        }

        public void DeleteTransactionComment(TransactionComment transactionComment)
        {
            if (transactionComment == null)
                throw new ArgumentNullException(nameof(transactionComment));

            _transactionCommentRepository.Delete(transactionComment);
            _eventPublisher.EntityDeleted(transactionComment);
        }

        public void SaveTransactionComment(TransactionComment transactionComment)
        {
            if (transactionComment == null)
                throw new ArgumentNullException(nameof(transactionComment));

            _transactionCommentRepository.Insert(transactionComment);
            _eventPublisher.EntityInserted(transactionComment);
        }

        public void UpdateTransactionComment(TransactionComment transactionComment)
        {
            if (transactionComment == null)
                throw new ArgumentNullException(nameof(transactionComment));

            _transactionCommentRepository.Update(transactionComment);
            _eventPublisher.EntityUpdated(transactionComment);
        }

        #endregion
    }
}
