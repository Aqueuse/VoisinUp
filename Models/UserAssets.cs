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

public class UserAssetsInInventory {
    public required string UserAssetId { get; set; }
    public required string AssetId { get; set; }
}

public class AssetData {
    public required string assetId { get; set; }
    public required string assetNameHumanReadable { get; set; }
    public required string assetDescription { get; set; }
    public required int assetCost { get; set; }
    public AssetCategory assetCategory { get; set; }
}

public enum AssetCategory {
    EPIC = 0,
    BUILDING = 1,
    NATURE = 2,
    DECOR = 3,
    RENCONTRE = 4,
    PROPRETE = 5,
    ECLAIRAGE = 6,
    FUN = 7
}