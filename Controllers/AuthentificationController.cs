using Microsoft.AspNetCore.Mvc;
using VoisinUp.Models;
using VoisinUp.Services;

namespace VoisinUp.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthentificationController : Controller {
    private readonly UserService _userService;
    private readonly AuthentificationService _authentificationService;

    public AuthentificationController(UserService userService, AuthentificationService authentificationService) {
        _userService = userService;
        _authentificationService = authentificationService;
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLogin request) {
        var result = await _userService.AuthenticateUserAsync(request.Email, request.Password);
        
        if (result is { StatusCode: 200, Data: User user }) {
            var token = _authentificationService.GenerateJwtToken(user);
            return StatusCode(200, new { token });
        }
        
        return StatusCode(result.StatusCode);
    }
}