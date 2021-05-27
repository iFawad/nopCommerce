using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Widgets.Ghost.ComingSoonProducts.Models
{
    public record ConfigurationModel : BaseNopModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Plugin.Widgets.Ghost.ComingSoonProducts.CategoryName")]
        public string CategoryName { get; set; }
        public bool CategoryName_OverrideForStore { get; set; }
    }
}
