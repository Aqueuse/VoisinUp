using VoisinUp.Models;
using VoisinUp.Repositories;

namespace VoisinUp.Services;

public class QuestCategoryService {
    private readonly QuestCategoryRepository _questCategoryRepository;

    public QuestCategoryService(QuestCategoryRepository questCategoryRepository) {
        _questCategoryRepository = questCategoryRepository;
    }

    public async Task AddQuestCategories(string questId, int[] questCategoriesId) {
        await _questCategoryRepository.AddQuestCategories(questId, questCategoriesId);
    }

    public async Task EditQuestCategories(string questId, int[] questCategoriesId) {
        await _questCategoryRepository.EditQuestCategories(questId, questCategoriesId);
    }
    
    public async Task<ServiceResult> GetAllCategories() {
        var allCategories = await _questCategoryRepository.GetAllCategoriesDetails();
        return new ServiceResult { StatusCode = 200, Data = allCategories};
    }

    public async Task<List<int>> GetQuestCategories(string questId) {
        var questCategories = await _questCategoryRepository.GetQuestCategoriesId(questId);
        return questCategories;
    }
}