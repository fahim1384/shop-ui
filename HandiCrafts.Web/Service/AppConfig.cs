using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HandiCrafts.Web.Service
{
    public class AppConfig
    {
        public string JwtSecretKey { get; set; }
        public int CacheLifeTimeMinutes { get; set; }
        public bool BypassRecaptcha { get; set; } = false;
        public string Currency { get; set; } = "ریال";
    }
}
