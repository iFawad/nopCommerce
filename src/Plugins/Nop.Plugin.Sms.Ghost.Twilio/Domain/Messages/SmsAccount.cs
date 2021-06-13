using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;

namespace Nop.Plugin.Sms.Ghost.Twilio.Domain.Messages
{
    /// <summary>
    /// Represents an sms account
    /// </summary>
    public class SmsAccount : BaseEntity
    {
        public string AccountSid { get; set; }
        public string DisplayName { get; set; }
        public string AuthToken { get; set; }
        public string TwilioPhoneNumber { get; set; }
        /// <summary>
        /// Gets or sets a value that controls whether the default system credentials of the application are sent with requests.
        /// </summary>
        public bool UseDefaultCredentials { get; set; }
        public bool IsDefaultSmsAccount { get; set; }
        public string SendTestSmsTo { get; set; }
    }
}
