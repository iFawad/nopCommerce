using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Domain.Seo;
using Nop.Plugin.Widgets.Ghost.TopMenuItem.Components;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Web.Framework.Infrastructure;

namespace Nop.Plugin.Widgets.Ghost.TopMenuItem
{
    public class TopMenuItemPluginProcessor : BasePlugin, IWidgetPlugin
    {
        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;
        private readonly ILocalizationService _localizationService;
        private readonly IStoreContext _storeContext;
        private readonly SeoSettings _seoSettings;

        public TopMenuItemPluginProcessor(ISettingService settingService,
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

        public Type GetWidgetViewComponent(string widgetZone)
        {
            return typeof(TopMenuItemViewComponent);
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
            return $"{_webHelper.GetStoreLocation()}Admin/TopMenuItem/Configure";
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


            await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
            {
                ["Plugin.Widgets.Ghost.TopMenuItem.TitleTopMenuItem"] = "Title TopMenuItem:",
                ["Plugin.Widgets.Ghost.TopMenuItem.TitleTopMenuItem.Hint"] = "Name to be displayed in header menu for TopMenuItem.",
                ["Plugin.Widgets.Ghost.TopMenuItem.RouteUrlTopMenuItem"] = "RouteUrl TopMenuItem:",
                ["Plugin.Widgets.Ghost.TopMenuItem.RouteUrlTopMenuItem.Hint"] = "RouteUrl link to the whole seller page.",
                ["Plugin.Widgets.Ghost.TopMenuItem.TitleContactUs"] = "Title Contact Us:",
                ["Plugin.Widgets.Ghost.TopMenuItem.TitleContactUs.Hint"] = "Name to be displayed in header menu for contact us.",
                ["Plugin.Widgets.Ghost.TopMenuItem.RouteUrlContactUs"] = "RouteUrl Contact Us:",
                ["Plugin.Widgets.Ghost.TopMenuItem.RouteUrlContactUs.Hint"] = "RouteUrl link to the contact us page.",
                ["Plugin.Widgets.Ghost.TopMenuItem.TitleHome"] = "Title Home:",
                ["Plugin.Widgets.Ghost.TopMenuItem.TitleHome.Hint"] = "Name to be displayed in header menu for home.",
                ["Plugin.Widgets.Ghost.TopMenuItem.RouteUrlHome"] = "RouteUrl Home:",
                ["Plugin.Widgets.Ghost.TopMenuItem.RouteUrlHome.Hint"] = "RouteUrl link to the home page."
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
            await _settingService.DeleteSettingAsync<TopMenuItemSettings>();

            //locales

            await _localizationService.DeleteLocaleResourcesAsync("Plugin.Widgets.Ghost.TopMenuItem");
            await base.UninstallAsync();
        }
    }
}
