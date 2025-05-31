namespace VoisinUp.Models;

public class GridAsset {
    public required string UserId { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public int Z { get; set; } = 0;
    public int? AssetId { get; set; }
}