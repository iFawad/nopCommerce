using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;
using Nop.Web.Infrastructure;

namespace Nop.Plugin.Widgets.Ghost.WholeSeller.Infrastructure
{
    public class RouteProvider : BaseRouteProvider, IRouteProvider
    {
        #region Properties

        /// <summary>
        /// Gets a priority of route provider
        /// </summary>
        public int Priority => 1003;

        #endregion

        #region Methods

        public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
        {
            //get language pattern
            //it's not needed to use language pattern in AJAX requests and for actions returning the result directly (e.g. file to download),
            //use it only for URLs of pages that the user can go to
            var lang = GetLanguageRoutePattern();

            //Whole seller page
            endpointRouteBuilder.MapControllerRoute(name: "WholeSellerApply",
                pattern: $"{lang}/vendor/apply",
                defaults: new { controller = "Vendor", action = "ApplyVendor" });
        }

        #endregion
    }
}
