namespace VoisinUp.Models;

public class QuestCategory {
    public int CategoryId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string ImageUrl { get; set; }
}

public class QuestCategories {
    public string QuestId { get; set; }
    public int CategoryId { get; set; }
}