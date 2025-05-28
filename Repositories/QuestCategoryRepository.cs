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
    public async Task<QuestCategory[]?> GetCategories() {
        await using var connection = new NpgsqlConnection(_connectionString);

        var questCategories = await connection.QueryAllAsync<QuestCategory>();

        return questCategories.ToArray();
    }
}