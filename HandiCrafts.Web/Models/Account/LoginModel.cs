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
        [Required(ErrorMessage = "ورود ایمیل اجباری می باشد")]
        public string Username { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "رمز عبور اجباری است")]
        public string Password { get; set; }
        
        public string ReturnUrl { get; set; }

        public bool RememberMe { get; set; }
        public string ReCaptchaKey { get; set; }
    }
}
