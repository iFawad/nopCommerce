using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Payments.Ghost.AuthorizeNet.Models;
using Nop.Services.Localization;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Payments.Ghost.AuthorizeNet.Components
{
    [ViewComponent(Name = "AuthorizeNet")]
    public class PaymentAuthorizeNetViewComponent : NopViewComponent
    {
        #region Fields

        private readonly AuthorizeNetPaymentSettings _authorizeNetPaymentSettings;
        private readonly ILocalizationService _localizationService;
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public PaymentAuthorizeNetViewComponent(AuthorizeNetPaymentSettings authorizeNetPaymentSettings,
            ILocalizationService localizationService,
            IStoreContext storeContext,
            IWorkContext workContext)
        {
            _authorizeNetPaymentSettings = authorizeNetPaymentSettings;
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
                DescriptionText = await _localizationService.GetLocalizedSettingAsync(_authorizeNetPaymentSettings,
                    x => x.DescriptionText, (await _workContext.GetWorkingLanguageAsync()).Id, (await _storeContext.GetCurrentStoreAsync()).Id)
            };

            return View("~/Plugins/Payments.Ghost.AuthorizeNet/Views/PaymentInfo.cshtml", model);
        }

        #endregion
    }
}
