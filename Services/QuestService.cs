using VoisinUp.Models;
using VoisinUp.Repositories;

namespace VoisinUp.Services;

public class QuestService {
    private QuestRepository _questRepository;
    private UserService _userService;

    public QuestService(QuestRepository questRepository, UserService userService) {
        _questRepository = questRepository;
        _userService = userService;
    }
    
    public async Task<ServiceResult> CreateQuest(CreateQuest createQuest, string userId) {
        var user = await _userService.GetUserByIdAsync(userId);
        if (user == null) return new ServiceResult { StatusCode = 404};
        
        var quest = new Quest {
            QuestId = Guid.NewGuid().ToString(),
            Description = createQuest.Description,
            Name = createQuest.Name,
            CreatedBy = userId,
            VoisinageId = user.VoisinageId,
            DateCreated = DateTime.UtcNow,
            Status = QuestStatus.await_participants.ToString(),
            Categories = createQuest.Categories
        };
        
        await _questRepository.CreateQuestAsync(quest);
        return new ServiceResult { StatusCode = 200};
    }

    public async Task<ServiceResult> DeleteQuest(string questId, string userId) {
        // verify if the quest exist
        var quest = await GetQuestByQuestId(questId);
        if (quest == null) return new ServiceResult { StatusCode = 404};
        
        // only the owner of the quest can delete it
        if (quest.CreatedBy != userId) return new ServiceResult {StatusCode = 403};
        
        await _questRepository.DeleteQuest(questId);
        return new ServiceResult { StatusCode = 200};
    }
    
    public async Task<ServiceResult> StartQuest(string questId, string userId) {
        // verify if the quest exist
        var quest = await GetQuestByQuestId(questId);
        if (quest == null) return new ServiceResult { StatusCode = 404};

        // only the owner of the quest can start it
        if (quest.CreatedBy != userId) return new ServiceResult {StatusCode = 403};

        await _questRepository.StartQuest(questId);
        return new ServiceResult { StatusCode = 200};
    }

    public async Task<ServiceResult> JoinQuest(string questId, string userId) {
        var quest = await GetQuestByQuestId(questId);
        if (quest == null) return new ServiceResult { StatusCode = 404};
        
        // you cant join if you have created the quest
        if (quest.CreatedBy == userId) return new ServiceResult { StatusCode = 409};
        
        await _questRepository.JoinQuest(questId, userId);
        return new ServiceResult { StatusCode = 200};
    }

    public async Task<ServiceResult> LeaveQuest(string questId, string userId) {
        var quest = await GetQuestByQuestId(questId);
        if (quest == null) return new ServiceResult { StatusCode = 404};
        
        // you cant leave if you have created the quest (you can only delete)
        if (quest.CreatedBy == userId) return new ServiceResult { StatusCode = 409};
        
        await _questRepository.LeaveQuest(questId, userId);
        return new ServiceResult { StatusCode = 200};
    }
    
    public async Task<ServiceResult> CompleteQuest(string questId, string userId) {
        // verify if the quest exist
        var quest = await GetQuestByQuestId(questId);
        if (quest == null) return new ServiceResult { StatusCode = 404};

        // only the createur of the quest can complete it
        if (userId != quest.CreatedBy) return new ServiceResult { StatusCode = 409};

        // POC : give asset
        // V1 : also give money (Todo)
        // V2 : sometimes give success (Todo)

        await _questRepository.DeleteQuest(questId);
        return new ServiceResult { StatusCode = 200};
    }

    public async Task<ServiceResult> UpdateOwner(string questId, string userId) {
        // check if the quest exist
        var quest = await GetQuestByQuestId(questId);
        if (quest == null) return new ServiceResult { StatusCode = 404};

        // check if the user is participating to the quest
        var isParticipating = await IsUserParticipatingOnQuest(userId, questId);
        if (!isParticipating) return new ServiceResult { StatusCode = 403};

        // check if the ownership is truly failed
        var isOwnershipFailed = await _questRepository.IsQuestOwnershipFailed(questId);
        if (!isOwnershipFailed) return new ServiceResult { StatusCode = 403};
        
        await _questRepository.UpdateOwnership(questId, userId);
        return new ServiceResult { StatusCode = 200};
    }

    public async Task<QuestCard[]?> GetQuestsByUserId(string userId) {
        var user = await _userService.GetUserByIdAsync(userId);
        if (user == null) return null;

        var quests = await _questRepository.GetQuestsByUserId(userId);
        return quests;
    }
    
    public async Task<Quest?> GetQuestByQuestId(string questId) {
        var quest = await _questRepository.GetQuestByQuestId(questId);
        return quest;
    }
    
    public async Task<QuestCard[]?> GetQuestsByUserVoisinageAsync(string userId) {
        var user = await _userService.GetUserByIdAsync(userId);
        if (user == null) return null;
        
        var quests = await _questRepository.GetQuestsByVoisinageId(user.VoisinageId);
        
        foreach (var quest in quests) {
            if (quest.QuestId == null) continue;
            quest.participants = await _questRepository.GetParticipantsForQuestAsync(quest.QuestId);
        }

        return quests;
    }
    
    private async Task<bool> IsUserParticipatingOnQuest(string userId, string questId) {
        var isParticipating = await _questRepository.IsUserParticipatingOnQuest(questId, userId);
        return isParticipating;
    }
}