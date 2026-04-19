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
    private readonly IAssessmentCalcService _calcService;
    private readonly IAssessmentAnalyticsService _analyticsService;

    public AssessmentController(IAssessmentService assessmentService
        , IAssessmentCalcService calcService
        , IAssessmentAnalyticsService analyticsService)
    {
        _assessmentService = assessmentService;
        _calcService = calcService;
        _analyticsService = analyticsService;
    }
    
    /// <summary>
    /// Получение оценок
    /// </summary>
    [Route("{id}")]
    [HttpGet]
    public async Task<IActionResult> GetAssessmentsAsync(int id, CancellationToken token = default)
    {
        var assessment = await _assessmentService.GetAssessmentAsync(id, token);
        var dto = new AssessmentDto(assessment);
        return Ok(dto);
    }
    
    /// Получение оценки по id
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAssessmentsAsync(CancellationToken token = default)
    {
        var query = new AssessmentQuery();
        var assessments = await _assessmentService.GetAssessmentsAsync(query, token);
        var dtos = assessments.Select(a => new AssessmentDto(a)).ToList();
        return Ok(dtos);
    }

    /// <summary>
    /// Создание оценки
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateAssessmentAsync(AssessmentCreateRequest request
                                                         , CancellationToken token = default)
    {
        var command = new CreateAssessmentCommand(request.TemplateId, (AssessmentType)request.Type
                                                , request.CandidateId, request.InspectorsIds);
        var errors = await _assessmentService.CreateAssessmentAsync(command, token);
        return Ok(errors); // TODO написать стандартные модели ответов: успех и ошибка
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

    /// <summary>
    /// Расчет оценки
    /// </summary>
    [Route("calculate")]
    [HttpPost]
    public async Task<IActionResult> CalculateAsync(AssessmentCalculateRequest request
                                                  , CancellationToken token = default)
    {
        var calc = await _calcService.CalculateAsync(request.Id, token);
        return Ok(calc);
    }
    
    /// <summary>
    /// Компетенции сотрудников
    /// </summary>
    [Route("participant_competencies")]
    [HttpGet]
    public async Task<IActionResult> GetParticipantCompetenciesAsync(CancellationToken token = default)
    {
        var competencies = await _analyticsService
            .GetParticipantsCompetenciesAsync(token);
        var dtos = competencies
            .Select(c => new ParticipantCompetenciesDto(c))
            .ToList(); 
        return Ok(dtos);
    }
}