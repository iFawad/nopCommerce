using Nop.Web.Framework.Models;

namespace Nop.Plugin.Payments.Ghost.DirectTransfer.Models
{
    public record PaymentInfoModel : BaseNopModel
    {
        public string DescriptionText { get; set; }
    }
}
