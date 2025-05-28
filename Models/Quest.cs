namespace VoisinUp.Models;

public enum QuestStatus {
    await_participants,
    in_progress
}

public class Quest {
    public string? QuestId { get; set; } = Guid.NewGuid().ToString();
    public string CreatedBy { get; set; }
    public int VoisinageId { get; set; }
    
    public string Name { get; set; }
    public string Description { get; set; }
    public string Status { get; set; }

    public int[] Categories { get; set; }

    public DateTime DateCreated { get; set; }
    public DateTime DateStarted { get; set; }
}   

public class CreateQuest {
    public string Name { get; set; }
    public string Description { get; set; }
    public int[] Categories { get; set; }
}

public class UpdateQuest {
    public string QuestId { get; set; }
}

public class QuestCard {
    public string? QuestId { get; set; } = Guid.NewGuid().ToString();
    
    public string Name { get; set; }
    public string Description { get; set; }

    public string CreatedBy { get; set; }
    
    public DateTime DateCreated { get; set; }
    public DateTime DateStarted { get; set; }

    public int[] Categories { get; set; }
    public List<UserCard> participants { get; set; }
}

public class QuestCategories {
    public string QuestId { get; set; }
    public int CategoryId { get; set; }
}