using AutoMapper;
using JudgeAPI.Application.Features.Auth.Dtos;
using JudgeAPI.Application.Features.Problems.Dtos;
using JudgeAPI.Application.Features.Submissions.Dtos;
using JudgeAPI.Application.Features.TestCases.Dtos;
using JudgeAPI.Application.Features.Units.Dtos;
using JudgeAPI.Application.Features.Users.Dtos;
using JudgeAPI.Domain.Entities;
using JudgeAPI.Infrastructure.Identity;
using JudgeAPI.Models.Unit;

namespace JudgeAPI.API.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Unit
            _ = CreateMap<Unit, UnitResponseDTO>();
            _ = CreateMap<UnitCreateDTO, Unit>();
            _ = CreateMap<UnitUpdateDTO, Unit>();
            _ = CreateMap<Unit, UnitWithProblemsDTO>();

            // Problems
            _ = CreateMap<Problem, ProblemResponseDTO>();
            _ = CreateMap<ProblemCreateDTO, Problem>();
            _ = CreateMap<ProblemUpdateDTO, Problem>();

            // Users
            _ = CreateMap<UserDTO, ApplicationUser>();
            _ = CreateMap<UserCreateDTO, ApplicationUser>();
            _ = CreateMap<UserPrivateDTO, ApplicationUser>();
            _ = CreateMap<UserUpdateDTO, UserDTO>()
                .ForMember(static dest => dest.Email, static opt => opt.Condition(static src => src.Email != null))
                .ForMember(static dest => dest.FirstName, static opt => opt.Condition(static src => src.FirstName != null))
                .ForMember(static dest => dest.LastName, static opt => opt.Condition(static src => src.LastName != null))
                .ForMember(static dest => dest.University, static opt => opt.Condition(static src => src.University != null));

            _ = CreateMap<UserUpdateDTO, UserPrivateDTO>()
                .ForMember(static dest => dest.Email, static opt => opt.Condition(static src => src.Email != null))
                .ForMember(static dest => dest.FirstName, static opt => opt.Condition(static src => src.FirstName != null))
                .ForMember(static dest => dest.LastName, static opt => opt.Condition(static src => src.LastName != null))
                .ForMember(static dest => dest.University, static opt => opt.Condition(static src => src.University != null));

            _ = CreateMap<UserDTO, UserPrivateDTO>();
            _ = CreateMap<ApplicationUser, UserPublicDTO>();
            _ = CreateMap<ApplicationUser, UserAdminDTO>();

            _ = CreateMap<ApplicationUser, TokenResponseDTO>() //TODO: Borrar cuando confirmemos que ya no se utiliza
                .ForMember(static dest => dest.Email, static opt => opt.Condition(static src => src.Email != null))
                .ForMember(static dest => dest.FirstName, static opt => opt.Condition(static src => src.FirstName != null))
                .ForMember(static dest => dest.LastName, static opt => opt.Condition(static src => src.LastName != null))
                .ForMember(static dest => dest.University, static opt => opt.Condition(static src => src.University != null));

            _ = CreateMap<UserDTO, TokenResponseDTO>()
                .ForMember(static dest => dest.Email, static opt => opt.Condition(static src => src.Email != null))
                .ForMember(static dest => dest.FirstName, static opt => opt.Condition(static src => src.FirstName != null))
                .ForMember(static dest => dest.LastName, static opt => opt.Condition(static src => src.LastName != null))
                .ForMember(static dest => dest.University, static opt => opt.Condition(static src => src.University != null));

            // Submissions
            _ = CreateMap<SubmissionCreateDTO, Submission>();
            _ = CreateMap<Submission, SubmissionResponseDTO>();

            _ = CreateMap<SubmissionResult, SubmissionResultResponseDTO>()
                .ForMember(static dest => dest.Input, static opt => opt.MapFrom(static src => src.TestCase!.InputData))
                .ForMember(static dest => dest.ExpectedOutput, static opt => opt.MapFrom(static src => src.TestCase!.ExpectedOutput));

            // Test Case
            _ = CreateMap<TestCaseCreateDTO, TestCase>();
            _ = CreateMap<TestCase, TestCaseResponseDTO>();

            _ = CreateMap<TestCaseUpdateDTO, TestCase>()
                .ForMember(static dest => dest.Id, static opt => opt.Ignore())
                .ForMember(static dest => dest.ProblemId, static opt => opt.Ignore());
        }
    }
}
