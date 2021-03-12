﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Payments.Ghost.CashOnDelivery.Models;
using Nop.Services.Localization;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Payments.Ghost.CashOnDelivery.Components
{
    [ViewComponent(Name = "CashOnDelivery")]
    public class PaymentCashOnDeliveryViewComponent : NopViewComponent
    {
        #region Fields

        private readonly CashOnDeliveryPaymentSettings _cashOnDeliveryPaymentSettings;
        private readonly ILocalizationService _localizationService;
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public PaymentCashOnDeliveryViewComponent(CashOnDeliveryPaymentSettings cashOnDeliveryPaymentSettings,
            ILocalizationService localizationService,
            IStoreContext storeContext,
            IWorkContext workContext)
        {
            _cashOnDeliveryPaymentSettings = cashOnDeliveryPaymentSettings;
            _localizationService = localizationService;
            _storeContext = storeContext;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = new PaymentInfoModel
            {
                DescriptionText = await _localizationService.GetLocalizedSettingAsync(_cashOnDeliveryPaymentSettings,
                    x => x.DescriptionText, (await _workContext.GetWorkingLanguageAsync()).Id, (await _storeContext.GetCurrentStoreAsync()).Id)
            };

            return View("~/Plugins/Payments.Ghost.CashOnDelivery/Views/PaymentInfo.cshtml", model);
        }

        #endregion

    }
}
