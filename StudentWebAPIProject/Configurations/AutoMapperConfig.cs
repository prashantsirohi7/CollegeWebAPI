using AutoMapper;
using StudentWebAPIProject.Models;

namespace StudentWebAPIProject.Configurations
{
    public class AutoMapperConfig : Profile
    {
        public AutoMapperConfig() 
        {
            CreateMap<StudentDTO, Student>().ReverseMap();

            //Config for different property names
            CreateMap<StudentDTO, Student>().ForMember(n => n.Name, opt => opt.MapFrom(x => x.name)).ReverseMap();

            //Config for ignoring property names
            //CreateMap<StudentDTO, DBSets.Student>().ReverseMap().ForMember(n => n.name, opt => opt.Ignore());

            //Config for transforming property names
            //CreateMap<StudentDTO, DBSets.Student>().ReverseMap().AddTransform<string>(x => string.IsNullOrWhiteSpace(x) ? "null value" : x);

            CreateMap<RoleDTO, Role>().ReverseMap();
            CreateMap<RolePrivilegeDTO, RolePrivilege>().ReverseMap();
        }
    }
}
