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

    public async Task<UserAssets?> GetAsset(string userId, string userAssetId) {
        await using var _connection = new NpgsqlConnection(_connectionString);

        var asset = await _connection.QueryAsync<UserAssets>(ua => ua.UserId == userId && ua.UserAssetId == userAssetId);

        return asset.FirstOrDefault();
    }

    public async Task<UserAssetsInInventory[]?> GetUserAssets(string userId) {
        await using var _connection = new NpgsqlConnection(_connectionString);

        var userAssets = await _connection.QueryAsync<UserAssets>(ua => ua.UserId == userId && ua.InInventory);

        var assetsInInventory = new List<UserAssetsInInventory>();

        var userAssetsEnumerable = userAssets as UserAssets[] ?? userAssets.ToArray();
        
        foreach (var userAsset in userAssetsEnumerable) {
            assetsInInventory.Add(new UserAssetsInInventory() { UserAssetId = userAsset.UserAssetId, AssetId = userAsset.AssetId});
        }
        
        return assetsInInventory.ToArray();
    }
    
    public async Task BuyAsset(UserAssets userAsset) {
        await using var _connection = new NpgsqlConnection(_connectionString);
        
        await _connection.InsertAsync(userAsset);
        
        Console.WriteLine("[Success] userAsset successfully buyed. UserAssetId is"+userAsset.UserAssetId);
    }
    
    public async Task UpdateAsset(UserAssets userAsset) {
        await using var _connection = new NpgsqlConnection(_connectionString);

        await _connection.UpdateAsync(userAsset);

        Console.WriteLine("[Success] userAsset successfully updated. UserAssetId is"+userAsset.UserAssetId);
    }
}