using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Configuration;

namespace Nop.Plugin.Widgets.Ghost.TopMenuItem
{
    public class TopMenuItemSettings : ISettings
    {
        public string TitleTopMenuItem { get; set; }
        public string RouteUrlTopMenuItem { get; set; }
        public string TitleContactUs { get; set; }
        public string RouteUrlContactUs { get; set; }
        public string TitleHome { get; set; }
        public string RouteUrlHome { get; set; }
    }
}
