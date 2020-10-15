using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace HandiCrafts.Web.Models.Account
{
    public class ForgotPasswordModel
    {       

        [MaxLength(50,ErrorMessage = "Maximum length should be 50 characters.")]
        [Required(ErrorMessage = "Email is required.")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }        
        
        public string Summery { get; set; }
    }
    
}
