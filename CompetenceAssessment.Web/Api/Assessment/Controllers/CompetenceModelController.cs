using CompetenceAssessment.Domain.Assessment;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize(Policy = "Authenticated")]
    public async Task<IActionResult> GetCompetenceModelsAsync([FromQuery] CompetenceModelGetRequest req
        , CancellationToken token = default)
    {
        var query = new CompetenceModelQuery(pageSize: req.PageSize, pageNumber: req.Page);
        var models = await _competenceService.GetPaginatedModelsAsync(query, token);
        return Ok(models);
    }

    /// <summary>
    /// Создание модели компетенций
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "Admin")]
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
    [Authorize(Policy = "Admin")]
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
    [Route("{id}")]
    [HttpDelete]
    [Authorize(Policy = "Admin")]
    public async Task<IActionResult> DeleteCompetenceModelAsync(int id, CancellationToken token = default)
    {
        var command = new DeleteCompetenceModelCommand(id);
        var errors = await _competenceService.DeleteCompetenceModelAsync(command, token);
        return errors.HasErrors
            ? BadRequest(errors)
            : Ok(errors);
    }
}