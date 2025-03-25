using Microsoft.AspNetCore.Mvc;
using VoisinUp.Services;

namespace VoisinUp.Controllers;

public class VoisinageController : Controller {
    private readonly VoisinageService _voisinageService;

    public VoisinageController(VoisinageService voisinageService) {
        _voisinageService = voisinageService;
    }

    [HttpGet("{voisinageId}")]
    public async Task<IActionResult> GetVoisins(int voisinageId) {
        var voisins = await _voisinageService.GetVoisinsByIdAsync(voisinageId);
        
        return Ok(voisins);
    }
}