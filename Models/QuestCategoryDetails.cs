namespace VoisinUp.Models;

public class QuestCategory {
    public int CategoryId { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
}

public class QuestCategories {
    public required string QuestId { get; set; }
    public int CategoryId { get; set; }
}