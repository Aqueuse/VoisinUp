using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VoisinUp.Models;
using VoisinUp.Services;

namespace VoisinUp.Controllers;

[ApiController]
[Route("/api/assets")]
public class AssetsController : Controller {
    private readonly AssetsService _assetsService;

    public AssetsController(AssetsService assetsService) {
        _assetsService = assetsService;
    }

    [Authorize]
    [HttpGet("get-all")]
    public async Task<IActionResult> GetAll() {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null) return Unauthorized();

        var result = await _assetsService.GetAll();

        return StatusCode(result.StatusCode);
    }
}