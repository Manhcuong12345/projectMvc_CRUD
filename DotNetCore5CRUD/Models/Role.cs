using System.ComponentModel.DataAnnotations;

namespace DotNetCore5CRUD.Models
{
    public class Role
    {
        [Required, StringLength(256)]
        public string Name { get; set; }
    }
}
