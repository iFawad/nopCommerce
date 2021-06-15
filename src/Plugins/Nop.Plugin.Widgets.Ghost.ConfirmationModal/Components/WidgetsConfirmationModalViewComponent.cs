using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Services.Configuration;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Widgets.Ghost.ConfirmationModal.Components
{
    [ViewComponent(Name = "WidgetsConfirmationModal")]
    public class WidgetsConfirmationModalViewComponent : NopViewComponent
    {
        private readonly ConfirmationModalSettings _confirmationModalSettings;
        private readonly IAclService _aclService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;

        public WidgetsConfirmationModalViewComponent(ConfirmationModalSettings confirmationModalSettings,
            IAclService aclService,
            IStoreMappingService storeMappingService,
            ISettingService settingService,
            IStoreContext storeContext)
        {
            _confirmationModalSettings = confirmationModalSettings;
            _aclService = aclService;
            _storeMappingService = storeMappingService;
            _settingService = settingService;
            _storeContext = storeContext;
        }

        public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
        {
            //Get Settings
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var confirmationModalSettings = await _settingService.LoadSettingAsync<ConfirmationModalSettings>(storeScope);

            return View("~/Plugins/Widgets.Ghost.ConfirmationModal/Views/ConfirmationModal.cshtml", confirmationModalSettings);
        }
    }
}
