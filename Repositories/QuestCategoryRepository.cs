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
    public async Task<QuestCategory[]?> GetAllCategoriesDetails() {
        await using var connection = new NpgsqlConnection(_connectionString);

        var questCategories = await connection.QueryAllAsync<QuestCategory>();

        return questCategories.ToArray();
    }

    public async Task<List<int>> GetQuestCategoriesId(string questId) {
        await using var connection = new NpgsqlConnection(_connectionString);

        List<int> questCategoriesId = new List<int>();

        var categories = await connection.QueryAsync<QuestCategories>(q => q.QuestId == questId);
        
        foreach (var category in categories) {
            var categoryDetail = await connection.QueryAsync<QuestCategory>(q => q.CategoryId == category.CategoryId);
            questCategoriesId.Add(categoryDetail.First().CategoryId);
        }
        
        return questCategoriesId;
    }

    public async Task AddQuestCategories(string questId, int[] questCategoriesId) {
        await using var connection = new NpgsqlConnection(_connectionString);
        
        foreach (var categoryId in questCategoriesId) {
            await connection.InsertAsync(new QuestCategories { QuestId = questId, CategoryId = categoryId});
        }
    }
    
    public async Task EditQuestCategories(string questId, int[] questCategoriesId) {
        await using var connection = new NpgsqlConnection(_connectionString);
        
        // remove old categories
        await connection.DeleteAsync<QuestCategories>(q => q.QuestId == questId);
        
        foreach (var categoryId in questCategoriesId) {
            await connection.InsertAsync(new QuestCategories { QuestId = questId, CategoryId = categoryId});
        }
    }
}