using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Configuration;

namespace Nop.Plugin.Widgets.Ghost.GoogleMaps
{
    public class GoogleMapsSettings : ISettings
    {
        public string MapEmbedScript { get; set; }
    }
}
