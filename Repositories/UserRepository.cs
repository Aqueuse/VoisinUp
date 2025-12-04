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

    public async Task CreateUserAsync(User user) {
        await using var connection = new NpgsqlConnection(_connectionString);
        
        var id = await connection.InsertAsync(user);

        Console.WriteLine("[Success] user successfully created. UserId is "+id);
    }

    public async Task EditUserAsync(User user) {
        await using var connection = new NpgsqlConnection(_connectionString);

        await connection.UpdateAsync(user);
        
        Console.WriteLine("[Success] user successfully edited");
    }

    public async Task DeleteUserAsync(string userId) {
        await using var connection = new NpgsqlConnection(_connectionString);

        await connection.DeleteAsync<UserQuests>(q => q.UserId == userId);

        await connection.DeleteAsync<User>(a => a.UserId == userId);
        
        Console.WriteLine("[Success] user successfully deleted");
        
        // TODO remove userStuff from UserAssets
    }
    
    public async Task<User?> GetUserByIdAsync(string userId) {
        await using var _connection = new NpgsqlConnection(_connectionString);

        var userQuery = await _connection.QueryAsync<User>(u => u.UserId == userId);
        var enumerable = userQuery.ToList();
        
        if (!enumerable.Any())
            return null;
        
        var user = enumerable.First();

        var userAssets = await _connection.QueryAsync<UserAssets>(ua => ua.UserId == userId);

        user.UserAssets = userAssets.ToList();
        
        return user;
    }
    
    public async Task<User?> GetUserByEmailAsync(string email) {
        await using var _connection = new NpgsqlConnection(_connectionString);

        var user = await _connection.QueryAsync<User>(u => u.Email == email);

        return user.FirstOrDefault();
    }
}