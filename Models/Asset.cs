namespace VoisinUp.Models;

public class UserAssets {
    public required string UserId { get; set; }
    public required string AssetId { get; set; }
    public required string Coordinates { get; set; }
    public required string Orientation { get; set; } // quaternion
}