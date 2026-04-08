namespace CompetenceAssessment.Domain.Assessment;

public class TestTemplate : ITemplate
{

    public TestTemplate(int id, string name, TemplateType type, ScaleType scale, DateTime creationDate
        , CompetenceModel model, List<TemplateWeight> weights) 
        : base(id, name, type, scale, creationDate, model, weights)
    { }
}