using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Services.Configuration;
using Nop.Services.Logging;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace Nop.Plugin.Sms.Ghost.Twilio.Services
{
    public class TwilioSmsManager
    {
        #region Fields

        private readonly ILogger _logger;
        private readonly ISettingService _settingService;
        private readonly IWorkContext _workContext;

        #endregion

        #region ctor

        public TwilioSmsManager(ILogger logger,
            ISettingService settingService,
            IWorkContext workContext)
        {
            _logger = logger;
            _settingService = settingService;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Send SMS 
        /// </summary>
        /// <param name="to">Phone number of the receiver</param>
        /// <param name="from">Name of sender</param>
        /// <param name="text">Text</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task SendSMSAsync(string to, string text)
        {
            //whether SMS notifications enabled
            var smsTwilioSettings = await _settingService.LoadSettingAsync<SmsTwilioSettings>();
            //if (!smsTwilioSettings.UseSmsNotifications)
            //    return;

            try
            {
                //check number and text
                if (string.IsNullOrEmpty(to) || string.IsNullOrEmpty(text))
                    throw new NopException("Phone number or SMS text is empty");

                TwilioClient.Init(smsTwilioSettings.AccountSid, smsTwilioSettings.AuthToken);

                //send SMS
                var toNumber = new PhoneNumber(to);
                var message = await MessageResource.CreateAsync(
                    to,
                    from: new PhoneNumber(smsTwilioSettings.TwilioPhoneNumber), //  From number, must be an SMS-enabled Twilio number ( This will send sms from ur "To" numbers ).  
                    body: text);
                
                await _logger.InformationAsync($"Twilio SMS sent: {message.ToString()}");
            }
            catch (Exception exception)
            {
                //log full error
                await _logger.ErrorAsync($"Twilio SMS sending error: {exception.Message}.", exception, await _workContext.GetCurrentCustomerAsync());
            }
        }

        #endregion
    }
}
