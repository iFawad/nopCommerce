using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Payments.Ghost.CashOnDelivery.Models
{
    public record PaymentInfoModel : BaseNopModel
    {
        public string DescriptionText { get; set; }
    }
}
