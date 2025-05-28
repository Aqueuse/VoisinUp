using Npgsql;
using RepoDb;
using VoisinUp.Configuration;
using VoisinUp.Models;

namespace VoisinUp.Repositories;

public class QuestRepository {
    private readonly string _connectionString;

    public QuestRepository(DbConfig dbConfig) {
        _connectionString = dbConfig.ConnectionString;
    }

    // CREATE
    public async Task CreateQuestAsync(Quest quest) {
        await using var connection = new NpgsqlConnection(_connectionString);

        if (string.IsNullOrEmpty(quest.QuestId)) {
            throw new InvalidOperationException("QuestId ne peut pas être null !");
        }

        await connection.InsertAsync(quest);
        
        await connection.InsertAsync(new UserQuests { QuestId = quest.QuestId, UserId = quest.CreatedBy});
        
        foreach (var categoryId in quest.Categories.Take(2)) {
            await connection.InsertAsync(new QuestCategories { QuestId = quest.QuestId, CategoryId = categoryId});
        }
        
        Console.WriteLine("[Success] Created quest with "+quest.QuestId);
    }
    
    // UPDATE
    public async Task<Quest> UpdateQuest(Quest quest) {
        await using var connection = new NpgsqlConnection(_connectionString);

        var rowsAffected = await connection.UpdateAsync(quest);

        if (rowsAffected > 0) {
            return quest;
        }

        Console.WriteLine("[Error] can't update the quest : "+quest.QuestId);
        
        return quest;
    }
    
    // GET
    public async Task<Quest?> GetQuestByQuestId(string questId) {
        await using var connection = new NpgsqlConnection(_connectionString);

        var quest = await connection.QueryAsync<Quest>(p => p.QuestId == questId);

        return quest.First();
    }

    public async Task<QuestCard[]> GetQuestsByVoisinageId(int voisinageId) {
        await using var connection = new NpgsqlConnection(_connectionString);

        var quests = await connection.QueryAsync<Quest>(p => p.VoisinageId == voisinageId);

        List<QuestCard> questCards = new List<QuestCard>();
        
        foreach (var quest in quests) {
            questCards.Add(new QuestCard {
                QuestId = quest.QuestId,
                CreatedBy = quest.CreatedBy,
                Categories = quest.Categories,
                DateCreated = quest.DateCreated,
                DateStarted = quest.DateStarted,
                Description = quest.Description,
                Name = quest.Name,
                participants = await GetParticipantsForQuestAsync(quest.QuestId)
            });
        }
        
        return questCards.ToArray();
    }

    public async Task<QuestCard[]> GetQuestsByUserId(string userId) {
        await using var connection = new NpgsqlConnection(_connectionString);
        
        // Étape 1 : Récupérer les QuestId associés à l'utilisateur
        var userQuests = await connection.QueryAsync<UserQuests>(q => q.UserId == userId);
        var questIds = userQuests.Select(q => q.QuestId).ToList();
        
        if (!questIds.Any()) return [];

        // Étape 2 : Récupérer les quêtes correspondantes
        var quests = await connection.QueryAsync<Quest>(q => questIds.Contains(q.QuestId));

        List<QuestCard> questCards = new List<QuestCard>();
        
        foreach (var quest in quests) {
            questCards.Add(new QuestCard {
                QuestId = quest.QuestId,
                CreatedBy = quest.CreatedBy,
                Categories = quest.Categories,
                DateCreated = quest.DateCreated,
                DateStarted = quest.DateStarted,
                Description = quest.Description,
                Name = quest.Name,
                participants = await GetParticipantsForQuestAsync(quest.QuestId)
            });
        }
        
        return questCards.ToArray();
    }

    public async Task<List<UserCard>> GetParticipantsForQuestAsync(string questId) {
        await using var connection = new NpgsqlConnection(_connectionString);

        var quests = await connection.QueryAsync<UserQuests>(q => q.QuestId == questId);
        var userIds = quests.Select(q => q.UserId).Distinct().ToList();
        
        if (!userIds.Any()) return new List<UserCard>();

        var users = await connection.QueryAsync<User>(u => userIds.Contains(u.UserId));
        
        return users.Select(u => new UserCard {
            Name = u.Name,
            AvatarUrl = u.AvatarUrl,
            Bio = u.Bio,
            LastLogin = u.LastLogin
        }).ToList();
    }

    public async Task JoinQuest(string questId, string userId) {
        await using var connection = new NpgsqlConnection(_connectionString);

        await connection.InsertAsync(new UserQuests { QuestId = questId, UserId = userId});
    }

    public async Task LeaveQuest(string questId, string userId) {
        await using var connection = new NpgsqlConnection(_connectionString);
        
        await connection.DeleteAsync<UserQuests>(q => q.UserId == userId && q.QuestId == questId);
    }
    
    public async Task StartQuest(string questId) {
        await using var connection = new NpgsqlConnection(_connectionString);

        var updatedQuest = new { Status = QuestStatus.in_progress };
        var condition = new { QuestId = questId };
        
        await connection.UpdateAsync(
            "Quest",
            updatedQuest,
            condition
        );
    }
    
    // UPDATE ownership
    public async Task UpdateOwnership(string questId, string userId) {
        await using var connection = new NpgsqlConnection(_connectionString);
        
        var updatedData = new { CreatedBy = userId };
        var condition = new { QuestId = questId };
        
        await connection.UpdateAsync(
            "Quest",
            updatedData,
            condition
        );
    }
    
    // DELETE
    public async Task DeleteQuest(string questId) {
        await using var connection = new NpgsqlConnection(_connectionString);

        await connection.DeleteAsync<Quest>(q => q.QuestId == questId);
    }

    public async Task<bool> IsUserParticipatingOnQuest(string questId, string userId) {
        await using var connection = new NpgsqlConnection(_connectionString);

        var participation = await connection.CountAsync<UserQuests>(q => q.QuestId == questId && q.UserId == userId);
        return participation != 0;
    }

    public async Task<bool> IsQuestOwnershipFailed(string questId) {
        await using var connection = new NpgsqlConnection(_connectionString);

        var quest = await connection.QueryAsync<Quest>(p => p.QuestId == questId);
        
        // check if CreatedBy is null (because last owner was deleted)
        return string.IsNullOrEmpty(quest.First().CreatedBy);
    }
}
