using CompetenceAssessment.Domain.Assessment.DTO;
using CompetenceAssessment.Domain.UserManagement;
using CompetenceAssessment.Web.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CompetenceAssessment.Web.UserManagement;

[ApiController]
[Route("api/users")]
public class UserController: ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }
    
    /// <summary>
    /// Получение пользователей
    /// </summary>
    [HttpGet]
    [Authorize(Policy = "Admin")]
    public async Task<IActionResult> GetUsersAsync(CancellationToken token = default)
    {
        var query = new UserQuery();
        var users = await _userService.GetUsersAsync(query, token);
        var dtos = users.Select(u => new UserDto(u)).ToList();
        return Ok(dtos);
    }
    
    /// <summary>
    /// Получение участников оценки
    /// </summary>
    [Route("assessment_participants")]
    [HttpGet]
    public async Task<IActionResult> GetAssessmentParticipantsAsync(CancellationToken token = default)
    {
        var query = new UserQuery();
        var users = await _userService.GetUsersAsync(query, token);
        var dtos = users.Select(u => 
            new AssessmentParticipant(u.Id, u.BossId, u.FullName)).ToList();
        return Ok(dtos);
    }
}