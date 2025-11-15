using VoisinUp.Models;
using VoisinUp.Repositories;

namespace VoisinUp.Services;

public class AssetsService {
    private readonly AssetsRepository _assetsRepository;

    public AssetsService(AssetsRepository assetsRepository) {
        _assetsRepository = assetsRepository;
    }
    
    public async Task<ServiceResult> GetAll() {
        var assets = await _assetsRepository.GetallAssets();
        return new ServiceResult { Data = assets, StatusCode = 200 };
    }
}