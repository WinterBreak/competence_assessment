namespace CompetenceAssessment.Domain.Assessment;

public class SurveyTemplate : ITemplate
{
    public SurveyTemplate(int id, string name, TemplateType type
        , ScaleType scale, DateTime creationDate, CompetenceModel model
        , List<TemplateWeight> weights) 
        : base(id, name, type, scale, creationDate, model, weights)
    { }
}