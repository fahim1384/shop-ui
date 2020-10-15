using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HandiCrafts.Web.Service
{
    public class JwtToken
    {
        public string AccessToken { get; set; }
        public int ExpiredIn { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
    }
}
