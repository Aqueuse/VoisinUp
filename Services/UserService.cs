using VoisinUp.Models;
using VoisinUp.Repositories;

namespace VoisinUp.Services;

public class UserService {
    private readonly UserRepository _userRepository;
    private readonly VoisinageRepository _voisinageRepository;
    private readonly AuthentificationService _authentificationService;

    public UserService(UserRepository userRepository, AuthentificationService authentificationService, VoisinageRepository voisinageRepository) {
        _userRepository = userRepository;
        _voisinageRepository = voisinageRepository;
        
        _authentificationService = authentificationService;
    }
    
    public async Task<User?> GetUserByIdAsync(string userId) {
        var user = await _userRepository.GetUserByIdAsync(userId);
        return user;
    }

    public async Task<ServiceResult> GetUserProfileByIdAsync(string userId) {
        var user = await _userRepository.GetUserByIdAsync(userId);
        if (user == null) return new ServiceResult { StatusCode = 404 };
        
        var userProfile = new UserProfile {
            Name = user.Name,
            VoisinageId = user.VoisinageId,
            AvatarUrl = user.AvatarUrl,
            Bio = user.Bio,
            BricksQuantity = user.BricksQuantity,
            CakesQuantity = user.CakesQuantity,
            Email = user.Email,
            CreationDate = user.CreationDate,
            LastLogin = user.LastLogin,

            UserAssets = user.UserAssets
        };

        return new ServiceResult { StatusCode = 200, Data = userProfile};
    }

    public async Task<UserCard?> GetUserCard(string userId) {
        var user = await _userRepository.GetUserByIdAsync(userId);

        if (user != null)
            return new UserCard {
                Name = user.Name,
                AvatarUrl = user.AvatarUrl,
                Bio = user.Bio,
                LastLogin = user.LastLogin
            };

        Console.WriteLine("[Error] user "+ userId +"not found");
        return null;
    }
    
    // 🔹 Crée un utilisateur et sa grille 100x100x5
    public async Task<ServiceResult> CreateUserAsync(CreateUser createUser) {
        // check if voisinage exist
        var voisinage = await _voisinageRepository.GetVoisinageByIdAsync(createUser.VoisinageId);
        if (voisinage == null) {
            return new ServiceResult { StatusCode = 404};
        }

        // check if user with email don't already exist, if so, return a specific message to redirect user (vuejs)
        var user = await _userRepository.GetUserByEmailAsync(createUser.Email);

        if (user != null) {
            return new ServiceResult { StatusCode = 409};
        }

        // Hash du mot de passe avant stockage
        string hashedPassword = _authentificationService.GetHashedPassword(createUser.PasswordHash);

        var userToCreate = new User {
            UserId = Guid.NewGuid().ToString(),
            Name = createUser.Name,
            Email = createUser.Email,
            PasswordHash = hashedPassword,
            Country = "",
            Commune = "",
            VoisinageId = createUser.VoisinageId,
            Bio = "",
            AvatarUrl = ""
        };
        
        await _userRepository.CreateUserAsync(userToCreate);
        return new ServiceResult { StatusCode = 200};
    }

    public async Task<ServiceResult> EditUserAsync(string userId, string name, string? bio, string? avatarURL) {
        var user = await _userRepository.GetUserByIdAsync(userId);
        if (user == null) return new ServiceResult { StatusCode = 404};

        user.Name = name;
        user.Bio = bio;
        user.AvatarUrl = avatarURL;
        
        await _userRepository.EditUserAsync(user);
        return new ServiceResult { StatusCode = 200};
    }

    public async Task<ServiceResult> AuthenticateUserAsync(string email, string password) {
        var user = await _userRepository.GetUserByEmailAsync(email);
        if (user == null) return new ServiceResult { StatusCode = 404};
        
        var isPasswordValid = _authentificationService.Verify(password, user.PasswordHash);
        if (isPasswordValid) return new ServiceResult { StatusCode = 200, Data = user};

        // TODO : mecanique anti forcing à prévoir
        return new ServiceResult { StatusCode = 401};
    }

    public async Task<ServiceResult> DeleteUserAsync(string userId) {
        await _userRepository.DeleteUserAsync(userId);

        return new ServiceResult { StatusCode = 200};
    }

    public async Task GiveBricks(string userId, int bricksQuantity) {
        var user = await _userRepository.GetUserByIdAsync(userId);
        if (user == null) return;

        user.BricksQuantity += bricksQuantity;
        
        await _userRepository.EditUserAsync(user);
    }
}