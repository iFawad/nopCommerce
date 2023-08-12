using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain;
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
        private readonly string _yes = "Yes";
        private readonly string _no = "No";
        private readonly string _urlOnNo = "http://google.com/";

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
            var storeInformationSettings = await _settingService.LoadSettingAsync<StoreInformationSettings>(storeScope);
            confirmationModalSettings.StoreClosed = storeInformationSettings.StoreClosed;

            LoadDefaultSettings(confirmationModalSettings);

            return View("~/Plugins/Widgets.Ghost.ConfirmationModal/Views/ConfirmationModal.cshtml", confirmationModalSettings);
        }

        private void LoadDefaultSettings(ConfirmationModalSettings confirmationModalSettings)
        {
            confirmationModalSettings.YesText = string.IsNullOrEmpty(confirmationModalSettings.YesText) ? _yes : confirmationModalSettings.YesText;
            confirmationModalSettings.NoText = string.IsNullOrEmpty(confirmationModalSettings.NoText) ? _no : confirmationModalSettings.NoText;
            confirmationModalSettings.UrlOnNo = string.IsNullOrEmpty(confirmationModalSettings.UrlOnNo) ? _urlOnNo : confirmationModalSettings.UrlOnNo;
        }
    }
}
