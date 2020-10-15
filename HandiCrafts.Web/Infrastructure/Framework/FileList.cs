using System.Collections.Generic;
using System.Linq;

namespace HandiCrafts.Web.Infrastructure.Framework
{
    public class FileList
    {
        private readonly List<string> list = new List<string>();

        public string[] List
        {
            get
            {
                return list.Distinct().ToArray();
            }
        }

        public FileList Append(params string[] url)
        {
            list.AddRange(url);
            return this;
        }

        public FileList Prepend(params string[] url)
        {
            Insert(0, url);
            return this;
        }

        public FileList Insert(int index, params string[] url)
        {
            list.InsertRange(index, url);
            return this;
        }
    }
}
