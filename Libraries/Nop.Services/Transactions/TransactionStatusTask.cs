using System;
using System.Linq;
using Nop.Core;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Tasks;
using Nop.Services.Vendors;

namespace Nop.Services.Transactions
{
    public partial class TransactionStatusTask : IScheduleTask
    {
        #region Fields

        private readonly ILogger _logger;
        private readonly ICustomerService _customerService;
        private readonly IWorkContext _workContext;
        private readonly ILocalizationService _localizationService;
        private readonly ITransactionService _transactionService;
        private readonly IStoreContext _storeContext;
        private readonly IVendorService _vendorService;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly IDateTimeHelper _dateTimeHelper;

        #endregion

        #region Ctor

        public TransactionStatusTask(
            ILogger logger,
            ICustomerService customerService,
            IWorkContext workContext,
            ILocalizationService localizationService,
            ITransactionService transactionService,
            IStoreContext storeContext,
            IVendorService vendorService,
            IWorkflowMessageService workflowMessageService,
            IDateTimeHelper dateTimeHelper)
        {
            _logger = logger;
            _customerService = customerService;
            _workContext = workContext;
            _localizationService = localizationService;
            _transactionService = transactionService;
            _storeContext = storeContext;
            _vendorService = vendorService;
            _workflowMessageService = workflowMessageService;
            _dateTimeHelper = dateTimeHelper;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Executes a task
        /// </summary>
        public virtual void Execute()
        {
            try
            {
                //get Transaction with status Waiting_For_Cauris_Transfer 
                var transactions = _transactionService.SchedulerTransactionList();


            }
            catch (Exception exc)
            {
                _logger.Error($"Error to update call scheduler. {exc.Message}", exc);
            }
        }

    }

    #endregion
}
