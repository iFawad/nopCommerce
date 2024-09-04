using FluentValidation;
using Nop.Plugin.DiscountRules.Ghost.MinCartAmount.Models;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Plugin.DiscountRules.Ghost.MinCartAmount.Validators;

/// <summary>
/// Represents an <see cref="RequirementModel"/> validator.
/// </summary>
public class RequirementModelValidator : BaseNopValidator<RequirementModel>
{
    public RequirementModelValidator(ILocalizationService localizationService)
    {
        RuleFor(model => model.DiscountId)
            .NotEmpty()
            .WithMessageAwait(localizationService.GetResourceAsync("Plugins.DiscountRules.Ghost.MinCartAmount.Fields.DiscountId.Required"));
        RuleFor(model => model.MinCartAmount)
            .GreaterThan(0)
            .WithMessageAwait(localizationService.GetResourceAsync("Plugins.DiscountRules.Ghost.MinCartAmount.Fields.MinCartAmount.Required"));
    }
}
