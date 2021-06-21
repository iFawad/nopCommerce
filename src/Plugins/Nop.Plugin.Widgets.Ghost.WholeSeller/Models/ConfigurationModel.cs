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

        [NopResourceDisplayName("Plugin.Widgets.Ghost.WholeSeller.Title")]
        public string Title { get; set; }
        public bool Title_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugin.Widgets.Ghost.WholeSeller.Url")]
        public string Url { get; set; }
        public bool Url_OverrideForStore { get; set; }
    }
}
