namespace VoisinUp.Models;

public class Asset {
    public required string AssetId { get; set; }
    public required string AssetName { get; set; }
    public int Cost { get; set; }
}

public class UserAssets {
    public required string UserId { get; set; }
    public required string AssetId { get; set; }
    public required string Coordinates { get; set; }
}

