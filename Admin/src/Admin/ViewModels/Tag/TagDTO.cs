using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetEd.CoreAdmin.ViewModels.Tag
{
    public class TagDTO
    {
        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public bool Selected { get; set; }
        public string Type { get; set; }
    }
}
