using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VoisinUp.Services;

namespace VoisinUp.Controllers;

[ApiController]
[Route("api/taverne")]
public class TaverneController : Controller {
    private readonly TaverneService _taverneService;

    public TaverneController(TaverneService taverneService) {
        _taverneService = taverneService;
    }

    [Authorize]
    [HttpGet("history")]
    public async Task<IActionResult> GetLastMessages() {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null) return Unauthorized();

        var result = await _taverneService.GetLastMessages();
        
        if (result.StatusCode == 404) return StatusCode(404);

        return Ok(result.Data);
    }
}