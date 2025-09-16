using JudgeAPI.Entities;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace JudgeAPI.Services.User
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;

        public CurrentUserService(
            IHttpContextAccessor httpContextAccessor,
            UserManager<ApplicationUser> userManager
        )
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        public async Task<ApplicationUser?> GetCurrentUserAsync()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            return user != null ? await _userManager.GetUserAsync(user) : null;
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
