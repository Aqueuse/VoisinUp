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
    
    public async Task<UserCard[]> GetVoisinsByVoisinageIdAsync(int voisinageId, string userId) {
        await using var connection = new NpgsqlConnection(_connectionString);
        
        var allUsers = await connection.QueryAsync<User>(q => q.VoisinageId == voisinageId);

        var enumerable = allUsers.ToList();
        var currentUser = enumerable.FirstOrDefault(u => u.UserId == userId);
        var others = enumerable.Where(u => u.UserId != userId);

        var result = new List<UserCard>();

        if (currentUser != null) {
            result.Add(new UserCard {
                Name = currentUser.Name,
                AvatarUrl = currentUser.AvatarUrl,
                Bio = currentUser.Bio,
                LastLogin = currentUser.LastLogin
            });
        }

        result.AddRange(others.Select(v => new UserCard {
            Name = v.Name,
            AvatarUrl = v.AvatarUrl,
            Bio = v.Bio,
            LastLogin = v.LastLogin
        }));
        
        return result.ToArray();
    }
}