namespace VoisinUp.Models;

public class Asset {
    public required string AssetId { get; set; }
    public required string AssetName { get; set; }
    public required string AssetDescription { get; set; }
    public required string AssetCategory { get; set; }
    public int Cost { get; set; }
}

public class UserAssets {
    public required string UserId { get; set; }
    public required string AssetId { get; set; }
    public required string Coordinates { get; set; }
    public required string Orientation { get; set; } // quaternion
}