﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Configuration;

namespace Nop.Plugin.Widgets.Ghost.ComingSoonProducts
{
    public class ComingSoonProductsSettings : ISettings
    {
        public string CategoryName { get; set; }
        public string SectionName { get; set; }
    }
}
