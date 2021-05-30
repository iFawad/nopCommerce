using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;

namespace Nop.Plugin.Widgets.Ghost.ComingSoonProducts.Services
//namespace Nop.Services.Catalog
{
    public partial interface ICSPProductService
    {
        /// <summary>
        /// Gets all products
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the products
        /// </returns>
        Task<IList<Product>> GetAllProductsAsync();

        public IProductService GetProductServiceInstance();
    }
}
