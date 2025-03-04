using Npgsql;
using RepoDb;
using VoisinUp.Configuration;
using VoisinUp.Models;

namespace VoisinUp.Services;

public class UserService {
    private string _connectionString;

    public UserService(DbConfig dbConfig) {
        _connectionString = dbConfig.ConnectionString;
    }
    
    // 🔹 Crée un utilisateur et sa grille 100x100x5
    public async Task CreateUserAsync(string name, string email, string country, string commune, int voisinageId) {
        var user = new User {
            Name = name, 
            Email = email,
            Country = country,
            Commune = commune,
            VoisinageId = voisinageId
        };

        await using var connection = new NpgsqlConnection(_connectionString);
        
        var id = await connection.InsertAsync(user);

        Console.WriteLine("user successfully created. UserId is "+id);

        // // Générer la grille de 100x100x5
        // var gridAssets = new List<GridAsset>();
        // for (int x = 0; x < 100; x++) {
        //     for (int y = 0; y < 100; y++) {
        //         for (int z = 0; z < 5; z++) {
        //             gridAssets.Add(new GridAsset {
        //                 UserId = user.UserId,
        //                 X = x,
        //                 Y = y,
        //                 Z = z,
        //                 AssetId = null // Aucune asset de base
        //             });
        //         }
        //     }
        // }
    }

    // 🔹 query un utilisateur
    public async Task<User?> GetUserByIdAsync(string userId) {
        await using var _connection = new NpgsqlConnection(_connectionString);

        var user = await _connection.QueryAsync<User>(u => u.UserId == userId);

        return user.FirstOrDefault();
    }

    // 🔹 Supprime un utilisateur et sa grille
    public async Task DeleteUserAsync(string userId) {
        await using var connection = new NpgsqlConnection(_connectionString);
        
        var deletedRows = connection.DeleteAsync<User>(a => a.UserId == userId);

        // TODO remove GRILLE
    }
}