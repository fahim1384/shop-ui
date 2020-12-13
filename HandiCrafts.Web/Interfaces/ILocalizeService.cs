
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HandiCrafts.Web.Interfaces
{
    public interface ILocalizeService 
    {
        Task<string> Localize(string text);
    }
}
