using AutoMapper;
using JudgeAPI.Constants;
using JudgeAPI.Data;
using JudgeAPI.Entities;
using JudgeAPI.Excerptions;
using JudgeAPI.Models.Submission;
using JudgeAPI.Models.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
namespace JudgeAPI.Services.User
{
    public class UserService : IUserService
    {
      private readonly UserManager<ApplicationUser> _userManager;
      private readonly AppDbContext _dbContext;
      private readonly ICurrentUserService _currentUserService;
      private readonly IMapper _mapper;
      private readonly RoleManager<IdentityRole> _roleManager;
      public UserService(
          UserManager<ApplicationUser> userManager,
          RoleManager<IdentityRole> roleManager,
          AppDbContext dbContext,
          ICurrentUserService currentUserService,
          IMapper mapper
          )
      {
        _userManager = userManager;
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _mapper = mapper;
        _roleManager = roleManager;
      }

      // ---- GET BY ID ---- //
      public async Task<UserBaseDTO> GetUserByIdAsync(string id)
      {
        // -- Busca Usuario por Id 
        var currentUser = await _currentUserService.GetUserByIdAsync(id);

        if (currentUser is null) throw new NotFoundException($"El usuario no encontrado");

        var roles = await _currentUserService.GetUserRolesByIdAsync(currentUser);

        // -- Usuario que realiza la consulta
        var currentUserId = _currentUserService.GetCurrentUserId();
        var currentUserRoles = _currentUserService.GetCurrentUserRole();
        // --------------------------------

        // Si un usuario No admin busca el profile de un usuario admin
        if (roles.Contains(Roles.Admin) && !currentUserRoles.Contains(Roles.Admin))
          throw new ForbiddenException($"No tiene permisos para acceder a este profile");

        // Buscamos submission Result del usuario a buscar
        var submissionUser = await _dbContext.Submissions.Where(x => x.UserId == id).ToListAsync();
        var submissionResponseDTO = _mapper.Map<List<SubmissionResponseDTO>>(submissionUser);

        // Si el usuario mira su propio profile
        if (currentUser.Id == currentUserId)
        {
          var privateUserResponse = _mapper.Map<UserPrivateDTO>(currentUser);
          privateUserResponse.Submissions = submissionResponseDTO;
          return privateUserResponse;
        }

        // Si el usuario actual tiene permisos de admin
        if (currentUserRoles.Contains(Roles.Admin))
        {
          if (currentUser.Id != currentUserId)
            throw new ForbiddenException("No tiene permisos para acceder a este profile");

          var adminUserResponse = _mapper.Map<UserAdminDTO>(currentUser);
          adminUserResponse.Submissons = submissionResponseDTO;
          return adminUserResponse;
        }

        // Si un usuario Admin o no admin, busca el profile de otro usuario
        var publicUserResponse = _mapper.Map<UserPublicDTO>(currentUser);
        publicUserResponse.Submissons = submissionResponseDTO;
        return publicUserResponse;
      }

      // -- RETORNAR USUARIO ACTUAL LOGEADO -- //
      public async Task<UserPrivateDTO> GetCurrectUser()
      {
        var currentUser = await _currentUserService.GetCurrentUserAsync(); 
        var roles = _currentUserService.GetCurrentUserRole();            
        var submissionList = _dbContext.Submissions.Where(s => s.UserId == currentUser.Id).ToList();

        // Armamos respuesta
        var userResponse = _mapper.Map<UserPrivateDTO>(currentUser);

        userResponse.Submissions = _mapper.Map<List<SubmissionResponseDTO>>(submissionList); 

        userResponse.UserId = currentUser.Id!;
        userResponse.Roles = roles.ToList();

        return userResponse; 
      }

      // ---- UPDATE ---- //
      public async Task<UserPrivateDTO> UpdateUser(UserUpdateDTO userUpdate)
      {
        // Traemos Id del usuario actual
        var currentUserId = _currentUserService.GetCurrentUserId();

        // Verificamos que los Id del actual y el usuario a actualizar sean los mismos
        if (currentUserId != userUpdate.Id)
          throw new ForbiddenException($"No tiene permiso para realizar modficiaciones en el usuario {userUpdate.Id}");

        // Traemos los datos completos almacenados del usuario 
        var currentUser = await _currentUserService.GetCurrentUserAsync();

        if (currentUser is null) throw new NotFoundException($"El usurio no encontrado");

        // Mapeamos los datos almacenados con los actuales.
        _mapper.Map(userUpdate, currentUser);

        // Guardamos el usuario
        _dbContext.Users.Update(_mapper.Map<ApplicationUser>(currentUser));
        await _dbContext.SaveChangesAsync();

        // Armamos la respuesta
        var roles = _currentUserService.GetCurrentUserRole();
        var submissionList = _dbContext.Submissions.Where(s => s.UserId == currentUser.Id).ToList();

        var updatedUser =  _mapper.Map<UserPrivateDTO>(currentUser);

        updatedUser.Submissions = _mapper.Map<List<SubmissionResponseDTO>>(submissionList); 

        updatedUser.UserId = currentUser.Id!;
        updatedUser.Roles = roles.ToList();

        return updatedUser;

      }

      // ---- UPDATE ROLES ---- //
      public async Task<UserPublicDTO> UpdateUserRoles(UserUpdateRolesDTO userUpdateRoles){
        var user = await _userManager.FindByIdAsync(userUpdateRoles.Id);

        if(user == null) throw new NotFoundException("Usuario no encontrado");

        var currentUserRoles = await _userManager.GetRolesAsync(user);

        // Agregamos roles nuevos
        foreach(var rol in userUpdateRoles.Roles.Distinct()){
          if(!await _roleManager.RoleExistsAsync(rol)) throw new NotFoundException($"Rol inválido: {rol}");

          if(!await _userManager.IsInRoleAsync(user, rol)) await _userManager.AddToRoleAsync(user, rol);
        }

        // Eliminamos los roles que no están
        foreach(var currentRoles in currentUserRoles){
          if(!userUpdateRoles.Roles.Contains(currentRoles)){
            await _userManager.RemoveFromRoleAsync(user, currentRoles);
          }
        }
        var updatedUserRoles = await _userManager.GetRolesAsync(user);

        var userResponse = new UserPublicDTO {
          UserId = user.Id,
          UserName = user.UserName!,
          Roles = updatedUserRoles.ToList()
        };

        return userResponse;
      }

      // ---- UPDATE PASSWORD ---- //
      public async Task<IdentityResult> ChangePasswordAsync(ChangePasswordDTO changePasswordDTO)
      {
        // Obtenemos el usuario del token
        var user = await _currentUserService.GetCurrentUserAsync();

        if (user is null) return IdentityResult.Failed(new IdentityError {Description = "Usuario no encontrado" });

        var userApplication = _mapper.Map<ApplicationUser>(user);

        return await _userManager.ChangePasswordAsync(userApplication, changePasswordDTO.OldPassword, changePasswordDTO.NewPassword);
      }

      // ---- GET USERS ---- //
      public async Task<UsersResponseDTO> GetUsersAsync(int page = 1, int totalPerPage = 20)
      {
        var baseQuery = _userManager.Users
          .Select(u => new {
              Id = u.Id!,
              UserName = u.UserName!,
              FirstName = u.FirstName,
              LastName = u.LastName,
              IsActive = u.IsActive
              });

        var total = await baseQuery.CountAsync();

        var users = await baseQuery
          .Skip((page - 1) * totalPerPage)
          .Take(totalPerPage)
          .ToListAsync();

        var usersId = users.Select(u => u.Id).ToList();

        var roles = await (from ur in _dbContext.UserRoles
            join r in _dbContext.Roles on ur.RoleId equals r.Id
            where usersId.Contains(ur.UserId)
            select new {ur.UserId, r.Name}
            ).ToListAsync();

        var result = users.Select(u =>  new UserDTO
            {
            Id = u.Id,
            UserName = u.UserName,
            FirstName = u.FirstName,
            LastName = u.LastName,
            IsActive = u.IsActive,
            Roles = roles.Where(r => r.UserId == u.Id)
            .Select(r => r.Name!)
            .ToList()
            }).ToList();

        var totalPages = (int)Math.Ceiling((double)total / totalPerPage);

        return new UsersResponseDTO
        {     
          Page = page,
          TotalPages = totalPages,
          TotalPerPage = totalPerPage,
          TotalAmount = total, 
          Users = result
        };
      }

      // ---- GET ROLES ---- //
      public async Task <RolesResponseDTO> GetRolesAsync(){
        var roles = await _roleManager.Roles.Select( r => new RoleDTO{ Name = r.Name!}).ToListAsync();

        return new RolesResponseDTO{
          Roles = roles
        };
      }
    }
}
