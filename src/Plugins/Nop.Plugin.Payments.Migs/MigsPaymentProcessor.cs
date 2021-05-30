using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Plugins;

namespace Nop.Plugin.Payments.Ghost.Migs
{
    public class MigsPaymentProcessor : BasePlugin, IPaymentMethod
    {
        #region Fields

        private readonly MigsPaymentSettings _migsPaymentSettings;
        private readonly ILocalizationService _localizationService;
        private readonly IPaymentService _paymentService;
        private readonly ISettingService _settingService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IWebHelper _webHelper;

        #endregion

        #region Ctor

        public MigsPaymentProcessor(MigsPaymentSettings migsPaymentSettings,
            ILocalizationService localizationService,
            IPaymentService paymentService,
            ISettingService settingService,
            IShoppingCartService shoppingCartService,
            IWebHelper webHelper)
        {
            _migsPaymentSettings = migsPaymentSettings;
            _localizationService = localizationService;
            _paymentService = paymentService;
            _settingService = settingService;
            _shoppingCartService = shoppingCartService;
            _webHelper = webHelper;
        }

        #endregion


        #region Methods

        /// <summary>
        /// Process a payment
        /// </summary>
        /// <param name="processPaymentRequest">Payment info required for an order processing</param>
        /// <returns>Process payment result</returns>
        public Task<ProcessPaymentResult> ProcessPaymentAsync(ProcessPaymentRequest processPaymentRequest)
        {
            var result = new ProcessPaymentResult
            {
                AllowStoringCreditCardNumber = true
            };
            
            //switch (_migsPaymentSettings.TransactMode)
            //{
            //    case TransactMode.Pending:
                    
            //        break;
            //    case TransactMode.Authorize:
                    
            //        break;
            //    case TransactMode.AuthorizeAndCapture:
                    
            //        break;
            //    default:
            //        result.AddError("Not supported transaction type");
            //        break;
            //}

            //result.NewPaymentStatus = PaymentStatus.Pending;
            //result.NewPaymentStatus = PaymentStatus.Authorized;
            result.NewPaymentStatus = PaymentStatus.Paid;


            return Task.FromResult(result);
            //return Task.FromResult(new ProcessPaymentResult());
        }

        /// <summary>
        /// Post process payment (used by payment gateways that require redirecting to a third-party URL)
        /// </summary>
        /// <param name="postProcessPaymentRequest">Payment info required for an order processing</param>
        public Task PostProcessPaymentAsync(PostProcessPaymentRequest postProcessPaymentRequest)
        {
            //nothing
            return Task.CompletedTask;
        }

        /// <summary>
        /// Returns a value indicating whether payment method should be hidden during checkout
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <returns>true - hide; false - display.</returns>
        public async Task<bool> HidePaymentMethodAsync(IList<ShoppingCartItem> cart)
        {
            //you can put any logic here
            //for example, hide this payment method if all products in the cart are downloadable
            //or hide this payment method if current customer is from certain country

            if (_migsPaymentSettings.ShippableProductRequired && !await _shoppingCartService.ShoppingCartRequiresShippingAsync(cart))
                return true;

            return false;
        }

        /// <summary>
        /// Gets additional handling fee
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <returns>Additional handling fee</returns>
        public async Task<decimal> GetAdditionalHandlingFeeAsync(IList<ShoppingCartItem> cart)
        {
            return await _paymentService.CalculateAdditionalFeeAsync(cart,
                _migsPaymentSettings.AdditionalFee, _migsPaymentSettings.AdditionalFeePercentage);
        }

        /// <summary>
        /// Captures payment
        /// </summary>
        /// <param name="capturePaymentRequest">Capture payment request</param>
        /// <returns>Capture payment result</returns>
        public Task<CapturePaymentResult> CaptureAsync(CapturePaymentRequest capturePaymentRequest)
        {
            return Task.FromResult(new CapturePaymentResult { Errors = new[] { "Capture method not supported" } });
        }

        /// <summary>
        /// Refunds a payment
        /// </summary>
        /// <param name="refundPaymentRequest">Request</param>
        /// <returns>Result</returns>
        public Task<RefundPaymentResult> RefundAsync(RefundPaymentRequest refundPaymentRequest)
        {
            return Task.FromResult(new RefundPaymentResult { Errors = new[] { "Refund method not supported" } });
        }

        /// <summary>
        /// Voids a payment
        /// </summary>
        /// <param name="voidPaymentRequest">Request</param>
        /// <returns>Result</returns>
        public Task<VoidPaymentResult> VoidAsync(VoidPaymentRequest voidPaymentRequest)
        {
            return Task.FromResult(new VoidPaymentResult { Errors = new[] { "Void method not supported" } });
        }

        /// <summary>
        /// Process recurring payment
        /// </summary>
        /// <param name="processPaymentRequest">Payment info required for an order processing</param>
        /// <returns>Process payment result</returns>
        public Task<ProcessPaymentResult> ProcessRecurringPaymentAsync(ProcessPaymentRequest processPaymentRequest)
        {
            return Task.FromResult(new ProcessPaymentResult { Errors = new[] { "Recurring payment not supported" } });
        }

        /// <summary>
        /// Cancels a recurring payment
        /// </summary>
        /// <param name="cancelPaymentRequest">Request</param>
        /// <returns>Result</returns>
        public Task<CancelRecurringPaymentResult> CancelRecurringPaymentAsync(CancelRecurringPaymentRequest cancelPaymentRequest)
        {
            return Task.FromResult(new CancelRecurringPaymentResult { Errors = new[] { "Recurring payment not supported" } });
        }

        /// <summary>
        /// Gets a value indicating whether customers can complete a payment after order is placed but not completed (for redirection payment methods)
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>Result</returns>
        public Task<bool> CanRePostProcessPaymentAsync(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            //it's not a redirection payment method. So we always return false
            return Task.FromResult(false);
        }

        /// <summary>
        /// Validate payment form
        /// </summary>
        /// <param name="form">The parsed form values</param>
        /// <returns>List of validating errors</returns>
        public Task<IList<string>> ValidatePaymentFormAsync(IFormCollection form)
        {
            return Task.FromResult<IList<string>>(new List<string>());
        }

        /// <summary>
        /// Get payment information
        /// </summary>
        /// <param name="form">The parsed form values</param>
        /// <returns>Payment info holder</returns>
        public Task<ProcessPaymentRequest> GetPaymentInfoAsync(IFormCollection form)
        {
            return Task.FromResult(new ProcessPaymentRequest());
        }

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/PaymentMigs/Configure";
        }

        /// <summary>
        /// Gets a name of a view component for displaying plugin in public store ("payment info" checkout step)
        /// </summary>
        /// <returns>View component name</returns>
        public string GetPublicViewComponentName()
        {
            return "PaymentMigs";
        }

        public override async Task InstallAsync()
        {
            var settings = new MigsPaymentSettings
            {
                DescriptionText = "<p>You will be redirected to Mastercard payment gateway for payment processing.<br />Once the payment is authorized; the order is confirmed, it will be processed.<br />Orders once confirmed, cannot be cancelled.</p>",
                SkipPaymentInfo = false
            };
            await _settingService.SaveSettingAsync(settings);

            //locales
            await _localizationService.AddLocaleResourceAsync(new Dictionary<string, string>
            {
                ["Plugin.Payments.Ghost.Migs.MerchantId"] = "Merchant Id",
                ["Plugin.Payments.Ghost.Migs.MerchantId.Hint"] = "Merchant Id provided by the payment gateway.",
                ["Plugin.Payments.Ghost.Migs.AccessCode"] = "Access Code",
                ["Plugin.Payments.Ghost.Migs.AccessCode.Hint"] = "Access Code provided by the payment gateway.",
                ["Plugin.Payments.Ghost.Migs.HashSecret"] = "Hash Secret",
                ["Plugin.Payments.Ghost.Migs.HashSecret.Hint"] = "Hash Secret provided by the payment gateway.",
                ["Plugin.Payments.Ghost.Migs.DescriptionText"] = "Description",
                ["Plugin.Payments.Ghost.Migs.DescriptionText.Hint"] = "Enter info that will be shown to customers during checkout",
                ["Plugin.Payments.Ghost.Migs.AdditionalFee"] = "Additional fee",
                ["Plugin.Payments.Ghost.Migs.AdditionalFee.Hint"] = "The additional fee.",
                ["Plugin.Payments.Ghost.Migs.AdditionalFeePercentage"] = "Additional fee. Use percentage",
                ["Plugin.Payments.Ghost.Migs.AdditionalFeePercentage.Hint"] = "Determines whether to apply a percentage additional fee to the order total. If not enabled, a fixed value is used.",
                ["Plugin.Payments.Ghost.Migs.ShippableProductRequired"] = "Shippable product required",
                ["Plugin.Payments.Ghost.Migs.ShippableProductRequired.Hint"] = "An option indicating whether shippable products are required in order to display this payment method during checkout.",
                ["Plugin.Payments.Ghost.Migs.PaymentMethodDescription"] = "Pay by \"Online payment\"",
                ["Plugin.Payments.Ghost.Migs.SkipPaymentInfo"] = "Skip payment information page",
                ["Plugin.Payments.Ghost.Migs.SkipPaymentInfo.Hint"] = "An option indicating whether we should display a payment information page for this plugin."
            });

            await base.InstallAsync();
        }

        public override async Task UninstallAsync()
        {
            //settings
            await _settingService.DeleteSettingAsync<MigsPaymentSettings>();

            //locales
            await _localizationService.DeleteLocaleResourcesAsync("Plugin.Payments.Ghost.Migs");

            await base.UninstallAsync();
        }

        /// <summary>
        /// Gets a payment method description that will be displayed on checkout pages in the public store
        /// </summary>
        /// <remarks>
        /// return description of this payment method to be display on "payment method" checkout step. good practice is to make it localizable
        /// for example, for a redirection payment method, description may be like this: "You will be redirected to PayPal site to complete the payment"
        /// </remarks>
        public async Task<string> GetPaymentMethodDescriptionAsync()
        {
            return await _localizationService.GetResourceAsync("Plugin.Payments.Ghost.Migs.PaymentMethodDescription");
        }

        #endregion


        #region Properies

        /// <summary>
        /// Gets a value indicating whether capture is supported
        /// </summary>
        public bool SupportCapture => false;

        /// <summary>
        /// Gets a value indicating whether partial refund is supported
        /// </summary>
        public bool SupportPartiallyRefund => false;

        /// <summary>
        /// Gets a value indicating whether refund is supported
        /// </summary>
        public bool SupportRefund => false;

        /// <summary>
        /// Gets a value indicating whether void is supported
        /// </summary>
        public bool SupportVoid => false;

        /// <summary>
        /// Gets a recurring payment type of payment method
        /// </summary>
        public RecurringPaymentType RecurringPaymentType => RecurringPaymentType.NotSupported;

        /// <summary>
        /// Gets a payment method type
        /// </summary>
        public PaymentMethodType PaymentMethodType => PaymentMethodType.Redirection;

        /// <summary>
        /// Gets a value indicating whether we should display a payment information page for this plugin
        /// </summary>
        public bool SkipPaymentInfo => _migsPaymentSettings.SkipPaymentInfo;

        /// <summary>
        /// Gets a payment method description that will be displayed on checkout pages in the public store
        /// </summary>
        /// <remarks>
        /// return description of this payment method to be display on "payment method" checkout step. good practice is to make it localizable
        /// for example, for a redirection payment method, description may be like this: "You will be redirected to PayPal site to complete the payment"
        /// </remarks>
        public Task<string> PaymentMethodDescription => _localizationService.GetResourceAsync("Plugin.Payments.Ghost.Migs.PaymentMethodDescription");

        #endregion
    }
}
