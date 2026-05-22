using CompetenceAssessment.Domain.UserManagement;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace CompetenceAssessment.Web.UserManagement;

[ApiController]
[Route("api/auth")]
public class AuthController: ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }
    
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new { message = "Неверный формат запроса", errors = ModelState });
        }

        try
        {
            var result = await _authService.LoginAsync(request.Email, request.Password, cancellationToken);

            if (!result.Succeeded)
            {
                return Unauthorized(new { message = result.ErrorMessage });
            }
            
            return Ok(new 
            { 
                userId = result.UserId, 
                email = result.Email,
                roles = result.Roles
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Внутренняя ошибка сервера" });
        }
    }
    
    [HttpPost("logout")]
    [AllowAnonymous]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        try
        {
            await _authService.LogoutAsync(cancellationToken);
            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Внутренняя ошибка сервера" });
        }
    }
    
    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetCurrentUser(CancellationToken cancellationToken)
    {
        try
        {
            var user = await _authService.GetCurrentUserAsync(cancellationToken);
            
            if (user == null)
            {
                return NotFound(new { message = "Пользователь не найден" });
            }

            return Ok(new
            {
                id = user.Id,
                email = user.Email,
                firstName = user.FirstName,
                secondName = user.SecondName,
                lastName = user.LastName,
                fullName = user.FullName,
                position = user.Position,
                department = user.Department,
                // roles = user.Roles
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Внутренняя ошибка сервера" });
        }
    }
}