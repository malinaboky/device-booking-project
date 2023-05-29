using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetEd.CoreAdmin.ViewModels.Tag
{
    public class ListOfTags
    {
        public List<TagDTO> ListType { get; set; }
        public List<TagDTO> ListOs { get; set; }
        public List<TagDTO> ListTag { get; set; }
    }
}
