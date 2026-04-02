using CompetenceAssessment.Domain.Assessment;
using Microsoft.AspNetCore.Mvc;

namespace CompetenceAssessment.Web.Assessment;

/// <summary>
/// Оценки
/// </summary>
[ApiController]
[Route("api/assessments")]
public class AssessmentController: ControllerBase
{
    private readonly IAssessmentService _assessmentService;

    public AssessmentController(IAssessmentService assessmentService)
    {
        _assessmentService = assessmentService;
    }
    
    /// <summary>
    /// Получение оценок
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAssessmentsAsync(CancellationToken token = default)
    {
        var query = new AssessmentQuery();
        var competencies = await _assessmentService.GetAssessmentsAsync(query, token);
        return Ok(competencies);
    }

    /// <summary>
    /// Создание оценки
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateAssessmentAsync(AssessmentCreateRequest request
                                                         , CancellationToken token = default)
    {
        var command = new CreateAssessmentCommand(request.TemplateId, (AssessmentType)request.AssessmentTypeId
                                                , request.CandidateId, request.InspectorsIds);
        var errors = await _assessmentService.CreateAssessmentAsync(command, token);
        return errors.HasErrors
            ? BadRequest(errors)
            : Ok(errors);
    }

    /// <summary>
    /// Изменение оценки
    /// </summary>
    [HttpPatch]
    public async Task<IActionResult> UpdateAssessmentAsync(AssessmentUpdateRequest request
                                                         , CancellationToken token = default)
    {
        var command = new UpdateAssessmentCommand(request.AssessmentId, request.Answers, request.Scores
                                                , request.Comments, request.Comment);
        var errors = await _assessmentService.UpdateAssessmentAsync(command, token);
        return errors.HasErrors
            ? BadRequest(errors)
            : Ok(errors);
    }
}