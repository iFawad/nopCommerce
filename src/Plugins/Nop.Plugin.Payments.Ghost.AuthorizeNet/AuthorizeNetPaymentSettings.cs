using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Configuration;

namespace Nop.Plugin.Payments.Ghost.AuthorizeNet
{
    public class AuthorizeNetPaymentSettings : ISettings
    {
        /// <summary>
        /// Get or sets Authorize.net Api Login Id
        /// </summary>
        public string ApiLoginId { get; set; }
        /// <summary>
        /// Get or sets Authorize.net Transaction Key
        /// </summary>
        public string TransactionKey { get; set; }
        /// <summary>
        /// Get or sets Authorize.net Environment.
        /// Possible values; "SANDBOX", "PRODUCTION", "LOCAL_VM", "HOSTED_VM" and "CUSTOM"
        /// </summary>
        public string Environment { get; set; }
        /// <summary>
        /// Gets or sets a additional fee
        /// </summary>
        public decimal AdditionalFee { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to "additional fee" is specified as percentage. true - percentage, false - fixed value.
        /// </summary>
        public bool AdditionalFeePercentage { get; set; }

        /// <summary>
        /// Gets or sets a payment information 
        /// </summary>
        public string DescriptionText { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether shippable products are required in order to display this payment method during checkout
        /// </summary>
        public bool ShippableProductRequired { get; set; }

        /// <summary>
        /// Gets a value indicating whether we should display a payment information page for this plugin
        /// </summary>
        public bool SkipPaymentInfo { get; set; }
    }
}
