namespace CompetenceAssessment.Infrastructure.Assessment;

public class Assessment
{
    public int Id { get; set; }
    
    public int TemplateId { get; set; }
    
    public int AssessmentTypeId { get; set; }
    
    public int UserId { get; set; }
    
    public int? ParentId { get; set; }
    
    public DateTime StartDate { get; set; }
    
    public DateTime? EndDate { get; set; }
    
    public string? Comment { get; set; }
    
    public int State { get; set; }
    
    public virtual Template Template { get; set; }

    public virtual ICollection<AssessmentInspector> Inspectors { get; set; } = [];
    
    public virtual ICollection<AssessmentResult> Results { get; set; } = [];
    
    public virtual Assessment Parent { get; set; }

    public Assessment() {}
    
    public Assessment(int templateId, int typeId, int userId, DateTime startDate, DateTime? endDate
        , int state)
    {
        TemplateId = templateId;
        AssessmentTypeId = typeId;
        UserId = userId;
        ParentId = null;
        StartDate = startDate;
        EndDate = endDate;
        State = state;
    }
    
    public Assessment(int templateId, int typeId, int userId, Assessment parent, DateTime startDate, DateTime? endDate
        , int state)
    {
        TemplateId = templateId;
        AssessmentTypeId = typeId;
        UserId = userId;
        Parent = parent;
        StartDate = startDate;
        EndDate = endDate;
        State = state;
    }
}