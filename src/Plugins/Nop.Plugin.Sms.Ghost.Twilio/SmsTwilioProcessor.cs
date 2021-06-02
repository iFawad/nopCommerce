using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Domain.Seo;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;

namespace Nop.Plugin.Sms.Ghost.Twilio
{
    public class SmsTwilioProcessor : BasePlugin, IPlugin
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
                ["Plugin.Sms.Ghost.Twilio.TwilioPhoneNumber.Hint"] = "Phone number provided by Twilio to send SMS messages from."
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

            await _localizationService.DeleteLocaleResourcesAsync("Plugins.Sms.Ghost.Twilio");
            await base.UninstallAsync();
        }
    }
}
