using AutoMapper;
using JudgeAPI.Data;
using JudgeAPI.Entities;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

/*
   Se consulta únicamente UserManager y se trabaja con ApplicationUser. El resto de mapping los realiza el servicio del usuario.
   */

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
    
    // -- RETORNAR APPLICATION USER -- //
    public async Task<ApplicationUser> GetCurrentUserAsync()
    {
      var user = _httpContextAccessor.HttpContext?.User;

      var userManager = user != null ? await _userManager.GetUserAsync(user) : null;

      if (userManager is null) return null;

      return userManager;
    }

    // -- RETORNA ID STRING DE USER -- //
    public string? GetCurrentUserId()
    {
      return _httpContextAccessor.HttpContext?.User
        .FindFirstValue(ClaimTypes.NameIdentifier);
    }

    // -- RETORNAR LISTADO DE ROLES STRING -- //
    public IList<string> GetCurrentUserRole()
    {
      var user = _httpContextAccessor.HttpContext?.User;

      if (user is null) return [];

      return user.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
    }

    // -- RETORNA USUARIO POR ID -- //
    public async Task<ApplicationUser?> GetUserByIdAsync(string id)
    {
      var user = await _userManager.FindByIdAsync(id);
      return user != null ? user : null;
    }

    // -- RETORNA ROLES POR USUARIO -- //
    public async Task<IList<string>> GetUserRolesByIdAsync(ApplicationUser user)
    {
      var roles = await _userManager.GetRolesAsync(user);
      return roles != null ? roles : [];
    }
  }
}
