using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Payments.Ghost.AuthorizeNet.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Payments.Ghost.AuthorizeNet.Controllers
{
    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    [AutoValidateAntiforgeryToken]
    public class AuthorizeNetController : BasePaymentController
    {
        #region Fields

        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;

        #endregion

        #region Ctor

        public AuthorizeNetController(ILanguageService languageService,
            ILocalizationService localizationService,
            INotificationService notificationService,
            IPermissionService permissionService,
            ISettingService settingService,
            IStoreContext storeContext)
        {
            _languageService = languageService;
            _localizationService = localizationService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _settingService = settingService;
            _storeContext = storeContext;
        }

        #endregion

        #region Methods

        public async Task<IActionResult> Configure()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var authorizeNetPaymentSettings = await _settingService.LoadSettingAsync<AuthorizeNetPaymentSettings>(storeScope);

            var model = new ConfigurationModel
            {
                DescriptionText = authorizeNetPaymentSettings.DescriptionText
            };

            //locales
            await AddLocalesAsync(_languageService, model.Locales, async (locale, languageId) =>
            {
                locale.DescriptionText = await _localizationService
                    .GetLocalizedSettingAsync(authorizeNetPaymentSettings, x => x.DescriptionText, languageId, 0, false, false);
            });
            model.ApiLoginId = authorizeNetPaymentSettings.ApiLoginId;
            model.TransactionKey = authorizeNetPaymentSettings.TransactionKey;
            model.Environment = authorizeNetPaymentSettings.Environment;
            model.AdditionalFee = authorizeNetPaymentSettings.AdditionalFee;
            model.AdditionalFeePercentage = authorizeNetPaymentSettings.AdditionalFeePercentage;
            model.ShippableProductRequired = authorizeNetPaymentSettings.ShippableProductRequired;
            model.SkipPaymentInfo = authorizeNetPaymentSettings.SkipPaymentInfo;
            model.ActiveStoreScopeConfiguration = storeScope;
            if (storeScope > 0)
            {
                model.ApiLoginId_OverrideForStore = await _settingService.SettingExistsAsync(authorizeNetPaymentSettings, x => x.ApiLoginId, storeScope);
                model.TransactionKey_OverrideForStore = await _settingService.SettingExistsAsync(authorizeNetPaymentSettings, x => x.TransactionKey, storeScope);
                model.Environment_OverrideForStore = await _settingService.SettingExistsAsync(authorizeNetPaymentSettings, x => x.Environment, storeScope);
                model.DescriptionText_OverrideForStore = await _settingService.SettingExistsAsync(authorizeNetPaymentSettings, x => x.DescriptionText, storeScope);
                model.AdditionalFee_OverrideForStore = await _settingService.SettingExistsAsync(authorizeNetPaymentSettings, x => x.AdditionalFee, storeScope);
                model.AdditionalFeePercentage_OverrideForStore = await _settingService.SettingExistsAsync(authorizeNetPaymentSettings, x => x.AdditionalFeePercentage, storeScope);
                model.ShippableProductRequired_OverrideForStore = await _settingService.SettingExistsAsync(authorizeNetPaymentSettings, x => x.ShippableProductRequired, storeScope);
                model.SkipPaymentInfo_OverrideForStore = await _settingService.SettingExistsAsync(authorizeNetPaymentSettings, x => x.SkipPaymentInfo, storeScope);
            }

            return View("~/Plugins/Payments.Ghost.AuthorizeNet/Views/Configure.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> Configure(ConfigurationModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return await Configure();

            //load settings for a chosen store scope
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var authorizeNetPaymentSettings = await _settingService.LoadSettingAsync<AuthorizeNetPaymentSettings>(storeScope);

            //save settings
            authorizeNetPaymentSettings.ApiLoginId = model.ApiLoginId;
            authorizeNetPaymentSettings.TransactionKey = model.TransactionKey;
            authorizeNetPaymentSettings.Environment = model.Environment;
            authorizeNetPaymentSettings.DescriptionText = model.DescriptionText;
            authorizeNetPaymentSettings.AdditionalFee = model.AdditionalFee;
            authorizeNetPaymentSettings.AdditionalFeePercentage = model.AdditionalFeePercentage;
            authorizeNetPaymentSettings.ShippableProductRequired = model.ShippableProductRequired;
            authorizeNetPaymentSettings.SkipPaymentInfo = model.SkipPaymentInfo;

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            await _settingService.SaveSettingOverridablePerStoreAsync(authorizeNetPaymentSettings, x => x.ApiLoginId, model.ApiLoginId_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(authorizeNetPaymentSettings, x => x.TransactionKey, model.TransactionKey_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(authorizeNetPaymentSettings, x => x.Environment, model.Environment_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(authorizeNetPaymentSettings, x => x.DescriptionText, model.DescriptionText_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(authorizeNetPaymentSettings, x => x.AdditionalFee, model.AdditionalFee_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(authorizeNetPaymentSettings, x => x.AdditionalFeePercentage, model.AdditionalFeePercentage_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(authorizeNetPaymentSettings, x => x.ShippableProductRequired, model.ShippableProductRequired_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(authorizeNetPaymentSettings, x => x.SkipPaymentInfo, model.SkipPaymentInfo_OverrideForStore, storeScope, false);

            //now clear settings cache
            await _settingService.ClearCacheAsync();

            //localization. no multi-store support for localization yet.
            foreach (var localized in model.Locales)
            {
                await _localizationService.SaveLocalizedSettingAsync(authorizeNetPaymentSettings,
                    x => x.DescriptionText, localized.LanguageId, localized.DescriptionText);
            }

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

            return await Configure();
        }

        #endregion
    }
}
