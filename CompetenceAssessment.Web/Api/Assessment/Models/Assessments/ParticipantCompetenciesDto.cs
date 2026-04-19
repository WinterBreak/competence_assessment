using CompetenceAssessment.Domain.Assessment;

namespace CompetenceAssessment.Web.Assessment;

public class ParticipantCompetenciesDto
{
    public int UserId { get; set; }
    
    public string FullName { get; set; }
    
    public string Position { get; set; }
    
    public string Department { get; set; }
    
    public List<AssessmentCompetenceDto> Competencies { get; set; }
    
    public ParticipantCompetenciesDto() {}

    public ParticipantCompetenciesDto(AssessmentParticipantCompetencies competencies)
    {
        UserId = competencies.User.Id;
        FullName = competencies.User.FullName;
        Position = competencies.User.Position;
        Department = competencies.User.Department;
        Competencies = competencies.ParticipantCompetences
            .Select(x => new AssessmentCompetenceDto(x)).ToList();
    }
}