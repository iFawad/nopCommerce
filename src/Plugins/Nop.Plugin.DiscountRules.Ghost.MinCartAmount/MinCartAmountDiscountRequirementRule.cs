using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Plugins;

namespace Nop.Plugin.DiscountRules.Ghost.MinCartAmount;

public partial class MinCartAmountDiscountRequirementRule : BasePlugin, IDiscountRequirementRule
{
    private readonly IActionContextAccessor _actionContextAccessor;
    private readonly ICustomerService _customerService;
    private readonly IDiscountService _discountService;
    private readonly ILocalizationService _localizationService;
    private readonly IOrderService _orderService;
    private readonly ISettingService _settingService;
    private readonly IUrlHelperFactory _urlHelperFactory;
    private readonly IWebHelper _webHelper;

    public MinCartAmountDiscountRequirementRule(IActionContextAccessor actionContextAccessor,
        ICustomerService customerService,
        IDiscountService discountService,
        ILocalizationService localizationService,
        IOrderService orderService,
        ISettingService settingService,
        IUrlHelperFactory urlHelperFactory,
        IWebHelper webHelper)
    {
        _actionContextAccessor = actionContextAccessor;
        _customerService = customerService;
        _discountService = discountService;
        _localizationService = localizationService;
        _orderService = orderService;
        _settingService = settingService;
        _urlHelperFactory = urlHelperFactory;
        _webHelper = webHelper;
    }

    /// <summary>
    /// Check discount requirement
    /// </summary>
    /// <param name="request">Object that contains all information required to check the requirement (Current customer, discount, etc)</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result
    /// </returns>
    public async Task<DiscountRequirementValidationResult> CheckRequirementAsync(DiscountRequirementValidationRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        //invalid by default
        var result = new DiscountRequirementValidationResult();

        var spentAmountRequirement = await _settingService.GetSettingByKeyAsync<decimal>($"DiscountRequirement.MinCartAmount-{request.DiscountRequirementId}");
        if (spentAmountRequirement == decimal.Zero)
        {
            //valid
            result.IsValid = true;
            return result;
        }

        if (request.Customer == null || await _customerService.IsGuestAsync(request.Customer))
            return result;

        var orders = await _orderService.SearchOrdersAsync(request.Store.Id,
            customerId: request.Customer.Id,
            osIds: new List<int> { (int)OrderStatus.Complete });
        var spentAmount = orders.Sum(o => o.OrderTotal);
        if (spentAmount > spentAmountRequirement)
        {
            result.IsValid = true;
        }
        else
        {
            result.UserError = await _localizationService.GetResourceAsync("Plugin.DiscountRules.Ghost.MinCartAmount.NotEnough");
        }

        return result;
    }

    /// <summary>
    /// Get URL for rule configuration
    /// </summary>
    /// <param name="discountId">Discount identifier</param>
    /// <param name="discountRequirementId">Discount requirement identifier (if editing)</param>
    /// <returns>URL</returns>
    public string GetConfigurationUrl(int discountId, int? discountRequirementId)
    {
        var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);

        return urlHelper.Action("Configure", "DiscountRulesMinCartAmount",
            new { discountId = discountId, discountRequirementId = discountRequirementId }, _webHelper.GetCurrentRequestProtocol());
    }

    /// <summary>
    /// Install the plugin
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public override async Task InstallAsync()
    {
        //locales
        await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
        {
            ["Plugin.DiscountRules.Ghost.MinCartAmount.Fields.Amount"] = "Minimum cart amount",
            ["Plugin.DiscountRules.Ghost.MinCartAmount.Fields.Amount.Hint"] = "Discount will be applied if customer has minimum x.xx amount in the cart.",
            ["Plugin.DiscountRules.Ghost.MinCartAmount.NotEnough"] = "Sorry, this offer requires more money in the cart",
            ["Plugin.DiscountRules.Ghost.MinCartAmount.Fields.MinCartAmount.Required"] = "Minimum cart amount should be greater than 0",
            ["Plugin.DiscountRules.Ghost.MinCartAmount.Fields.DiscountId.Required"] = "Discount is required"
        });

        await base.InstallAsync();
    }

    /// <summary>
    /// Uninstall the plugin
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public override async Task UninstallAsync()
    {
        //discount requirements
        var discountRequirements = (await _discountService.GetAllDiscountRequirementsAsync())
            .Where(discountRequirement => discountRequirement.DiscountRequirementRuleSystemName == DiscountRequirementDefaults.SYSTEM_NAME);
        foreach (var discountRequirement in discountRequirements)
        {
            await _discountService.DeleteDiscountRequirementAsync(discountRequirement, false);
        }

        //locales
        await _localizationService.DeleteLocaleResourcesAsync("Plugin.DiscountRules.Ghost.MinCartAmount");

        await base.UninstallAsync();
    }
}