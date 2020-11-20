using System;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

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
        [Compare("Password", ErrorMessage = "رمزها باهم تطابقت ندارد")]
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

    public class ProfileInfo
    {
        /*[Display(Name = "نام")]
        [Required(ErrorMessage = "فیلد {0} اجباری است")]
        public string FirstName { get; set; }

        [Display(Name = "نام خانوادگی")]
        [Required(ErrorMessage = "فیلد {0} اجباری است")]
        public string LastName { get; set; }*/

        [Display(Name = "نام و نام خانوادگی")]
        [Required(ErrorMessage = "فیلد {0} اجباری است")]
        public string FullName { get; set; }

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
        [Compare("Password", ErrorMessage = "رمزها باهم تطابقت ندارد")]
        [RegularExpression("^[\a-zA-Z\\s\\p{N}]+$", ErrorMessage = "فقط اعداد و حروف لاتین مجاز می باشد")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "ایمیل")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "فیلد {0} بایستی بین {2} الی {1} کاراکتر داشته باشد")]
        [Required(ErrorMessage = "فیلد {0} اجباری است")]
        [EmailAddress(ErrorMessage = "لطفا ایمیل معتبر وارد نمایید")]
        public string Email { get; set; }

        [Display(Name = "کد ملی")]
        [Required(ErrorMessage = "فیلد {0} اجباری است")]
        [CodeMelli(ErrorMessage ="کد ملی وارد شده نامعتبر می باشد")]
        public string MelliCode { get; set; }

        [Display(Name = "تاریخ تولد")]
        [Required(ErrorMessage = "فیلد {0} اجباری است")]
        public string Bdate { get; set; }

        [Display(Name = "شغل")]
        [Required(ErrorMessage = "فیلد {0} اجباری است")]
        public string Shoghl { get; set; }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public class CodeMelliAttribute : ValidationAttribute
    {
        #region IsValidNationalCode بررسی صحت کدملی

        /*public CodeMelliAttribute(string ErrorMessage)
            : base()
        {
            this.ErrorMessage = ErrorMessage;
        }*/

        //public override bool IsValid(object value)
        //{
        //    if (value == null) return new ValidationResult(ErrorMessage);
        //    if (!Utility.IsValidNationalCode(value.ToString())) return new ValidationResult(ErrorMessage);
        //    return ValidationResult.Success;
        //}
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null) return new ValidationResult("فیلد کد ملی را وارد نمایید");
            if (IsValidNationalCode(value.ToString()) == false) return new ValidationResult("فیلد کد ملی معتبر نمی باشد");
            return ValidationResult.Success;
        }
        public bool IsValidNationalCode(string nationalcode)
        {
            if (string.IsNullOrEmpty(nationalcode)) return false;
            if (!new Regex(@"\d{10}").IsMatch(nationalcode)) return false;
            var array = nationalcode.ToCharArray();
            //if (array.Length != 10) return false;
            //for (int i = 0; i < array.Length; i++)
            //{
            //    if (!char.IsDigit(array[i])) return false;
            //}
            var allDigitEqual = new[] { "0000000000", "1111111111", "2222222222", "3333333333", "4444444444", "5555555555", "6666666666", "7777777777", "8888888888", "9999999999" };
            if (allDigitEqual.Contains(nationalcode)) return false;
            var j = 10;
            var sum = 0;
            for (var i = 0; i < array.Length - 1; i++)
            {
                sum += Int32.Parse(array[i].ToString(CultureInfo.InvariantCulture)) * j;
                j--;
            }
            var div = sum / 11;
            var r = div * 11;
            var diff = Math.Abs(sum - r);
            if (diff <= 2)
            {
                return diff == Int32.Parse(array[9].ToString(CultureInfo.InvariantCulture));
            }
            var temp = Math.Abs(diff - 11);
            return temp == Int32.Parse(array[9].ToString(CultureInfo.InvariantCulture));
        }
        #endregion
    }
}
