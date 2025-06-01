namespace VoisinUp.Models;

public class User {
    public string? UserId { get; set; }
    
    public string Name { get; set; }
    
    public int VoisinageId { get; set; }

    public string? AvatarUrl { get; set; }
    public string? Bio { get; set; }
    
    public int TraitQuantity { get; set; }
    public int CarreauQuantity { get; set; }
    
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    
    public DateTime? CreationDate { get; set; }
    public DateTime? LastLogin { get; set; }

    public string Commune { get; set; }
    public string Country { get; set; }
    
    // 🔥 Relation avec la table des assets de l'utilisateur
    public List<GridAsset> GrilleAssets { get; set; } = new();
}

public class UserLogin  {
    public string Email { get; set; }
    public string Password { get; set; }
}

public class UserCard {
    public string Name { get; set; }
    public string? AvatarUrl { get; set; }
    public string? Bio { get; set; }
    public DateTime? LastLogin { get; set; }
}

public class UserQuests {
    public string QuestId { get; set; }
    public string UserId { get; set; }
}

public class CreateUser {
    public string Name { get; init; }
    public string Email { get; set; }

    public int VoisinageId { get; init; }
    public string PasswordHash { get; set; }

    public string? AvatarUrl { get; set; }
    public string? Bio { get; set; }
}

public class EditUser {
    public string Name { get; init; }
    public string? AvatarUrl { get; set; }
    public string? Bio { get; set; }
}