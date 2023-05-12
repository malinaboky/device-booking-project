using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetEd.CoreAdmin.ViewModels.Device
{
    public class DeviceDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public bool Selected { get; set; }
        public string Os { get; set; }
        public double Diagonal { get; set; }
        public string ImagePath { get; set; }
        public string QrPath { get; set; }

    }
}
