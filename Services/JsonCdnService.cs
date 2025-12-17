using System.Text.Json;
using VoisinUp.Models;

namespace VoisinUp.Services;

public class JsonCdnService() {
    private readonly List<AssetData> assetCatalog = [];
    
    public async Task<List<AssetData>?> TryGetJsonCatalog() {
        if (assetCatalog.Count > 0)
            return assetCatalog;

        HttpClient httpClient = new HttpClient();

        var json = await httpClient.GetStringAsync("https://d3sacmavupt5l0.cloudfront.net/catalog.json");
        var assetCatalogJson = JsonSerializer.Deserialize<List<AssetData>>(json);

        if (assetCatalogJson != null) return assetCatalogJson;

        Console.WriteLine("[Critical Error] Can‘t retreive jsonCatalog");
        return null;
    }
}