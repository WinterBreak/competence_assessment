namespace CompetenceAssessment.Domain.Assessment.DTO;

public class AssessmentParticipant
{
    public int Id { get; set; }
    
    public int? BossId { get; set; }
    
    public string FullName { get; set; }

    public AssessmentParticipant() {}
    
    public AssessmentParticipant(int id, int? bossId, string fullName)
    {
        Id = id;
        BossId = bossId;
        FullName = fullName;
    }
}