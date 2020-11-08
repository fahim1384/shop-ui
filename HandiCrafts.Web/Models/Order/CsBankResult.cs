using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HandiCrafts.Web.Models.Order
{
    public class CsBankResult
    {
        public string Authority { get; set; }
        public string Status { get; set; }

        public string Message { get; set; }
    }
}
