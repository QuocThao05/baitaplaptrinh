using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace database.Models.ViewModel
{
    public class RegisterVM
    {
        [Required]
        [Display(Name = "Tên Đăng Nhập")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Mật Khẩu")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Xác Nhận Mật Khẩu")]
        [Compare("Password", ErrorMessage = "Mật khẩu không khớp.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "Họ Tên")]
        public string CustomerName { get; set; }

        [Required]
        [Display(Name = "Số Điện Thoại")]
        [DataType(DataType.PhoneNumber)]
        public string CustomerPhone { get; set; }

        [Required]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        public string CustomerEmail { get; set; }

        [Required]
        [Display(Name = "Địa Chỉ")]
        public string CustomerAddress { get; set; }

    }
}