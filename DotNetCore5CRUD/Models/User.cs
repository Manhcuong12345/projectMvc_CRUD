using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace DotNetCore5CRUD.Models
{
    public class User { 
       
        public string Id { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [Phone]
        [Display(Name = "Số điện thoại")]
        public string PhoneNumber { get; set; }

        [Required]
        [DataType(DataType.Password)]
        // [Display(Name = "Mật khẩu")]
        public string Password { get; set; }

    }
}
