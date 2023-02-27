using System.ComponentModel.DataAnnotations;

namespace DotNetCore5CRUD.Dto
{
    public class StaffDto
    {
        public int Id { get; set; }

        [Required, MaxLength(250)]
        public string FullName { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        public byte[] Poster { get; set; }
    }
}
