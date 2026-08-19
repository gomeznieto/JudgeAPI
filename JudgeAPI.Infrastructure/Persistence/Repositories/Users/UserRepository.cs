using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using JudgeAPI.Application.Features.Users.Interfaces;
using JudgeAPI.Infrastructure.Data;
using JudgeAPI.Infrastructure.Identity;
using JudgeAPI.Application.Features.Users.Dtos;

namespace JudgeAPI.Infrastructure.Persistence.Repositories.Users
{
    public class UserRespository(UserManager<ApplicationUser> userManager,
                                 AppDbContext dbContext) : IUserRepository
    {
        private readonly AppDbContext _dbContext = dbContext;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public async Task<List<UserDTO>> GetUsersPagedAsync(int page = 1, int totalPerPage = 20)
        {
            var baseQuery = _userManager.Users
                .Select(u => new
                {
                    u.Id,
                    u.UserName,
                    u.Email,
                    u.FirstName,
                    u.LastName,
                    u.University,
                    u.IsActive
                });

            int total = await baseQuery.CountAsync();

            var users = await baseQuery
                .Skip((page - 1) * totalPerPage)
                .Take(totalPerPage)
                .ToListAsync();

            List<string> usersId = [.. users.Select(u => u.Id.ToString())];

            var roles = await (from ur in _dbContext.UserRoles
                               join r in _dbContext.Roles on ur.RoleId equals r.Id
                               where usersId.Contains(ur.UserId.ToString())
                               select new { ur.UserId, r.Name }
                    )
                .ToListAsync();

            List<UserDTO> result = [.. users.Select(u => new UserDTO
            {
                Id = u.Id.ToString(),
                UserName = u.UserName!,
                Email = u.Email,
                FirstName = u.FirstName,
                LastName = u.LastName,
                University = u.University,
                IsActive = u.IsActive,
                Roles = [.. roles.Where(r => r.UserId == u.Id)
                    .Select(r => r.Name!)]
            })];

            return result;
        }

        public async Task<int> GetTotalUsersCountAsync()
        {
            return await _userManager.Users
                .Select(static u => new
                {
                    u.Id,
                    u.UserName,
                    u.FirstName,
                    u.LastName,
                    u.IsActive
                })
            .CountAsync();
        }
    }
}
