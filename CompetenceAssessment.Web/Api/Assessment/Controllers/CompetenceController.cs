using CompetenceAssessment.Domain.Assessment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CompetenceAssessment.Web.Assessment;

/// <summary>
/// Компетенции
/// </summary>
[Route("api/competencies")]
[ApiController]
public class CompetenceController: ControllerBase
{
    private readonly ICompetenceService _competenceService;

    public CompetenceController(ICompetenceService competenceService)
    {
        _competenceService = competenceService;
    }

    /// <summary>
    /// Получение компетенций
    /// </summary>
    [HttpGet]
    [Authorize(Policy = "Authenticated")]
    public async Task<IActionResult> GetCompetencesAsync(CancellationToken token = default)
    {
        var query = new CompetenceQuery();
        var competencies = await _competenceService.GetCompetenciesAsync(query, token);
        return Ok(competencies);
    }

    /// <summary>
    /// Создание компетенции
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "Admin")]
    public async Task<IActionResult> CreateCompetenceAsync(CompetenceCreateRequest request
        , CancellationToken token = default)
    {
        var command = new CreateCompetenceCommand(request.Name, request.Description);
        var errors = await _competenceService.CreateCompetenceAsync(command, token);
        return errors.HasErrors
            ? BadRequest(errors)
            : Ok(errors);
    }

    /// <summary>
    /// Изменение компетенции
    /// </summary>
    [HttpPatch]
    [Authorize(Policy = "Admin")]
    public async Task<IActionResult> UpdateCompetenceAsync(CompetenceUpdateRequest request
        , CancellationToken token = default)
    {
        var command = new UpdateCompetenceCommand(request.Id, request.Name, request.Description);
        var errors = await _competenceService.UpdateCompetenceAsync(command, token);
        return errors.HasErrors
            ? BadRequest(errors)
            : Ok(errors);
    }

    /// <summary>
    /// Удаление компетенции
    /// </summary>
    [Route("{id}")]
    [HttpDelete]
    [Authorize(Policy = "Admin")]
    public async Task<IActionResult> DeleteCompetenceAsync(int id
        , CancellationToken token = default)
    {
        var command = new DeleteCompetenceCommand(id);
        var errors = await _competenceService.DeleteCompetenceAsync(command, token);
        return errors.HasErrors
            ? BadRequest(errors)
            : Ok(errors);
    }
}