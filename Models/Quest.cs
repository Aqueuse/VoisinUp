namespace VoisinUp.Models;

public class Quest {
    public string QuestId { get; set; } = Guid.NewGuid().ToString();
    public required string CreatedBy { get; set; }
    public int VoisinageId { get; set; }

    public required string Name { get; set; }
    public required string Description { get; set; }

    public DateTime DateCreated { get; set; }
    public DateTime DateStarted { get; set; }
}

public class CreateQuest {
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required int[] Categories { get; set; }
}

public class UpdateQuest {
    public required string QuestId { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required int[] Categories { get; set; }
}

public class QuestDetails {
    public string? QuestId { get; set; } = Guid.NewGuid().ToString();
    
    public required string Name { get; set; }
    public required string Description { get; set; }

    public required string CreatedBy { get; set; }
    public bool IsOwner { get; set; }
    public bool IsOrphan { get; set; }
    
    public DateTime DateCreated { get; set; }
    public DateTime DateStarted { get; set; }

    public required List<int> Categories { get; set; }
    public required List<UserCard> Participants { get; set; }
}

public class QuestComment
{
    public string CommentId { get; set; } = Guid.NewGuid().ToString();
    public required string QuestId { get; set; }
    public required string UserId { get; set; }

    public required string Content { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public string? UserName { get; set; } // pratique pour afficher directement
    public string? AvatarUrl { get; set; }
}