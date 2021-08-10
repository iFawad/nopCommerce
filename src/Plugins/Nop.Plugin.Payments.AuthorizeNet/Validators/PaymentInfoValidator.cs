using System;
using FluentValidation;
using Nop.Plugin.Payments.AuthorizeNet.Models;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Plugin.Payments.AuthorizeNet.Validators
{
    public class PaymentInfoValidator : BaseNopValidator<PaymentInfoModel>
    {
        #region Ctor

        public PaymentInfoValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.CardholderName)
                .NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("Payment.CardholderName.Required"));

            RuleFor(x => x.CardNumber)
                .IsCreditCard()
                .Matches(@"^[45]") //Only Visa or Mastercard allowed.
                .WithMessageAwait(localizationService.GetResourceAsync("Payment.CardNumber.Wrong"));

            RuleFor(x => x.CardCode)
                .Matches(@"^[0-9]{3,4}$")
                .WithMessageAwait(localizationService.GetResourceAsync("Payment.CardCode.Wrong"));

            RuleFor(x => x.ExpireMonth)
                .NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("Payment.ExpireMonth.Required"))
                .Must(x => Convert.ToInt32(x) >= DateTime.Now.Month)
                .WithMessage("The credit card expiry month is invalid")
                .When(f => Convert.ToInt32(f.ExpireYear) == DateTime.Now.Year);

            RuleFor(x => x.ExpireYear)
                .NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("Payment.ExpireYear.Required"))
                .Must(x => Convert.ToInt32(x) >= DateTime.Now.Year)
                //.When(f => !string.IsNullOrEmpty(f.ExpireYear))
                .WithMessage("The credit card expiry year is invalid");
        }

        #endregion
    }
}