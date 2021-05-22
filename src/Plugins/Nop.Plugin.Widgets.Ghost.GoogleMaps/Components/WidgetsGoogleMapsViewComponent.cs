using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Widgets.Ghost.GoogleMaps.Components
{
    [ViewComponent(Name = "WidgetsGoogleMaps")]
    public class WidgetsGoogleMapsViewComponent : NopViewComponent
    {
        private readonly GoogleMapsSettings _googleMapsSettings;

        public WidgetsGoogleMapsViewComponent(GoogleMapsSettings googleMapsSettings)
        {
            _googleMapsSettings = googleMapsSettings;
        }

        public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
        {
            return View("~/Plugins/Widgets.Ghost.GoogleMaps/Views/GoogleMapsWidget.cshtml", _googleMapsSettings.MapEmbedScript);
        }
    }
}
