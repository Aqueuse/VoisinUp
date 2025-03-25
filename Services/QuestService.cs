using VoisinUp.Models;
using VoisinUp.Repositories;

namespace VoisinUp.Services;

public class QuestService {
    private QuestRepository _questRepository;
    private UserRepository _userRepository;

    public QuestService(QuestRepository questRepository, UserRepository userRepository) {
        _questRepository = questRepository;
        _userRepository = userRepository;
    }
    
    public async Task<ServiceResult> CreateQuest(CreateQuest createQuest, string userId) {
        var user = await _userRepository.GetUserByIdAsync(userId);
        if (user == null) return new ServiceResult { StatusCode = 404, Message = "utilisateur non trouvé" };
        
        if (createQuest.Categories.Length == 0)
            return new ServiceResult { StatusCode = 400, Message = "au moins une catégorie est requise" };
        
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
        return new ServiceResult { StatusCode = 200, Message = "quête créée" };
    }

    public async Task<ServiceResult> DeleteQuest(string questId, string userId) {
        // verify if the quest exist
        var quest = await GetQuestByQuestId(questId);
        if (quest == null) return new ServiceResult { StatusCode = 404, Message = "quête non trouvée" };
        
        // only the owner of the quest can delete it
        if (quest.CreatedBy != userId) return new ServiceResult {StatusCode = 403, Message = "seul le créateur peut supprimer sa quête"};
        
        await _questRepository.DeleteQuest(questId);
        return new ServiceResult { StatusCode = 200, Message = "quête supprimée" };
    }
    
    public async Task<ServiceResult> StartQuest(string questId, string userId) {
        // verify if the quest exist
        var quest = await GetQuestByQuestId(questId);
        if (quest == null) return new ServiceResult { StatusCode = 404, Message = "quête non trouvée" };

        // only the owner of the quest can start it
        if (quest.CreatedBy != userId) return new ServiceResult {StatusCode = 403, Message = "seul le créateur peut supprimer sa quête"};

        await _questRepository.StartQuest(questId);
        return new ServiceResult { StatusCode = 200, Message = "quête démarrée"};
    }

    public async Task<ServiceResult> JoinQuest(string questId, string userId) {
        var quest = await GetQuestByQuestId(questId);
        if (quest == null) return new ServiceResult { StatusCode = 404, Message = "quête non trouvée" };
        
        // you cant join if you have created the quest
        if (quest.CreatedBy == userId) return new ServiceResult { StatusCode = 409, Message = "Vous êtes déjà dans la quête, créateur"};
        
        await _questRepository.JoinQuest(questId, userId);
        return new ServiceResult { StatusCode = 200, Message = "Vous avez rejoins la quête"};
    }

    public async Task<ServiceResult> LeaveQuest(string questId, string userId) {
        var quest = await GetQuestByQuestId(questId);
        if (quest == null) return new ServiceResult { StatusCode = 404, Message = "quête non trouvée" };
        
        // you cant leave if you have created the quest (you can only delete)
        if (quest.CreatedBy == userId) return new ServiceResult { StatusCode = 409, Message = "Le créateur ne peut pas quitter sa quête, seulement la supprimer"};
        
        await _questRepository.LeaveQuest(questId, userId);
        return new ServiceResult { StatusCode = 200, Message = "Quête quittée" };
    }
    
    public async Task<ServiceResult> CompleteQuest(string questId, string userId) {
        // verify if the quest exist
        var quest = await GetQuestByQuestId(questId);
        if (quest == null) return new ServiceResult { StatusCode = 404, Message = "quête non trouvée" };

        // only the createur of the quest can complete it
        if (userId != quest.CreatedBy) return new ServiceResult { StatusCode = 409, Message = "Seul le créateur d'une quête peux la marquer complétée"};

        // POC : give asset
        // V1 : also give money (Todo)
        // V2 : sometimes give success (Todo)

        await _questRepository.DeleteQuest(questId);
        return new ServiceResult { StatusCode = 200, Message = "Quête complétée" };
    }

    public async Task<ServiceResult> UpdateOwner(string questId, string userId) {
        // check if the quest exist
        var quest = await GetQuestByQuestId(questId);
        if (quest == null) return new ServiceResult { StatusCode = 404, Message = "quête non trouvée" };

        // check if the user is participating to the quest
        var isParticipating = await IsUserParticipatingOnQuest(userId, questId);
        if (!isParticipating) return new ServiceResult { StatusCode = 403, Message = "seul un utilisateur participant à la quête peut en réclamer la propriété"};

        // check if the ownership is truly failed
        var isOwnershipFailed = await _questRepository.IsQuestOwnershipFailed(questId);
        if (!isOwnershipFailed) return new ServiceResult { StatusCode = 403, Message = "la quête a déjà un propriétaire"};
        
        await _questRepository.UpdateOwnership(questId, userId);
        return new ServiceResult { StatusCode = 200, Message = "vous êtes le nouveau propriétaire" };
    }

    public async Task<Quest[]> GetQuestsByUserId(string userId) {
        var quests = await _questRepository.GetQuestsByUserId(userId);

        return quests;
    }
    
    public async Task<Quest?> GetQuestByQuestId(string questId) {
        var quest = await _questRepository.GetQuestByQuestId(questId);
        
        return quest;
    }
    
    public async Task<Quest[]> GetQuestsByVoisinageId(int voisinageId) {
        var quest = await _questRepository.GetQuestsByVoisinageId(voisinageId);

        return quest;
    }

    private async Task<bool> IsUserParticipatingOnQuest(string userId, string questId) {
        var isParticipating = await _questRepository.IsUserParticipatingOnQuest(questId, userId);
        return isParticipating;
    }
}