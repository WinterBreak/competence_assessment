namespace CompetenceAssessment.Domain.Assessment;

public class TestTemplate : ITemplate
{

    public TestTemplate(int id, string name, TemplateType type, ScaleType scale, DateTime creationDate
        , int competenceModelId, List<TemplateWeight> weights) 
        : base(id, name, type, scale, creationDate, competenceModelId, weights)
    { }
}