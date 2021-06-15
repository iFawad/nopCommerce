using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Sms.Ghost.Twilio.Services;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Messages;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.Sms.Ghost.Twilio.Controllers
{
    public class SmsTwilioExternalController : BasePluginController
    {
        #region Fields

        private readonly INotificationService _notificationService;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        private readonly IAddressService _addressService;
        private readonly ICustomerService _customerService;
        private readonly TwilioSmsManager _twilioSmsManager;

        #endregion

        #region Ctor

        public SmsTwilioExternalController(INotificationService notificationService,
            ISettingService settingService,
            IStoreContext storeContext,
            IAddressService addressService,
            ICustomerService customerService,
            TwilioSmsManager twilioSmsManager)
        {
            _notificationService = notificationService;
            _settingService = settingService;
            _storeContext = storeContext;
            _addressService = addressService;
            _customerService = customerService;
            _twilioSmsManager = twilioSmsManager;
        }

        #endregion

        #region Methods
        /// <summary>
        /// Customized method to facilitate Webhook for Tawk.to
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task Notify()
        {
            var smsTwilioSettings = await _settingService.LoadSettingAsync<SmsTwilioSettings>();
            //Check if service is enabled.
            if (!smsTwilioSettings.Enabled)
                return;
            //Verify HMAC
            if (!HttpContext.Request.Headers.ContainsKey("x-tawk-signature"))
                return;
            if (HttpContext.Request.Headers["x-tawk-signature"].ToString() != smsTwilioSettings.Hmac)
                return;

            //get Admin
            var customer = await _customerService.GetCustomerByEmailAsync(smsTwilioSettings.AdminEmail);

            //get customer Address for Phone number
            var address = await _addressService.GetAddressByIdAsync((int)customer.ShippingAddressId);

            //try to send SMS
            await _twilioSmsManager.SendSMSAsync(address.PhoneNumber, "Chat started on: Friends Smoke & Vapor.");
        }

        #endregion
    }
}
