using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace HandiCrafts.Web.Models.Account
{
    public class RegisterModel
    {
        [Display(Name = "نام")]
        [Required(ErrorMessage = "فیلد {0} اجباری است")]
        public string FirstName { get; set; }

        [Display(Name = "نام خانوادگی")]
        [Required(ErrorMessage = "فیلد {0} اجباری است")]
        public string LastName { get; set; }

        [Display(Name = "شماره موبایل")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(11, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        [RegularExpression("09(0[1-9]|[0-9][0-9]|3[0-9]|2[0-1])-?[0-9]{3}-?[0-9]{4}", ErrorMessage = "شماره موبایل معتبر نمی باشد")]
        public string MobileNo { get; set; }

        [Display(Name = "رمز عبور")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "فیلد {0} اجباری است")]
        [RegularExpression("^[\a-zA-Z\\s\\p{N}]+$", ErrorMessage = "فقط اعداد و حروف لاتین مجاز می باشد")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "تعداد کارکتر مجاز بین {1} تا {2} می باشد")]
        public string Password { get; set; }

        [Display(Name = "تکرار رمز عبور")]
        [Required(ErrorMessage = "فیلد {0} اجباری است")]
        [DataType(DataType.Password)]
        [Compare("Password",ErrorMessage = "رمزها باهم تطابقت ندارد")]
        [RegularExpression("^[\a-zA-Z\\s\\p{N}]+$", ErrorMessage = "فقط اعداد و حروف لاتین مجاز می باشد")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "ایمیل")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "فیلد {0} بایستی بین {2} الی {1} کاراکتر داشته باشد")]
        [Required(ErrorMessage = "فیلد {0} اجباری است")]
        [EmailAddress(ErrorMessage = "لطفا ایمیل معتبر وارد نمایید")]
        public string Email { get; set; }        
        
        //public string ReCaptchaKey { get; set; }        
        
        //public string ReferralCode { get; set; }        
        
        //[Required(ErrorMessage = "Please agree to terms and conditions.")]
        //[Compare(nameof(IsTrue), ErrorMessage = "Please agree to Terms and Conditions")]
        //public bool ConfirmTerms { get; set; }

        //public bool IsTrue => true;
    }
}
