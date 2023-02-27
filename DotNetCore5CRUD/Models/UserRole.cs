using DotNetCore5CRUD.Dto;
using System.Collections.Generic;

namespace DotNetCore5CRUD.Models
{
    public class UserRole
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public List<CheckBoxDto> Roles { get; set; }
    }
}
