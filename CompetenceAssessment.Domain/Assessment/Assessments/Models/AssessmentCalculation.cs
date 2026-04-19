namespace CompetenceAssessment.Domain.Assessment;

public class AssessmentCalculation
{
    public int AssessmentId { get; set; }
    
    public int CandidateId { get; set; }
    
    public decimal ReferenceTotal { get; set; }
    
    public decimal ReceivedTotal { get; set; }
    
    public decimal TotalReceivedPercentage { get; set; }

    public Dictionary<int, decimal> CompetenceReferences { get; set; } = new();
    
    public Dictionary<int, decimal> CompetenciesReceived { get; set; } = new();
    
    public Dictionary<int, decimal> CompetenciesReceivedPercentage { get; set; } = new();
    
    public Dictionary<int, string> CompetenceNames { get; set; } = new();
    
    public AssessmentCalculation() {}

    public AssessmentCalculation(Assessment assessment)
    {
        AssessmentId = assessment.Id;
        CandidateId = assessment.Candidate.Id;
        CompetenceNames = assessment.Template.Model.Competencies
            .ToDictionary(c => c.Competence.Id, c => c.Competence.Name);
    }
}