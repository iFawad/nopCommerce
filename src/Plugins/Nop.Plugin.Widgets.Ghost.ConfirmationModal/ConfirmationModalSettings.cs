using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Configuration;

namespace Nop.Plugin.Widgets.Ghost.ConfirmationModal
{
    public class ConfirmationModalSettings : ISettings
    {
        public string Topic { get; set; }
        public string Title { get; set; }
        public string YesText { get; set; }
        public string NoText { get; set; }
    }
}
