using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Domain.Seo;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;

namespace Nop.Plugin.Sms.Ghost.Twilio
{
    public class SmsTwilioProcessor : BasePlugin, IPlugin, IMiscPlugin//, IAdminMenuPlugin
    {
        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;
        private readonly ILocalizationService _localizationService;
        private readonly IStoreContext _storeContext;
        private readonly SeoSettings _seoSettings;

        public SmsTwilioProcessor(ISettingService settingService,
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

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/SmsTwilio/Configure";
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
                ["Plugin.Sms.Ghost.Twilio.AccountSid"] = "Account Sid:",
                ["Plugin.Sms.Ghost.Twilio.AccountSid.Hint"] = "Account SID provided by Twilio.",
                ["Plugin.Sms.Ghost.Twilio.AuthToken"] = "Auth Token:",
                ["Plugin.Sms.Ghost.Twilio.AuthToken.Hint"] = "Auth Token provided by Twilio",
                ["Plugin.Sms.Ghost.Twilio.TwilioPhoneNumber"] = "Twilio Phone Number:",
                ["Plugin.Sms.Ghost.Twilio.TwilioPhoneNumber.Hint"] = "Phone number provided by Twilio to send SMS messages from.",
                ["Plugin.Sms.Ghost.Twilio.AdminEmail"] = "Admin Email:",
                ["Plugin.Sms.Ghost.Twilio.AdminEmail.Hint"] = "Email address of admin for sms notification to registered phone number.",
                ["Plugin.Sms.Ghost.Twilio.Hmac"] = "HMAC:",
                ["Plugin.Sms.Ghost.Twilio.Hmac.Hint"] = "Secret key provided by Tawk.to for Start chat event Web hook.",
                ["Plugin.Sms.Ghost.Twilio.Enabled"] = "Enabled:",
                ["Plugin.Sms.Ghost.Twilio.Enabled.Hint"] = "Identifies if SMS service is enabled."
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
            await _settingService.DeleteSettingAsync<SmsTwilioSettings>();

            //locales
            await _localizationService.DeleteLocaleResourcesAsync("Plugin.Sms.Ghost.Twilio");
            await base.UninstallAsync();
        }

        //public Task ManageSiteMapAsync(SiteMapNode rootNode)
        //{
        //    var menuItem = new SiteMapNode()
        //    {
        //        SystemName = "Sms aacounts",
        //        Title = "Sms accounts",
        //        ControllerName = @"SmsTwilio",
        //        ActionName = "ConfigureSmsAccounts",
        //        Visible = true,
        //        IconClass = "far fa-dot-circle",
        //        RouteValues = new RouteValueDictionary() { { "area", "Admin" } },
        //    };
        //    var pluginNode = rootNode.ChildNodes.FirstOrDefault(x => x.SystemName == "Configuration");
        //    if (pluginNode != null)
        //        pluginNode.ChildNodes.Add(menuItem);
        //    else
        //        rootNode.ChildNodes.Add(menuItem);

        //    return Task.CompletedTask;
        //}


    }
}
