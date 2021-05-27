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
        private readonly ICatalogModelFactory _catalogModelFactory;
        private readonly ICategoryService _categoryService;

        public WidgetsComingSoonProductsViewComponent(ComingSoonProductsSettings comingSoonProductsSettings,
            IAclService aclService,
            IProductModelFactory productModelFactory,
            IProductService productService,
            IStoreMappingService storeMappingService,
            ICatalogModelFactory catalogModelFactory,
            ICategoryService categoryService)
        {
            _comingSoonProductsSettings = comingSoonProductsSettings;
            _aclService = aclService;
            _productModelFactory = productModelFactory;
            _productService = productService;
            _storeMappingService = storeMappingService;
            _catalogModelFactory = catalogModelFactory;
            _categoryService = categoryService;
        }

        public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
        {
            string categoryName = "Coming-soon";
            if (!string.IsNullOrWhiteSpace(_comingSoonProductsSettings.CategoryName))
            {
                categoryName = _comingSoonProductsSettings.CategoryName;
            }
            var products = await (await _productService.GetAllProductsAsync())
            //ACL and store mapping
            .WhereAwait(async p => await _aclService.AuthorizeAsync(p) && await _storeMappingService.AuthorizeAsync(p))
            //availability dates
            .Where(p => _productService.ProductIsAvailable(p))
            //visible individually
            .Where(p => p.VisibleIndividually)
            .ToListAsync();

            var allCategories = await _categoryService.GetAllCategoriesAsync();
            var comingSoonCategory = allCategories.Where(category => category.Name == categoryName).FirstOrDefault();
            int categoryId = 0;

            if (comingSoonCategory != null)
                categoryId = comingSoonCategory.Id;

            var existingProductCategories = await _categoryService.GetProductCategoriesByCategoryIdAsync(categoryId, showHidden: true);
            
            if(existingProductCategories.Count < 1)
                return Content("");

            products = products.Where(product => 
                existingProductCategories.All(productCategory => 
                productCategory.ProductId == product.Id))
                .ToList();

            if (!products.Any())
                return Content("");

            var model = (await _productModelFactory.PrepareProductOverviewModelsAsync(products, true, true)).ToList();
            return View("~/Plugins/Widgets.Ghost.ComingSoonProducts/Views/ComingSoonProducts.cshtml", model);
        }
    }
}
