using System.Security.Claims;
using CompetenceAssessment.Core.Extensions;
using CompetenceAssessment.Domain.Assessment;
using CompetenceAssessment.Domain.Export;
using CompetenceAssessment.Domain.Export.Enumerations;
using CompetenceAssessment.Domain.Export.Models;
using CompetenceAssessment.Domain.UserManagement.Enumerations;
using Microsoft.AspNetCore.Authorization;
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
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IReportExportService _exportService;

    public AssessmentController(IAssessmentService assessmentService
        , IAssessmentCalcService calcService
        , IAssessmentAnalyticsService analyticsService
        , IHttpContextAccessor httpContextAccessor
        , IReportExportService exportService)
    {
        _assessmentService = assessmentService;
        _calcService = calcService;
        _analyticsService = analyticsService;
        _httpContextAccessor = httpContextAccessor;
        _exportService = exportService;
    }
    
    /// <summary>
    /// Получение оценки по id
    /// </summary>
    [Route("{id}")]
    [HttpGet]
    [Authorize(Policy = "Authenticated")]
    public async Task<IActionResult> GetAssessmentsAsync(int id, CancellationToken token = default)
    {
        var assessment = await _assessmentService.GetAssessmentAsync(id, token);
        var dto = new AssessmentDto(assessment);
        return Ok(dto);
    }
    
    /// Получение оценок
    /// </summary>
    [HttpGet]
    [Authorize(Policy = "Authenticated")]
    public async Task<IActionResult> GetAssessmentsAsync(CancellationToken token = default)
    {
        var query = IsAdmin()
            ? new AssessmentQuery()
            : new AssessmentQuery(candidateIds: new List<int> { GetCurrUserId() }, isFinished: false);
        
        var assessments = await _assessmentService.GetAssessmentsAsync(query, token);
        var dtos = assessments.Select(a => new AssessmentDto(a)).ToList();
        return Ok(dtos);
    }

    /// Получение оценок
    /// </summary>
    [HttpGet]
    [Authorize(Policy = "Candidate")]
    [Route("history")]

    public async Task<IActionResult> GetAssessmentHistoryAsync(CancellationToken token = default)
    {
        var query = new AssessmentQuery(candidateIds: new List<int> { GetCurrUserId() }, isFinished: true);
        var assessments = await _assessmentService.GetAssessmentsAsync(query, token);
        var dtos = assessments.Select(a => new AssessmentDto(a)).ToList();
        return Ok(dtos);
    }

    /// <summary>
    /// Создание оценки
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "Admin")]
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
    [Authorize(Policy = "Authenticated")]
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
    [Authorize(Policy = "Authenticated")]
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
    [Authorize(Policy = "Admin")]
    public async Task<IActionResult> GetParticipantCompetenciesAsync(CancellationToken token = default)
    {
        var competencies = await _analyticsService
            .GetParticipantsCompetenciesAsync(token);
        var dtos = competencies
            .Select(c => new ParticipantCompetenciesDto(c))
            .ToList(); 
        return Ok(dtos);
    }
    
    /// <summary>
    /// Компетенции по должностям
    /// </summary>
    [Route("position_competencies")]
    [HttpGet]
    [Authorize(Policy = "Admin")]
    public async Task<IActionResult> GetPositionsCompetenciesAsync(CancellationToken token = default)
    {
        var competencies = await _analyticsService.GetPositionsCompetenciesAsync(token);
        var dtos = competencies
            .Select(c => new PositionsCompetenciesDto(c))
            .ToList(); 
        return Ok(dtos);
    }
    
    /// <summary>
    /// Компетенции по департаменту
    /// </summary>
    [Route("department_competencies")]
    [HttpGet]
    [Authorize(Policy = "Admin")]
    public async Task<IActionResult> GetDepartmentsCompetenciesAsync(CancellationToken token = default)
    {
        var competencies = await _analyticsService.GetDepartmentCompetenciesAsync(token);
        var dtos = competencies
            .Select(c => new DepartmentsCompetenciesDto(c))
            .ToList(); 
        return Ok(dtos);
    }
    
    /// <summary>
    /// Получение истории развития компетенции сотрудника
    /// </summary>
    [Route("competence_development/{employeeId}")]
    [HttpGet]
    [Authorize(Policy = "Candidate")]
    public async Task<IActionResult> GetCompetenceDevelopmentAsync(int employeeId, CancellationToken token = default)
    {
        var data = await _analyticsService.GetCompetenceDevelopmentAsync(employeeId, token);
        var dto = new CompetenceDevelopmentDto(data);
        return Ok(dto);
    }
    
    [HttpPost("export")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ExportReport(
        [FromBody] ExportReportRequest request,
        CancellationToken token = default)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            ReportRequest reportRequest = request.ReportType switch
            {
                ReportType.EmployeeCompetenceHeatmap => new HeatmapReportRequest
                {
                    DepartmentId = request.DepartmentId,
                    SortBy = request.SortBy
                },
                ReportType.PositionMatrix => new PositionMatrixReportRequest
                {
                    PositionId = request.PositionId
                },
                ReportType.OrganizationalMaturity => new OrganizationalMaturityReportRequest
                {
                    DepartmentId = request.DepartmentId
                },
                _ => throw new ArgumentOutOfRangeException(
                    nameof(request.ReportType), 
                    $"Неподдерживаемый тип отчёта: {request.ReportType}")
            };

            byte[] fileBytes = await _exportService.GenerateReportAsync(reportRequest, token);

            string fileName = request.ReportType switch
            {
                ReportType.EmployeeCompetenceHeatmap => "Тепловая_карта_компетенций.xlsx",
                ReportType.PositionMatrix => "Матрица_должностей.xlsx",
                ReportType.OrganizationalMaturity => "Матрица_зрелости.xlsx",
                _ => "Отчёт.xlsx"
            };

            return File(
                fileContents: fileBytes,
                contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileDownloadName: fileName
            );
        }
        catch (NotSupportedException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Внутренняя ошибка при формировании отчёта" });
        }
    }

    private bool IsAdmin()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null) throw new ArgumentNullException(nameof(httpContext));

        var userRoles = httpContext.User.Claims
            .Where(c => c.Type == ClaimTypes.Role)
            .Select(c => c.Value);
        
        return userRoles.Any(r => r == RolesEnum.Admin.GetDescription());
    }

    private int GetCurrUserId()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null) throw new ArgumentNullException(nameof(httpContext));
        var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (int.TryParse(userIdClaim, out int userId))
        {
            return userId;
        }
        else
        {
            throw new ArgumentNullException(nameof(userId));
        }
    }
}