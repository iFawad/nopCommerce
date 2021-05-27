using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Domain.Seo;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Web.Framework.Infrastructure;

namespace Nop.Plugin.Widgets.Ghost.ComingSoonProducts
{
    public class ComingSoonProductsPluginProcessor : BasePlugin, IWidgetPlugin
    {
        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;
        private readonly ILocalizationService _localizationService;
        private readonly IStoreContext _storeContext;
        private readonly SeoSettings _seoSettings;

        public ComingSoonProductsPluginProcessor(ISettingService settingService,
            IWebHelper webHelper,
            ILocalizationService localizationService,
            IStoreContext storeContext,
            SeoSettings seoSettings
            )
        {
            _settingService = settingService;
            _webHelper = webHelper;
            _localizationService = localizationService;
            _storeContext = storeContext;
            _seoSettings = seoSettings;
        }

        public bool HideInWidgetList => false;

        public string GetWidgetViewComponentName(string widgetZone)
        {
            return "WidgetsComingSoonProducts";
        }

        public Task<IList<string>> GetWidgetZonesAsync()
        {
            return Task.FromResult<IList<string>>(new List<string>
            { PublicWidgetZones.HomepageBeforeNews }
            );
        }

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/WidgetsComingSoonProducts/Configure";
        }

        /// <summary>
        /// Install plugin
        /// </summary>
        public override async Task InstallAsync()
        {
            // Adding Meta Tags.

            var customHeadTags = _seoSettings.CustomHeadTags;
            var finalCustomHeadTags = customHeadTags + "<meta name=\"referrer\"content=\"no-referrer-when-downgrade\">";
            _seoSettings.CustomHeadTags = finalCustomHeadTags;

            await _settingService.SaveSettingAsync(_seoSettings, x => x.CustomHeadTags);
            await _settingService.ClearCacheAsync();


            await _localizationService.AddLocaleResourceAsync(new Dictionary<string, string>
            {
                ["Plugin.Widgets.Ghost.ComingSoonProducts.CategoryName"] = "Category name:",
                ["Plugin.Widgets.Ghost.ComingSoonProducts.CategoryName.Hint"] = "Category name example 'Coming-Soon' which will display on home screen."
            });


            await base.InstallAsync();
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        public override async Task UninstallAsync()
        {
            // Deleting Meta Tags


            //settings
            await _settingService.DeleteSettingAsync<ComingSoonProductsSettings>();

            //locales

            await _localizationService.DeleteLocaleResourcesAsync("Plugins.Widgets.Ghost.ComingSoonProducts");
            await base.UninstallAsync();
        }



    }
}
