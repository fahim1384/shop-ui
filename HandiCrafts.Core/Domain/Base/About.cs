using System;

namespace HandiCrafts.Core.Domain.Base
{
    public class About :
         BaseEntity
    {
        public string Body { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}