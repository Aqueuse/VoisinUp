using VoisinUp.Models;
using VoisinUp.Repositories;

namespace VoisinUp.Services;

public class QuestCategoryService {
    private readonly QuestCategoryRepository _questCategoryRepository;

    public QuestCategoryService(QuestCategoryRepository questCategoryRepository) {
        _questCategoryRepository = questCategoryRepository;
    }

    public async Task AddQuestCategories(string questId, QuestCategoryDetails[] questCategoriesId) {
        await _questCategoryRepository.AddQuestCategories(questId, questCategoriesId);
    }

    public async Task EditQuestCategories(string questId, QuestCategoryDetails[] questCategoriesId) {
        await _questCategoryRepository.EditQuestCategories(questId, questCategoriesId);
    }
    
    public async Task<ServiceResult> GetAllCategories() {
        var allCategories = await _questCategoryRepository.GetAllCategories();
        return new ServiceResult { StatusCode = 200, Data = allCategories};
    }

    public async Task<List<QuestCategoryDetails>> GetQuestCategories(string questId) {
        var questCategories = await _questCategoryRepository.GetQuestCategories(questId);
        return questCategories;
    }
}