using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Notifications;
using Nop.Core.Domain.Transactions;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Notifications;
using Nop.Services.Quotations;
using Nop.Services.Security;
using Nop.Services.Transactions;
using Nop.Web.Areas.Admin.Factories.Corus;
using Nop.Web.Areas.Admin.Models.Cauris.Transaction;

namespace Nop.Web.Areas.Admin.Controllers.Corus
{
    public class TransactionController : BaseAdminController
    {
        #region Fields

        private readonly ITransactionModelFactory _transactionModelFactory;
        private readonly IPermissionService _permissionService;
        private readonly ITransactionService _transactionService;
        private readonly INotificationService _notificationService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly IWorkContext _workContext;
        private readonly IQuotationService _quotationService;
        private readonly ICustomerService _customerService;
        private readonly ICustomNotificationService _customNotificationService;

        #endregion

        #region Ctor

        public TransactionController(ITransactionModelFactory transactionModelFactory,
             IPermissionService permissionService,
             ITransactionService transactionService,
             INotificationService notificationService,
             ILocalizationService localizationService,
             IWorkflowMessageService workflowMessageService,
             IWorkContext workContext,
             IQuotationService quotationService,
             ICustomerService customerService,
             ICustomNotificationService customNotificationService)
        {
            _transactionModelFactory = transactionModelFactory;
            _permissionService = permissionService;
            _transactionService = transactionService;
            _notificationService = notificationService;
            _localizationService = localizationService;
            _workflowMessageService = workflowMessageService;
            _workContext = workContext;
            _quotationService = quotationService;
            _customerService = customerService;
            _customNotificationService = customNotificationService;
        }

        #endregion

        #region Method

        public virtual IActionResult List(List<int> transactionStatus = null)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCaurisTransaction))
                return AccessDeniedView();

            //prepare model
            var model = _transactionModelFactory.PrepareTransactionSearchModel(new TransactionSearchModel
            {
                TransactionStatusIds = transactionStatus,
            });

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult List(TransactionSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCaurisTransaction))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = _transactionModelFactory.PrepareTransactionListModel(searchModel);

            return Json(model);
        }

        public virtual IActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCaurisTransaction))
                return AccessDeniedView();

            var transaction = _transactionService.GetTransactionById(id);
            if (transaction == null)
                return RedirectToAction("List");

            var model = _transactionModelFactory.PrepareTransactionModel(null, transaction);

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult Edit(TransactionModel model, IFormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCaurisTransaction))
                return AccessDeniedView();
            //try to get a customer with the specified id
            var transaction = _transactionService.GetTransactionById(model.Id);
            if (transaction == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                try
                {
                    #region Transaction Insert

                    transaction.ContractSignatureDate = model.ContractSignatureDate;
                    //BUYER SELLER PENDING 
                    //transaction. = model.BuyerName;
                    transaction.Amount = model.Price;
                    transaction.Quantity = model.Quantity;
                    transaction.Tax = model.Tax;
                    transaction.PaymentTerm = model.PaymentTerm;
                    transaction.Inspection = model.Inspection;
                    transaction.Insurance = model.Insurance;
                    transaction.PartialShipping = model.PartialShipping;
                    transaction.TransactionStatusId = (int)TransactionStatus.Waiting_Seller_Proof_Of_Payment;
                    transaction.DeliveryTermId = model.DeliveryTermId;
                    transaction.LoadingOrigin = model.LoadingOrigin;
                    transaction.Destination = model.Destination;
                    transaction.LastDateOfShipment = model.LastDateOfShipment;
                    transaction.DocumentsRequirement = model.DocumentsRequirement;

                    _transactionService.UpdateTransaction(transaction);

                    #endregion

                    #region Transaction Comment
                    if(string.IsNullOrEmpty(model.ModeratorComment)==false)
                    {
                        var transComment = new TransactionComment()
                        {
                            TransactionId = transaction.Id,
                            ModeratorComment = model.ModeratorComment,
                            CreatedDate = DateTime.UtcNow,
                            ModeratorId = transaction.ModeratorIndentity
                        };
                        _transactionService.SaveTransactionComment(transComment);
                    }
                   
                    #endregion
                    #region Need to add the message template 

                    var quotation = _quotationService.Get(transaction.QuotationId);
                    if (quotation != null)
                    {
                        /// ADD the email and message template 
                        _workflowMessageService.ModeratorValidateContractNotification(transaction.Id, quotation.CustomerId, _workContext.WorkingLanguage.Id);
                        _workflowMessageService.ModeratorValidateContractNotification(transaction.Id, _customerService.GetCustomerByVendorId(quotation.VendorId).Id, _workContext.WorkingLanguage.Id);

                        var vendorNotification = new Notification
                        {
                            EntityId = transaction.Id,
                            EntityName = "Transaction",
                            CreatedBy = _workContext.CurrentCustomer.Id,
                            CreatedFor = _customerService.GetCustomerByVendorId(quotation.VendorId).Id,
                            IsRead = false,
                            Description = "Moderator has been validate the contract",
                            CreatedDate = DateTime.UtcNow
                        };
                        _customNotificationService.InsertNotification(vendorNotification);

                        var customerNotification = new Notification
                        {
                            EntityId = transaction.Id,
                            EntityName = "Transaction",
                            CreatedBy = _workContext.CurrentCustomer.Id,
                            CreatedFor = quotation.CustomerId,
                            IsRead = false,
                            Description = "Moderator has been validate the contract",
                            CreatedDate = DateTime.UtcNow
                        };
                        _customNotificationService.InsertNotification(customerNotification);
                    }

                    #endregion
                    _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Customers.Customers.Updated"));

                    return RedirectToAction("Edit", new { id = transaction.Id });
                }
                catch (Exception exc)
                {
                    _notificationService.ErrorNotification(exc.Message);
                }
            }

            ////prepare model
            model = _transactionModelFactory.PrepareTransactionModel(null, transaction);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual IActionResult TransactionClarify(string transactionComment, int transactionId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCaurisTransaction))
                return AccessDeniedView();

            if (transactionId == 0)
                throw new ArgumentException();

            //try to get a customer with the specified id
            var transaction = _transactionService.GetTransactionById(transactionId)
                ?? throw new ArgumentException("No transaction found with the specified id");

            //if (transaction != null)
            //{
            //    var transComment = new TransactionComment()
            //    {
            //        TransactionId = transactionId,
            //        ModeratorComment = transactionComment,
            //        CreatedDate = DateTime.UtcNow,
            //        ModeratorId = transaction.ModeratorIndentity
            //    };
            //    _transactionService.SaveTransactionComment(transComment);
            //}

            //Update the contract Status 
            transaction.TransactionStatusId = (int)TransactionStatus.Waiting_Validation_Contract_By_Moderator;
            _transactionService.UpdateTransaction(transaction);

            #region Need to add the message template for admin comment 
            /// ADD the email and message template 

            var quotation = _quotationService.Get(transaction.QuotationId);
            if (quotation != null)
            {
                _workflowMessageService.ModeratorValidateContractNotification(transaction.Id, quotation.CustomerId, _workContext.WorkingLanguage.Id);
                _workflowMessageService.ModeratorValidateContractNotification(transaction.Id, _customerService.GetCustomerByVendorId(quotation.VendorId).Id, _workContext.WorkingLanguage.Id);

                var vendorNotification = new Notification
                {
                    EntityId = transaction.Id,
                    EntityName = "Transaction",
                    CreatedBy = _workContext.CurrentCustomer.Id,
                    CreatedFor = _customerService.GetCustomerByVendorId(quotation.VendorId).Id,
                    IsRead = false,
                    Description = "Moderator has been request for clarification the contract",
                    CreatedDate = DateTime.UtcNow
                };
                _customNotificationService.InsertNotification(vendorNotification);

                var customerNotification = new Notification
                {
                    EntityId = transaction.Id,
                    EntityName = "Transaction",
                    CreatedBy = _workContext.CurrentCustomer.Id,
                    CreatedFor = quotation.CustomerId,
                    IsRead = false,
                    Description = "Moderator has been request for clarification the contract",
                    CreatedDate = DateTime.UtcNow
                };
                _customNotificationService.InsertNotification(customerNotification);
            }

            #endregion

            return Json(new { Result = true });
        }


        public virtual IActionResult TransactionFundsAvailable(int transactionId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCaurisTransaction))
                return AccessDeniedView();

            if (transactionId == 0)
                throw new ArgumentException();

            //try to get a customer with the specified id
            var transaction = _transactionService.GetTransactionById(transactionId)
                ?? throw new ArgumentException("No transaction found with the specified id");

            //Update the contract Status 
            transaction.TransactionStatusId = (int)TransactionStatus.Waiting_Proof_Vendor_Delivery;
            _transactionService.UpdateTransaction(transaction);

            #region Need to add the message template for admin comment 
            /// ADD the email and message template 

            var quotation = _quotationService.Get(transaction.QuotationId);
            if (quotation != null)
            {
                //_workflowMessageService.ModeratorValidateContractNotification(transaction.Id, quotation.CustomerId, _workContext.WorkingLanguage.Id);
                //_workflowMessageService.ModeratorValidateContractNotification(transaction.Id, _customerService.GetCustomerByVendorId(quotation.VendorId).Id, _workContext.WorkingLanguage.Id);

                var vendorNotification = new Notification
                {
                    EntityId = transaction.Id,
                    EntityName = "Transaction",
                    CreatedBy = _workContext.CurrentCustomer.Id,
                    CreatedFor = _customerService.GetCustomerByVendorId(quotation.VendorId).Id,
                    IsRead = false,
                    Description = _localizationService.GetResource("TransactionFundsAvailable.Notification.Seller.Waiting_Proof"),
                     CreatedDate = DateTime.UtcNow
                };
                _customNotificationService.InsertNotification(vendorNotification);

                var customerNotification = new Notification
                {
                    EntityId = transaction.Id,
                    EntityName = "Transaction",
                    CreatedBy = _workContext.CurrentCustomer.Id,
                    CreatedFor = quotation.CustomerId,
                    IsRead = false,
                    Description = _localizationService.GetResource("TransactionFundsAvailable.Notification.Buyer.Waiting_Proof"),
                   CreatedDate = DateTime.UtcNow
                };
                _customNotificationService.InsertNotification(customerNotification);
            }

            #endregion

            return Json(new { Result = true });
        }
        #endregion

    }
}
