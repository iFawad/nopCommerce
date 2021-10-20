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
        public string TitleWholeSeller { get; set; }
        public string RouteUrlWholeSeller { get; set; }
        public string TitleContactUs { get; set; }
        public string RouteUrlContactUs { get; set; }
        public string TitleHome { get; set; }
        public string RouteUrlHome { get; set; }
    }
}
