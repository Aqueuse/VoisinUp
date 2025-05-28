using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VoisinUp.Services;

namespace VoisinUp.Controllers;

[ApiController]
[Route("/api/quest-category")]
public class QuestCategoryController : Controller {
    private readonly QuestCategoryService _questCategoryService;

    public QuestCategoryController(QuestCategoryService questCategoryService) {
        _questCategoryService = questCategoryService;
    }
    
    [Authorize]
    [HttpPost("get-categories")]
    public async Task<IActionResult> GetCategories() {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null) return Unauthorized();

        var resultat = await _questCategoryService.GetCategories();
        
        return StatusCode(resultat.StatusCode, resultat.Data);
    }
}