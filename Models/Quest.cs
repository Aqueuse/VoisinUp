namespace VoisinUp.Models;

public class Quest {
    public string QuestId { get; set; }
    public string CreatedBy { get; set; }
    public int VoisinageId { get; set; }
    
    public string Name { get; set; }
    public string Description { get; set; }
    public string Status { get; set; }
    
    public string DateCreated { get; set; }
    public string DateStarted { get; set; }
}