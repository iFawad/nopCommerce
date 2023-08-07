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
using Nop.Plugin.Widgets.Ghost.ComingSoonProducts.Services;
using Nop.Plugin.Widgets.Ghost.ComingSoonProducts.ViewModels;
using Nop.Services.Configuration;
using Nop.Core;

namespace Nop.Plugin.Widgets.Ghost.ComingSoonProducts.Components
{
    [ViewComponent(Name = "WidgetsComingSoonProducts")]
    public class WidgetsComingSoonProductsViewComponent : NopViewComponent
    {
        private readonly ComingSoonProductsSettings _comingSoonProductsSettings;
        private readonly IAclService _aclService;
        private readonly IProductModelFactory _productModelFactory;
        private readonly ICSPProductService _cspProductService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly ICategoryService _categoryService;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;

        public WidgetsComingSoonProductsViewComponent(ComingSoonProductsSettings comingSoonProductsSettings,
            IAclService aclService,
            IProductModelFactory productModelFactory,
            ICSPProductService cspProductService,
            IStoreMappingService storeMappingService,
            ICategoryService categoryService,
            ISettingService settingService,
            IStoreContext storeContext)
        {
            _comingSoonProductsSettings = comingSoonProductsSettings;
            _aclService = aclService;
            _productModelFactory = productModelFactory;
            _cspProductService = cspProductService;
            _storeMappingService = storeMappingService;
            _categoryService = categoryService;
            _settingService = settingService;
            _storeContext = storeContext;
        }

        public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
        {
            ComingSoonProductsViewModel viewModel = new ComingSoonProductsViewModel();
            int categoryId = _comingSoonProductsSettings.CategoryId;
            var products = await (await _cspProductService.GetAllProductsAsync())
            //ACL and store mapping
            .WhereAwait(async p => await _aclService.AuthorizeAsync(p) && await _storeMappingService.AuthorizeAsync(p))
            //availability dates
            .Where(p => _cspProductService.GetProductServiceInstance().ProductIsAvailable(p))
            //visible individually
            .Where(p => p.VisibleIndividually)
            .ToListAsync();

            //var comingSoonCategory = await _categoryService.GetCategoryByIdAsync(categoryId);

            //if (comingSoonCategory != null)
            //    categoryId = comingSoonCategory.Id;

            var existingProductCategories = await _categoryService.GetProductCategoriesByCategoryIdAsync(categoryId, showHidden: true);
            
            if(existingProductCategories.Count < 1)
                return Content("");

            //var filteredProducts = products.Where(product =>
            //    existingProductCategories.All(productCategory =>
            //    productCategory.ProductId == product.Id))
            //    .ToList();

            //var filteredProducts =
            //    from product in products
            //    where existingProductCategories.All(productCategory =>
            //    productCategory.ProductId == product.Id)
            //    select product;

            var filteredProducts = products.Join(existingProductCategories, product => 
            product.Id, existingProductCategory => 
            existingProductCategory.ProductId, (product, existingProductCategory) => 
            product);

            if (!filteredProducts.Any())
                return Content("");

            viewModel.Products = (await _productModelFactory.PrepareProductOverviewModelsAsync(filteredProducts, true, true)).ToList();

            //Get Settings
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            viewModel.ComingSoonProductsSettings = await _settingService.LoadSettingAsync<ComingSoonProductsSettings>(storeScope);

            return View("~/Plugins/Widgets.Ghost.ComingSoonProducts/Views/ComingSoonProducts.cshtml", viewModel);
        }
    }
}
