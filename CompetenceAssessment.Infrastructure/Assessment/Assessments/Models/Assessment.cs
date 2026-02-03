namespace CompetenceAssessment.Infrastructure.Assessment;

public class Assessment
{
    public int Id { get; set; }
    
    public int AssessmentTypeId { get; set; }
    
    public int UserId { get; set; }
    
    public DateTime StartDate { get; set; }
    
    public DateTime? EndDate { get; set; }
    
    public string Comment { get; set; }
    
    public bool IsFinished { get; set; }

    public virtual ICollection<AssessmentInspector> Inspectors { get; set; } = [];
    
    public virtual ICollection<AssessmentResult> Results { get; set; } = [];

    public Assessment(int typeId, int userId, DateTime startDate, DateTime endDate
        , bool isFinished = false)
    {
        AssessmentTypeId = typeId;
        UserId = userId;
        StartDate = startDate;
        EndDate = endDate;
        IsFinished = isFinished;
    }
}