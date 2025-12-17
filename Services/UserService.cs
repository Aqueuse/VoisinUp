using VoisinUp.Models;
using VoisinUp.Repositories;

namespace VoisinUp.Services;

public class UserService {
    private readonly UserRepository _userRepository;
    private readonly VoisinageRepository _voisinageRepository;
    private readonly AuthentificationService _authentificationService;
    private readonly JsonCdnService _jsonCdnService;

    public UserService(UserRepository userRepository, AuthentificationService authentificationService, VoisinageRepository voisinageRepository, JsonCdnService jsonCdnService) {
        _userRepository = userRepository;
        _voisinageRepository = voisinageRepository;
        
        _authentificationService = authentificationService;
        _jsonCdnService = jsonCdnService;
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
    
    public async Task<ServiceResult> CreateUserAsync(CreateUser createUser) {
        var voisinage = await _voisinageRepository.GetVoisinageByIdAsync(createUser.VoisinageId);
        if (voisinage == null) {
            return new ServiceResult { StatusCode = 404};
        }

        var user = await _userRepository.GetUserByEmailAsync(createUser.Email);

        if (user != null) {
            return new ServiceResult { StatusCode = 409};
        }

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
            AvatarUrl = "",
            BricksQuantity = 50
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

    public async Task<ServiceResult> GiveBricks(string userId, int bricksQuantity) {
        var user = await _userRepository.GetUserByIdAsync(userId);
        if (user == null) return new ServiceResult { StatusCode = 404};

        user.BricksQuantity += bricksQuantity;
        
        await _userRepository.EditUserAsync(user);
        
        return new ServiceResult { StatusCode = 200};
    }

    public async Task<ServiceResult> BuyAsset(string userId, string assetId) {
        var user = await _userRepository.GetUserByIdAsync(userId);
        if (user == null) return new ServiceResult { StatusCode = 404};

        var userAsset = new UserAssets() {
            UserId = userId,
            AssetId = assetId,
            Coordinates = "0",
            InInventory = true,
            Orientation = "0",
            UserAssetId = "assetId-" + Guid.NewGuid()
        };
        
        var assetCatalog = await _jsonCdnService.TryGetJsonCatalog();
        if (assetCatalog == null) return new ServiceResult { StatusCode = 404};

        var assetInCatalog = assetCatalog.FirstOrDefault(ua => ua.assetId == assetId);
        if (assetInCatalog == null) return new ServiceResult { StatusCode = 404};

        if (assetInCatalog.assetCost > user.BricksQuantity)
            return new ServiceResult() { StatusCode = 403 }; // pas assez de monnaie (mais innaccessible normalement car on vérifie en front, hello cheater)
        
        await _userRepository.BuyAsset(userAsset);
        await _userRepository.RemoveUserBrickQuantity(userId, assetInCatalog.assetCost);
        
        var userInventory = await _userRepository.GetUserAssets(userId);
        
        return new ServiceResult { StatusCode = 200, Data = userInventory};
    }
    
    public async Task<ServiceResult> UpdateAsset(string userId, string userAssetId, string coordinates, string orientation, bool inInventory) {
        var user = await _userRepository.GetUserByIdAsync(userId);
        if (user == null) return new ServiceResult { StatusCode = 404};

        var asset = await _userRepository.GetAsset(userId, userAssetId);
        if (asset == null) return new ServiceResult { StatusCode = 404};

        asset.Coordinates = coordinates;
        asset.Orientation = orientation;
        asset.InInventory = inInventory;

        await _userRepository.UpdateAsset(asset);
        
        return new ServiceResult { StatusCode = 200};
    }
}