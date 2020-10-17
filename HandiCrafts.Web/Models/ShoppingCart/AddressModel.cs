using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HandiCrafts.Web.Models.ShoppingCart
{
    public class AddressContainerModel
    {
        public List<AddressModel> AddressList { get; set; }
    }
    public class AddressModel
    {

        [Display(Name = "Lng")]
        public decimal Lng { get; set; }

        [Display(Name ="Lat")]
        public decimal Lat { get; set; }

        [Display(Name = "استان")]
        [Required(ErrorMessage = "فیلد {0} اجباری است")]
        public int SatatId { get; set; }

        [Display(Name = "شهر")]
        [Required(ErrorMessage = "فیلد {0} اجباری است")]
        public int CityId { get; set; }

        [Display(Name = "محله")]
        [Required(ErrorMessage = "فیلد {0} اجباری است")]
        public int DistrictId { get; set; }

        public string SatatStr { get; set; }

        public string CityStr { get; set; }

        public string DistrictStr { get; set; }

        [Display(Name = "آدرس پستی")]
        [Required(ErrorMessage = "فیلد {0} اجباری است")]
        [StringLength(250, MinimumLength = 6, ErrorMessage = "حداکثر طول آدرس پستی 250 و حداقل 3 کارکتر می باشد")]
        public string PostalAddress { get; set; }

        [Display(Name = "پلاک")]
        [Required(ErrorMessage = "فیلد {0} اجباری است")]
        public string Pelak { get; set; }

        [Display(Name = "واحد")]
        [Required(ErrorMessage = "فیلد {0} اجباری است")]
        public string AptId { get; set; }

        [Display(Name = "کد پستی")]
        [Required(ErrorMessage = "فیلد {0} اجباری است")]
        public string PostalCode { get; set; }

        [Display(Name = "گیرنده سفارش خودم هستم")]
        public bool RecipientIsSelf { get; set; }

        [Display(Name = "نام")]
        [Required(ErrorMessage = "فیلد {0} اجباری است")]
        public string FirstName { get; set; }

        [Display(Name = "نام خانوادگی")]
        [Required(ErrorMessage = "فیلد {0} اجباری است")]
        public string LastName { get; set; }

        [Display(Name = "شماره ملی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(10, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string NationalCode { get; set; }

        [Display(Name = "شماره موبایل")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(11, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        [RegularExpression("09(0[1-9]|[0-9][0-9]|3[0-9]|2[0-1])-?[0-9]{3}-?[0-9]{4}", ErrorMessage = "شماره موبایل معتبر نمی باشد")]
        public string MobileNo { get; set; }

    }
}
