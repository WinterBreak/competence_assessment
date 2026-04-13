namespace CompetenceAssessment.Web.Assessment;

public class AssessmentDto
{
    public int Id { get; set; }
    
    public Domain.Assessment.DTO.AssessmentParticipant Candidate { get; set; } // хорошая ли идея, именно эту модель использовать. мб лучше свою завести?
    
    public DateTime StartDate { get; set; }
    
    public DateTime? EndDate { get; set; }
    
    public int Type { get; set; }
    
    public AssessmentTemplateDto Template { get; set; }
    
    public List<Domain.Assessment.DTO.AssessmentParticipant> Inspectors { get; set; }
    
    public List<AssessmentResultDto> Results { get; set; }
    
    public bool IsFinished { get; set; }
    
    public AssessmentDto() {}

    public AssessmentDto(Domain.Assessment.Assessment assessment)
    {
        Id = assessment.Id;
        Candidate = assessment.Candidate;
        StartDate = assessment.StartDate;
        EndDate = assessment.EndDate;
        Type = (int)assessment.Type;
        Template = new AssessmentTemplateDto(assessment.Template);
        Inspectors = assessment.Inspectors;
        Results = assessment.Results?.Select(a => new AssessmentResultDto(a)).ToList();
        IsFinished = assessment.IsFinished;
    }
}