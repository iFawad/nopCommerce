using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Sms.Ghost.Twilio.Services;
using Nop.Services.Configuration;
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
        private readonly TwilioSmsManager _twilioSmsManager;

        #endregion

        #region Ctor

        public SmsTwilioExternalController(INotificationService notificationService,
            ISettingService settingService,
            IStoreContext storeContext,
            TwilioSmsManager twilioSmsManager)
        {
            _notificationService = notificationService;
            _settingService = settingService;
            _storeContext = storeContext;
            _twilioSmsManager = twilioSmsManager;
        }

        #endregion

        #region Methods

        [HttpPost]
        public async Task Notify()
        {
            //try to send SMS
            await _twilioSmsManager.SendSMSAsync("+12346578", "Chat started on: Friends Smoke & Vapor.");
        }

        #endregion
    }
}
