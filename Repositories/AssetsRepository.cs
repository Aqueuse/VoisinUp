using Npgsql;
using RepoDb;
using VoisinUp.Configuration;
using VoisinUp.Models;

namespace VoisinUp.Repositories;

public class AssetsRepository {
    private readonly string _connectionString;

    public AssetsRepository(DbConfig dbConfig) {
        _connectionString = dbConfig.ConnectionString;
    }

    public async Task<Asset[]> GetallAssets() {
        await using var connection = new NpgsqlConnection(_connectionString);

        // Étape 1 : Récupérer les QuestId associés à l'utilisateur
        var assets = await connection.QueryAllAsync<Asset>();
        return assets.ToArray();
    }
}