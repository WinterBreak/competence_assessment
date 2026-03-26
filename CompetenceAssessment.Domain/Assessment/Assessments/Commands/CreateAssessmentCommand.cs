using CompetenceAssessment.Domain.Assessment.DTO;

namespace CompetenceAssessment.Domain.Assessment;

public class CreateAssessmentCommand
{
    public int TemplateId { get; }

    public AssessmentType Type { get; }
    
    public int CandidateId { get; }
    
    public List<int> InspectorsIds { get; } = [];

    public CreateAssessmentCommand(int templateId, AssessmentType type, int candidateId, List<int> inspectorsIds)
    {
        TemplateId = templateId;
        Type = type;
        CandidateId = candidateId;
        InspectorsIds = inspectorsIds;
    }

    public Assessment Create()
    {
        var candidate = new AssessmentParticipant { Id = CandidateId };
        var template = new ITemplate { Id = TemplateId };
        var inspectors = new List<AssessmentParticipant>();
        InspectorsIds.ForEach(id => inspectors.Add(new AssessmentParticipant { Id = id }));
        
        return new Assessment(DateTime.UtcNow, null, candidate, Type, template, inspectors, false);
    }
}