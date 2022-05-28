using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Nop.Core.Domain.Quotations
{
    public enum QuotationStatus
    {
        [Description("Customer Sent")]
        /// <summary>
        ///  Customer request a quotation
        /// </summary>
        CustomerSend = 10,
        [Description("Moderation Pending")]
        /// <summary>
        /// if specification text contains moderation the status passed to pending 
        /// then the moderator can validate and reset to CustomerSend status
        /// </summary>
        ModerationPending = 20,

        [Description("Vendor Response")]
        /// <summary>
        /// After CustomerSend vendor recive a request then response to the request
        /// the status passed to vendorSend
        /// </summary>
        VendorSend = 30,
        [Description("Customer Accecpt")]
        /// <summary>
        /// customer accept quotation 
        /// </summary>
        CustomerAccept = 40,
        [Description("Customer Refuse")]
        /// <summary>
        /// Customer refuse quotation
        /// </summary>
        CustomerRefuse = 50,
        [Description("Moderator Refuse")]
        /// <summary>
        /// Moderator Refuse
        /// </summary>
        ModeratorRefuse = 60
    }
}
