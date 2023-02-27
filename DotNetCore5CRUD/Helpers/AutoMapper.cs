using DotNetCore5CRUD.Dto;
using AutoMapper;
using DotNetCore5CRUD.Models;

namespace DotNetCore5CRUD.Helpers
{
    public class AutoMapper : Profile
    {
        public AutoMapper()
        {
            CreateMap<Staff, StaffDto>().ReverseMap();
        }
    }
}
