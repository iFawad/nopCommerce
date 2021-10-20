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

namespace Nop.Plugin.Widgets.Ghost.WholeSeller
{
    public class WholeSellerPluginProcessor : BasePlugin, IWidgetPlugin
    {
        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;
        private readonly ILocalizationService _localizationService;
        private readonly IStoreContext _storeContext;
        private readonly SeoSettings _seoSettings;

        public WholeSellerPluginProcessor(ISettingService settingService,
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
            return "WholeSeller";
        }

        public Task<IList<string>> GetWidgetZonesAsync()
        {
            return Task.FromResult<IList<string>>(new List<string>
            { PublicWidgetZones.HeaderLinksBefore }
            );
        }

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/WholeSeller/Configure";
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
                ["Plugin.Widgets.Ghost.WholeSeller.TitleWholeSeller"] = "Title Wholeseller:",
                ["Plugin.Widgets.Ghost.WholeSeller.TitleWholeSeller.Hint"] = "Name to be displayed in header menu for wholeseller.",
                ["Plugin.Widgets.Ghost.WholeSeller.RouteUrlWholeSeller"] = "RouteUrl Wholeseller:",
                ["Plugin.Widgets.Ghost.WholeSeller.RouteUrlWholeSeller.Hint"] = "RouteUrl link to the whole seller page.",
                ["Plugin.Widgets.Ghost.WholeSeller.TitleContactUs"] = "Title Contact Us:",
                ["Plugin.Widgets.Ghost.WholeSeller.TitleContactUs.Hint"] = "Name to be displayed in header menu for contact us.",
                ["Plugin.Widgets.Ghost.WholeSeller.RouteUrlContactUs"] = "RouteUrl Contact Us:",
                ["Plugin.Widgets.Ghost.WholeSeller.RouteUrlContactUs.Hint"] = "RouteUrl link to the contact us page.",
                ["Plugin.Widgets.Ghost.WholeSeller.TitleHome"] = "Title Home:",
                ["Plugin.Widgets.Ghost.WholeSeller.TitleHome.Hint"] = "Name to be displayed in header menu for home.",
                ["Plugin.Widgets.Ghost.WholeSeller.RouteUrlHome"] = "RouteUrl Home:",
                ["Plugin.Widgets.Ghost.WholeSeller.RouteUrlHome.Hint"] = "RouteUrl link to the home page."
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
            await _settingService.DeleteSettingAsync<WholeSellerSettings>();

            //locales

            await _localizationService.DeleteLocaleResourcesAsync("Plugin.Widgets.Ghost.WholeSeller");
            await base.UninstallAsync();
        }
    }
}
