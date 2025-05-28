using VoisinUp.Models;
using VoisinUp.Repositories;

namespace VoisinUp.Services;

public class QuestCategoryService {
    private readonly QuestCategoryRepository _questCategoryRepository;

    public QuestCategoryService(QuestCategoryRepository questCategoryRepository) {
        _questCategoryRepository = questCategoryRepository;
    }
    
    public async Task<ServiceResult> GetCategories() {
        var questCategories = await _questCategoryRepository.GetCategories();
        return new ServiceResult { StatusCode = 200, Data = questCategories};
    }
}