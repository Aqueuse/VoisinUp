using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using VoisinUp.Models;
using VoisinUp.Services;

namespace VoisinUp.Hubs;

public class TaverneHub : Hub {
    private readonly UserService _userService;
    private readonly TaverneService _taverneService;

    public TaverneHub(UserService userService, TaverneService taverneService) {
        _userService = userService;
        _taverneService = taverneService;
    }
    
    [Authorize]
    public override async Task OnConnectedAsync() {
        var userIdClaim = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        Console.WriteLine(" 👋 Voisin connecté : "+userIdClaim);
        await base.OnConnectedAsync();
    }

    [Authorize]
    public async Task SendMessage(string message) {
        var userIdClaim = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null) return;
        
        var userCard = await _userService.GetUserCard(userIdClaim);
        
        if (userCard == null) {
            Console.WriteLine("[Error] user "+ userIdClaim +"not found");
            return;
        }
        
        Console.WriteLine($"📨 Message reçu : {message}");

        if (message.Length > 0) {
            await _taverneService.SaveMessage(new TaverneMessage {
                Content = message,
                MessageId = Guid.NewGuid().ToString(),
                UserId = userIdClaim,
                Timestamp = DateTime.UtcNow
            });
        
            await Clients.All.SendAsync("ReceiveMessage",
                new TaverneMessageDto {
                    Content = message,
                    Timestamp = DateTime.UtcNow,
                    UserCard =  userCard
                });

            await Clients.All.SendAsync("receiveMessage", message);
        }
    }
}