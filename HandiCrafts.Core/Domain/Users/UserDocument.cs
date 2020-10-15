using System;
using System.ComponentModel.DataAnnotations;
using HandiCrafts.Core.Domain;
using HandiCrafts.Core.Enums;

namespace HandiCrafts.Core.Domain.Users
{
    public  class UserDocument : BaseEntity
    {
        [Required]
        public Guid DocumentId { get; set; }
        public string File { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UploadDate { get; set; }
        public long UserId { get; set; }
        public bool IsDeleted { get; set; }
        public UserDocumentStatus DocumentStatus { get; set; }
    }
}