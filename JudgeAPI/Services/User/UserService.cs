using AutoMapper;
using JudgeAPI.Constants;
using JudgeAPI.Data;
using JudgeAPI.Entities;
using JudgeAPI.Excerptions;
using JudgeAPI.Models.Submission;
using JudgeAPI.Models.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace JudgeAPI.Services.User
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

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
        }

        // GET BY ID
        public async Task<UserBaseDTO> GetUserByIdAsync(string id)
        {
            // Búsqueda del user según el ID pasado
            var user = await _userManager.FindByIdAsync(id);

            if (user is null) throw new NotFoundException($"El usuario no encontrado");

            var roles = await _userManager.GetRolesAsync(user);
            // --------------------------------

            // Buscamos al usurio que realiza la consula y está regisrado con token
            var currentUserId = _currentUserService.GetCurrentUserId();
            var currentUserRoles = _currentUserService.GetCurrentUserRole();
            // --------------------------------

            // Si un usuario No admin busca el profile de un usuaior admin
            if (roles.Contains(Roles.Admin) && !currentUserRoles.Contains(Roles.Admin))
                throw new ForbiddenException($"No tiene permisos para acceder a este profile");

            // Bsucamos submission Result del usuario a buscar
            var submissionUser = await _dbContext.Submissions.Where(x => x.UserId == id).ToListAsync();
            var submissionResponseDTO = _mapper.Map<List<SubmissionResponseDTO>>(submissionUser);

            // Si el usuario mira su propio profile
            if (user.Id == currentUserId)
            {
                var privateUserResponse = _mapper.Map<UserPrivateDTO>(user);
                privateUserResponse.Submissons = submissionResponseDTO;
                return privateUserResponse;
            }

            // Si el usuario actual tiene permisos de admin
            if (currentUserRoles.Contains(Roles.Admin))
            {
                if (user.Id != currentUserId)
                    throw new ForbiddenException("No tiene permisos para acceder a este profile");

                var adminUserResponse = _mapper.Map<UserAdminDTO>(user);
                adminUserResponse.Submissons = submissionResponseDTO;
                return adminUserResponse;
            }

            // Si un usuario Admin o no admin, busca el profile de otro usuario
            var publicUserResponse = _mapper.Map<UserPublicDTO>(user);
            publicUserResponse.Submissons = submissionResponseDTO;
            return publicUserResponse;
        }

        // UPDATE
        public async Task<UserPrivateDTO> UpdateUser(UserUpdateDTO userUpdate)
        {
            // Usuario acutal
            var currentUserId = _currentUserService.GetCurrentUserId();

            if (currentUserId != userUpdate.Id)
                throw new ForbiddenException($"No tiene permiso para realizar modficiaciones en el usuario {userUpdate.Id}");

            // Obtenemos el usuario solo si sabemos que se trata del mismo
            var currentUser = await _currentUserService.GetCurrentUserAsync();

            if (currentUser is null) throw new NotFoundException($"El usuario no encontrado");

            _mapper.Map(userUpdate, currentUser);

            await _dbContext.SaveChangesAsync();

            return _mapper.Map<UserPrivateDTO>(currentUser);

        }

        // UPDATE PASSWORD
        public async Task<IdentityResult> ChangePasswordAsync(ChangePasswordDTO changePasswordDTO)
        {
            var user = await _currentUserService.GetCurrentUserAsync();

            if (user is null) return IdentityResult.Failed(new IdentityError {Description = "Usuario no encontrado" });

            return await _userManager.ChangePasswordAsync(user, changePasswordDTO.OldPassword, changePasswordDTO.NewPassword);
        }
    }
}
