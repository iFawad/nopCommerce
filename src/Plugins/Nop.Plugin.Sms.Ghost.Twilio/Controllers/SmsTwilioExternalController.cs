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
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly TwilioSmsManager _twilioSmsManager;

        #endregion

        #region Ctor

        public SmsTwilioExternalController(INotificationService notificationService,
            ISettingService settingService,
            IStoreContext storeContext,
            IAddressService addressService,
            ICustomerService customerService,
            IGenericAttributeService genericAttributeService,
            TwilioSmsManager twilioSmsManager)
        {
            _notificationService = notificationService;
            _settingService = settingService;
            _storeContext = storeContext;
            _addressService = addressService;
            _customerService = customerService;
            _genericAttributeService = genericAttributeService;
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

            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();

            //get Admin
            var customer = await _customerService.GetCustomerByEmailAsync(smsTwilioSettings.AdminEmail);

            //get customer generic attributes for Phone number
            var keyGroup = customer.GetType().Name;
            var props = (await _genericAttributeService.GetAttributesForEntityAsync(customer.Id, keyGroup))
                .Where(x => x.StoreId == storeScope)
                .ToList();
            //Get phone number
            var phoneNumber = props.FirstOrDefault(prop =>
            prop.Key.ToLower() == "Phone".ToLower()).Value;

            //var address = await _addressService.GetAddressByIdAsync((int)customer.ShippingAddressId);

            //try to send SMS
            await _twilioSmsManager.SendSMSAsync(phoneNumber, "Chat started on: Friends Smoke & Vapor.");
        }

        #endregion
    }
}
