using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Sms.Ghost.Twilio.Models
{
    public record ConfigurationModel : BaseNopModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Plugin.Sms.Ghost.Twilio.AccountSid")]
        public string AccountSid { get; set; }
        public bool AccountSid_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugin.Sms.Ghost.Twilio.AuthToken")]
        public string AuthToken { get; set; }
        public bool AuthToken_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugin.Sms.Ghost.Twilio.TwilioPhoneNumber")]
        public string TwilioPhoneNumber { get; set; }
        public bool TwilioPhoneNumber_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugin.Sms.Ghost.Twilio.Enabled")]
        public bool Enabled { get; set; }
        public bool Enabled_OverrideForStore { get; set; }
    }
}
