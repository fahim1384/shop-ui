using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace HandiCrafts.Web.Models.Account
{
    public class LoginContainerModel
    {
        public LoginModel LoginModel { get; set; }
        public RegisterModel RegisterModel { get; set; }
        public ForgotPasswordModel ForgotPasswordModel { get; set; }
        public string DefaultTab { get; set; }
    }
    public class LoginModel
    {
        [StringLength(100, MinimumLength = 3, ErrorMessage = "حداکثر طول ایمیل 100 و حداقل 3 کارکتر می باشد")]
        [Required(ErrorMessage = "ورود نام کاربری اجباری می باشد")]
        [Display(Name ="نام کاربری")]
        public string Username { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "رمز عبور اجباری است")]
        [Display(Name = "رمزعبور")]
        public string Password { get; set; }
        
        public string ReturnUrl { get; set; }

        public bool RememberMe { get; set; }
        public string ReCaptchaKey { get; set; }
    }
}
