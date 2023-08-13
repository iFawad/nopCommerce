using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Services.Configuration;
using Nop.Services.Stores;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Widgets.Ghost.TopMenuItem.Components
{
    [ViewComponent(Name = "TopMenuItem")]
    public class TopMenuItemViewComponent : NopViewComponent
    {
        private readonly TopMenuItemSettings _topMenuItemSettings;
        private readonly IStoreMappingService _storeMappingService;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;

        public TopMenuItemViewComponent(TopMenuItemSettings topMenuItemSettings,
            IStoreMappingService storeMappingService,
            ISettingService settingService,
            IStoreContext storeContext)
        {
            _topMenuItemSettings = topMenuItemSettings;
            _storeMappingService = storeMappingService;
            _settingService = settingService;
            _storeContext = storeContext;
        }

        public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
        {
            
            //Get Settings
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var topMenuItemSettings = await _settingService.LoadSettingAsync<TopMenuItemSettings>(storeScope);

            return View("~/Plugins/Widgets.Ghost.TopMenuItem/Views/HeaderMenu.cshtml", topMenuItemSettings);
        }
    }
}
