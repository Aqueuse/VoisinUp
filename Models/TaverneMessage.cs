namespace VoisinUp.Models;

public class TaverneMessage {
    public string MessageId { get ; set; }
    public string UserId { get ; set; }
    public string Content { get ; set; }
    public DateTime Timestamp { get ; set; }
}

public class TaverneMessageDto {
    public UserCard UserCard { get; set; }
    public string Content { get; set; }
    public DateTime Timestamp { get; set; }
}