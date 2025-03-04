namespace VoisinUp.Models;

public class User {
    public string UserId { get; set; }
    
    public required string Name { get; init; }
    public int VoisinageId { get; init; }

    public string AvatarUrl { get; set; }
    public string Bio { get; set; }
    
    public int TraitQuantity { get; set; }
    public int CarreauQuantity { get; set; }
    
    public required string Email { get; init; }
    
    public string BirthDate { get; set; }
    public string CreationDate { get; set; }
    public string LastLogin { get; set; }

    public required string Commune { get; init; }
    public required string Country { get; init; }
    
    // 🔥 Relation avec la table des assets de l'utilisateur
    public List<GridAsset> GrilleAssets { get; set; } = new();
}