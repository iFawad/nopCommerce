using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Configuration;

namespace Nop.Plugin.Widgets.Ghost.WholeSeller
{
    public class WholeSellerSettings : ISettings
    {
        public string Title { get; set; }
        public string Url { get; set; }
    }
}
