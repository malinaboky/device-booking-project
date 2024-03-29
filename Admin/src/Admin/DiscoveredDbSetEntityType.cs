﻿using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetEd.CoreAdmin
{
    public class DiscoveredDbSetEntityType
    {
        public Type DbContextType { get; set; }
        public string Name { get; internal set; }
        public Type DbSetType { get; internal set; }
        public Type UnderlyingType { get; internal set; }
    } 
}
