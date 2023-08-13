using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Core;
using Nop.Core.Configuration;
using Nop.Services.Configuration;
using Nop.Web.Framework.Mvc.Routing;
using Nop.Web.Infrastructure;

namespace Nop.Plugin.Widgets.Ghost.TopMenuItem.Infrastructure
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

            //TopMenuItem page
            endpointRouteBuilder.MapControllerRoute(name: "TopMenuItem",
                pattern: $"{lang}/vendor/apply",
                defaults: new { controller = "Vendor", action = "ApplyVendor" });

            //Contact us page
            endpointRouteBuilder.MapControllerRoute(name: "ContactUs2",
                pattern: $"{lang}/contactus",
                defaults: new { controller = "Common", action = "ContactUs" });

            //Home page
            endpointRouteBuilder.MapControllerRoute(name: "Homepage2",
                pattern: $"{lang}",
                defaults: new { controller = "Home", action = "Index" });
        }

        #endregion
    }
}
