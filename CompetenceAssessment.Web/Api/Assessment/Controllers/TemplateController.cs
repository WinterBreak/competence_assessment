using CompetenceAssessment.Domain.Assessment;
using Microsoft.AspNetCore.Mvc;

namespace CompetenceAssessment.Web.Assessment;

/// <summary>
/// Шаблоны
/// </summary>
[ApiController]
[Route("api/templates")]
public class TemplateController: ControllerBase
{
    private readonly ITemplateService _assessmentService;

    public TemplateController(ITemplateService assessmentService)
    {
        _assessmentService = assessmentService;
    }
    
    /// <summary>
    /// Получение шаблонов
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetTemplatesAsync(CancellationToken token = default)
    {
        var query = new TemplateQuery();
        var competencies = await _assessmentService.GetTemplatesAsync(query, token);
        return Ok(competencies);
    }

    /// <summary>
    /// Создание шаблона
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateTemplatesAsync(TemplateCreateRequest request
                                                         , CancellationToken token = default)
    {
        var command = new CreateTemplateCommand(request.Name, (TemplateType)request.Type, (ScaleType)request.Scale
                                              , request.CompetenceModelId, request.Weights, request.CompetenciesToTasks);
        var errors = await _assessmentService.CreateTemplateAsync(command, token);
        return errors.HasErrors
            ? BadRequest(errors)
            : Ok(errors);
    }

    /// <summary>
    /// Изменение шаблона
    /// </summary>
    [HttpPatch]
    public async Task<IActionResult> UpdateTemplateAsync(TemplateUpdateRequest request
                                                         , CancellationToken token = default)
    {
        var command = new UpdateTemplateCommand(request.Id, request.Name, (TemplateType)request.Type
                                             , (ScaleType)request.Scale, request.CompetenceModelId, request.Weights
                                             , request.CompetenciesToTasks);
        var errors = await _assessmentService.UpdateTemplateAsync(command, token);
        return errors.HasErrors
            ? BadRequest(errors)
            : Ok(errors);
    }
    
    /// <summary>
    /// Удаление шаблона
    /// </summary>
    [HttpDelete]
    public async Task<IActionResult> DeleteTemplateAsync(TemplateDeleteRequest request
        , CancellationToken token = default)
    {
        var command = new DeleteTemplateCommand(request.Id);
        var errors = await _assessmentService.DeleteTemplateAsync(command, token);
        return errors.HasErrors
            ? BadRequest(errors)
            : Ok(errors);
    }
}