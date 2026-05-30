using System.ComponentModel.DataAnnotations;
using CompetenceAssessment.Domain.Export.Enumerations;

namespace CompetenceAssessment.Web.Assessment;

public class ExportReportRequest
{
    [Required(ErrorMessage = "Тип отчёта обязателен")]
    public int ReportType { get; set; }

    /// <summary>
    /// Идентификатор департамента. null — все департаменты.
    /// Используется для: EmployeeCompetenceHeatmap, OrganizationalMaturity
    /// </summary>
    public int? DepartmentId { get; set; }

    /// <summary>
    /// Идентификатор должности.
    /// Используется для: PositionMatrix
    /// </summary>
    public int? PositionId { get; set; }

    /// <summary>
    /// Идентификатор сотрудника.
    /// Используется для: CompetenceDevelopmentTracker
    /// </summary>
    public int? EmployeeId { get; set; }

    /// <summary>
    /// Сортировка: "fullName" или "avgScore".
    /// Используется для: EmployeeCompetenceHeatmap
    /// </summary>
    public string SortBy { get; set; } = "fullName";
}