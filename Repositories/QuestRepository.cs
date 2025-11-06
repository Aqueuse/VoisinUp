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
        
        await connection.InsertAsync(quest);
        
        await connection.InsertAsync(new UserQuests { QuestId = quest.QuestId, UserId = quest.CreatedBy});
        
        Console.WriteLine("[Success] Created quest with "+quest.QuestId);
    }
    
    // UPDATE
    public async Task UpdateQuest(Quest quest) {
        await using var connection = new NpgsqlConnection(_connectionString);

        var rowsAffected = await connection.UpdateAsync(quest);
        if (rowsAffected > 0) {
            Console.WriteLine("[Success] quest "+quest.QuestId +" updated");
            return;
        }

        Console.WriteLine("[Error] can't update the quest : "+quest.QuestId);
    }
    
    // GET
    public async Task<Quest?> GetQuestByQuestId(string questId) {
        await using var connection = new NpgsqlConnection(_connectionString);

        try {
            var quest = await connection.QueryAsync<Quest>(p => p.QuestId == questId);
            var enumerable = quest as Quest[] ?? quest.ToArray();
            
            if (enumerable.Length == 0) {
                Console.WriteLine("[Error] can't find the quest with questId : "+questId);
                return null;
            }
            return enumerable.First();
        }
        catch (Exception e) {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<QuestDetails[]> GetQuestsByVoisinageId(int voisinageId, string userId) {
        await using var connection = new NpgsqlConnection(_connectionString);

        var quests = await connection.QueryAsync<Quest>(p => p.VoisinageId == voisinageId);

        List<QuestDetails> questCards = new List<QuestDetails>();
        
        foreach (var quest in quests) {
            questCards.Add(new QuestDetails {
                QuestId = quest.QuestId,
                CreatedBy = quest.CreatedBy,
                Name = quest.Name,
                Description = quest.Description,
                Status = quest.Status,
                DateCreated = quest.DateCreated,
                DateStarted = quest.DateStarted,
                IsOrphan = await IsUserParticipatingOnQuest(quest.CreatedBy, quest.QuestId)
            });
        }
        
        return questCards.ToArray();
    }

    public async Task<Quest[]> GetQuestsByUserId(string userId) {
        await using var connection = new NpgsqlConnection(_connectionString);
        
        // Étape 1 : Récupérer les QuestId associés à l'utilisateur
        var userQuests = await connection.QueryAsync<UserQuests>(q => q.UserId == userId);
        var questIds = userQuests.Select(q => q.QuestId).ToList();
        
        if (!questIds.Any()) return [];

        // Étape 2 : Récupérer les quêtes correspondantes
        var quests = await connection.QueryAsync<Quest>(q => questIds.Contains(q.QuestId));

        List<Quest> questDetailsList = new List<Quest>();
        
        foreach (var quest in quests) {
            questDetailsList.Add(new Quest {
                QuestId = quest.QuestId,
                CreatedBy = quest.CreatedBy,
                DateCreated = quest.DateCreated,
                DateStarted = quest.DateStarted,
                Description = quest.Description,
                Name = quest.Name,
                Status = quest.Status
            });
        }
        
        return questDetailsList.ToArray();
    }

    public async Task<List<UserCard>> GetParticipantsForQuestAsync(string questId) {
        await using var connection = new NpgsqlConnection(_connectionString);

        var usersOnQuest = await connection.QueryAsync<UserQuests>(q => q.QuestId == questId);
        
        // TODO : optimize with LINQ
        var userQuestsEnumerable = usersOnQuest.ToList();
        
        var userCards = new List<UserCard>();
        
        foreach (var userQuest in userQuestsEnumerable) {
            var users = await connection.QueryAsync<User>(u => u.UserId == userQuest.UserId);

            var user = users.First();
            
            userCards.Add(
                new UserCard {
                    Name = user.Name,
                    AvatarUrl = user.AvatarUrl,
                    Bio = user.Bio,
                    LastLogin = user.LastLogin
                }
            );
        }

        return userCards;
    }

    public async Task<List<string>> GetParticipantsUserIdForQuestAsync(string questId) {
        await using var connection = new NpgsqlConnection(_connectionString);

        var usersOnQuest = await connection.QueryAsync<UserQuests>(q => q.QuestId == questId);
        
        // TODO : optimize with LINQ
        var userQuestsEnumerable = usersOnQuest.ToList();
        
        var participants = new List<string>();
        
        foreach (var userQuest in userQuestsEnumerable) {
            var users = await connection.QueryAsync<User>(u => u.UserId == userQuest.UserId);

            var user = users.First();
            if (user.UserId != null)
                participants.Add(user.UserId);
        }

        return participants;
    }
    
    public async Task JoinQuest(string questId, string userId) {
        await using var connection = new NpgsqlConnection(_connectionString);

        await connection.InsertAsync(new UserQuests { QuestId = questId, UserId = userId});
    }

    public async Task LeaveQuest(string questId, string userId) {
        await using var connection = new NpgsqlConnection(_connectionString);
        
        await connection.DeleteAsync<UserQuests>(q => q.UserId == userId && q.QuestId == questId);
        
        Console.WriteLine("[success] Leaved quest with questId "+questId);
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
