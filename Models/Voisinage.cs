namespace VoisinUp.Models;

public class Voisinage {
    public int VoisinageId { get; set; }
    
    public required string Name { get; set; }
    public required string Commune { get; set; }
    public required string Country { get; set; }
    public int Population { get; set; }
}