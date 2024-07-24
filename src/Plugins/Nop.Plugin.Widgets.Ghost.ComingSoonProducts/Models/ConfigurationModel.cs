using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Widgets.Ghost.ComingSoonProducts.Models
{
    public record ConfigurationModel : BaseNopModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Plugin.Widgets.Ghost.ComingSoonProducts.SectionName")]
        public string SectionName { get; set; }
        public bool SectionName_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugin.Widgets.Ghost.ComingSoonProducts.CategoryList")]
        public int CategoryId { get; set; }
        public bool CategoryId_OverrideForStore { get; set; }

        public IList<SelectListItem> AvailableCategories { get; set; }
    }
}
