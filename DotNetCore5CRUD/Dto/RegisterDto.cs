using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace DotNetCore5CRUD.Dto
{
    public class RegisterDto
    { 
        public string Id { get; set; }

        [Required]
        [EmailAddress]
       // [Display(Name = "Email")]
        [StringLength(50, ErrorMessage = "Email không hợp lệ.")]
        public string Email { get; set; }

        [Required]
        [Phone]
        [StringLength(50, ErrorMessage = "Số điện thoại không hợp lệ.")]
        public string PhoneNumber { get; set; }

        [Required]
        [DataType(DataType.Password)]
       // [Display(Name = "Mật khẩu")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Mật khẩu và mật khẩu xác nhận không trùng khớp!")]
        public string ConfirmPassword { get; set; }
    }
}
