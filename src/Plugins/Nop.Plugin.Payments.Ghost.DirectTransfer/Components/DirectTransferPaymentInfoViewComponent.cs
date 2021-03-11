using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Payments.Ghost.DirectTransfer.Models;
using Nop.Services.Localization;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Payments.Ghost.DirectTransfer.Components
{
    [ViewComponent(Name = "DirectTransfer")]
    public class DirectTransferPaymentInfoViewComponent : NopViewComponent
    {
        private readonly DirectTransferPaymentSettings _directTransferPaymentSettings;
        private readonly ILocalizationService _localizationService;
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;

        public DirectTransferPaymentInfoViewComponent(DirectTransferPaymentSettings directTransferPaymentSettings,
            ILocalizationService localizationService,
            IStoreContext storeContext,
            IWorkContext workContext)
        {
            _directTransferPaymentSettings = directTransferPaymentSettings;
            _localizationService = localizationService;
            _storeContext = storeContext;
            _workContext = workContext;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = new PaymentInfoModel
            {
                DescriptionText = await _localizationService.GetLocalizedSettingAsync(_directTransferPaymentSettings,
                    x => x.DescriptionText, (await _workContext.GetWorkingLanguageAsync()).Id, (await _storeContext.GetCurrentStoreAsync()).Id)
            };

            return View("~/Plugins/Payments.Ghost.DirectTransfer/Views/PaymentInfo.cshtml", model);
        }
    }
}
