using FluentValidation;
using Nop.Core.Domain.Transactions;
using Nop.Data;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Cauris.Transaction;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.Transactions
{
    public partial class TransactionValidator : BaseNopValidator<TransactionModel>
    {
        public TransactionValidator(ILocalizationService localizationService, INopDataProvider dataProvider)
        {
            RuleFor(x => x.ContractSignatureDate).NotEmpty().WithMessage(localizationService.GetResource("Corus.Admin.Transaction.Field.ContractSignatureDate.Required"));

            RuleFor(x => x.BuyerName).NotEmpty().WithMessage(localizationService.GetResource("Corus.Admin.Transaction.Field.BuyerName.Required"));
            RuleFor(x => x.SellerName).NotEmpty().WithMessage(localizationService.GetResource("Corus.Admin.Transaction.Field.SellerName.Required"));

            RuleFor(x => x.Quantity).NotEmpty().WithMessage(localizationService.GetResource("Corus.Admin.Transaction.Field.Quantity.Required"));
            RuleFor(x => x.Price).NotEmpty().WithMessage(localizationService.GetResource("Corus.Admin.Transaction.Field.Price.Required"));

            RuleFor(x => x.Tax).NotEmpty().WithMessage(localizationService.GetResource("Corus.Admin.Transaction.Field.Tax.Required"));
            RuleFor(x => x.PaymentTerm).NotEmpty().WithMessage(localizationService.GetResource("Corus.Admin.Transaction.Field.PaymentTerm.Required"));

            RuleFor(x => x.LoadingOrigin).NotEmpty().WithMessage(localizationService.GetResource("Corus.Admin.Transaction.Field.LoadingOrigin.Required"));
            RuleFor(x => x.Destination).NotEmpty().WithMessage(localizationService.GetResource("Corus.Admin.Transaction.Field.Destination.Required"));

            RuleFor(x => x.LastDateOfShipment).NotEmpty().WithMessage(localizationService.GetResource("Corus.Admin.Transaction.Field.LastDateOfShipment.Required"));
            RuleFor(x => x.DocumentsRequirement).NotEmpty().WithMessage(localizationService.GetResource("Corus.Admin.Transaction.Field.DocumentsRequirement.Required"));
            //RuleFor(x => x.ModeratorComment).NotEmpty().WithMessage(localizationService.GetResource("Corus.Admin.Transaction.Field.ModeratorComment.Required"));
            
            SetDatabaseValidationRules<Transaction>(dataProvider);
        }
    }
}