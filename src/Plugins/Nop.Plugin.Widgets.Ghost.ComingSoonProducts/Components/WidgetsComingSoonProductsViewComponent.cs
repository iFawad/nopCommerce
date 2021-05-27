using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Services.Catalog;
using Nop.Services.Security;
using Nop.Services.Stores;
//using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Framework.Components;
using Nop.Web.Factories;

namespace Nop.Plugin.Widgets.Ghost.ComingSoonProducts.Components
{
    [ViewComponent(Name = "WidgetsComingSoonProducts")]
    public class WidgetsComingSoonProductsViewComponent : NopViewComponent
    {
        private readonly ComingSoonProductsSettings _comingSoonProductsSettings;
        private readonly IAclService _aclService;
        private readonly IProductModelFactory _productModelFactory;
        private readonly IProductService _productService;
        private readonly IStoreMappingService _storeMappingService;

        public WidgetsComingSoonProductsViewComponent(ComingSoonProductsSettings comingSoonProductsSettings,
            IAclService aclService,
            IProductModelFactory productModelFactory,
            IProductService productService,
            IStoreMappingService storeMappingService)
        {
            _comingSoonProductsSettings = comingSoonProductsSettings;
            _aclService = aclService;
            _productModelFactory = productModelFactory;
            _productService = productService;
            _storeMappingService = storeMappingService;
        }

        public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
        {
            var products = await (await _productService.GetAllProductsDisplayedOnHomepageAsync())
            //ACL and store mapping
            .WhereAwait(async p => await _aclService.AuthorizeAsync(p) && await _storeMappingService.AuthorizeAsync(p))
            //availability dates
            .Where(p => _productService.ProductIsAvailable(p))
            //visible individually
            .Where(p => p.VisibleIndividually).ToListAsync();

            if (!products.Any())
                return Content("");

            var model = (await _productModelFactory.PrepareProductOverviewModelsAsync(products, true, true)).ToList();

            return View("~/Plugins/Widgets.Ghost.ComingSoonProducts/Views/ComingSoonProducts.cshtml", model);
        }
    }
}
