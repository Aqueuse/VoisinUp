﻿using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
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

    [Authorize]
    [HttpGet("get-user-profile")]
    public async Task<IActionResult> GetUserProfile() {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null) return Unauthorized();
        
        var result = await _userService.GetUserProfileByIdAsync(userIdClaim);
        if (result.StatusCode == 404) return StatusCode(404);

        return Ok(result.Data);
    }
    
    [HttpPost("create")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUser createUser) {
        var result = await _userService.CreateUserAsync(createUser);
        
        return StatusCode(result.StatusCode);
    }
    
    [Authorize]
    [HttpPost("edit")]
    public async Task<IActionResult> EditUser([FromBody] EditUser editUser) {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null) return Unauthorized();

        var result = await _userService.EditUserAsync(userIdClaim, editUser.Name, editUser.Bio, editUser.AvatarUrl);
        
        return StatusCode(result.StatusCode);
    }
    
    [Authorize]
    [HttpDelete("delete")]
    public async Task<IActionResult> DeleteUser([FromBody] string userId) {
        var result = await _userService.DeleteUserAsync(userId);
        
        return StatusCode(result.StatusCode);
    }
}