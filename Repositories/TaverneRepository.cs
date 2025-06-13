using Npgsql;
using RepoDb;
using RepoDb.Enumerations;
using VoisinUp.Configuration;
using VoisinUp.Models;

namespace VoisinUp.Repositories;

public class TaverneRepository {
    private readonly string _connectionString;
    
    public TaverneRepository(DbConfig dbConfig) {
        _connectionString = dbConfig.ConnectionString;
    }

    public async Task DeleteMessagesOlderThan(DateTime threshold) {
        await using var connection = new NpgsqlConnection(_connectionString);

        await connection.DeleteAsync<TaverneMessage>(m => m.Timestamp < threshold);
    }

    public async Task<TaverneMessage[]> GetLastMessages() {
        await using var connection = new NpgsqlConnection(_connectionString);
        
        var sql = """SELECT * FROM "TaverneMessage" ORDER BY "Timestamp" DESC LIMIT 100""";

        var messagesDesc = await connection.ExecuteQueryAsync<TaverneMessage>(sql);
        
        return messagesDesc.ToArray();
    }

    public async Task SaveMessage(TaverneMessage taverneMessage) {
        await using var connection = new NpgsqlConnection(_connectionString);

        await connection.InsertAsync(taverneMessage);
    }
}