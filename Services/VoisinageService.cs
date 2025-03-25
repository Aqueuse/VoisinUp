using VoisinUp.Models;
using VoisinUp.Repositories;

namespace VoisinUp.Services;

public class VoisinageService {
    private readonly VoisinageRepository _voisinageRepository;

    public VoisinageService(VoisinageRepository voisinageRepository) {
        _voisinageRepository = voisinageRepository;
    }
    
    public async Task<UserInfo[]> GetVoisinsByIdAsync(int voisinageId) {
        var voisins = await _voisinageRepository.GetVoisinsByIdAsync(voisinageId);
    
        var voisinsInfo = voisins.Select(v => new UserInfo {
            Name = v.Name,
            AvatarUrl = v.AvatarUrl,
            Bio = v.Bio
        });

        return voisinsInfo.ToArray();
    }
    
}