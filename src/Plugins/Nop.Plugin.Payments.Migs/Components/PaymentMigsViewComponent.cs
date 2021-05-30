using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Plugin.Payments.Ghost.Migs.Models;
using Nop.Services.Localization;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Payments.Ghost.Migs.Components
{
    [ViewComponent(Name = "PaymentMigs")]
    public class PaymentMigsViewComponent : NopViewComponent
    {
        #region Fields

        private readonly MigsPaymentSettings _migsPaymentSettings;
        private readonly ILocalizationService _localizationService;
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public PaymentMigsViewComponent(MigsPaymentSettings migsPaymentSettings,
            ILocalizationService localizationService,
            IStoreContext storeContext,
            IWorkContext workContext)
        {
            _migsPaymentSettings = migsPaymentSettings;
            _localizationService = localizationService;
            _storeContext = storeContext;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = new PaymentInfoModel()
            {
                DescriptionText = await _localizationService.GetLocalizedSettingAsync(_migsPaymentSettings,
                    x => x.DescriptionText, (await _workContext.GetWorkingLanguageAsync()).Id, (await _storeContext.GetCurrentStoreAsync()).Id)
            };


            //set postback values (we cannot access "Form" with "GET" requests)
            if (Request.Method != WebRequestMethods.Http.Get)
            {
                var form = Request.Form;
                
            }

            return View("~/Plugins/Payments.Ghost.Migs/Views/PaymentInfo.cshtml", model);
        }

        #endregion
    }
}
