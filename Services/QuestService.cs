using VoisinUp.Models;
using VoisinUp.Repositories;

namespace VoisinUp.Services;

public class QuestService {
    private readonly QuestRepository _questRepository;
    private readonly UserService _userService;
    private readonly QuestCategoryService _questCategoryService;

    public QuestService(QuestRepository questRepository, UserService userService, QuestCategoryService questCategoryService) {
        _questRepository = questRepository;
        _userService = userService;
        _questCategoryService = questCategoryService;
    }
    
    public async Task<ServiceResult> CreateQuest(CreateQuest createQuest, string userId) {
        var user = await _userService.GetUserByIdAsync(userId);
        if (user == null) return new ServiceResult { StatusCode = 404};
        
        var quest = new Quest {
            QuestId = Guid.NewGuid().ToString(),
            CreatedBy = userId,
            VoisinageId = user.VoisinageId,
            Name = createQuest.Name,
            Description = createQuest.Description,
            Status = nameof(QuestStatus.await_participants),
            DateCreated = DateTime.UtcNow
        };
        
        await _questRepository.CreateQuestAsync(quest);

        await _questCategoryService.AddQuestCategories(quest.QuestId, createQuest.Categories);
        
        return new ServiceResult { StatusCode = 200};
    }
    
    public async Task<ServiceResult> UpdateQuest(UpdateQuest updateQuest, string userId) {
        var user = await _userService.GetUserByIdAsync(userId);
        if (user == null) return new ServiceResult { StatusCode = 404 };

        var questToUpdate = await _questRepository.GetQuestByQuestId(updateQuest.QuestId);
        if (questToUpdate == null) return new ServiceResult { StatusCode = 404 };
        
        var quest = new Quest {
            QuestId = questToUpdate.QuestId,
            CreatedBy = questToUpdate.CreatedBy,
            VoisinageId = questToUpdate.VoisinageId,
            Name = updateQuest.Name,
            Description = updateQuest.Description,
            Status = questToUpdate.Status,
            DateCreated = DateTime.SpecifyKind(questToUpdate.DateCreated, DateTimeKind.Utc)
        };
        
        await _questRepository.UpdateQuest(quest);

        await _questCategoryService.EditQuestCategories(quest.QuestId, updateQuest.Categories);
        
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
    
    public async Task<ServiceResult> LaunchQuest(string questId, string userId) {
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

    public async Task<ServiceResult> Claim(string questId, string userId) {
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

    public async Task<QuestDetails[]?> GetQuestsByUserId(string userId) {
        var user = await _userService.GetUserByIdAsync(userId);
        if (user == null) return null;

        var quests = await _questRepository.GetQuestsByUserId(userId);

        var questsDetailsList = new List<QuestDetails>();

        foreach (var quest in quests) {
            var participants = await _questRepository.GetParticipantsForQuestAsync(quest.QuestId);
            
            questsDetailsList.Add(
                new QuestDetails {
                    QuestId = quest.QuestId,
                    CreatedBy = quest.CreatedBy,
                    Name = quest.Name,
                    Description = quest.Description,
                    Status = quest.Status,
                    DateCreated = quest.DateCreated,
                    DateStarted = quest.DateStarted,
                    Participants = participants,
                    Categories = await _questCategoryService.GetQuestCategories(quest.QuestId),
                    IsOwner = userId == quest.CreatedBy,
                    IsOrphan = await IsUserParticipatingOnQuest(quest.CreatedBy, quest.QuestId)
                }
            );
        }
        
        return questsDetailsList.ToArray();
    }
    
    public async Task<QuestDetails?> GetQuestByQuestId(string questId) {
        var quest = await _questRepository.GetQuestByQuestId(questId);
        if (quest == null) return null;
        
        var questDetails = new QuestDetails {
            QuestId = quest.QuestId,
            CreatedBy = quest.CreatedBy,
            Name = quest.Name,
            Description = quest.Description,
            Status = quest.Status,
            DateCreated = quest.DateCreated,
            DateStarted = quest.DateStarted,
            Participants = await _questRepository.GetParticipantsForQuestAsync(questId),
            Categories = await _questCategoryService.GetQuestCategories(questId),
            IsOrphan = await IsUserParticipatingOnQuest(quest.CreatedBy, quest.QuestId)
        };
        
        return questDetails;
    }
    
    public async Task<QuestDetails[]?> GetQuestsByUserVoisinageId(string userId) {
        var user = await _userService.GetUserByIdAsync(userId);
        if (user == null) return null;
        
        var quests = await _questRepository.GetQuestsByVoisinageId(user.VoisinageId, userId);
        
        foreach (var quest in quests) {
            if (quest.QuestId == null) continue;
            quest.Participants = await _questRepository.GetParticipantsForQuestAsync(quest.QuestId);
            quest.Categories = await _questCategoryService.GetQuestCategories(quest.QuestId);
        }

        return quests;
    }
    
    private async Task<bool> IsUserParticipatingOnQuest(string userId, string questId) {
        var isParticipating = await _questRepository.IsUserParticipatingOnQuest(questId, userId);
        return isParticipating;
    }
}