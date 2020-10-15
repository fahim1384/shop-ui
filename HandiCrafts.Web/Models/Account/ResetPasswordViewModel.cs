using System.ComponentModel.DataAnnotations;

namespace HandiCrafts.Web.Models.Account
{
    public class ResetPasswordViewModel
    {

        [Required(ErrorMessage = "Password is required.")]
        [RegularExpression("^[\a-zA-Z\\s\\p{N}]+$", ErrorMessage = "Only english chars and numbers")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "The {0} must be at least {2} characters")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm password is required.")]
        [RegularExpression("^[\a-zA-Z\\s\\p{N}]+$", ErrorMessage = "Only english chars and numbers")]
        [Compare("Password", ErrorMessage = "Confirm password is not same as password.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        public string Summery { get; set; }
        
        public int UserId { get; set; }
        
        public string Token { get; set; }
        
        public string ReCaptchaKey { get; set; }
    }
    
}
