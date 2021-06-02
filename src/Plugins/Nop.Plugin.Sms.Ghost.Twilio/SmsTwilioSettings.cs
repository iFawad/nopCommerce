using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Configuration;

namespace Nop.Plugin.Sms.Ghost.Twilio
{
    public class SmsTwilioSettings : ISettings
    {
        public string AccountSid { get; set; }
        public string AuthToken { get; set; }
        public string TwilioPhoneNumber { get; set; }
    }
}
