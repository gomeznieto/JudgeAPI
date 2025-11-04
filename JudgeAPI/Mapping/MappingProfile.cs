using AutoMapper;
using JudgeAPI.Entities;
using JudgeAPI.Models.Auth;
using JudgeAPI.Models.Problem;
using JudgeAPI.Models.Submission;
using JudgeAPI.Models.TestCase;
using JudgeAPI.Models.Unit;
using JudgeAPI.Models.User;
using Microsoft.AspNetCore.SignalR;
using StackExchange.Redis;

namespace JudgeAPI.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {

            // Unit
            CreateMap<Unit, UnitResponseDTO>();
            CreateMap<UnitCreateDTO, Unit>();
            CreateMap<UnitUpdateDTO, Unit>();
            CreateMap<Unit, UnitWithProblemsDTO>();

            // Problems
            CreateMap<Problem, ProblemResponseDTO>();
            CreateMap<ProblemCreateDTO, Problem>();
            CreateMap<ProblemUpdateDTO, Problem>();

            // Users
            CreateMap<UserCreateDTO, ApplicationUser>();
            CreateMap<TokenResponse, TokenResponse>();
            CreateMap<UserUpdateDTO, ApplicationUser>()
                .ForMember(dest => dest.Email, opt => opt.Condition(src => src.Email != null))
                .ForMember(dest => dest.FirstName, opt => opt.Condition(src => src.FirstName != null))
                .ForMember(dest => dest.LastName, opt => opt.Condition(src => src.LastName != null))
                .ForMember(dest => dest.University, opt => opt.Condition(src => src.University != null))
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());

            CreateMap<ApplicationUser, UserPrivateDTO>();
            CreateMap<ApplicationUser, UserPublicDTO>();
            CreateMap<ApplicationUser, UserAdminDTO>();

            CreateMap<ApplicationUser, TokenResponse>()
                .ForMember(dest => dest.Email, opt => opt.Condition(src => src.Email != null))
                .ForMember(dest => dest.FirstName, opt => opt.Condition(src => src.FirstName != null))
                .ForMember(dest => dest.LastName, opt => opt.Condition(src => src.LastName != null))
                .ForMember(dest => dest.University, opt => opt.Condition(src => src.University != null));

            // Submissions
            CreateMap<SubmissionCreateDTO, Submission>();
            CreateMap<Submission, SubmissionResponseDTO>();
            CreateMap<Submission, SubmissionResponseWithResultDTO>();

            CreateMap<SubmissionResult, SubmissionResultResponseDTO>()
                .ForMember(dest => dest.Input, opt => opt.MapFrom(src => src.TestCase!.InputData))
                .ForMember(dest => dest.ExpectedOutput, opt => opt.MapFrom(src => src.TestCase!.ExpectedOutput));

            // Test Case
            CreateMap<TestCaseCreateDTO, TestCase>();
            CreateMap<TestCase,  TestCaseResponseDTO>();

            CreateMap<TestCaseUpdateDTO, TestCase>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ProblemId, opt => opt.Ignore());
        }
    }
}
