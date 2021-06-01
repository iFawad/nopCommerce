using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elsheimy.Components.ePayment.Migs.Commands;
using Elsheimy.Components.ePayment.Migs.Web;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Plugin.Payments.Ghost.Migs.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Security;
using Nop.Web.Factories;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Models.Checkout;

namespace Nop.Plugin.Payments.Ghost.Migs.Controllers
{
    
    [AutoValidateAntiforgeryToken]
    public class PaymentMigsController : BasePaymentController
    {
        #region Fields

        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;
        private readonly IPaymentPluginManager _paymentPluginManager;
        private readonly IOrderService _orderService;
        private readonly ICheckoutModelFactory _checkoutModelFactory;

        #endregion

        #region Ctor

        public PaymentMigsController(ILanguageService languageService,
            ILocalizationService localizationService,
            INotificationService notificationService,
            IPermissionService permissionService,
            ISettingService settingService,
            IStoreContext storeContext,
            IWorkContext workContext,
            IPaymentPluginManager paymentPluginManager,
            IOrderService orderService,
            ICheckoutModelFactory checkoutModelFactory)
        {
            _languageService = languageService;
            _localizationService = localizationService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _settingService = settingService;
            _storeContext = storeContext;
            _workContext = workContext;
            _paymentPluginManager = paymentPluginManager;
            _orderService = orderService;
            _checkoutModelFactory = checkoutModelFactory;
        }

        #endregion

        #region Methods
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public async Task<IActionResult> Configure()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var migsPaymentSettings = await _settingService.LoadSettingAsync<MigsPaymentSettings>(storeScope);

            var model = new ConfigurationModel
            {
                DescriptionText = migsPaymentSettings.DescriptionText
            };

            //locales
            await AddLocalesAsync(_languageService, model.Locales, async (locale, languageId) =>
            {
                locale.DescriptionText = await _localizationService
                    .GetLocalizedSettingAsync(migsPaymentSettings, x => x.DescriptionText, languageId, 0, false, false);
            });
            model.MerchantId = migsPaymentSettings.MerchantId;
            model.AccessCode = migsPaymentSettings.AccessCode;
            model.HashSecret = migsPaymentSettings.HashSecret;
            model.AdditionalFee = migsPaymentSettings.AdditionalFee;
            model.AdditionalFeePercentage = migsPaymentSettings.AdditionalFeePercentage;
            model.ShippableProductRequired = migsPaymentSettings.ShippableProductRequired;
            model.SkipPaymentInfo = migsPaymentSettings.SkipPaymentInfo;
            model.ActiveStoreScopeConfiguration = storeScope;
            if (storeScope > 0)
            {
                model.MerchantId_OverrideForStore = await _settingService.SettingExistsAsync(migsPaymentSettings, x => x.MerchantId, storeScope);
                model.AccessCode_OverrideForStore = await _settingService.SettingExistsAsync(migsPaymentSettings, x => x.AccessCode, storeScope);
                model.HashSecret_OverrideForStore = await _settingService.SettingExistsAsync(migsPaymentSettings, x => x.HashSecret, storeScope);
                model.DescriptionText_OverrideForStore = await _settingService.SettingExistsAsync(migsPaymentSettings, x => x.DescriptionText, storeScope);
                model.AdditionalFee_OverrideForStore = await _settingService.SettingExistsAsync(migsPaymentSettings, x => x.AdditionalFee, storeScope);
                model.AdditionalFeePercentage_OverrideForStore = await _settingService.SettingExistsAsync(migsPaymentSettings, x => x.AdditionalFeePercentage, storeScope);
                model.ShippableProductRequired_OverrideForStore = await _settingService.SettingExistsAsync(migsPaymentSettings, x => x.ShippableProductRequired, storeScope);
                model.SkipPaymentInfo_OverrideForStore = await _settingService.SettingExistsAsync(migsPaymentSettings, x => x.SkipPaymentInfo, storeScope);
            }

            return View("~/Plugins/Payments.Ghost.Migs/Views/Configure.cshtml", model);
        }

        [HttpPost]
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public async Task<IActionResult> Configure(ConfigurationModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return await Configure();

            //load settings for a chosen store scope
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var migsPaymentSettings = await _settingService.LoadSettingAsync<MigsPaymentSettings>(storeScope);

            //save settings
            migsPaymentSettings.MerchantId = model.MerchantId;
            migsPaymentSettings.AccessCode = model.AccessCode;
            migsPaymentSettings.HashSecret = model.HashSecret;
            migsPaymentSettings.DescriptionText = model.DescriptionText;
            migsPaymentSettings.AdditionalFee = model.AdditionalFee;
            migsPaymentSettings.AdditionalFeePercentage = model.AdditionalFeePercentage;
            migsPaymentSettings.ShippableProductRequired = model.ShippableProductRequired;
            migsPaymentSettings.SkipPaymentInfo = model.SkipPaymentInfo;

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            await _settingService.SaveSettingOverridablePerStoreAsync(migsPaymentSettings, x => x.MerchantId, model.MerchantId_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(migsPaymentSettings, x => x.AccessCode, model.AccessCode_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(migsPaymentSettings, x => x.HashSecret, model.HashSecret_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(migsPaymentSettings, x => x.DescriptionText, model.DescriptionText_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(migsPaymentSettings, x => x.AdditionalFee, model.AdditionalFee_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(migsPaymentSettings, x => x.AdditionalFeePercentage, model.AdditionalFeePercentage_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(migsPaymentSettings, x => x.ShippableProductRequired, model.ShippableProductRequired_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(migsPaymentSettings, x => x.SkipPaymentInfo, model.SkipPaymentInfo_OverrideForStore, storeScope, false);

            //now clear settings cache
            await _settingService.ClearCacheAsync();

            //localization. no multi-store support for localization yet.
            foreach (var localized in model.Locales)
            {
                await _localizationService.SaveLocalizedSettingAsync(migsPaymentSettings,
                    x => x.DescriptionText, localized.LanguageId, localized.DescriptionText);
            }

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

            return await Configure();
        }

        public async Task<IActionResult> ServerHostedPaymentCallback()
        {
            var queryParameters = HttpContext.Request.Query.Select(a => new QueryParameter(a.Key, a.Value));
            List<QueryParameter> paramList = new List<QueryParameter>();

            foreach (var par in queryParameters)
            {
                paramList.Add(new QueryParameter(par.Name, par.Value));
            }

            VpcPaymentResult result = new VpcPaymentResult();
            result.LoadParameters(paramList);


            TempData["QueryParams"] = JsonConvert.SerializeObject(result, Formatting.Indented);

            var customer = await _workContext.GetCurrentCustomerAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            var paymentMethod = await _paymentPluginManager.LoadPluginBySystemNameAsync("Payments.Ghost.Migs", customer, store.Id);
            if (!_paymentPluginManager.IsPluginActive(paymentMethod) || paymentMethod is not MigsPaymentProcessor plugin)
                throw new NopException($"{"Payments.Ghost.Migs"} error. Module cannot be loaded");

            //var orderId = await plugin.HandleTransactionAsync(result);
            //if (!orderId.HasValue)
            //    throw new NopException($"{"Payments.Ghost.Migs"} error. Order not found");

            if(result.TxnResponseCode != "0")
            {
                //return View();
                //something failed, redisplay form
                //var paymenInfoModel = await _checkoutModelFactory.PreparePaymentInfoModelAsync(paymentMethod);
                //return Json(new
                //{
                //    update_section = new UpdateSectionJsonModel
                //    {
                //        name = "payment-info",
                //        html = await RenderPartialViewToStringAsync("OpcPaymentInfo", paymenInfoModel)
                //    }
                //});
                //return Content(await _localizationService.GetResourceAsync("Checkout.RedirectMessage"));
                
                return RedirectToRoute("CheckoutOnePage");
                //return RedirectToRoute("CheckoutPaymentMethod");
            }

            //get the order
            var order = (await _orderService.SearchOrdersAsync(storeId: (await _storeContext.GetCurrentStoreAsync()).Id,
            customerId: (await _workContext.GetCurrentCustomerAsync()).Id, pageSize: 1)).FirstOrDefault();

            return RedirectToRoute("CheckoutCompleted", new { orderId = order.Id });
            //return RedirectToAction(nameof(Results));
        }

        public IActionResult Results()
        {
            return View();
        }

        #endregion
    }
}
