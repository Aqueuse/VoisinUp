using Npgsql;
using RepoDb;
using VoisinUp.Configuration;
using VoisinUp.Models;

namespace VoisinUp.Repositories;

public class VoisinageRepository {
    private readonly string _connectionString;
    
    public VoisinageRepository(DbConfig dbConfig) {
        _connectionString = dbConfig.ConnectionString;
    }

    public async Task<Voisinage?> GetVoisinageByIdAsync(int voisinageId) {
        await using var connection = new NpgsqlConnection(_connectionString);
        
        var voisinage = await connection.QueryAsync<Voisinage>(q => q.VoisinageId == voisinageId);

        return voisinage.First();
    }
    
    public async Task<User[]> GetVoisinsByIdAsync(int voisinageId) {
        await using var connection = new NpgsqlConnection(_connectionString);

        var voisins = await connection.QueryAsync<User>(q => q.VoisinageId == voisinageId);

        return voisins.ToArray();
    }
}