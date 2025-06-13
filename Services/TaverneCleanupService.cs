namespace VoisinUp.Services;

public class TaverneCleanupService : BackgroundService {
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<TaverneCleanupService> _logger;
    
    private readonly TimeSpan _interval = TimeSpan.FromHours(24);
    private readonly int _messageLifetimeInDays = 30;

    public TaverneCleanupService(IServiceScopeFactory scopeFactory, ILogger<TaverneCleanupService> logger) {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken) {
        _logger.LogInformation("🧹 taverne cleanup service started");
        
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var chatService = scope.ServiceProvider.GetRequiredService<TaverneService>();

                await chatService.PurgeOldMessages(_messageLifetimeInDays);
                _logger.LogInformation("✅ Messages anciens purgés !");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Erreur lors de la purge des messages");
            }

            await Task.Delay(_interval, cancellationToken);
        }
    }
}
