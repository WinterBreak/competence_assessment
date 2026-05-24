using CompetenceAssessment.Domain.Assessment.DTO;

namespace CompetenceAssessment.Domain.Assessment;

public class Assessment
{
    public int Id { get; set; }
    
    public AssessmentParticipant Candidate { get; set; }
    
    public DateTime StartDate { get; set; }
    
    public DateTime? EndDate { get; set; }
    
    public AssessmentType Type { get; set; }
    
    public ITemplate Template { get; set; }
    
    public List<AssessmentParticipant> Inspectors { get; set; }

    public List<AssessmentResult> Results { get; set; } = [];
    
    public AssessmentState State { get; set; }

    public string Comment { get; set; }
    
    public Assessment() {}

    public Assessment(int id, DateTime startDate, DateTime? endDate, AssessmentParticipant candidate
        , AssessmentType type, ITemplate template, List<AssessmentParticipant> inspectors
        , AssessmentState state, string comment, List<AssessmentResult> results = default)
    {
        Id = id;
        StartDate = startDate;
        EndDate = endDate;
        Candidate = candidate;
        Type = type;
        Template = template;
        Inspectors = inspectors;
        Results = results;
        State = state;
        Comment = comment;
    }
    
    public Assessment(DateTime startDate, DateTime? endDate, AssessmentParticipant candidate
        , AssessmentType type, ITemplate template, List<AssessmentParticipant> inspectors
        , AssessmentState state, List<AssessmentResult> results = default)
    {
        StartDate = startDate;
        EndDate = endDate;
        Candidate = candidate;
        Type = type;
        Template = template;
        Inspectors = inspectors;
        Results = results;
        State = state;
    }
}