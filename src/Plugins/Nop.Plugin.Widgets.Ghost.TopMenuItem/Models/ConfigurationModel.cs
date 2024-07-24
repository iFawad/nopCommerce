using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Widgets.Ghost.TopMenuItem.Models
{
    public record ConfigurationModel : BaseNopModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Plugin.Widgets.Ghost.TopMenuItem.TitleTopMenuItem")]
        public string TitleTopMenuItem { get; set; }
        public bool TitleTopMenuItem_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugin.Widgets.Ghost.TopMenuItem.RouteUrlTopMenuItem")]
        public string RouteUrlTopMenuItem { get; set; }
        public bool RouteUrlTopMenuItem_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugin.Widgets.Ghost.TopMenuItem.TitleContactUs")]
        public string TitleContactUs { get; set; }
        public bool TitleContactUs_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugin.Widgets.Ghost.TopMenuItem.RouteUrlContactUs")]
        public string RouteUrlContactUs { get; set; }
        public bool RouteUrlContactUs_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugin.Widgets.Ghost.TopMenuItem.TitleHome")]
        public string TitleHome { get; set; }
        public bool TitleHome_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugin.Widgets.Ghost.TopMenuItem.RouteUrlHome")]
        public string RouteUrlHome { get; set; }
        public bool RouteUrlHome_OverrideForStore { get; set; }
    }
}
