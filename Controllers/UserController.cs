using Microsoft.AspNetCore.Mvc;
using VoisinUp.Models;
using VoisinUp.Services;

namespace VoisinUp.Controllers;

[ApiController]
[Route("api/user")]
public class UserController : Controller {
    private readonly UserService _userService;

    public UserController(UserService userService) {
        _userService = userService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUser createUser) {
        var result = await _userService.CreateUserAsync(createUser);
        
        return StatusCode(result.StatusCode, result.Message);
    }

    [HttpDelete("{userId}")]
    public async Task<IActionResult> DeleteUser(string userId) {
        var result = await _userService.DeleteUserAsync(userId);
        
        return StatusCode(result.StatusCode, result.Message);
    }
}