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

namespace Nop.Plugin.Widgets.Ghost.WholeSeller.Components
{
    [ViewComponent(Name = "WholeSeller")]
    public class WholeSellerViewComponent : NopViewComponent
    {
        private readonly WholeSellerSettings _wholeSellerSettings;
        private readonly IStoreMappingService _storeMappingService;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;

        public WholeSellerViewComponent(WholeSellerSettings wholeSellerSettings,
            IStoreMappingService storeMappingService,
            ISettingService settingService,
            IStoreContext storeContext)
        {
            _wholeSellerSettings = wholeSellerSettings;
            _storeMappingService = storeMappingService;
            _settingService = settingService;
            _storeContext = storeContext;
        }

        public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
        {
            
            //Get Settings
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var wholeSellerSettings = await _settingService.LoadSettingAsync<WholeSellerSettings>(storeScope);

            return View("~/Plugins/Widgets.Ghost.WholeSeller/Views/HeaderMenu.cshtml", wholeSellerSettings);
        }
    }
}
