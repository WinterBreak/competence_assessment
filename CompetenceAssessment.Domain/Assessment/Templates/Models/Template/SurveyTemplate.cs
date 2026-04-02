namespace CompetenceAssessment.Domain.Assessment;

public class SurveyTemplate : ITemplate
{
    public SurveyTemplate(int id, string name, TemplateType type
        , ScaleType scale, DateTime creationDate, int competenceModelId
        , List<TemplateWeight> weights) 
        : base(id, name, type, scale, creationDate, competenceModelId, weights)
    { }
}