using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Widgets.Ghost.TopMenuItem.Models;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Widgets.Ghost.TopMenuItem.Controllers
{
    [Area(AreaNames.Admin)]
    [AuthorizeAdmin]
    [AutoValidateAntiforgeryToken]
    public class TopMenuItemController : BasePluginController
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;

        #endregion

        #region Ctor

        public TopMenuItemController(ILocalizationService localizationService,
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
            var topMenuItemSettings = await _settingService.LoadSettingAsync<TopMenuItemSettings>(storeScope);

            var model = new ConfigurationModel
            {
                TitleTopMenuItem = topMenuItemSettings.TitleTopMenuItem,
                RouteUrlTopMenuItem = topMenuItemSettings.RouteUrlTopMenuItem,
                TitleContactUs = topMenuItemSettings.TitleContactUs,
                RouteUrlContactUs = topMenuItemSettings.RouteUrlContactUs,
                TitleHome = topMenuItemSettings.TitleHome,
                RouteUrlHome = topMenuItemSettings.RouteUrlHome,
                ActiveStoreScopeConfiguration = storeScope
            };


            if (storeScope > 0)
            {
                model.TitleTopMenuItem_OverrideForStore = await _settingService.SettingExistsAsync(topMenuItemSettings, x => x.TitleTopMenuItem, storeScope);
                model.RouteUrlTopMenuItem_OverrideForStore = await _settingService.SettingExistsAsync(topMenuItemSettings, x => x.RouteUrlTopMenuItem, storeScope);
                model.TitleContactUs_OverrideForStore = await _settingService.SettingExistsAsync(topMenuItemSettings, x => x.TitleContactUs, storeScope);
                model.RouteUrlContactUs_OverrideForStore = await _settingService.SettingExistsAsync(topMenuItemSettings, x => x.RouteUrlContactUs, storeScope);
                model.TitleHome_OverrideForStore = await _settingService.SettingExistsAsync(topMenuItemSettings, x => x.TitleHome, storeScope);
                model.RouteUrlHome_OverrideForStore = await _settingService.SettingExistsAsync(topMenuItemSettings, x => x.RouteUrlHome, storeScope);
            }

            return View("~/Plugins/Widgets.Ghost.TopMenuItem/Views/Configure.cshtml", model);
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
            var topMenuItemSettings = await _settingService.LoadSettingAsync<TopMenuItemSettings>(storeScope);

            //save settings
            topMenuItemSettings.TitleTopMenuItem = model.TitleTopMenuItem;
            topMenuItemSettings.RouteUrlTopMenuItem = model.RouteUrlTopMenuItem;
            topMenuItemSettings.TitleContactUs = model.TitleContactUs;
            topMenuItemSettings.RouteUrlContactUs = model.RouteUrlContactUs;
            topMenuItemSettings.TitleHome = model.TitleHome;
            topMenuItemSettings.RouteUrlHome = model.RouteUrlHome;

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            await _settingService.SaveSettingOverridablePerStoreAsync(topMenuItemSettings, x => x.TitleTopMenuItem, model.TitleTopMenuItem_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(topMenuItemSettings, x => x.RouteUrlTopMenuItem, model.RouteUrlTopMenuItem_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(topMenuItemSettings, x => x.TitleContactUs, model.TitleContactUs_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(topMenuItemSettings, x => x.RouteUrlContactUs, model.RouteUrlContactUs_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(topMenuItemSettings, x => x.TitleHome, model.TitleHome_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(topMenuItemSettings, x => x.RouteUrlHome, model.RouteUrlHome_OverrideForStore, storeScope, false);

            //now clear settings cache
            await _settingService.ClearCacheAsync();

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

            return await Configure();
        }

        #endregion
    }
}
