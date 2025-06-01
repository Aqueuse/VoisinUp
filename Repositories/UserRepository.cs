using Npgsql;
using RepoDb;
using VoisinUp.Configuration;
using VoisinUp.Models;

namespace VoisinUp.Repositories;

public class UserRepository {
    private readonly string _connectionString;

    public UserRepository(DbConfig dbConfig) {
        _connectionString = dbConfig.ConnectionString;
    }

    // 🔹 Crée un utilisateur et sa grille 100x100x5
    public async Task CreateUserAsync(User user) {
        await using var connection = new NpgsqlConnection(_connectionString);
        
        var id = await connection.InsertAsync(user);

        Console.WriteLine("[Success] user successfully created. UserId is "+id);

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

    public async Task EditUserAsync(User user) {
        await using var connection = new NpgsqlConnection(_connectionString);

        await connection.UpdateAsync(user);
        
        Console.WriteLine("[Success] user successfully edited");
    }

    // 🔹 Supprime un utilisateur et sa grille
    public async Task DeleteUserAsync(string userId) {
        await using var connection = new NpgsqlConnection(_connectionString);

        await connection.DeleteAsync<UserQuests>(q => q.UserId == userId);

        await connection.DeleteAsync<User>(a => a.UserId == userId);
        
        Console.WriteLine("[Success] user successfully deleted");
        
        // TODO remove GRILLE
    }
    
    // 🔹 query un utilisateur by userId
    public async Task<User?> GetUserByIdAsync(string userId) {
        await using var _connection = new NpgsqlConnection(_connectionString);

        var user = await _connection.QueryAsync<User>(u => u.UserId == userId);

        return user.First();
    }
    
    // 🔹 query un utilisateur by email
    public async Task<User?> GetUserByEmailAsync(string email) {
        await using var _connection = new NpgsqlConnection(_connectionString);

        var user = await _connection.QueryAsync<User>(u => u.Email == email);

        return user.FirstOrDefault();
    }
}