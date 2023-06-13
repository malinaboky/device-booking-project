using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Configure
{
    public class AdminAuthorizationInfo
    {
        public UserInfo Admin { get; set; }
        public UserInfo Viewer { get; set; }
        public UserInfo Editor { get; set; }
    }
}
