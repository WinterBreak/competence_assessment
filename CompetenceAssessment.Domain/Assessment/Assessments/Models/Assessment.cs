using CompetenceAssessment.Domain.UserManagement;

namespace CompetenceAssessment.Domain.Assessment.Assessments.Models;

public class Assessment
{
    public int Id { get; set; }
    
    public User Candidate { get; set; }
    
    public DateTime StartDate { get; set; }
    
    public DateTime EndDate { get; set; }
    
    public AssessmentType Type { get; set; }
    
    public ITemplate Template { get; set; }
    
    public List<User> Inspectors { get; set; }
    
    public List<AssessmentResult> Results { get; set; }
    
    public Assessment() {}

    public Assessment(int id, DateTime startDate, DateTime endDate
        , AssessmentType type, ITemplate template, List<User> inspectors
        , List<AssessmentResult> results)
    {
        Id = id;
        StartDate = startDate;
        EndDate = endDate;
        Type = type;
        Template = template;
        Inspectors = inspectors;
        Results = results;
    }
}