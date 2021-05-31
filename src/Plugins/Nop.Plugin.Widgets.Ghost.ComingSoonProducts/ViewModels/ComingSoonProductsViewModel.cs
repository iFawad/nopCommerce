using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Web.Framework.Models;
using Nop.Web.Models.Catalog;

namespace Nop.Plugin.Widgets.Ghost.ComingSoonProducts.ViewModels
{
    public record ComingSoonProductsViewModel : BaseNopModel
    {
        public IList<ProductOverviewModel> Products;
        public ComingSoonProductsSettings ComingSoonProductsSettings;
    }
}
