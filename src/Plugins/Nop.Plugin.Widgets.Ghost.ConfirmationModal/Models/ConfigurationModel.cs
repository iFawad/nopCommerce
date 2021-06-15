using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Widgets.Ghost.ConfirmationModal.Models
{
    public record ConfigurationModel : BaseNopModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Plugin.Widgets.Ghost.ConfirmationModal.Topic")]
        public string Topic { get; set; }
        public bool Topic_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugin.Widgets.Ghost.ConfirmationModal.Title")]
        public string Title { get; set; }
        public bool Title_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugin.Widgets.Ghost.ConfirmationModal.YesText")]
        public string YesText { get; set; }
        public bool YesText_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugin.Widgets.Ghost.ConfirmationModal.NoText")]
        public string NoText { get; set; }
        public bool NoText_OverrideForStore { get; set; }
    }
}
