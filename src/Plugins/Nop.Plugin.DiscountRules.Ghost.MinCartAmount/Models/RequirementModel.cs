using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.DiscountRules.Ghost.MinCartAmount.Models;

public record RequirementModel
{
    public int DiscountId { get; set; }

    public int RequirementId { get; set; }

    [NopResourceDisplayName("Plugins.DiscountRules.Ghost.MinCartAmount.Fields.Amount")]
    public decimal MinCartAmount { get; set; }
}