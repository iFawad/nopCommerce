using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Sms.Ghost.Twilio.Models
{
    /// <summary>
    /// Represents an sms account model
    /// </summary>
    public record SmsAccountModel : BaseNopEntityModel
    {
        #region Properties

        [NopResourceDisplayName("Plugin.Sms.Ghost.Twilio.AccountSid")]
        public string AccountSid { get; set; }

        [NopResourceDisplayName("Plugin.Sms.Ghost.Twilio.DisplayName")]
        public string DisplayName { get; set; }

        [NopResourceDisplayName("Plugin.Sms.Ghost.Twilio.AuthToken")]
        public string AuthToken { get; set; }

        [NopResourceDisplayName("Plugin.Sms.Ghost.Twilio.TwilioPhoneNumber")]
        public string TwilioPhoneNumber { get; set; }

        [NopResourceDisplayName("Plugin.Sms.Ghost.Twilio.UseDefaultCredentials")]
        public bool UseDefaultCredentials { get; set; }

        [NopResourceDisplayName("Plugin.Sms.Ghost.Twilio.IsDefaultSmsAccount")]
        public bool IsDefaultSmsAccount { get; set; }

        [NopResourceDisplayName("Plugin.Sms.Ghost.Twilio.SendTestSmsTo")]
        public string SendTestSmsTo { get; set; }

        #endregion
    }
}
