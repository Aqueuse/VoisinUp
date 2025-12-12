namespace VoisinUp.Models;

public class UserAssets {
    public required string UserAssetId { get; set; }
    public required string UserId { get; set; }
    public required string AssetId { get; set; }
    public required string Coordinates { get; set; }
    public required string Orientation { get; set; } // quaternion
    public required bool InInventory { get; set; }
}

public class UpdateAsset {
    public required string UserAssetId { get; set; }
    public required string Coordinates { get; set; }
    public required string Orientation { get; set; } // quaternion
    public required bool InInventory { get; set; }
}