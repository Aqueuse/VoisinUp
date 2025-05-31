using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VoisinUp.Models;
using VoisinUp.Services;

namespace VoisinUp.Controllers;

[ApiController]
[Route("/api/quest")]
public class QuestController : Controller {
    private readonly QuestService _questService;
    
    public QuestController(QuestService questService) {
        _questService = questService;
    }
    
    [Authorize]
    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateQuest createQuest) {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null) return Unauthorized();
        
        var result = await _questService.CreateQuest(createQuest, userIdClaim);
        
        return StatusCode(result.StatusCode);
    }
    
    [Authorize]
    [HttpPost("update")]
    public async Task<IActionResult> Update([FromBody] UpdateQuest updatedQuest) {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null) return Unauthorized();
        
        var result = await _questService.UpdateQuest(updatedQuest, userIdClaim);
        
        return StatusCode(result.StatusCode);
    }

    [Authorize]
    [HttpPost("delete")]
    public async Task<IActionResult> Delete(string questId) {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null) return Unauthorized();

        var result = await _questService.DeleteQuest(questId, userIdClaim);

        return StatusCode(result.StatusCode);
    }
    
    [Authorize]
    [HttpPost("join")]
    public async Task<IActionResult> JoinQuest(string questId) {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null) return Unauthorized();

        var result = await _questService.JoinQuest(questId, userIdClaim);

        return StatusCode(result.StatusCode);
    }
    
    [Authorize]
    [HttpDelete("leave")]
    public async Task<IActionResult> LeaveQuest(string questId) {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null) return Unauthorized();

        var result = await _questService.LeaveQuest(questId, userIdClaim);
        
        return StatusCode(result.StatusCode);
    }
    
    [Authorize]
    [HttpPost("start")]
    public async Task<IActionResult> StartQuest([FromBody]UpdateQuest updateQuest) {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null) return Unauthorized();

        var result = await _questService.StartQuest(updateQuest.QuestId, userIdClaim);

        return StatusCode(result.StatusCode);
    }
    
    [Authorize]
    [HttpPost("complete")]
    public async Task<IActionResult> CompleteQuest([FromBody]UpdateQuest updateQuest) {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null) return Unauthorized();

        var result = await _questService.CompleteQuest(updateQuest.QuestId, userIdClaim);
        
        return StatusCode(result.StatusCode);
    }
    
    [Authorize]
    [HttpPost("update-owner")]
    public async Task<IActionResult> UpdateOwner([FromBody]UpdateQuest updateQuest) {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null) return Unauthorized();

        var result = await _questService.UpdateOwner(updateQuest.QuestId, userIdClaim);

        return StatusCode(result.StatusCode);
    }
    
    [Authorize]
    [HttpGet("get-by-id/{questId}")]
    public async Task<IActionResult> GetQuestByQuestId(string questId) {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null) return Unauthorized();

        var quest = await _questService.GetQuestByQuestId(questId);
        
        if (quest == null) return NotFound();

        return Ok(quest);
    }
    
    [Authorize]
    [HttpGet("get-by-voisinage")]
    public async Task<IActionResult> GetQuestsByVoisinageId() {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null) return Unauthorized();
        
        var quests = await _questService.GetQuestsByUserVoisinageId(userIdClaim);
        
        if (quests == null) return NotFound();

        Console.WriteLine(quests.Length);
        return Ok(quests);
    }
    
    [Authorize]
    [HttpGet("get-by-user")]
    public async Task<IActionResult> GetQuestsByUserId() {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null) return Unauthorized();
        
        var quests = await _questService.GetQuestsByUserId(userIdClaim);
        if (quests == null) return NotFound();
        
        return Ok(quests);
    }
}