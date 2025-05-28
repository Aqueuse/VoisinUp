using VoisinUp.Models;
using VoisinUp.Repositories;

namespace VoisinUp.Services;

public class VoisinageService {
    private readonly VoisinageRepository _voisinageRepository;
    private readonly UserService _userService;

    public VoisinageService(VoisinageRepository voisinageRepository, UserService userService) {
        _voisinageRepository = voisinageRepository;
        _userService = userService;
    }
    
    public async Task<UserCard[]?> GetUserVoisinsAsync(string userId) {
        var user = await _userService.GetUserByIdAsync(userId);
        if (user == null) return null;

        var voisins = await _voisinageRepository.GetVoisinsByVoisinageIdAsync(user.VoisinageId, userId);
        
        var voisinsInfo = voisins.Select(v => new UserCard {
            Name = v.Name,
            AvatarUrl = v.AvatarUrl,
            Bio = v.Bio,
            LastLogin = v.LastLogin
        });

        return voisinsInfo.ToArray();
    }
}