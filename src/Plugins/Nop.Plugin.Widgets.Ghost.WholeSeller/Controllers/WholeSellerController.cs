using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Widgets.Ghost.WholeSeller.Models;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Widgets.Ghost.WholeSeller.Controllers
{
    [Area(AreaNames.Admin)]
    [AuthorizeAdmin]
    [AutoValidateAntiforgeryToken]
    public class WholeSellerController : BasePluginController
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;

        #endregion

        #region Ctor

        public WholeSellerController(ILocalizationService localizationService,
            INotificationService notificationService,
            IPermissionService permissionService,
            ISettingService settingService,
            IStoreContext storeContext)
        {
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
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageWidgets))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var wholeSellerSettings = await _settingService.LoadSettingAsync<WholeSellerSettings>(storeScope);

            var model = new ConfigurationModel
            {
                Title = wholeSellerSettings.Title,
                Url = wholeSellerSettings.Url,
                ActiveStoreScopeConfiguration = storeScope
            };


            if (storeScope > 0)
            {
                model.Title_OverrideForStore = await _settingService.SettingExistsAsync(wholeSellerSettings, x => x.Title, storeScope);
                model.Url_OverrideForStore = await _settingService.SettingExistsAsync(wholeSellerSettings, x => x.Url, storeScope);
            }

            return View("~/Plugins/Widgets.Ghost.WholeSeller/Views/Configure.cshtml", model);
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
            var wholeSellerSettings = await _settingService.LoadSettingAsync<WholeSellerSettings>(storeScope);

            //save settings
            wholeSellerSettings.Title = model.Title;
            wholeSellerSettings.Url = model.Url;

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            await _settingService.SaveSettingOverridablePerStoreAsync(wholeSellerSettings, x => x.Title, model.Title_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(wholeSellerSettings, x => x.Url, model.Url_OverrideForStore, storeScope, false);

            //now clear settings cache
            await _settingService.ClearCacheAsync();

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

            return await Configure();
        }

        #endregion
    }
}
