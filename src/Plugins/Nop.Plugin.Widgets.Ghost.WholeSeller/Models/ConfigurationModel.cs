using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Widgets.Ghost.WholeSeller.Models
{
    public record ConfigurationModel : BaseNopModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Plugin.Widgets.Ghost.WholeSeller.TitleWholeSeller")]
        public string TitleWholeSeller { get; set; }
        public bool TitleWholeSeller_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugin.Widgets.Ghost.WholeSeller.RouteUrlWholeSeller")]
        public string RouteUrlWholeSeller { get; set; }
        public bool RouteUrlWholeSeller_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugin.Widgets.Ghost.WholeSeller.TitleContactUs")]
        public string TitleContactUs { get; set; }
        public bool TitleContactUs_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugin.Widgets.Ghost.WholeSeller.RouteUrlContactUs")]
        public string RouteUrlContactUs { get; set; }
        public bool RouteUrlContactUs_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugin.Widgets.Ghost.WholeSeller.TitleHome")]
        public string TitleHome { get; set; }
        public bool TitleHome_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugin.Widgets.Ghost.WholeSeller.RouteUrlHome")]
        public string RouteUrlHome { get; set; }
        public bool RouteUrlHome_OverrideForStore { get; set; }
    }
}
