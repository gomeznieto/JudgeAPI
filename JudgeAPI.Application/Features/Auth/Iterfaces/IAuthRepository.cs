using JudgeAPI.Application.Common.Interfaces;
using JudgeAPI.Application.Features.Users.Dtos;

namespace JudgeAPI.Application.Features.Auth.Iterfaces
{
    public interface IAuthRepository : IRepository<UserDTO>
    {

    }
}
