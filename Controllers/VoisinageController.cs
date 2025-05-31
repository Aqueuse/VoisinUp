using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VoisinUp.Services;

namespace VoisinUp.Controllers;

[ApiController]
[Route("/api/voisinage")]
public class VoisinageController : Controller {
    private readonly VoisinageService _voisinageService;

    public VoisinageController(VoisinageService voisinageService) {
        _voisinageService = voisinageService;
    }

    [Authorize]
    [HttpGet("get-by-voisinage-id")]
    public async Task<IActionResult> GetVoisins() {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null) return Unauthorized();
        
        var voisins = await _voisinageService.GetUserVoisinsAsync(userIdClaim);
        if (voisins == null) return NotFound();
        
        return Ok(voisins);
    }
}