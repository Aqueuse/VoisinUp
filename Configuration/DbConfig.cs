namespace VoisinUp.Configuration;

public class DbConfig {
    public string ConnectionString { get; }

    public DbConfig() {
        var connectionString = Environment.GetEnvironmentVariable("VoisinUpDbConnection");

        if (string.IsNullOrEmpty(connectionString)) {
            throw new ArgumentNullException(nameof(connectionString), "❌ ERREUR : La variable d'environnement 'VoisinUpDbConnection' est introuvable !");
        }

        ConnectionString = connectionString;
    }
}