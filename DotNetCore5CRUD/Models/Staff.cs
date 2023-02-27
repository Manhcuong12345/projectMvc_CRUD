using System.ComponentModel.DataAnnotations;

namespace DotNetCore5CRUD.Models
{
    public class Staff
    {
        public int Id { get; set; }

        [Required, MaxLength(250)]
        public string FullName { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Job { get; set; }

        [Required]
        public string Email { get; set; }

        [Required, MaxLength(2500)]
        public string Address { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        public byte[] Poster { get; set; }

       
    }
}