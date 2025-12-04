namespace VoisinUp.Models;

public class User {
    public required string UserId { get; set; }
    
    public required string Name { get; set; }
    
    public int VoisinageId { get; set; }

    public string? AvatarUrl { get; set; }
    public string? Bio { get; set; }
    
    public int BricksQuantity { get; set; }
    public int CakesQuantity { get; set; }
    
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    
    public DateTime? CreationDate { get; set; }
    public DateTime? LastLogin { get; set; }

    public required string Commune { get; set; }
    public required string Country { get; set; }
    
    // 🔥 Relation avec la table des assets de l'utilisateur
    public List<UserAssets> UserAssets { get; set; } = new();
}

public class UserProfile {
    public required string Name { get; set; }
    
    public int VoisinageId { get; set; }

    public string? AvatarUrl { get; set; }
    public string? Bio { get; set; }
    
    public int BricksQuantity { get; set; }
    public int CakesQuantity { get; set; }
    
    public required string Email { get; set; }
    
    public DateTime? CreationDate { get; set; }
    public DateTime? LastLogin { get; set; }
    
    // 🔥 Relation avec la table des assets de l'utilisateur
    public List<UserAssets> UserAssets { get; set; } = new();
}

public class UserLogin  {
    public required string Email { get; set; }
    public required string Password { get; set; }
}

public class UserCard {
    public required string Name { get; set; }
    public string? AvatarUrl { get; set; }
    public string? Bio { get; set; }
    public DateTime? LastLogin { get; set; }
}

public class UserQuests {
    public required string QuestId { get; set; }
    public required string UserId { get; set; }
}

public class CreateUser {
    public required string Name { get; init; }
    public required string Email { get; set; }

    public int VoisinageId { get; init; }
    public required string PasswordHash { get; set; }
}

public class EditUser {
    public required string Name { get; init; }
    public string? AvatarUrl { get; set; }
    public string? Bio { get; set; }
}