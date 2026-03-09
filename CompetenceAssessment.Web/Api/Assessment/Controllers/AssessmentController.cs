using CompetenceAssessment.Domain.Assessment;
using Microsoft.AspNetCore.Mvc;

namespace CompetenceAssessment.Web.Assessment;

/// <summary>
/// Шаблоны
/// </summary>
[ApiController]
[Route("api/templates")]
public class AssessmentController: ControllerBase
{
    private readonly ITemplateService _templateService;

    public AssessmentController(ITemplateService templateService)
    {
        _templateService = templateService;
    }
    
    /// <summary>
    /// Получение шаблонов
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetCompetenceModelsAsync(CancellationToken token = default)
    {
        var query = new TemplateQuery();
        var competencies = await _templateService.GetTemplatesAsync(query, token);
        return Ok(competencies);
    }

    /// <summary>
    /// Создание шаблона
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateCompetenceModelAsync(TemplateCreateRequest request
                                                              , CancellationToken token = default)
    {
        var command = new CreateTemplateCommand(request.Name, (TemplateType)request.Type
                                             , (ScaleType)request.Scale, request.CompetenceModelId
                                             , request.Weights);
        var errors = await _templateService.CreateTemplateAsync(command, token);
        return errors.HasErrors
            ? BadRequest(errors)
            : Ok(errors);
    }

    /// <summary>
    /// Изменение шаблона
    /// </summary>
    [HttpPatch]
    public async Task<IActionResult> UpdateCompetenceModelAsync(TemplateUpdateRequest request
                                                              , CancellationToken token = default)
    {
        var command = new UpdateTemplateCommand(request.Id, request.Name, (TemplateType)request.Type
                                             , (ScaleType)request.Scale, request.CompetenceModelId
                                             , request.Weights);
        var errors = await _templateService.UpdateTemplateAsync(command, token);
        return errors.HasErrors
            ? BadRequest(errors)
            : Ok(errors);
    }

    /// <summary>
    /// Удаление шаблона
    /// </summary>
    [HttpDelete]
    public async Task<IActionResult> DeleteCompetenceModelAsync(CompetenceModelDeleteRequest request
                                                              , CancellationToken token = default)
    {
        var command = new DeleteTemplateCommand(request.Id);
        var errors = await _templateService.DeleteTemplateAsync(command, token);
        return errors.HasErrors
            ? BadRequest(errors)
            : Ok(errors);
    }
}