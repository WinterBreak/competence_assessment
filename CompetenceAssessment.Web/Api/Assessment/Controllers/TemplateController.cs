using CompetenceAssessment.Core.Models;
using CompetenceAssessment.Domain.Assessment;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize(Policy = "Authenticated")]
    public async Task<IActionResult> GetTemplatesAsync([FromQuery] TemplateGetRequest req
        , CancellationToken token = default)
    {
        var query = new TemplateQuery(page: req.Page, pageSize: req.PageSize);
        var templates = await _assessmentService.GetPaginatedTemplatesAsync(query, token);
        var dtos = templates.Items.Select(template => new TemplateDto(template)).ToList();
        
        var response = new PaginatedResponse<TemplateDto>()
        {
            PageSize = templates.PageSize,
            CurrentPage = templates.CurrentPage,
            TotalCount = templates.TotalCount,
            Items = dtos,
        };
        return Ok(response);
    }

    /// <summary>
    /// Создание шаблона
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "Admin")]
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
    [Authorize(Policy = "Admin")]
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
    [Route("{id}")]
    [HttpDelete]
    [Authorize(Policy = "Admin")]
    public async Task<IActionResult> DeleteTemplateAsync(int id, CancellationToken token = default)
    {
        var command = new DeleteTemplateCommand(id);
        var errors = await _assessmentService.DeleteTemplateAsync(command, token);
        return errors.HasErrors
            ? BadRequest(errors)
            : Ok(errors);
    }
}