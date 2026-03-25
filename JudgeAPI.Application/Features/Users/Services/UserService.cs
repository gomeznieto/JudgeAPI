using JudgeAPI.Application.Common;
using JudgeAPI.Domain;
using AutoMapper;
using JudgeAPI.Application.Features.Submissions.Dtos;
using JudgeAPI.Application.Features.Users.Dtos;
using JudgeAPI.Application.Features.Submissions.Interfaces;
using JudgeAPI.Application.Features.Users.Interfaces;
using JudgeAPI.Application.Common.Interfaces;
using JudgeAPI.Domain.Entities;

namespace JudgeAPI.Application.Features.Users.Services
{
    public class UserService(
            ISubmissionRepository submissionRepository,
            IIdentityService identityService,
            IUserRepository userService,
            IMapper mapper
            )
        : IUserService
    {
        private readonly IMapper _mapper = mapper;
        private readonly IIdentityService _identityService = identityService;
        private readonly ISubmissionRepository _submissionRepository = submissionRepository;
        private readonly IUserRepository _userRepository = userService;

        // ---- GET BY ID ---- //
        public async Task<UserBaseDTO> GetUserByIdAsync(string id, string currentId)
        {
            // -- Busca Usuario que realiza la consulta por Id 
            UserDTO? currentUser = await _identityService.FindByIdAsync(currentId);
            UserDTO? searchUser = await _identityService.FindByIdAsync(id);

            if (currentUser is null || searchUser is null)
            {
                throw new NotFoundException($"El usuario no encontrado");
            }

            // -- Roles del usuario que realiza la consulta
            IList<string> currentUserRoles = await _identityService.GetRoleAsync(currentUser) ?? [];

            // -- Roles del usuario buscado
            IList<string> searchUserRoles = await _identityService.GetRoleAsync(searchUser) ?? [];

            // --------------------------------

            // Si un usuario No admin busca el profile de un usuario admin
            if (searchUserRoles.Contains(Roles.Admin) && !currentUserRoles.Contains(Roles.Admin))
            {
                throw new ForbiddenException($"No tiene permisos para acceder a este profile");
            }

            // Buscamos submission Result del usuario a buscar
            List<Submission> submissionUser = await _submissionRepository.GetAllByUserIdAsync(id);
            List<SubmissionResponseDTO> submissionResponseDTO = _mapper.Map<List<SubmissionResponseDTO>>(submissionUser);

            // Si el usuario mira su propio profile
            if (currentUser.Id == id)
            {
                UserPrivateDTO privateUserResponse = _mapper.Map<UserPrivateDTO>(currentUser);
                privateUserResponse.Submissions = submissionResponseDTO;
                return privateUserResponse;
            }

            // Si el usuario actual tiene permisos de admin
            if (currentUserRoles.Contains(Roles.Admin))
            {
                // Evitamos que otro Admin pueda ver el perfil
                if (currentUser.Id != id)
                {
                    throw new ForbiddenException("No tiene permisos para acceder a este profile");
                }

                UserAdminDTO adminUserResponse = _mapper.Map<UserAdminDTO>(currentUser);
                adminUserResponse.Submissons = submissionResponseDTO;
                return adminUserResponse;
            }

            // Si un usuario Admin o no admin, busca el profile de otro usuario
            UserPublicDTO publicUserResponse = _mapper.Map<UserPublicDTO>(currentUser);
            publicUserResponse.Submissons = submissionResponseDTO;
            return publicUserResponse;
        }

        // -- RETORNAR USUARIO ACTUAL LOGEADO -- //
        public async Task<UserPrivateDTO> GetCurrectUser(string id)
        {
            UserDTO? currentUser = await _identityService.FindByIdAsync(id) ?? throw new KeyNotFoundException("El usuario que busca no existe");

            IList<string> roles = await _identityService.GetRoleAsync(currentUser) ?? [];

            List<Submission> submissionList = await _submissionRepository.GetAllByUserIdAsync(id);

            // Armamos respuesta
            UserPrivateDTO userResponse = _mapper.Map<UserPrivateDTO>(currentUser);

            userResponse.Submissions = _mapper.Map<List<SubmissionResponseDTO>>(submissionList);

            userResponse.UserId = id;
            userResponse.Roles = [.. roles];

            return userResponse;
        }

        // ---- UPDATE ---- //
        public async Task<UserPrivateDTO> UpdateUser(UserUpdateDTO userUpdate, string currentId)
        {
            // Traemos los datos completos almacenados del usuario 
            UserDTO? currentIdentityUser = await _identityService.FindByIdAsync(currentId) ?? throw new NotFoundException($"El usurio no encontrado");

            // Mapeamos los datos almacenados con los actuales.
            _ = _mapper.Map(userUpdate, currentIdentityUser);

            // Guardamos el usuario
            _ = await _identityService.UpdateUserAsync(userUpdate, currentId);


            // Armamos la respuesta
            IList<string> roles = await _identityService.GetRoleAsync(currentIdentityUser) ?? [];

            List<Submission> submissionList = await _submissionRepository.GetAllByUserIdAsync(currentId);

            UserPrivateDTO updatedUser = _mapper.Map<UserPrivateDTO>(currentIdentityUser);

            updatedUser.Submissions = _mapper.Map<List<SubmissionResponseDTO>>(submissionList);

            updatedUser.UserId = currentIdentityUser.Id;
            updatedUser.Roles = [.. roles];

            return updatedUser;

        }

        // ---- UPDATE ROLES ---- //
        public async Task<UserPublicDTO> UpdateUserRoles(UserUpdateRolesDTO userUpdateRoles)
        {
            UserDTO user = await _identityService.FindByIdAsync(userUpdateRoles.Id) ?? throw new NotFoundException("Usuario no encontrado");

            IList<string> currentUserRoles = await _identityService.GetRoleAsync(user) ?? [];

            // Agregamos roles nuevos
            foreach (string rol in userUpdateRoles.Roles.Distinct())
            {
                if (!await _identityService.RoleExistsAsync(rol))
                {
                    throw new NotFoundException($"Rol inválido: {rol}");
                }

                if (!await _identityService.IsInRolAsync(user, rol))
                {
                    await _identityService.AddRoleAsync(user, rol);
                }
            }

            // Eliminamos los roles que no están
            foreach (string currentRoles in currentUserRoles)
            {
                if (!userUpdateRoles.Roles.Contains(currentRoles))
                {
                    _ = await _identityService.RemoveFromRoleAsync(user, currentRoles);
                }
            }

            IList<string> updatedUserRoles = await _identityService.GetRoleAsync(user) ?? [];

            return new UserPublicDTO
            {
                UserId = user.Id,
                UserName = user.UserName,
                Roles = [.. updatedUserRoles]
            };

        }

        // ---- UPDATE PASSWORD ---- //
        public async Task<UserDTO> ChangePasswordAsync(ChangePasswordDTO dto, string userId)
        {
            UserDTO? user = await _identityService.FindByIdAsync(userId) ?? throw new KeyNotFoundException("El usuario que está bsucando no existe");

            _ = await _identityService.ChangePasswordAsync(user.Id, dto);

            return user;
        }

        // ---- GET USERS ---- //
        public async Task<UsersResponseDTO> GetUsersAsync(int page = 1, int totalPerPage = 20)
        {
            List<UserDTO> users = await _userRepository.GetUsersPagedAsync(page, totalPerPage);
            int total = await _userRepository.GetTotalUsersCountAsync();
            int totalPages = (int)Math.Ceiling((double)total / totalPerPage);

            return new UsersResponseDTO
            {
                Page = page,
                TotalPages = totalPages,
                TotalPerPage = totalPerPage,
                TotalAmount = total,
                Users = users
            };
        }

        // ---- GET ROLES ---- //
        public async Task<RolesResponseDTO> GetRolesAsync()
        {
            IList<string> roles = await _identityService.GetAllRolesAsync();

            return new RolesResponseDTO
            {
                Roles = [.. roles.Select(static r => new RoleDTO { Name = r })]
            };
        }

        public Task<IdentityResultDTO> ChangePasswordAsync(ChangePasswordDTO changePasswordDTO)
        {
            throw new NotImplementedException();
        }

        public Task<UserPrivateDTO> GetCurrectUser()
        {
            throw new NotImplementedException();
        }

        public Task<UserPrivateDTO> UpdateUser(UserUpdateDTO userUpdate)
        {
            throw new NotImplementedException();
        }
    }

}
