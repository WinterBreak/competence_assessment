using CompetenceAssessment.Domain.Assessment.DTO;

namespace CompetenceAssessment.Domain.Assessment;

public class AssessmentResultsByDateRange
{
    public AssessmentParticipant Participant { get; set; }
    
    public DateTime FinishDate { get; set; }
    
    public string AssessmentName { get; set; }
    
    public List<AssessmentParticipantCompetencies> Competencies { get; set; }
    
    public AssessmentResultsByDateRange() {}

    public AssessmentResultsByDateRange(DateTime finishDate, string assessmentName
        , List<AssessmentParticipantCompetencies> competencies)
    {
        FinishDate = finishDate;
        AssessmentName = assessmentName;
        Competencies = competencies;
    }
}