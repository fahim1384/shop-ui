using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HandiCrafts.Web.Areas.Seller.Models.Account
{
    public class ShortLoginModel
    {
        [Display(Name = "شماره موبایل یا ایمیل")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        //[MaxLength(11, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        //[RegularExpression("09(0[1-9]|[0-9][0-9]|3[0-9]|2[0-1])-?[0-9]{3}-?[0-9]{4}", ErrorMessage = "شماره موبایل معتبر نمی باشد")]
        [emailmobilevalidation(ErrorMessage = "شماره موبایل یا ایمیل معتبر نمی باشد")]
        public string EmailorMobileNo { get; set; }
    }

    public class AcceptCodeModel
    {
        public string DisplayMobileEmail { get; set; }

        [Required(ErrorMessage = "فیلد اجباری")]
        [Display(Name = "کد کاربر")]
        public long UserId { get; set; }

        [Display(Name = "کد تایید")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [RegularExpression("[0-9]{4}", ErrorMessage = "کد تایید معتبر نمی باشد")]
        public string AcceptCode { get; set; }
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
    }

    public class PersonalInformationVModel
    {
        [Display(Name = "نام")]
        [Required(ErrorMessage = "فیلد {0} اجباری است")]
        public string FirstName { get; set; }

        [Display(Name = "نام خانوادگی")]
        [Required(ErrorMessage = "فیلد {0} اجباری است")]
        public string LastName { get; set; }

        [Display(Name = "تاریخ تولد")]
        [Required(ErrorMessage = "فیلد {0} اجباری است")]
        public string BirthDate { get; set; }

        [Display(Name = "جنسیت")]
        [Required(ErrorMessage = "فیلد {0} اجباری است")]
        public int Gender { get; set; }

        [Display(Name = "شماره شناسنامه")]
        //[Required(ErrorMessage = "فیلد {0} اجباری است")]
        public string ShenasnameNo { get; set; }

        [Display(Name = "کد ملی")]
        [Required(ErrorMessage = "فیلد {0} اجباری است")]
        [RegularExpression("([0-9]){10}", ErrorMessage = "فیلد {0} صحیح نمی‌باشد")]
        public string NationalCode { get; set; }

        [Display(Name = "استان")]
        [Required(ErrorMessage = "فیلد {0} اجباری است")]
        public string Province { get; set; }

        [Display(Name = "شهر")]
        [Required(ErrorMessage = "فیلد {0} اجباری است")]
        public string City { get; set; }

        [Display(Name = "آدرس")]
        [Required(ErrorMessage = "فیلد {0} اجباری است")]
        public string Address { get; set; }

        [Display(Name = "کد پستی")]
        [Required(ErrorMessage = "فیلد {0} اجباری است")]
        //[DataType(DataType.PostalCode)]
        //[RegularExpression(pattern: @"\b(?!(\d)\1{3})[13-9]{4}[1346-9][013-9]{5}\b", ErrorMessage ="کد پستی شما نادرست می‌باشد")]
        [RegularExpression(@"^(?!00000)[0-9]{10,10}$", ErrorMessage = "کد پستی شما نادرست می‌باشد")]
        public string PostalCode { get; set; }

        [Display(Name = "طول جغرافیایی")]
        [Required(ErrorMessage = "فیلد {0} اجباری است")]
        public string Lat { get; set; }

        [Display(Name = "عرض جغرافیایی")]
        [Required(ErrorMessage = "فیلد {0} اجباری است")]
        public string Lng { get; set; }

        [Display(Name = "تلفن ثابت")]
        [Required(ErrorMessage = "فیلد {0} اجباری است")]
        [RegularExpression("^0[0-9]{10}$", ErrorMessage = "شماره تلفن معتبر نمی باشد")]
        public string Phone { get; set; }

        [Display(Name = "تلفن همراه")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(11, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        [RegularExpression("09(0[1-9]|[0-9][0-9]|3[0-9]|2[0-1])-?[0-9]{3}-?[0-9]{4}", ErrorMessage = "شماره موبایل معتبر نمی باشد")]
        public string MobileNo { get; set; }

        [Display(Name = "تلفن همراه(ثانویه)")]
        //[Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        //[MaxLength(11, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        [RegularExpression("^$|09(0[1-9]|[0-9][0-9]|3[0-9]|2[0-1])-?[0-9]{3}-?[0-9]{4}", ErrorMessage = "شماره موبایل معتبر نمی باشد")]
        public string MobileNo2 { get; set; }

        [Display(Name = "شماره شبا(به نام شخص)")]
        [Required(ErrorMessage = "فیلد {0} اجباری است")]
        public string ShabaCode { get; set; }

        [Display(Name = "رمز عبور")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "فیلد {0} اجباری است")]
        [RegularExpression("^[\a-zA-Z\\s\\p{N}]+$", ErrorMessage = "فقط اعداد و حروف لاتین مجاز می باشد")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "تعداد کارکتر مجاز بین {1} تا {2} می باشد")]
        public string Password { get; set; }

        [Required(ErrorMessage = "فیلد اجباری")]
        [Display(Name = "کد کاربر")]
        public long UserId { get; set; }

        //[Display(Name = "کد تایید")]
        //[Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        //[RegularExpression("[0-9]{5}", ErrorMessage = "کد تایید معتبر نمی باشد")]
        //public int AcceptCode { get; set; }
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
