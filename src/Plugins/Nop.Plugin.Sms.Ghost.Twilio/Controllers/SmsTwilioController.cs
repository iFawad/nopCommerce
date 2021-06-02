using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Sms.Ghost.Twilio.Models;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Sms.Ghost.Twilio.Controllers
{
    [Area(AreaNames.Admin)]
    [AuthorizeAdmin]
    [AutoValidateAntiforgeryToken]
    public class SmsTwilioController : BasePluginController
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        private readonly ICategoryService _categoryService;

        #endregion

        #region Ctor

        public SmsTwilioController(ILocalizationService localizationService,
            INotificationService notificationService,
            IPermissionService permissionService,
            ISettingService settingService,
            IStoreContext storeContext,
            ICategoryService categoryService)
        {
            _localizationService = localizationService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _settingService = settingService;
            _storeContext = storeContext;
            _categoryService = categoryService;
        }

        #endregion

        #region Methods

        public async Task<IActionResult> Configure()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageWidgets))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var smsTwilioSettings = await _settingService.LoadSettingAsync<SmsTwilioSettings>(storeScope);
            var categories = await _categoryService.GetAllCategoriesAsync();

            var model = new ConfigurationModel
            {
                ActiveStoreScopeConfiguration = storeScope
            };

            if (storeScope > 0)
            {
                model.AccountSid_OverrideForStore = await _settingService.SettingExistsAsync(smsTwilioSettings, x => x.AccountSid, storeScope);
                model.AuthToken_OverrideForStore = await _settingService.SettingExistsAsync(smsTwilioSettings, x => x.AuthToken, storeScope);
                model.TwilioPhoneNumber_OverrideForStore = await _settingService.SettingExistsAsync(smsTwilioSettings, x => x.TwilioPhoneNumber, storeScope);
            }

            return View("~/Plugins/Sms.Ghost.Twilio/Views/Configure.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> Configure(ConfigurationModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageWidgets))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return await Configure();

            //load settings for a chosen store scope
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var smsTwilioSettings = await _settingService.LoadSettingAsync<SmsTwilioSettings>(storeScope);

            //save settings
            smsTwilioSettings.AccountSid = model.AccountSid;
            smsTwilioSettings.AuthToken = model.AuthToken;
            smsTwilioSettings.TwilioPhoneNumber = model.TwilioPhoneNumber;

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            await _settingService.SaveSettingOverridablePerStoreAsync(smsTwilioSettings, x => x.AccountSid, model.AccountSid_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(smsTwilioSettings, x => x.AuthToken, model.AuthToken_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(smsTwilioSettings, x => x.TwilioPhoneNumber, model.TwilioPhoneNumber_OverrideForStore, storeScope, false);

            //now clear settings cache
            await _settingService.ClearCacheAsync();

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

            return await Configure();
        }

        #endregion
    }
}
