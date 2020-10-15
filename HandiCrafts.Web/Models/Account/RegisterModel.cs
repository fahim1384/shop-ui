using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace HandiCrafts.Web.Models.Account
{
    public class RegisterModel
    {
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password is required.")]
        [RegularExpression("^[\a-zA-Z\\s\\p{N}]+$", ErrorMessage = "Only english chars and numbers")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "The {0} must be at least {2} characters")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm password is required.")]
        [DataType(DataType.Password)]
        [Compare("Password",ErrorMessage = "Confirm password is not same as password.")]
        [RegularExpression("^[\a-zA-Z\\s\\p{N}]+$", ErrorMessage = "Only english chars and numbers")]
        public string ConfirmPassword { get; set; }

        [StringLength(100, MinimumLength = 3, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.")]
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Please input valid email.")]
        public string Email { get; set; }        
        
        public string ReCaptchaKey { get; set; }        
        
        public string ReferralCode { get; set; }        
        
        [Required(ErrorMessage = "Please agree to terms and conditions.")]
        [Compare(nameof(IsTrue), ErrorMessage = "Please agree to Terms and Conditions")]
        public bool ConfirmTerms { get; set; }

        public bool IsTrue => true;
    }
}
