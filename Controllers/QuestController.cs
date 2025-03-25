using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VoisinUp.Models;
using VoisinUp.Services;

namespace VoisinUp.Controllers;

[ApiController]
[Route("/api/quest")]
public class QuestController : Controller {
    private QuestService _questService;
    
    public QuestController(QuestService questService) {
        _questService = questService;
    }
    
    [Authorize]
    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateQuest createQuest) {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null) return Unauthorized();
        
        var result = await _questService.CreateQuest(createQuest, userIdClaim);
        
        return StatusCode(result.StatusCode, result.Message);
    }

    [Authorize]
    [HttpPost("delete")]
    public async Task<IActionResult> Delete(string questId) {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null) return Unauthorized();

        var result = await _questService.DeleteQuest(questId, userIdClaim);

        return StatusCode(result.StatusCode, result.Message);
    }
    
    [Authorize]
    [HttpPost("join")]
    public async Task<IActionResult> JoinQuest(string questId) {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userIdClaim == null) return Unauthorized();

        var result = await _questService.JoinQuest(questId, userIdClaim);

        return StatusCode(result.StatusCode, result.Message);
    }
    
    [Authorize]
    [HttpDelete("leave")]
    public async Task<IActionResult> LeaveQuest(string questId) {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userIdClaim == null) return Unauthorized();

        var result = await _questService.LeaveQuest(questId, userIdClaim);
        
        return StatusCode(result.StatusCode, result.Message);
    }
    
    [Authorize]
    [HttpPost("start")]
    public async Task<IActionResult> StartQuest([FromBody]UpdateQuest updateQuest) {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userIdClaim == null) return Unauthorized();

        var result = await _questService.StartQuest(updateQuest.QuestId, userIdClaim);

        return StatusCode(result.StatusCode, result.Message);
    }
    
    [Authorize]
    [HttpPost("complete")]
    public async Task<IActionResult> CompleteQuest([FromBody]UpdateQuest updateQuest) {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var result = await _questService.CompleteQuest(updateQuest.QuestId, userIdClaim);
        
        return StatusCode(result.StatusCode, result.Message);
    }
    
    [Authorize]
    [HttpPost("update-owner")]
    public async Task<IActionResult> UpdateOwner([FromBody]UpdateQuest updateQuest) {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var result = await _questService.UpdateOwner(updateQuest.QuestId, userIdClaim);

        return StatusCode(result.StatusCode, result.Message);
    }
    
    [Authorize]
    [HttpPost("get")]
    public async Task<IActionResult> GetQuestByQuestId(string questId) {
        var quest = await _questService.GetQuestByQuestId(questId);

        if (quest == null) return NotFound();
        
        return Ok(quest);
    }
    
    [Authorize]
    [HttpPost("by-voisinage")]
    public async Task<IActionResult> GetQuestsByVoisinageId(int voisinageId) {
        var quests = await _questService.GetQuestsByVoisinageId(voisinageId);

        return Ok(quests);
    }
    
    [Authorize]
    [HttpPost("by-user")]
    public async Task<IActionResult> GetQuestsByUserId(string userId) {
        var quests = await _questService.GetQuestsByUserId(userId);

        return Ok(quests);
    }
}