using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Catalog;
using Nop.Data;
using Nop.Services.Catalog;

namespace Nop.Plugin.Widgets.Ghost.ComingSoonProducts.Services
//namespace Nop.Services.Catalog
{
    public partial class CSPProductService : ICSPProductService
    {
        protected readonly IRepository<Product> _productRepository;
        protected readonly IProductService _productService;

        public CSPProductService(IRepository<Product> productRepository,
            IProductService productService)
        {
            _productRepository = productRepository;
            _productService = productService;
        }

        public IProductService GetProductServiceInstance()
        {
            return _productService;
        }

        /// <summary>
        /// Gets all products
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the products
        /// </returns>
        public virtual async Task<IList<Product>> GetAllProductsAsync()
        {
            var products = await _productRepository.GetAllAsync(query =>
            {
                return from p in query
                       orderby p.DisplayOrder, p.Id
                       where p.Published &&
                             !p.Deleted
                       select p;
            });//, cache => cache.PrepareKeyForDefaultCache(NopCatalogDefaults.ProductsHomepageCacheKey));

            return products;
        }
    }
}
