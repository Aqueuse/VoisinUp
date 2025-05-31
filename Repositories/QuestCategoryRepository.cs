using Npgsql;
using RepoDb;
using VoisinUp.Configuration;
using VoisinUp.Models;

namespace VoisinUp.Repositories;

public class QuestCategoryRepository {
    private readonly string _connectionString;

    public QuestCategoryRepository(DbConfig dbConfig) {
        _connectionString = dbConfig.ConnectionString;
    }
    
    // GET
    public async Task<QuestCategoryDetails[]?> GetAllCategories() {
        await using var connection = new NpgsqlConnection(_connectionString);

        var questCategories = await connection.QueryAllAsync<QuestCategoryDetails>();

        return questCategories.ToArray();
    }

    public async Task<List<QuestCategoryDetails>> GetQuestCategories(string questId) {
        await using var connection = new NpgsqlConnection(_connectionString);

        List<QuestCategoryDetails> questCategoriesDetails = new List<QuestCategoryDetails>();

        var categories = await connection.QueryAsync<QuestCategories>(q => q.QuestId == questId);
        
        foreach (var category in categories) {
            var categoryDetail = await connection.QueryAsync<QuestCategoryDetails>(q => q.CategoryId == category.CategoryId);
            questCategoriesDetails.Add(categoryDetail.First());
        }
        
        return questCategoriesDetails;
    }

    public async Task AddQuestCategories(string questId, QuestCategoryDetails[] questCategories) {
        await using var connection = new NpgsqlConnection(_connectionString);
        
        foreach (var category in questCategories) {
            await connection.InsertAsync(new QuestCategories { QuestId = questId, CategoryId = category.CategoryId});
        }
    }
    
    public async Task EditQuestCategories(string questId, QuestCategoryDetails[] questCategories) {
        await using var connection = new NpgsqlConnection(_connectionString);
        
        // remove old categories
        await connection.DeleteAsync<QuestCategories>(q => q.QuestId == questId);
        
        foreach (var category in questCategories) {
            await connection.InsertAsync(new QuestCategories { QuestId = questId, CategoryId = category.CategoryId});
        }
    }
}