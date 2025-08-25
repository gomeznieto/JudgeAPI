using AutoMapper;
using JudgeAPI.Entities;
using JudgeAPI.Models;

namespace JudgeAPI.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Unit, UnitResponseDTO>();
            CreateMap<UnitCreateDTO, Unit>();
            CreateMap<UnitUpdateDTO, Unit>();
            CreateMap<Unit, UnitWithProblemsDTO>();
            CreateMap<Problem, ProblemResponseDTO>();
            CreateMap<ProblemCreateDTO, Problem>();
            CreateMap<ProblemUpdateDTO, Problem>();
            CreateMap<UserCreateDTO, ApplicationUser>();
            CreateMap<ApplicationUser, UserResponseDTO>();
        }
    }
}
