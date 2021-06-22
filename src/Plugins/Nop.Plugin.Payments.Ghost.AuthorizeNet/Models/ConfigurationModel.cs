using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Payments.Ghost.AuthorizeNet.Models
{
    public record ConfigurationModel : BaseNopModel, ILocalizedModel<ConfigurationModel.ConfigurationLocalizedModel>
    {
        public ConfigurationModel()
        {
            Locales = new List<ConfigurationLocalizedModel>();
        }

        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Plugin.Payments.Ghost.AuthorizeNet.DescriptionText")]
        public string DescriptionText { get; set; }
        public bool DescriptionText_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugin.Payments.Ghost.AuthorizeNet.AdditionalFee")]
        public decimal AdditionalFee { get; set; }
        public bool AdditionalFee_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugin.Payments.Ghost.AuthorizeNet.AdditionalFeePercentage")]
        public bool AdditionalFeePercentage { get; set; }
        public bool AdditionalFeePercentage_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugin.Payments.Ghost.AuthorizeNet.ShippableProductRequired")]
        public bool ShippableProductRequired { get; set; }
        public bool ShippableProductRequired_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugin.Payments.Ghost.AuthorizeNet.SkipPaymentInfo")]
        public bool SkipPaymentInfo { get; set; }
        public bool SkipPaymentInfo_OverrideForStore { get; set; }

        public IList<ConfigurationLocalizedModel> Locales { get; set; }

        #region Nested class

        public partial class ConfigurationLocalizedModel : ILocalizedLocaleModel
        {
            public int LanguageId { get; set; }

            [NopResourceDisplayName("Plugin.Payment.Ghost.AuthorizeNet.DescriptionText")]
            public string DescriptionText { get; set; }
        }

        #endregion
    }
}
