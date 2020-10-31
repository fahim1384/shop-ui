using System;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Text.RegularExpressions;

namespace HandiCrafts.Web.Models.Account
{
    public class LoginContainerModel
    {
        public ShortLoginModel ShortLoginModel { get; set; }
        public AcceptCodeModel AcceptCodeModel { get; set; }
        public LoginModel LoginModel { get; set; }
        public RegisterModel RegisterModel { get; set; }
        public ForgotPasswordModel ForgotPasswordModel { get; set; }
        public string DefaultTab { get; set; }
    }
    public class LoginModel
    {
        [StringLength(100, MinimumLength = 3, ErrorMessage = "حداکثر طول ایمیل 100 و حداقل 3 کارکتر می باشد")]
        [Required(ErrorMessage = "ورود نام کاربری اجباری می باشد")]
        [Display(Name = "نام کاربری")]
        public string Username { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "رمز عبور اجباری است")]
        [Display(Name = "رمزعبور")]
        public string Password { get; set; }

        public string ReturnUrl { get; set; }

        public bool RememberMe { get; set; }
        public string ReCaptchaKey { get; set; }
    }

    public class ShortLoginModel
    {
        [Display(Name = "شماره موبایل یا ایمیل")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        //[MaxLength(11, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        //[RegularExpression("09(0[1-9]|[0-9][0-9]|3[0-9]|2[0-1])-?[0-9]{3}-?[0-9]{4}", ErrorMessage = "شماره موبایل معتبر نمی باشد")]
        [emailmobilevalidation(ErrorMessage = "شماره موبایل یا ایمیل معتبر نمی باشد")]
        public string EmailorMobileNo { get; set; }
    }

    public class ShortRegisterModel
    {
        [Required(ErrorMessage = "فیلد اجباری")]
        [Display(Name = "کد کاربر")]
        public long UserId { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "رمز عبور اجباری است")]
        [Display(Name = "رمزعبور")]
        public string Password { get; set; }

        //[Display(Name = "شماره موبایل")]
        //[Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        //[MaxLength(11, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        //[RegularExpression("09(0[1-9]|[0-9][0-9]|3[0-9]|2[0-1])-?[0-9]{3}-?[0-9]{4}", ErrorMessage = "شماره موبایل معتبر نمی باشد")]
        //public string MobileNo { get; set; }
    }

    public class AcceptCodeModel
    {
        [Required(ErrorMessage = "فیلد اجباری")]
        [Display(Name = "کد کاربر")]
        public long UserId { get; set; }

        [Display(Name = "کد تایید")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        //[MaxLength(6, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        [RegularExpression("[0-9]", ErrorMessage = "کد تایید معتبر نمی باشد")]
        public int AcceptCode { get; set; }
    }

    [System.AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class emailmobilevalidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                string email = value.ToString();
                if (Regex.IsMatch(email, @"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", RegexOptions.IgnoreCase))
                {
                    return ValidationResult.Success;
                }
                else if (Regex.IsMatch(email, @"09(0[1-9]|[0-9][0-9]|3[0-9]|2[0-1])-?[0-9]{3}-?[0-9]{4}", RegexOptions.IgnoreCase))
                {
                    return ValidationResult.Success;
                }
                else
                {
                    return new ValidationResult("شماره موبایل یا ایمیل معتبر نمی باشد");
                }

            }
            else
            {
                return new ValidationResult("" + validationContext.DisplayName + "اجباری است");
            }
            //return base.IsValid(value, validationContext);
        }

    }
}
