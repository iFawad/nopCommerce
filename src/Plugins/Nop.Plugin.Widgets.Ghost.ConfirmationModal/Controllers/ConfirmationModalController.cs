using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Widgets.Ghost.ConfirmationModal.Models;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Widgets.Ghost.ConfirmationModal.Controllers
{
    [Area(AreaNames.Admin)]
    [AuthorizeAdmin]
    [AutoValidateAntiforgeryToken]
    public class ConfirmationModalController : BasePluginController
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

        public ConfirmationModalController(ILocalizationService localizationService,
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
            var confirmationModalSettings = await _settingService.LoadSettingAsync<ConfirmationModalSettings>(storeScope);
            var categories = await _categoryService.GetAllCategoriesAsync();

            var model = new ConfigurationModel
            {
                ActiveStoreScopeConfiguration = storeScope,
                Topic = confirmationModalSettings.Topic,
                Title = confirmationModalSettings.Title,
                YesText = confirmationModalSettings.YesText,
                NoText = confirmationModalSettings.NoText
            };

            
            if (storeScope > 0)
            {
                model.Topic_OverrideForStore = await _settingService.SettingExistsAsync(confirmationModalSettings, x => x.Topic, storeScope);
                model.Title_OverrideForStore = await _settingService.SettingExistsAsync(confirmationModalSettings, x => x.Title, storeScope);
                model.YesText_OverrideForStore = await _settingService.SettingExistsAsync(confirmationModalSettings, x => x.YesText, storeScope);
                model.NoText_OverrideForStore = await _settingService.SettingExistsAsync(confirmationModalSettings, x => x.NoText, storeScope);
            }

            return View("~/Plugins/Widgets.Ghost.ConfirmationModal/Views/Configure.cshtml", model);
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
            var confirmationModalSettings = await _settingService.LoadSettingAsync<ConfirmationModalSettings>(storeScope);

            //save settings
            confirmationModalSettings.Topic = model.Topic;
            confirmationModalSettings.Title = model.Title;
            confirmationModalSettings.YesText = model.YesText;
            confirmationModalSettings.NoText = model.NoText;

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            await _settingService.SaveSettingOverridablePerStoreAsync(confirmationModalSettings, x => x.Topic, model.Topic_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(confirmationModalSettings, x => x.Title, model.Title_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(confirmationModalSettings, x => x.YesText, model.YesText_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(confirmationModalSettings, x => x.NoText, model.NoText_OverrideForStore, storeScope, false);

            //now clear settings cache
            await _settingService.ClearCacheAsync();

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

            return await Configure();
        }

        #endregion
    }
}
