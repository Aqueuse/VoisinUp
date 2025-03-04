using Npgsql;
using RepoDb;
using VoisinUp.Configuration;
using VoisinUp.Models;

namespace VoisinUp.Services;

public class VoisinageService {
    private readonly string _connectionString;
    
    public VoisinageService(DbConfig dbConfig) {
        _connectionString = dbConfig.ConnectionString;
    }
    
    public async Task<Voisinage?> GetVoisinageByIdAsync(int voisinageId) {
        await using var connection = new NpgsqlConnection(_connectionString);

        IEnumerable<Voisinage?> voisinages = await connection.QueryAsync<Voisinage>(q => q.VoisinageId == voisinageId);

        return voisinages?.First();
    }
}