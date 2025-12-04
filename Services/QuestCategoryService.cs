using VoisinUp.Models;
using VoisinUp.Repositories;

namespace VoisinUp.Services;

public class QuestCategoryService {
    private readonly QuestCategoryRepository _questCategoryRepository;
    private readonly QuestRepository _questRepository;

    public QuestCategoryService(QuestCategoryRepository questCategoryRepository, QuestRepository questRepository) {
        _questCategoryRepository = questCategoryRepository;
        _questRepository = questRepository;
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
        var questCategories = await _questRepository.GetQuestCategoriesId(questId);
        return questCategories;
    }
}