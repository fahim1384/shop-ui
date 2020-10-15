using System;

namespace HandiCrafts.Core.Domain.Users
{
    public class Document : BaseEntity
    {
        public string Title { get; set; }
        public long CreatedUserId { get; set; }
        public string IconClass { get; set; }
        public bool IsDelete { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? UpdatedDate { get; set; }
    }
}