using Microsoft.AspNetCore.Mvc;
using VoisinUp.Models;
using VoisinUp.Services;

namespace VoisinUp.Controllers;

/// <summary>
/// Gère la création et la suppression des utilisateurs et de leur grille.
/// </summary>
[ApiController]
[Route("api/user")]
public class UserController : Controller {
    private readonly UserService _userService;

    public UserController(UserService userService) {
        _userService = userService;
    }

    /// <summary>
    /// Crée un utilisateur et génère sa grille (100x100x5).
    /// </summary>
    /// <param name="user">Données de l'utilisateur.</param>
    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] User user) {
        await _userService.CreateUserAsync(
            user.Name, 
            user.Email, 
            user.Country, 
            user.Commune, 
            user.VoisinageId
        );
        
        return Ok(new { message = "Utilisateur et grille créés !" });
    }

    /// <summary>
    /// Supprime un utilisateur et sa grille.
    /// </summary>
    /// <param name="userId">ID de l'utilisateur à supprimer.</param>
    [HttpDelete("{userId}")]
    public async Task<IActionResult> DeleteUser(string userId) {
        await _userService.DeleteUserAsync(userId);
        return Ok(new { message = "Utilisateur et grille supprimés !" });
    }
}