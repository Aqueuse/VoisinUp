namespace VoisinUp.Models;

public class ServiceResult {
    public int StatusCode { get; set; } // ex: 200, 404, 400
    public string Message { get; set; }
    public object? Data { get; set; }
}