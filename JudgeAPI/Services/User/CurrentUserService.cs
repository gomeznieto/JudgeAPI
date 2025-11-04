using AutoMapper;
using Azure.Core;
using JudgeAPI.Data;
using JudgeAPI.Entities;
using JudgeAPI.Models.Auth;
using JudgeAPI.Models.Submission;
using JudgeAPI.Services.Token;
using Microsoft.AspNetCore.Identity;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;

namespace JudgeAPI.Services.User
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _appDbContext;
        private readonly IMapper _mapper;

        public CurrentUserService(
            IHttpContextAccessor httpContextAccessor,
            UserManager<ApplicationUser> userManager,
            AppDbContext appDbContext,
            IMapper mapper
        )
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _appDbContext = appDbContext;
            _mapper = mapper;
        }

        public async Task<TokenResponse> GetCurrentUserAsync()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            
            var userManager = user != null ? await _userManager.GetUserAsync(user) : null;

            if (userManager is null) return null;

            var roles = await _userManager.GetRolesAsync(userManager);
            var submissionList = _appDbContext.Submissions.Where(s => s.UserId == userManager.Id).ToList();

            var tokenResponse = _mapper.Map<TokenResponse>(userManager);
            tokenResponse.Submissions = _mapper.Map<List<SubmissionResponseDTO>>(submissionList); 
            tokenResponse.UserId = userManager.Id!;
            tokenResponse.Roles = roles.ToList();

            return tokenResponse;
        }

        public string? GetCurrentUserId()
        {
            return _httpContextAccessor.HttpContext?.User
                .FindFirstValue(ClaimTypes.NameIdentifier);
        }

        public IList<string> GetCurrentUserRole()
        {
            var user = _httpContextAccessor.HttpContext?.User;

            if (user is null) return [];

            return user.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
        }
    }
}
