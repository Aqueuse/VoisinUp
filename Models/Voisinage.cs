namespace VoisinUp.Models;

public class Voisinage {
    public int VoisinageId { get; set; }
    
    public string Name { get; set; }
    public string Commune { get; set; }
    public string Country { get; set; }
    public int Population { get; set; }
}