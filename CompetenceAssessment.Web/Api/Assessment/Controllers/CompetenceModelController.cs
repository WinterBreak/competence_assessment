using CompetenceAssessment.Domain.Assessment;
using Microsoft.AspNetCore.Mvc;

namespace CompetenceAssessment.Web.Assessment;

/// <summary>
/// Модели компетенций
/// </summary>
[Route("api/competence_models")]
[ApiController]
public class CompetenceModelController: ControllerBase
{
    private readonly ICompetenceModelService _competenceService;

    public CompetenceModelController(ICompetenceModelService competenceService)
    {
        _competenceService = competenceService;
    }

    /// <summary>
    /// Получение моделей компетенций
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetCompetenceModelsAsync(CancellationToken token = default)
    {
        var query = new CompetenceModelQuery();
        var competencies = await _competenceService.GetCompetenceModelsAsync(query, token);
        return Ok(competencies);
    }

    /// <summary>
    /// Создание модели компетенций
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateCompetenceModelAsync(CompetenceModelCreateRequest request
        , CancellationToken token = default)
    {
        var command = new CreateCompetenceModelCommand(request.Name, request.Weights, request.Description);
        var errors = await _competenceService.CreateCompetenceModelAsync(command, token);
        return errors.HasErrors
            ? BadRequest(errors)
            : Ok(errors);
    }

    /// <summary>
    /// Изменение модели компетенций
    /// </summary>
    [HttpPatch]
    public async Task<IActionResult> UpdateCompetenceModelAsync(CompetenceModelUpdateRequest request
        , CancellationToken token = default)
    {
        var command = new UpdateCompetenceModelCommand(request.Id, request.Name, request.Description, request.Weights);
        var errors = await _competenceService.UpdateCompetenceModelAsync(command, token);
        return errors.HasErrors
            ? BadRequest(errors)
            : Ok(errors);
    }

    /// <summary>
    /// Удаление модели компетенций
    /// </summary>
    [HttpDelete]
    public async Task<IActionResult> DeleteCompetenceModelAsync(CompetenceModelDeleteRequest request
        , CancellationToken token = default)
    {
        var command = new DeleteCompetenceModelCommand(request.Id);
        var errors = await _competenceService.DeleteCompetenceModelAsync(command, token);
        return errors.HasErrors
            ? BadRequest(errors)
            : Ok(errors);
    }
}