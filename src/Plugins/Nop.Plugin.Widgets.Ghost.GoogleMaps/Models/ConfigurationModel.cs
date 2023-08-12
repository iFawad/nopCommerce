using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Widgets.Ghost.GoogleMaps.Models
{
    public record ConfigurationModel : BaseNopModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Plugin.Widgets.Ghost.GoogleMaps.MapEmbedScript")]
        public string MapEmbedScript { get; set; }
        public bool MapEmbedScript_OverrideForStore { get; set; }
    }
}
