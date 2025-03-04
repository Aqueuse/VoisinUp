using Microsoft.AspNetCore.Mvc;
using VoisinUp.Services;

namespace VoisinUp.Controllers;

public class VoisinageController : Controller {
    private readonly VoisinageService _voisinageService;

    public VoisinageController(VoisinageService voisinageService) {
        _voisinageService = voisinageService;
    }

    [HttpGet("{voisinageId}")]
    public async Task<IActionResult> GetVoisinage(int voisinageId) {
        var voisinage = await _voisinageService.GetVoisinageByIdAsync(voisinageId);
        
        if (voisinage == null) return NotFound();
        
        return Ok(voisinage);
    }
}