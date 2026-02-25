using CompetenceAssessment.Domain.Assessment;
using Microsoft.AspNetCore.Mvc;

namespace CompetenceAssessment.Web.Assessment;

/// <summary>
/// Категории
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
    /// Получение всех компетенций
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetCompetencesAsync(CancellationToken cancellationToken = default)
    {
        var query = new CompetenceQuery();
        var competencies = await _competenceService.GetCompetenciesAsync(query, cancellationToken);
        return Ok(competencies);
    }

    /// <summary>
    /// Создание компетенции
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateCompetenceAsync(CompetenceCreateRequest request
        , CancellationToken cancellationToken = default)
    {
        var command = new CreateCompetenceCommand(request.Name, request.Description);
        var errors = await _competenceService.CreateCompetenceAsync(command, cancellationToken);
        return errors.HasErrors
            ? BadRequest(errors)
            : Ok(errors);
    }

    /// <summary>
    /// Изменение компетенции
    /// </summary>
    [HttpPatch]
    public async Task<IActionResult> UpdateCompetenceAsync(CompetenceUpdateRequest request
        , CancellationToken cancellationToken = default)
    {
        var command = new UpdateCompetenceCommand(request.Id, request.Name, request.Description);
        var errors = await _competenceService.UpdateCompetenceAsync(command, cancellationToken);
        return errors.HasErrors
            ? BadRequest(errors)
            : Ok(errors);
    }

    /// <summary>
    /// Удаление компетенции
    /// </summary>
    [HttpDelete]
    public async Task<IActionResult> DeleteCompetenceAsync(CompetenceDeleteRequest request
        , CancellationToken cancellationToken = default)
    {
        var command = new DeleteCompetenceCommand(request.Id);
        var errors = await _competenceService.DeleteCompetenceAsync(command, cancellationToken);
        return errors.HasErrors
            ? BadRequest(errors)
            : Ok(errors);
    }
}