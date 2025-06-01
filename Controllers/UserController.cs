using System.Security.Claims;
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

    [HttpGet("get-user-profile")]
    public async Task<IActionResult> GetUserProfile() {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null) return Unauthorized();
        
        var profile = await _userService.GetUserByIdAsync(userIdClaim);
        if (profile == null) return NotFound();
        
        return Ok(profile);
    }
    
    [HttpPost("create")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUser createUser) {
        var result = await _userService.CreateUserAsync(createUser);
        
        return StatusCode(result.StatusCode);
    }
    
    [HttpPost("edit")]
    public async Task<IActionResult> EditUser([FromBody] EditUser editUser) {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null) return Unauthorized();

        var result = await _userService.EditUserAsync(userIdClaim, editUser.Name, editUser.Bio, editUser.AvatarUrl);
        
        return StatusCode(result.StatusCode);
    }
    
    [HttpDelete("{userId}")]
    public async Task<IActionResult> DeleteUser(string userId) {
        var result = await _userService.DeleteUserAsync(userId);
        
        return StatusCode(result.StatusCode);
    }
}