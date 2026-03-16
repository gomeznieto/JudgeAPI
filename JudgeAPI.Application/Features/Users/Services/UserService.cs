namespace JudgeAPI.Application.Features;
using JudgeAPI.Application.Common;
using JudgeAPI.Domain;
using AutoMapper;


public class UserService : IUserService
{
    private readonly IMapper _mapper;
    private readonly IIdentityService _identityService;
    private readonly ISubmissionRepository _submissionRepository;

    public UserService(
            ISubmissionRepository submissionRepository,
            IIdentityService identityService,
            IMapper mapper
            )
    {
        _mapper = mapper; }

    // ---- GET BY ID ---- //
    public async Task<UserBaseDTO> GetUserByIdAsync(string id, string currentId)
    {
        // -- Busca Usuario que realiza la consulta por Id 
        var currentUser = await _identityService.FindByIdAsync(currentId);
        var searchUser = await _identityService.FindByIdAsync(id);

        if (currentUser is null || searchUser is null) throw new NotFoundException($"El usuario no encontrado");

        // -- Roles del usuario que realiza la consulta
        var currentUserRoles = await _identityService.GetRoleAsync(currentUser) ?? [];

        // -- Roles del usuario buscado
        var searchUserRoles = await _identityService.GetRoleAsync(searchUser) ?? [];

        // --------------------------------

        // Si un usuario No admin busca el profile de un usuario admin
        if (searchUserRoles.Contains(Roles.Admin) && !currentUserRoles.Contains(Roles.Admin))
            throw new ForbiddenException($"No tiene permisos para acceder a este profile");

        // Buscamos submission Result del usuario a buscar
        var submissionUser = await _submissionRepository.GetAllByUserIdAsync(id);
        var submissionResponseDTO = _mapper.Map<List<SubmissionResponseDTO>>(submissionUser);

        // Si el usuario mira su propio profile
        if (currentUser.Id == id)
        {
            var privateUserResponse = _mapper.Map<UserPrivateDTO>(currentUser);
            privateUserResponse.Submissions = submissionResponseDTO;
            return privateUserResponse;
        }

        // Si el usuario actual tiene permisos de admin
        if (currentUserRoles.Contains(Roles.Admin))
        {
            // Evitamos que otro Admin pueda ver el perfil
            if(currentUser.Id != id)
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
    public async Task<UserPrivateDTO> GetCurrectUser(string id)
    {
        var currentUser = await _identityService.FindByIdAsync(id);
        
        if(currentUser is null) throw new KeyNotFoundException("El usuario que busca no existe");

        var roles = await _identityService.GetRoleAsync(currentUser) ?? [];

        var submissionList = await _submissionRepository.GetAllByUserIdAsync(id);

        // Armamos respuesta
        var userResponse = _mapper.Map<UserPrivateDTO>(currentUser);

        userResponse.Submissions = _mapper.Map<List<SubmissionResponseDTO>>(submissionList); 

        userResponse.UserId = id;
        userResponse.Roles = roles.ToList();

        return userResponse; 
    }

    // ---- UPDATE ---- //
    public async Task<UserPrivateDTO> UpdateUser(UserUpdateDTO userUpdate,  string currentId)
    {
        // Traemos los datos completos almacenados del usuario 
        var currentIdentityUser = await _identityService.FindByIdAsync(currentId);

        if (currentIdentityUser is null)
            throw new NotFoundException($"El usurio no encontrado");

        // Mapeamos los datos almacenados con los actuales.
        _mapper.Map(userUpdate, currentIdentityUser);

        // Guardamos el usuario
        await _identityService.UpdateUserAsync(userUpdate, currentId);


        // Armamos la respuesta
        var roles = await _identityService.GetRoleAsync(currentIdentityUser) ?? [];

        var submissionList = await _submissionRepository.GetAllByUserIdAsync(currentId); 

        var updatedUser =  _mapper.Map<UserPrivateDTO>(currentIdentityUser);

        updatedUser.Submissions = _mapper.Map<List<SubmissionResponseDTO>>(submissionList); 

        updatedUser.UserId = currentIdentityUser.Id!;
        updatedUser.Roles = roles.ToList();

        return updatedUser;

    }

    // ---- UPDATE ROLES ---- //
    public async Task<UserPublicDTO> UpdateUserRoles(UserUpdateRolesDTO userUpdateRoles){
        var user = await _identityService.FindByIdAsync(userUpdateRoles.Id);

        if(user == null) throw new NotFoundException("Usuario no encontrado");

        var currentUserRoles = await _identityService.GetRoleAsync(user) ?? [];

        // Agregamos roles nuevos
        foreach(var rol in userUpdateRoles.Roles.Distinct()){
            if(!await _identityService.RoleExistsAsync(rol)) throw new NotFoundException($"Rol inválido: {rol}");
            if(!await _identityService.IsInRolAsync(user, rol)) await _identityService.AddRoleAsync(user, rol);
        }

        // Eliminamos los roles que no están
        foreach(var currentRoles in currentUserRoles){
            if(!userUpdateRoles.Roles.Contains(currentRoles)){
                await _identityService.RemoveFromRoleAsync(user, currentRoles);
            }
        }
        var updatedUserRoles = await _identityService.GetRoleAsync(user);

        var userResponse = new UserPublicDTO {
            UserId = user.Id,
            UserName = user.UserName!,
            Roles = updatedUserRoles!.ToList() ?? []
        };

        return userResponse;
    }

    // ---- UPDATE PASSWORD ---- //
    public async Task<UserDTO> ChangePasswordAsync(ChangePasswordDTO dto, string userId)
    {
        var user = await _identityService.FindByIdAsync(userId);
        if (user is null) throw new KeyNotFoundException ("El usuario que está bsucando no existe");
        await _identityService.ChangePasswordAsync(user.Id, dto);
        return user;
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
        var roles = await _identityService.GetAllRolesAsync();

        return new RolesResponseDTO{
            Roles = roles.Select(r => new RoleDTO{ Name = r}).ToList()
        };
    }
}

