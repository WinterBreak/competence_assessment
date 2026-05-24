using CompetenceAssessment.Domain.Assessment;

namespace CompetenceAssessment.Web.Assessment;

public class TaskGetRequest
{
    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 10;

    public int Type { get; set; } = (int)TaskType.None;
}