using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Plugin.Widgets.Ghost.ComingSoonProducts.Models;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Widgets.Ghost.ComingSoonProducts.Controllers
{
    [Area(AreaNames.Admin)]
    [AuthorizeAdmin]
    [AutoValidateAntiforgeryToken]
    public class WidgetsComingSoonProductsController : BasePluginController
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

        public WidgetsComingSoonProductsController(ILocalizationService localizationService,
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
            var comingSoonProductsSettings = await _settingService.LoadSettingAsync<ComingSoonProductsSettings>(storeScope);
            var categories = await _categoryService.GetAllCategoriesAsync();

            var model = new ConfigurationModel
            {
                SectionName = comingSoonProductsSettings.SectionName,
                ActiveStoreScopeConfiguration = storeScope,
                AvailableCategories = new List<SelectListItem>()
            };

            //Set Default selection item
            SelectListItem defaultItem = new SelectListItem
            {
                Value = "0",
                Text = "Select category"
            };
            model.AvailableCategories.Add(defaultItem);

            foreach (var category in categories)
            {
                SelectListItem item = new SelectListItem
                {
                    Value = category.Id.ToString(),
                    Text = category.Name
                };
                model.AvailableCategories.Add(item);
            }

            if (storeScope > 0)
            {
                model.SectionName_OverrideForStore = await _settingService.SettingExistsAsync(comingSoonProductsSettings, x => x.SectionName, storeScope);
                model.CategoryId_OverrideForStore = await _settingService.SettingExistsAsync(comingSoonProductsSettings, x => x.CategoryId, storeScope);
            }

            return View("~/Plugins/Widgets.Ghost.ComingSoonProducts/Views/Configure.cshtml", model);
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
            var comingSoonProductsSettings = await _settingService.LoadSettingAsync<ComingSoonProductsSettings>(storeScope);

            //save settings
            comingSoonProductsSettings.SectionName = model.SectionName;
            comingSoonProductsSettings.CategoryId = model.CategoryId;

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            await _settingService.SaveSettingOverridablePerStoreAsync(comingSoonProductsSettings, x => x.SectionName, model.SectionName_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(comingSoonProductsSettings, x => x.CategoryId, model.CategoryId_OverrideForStore, storeScope, false);

            //now clear settings cache
            await _settingService.ClearCacheAsync();

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

            return await Configure();
        }

        #endregion
    }
}
