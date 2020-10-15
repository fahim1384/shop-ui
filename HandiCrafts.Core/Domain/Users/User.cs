using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using HandiCrafts.Core.Domain.Base;
using HandiCrafts.Core.Enums;
using Microsoft.AspNetCore.Identity;

namespace HandiCrafts.Core.Domain.Users
{
    public class User : IdentityUser<long>
    {
        public User()
        {
            UserRoleNames = new List<string>();
        }
        public string FirstName { get; set; } = "";

        public string LastName { get; set; } = "";
        public bool Gender { get; set; }//0 Male  1 Femail
        public string Address { get; set; } = "";
        public DateTime LastLoginDate { get; set; }
        public bool IsActive { get; set; }
        public decimal WalletCredit { get; set; }
        public string ReferralCode { get; set; }        
        public string ConfirmationToken { get; set; }        
        public string PasswordToken { get; set; }        
        public string City { get; set; }        
        public Guid? CountryId { get; set; }
        public string PostalCode { get; set; }
        public string Company { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public UserStatus Status { get; set; }

        [NotMapped]
        public List<string> UserRoleNames { get; set; }

        public string Image { get; set; }

        public bool IsDeleted { get; set; }
        public string FullName
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }
    }
}
